using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;
using WaterAllocationRecordType = WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import.WaterAllocationRecordType;

namespace WesternStatesWater.WaDE.Accessors.Tests
{
    [TestClass]
    public class ImportWaterAllocationAccessorTests : DbTestBase
    {
        private readonly ILoggerFactory LoggerFactory = new LoggerFactory();

        [TestMethod]
        public async Task LoadWaterAllocation_SimpleLoad()
        {
            OrganizationsDim organization;
            VariablesDim variable;
            WaterSourcesDim waterSource;
            MethodsDim method;
            DateDim dataPublicationDate;
            DateDim allocationPriorityDate;
            WaterAllocation waterAllocation;
            string startTestString = "01/01";
            string endTestString = "12/01";

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                waterSource = await WaterSourcesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                dataPublicationDate = await DateDimBuilder.Load(db);
                allocationPriorityDate = await DateDimBuilder.Load(db);

                waterAllocation = WaterAllocationBuilder.Create(new WaterAllocationBuilderOptions { RecordType = WaterAllocationRecordType.None });

                waterAllocation.OrganizationUUID = organization.OrganizationUuid;
                waterAllocation.VariableSpecificUUID = variable.VariableSpecificUuid;
                waterAllocation.WaterSourceUUID = waterSource.WaterSourceUuid;
                waterAllocation.MethodUUID = method.MethodUuid;
                waterAllocation.DataPublicationDate = dataPublicationDate.Date;
                waterAllocation.AllocationPriorityDate = allocationPriorityDate.Date;
                waterAllocation.AllocationTimeframeStart = startTestString;
                waterAllocation.AllocationTimeframeEnd = endTestString;
            }

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.LoadWaterAllocation((new Faker()).Random.AlphaNumeric(10), new[] { waterAllocation });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbAllocationAmount = await db.AllocationAmountsFact.SingleAsync();

                dbAllocationAmount.AllocationAmountId.Should().NotBe(0);
                dbAllocationAmount.OrganizationId.Should().Be(organization.OrganizationId);
                dbAllocationAmount.VariableSpecificId.Should().Be(variable.VariableSpecificId);
                dbAllocationAmount.WaterSourceId.Should().Be(waterSource.WaterSourceId);
                dbAllocationAmount.MethodId.Should().Be(method.MethodId);
                dbAllocationAmount.DataPublicationDateId.Should().Be(dataPublicationDate.DateId);
                dbAllocationAmount.AllocationPriorityDateID.Should().Be(allocationPriorityDate.DateId);
                dbAllocationAmount.AllocationTimeframeStart.Should().Be(startTestString);
                dbAllocationAmount.AllocationTimeframeEnd.Should().Be(endTestString);
                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadWaterAllocation_BadLoad_CivilianAndPower()
        {
            OrganizationsDim organization;
            VariablesDim variable;
            WaterSourcesDim waterSource;
            MethodsDim method;
            DateDim dataPublicationDate;
            DateDim allocationPriorityDate;
            WaterAllocation waterAllocation;
            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                waterSource = await WaterSourcesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                dataPublicationDate = await DateDimBuilder.Load(db);
                allocationPriorityDate = await DateDimBuilder.Load(db);

                waterAllocation = WaterAllocationBuilder.Create(new WaterAllocationBuilderOptions { RecordType = WaterAllocationRecordType.Civilian });

                waterAllocation.OrganizationUUID = organization.OrganizationUuid;
                waterAllocation.VariableSpecificUUID = variable.VariableSpecificUuid;
                waterAllocation.WaterSourceUUID = waterSource.WaterSourceUuid;
                waterAllocation.MethodUUID = method.MethodUuid;
                waterAllocation.DataPublicationDate = dataPublicationDate.Date;
                waterAllocation.AllocationPriorityDate = allocationPriorityDate.Date;

                waterAllocation.GeneratedPowerCapacityMW = (new Faker()).Random.Decimal(0, 1000).ToString();
            }

