﻿using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;
using WaterAllocationRecordType = WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import.WaterAllocationRecordType;

namespace WesternStatesWater.WaDE.Accessors.Tests
{
    [TestClass]
    public class ImportDataIngestionAccessorTests : DbTestBase
    {
        private readonly ILoggerFactory LoggerFactory = new LoggerFactory();

        [TestMethod]
        public async Task LoadWaterAllocation_SimpleLoad()
        {
            OrganizationsDim organization;
            VariablesDim variable;
            MethodsDim method;
            DateDim dataPublicationDate;
            DateDim allocationPriorityDate;
            WaterAllocation waterAllocation;
            string startTestString = "01/01";
            string endTestString = "12/01";
            BeneficialUsesCV primaryUseCategoryCV;
            OwnerClassificationCv ownerClassificationCV;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                dataPublicationDate = await DateDimBuilder.Load(db);
                allocationPriorityDate = await DateDimBuilder.Load(db);
                primaryUseCategoryCV = await BeneficalUsesBuilder.Load(db);
                ownerClassificationCV = await OwnerClassificationBuilder.Load(db);

                waterAllocation = WaterAllocationBuilder.Create(new WaterAllocationBuilderOptions { RecordType = WaterAllocationRecordType.None });

                waterAllocation.OrganizationUUID = organization.OrganizationUuid;
                waterAllocation.VariableSpecificUUID = variable.VariableSpecificUuid;
                waterAllocation.MethodUUID = method.MethodUuid;
                waterAllocation.DataPublicationDate = dataPublicationDate.Date;
                waterAllocation.AllocationPriorityDate = allocationPriorityDate.Date;
                waterAllocation.AllocationTimeframeStart = startTestString;
                waterAllocation.AllocationTimeframeEnd = endTestString;
                waterAllocation.PrimaryUseCategory = primaryUseCategoryCV.Name;
                waterAllocation.OwnerClassificationCV = ownerClassificationCV.Name;
            }

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadWaterAllocation((new Faker()).Random.AlphaNumeric(10), new[] { waterAllocation });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbAllocationAmount = await db.AllocationAmountsFact.SingleAsync();

                dbAllocationAmount.AllocationAmountId.Should().NotBe(0);
                dbAllocationAmount.OrganizationId.Should().Be(organization.OrganizationId);
                dbAllocationAmount.VariableSpecificId.Should().Be(variable.VariableSpecificId);
                dbAllocationAmount.MethodId.Should().Be(method.MethodId);
                dbAllocationAmount.DataPublicationDateId.Should().Be(dataPublicationDate.DateId);
                dbAllocationAmount.AllocationPriorityDateID.Should().Be(allocationPriorityDate.DateId);
                dbAllocationAmount.AllocationTimeframeStart.Should().Be(startTestString);
                dbAllocationAmount.AllocationTimeframeEnd.Should().Be(endTestString);
                dbAllocationAmount.PrimaryUseCategoryCV.Should().Be(primaryUseCategoryCV.Name);
                dbAllocationAmount.OwnerClassificationCV.Should().Be(ownerClassificationCV.Name);
                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadWaterAllocation_SimpleLoad_InvalidPrimaryUseCategory_ThrowsError()
        {
            OrganizationsDim organization;
            VariablesDim variable;
            MethodsDim method;
            DateDim dataPublicationDate;
            DateDim allocationPriorityDate;
            WaterAllocation waterAllocation;
            string startTestString = "01/01";
            string endTestString = "12/01";
            OwnerClassificationCv ownerClassificationCV;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                dataPublicationDate = await DateDimBuilder.Load(db);
                allocationPriorityDate = await DateDimBuilder.Load(db);
                ownerClassificationCV = await OwnerClassificationBuilder.Load(db);

                waterAllocation = WaterAllocationBuilder.Create(new WaterAllocationBuilderOptions { RecordType = WaterAllocationRecordType.None });

                waterAllocation.OrganizationUUID = organization.OrganizationUuid;
                waterAllocation.VariableSpecificUUID = variable.VariableSpecificUuid;
                waterAllocation.MethodUUID = method.MethodUuid;
                waterAllocation.DataPublicationDate = dataPublicationDate.Date;
                waterAllocation.AllocationPriorityDate = allocationPriorityDate.Date;
                waterAllocation.AllocationTimeframeStart = startTestString;
                waterAllocation.AllocationTimeframeEnd = endTestString;
                waterAllocation.PrimaryUseCategory = Guid.NewGuid().ToString();
                waterAllocation.OwnerClassificationCV = ownerClassificationCV.Name;
            }

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadWaterAllocation((new Faker()).Random.AlphaNumeric(10), new[] { waterAllocation });

            result.Should().BeFalse();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbAllocationAmount = await db.AllocationAmountsFact.FirstOrDefaultAsync();
                dbAllocationAmount.Should().BeNull();

