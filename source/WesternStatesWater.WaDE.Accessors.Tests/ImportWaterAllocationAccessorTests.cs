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
        public async Task LoadWaterAllocation_LoadAllocationBridge_PrimaryUseCategoryCV_Null()
        {
            OrganizationsDim organization;
            VariablesDim variable;
            WaterSourcesDim waterSource;
            MethodsDim method;
            DateDim dataPublicationDate;
            DateDim allocationPriorityDate;
            WaterAllocation waterAllocation;
            BeneficialUsesCV beneficialUsesCV;
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
                beneficialUsesCV = await BeneficalUsesBuilder.Load(db);

                waterAllocation = WaterAllocationBuilder.Create(new WaterAllocationBuilderOptions { RecordType = WaterAllocationRecordType.None });

                waterAllocation.PrimaryUseCategory = null;

                waterAllocation.BeneficialUseCategory = beneficialUsesCV.Name;
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
                dbAllocationAmount.Should().NotBeNull();
                dbAllocationAmount.PrimaryUseCategoryCV.Should().BeNull();

                var dbAllocationBridge = await db.AllocationBridgeBeneficialUsesFact.SingleAsync();
                dbAllocationBridge.Should().NotBeNull();

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadWaterAllocation_LoadAggBridge_PrimaryUseCategoryCV_Null()
        {
            OrganizationsDim organization;
            ReportingUnitsDim reportingUnit;
            VariablesDim variable;
            WaterSourcesDim waterSource;
            MethodsDim method;
            DateDim dataPublicationDate;
            DateDim allocationPriorityDate;
            AggregatedAmount aggregateAmount;
            BeneficialUsesCV beneficialUsesCV;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                reportingUnit = await ReportingUnitsDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                waterSource = await WaterSourcesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                dataPublicationDate = await DateDimBuilder.Load(db);
                allocationPriorityDate = await DateDimBuilder.Load(db);
                beneficialUsesCV = await BeneficalUsesBuilder.Load(db);

                aggregateAmount = AggregatedAmountBuilder.Create();

                aggregateAmount.PrimaryUseCategory = null;
                
                aggregateAmount.ReportingUnitUUID = reportingUnit.ReportingUnitUuid;
                aggregateAmount.BeneficialUseCategory = beneficialUsesCV.Name;
                aggregateAmount.OrganizationUUID = organization.OrganizationUuid;
                aggregateAmount.VariableSpecificUUID = variable.VariableSpecificUuid;
                aggregateAmount.WaterSourceUUID = waterSource.WaterSourceUuid;
                aggregateAmount.MethodUUID = method.MethodUuid;
                aggregateAmount.DataPublicationDate = dataPublicationDate.Date;
            }

            var sut = CreateWaterAllocationAccessor();
            
            var result = await sut.LoadAggregatedAmounts((new Faker()).Random.AlphaNumeric(10), new[] { aggregateAmount });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbAggregateAmount = await db.AggregatedAmountsFact.SingleAsync();
                dbAggregateAmount.Should().NotBeNull();
                dbAggregateAmount.PrimaryUseCategoryCV.Should().BeNull();

                var dbAllocationBridge = await db.AggBridgeBeneficialUsesFact.SingleAsync();
                dbAllocationBridge.Should().NotBeNull();
                
                db.ImportErrors.Should().HaveCount(0);
            }
        }

        private IWaterAllocationAccessor CreateWaterAllocationAccessor()
        {
            return new WaterAllocationAccessor(Configuration.GetConfiguration(), LoggerFactory);
        }
    }
}