            var sut = CreateWaterAllocationAccessor();
            var runId = (new Faker()).Random.AlphaNumeric(10);
            var result = await sut.LoadWaterAllocation(runId, new[] { waterAllocation });

            result.Should().BeFalse();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                db.AllocationAmountsFact.Should().HaveCount(0);

                var importError = db.ImportErrors.Single();
                importError.RunId.Should().Be(runId);
                importError.Data.Should().Contain("Cross Group Not Valid");
            }
        }

        [TestMethod]
        [DynamicData(nameof(ExemptOfPriorityFlowTestData))]
        public async Task LoadWaterAllocation_ExemptOfPriorityFlow(bool exemptOfPriorityFlow, DateTime? priorityDate, string allocationFlow_CFS, string allocationVolume_AF)
        {
            OrganizationsDim organization;
            VariablesDim variable;
            WaterSourcesDim waterSource;
            MethodsDim method;
            DateDim dataPublicationDate;
            WaterAllocation waterAllocation;
            string startTestString = "01/01";
            string endTestString = "12/01";

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                waterSource = await WaterSourcesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                dataPublicationDate = await DateDimBuilder.Load(db);

                waterAllocation = WaterAllocationBuilder.Create(new WaterAllocationBuilderOptions { RecordType = WaterAllocationRecordType.None });

                //////////////////// Exempt of Volume Flow Priority Columns
                // When exempt is TRUE priority date, allocation flow or allocation volume can be NULL.
                waterAllocation.ExemptOfVolumeFlowPriority = exemptOfPriorityFlow;
                waterAllocation.AllocationPriorityDate = priorityDate;
                waterAllocation.AllocationFlow_CFS = allocationFlow_CFS;
                waterAllocation.AllocationVolume_AF = allocationVolume_AF;
                ////////////////////

                waterAllocation.OrganizationUUID = organization.OrganizationUuid;
                waterAllocation.VariableSpecificUUID = variable.VariableSpecificUuid;
                waterAllocation.WaterSourceUUID = waterSource.WaterSourceUuid;
                waterAllocation.MethodUUID = method.MethodUuid;
                waterAllocation.DataPublicationDate = dataPublicationDate.Date;
                waterAllocation.AllocationTimeframeStart = startTestString;
                waterAllocation.AllocationTimeframeEnd = endTestString;
            }

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.LoadWaterAllocation((new Faker()).Random.AlphaNumeric(10), new[] { waterAllocation });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbAllocationAmount = await db.AllocationAmountsFact.SingleAsync();

                dbAllocationAmount.AllocationAmountId.Should().NotBe(0);
                dbAllocationAmount.ExemptOfVolumeFlowPriority.GetValueOrDefault(false).Should().Be(exemptOfPriorityFlow);
                dbAllocationAmount.AllocationFlow_CFS.ToString().Should().Be(allocationFlow_CFS ?? "");
                dbAllocationAmount.AllocationVolume_AF.ToString().Should().Be(allocationVolume_AF ?? "");

                if (priorityDate.HasValue)
                {
                    dbAllocationAmount.AllocationPriorityDateID.Should().BeGreaterThan(0);
                }
                else
                {
                    dbAllocationAmount.AllocationPriorityDateID.Should().BeNull();
                }

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        [DynamicData(nameof(ExemptOfPriorityFlowBadTestData))]
        public async Task LoadWaterAllocation_ExemptOfPriorityFlow_BadData(bool exemptOfPriorityFlow, DateTime? priorityDate, string allocationFlow_CFS, string allocationVolume_AF)
        {
            OrganizationsDim organization;
            VariablesDim variable;
            WaterSourcesDim waterSource;
            MethodsDim method;
            DateDim dataPublicationDate;
            WaterAllocation waterAllocation;
            string startTestString = "01/01";
            string endTestString = "12/01";
            int errorCount = 0;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                errorCount = db.ImportErrors.Count();

                organization = await OrganizationsDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                waterSource = await WaterSourcesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                dataPublicationDate = await DateDimBuilder.Load(db);

                waterAllocation = WaterAllocationBuilder.Create(new WaterAllocationBuilderOptions { RecordType = WaterAllocationRecordType.None });

                //////////////////// Exempt of Volume Flow Priority Columns
                waterAllocation.ExemptOfVolumeFlowPriority = exemptOfPriorityFlow;
                waterAllocation.AllocationPriorityDate = priorityDate;
                waterAllocation.AllocationFlow_CFS = allocationFlow_CFS;
                waterAllocation.AllocationVolume_AF = allocationVolume_AF;
                ////////////////////

                waterAllocation.OrganizationUUID = organization.OrganizationUuid;
                waterAllocation.VariableSpecificUUID = variable.VariableSpecificUuid;
                waterAllocation.WaterSourceUUID = waterSource.WaterSourceUuid;
                waterAllocation.MethodUUID = method.MethodUuid;
                waterAllocation.DataPublicationDate = dataPublicationDate.Date;
                waterAllocation.AllocationTimeframeStart = startTestString;
                waterAllocation.AllocationTimeframeEnd = endTestString;
            }

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.LoadWaterAllocation((new Faker()).Random.AlphaNumeric(10), new[] { waterAllocation });

            result.Should().BeFalse();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                db.ImportErrors.Should().HaveCount(errorCount + 1);
                var error = await db.ImportErrors.LastAsync();
                error.Data.Should().Contain("Allocation Not Exempt of Volume Flow Priority");
                
            }
        }

        protected static IEnumerable<object[]> ExemptOfPriorityFlowTestData
        {
            get
            {
                return new[]
                {
                    // { Exempt, PriorityDate, AllocationFlow_CFS, AllocationVolume_AF }
                    new object[] { true, null, null, null },
                    new object[] { true, DateTime.Now, null, null },
                    new object[] { true, null, "1000.75", null },
                    new object[] { true, null, null, "30.123456"},
                    new object[] { true, null, "1000.75", "30.123456"},
                    new object[] { true, DateTime.Now, "1000.75", null },
                    new object[] { true, DateTime.Now, "1000.75", "30.123456"},
                    new object[] { true, null, "1000.75", "30.123456"},
                    new object[] { true, DateTime.Now, null, "30.123456"},
                    new object[] { false, DateTime.Now, "1000.75", "30.123456"},
                    new object[] { null, DateTime.Now, "1000.75", "30.123456"},
                };
            }
        }

        protected static IEnumerable<object[]> ExemptOfPriorityFlowBadTestData
        {
            get
            {
                return new[]
                {
                    // Exempt is false or NULL, all fields are required
                    // { Exempt, PriorityDate, AllocationFlow_CFS, AllocationVolume_AF }
                    new object[] { false, null, null, null },
                    new object[] { false, DateTime.Now, null, null },
                    new object[] { false, null, "1000.75", null },
                    new object[] { false, null, null, "30.123456"},
                    new object[] { false, null, "1000.75", "30.123456"},
                    new object[] { false, DateTime.Now, "1000.75", null },
                    new object[] { false, null, "1000.75", "30.123456"},
                    new object[] { false, DateTime.Now, null, "30.123456"},

                    new object[] { null, null, null, null },
                    new object[] { null, DateTime.Now, null, null },
                    new object[] { null, null, "1000.75", null },
                    new object[] { null, null, null, "30.123456"},
                    new object[] { null, null, "1000.75", "30.123456"},
                    new object[] { null, DateTime.Now, "1000.75", null },
                    new object[] { null, null, "1000.75", "30.123456"},
                    new object[] { null, DateTime.Now, null, "30.123456"},
                };
            }
        }

        private IWaterAllocationAccessor CreateWaterAllocationAccessor()
        {
            return new WaterAllocationAccessor(Configuration.GetConfiguration(), LoggerFactory);
        }
    }
}