                db.ImportErrors.Should().HaveCount(1);
            }
        }

        [TestMethod]
        public async Task LoadWaterAllocation_SimpleLoad_InvalidBeneficialUseCategory_ThrowsError()
        {
            OrganizationsDim organization;
            VariablesDim variable;
            MethodsDim method;
            DateDim dataPublicationDate;
            DateDim allocationPriorityDate;
            WaterAllocation waterAllocation;
            string startTestString = "01/01";
            string endTestString = "12/01";
            OwnerClassificationCv ownerClassificationCV;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                dataPublicationDate = await DateDimBuilder.Load(db);
                allocationPriorityDate = await DateDimBuilder.Load(db);
                ownerClassificationCV = await OwnerClassificationBuilder.Load(db);

                waterAllocation = WaterAllocationBuilder.Create(new WaterAllocationBuilderOptions { RecordType = WaterAllocationRecordType.None });

                waterAllocation.OrganizationUUID = organization.OrganizationUuid;
                waterAllocation.VariableSpecificUUID = variable.VariableSpecificUuid;
                waterAllocation.MethodUUID = method.MethodUuid;
                waterAllocation.DataPublicationDate = dataPublicationDate.Date;
                waterAllocation.AllocationPriorityDate = allocationPriorityDate.Date;
                waterAllocation.AllocationTimeframeStart = startTestString;
                waterAllocation.AllocationTimeframeEnd = endTestString;
                waterAllocation.BeneficialUseCategory = Guid.NewGuid().ToString();
                waterAllocation.OwnerClassificationCV = ownerClassificationCV.Name;
            }

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadWaterAllocation((new Faker()).Random.AlphaNumeric(10), new[] { waterAllocation });

            result.Should().BeFalse();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbAllocationAmount = await db.AllocationAmountsFact.FirstOrDefaultAsync();
                dbAllocationAmount.Should().BeNull();

                db.ImportErrors.Should().HaveCount(1);
            }
        }

        [TestMethod]
        public async Task LoadWaterAllocation_BadLoad_CivilianAndPower()
        {
            OrganizationsDim organization;
            VariablesDim variable;
            MethodsDim method;
            DateDim dataPublicationDate;
            DateDim allocationPriorityDate;
            WaterAllocation waterAllocation;
            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                dataPublicationDate = await DateDimBuilder.Load(db);
                allocationPriorityDate = await DateDimBuilder.Load(db);

                waterAllocation = WaterAllocationBuilder.Create(new WaterAllocationBuilderOptions { RecordType = WaterAllocationRecordType.Civilian });

                waterAllocation.OrganizationUUID = organization.OrganizationUuid;
                waterAllocation.VariableSpecificUUID = variable.VariableSpecificUuid;
                waterAllocation.MethodUUID = method.MethodUuid;
                waterAllocation.DataPublicationDate = dataPublicationDate.Date;
                waterAllocation.AllocationPriorityDate = allocationPriorityDate.Date;

                waterAllocation.GeneratedPowerCapacityMW = (new Faker()).Random.Decimal(0, 1000).ToString();
            }

            var sut = CreateDataIngestionAccessor();
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
            MethodsDim method;
            DateDim dataPublicationDate;
            WaterAllocation waterAllocation;
            string startTestString = "01/01";
            string endTestString = "12/01";

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
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
                waterAllocation.MethodUUID = method.MethodUuid;
                waterAllocation.DataPublicationDate = dataPublicationDate.Date;
                waterAllocation.AllocationTimeframeStart = startTestString;
                waterAllocation.AllocationTimeframeEnd = endTestString;
            }

            var sut = CreateDataIngestionAccessor();
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
                waterAllocation.MethodUUID = method.MethodUuid;
                waterAllocation.DataPublicationDate = dataPublicationDate.Date;
                waterAllocation.AllocationTimeframeStart = startTestString;
                waterAllocation.AllocationTimeframeEnd = endTestString;
            }

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadWaterAllocation((new Faker()).Random.AlphaNumeric(10), new[] { waterAllocation });

            result.Should().BeFalse();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                db.ImportErrors.Should().HaveCount(errorCount + 1);
                var error = await db.ImportErrors.OrderByDescending(a => a.Id).FirstAsync();
                error.Data.Should().Contain("Allocation Not Exempt of Volume Flow Priority");

            }
        }

        [TestMethod]
        public async Task LoadWaterAllocation_BigSiteUuid()
        {
            OrganizationsDim organization;
            VariablesDim variable;
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
                method = await MethodsDimBuilder.Load(db);
                dataPublicationDate = await DateDimBuilder.Load(db);
                allocationPriorityDate = await DateDimBuilder.Load(db);

                var commaSeparatedSites = "";
                while (commaSeparatedSites.Length < 2000)
                {
                    commaSeparatedSites += (await SitesDimBuilder.Load(db)).SiteUuid + ",";
                }
                commaSeparatedSites = commaSeparatedSites.Substring(0, commaSeparatedSites.Length - 1);

                waterAllocation = WaterAllocationBuilder.Create(new WaterAllocationBuilderOptions { RecordType = WaterAllocationRecordType.None });

                waterAllocation.SiteUUID = commaSeparatedSites;
                waterAllocation.OrganizationUUID = organization.OrganizationUuid;
                waterAllocation.VariableSpecificUUID = variable.VariableSpecificUuid;
                waterAllocation.MethodUUID = method.MethodUuid;
                waterAllocation.DataPublicationDate = dataPublicationDate.Date;
                waterAllocation.AllocationPriorityDate = allocationPriorityDate.Date;
                waterAllocation.AllocationTimeframeStart = startTestString;
                waterAllocation.AllocationTimeframeEnd = endTestString;
            }

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadWaterAllocation((new Faker()).Random.AlphaNumeric(10), new[] { waterAllocation });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbAllocationAmount = await db.AllocationAmountsFact.SingleAsync();

                dbAllocationAmount.AllocationAmountId.Should().NotBe(0);
                dbAllocationAmount.OrganizationId.Should().Be(organization.OrganizationId);
                dbAllocationAmount.VariableSpecificId.Should().Be(variable.VariableSpecificId);
                dbAllocationAmount.MethodId.Should().Be(method.MethodId);
                dbAllocationAmount.DataPublicationDateId.Should().Be(dataPublicationDate.DateId);
                dbAllocationAmount.AllocationPriorityDateID.Should().Be(allocationPriorityDate.DateId);
                dbAllocationAmount.AllocationTimeframeStart.Should().Be(startTestString);
                dbAllocationAmount.AllocationTimeframeEnd.Should().Be(endTestString);
                db.ImportErrors.Should().HaveCount(0);
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
                    new object[] { false, DateTime.Now, null, "30.123456"},
                    new object[] { false, DateTime.Now, "1000.75", null}
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

                    new object[] { null, null, null, null },
                    new object[] { null, DateTime.Now, null, null },
                    new object[] { null, null, "1000.75", null },
                    new object[] { null, null, null, "30.123456"},
                    new object[] { null, null, "1000.75", "30.123456"}
                };
            }
        }

        [TestMethod]
        public async Task LoadWaterAllocation_LoadAllocationBridge_PrimaryUseCategoryCV_Populated()
        {
            OrganizationsDim organization;
            VariablesDim variable;
            MethodsDim method;
            DateDim dataPublicationDate;
            DateDim allocationPriorityDate;
            WaterAllocation waterAllocation;
            BeneficialUsesCV beneficialUsesCV;
            BeneficialUsesCV primaryUseCategoryCV;
            string startTestString = "01/01";
            string endTestString = "12/01";

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                dataPublicationDate = await DateDimBuilder.Load(db);
                allocationPriorityDate = await DateDimBuilder.Load(db);
                beneficialUsesCV = await BeneficalUsesBuilder.Load(db);
                primaryUseCategoryCV = await BeneficalUsesBuilder.Load(db);

                waterAllocation = WaterAllocationBuilder.Create(new WaterAllocationBuilderOptions { RecordType = WaterAllocationRecordType.None });

                waterAllocation.PrimaryUseCategory = primaryUseCategoryCV.Name;

                waterAllocation.BeneficialUseCategory = beneficialUsesCV.Name;
                waterAllocation.OrganizationUUID = organization.OrganizationUuid;
                waterAllocation.VariableSpecificUUID = variable.VariableSpecificUuid;
                waterAllocation.MethodUUID = method.MethodUuid;
                waterAllocation.DataPublicationDate = dataPublicationDate.Date;
                waterAllocation.AllocationPriorityDate = allocationPriorityDate.Date;
                waterAllocation.AllocationTimeframeStart = startTestString;
                waterAllocation.AllocationTimeframeEnd = endTestString;
            }

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadWaterAllocation((new Faker()).Random.AlphaNumeric(10), new[] { waterAllocation });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbAllocationAmount = await db.AllocationAmountsFact.SingleAsync();
                dbAllocationAmount.Should().NotBeNull();
                dbAllocationAmount.PrimaryUseCategoryCV.Should().Be(primaryUseCategoryCV.Name);

                var dbAllocationBridge = await db.AllocationBridgeBeneficialUsesFact.SingleAsync();
                dbAllocationBridge.Should().NotBeNull();
                dbAllocationBridge.AllocationAmountId.Should().BeGreaterThan(0);
                dbAllocationBridge.BeneficialUseCV.Should().Be(beneficialUsesCV.Name);

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadWaterAllocation_LoadAllocationBridge_PrimaryUseCategoryCV_Null()
        {
            OrganizationsDim organization;
            VariablesDim variable;
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
                method = await MethodsDimBuilder.Load(db);
                dataPublicationDate = await DateDimBuilder.Load(db);
                allocationPriorityDate = await DateDimBuilder.Load(db);
                beneficialUsesCV = await BeneficalUsesBuilder.Load(db);

                waterAllocation = WaterAllocationBuilder.Create(new WaterAllocationBuilderOptions { RecordType = WaterAllocationRecordType.None });

                waterAllocation.PrimaryUseCategory = null;

                waterAllocation.BeneficialUseCategory = beneficialUsesCV.Name;
                waterAllocation.OrganizationUUID = organization.OrganizationUuid;
                waterAllocation.VariableSpecificUUID = variable.VariableSpecificUuid;
                waterAllocation.MethodUUID = method.MethodUuid;
                waterAllocation.DataPublicationDate = dataPublicationDate.Date;
                waterAllocation.AllocationPriorityDate = allocationPriorityDate.Date;
                waterAllocation.AllocationTimeframeStart = startTestString;
                waterAllocation.AllocationTimeframeEnd = endTestString;
            }

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadWaterAllocation((new Faker()).Random.AlphaNumeric(10), new[] { waterAllocation });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbAllocationAmount = await db.AllocationAmountsFact.SingleAsync();
                dbAllocationAmount.Should().NotBeNull();
                dbAllocationAmount.PrimaryUseCategoryCV.Should().BeNull();

                var dbAllocationBridge = await db.AllocationBridgeBeneficialUsesFact.SingleAsync();
                dbAllocationBridge.Should().NotBeNull();
                dbAllocationBridge.AllocationBridgeId.Should().BeGreaterThan(0);
                dbAllocationBridge.AllocationAmountId.Should().Be(dbAllocationBridge.AllocationAmountId);
                dbAllocationBridge.BeneficialUseCV.Should().Be(beneficialUsesCV.Name);

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadWaterAllocation_LoadAggBridge_PrimaryUseCategoryCV_Populated()
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
            BeneficialUsesCV primaryUseCategoryCV;

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
                primaryUseCategoryCV = await BeneficalUsesBuilder.Load(db);

                aggregateAmount = AggregatedAmountBuilder.Create();

                aggregateAmount.PrimaryUseCategory = primaryUseCategoryCV.Name;

                aggregateAmount.ReportingUnitUUID = reportingUnit.ReportingUnitUuid;
                aggregateAmount.BeneficialUseCategory = beneficialUsesCV.Name;
                aggregateAmount.OrganizationUUID = organization.OrganizationUuid;
                aggregateAmount.VariableSpecificUUID = variable.VariableSpecificUuid;
                aggregateAmount.WaterSourceUUID = waterSource.WaterSourceUuid;
                aggregateAmount.MethodUUID = method.MethodUuid;
                aggregateAmount.DataPublicationDate = dataPublicationDate.Date;
            }

            var sut = CreateDataIngestionAccessor();

            var result = await sut.LoadAggregatedAmounts((new Faker()).Random.AlphaNumeric(10), new[] { aggregateAmount });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbAggregateAmount = await db.AggregatedAmountsFact.SingleAsync();
                dbAggregateAmount.Should().NotBeNull();

                dbAggregateAmount.PrimaryUseCategoryCV.Should().Be(primaryUseCategoryCV.Name);

                var dbAggBridge = await db.AggBridgeBeneficialUsesFact.SingleAsync();
                dbAggBridge.Should().NotBeNull();
                dbAggBridge.AggBridgeId.Should().BeGreaterThan(0);
                dbAggBridge.AggregatedAmountId.Should().Be(dbAggregateAmount.AggregatedAmountId);
                dbAggBridge.BeneficialUseCV.Should().Be(beneficialUsesCV.Name);

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

            var sut = CreateDataIngestionAccessor();

            var result = await sut.LoadAggregatedAmounts((new Faker()).Random.AlphaNumeric(10), new[] { aggregateAmount });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbAggregateAmount = await db.AggregatedAmountsFact.SingleAsync();
                dbAggregateAmount.Should().NotBeNull();
                dbAggregateAmount.PrimaryUseCategoryCV.Should().BeNull();

                var dbAggBridge = await db.AggBridgeBeneficialUsesFact.SingleAsync();
                dbAggBridge.Should().NotBeNull();
                dbAggBridge.AggBridgeId.Should().BeGreaterThan(0);
                dbAggBridge.AggregatedAmountId.Should().Be(dbAggregateAmount.AggregatedAmountId);
                dbAggBridge.BeneficialUseCV.Should().Be(beneficialUsesCV.Name);

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadWaterAllocation_LoadAggBridge_InvalidPrimaryUseCategoryCVAndBeneficialUseCategoryCV_ThrowsError()
        {
            OrganizationsDim organization;
            ReportingUnitsDim reportingUnit;
            VariablesDim variable;
            WaterSourcesDim waterSource;
            MethodsDim method;
            DateDim dataPublicationDate;
            DateDim allocationPriorityDate;
            AggregatedAmount aggregateAmount;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                reportingUnit = await ReportingUnitsDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                waterSource = await WaterSourcesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                dataPublicationDate = await DateDimBuilder.Load(db);
                allocationPriorityDate = await DateDimBuilder.Load(db);

                aggregateAmount = AggregatedAmountBuilder.Create();

                aggregateAmount.PrimaryUseCategory = Guid.NewGuid().ToString();

                aggregateAmount.ReportingUnitUUID = reportingUnit.ReportingUnitUuid;
                aggregateAmount.BeneficialUseCategory = Guid.NewGuid().ToString();
                aggregateAmount.OrganizationUUID = organization.OrganizationUuid;
                aggregateAmount.VariableSpecificUUID = variable.VariableSpecificUuid;
                aggregateAmount.WaterSourceUUID = waterSource.WaterSourceUuid;
                aggregateAmount.MethodUUID = method.MethodUuid;
                aggregateAmount.DataPublicationDate = dataPublicationDate.Date;
            }

            var sut = CreateDataIngestionAccessor();

            var result = await sut.LoadAggregatedAmounts((new Faker()).Random.AlphaNumeric(10), new[] { aggregateAmount });

            result.Should().BeFalse();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbAggregateAmount = await db.AggregatedAmountsFact.FirstOrDefaultAsync();
                dbAggregateAmount.Should().BeNull();

                var dbAggBridge = await db.AggBridgeBeneficialUsesFact.FirstOrDefaultAsync();
                dbAggBridge.Should().BeNull();

                db.ImportErrors.Should().HaveCount(1);
            }
        }

        [TestMethod]
        public async Task LoadSiteSpecificAmounts_SimpleLoad_Civilian()
        {
            OrganizationsDim organization;
            SitesDim site;
            VariablesDim variable;
            WaterSourcesDim waterSource;
            MethodsDim method;
            SiteSpecificAmount siteSpecificAmount;
            BeneficialUsesCV beneficialUses;
            BeneficialUsesCV primaryUseCategory;
            ReportYearCv reportYear;
            DateDim publicationDate;
            CustomerType customerType;
            SDWISIdentifier sdwisIdentifier;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                site = await SitesDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                waterSource = await WaterSourcesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                beneficialUses = await BeneficalUsesBuilder.Load(db);
                primaryUseCategory = await BeneficalUsesBuilder.Load(db);
                reportYear = await ReportYearCvBuilder.Load(db);
                publicationDate = await DateDimBuilder.Load(db);
                customerType = await CustomerTypeBuilder.Load(db);
                sdwisIdentifier = await SDWISIdentifierBuilder.Load(db);

                siteSpecificAmount = SiteSpecificAmountBuilder.Create(new SiteSpecificAmountBuilderOptions
                {
                    RecordType = SiteSpecificRecordType.Civilian,
                    Method = method,
                    Organization = organization,
                    DataPublicationDate = publicationDate,
                    Site = site,
                    Variable = variable,
                    WaterSource = waterSource,
                    BeneficialUse = beneficialUses,
                    PrimaryUseCategory = primaryUseCategory,
                    ReportYear = reportYear,
                    CustomerType = customerType,
                    SDWISIdentifier = sdwisIdentifier
                });
            }

            siteSpecificAmount.PopulationServed.Should().NotBeNullOrEmpty("Required field");
            siteSpecificAmount.CommunityWaterSupplySystem.Should().NotBeNullOrEmpty("Required field");
            siteSpecificAmount.CustomerTypeCV.Should().NotBeNullOrEmpty("Required field");
            siteSpecificAmount.SDWISIdentifier.Should().NotBeNullOrEmpty("Required field");

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadSiteSpecificAmounts((new Faker()).Random.AlphaNumeric(10), new[] { siteSpecificAmount });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbSiteVariableAmount = await db.SiteVariableAmountsFact.SingleAsync();

                dbSiteVariableAmount.OrganizationId.Should().Be(organization.OrganizationId);
                dbSiteVariableAmount.SiteId.Should().Be(site.SiteId);
                dbSiteVariableAmount.VariableSpecificId.Should().Be(variable.VariableSpecificId);
                dbSiteVariableAmount.WaterSourceId.Should().Be(waterSource.WaterSourceId);
                dbSiteVariableAmount.MethodId.Should().Be(method.MethodId);
                dbSiteVariableAmount.PrimaryUseCategoryCV.Should().Be(primaryUseCategory.Name);
                dbSiteVariableAmount.ReportYearCv.Should().Be(reportYear.Name);

                dbSiteVariableAmount.PopulationServed.Should().Be(long.Parse(siteSpecificAmount.PopulationServed));
                dbSiteVariableAmount.CommunityWaterSupplySystem.Should().Be(siteSpecificAmount.CommunityWaterSupplySystem);
                dbSiteVariableAmount.CustomerTypeCv.Should().Be(customerType.Name);
                dbSiteVariableAmount.SDWISIdentifierCv.Should().Be(sdwisIdentifier.Name);

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadSiteSpecificAmounts_SimpleLoad_Civilian_InvalidPrimaryUseCategoryCV_ThrowsError()
        {
            OrganizationsDim organization;
            SitesDim site;
            VariablesDim variable;
            WaterSourcesDim waterSource;
            MethodsDim method;
            SiteSpecificAmount siteSpecificAmount;
            ReportYearCv reportYear;
            DateDim publicationDate;
            CustomerType customerType;
            SDWISIdentifier sdwisIdentifier;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                site = await SitesDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                waterSource = await WaterSourcesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                reportYear = await ReportYearCvBuilder.Load(db);
                publicationDate = await DateDimBuilder.Load(db);
                customerType = await CustomerTypeBuilder.Load(db);
                sdwisIdentifier = await SDWISIdentifierBuilder.Load(db);

                siteSpecificAmount = SiteSpecificAmountBuilder.Create(new SiteSpecificAmountBuilderOptions
                {
                    RecordType = SiteSpecificRecordType.Civilian,
                    Method = method,
                    Organization = organization,
                    DataPublicationDate = publicationDate,
                    Site = site,
                    Variable = variable,
                    WaterSource = waterSource,
                    PrimaryUseCategory = new BeneficialUsesCV { Name = Guid.NewGuid().ToString() },
                    ReportYear = reportYear,
                    CustomerType = customerType,
                    SDWISIdentifier = sdwisIdentifier
                });
            }

            siteSpecificAmount.PopulationServed.Should().NotBeNullOrEmpty("Required field");
            siteSpecificAmount.CommunityWaterSupplySystem.Should().NotBeNullOrEmpty("Required field");
            siteSpecificAmount.CustomerTypeCV.Should().NotBeNullOrEmpty("Required field");
            siteSpecificAmount.SDWISIdentifier.Should().NotBeNullOrEmpty("Required field");

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadSiteSpecificAmounts((new Faker()).Random.AlphaNumeric(10), new[] { siteSpecificAmount });

            result.Should().BeFalse();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbSiteVariableAmount = await db.SiteVariableAmountsFact.FirstOrDefaultAsync();
                dbSiteVariableAmount.Should().BeNull();

                db.ImportErrors.Should().HaveCount(1);
            }
        }

        [TestMethod]
        public async Task LoadSiteSpecificAmounts_SimpleLoad_Agriculture()
        {
            OrganizationsDim organization;
            SitesDim site;
            VariablesDim variable;
            WaterSourcesDim waterSource;
            MethodsDim method;
            SiteSpecificAmount siteSpecificAmount;
            BeneficialUsesCV beneficialUses;
            BeneficialUsesCV primaryUseCategory;
            ReportYearCv reportYear;
            CropType cropType;
            IrrigationMethod irrigationMethod;
            DateDim publicationDate;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                site = await SitesDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                waterSource = await WaterSourcesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                beneficialUses = await BeneficalUsesBuilder.Load(db);
                primaryUseCategory = await BeneficalUsesBuilder.Load(db);
                reportYear = await ReportYearCvBuilder.Load(db);
                cropType = await CropTypeBuilder.Load(db);
                irrigationMethod = await IrrigationMethodBuilder.Load(db);
                publicationDate = await DateDimBuilder.Load(db);

                siteSpecificAmount = SiteSpecificAmountBuilder.Create(new SiteSpecificAmountBuilderOptions
                {
                    RecordType = SiteSpecificRecordType.Ag,
                    Method = method,
                    Organization = organization,
                    DataPublicationDate = publicationDate,
                    Site = site,
                    Variable = variable,
                    WaterSource = waterSource,
                    BeneficialUse = beneficialUses,
                    PrimaryUseCategory = primaryUseCategory,
                    ReportYear = reportYear,
                    CropType = cropType,
                    IrrigationMethod = irrigationMethod
                });
            }

            // Ag
            siteSpecificAmount.IrrigatedAcreage.Should().NotBeNullOrEmpty("Required field");
            siteSpecificAmount.CropTypeCV.Should().NotBeNullOrEmpty("Required field");
            siteSpecificAmount.IrrigationMethodCV.Should().NotBeNullOrEmpty("Required field");
            siteSpecificAmount.AllocationCropDutyAmount.Should().NotBeNullOrEmpty("Required field");

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadSiteSpecificAmounts((new Faker()).Random.AlphaNumeric(10), new[] { siteSpecificAmount });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbSiteVariableAmount = await db.SiteVariableAmountsFact.SingleAsync();

                dbSiteVariableAmount.OrganizationId.Should().Be(organization.OrganizationId);
                dbSiteVariableAmount.SiteId.Should().Be(site.SiteId);
                dbSiteVariableAmount.VariableSpecificId.Should().Be(variable.VariableSpecificId);
                dbSiteVariableAmount.WaterSourceId.Should().Be(waterSource.WaterSourceId);
                dbSiteVariableAmount.MethodId.Should().Be(method.MethodId);
                dbSiteVariableAmount.PrimaryUseCategoryCV.Should().Be(primaryUseCategory.Name);
                dbSiteVariableAmount.ReportYearCv.Should().Be(reportYear.Name);

                dbSiteVariableAmount.IrrigatedAcreage.Should().Be(double.Parse(siteSpecificAmount.IrrigatedAcreage));
                dbSiteVariableAmount.CropTypeCv.Should().Be(cropType.Name);
                dbSiteVariableAmount.IrrigationMethodCv.Should().Be(irrigationMethod.Name);
                dbSiteVariableAmount.AllocationCropDutyAmount.Should().Be(double.Parse(siteSpecificAmount.AllocationCropDutyAmount));

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadSiteSpecificAmounts_SimpleLoad_Power()
        {
            OrganizationsDim organization;
            SitesDim site;
            VariablesDim variable;
            WaterSourcesDim waterSource;
            MethodsDim method;
            SiteSpecificAmount siteSpecificAmount;
            BeneficialUsesCV beneficialUses;
            BeneficialUsesCV primaryUseCategory;
            ReportYearCv reportYear;
            DateDim publicationDate;
            PowerType powerType;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                site = await SitesDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                waterSource = await WaterSourcesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                beneficialUses = await BeneficalUsesBuilder.Load(db);
                primaryUseCategory = await BeneficalUsesBuilder.Load(db);
                reportYear = await ReportYearCvBuilder.Load(db);
                publicationDate = await DateDimBuilder.Load(db);
                powerType = await PowerTypeBuilder.Load(db);

                siteSpecificAmount = SiteSpecificAmountBuilder.Create(new SiteSpecificAmountBuilderOptions
                {
                    RecordType = SiteSpecificRecordType.Power,
                    Method = method,
                    Organization = organization,
                    DataPublicationDate = publicationDate,
                    Site = site,
                    Variable = variable,
                    WaterSource = waterSource,
                    BeneficialUse = beneficialUses,
                    PrimaryUseCategory = primaryUseCategory,
                    ReportYear = reportYear,
                    PowerType = powerType
                });
            }

            siteSpecificAmount.PowerGeneratedGWh.Should().NotBeNullOrEmpty("Required field");
            siteSpecificAmount.PowerType.Should().NotBeNullOrEmpty("Required field");

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadSiteSpecificAmounts((new Faker()).Random.AlphaNumeric(10), new[] { siteSpecificAmount });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbSiteVariableAmount = await db.SiteVariableAmountsFact.SingleAsync();

                dbSiteVariableAmount.OrganizationId.Should().Be(organization.OrganizationId);
                dbSiteVariableAmount.SiteId.Should().Be(site.SiteId);
                dbSiteVariableAmount.VariableSpecificId.Should().Be(variable.VariableSpecificId);
                dbSiteVariableAmount.WaterSourceId.Should().Be(waterSource.WaterSourceId);
                dbSiteVariableAmount.MethodId.Should().Be(method.MethodId);
                dbSiteVariableAmount.PrimaryUseCategoryCV.Should().Be(primaryUseCategory.Name);
                dbSiteVariableAmount.ReportYearCv.Should().Be(reportYear.Name);

                dbSiteVariableAmount.PowerGeneratedGwh.Should().Be(double.Parse(siteSpecificAmount.PowerGeneratedGWh));
                dbSiteVariableAmount.PowerType.Should().Be(powerType.Name);

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadSiteSpecificAmounts_SimpleLoad_Power_InvalidBeneficialUseCategory_ThrowsError()
        {
            OrganizationsDim organization;
            SitesDim site;
            VariablesDim variable;
            WaterSourcesDim waterSource;
            MethodsDim method;
            SiteSpecificAmount siteSpecificAmount;
            BeneficialUsesCV primaryUseCategory;
            ReportYearCv reportYear;
            DateDim publicationDate;
            PowerType powerType;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                site = await SitesDimBuilder.Load(db);
                variable = await VariablesDimBuilder.Load(db);
                waterSource = await WaterSourcesDimBuilder.Load(db);
                method = await MethodsDimBuilder.Load(db);
                primaryUseCategory = await BeneficalUsesBuilder.Load(db);
                reportYear = await ReportYearCvBuilder.Load(db);
                publicationDate = await DateDimBuilder.Load(db);
                powerType = await PowerTypeBuilder.Load(db);

                siteSpecificAmount = SiteSpecificAmountBuilder.Create(new SiteSpecificAmountBuilderOptions
                {
                    RecordType = SiteSpecificRecordType.Power,
                    Method = method,
                    Organization = organization,
                    DataPublicationDate = publicationDate,
                    Site = site,
                    Variable = variable,
                    WaterSource = waterSource,
                    BeneficialUse = new BeneficialUsesCV { Name = Guid.NewGuid().ToString() },
                    PrimaryUseCategory = primaryUseCategory,
                    ReportYear = reportYear,
                    PowerType = powerType
                });
            }

            siteSpecificAmount.PowerGeneratedGWh.Should().NotBeNullOrEmpty("Required field");
            siteSpecificAmount.PowerType.Should().NotBeNullOrEmpty("Required field");

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadSiteSpecificAmounts((new Faker()).Random.AlphaNumeric(10), new[] { siteSpecificAmount });

            result.Should().BeFalse();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbSiteVariableAmount = await db.SiteVariableAmountsFact.FirstOrDefaultAsync();
                dbSiteVariableAmount.Should().BeNull();

                db.ImportErrors.Should().HaveCount(1);
            }
        }

        [TestMethod]
        public async Task LoadRegulatoryReportingUnits_SimpleLoad()
        {
            OrganizationsDim organization;
            RegulatoryOverlayDim regulatoryOverlay;
            ReportingUnitsDim reportingUnit;
            DateDim date;

            RegulatoryReportingUnits regulatoryReportingUnit;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                organization = await OrganizationsDimBuilder.Load(db);
                regulatoryOverlay = await RegulatoryOverlayDimBuilder.Load(db);
                reportingUnit = await ReportingUnitsDimBuilder.Load(db);
                date = await DateDimBuilder.Load(db);

                regulatoryReportingUnit = RegulatoryReportingUnitBuilder.Create(new RegulatoryReportingUnitBuilderOptions
                {
                    Organization = organization,
                    RegulatoryOverlay = regulatoryOverlay,
                    ReportingUnit = reportingUnit,
                    DatePublication = date
                });
            }

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadRegulatoryReportingUnits((new Faker()).Random.AlphaNumeric(10), new[] { regulatoryReportingUnit });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbRegulatoryReportingUnit = await db.RegulatoryReportingUnitsFact.SingleAsync();

                dbRegulatoryReportingUnit.DataPublicationDateId.Should().Be(date.DateId);
                dbRegulatoryReportingUnit.OrganizationId.Should().Be(organization.OrganizationId);
                dbRegulatoryReportingUnit.RegulatoryOverlayId.Should().Be(regulatoryOverlay.RegulatoryOverlayId);
                dbRegulatoryReportingUnit.ReportingUnitId.Should().Be(reportingUnit.ReportingUnitId);

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadSite_SimpleLoad()
        {
            SitesDim siteDim;
            RegulatoryOverlayDim regulatoryOverlay;

            Site site;
            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                siteDim = await SitesDimBuilder.Load(db);
                regulatoryOverlay = await RegulatoryOverlayDimBuilder.Load(db);

                site = SiteBuilder.Create(new SiteBuilderOptions()
                {
                    Site = siteDim,
                    RegulatoryOverlayDims = new List<RegulatoryOverlayDim> { regulatoryOverlay }
                });
            }

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadSites((new Faker()).Random.AlphaNumeric(10), new[] { site });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbRegulatoryOverlayBridgeSitesFact = await db.RegulatoryOverlayBridgeSitesFact.SingleAsync();

                dbRegulatoryOverlayBridgeSitesFact.SiteId.Should().Be(siteDim.SiteId);
                dbRegulatoryOverlayBridgeSitesFact.RegulatoryOverlayId.Should().Be(regulatoryOverlay.RegulatoryOverlayId);

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadSite_LoadTwo()
        {
            SitesDim siteDim;
            RegulatoryOverlayDim regulatoryOverlay1;
            RegulatoryOverlayDim regulatoryOverlay2;

            Site site;
            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                siteDim = await SitesDimBuilder.Load(db);
                regulatoryOverlay1 = await RegulatoryOverlayDimBuilder.Load(db);
                regulatoryOverlay2 = await RegulatoryOverlayDimBuilder.Load(db);

                site = SiteBuilder.Create(new SiteBuilderOptions()
                {
                    Site = siteDim,
                    RegulatoryOverlayDims = new List<RegulatoryOverlayDim> { regulatoryOverlay1, regulatoryOverlay2 }
                });
            }

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadSites((new Faker()).Random.AlphaNumeric(10), new[] { site });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbRegulatoryOverlayBridgeSitesFact = db.RegulatoryOverlayBridgeSitesFact;
                dbRegulatoryOverlayBridgeSitesFact.Count().Should().Be(2);

                var reg1 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay1.RegulatoryOverlayId).ToList();
                reg1.Count().Should().Be(1);
                reg1[0].SiteId.Should().Be(siteDim.SiteId);

                var reg2 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay1.RegulatoryOverlayId).ToList();
                reg2.Count().Should().Be(1);
                reg2[0].SiteId.Should().Be(siteDim.SiteId);

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadSite_LoadTwo_AddOneMore()
        {
            SitesDim siteDim;
            RegulatoryOverlayDim regulatoryOverlay1;
            RegulatoryOverlayDim regulatoryOverlay2;

            Site site;
            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                siteDim = await SitesDimBuilder.Load(db);
                regulatoryOverlay1 = await RegulatoryOverlayDimBuilder.Load(db);
                regulatoryOverlay2 = await RegulatoryOverlayDimBuilder.Load(db);

                site = SiteBuilder.Create(new SiteBuilderOptions()
                {
                    Site = siteDim,
                    RegulatoryOverlayDims = new List<RegulatoryOverlayDim> { regulatoryOverlay1, regulatoryOverlay2 }
                });
            }

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadSites((new Faker()).Random.AlphaNumeric(10), new[] { site });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbRegulatoryOverlayBridgeSitesFact = db.RegulatoryOverlayBridgeSitesFact;
                dbRegulatoryOverlayBridgeSitesFact.Count().Should().Be(2);

                var reg1 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay1.RegulatoryOverlayId).ToList();
                reg1.Count().Should().Be(1);
                reg1[0].SiteId.Should().Be(siteDim.SiteId);

                var reg2 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay1.RegulatoryOverlayId).ToList();
                reg2.Count().Should().Be(1);
                reg2[0].SiteId.Should().Be(siteDim.SiteId);

                db.ImportErrors.Should().HaveCount(0);
            }

            RegulatoryOverlayDim regulatoryOverlay3;
            Site updatedSite;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                regulatoryOverlay3 = await RegulatoryOverlayDimBuilder.Load(db);

                updatedSite = SiteBuilder.Create(new SiteBuilderOptions()
                {
                    Site = siteDim,
                    RegulatoryOverlayDims = new List<RegulatoryOverlayDim> { regulatoryOverlay1, regulatoryOverlay2, regulatoryOverlay3 }
                });
            }

            var updatedResult = await sut.LoadSites((new Faker()).Random.AlphaNumeric(10), new[] { updatedSite });

            updatedResult.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbRegulatoryOverlayBridgeSitesFact = db.RegulatoryOverlayBridgeSitesFact;
                dbRegulatoryOverlayBridgeSitesFact.Count().Should().Be(3);

                var reg1 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay1.RegulatoryOverlayId).ToList();
                reg1.Count().Should().Be(1);
                reg1[0].SiteId.Should().Be(siteDim.SiteId);

                var reg2 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay1.RegulatoryOverlayId).ToList();
                reg2.Count().Should().Be(1);
                reg2[0].SiteId.Should().Be(siteDim.SiteId);

                var reg3 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay3.RegulatoryOverlayId).ToList();
                reg3.Count().Should().Be(1);
                reg3[0].SiteId.Should().Be(siteDim.SiteId);

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadSite_LoadTwo_AddOne_RemoveOne()
        {
            SitesDim siteDim;
            RegulatoryOverlayDim regulatoryOverlay1;
            RegulatoryOverlayDim regulatoryOverlay2;

            Site site;
            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                siteDim = await SitesDimBuilder.Load(db);
                regulatoryOverlay1 = await RegulatoryOverlayDimBuilder.Load(db);
                regulatoryOverlay2 = await RegulatoryOverlayDimBuilder.Load(db);

                site = SiteBuilder.Create(new SiteBuilderOptions()
                {
                    Site = siteDim,
                    RegulatoryOverlayDims = new List<RegulatoryOverlayDim> { regulatoryOverlay1, regulatoryOverlay2 }
                });
            }

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadSites((new Faker()).Random.AlphaNumeric(10), new[] { site });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbRegulatoryOverlayBridgeSitesFact = db.RegulatoryOverlayBridgeSitesFact;
                dbRegulatoryOverlayBridgeSitesFact.Count().Should().Be(2);

                var reg1 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay1.RegulatoryOverlayId).ToList();
                reg1.Count().Should().Be(1);
                reg1[0].SiteId.Should().Be(siteDim.SiteId);

                var reg2 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay1.RegulatoryOverlayId).ToList();
                reg2.Count().Should().Be(1);
                reg2[0].SiteId.Should().Be(siteDim.SiteId);

                db.ImportErrors.Should().HaveCount(0);
            }

            RegulatoryOverlayDim regulatoryOverlay3;
            Site updatedSite;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                regulatoryOverlay3 = await RegulatoryOverlayDimBuilder.Load(db);

                updatedSite = SiteBuilder.Create(new SiteBuilderOptions()
                {
                    Site = siteDim,
                    RegulatoryOverlayDims = new List<RegulatoryOverlayDim> { regulatoryOverlay1, regulatoryOverlay3 }
                });
            }

            var updatedResult = await sut.LoadSites((new Faker()).Random.AlphaNumeric(10), new[] { updatedSite });

            updatedResult.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbRegulatoryOverlayBridgeSitesFact = db.RegulatoryOverlayBridgeSitesFact;
                dbRegulatoryOverlayBridgeSitesFact.Count().Should().Be(2);

                var reg1 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay1.RegulatoryOverlayId).ToList();
                reg1.Count().Should().Be(1);
                reg1[0].SiteId.Should().Be(siteDim.SiteId);

                var reg3 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay3.RegulatoryOverlayId).ToList();
                reg3.Count().Should().Be(1);
                reg3[0].SiteId.Should().Be(siteDim.SiteId);

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadSite_LoadTwo_RemoveAll()
        {
            SitesDim siteDim;
            RegulatoryOverlayDim regulatoryOverlay1;
            RegulatoryOverlayDim regulatoryOverlay2;

            Site site;
            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                siteDim = await SitesDimBuilder.Load(db);
                regulatoryOverlay1 = await RegulatoryOverlayDimBuilder.Load(db);
                regulatoryOverlay2 = await RegulatoryOverlayDimBuilder.Load(db);

                site = SiteBuilder.Create(new SiteBuilderOptions()
                {
                    Site = siteDim,
                    RegulatoryOverlayDims = new List<RegulatoryOverlayDim> { regulatoryOverlay1, regulatoryOverlay2 }
                });
            }

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadSites((new Faker()).Random.AlphaNumeric(10), new[] { site });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbRegulatoryOverlayBridgeSitesFact = db.RegulatoryOverlayBridgeSitesFact;
                dbRegulatoryOverlayBridgeSitesFact.Count().Should().Be(2);

                var reg1 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay1.RegulatoryOverlayId).ToList();
                reg1.Count().Should().Be(1);
                reg1[0].SiteId.Should().Be(siteDim.SiteId);

                var reg2 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay1.RegulatoryOverlayId).ToList();
                reg2.Count().Should().Be(1);
                reg2[0].SiteId.Should().Be(siteDim.SiteId);

                db.ImportErrors.Should().HaveCount(0);
            }

            Site updatedSite;

            updatedSite = SiteBuilder.Create(new SiteBuilderOptions()
            {
                Site = siteDim,
                RegulatoryOverlayDims = new List<RegulatoryOverlayDim>()
            });

            var updatedResult = await sut.LoadSites((new Faker()).Random.AlphaNumeric(10), new[] { updatedSite });

            updatedResult.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbRegulatoryOverlayBridgeSitesFact = db.RegulatoryOverlayBridgeSitesFact;
                dbRegulatoryOverlayBridgeSitesFact.Count().Should().Be(0);

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadSite_WithNoRegulatoryOverlays()
        {
            SitesDim siteDim;

            Site site;
            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                siteDim = await SitesDimBuilder.Load(db);

                site = SiteBuilder.Create(new SiteBuilderOptions()
                {
                    Site = siteDim,
                    RegulatoryOverlayDims = new List<RegulatoryOverlayDim> { }
                });
            }

            var sut = CreateDataIngestionAccessor();
            var result = await sut.LoadSites((new Faker()).Random.AlphaNumeric(10), new[] { site });

            result.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbRegulatoryOverlayBridgeSitesFact = db.RegulatoryOverlayBridgeSitesFact;
                dbRegulatoryOverlayBridgeSitesFact.Count().Should().Be(0);

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task LoadSite_TwoSites()
        {
            SitesDim siteDim1;
            RegulatoryOverlayDim regulatoryOverlay1;
            Site site1;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                siteDim1 = await SitesDimBuilder.Load(db);
                regulatoryOverlay1 = await RegulatoryOverlayDimBuilder.Load(db);

                site1 = SiteBuilder.Create(new SiteBuilderOptions()
                {
                    Site = siteDim1,
                    RegulatoryOverlayDims = new List<RegulatoryOverlayDim> { regulatoryOverlay1 }
                });
            }

            var sut = CreateDataIngestionAccessor();

            var result1 = await sut.LoadSites((new Faker()).Random.AlphaNumeric(10), new[] { site1 });
            result1.Should().BeTrue();

            SitesDim siteDim2;
            RegulatoryOverlayDim regulatoryOverlay2;
            Site site2;

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                siteDim2 = await SitesDimBuilder.Load(db);
                regulatoryOverlay2 = await RegulatoryOverlayDimBuilder.Load(db);

                site2 = SiteBuilder.Create(new SiteBuilderOptions()
                {
                    Site = siteDim2,
                    RegulatoryOverlayDims = new List<RegulatoryOverlayDim> { regulatoryOverlay2 }
                });
            }


            var result2 = await sut.LoadSites((new Faker()).Random.AlphaNumeric(10), new[] { site2 });
            result2.Should().BeTrue();

            using (var db = new WaDEContext(Configuration.GetConfiguration()))
            {
                var dbRegulatoryOverlayBridgeSitesFact = db.RegulatoryOverlayBridgeSitesFact;
                dbRegulatoryOverlayBridgeSitesFact.Count().Should().Be(2);

                var reg1 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay1.RegulatoryOverlayId).ToList();
                reg1.Count().Should().Be(1);
                reg1[0].SiteId.Should().Be(siteDim1.SiteId);

                var reg2 = dbRegulatoryOverlayBridgeSitesFact.Where(x =>
                    x.RegulatoryOverlayId == regulatoryOverlay2.RegulatoryOverlayId).ToList();
                reg2.Count().Should().Be(1);
                reg2[0].SiteId.Should().Be(siteDim2.SiteId);

                db.ImportErrors.Should().HaveCount(0);
            }
        }

        private IDataIngestionAccessor CreateDataIngestionAccessor()
        {
            return new DataIngestionAccessor(Configuration.GetConfiguration(), LoggerFactory);
        }
    }
}