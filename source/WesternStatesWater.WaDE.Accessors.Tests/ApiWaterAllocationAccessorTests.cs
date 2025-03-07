using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.JustMock;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Accessors.Tests
{
    [TestClass]
    public class ApiWaterAllocationAccessorTests : DbTestBase
    {
        private readonly ILoggerFactory LoggerFactory = new LoggerFactory();

        [TestMethod]
        public async Task GetSiteAllocationAmountsAsync_Filters_None()
        {
            var configuration = Configuration.GetConfiguration();
            AllocationAmountsFact allocationAmountsFact;
            SitesDim siteDim;
            WaterSourcesDim waterSourceDim;
            RegulatoryOverlayDim regulatoryOverlayDim;
            using (var db = new WaDEContext(configuration))
            {
                allocationAmountsFact = await AllocationAmountsFactBuilder.Load(db);
                waterSourceDim = await WaterSourcesDimBuilder.Load(db);
                regulatoryOverlayDim = await RegulatoryOverlayDimBuilder.Load(db);

                siteDim = await SitesDimBuilder.Load(db);
                await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
                {
                    SitesDim = siteDim,
                    AllocationAmountsFact = allocationAmountsFact
                });

                await WaterSourceBridgeSitesFactBuilder.Load(db, new WaterSourceBridgeSitesFactBuilderOptions
                {
                    SitesDim = siteDim,
                    WaterSourcesDim = waterSourceDim
                });

                await RegulatoryOverlayBridgeSitesFactBuilder.Load(db,
                    new RegulatoryOverlayBridgeSitesFactBuilderOptions
                    {
                        SitesDim = siteDim,
                        RegulatoryOverlayDim = regulatoryOverlayDim
                    });
                allocationAmountsFact.AllocationAmountId.Should().NotBe(0);
            }

            var filters = new SiteAllocationAmountsFilters();

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.GetSiteAllocationAmountsAsync(filters, 0, int.MaxValue);

            result.TotalWaterAllocationsCount.Should().Be(1);
            result.Organizations.Should().HaveCount(1);

            var org = result.Organizations.Single();
            org.OrganizationId.Should().Be(allocationAmountsFact.OrganizationId);
            org.WaterAllocations.Should().HaveCount(1);
            org.WaterAllocations[0].AllocationAmountId.Should().Be(allocationAmountsFact.AllocationAmountId);
            org.Sites.Should().HaveCount(1);
            org.Sites[0].WaterSourceUUIDs.Should().HaveCount(1)
                .And.Contain(waterSourceDim.WaterSourceUuid);
            org.WaterAllocations[0].SitesUUIDs.Should().HaveCount(1)
                .And.Contain(org.Sites[0].SiteUUID);

            org.WaterSources.Should().HaveCount(1)
                .And.Contain(a => a.WaterSourceUUID == waterSourceDim.WaterSourceUuid);

            org.RegulatoryOverlays.Should().HaveCount(1);
            org.RegulatoryOverlays[0].RegulatoryOverlayUUID.Should().Be(regulatoryOverlayDim.RegulatoryOverlayUuid);
        }

        [DataTestMethod]
        [DataRow(0, new int[0])]
        [DataRow(1, new[] { 0 })]
        [DataRow(1, new[] { 1 })]
        [DataRow(1, new[] { 2 })]
        [DataRow(2, new[] { 0, 0 })]
        [DataRow(2, new[] { 1, 0 })]
        [DataRow(2, new[] { 0, 1 })]
        [DataRow(2, new[] { 1, 1 })]
        [DataRow(2, new[] { 2, 0 })]
        [DataRow(2, new[] { 2, 1 })]
        [DataRow(2, new[] { 2, 2 })]
        public async Task GetSiteAllocationAmountsAsync_WaterSources(int siteCount, int[] waterSourceCounts)
        {
            var configuration = Configuration.GetConfiguration();
            AllocationAmountsFact allocationAmountsFact;
            SitesDim[] siteDims = new SitesDim[siteCount];
            WaterSourcesDim[] waterSourceDims;
            var maxWaterSources = waterSourceCounts.DefaultIfEmpty().Max();
            using (var db = new WaDEContext(configuration))
            {
                allocationAmountsFact = await AllocationAmountsFactBuilder.Load(db);

                waterSourceDims = Enumerable.Range(0, maxWaterSources).Select(a => WaterSourcesDimBuilder.Load(db).Result).ToArray();

                for (var i = 0; i < siteCount; i++)
                {
                    siteDims[i] = await SitesDimBuilder.Load(db);
                    await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
                    {
                        SitesDim = siteDims[i],
                        AllocationAmountsFact = allocationAmountsFact
                    });
                    for (var j = 0; j < waterSourceCounts[i]; j++)
                    {
                        await WaterSourceBridgeSitesFactBuilder.Load(db, new WaterSourceBridgeSitesFactBuilderOptions
                        {
                            SitesDim = siteDims[i],
                            WaterSourcesDim = waterSourceDims[j]
                        });
                    }
                }
            }

            var filters = new SiteAllocationAmountsFilters();

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.GetSiteAllocationAmountsAsync(filters, 0, int.MaxValue);

            result.TotalWaterAllocationsCount.Should().Be(1);
            result.Organizations.Should().HaveCount(1);

            var org = result.Organizations.Single();
            org.OrganizationId.Should().Be(allocationAmountsFact.OrganizationId);
            org.WaterAllocations.Should().HaveCount(1);
            org.WaterAllocations[0].AllocationAmountId.Should().Be(allocationAmountsFact.AllocationAmountId);
            org.Sites.Should().HaveCount(siteCount);
            for (var i = 0; i < siteCount; i++)
            {
                org.Sites[i].WaterSourceUUIDs.Should().HaveCount(waterSourceCounts[i])
                    .And.BeEquivalentTo(waterSourceDims[..waterSourceCounts[i]].Select(a => a.WaterSourceUuid));
            }

            org.WaterSources.Should().HaveCount(maxWaterSources)
                .And.Subject.Select(a => a.WaterSourceUUID).Should()
                .BeEquivalentTo(waterSourceDims.Select(a => a.WaterSourceUuid));
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task GetSiteAllocationAmountsAsync_Sites_SameSiteMultipleAllocations(bool sameOrganization)
        {
            var configuration = Configuration.GetConfiguration();
            AllocationAmountsFact allocationAmountsFact1, allocationAmountsFact2;
            SitesDim allocationSite, podSite, pouSite;
            OrganizationsDim organization1, organization2;
            using (var db = new WaDEContext(configuration))
            {
                organization1 = await OrganizationsDimBuilder.Load(db);
                organization2 = sameOrganization ? organization1 : await OrganizationsDimBuilder.Load(db);

                allocationAmountsFact1 = await AllocationAmountsFactBuilder.Load(db, new AllocationAmountsFactBuilderOptions
                {
                    OrganizationsDim = organization1
                });
                allocationAmountsFact2 = await AllocationAmountsFactBuilder.Load(db, new AllocationAmountsFactBuilderOptions
                {
                    OrganizationsDim = organization2
                });

                allocationSite = await SitesDimBuilder.Load(db);
                podSite = await SitesDimBuilder.Load(db);
                pouSite = await SitesDimBuilder.Load(db);
                await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
                {
                    SitesDim = allocationSite,
                    AllocationAmountsFact = allocationAmountsFact1
                });
                await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
                {
                    SitesDim = allocationSite,
                    AllocationAmountsFact = allocationAmountsFact2
                });
                await PODSiteToPOUSiteFactBuilder.Load(db, new PODSiteToPOUSiteFactBuilderOptions
                {
                    PouSite = allocationSite,
                    PodSite = podSite
                });
                await PODSiteToPOUSiteFactBuilder.Load(db, new PODSiteToPOUSiteFactBuilderOptions
                {
                    PodSite = allocationSite,
                    PouSite = pouSite
                });
            }

            var filters = new SiteAllocationAmountsFilters();

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.GetSiteAllocationAmountsAsync(filters, 0, int.MaxValue);

            result.TotalWaterAllocationsCount.Should().Be(2);
            if (sameOrganization)
            {
                result.Organizations.Should().HaveCount(1)
                .And.Contain(a => a.OrganizationId == organization1.OrganizationId);

                var org = result.Organizations.Single();
                org.OrganizationId.Should().Be(allocationAmountsFact1.OrganizationId);
                org.WaterAllocations.Should().HaveCount(2)
                    .And.Contain(a => a.AllocationAmountId == allocationAmountsFact1.AllocationAmountId)
                    .And.Contain(a => a.AllocationAmountId == allocationAmountsFact2.AllocationAmountId);

                org.Sites.Should().HaveCount(1)
                .And.Contain(a => a.SiteUUID == allocationSite.SiteUuid)
                .And.Contain(a => a.SiteName == allocationSite.SiteName)
                .And.Contain(a => a.SiteTypeCV == allocationSite.SiteTypeCv);

                org.Sites[0].RelatedPODSites.Should().HaveCount(1)
                    .And.Contain(a => a.SiteUUID == podSite.SiteUuid);
                org.Sites[0].RelatedPOUSites.Should().HaveCount(1)
                    .And.Contain(a => a.SiteUUID == pouSite.SiteUuid);
            }
            else
            {
                result.Organizations.Should().HaveCount(2)
                    .And.Contain(a => a.OrganizationId == organization1.OrganizationId)
                    .And.Contain(a => a.OrganizationId == organization2.OrganizationId);

                var org1 = result.Organizations.Single(a=> a.OrganizationId == organization1.OrganizationId);
                org1.OrganizationId.Should().Be(allocationAmountsFact1.OrganizationId);
                org1.WaterAllocations.Should().HaveCount(1)
                    .And.Contain(a => a.AllocationAmountId == allocationAmountsFact1.AllocationAmountId);

                org1.Sites.Should().HaveCount(1)
                .And.Contain(a => a.SiteUUID == allocationSite.SiteUuid);

                org1.Sites[0].RelatedPODSites.Should().HaveCount(1)
                    .And.Contain(a => a.SiteUUID == podSite.SiteUuid);
                org1.Sites[0].RelatedPOUSites.Should().HaveCount(1)
                    .And.Contain(a => a.SiteUUID == pouSite.SiteUuid);

                var org2 = result.Organizations.Single(a => a.OrganizationId == organization2.OrganizationId);
                org2.OrganizationId.Should().Be(allocationAmountsFact2.OrganizationId);
                org2.WaterAllocations.Should().HaveCount(1)
                    .And.Contain(a => a.AllocationAmountId == allocationAmountsFact2.AllocationAmountId);

                org2.Sites.Should().HaveCount(1)
                    .And.Contain(a => a.SiteUUID == allocationSite.SiteUuid);

                org2.Sites[0].RelatedPODSites.Should().HaveCount(1)
                    .And.Contain(a => a.SiteUUID == podSite.SiteUuid);
                org2.Sites[0].RelatedPOUSites.Should().HaveCount(1)
                    .And.Contain(a => a.SiteUUID == pouSite.SiteUuid);
            }
        }

        [DataTestMethod]
        [DataRow(null, null, null, true)]
        [DataRow(null, "POINT(-96.7014 40.8146)", null, true)]
        [DataRow(null, null, "POINT(-96.7014 40.8146)", false)]
        [DataRow(null, "POINT(-96.7014 40.8146)", "POINT(-96.7014 40.8146)", true)] //same point
        [DataRow(null, "POINT(-96.7014 40.8146)", "POINT(-96.7008 40.8147)", false)] //different points
        [DataRow(null, "POINT(-96.7014 40.8146)", "POINT(-96.7014 40.8147)", false)] //different long
        [DataRow(null, "POINT(-96.7014 40.8146)", "POINT(-96.7008 40.8146)", false)] //different lat
        [DataRow(null, "POINT(-96.7014 40.8146)", "POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", true)] //inside
        [DataRow(null, "POINT(-96.7014 40.8146)", "POLYGON((-96.7013 40.8147,-96.7012 40.8147,-96.7012 40.8146,-96.7013 40.8146,-96.7013 40.8147))", false)] //outside
        [DataRow(null, "POINT(-96.7014 40.8146)", "POLYGON((-96.7014 40.8146,-96.7012 40.8146,-96.7012 40.8145,-96.7014 40.8145,-96.7014 40.8146))", true)] //corner
        [DataRow(null, "POINT(-96.7014 40.8146)", "POLYGON((-96.7014 40.8147,-96.7012 40.8147,-96.7012 40.8145,-96.7014 40.8145,-96.7014 40.8147))", true)] //side
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", null, null, true)]
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", null, "POINT(-96.7016 40.8147)", false)] //outside
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", null, "POINT(-96.7014 40.8147)", true)] //inside
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", null, "POINT(-96.7012 40.8146)", true)] //corner
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", null, "POINT(-96.7013 40.8149)", true)] //side
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", null, "POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", true)] //same
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", null, "POLYGON((-96.7113 40.8147,-96.7112 40.8147,-96.7112 40.8146,-96.7113 40.8146,-96.7113 40.8147))", false)] //no overlap
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", null, "POLYGON((-96.7014 40.8148,-96.7013 40.8148,-96.7013 40.8147,-96.7014 40.8147,-96.7014 40.8148))", true)] //sub shape
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", null, "POLYGON((-96.7016 40.8150,-96.7011 40.8150,-96.7011 40.8145,-96.7016 40.8145,-96.7016 40.8150))", true)] //bigger shape
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", null, "POLYGON((-96.7016 40.8148,-96.7013 40.8148,-96.7013 40.8145,-96.7016 40.8145,-96.7016 40.8148))", true)] //partial overlap
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", null, "POLYGON((-96.7018 40.8149,-96.7015 40.8149,-96.7015 40.8146,-96.7018 40.8146,-96.7018 40.8149))", true)] //share side
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", null, "POLYGON((-96.7018 40.8152,-96.7015 40.8152,-96.7015 40.8149,-96.7018 40.8149,-96.7018 40.8152))", true)] //share corner
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", "POINT(-96.7008 40.8146)", "POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", true)] //match geo not point
        [DataRow("POLYGON((-96.7113 40.8147,-96.7112 40.8147,-96.7112 40.8146,-96.7113 40.8146,-96.7113 40.8147))", "POINT(-96.7014 40.8146)", "POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", true)] //match point not geo
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", "POINT(-96.7014 40.8146)", "POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", true)] //match both
        [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", "POINT(-96.7014 40.8146)", "POLYGON((-96.7113 40.8147,-96.7112 40.8147,-96.7112 40.8146,-96.7113 40.8146,-96.7113 40.8147))", false)] //match neither
        public async Task GetSiteAllocationAmountsAsync_Filters_Geometry(string geometry, string sitePoint, string filterGeometry, bool shouldMatch)
        {
            var configuration = Configuration.GetConfiguration();
            AllocationAmountsFact allocationAmountsFact;
            SitesDim siteDim;
            using (var db = new WaDEContext(configuration))
            {
                allocationAmountsFact = await AllocationAmountsFactBuilder.Load(db);

                siteDim = await SitesDimBuilder.Load(db);

                siteDim.SitePoint = GeometryExtensions.GetGeometryByWkt(sitePoint);
                siteDim.Geometry = GeometryExtensions.GetGeometryByWkt(geometry);

                await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
                {
                    AllocationAmountsFact = allocationAmountsFact,
                    SitesDim = siteDim
                });
            }

            var filters = new SiteAllocationAmountsFilters
            {
                Geometry = GeometryExtensions.GetGeometryByWkt(filterGeometry)
            };

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.GetSiteAllocationAmountsAsync(filters, 0, int.MaxValue);

            if (shouldMatch)
            {
                result.TotalWaterAllocationsCount.Should().Be(1);
                result.Organizations.Should().HaveCount(1);

                var org = result.Organizations.Single();
                org.OrganizationId.Should().Be(allocationAmountsFact.OrganizationId);
                org.WaterAllocations.Should().HaveCount(1);
                org.WaterAllocations[0].AllocationAmountId.Should().Be(allocationAmountsFact.AllocationAmountId);
            }
            else
            {
                result.TotalWaterAllocationsCount.Should().Be(0);
                result.Organizations.Should().HaveCount(0);
            }
        }

        [DataTestMethod]
        [DataRow("2022-02-24", null, null, true)]
        [DataRow("2022-02-24", "2022-02-23", null, true)]
        [DataRow("2022-02-24", "2022-02-24", null, true)]
        [DataRow("2022-02-24", "2022-02-25", null, false)]
        [DataRow("2022-02-24", null, "2022-02-23", false)]
        [DataRow("2022-02-24", null, "2022-02-24", true)]
        [DataRow("2022-02-24", null, "2022-02-25", true)]
        [DataRow("2022-02-22", "2022-02-23", "2022-02-25", false)]
        [DataRow("2022-02-23", "2022-02-23", "2022-02-25", true)]
        [DataRow("2022-02-24", "2022-02-23", "2022-02-25", true)]
        [DataRow("2022-02-25", "2022-02-23", "2022-02-25", true)]
        [DataRow("2022-02-26", "2022-02-23", "2022-02-25", false)]
        public async Task GetSiteAllocationAmountsAsync_Filters_DataPublicationDate(string dataPublicationDate, string startDataPublicationDate, string endDataPublicationDate, bool shouldMatch)
        {
            var configuration = Configuration.GetConfiguration();
            AllocationAmountsFact allocationAmountsFact;
            DateDim publicationDate;
            using (var db = new WaDEContext(configuration))
            {
                publicationDate = await DateDimBuilder.Load(db);
                publicationDate.Date = DateTime.Parse(dataPublicationDate);

                allocationAmountsFact = await AllocationAmountsFactBuilder.Load(db, new AllocationAmountsFactBuilderOptions
                {
                    DataPublicationDate = publicationDate
                });
            }

            var filters = new SiteAllocationAmountsFilters
            {
                StartDataPublicationDate = startDataPublicationDate == null ? null : DateTime.Parse(startDataPublicationDate),
                EndDataPublicationDate = endDataPublicationDate == null ? null : DateTime.Parse(endDataPublicationDate),
            };

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.GetSiteAllocationAmountsAsync(filters, 0, int.MaxValue);

            if (shouldMatch)
            {
                result.TotalWaterAllocationsCount.Should().Be(1);
                result.Organizations.Should().HaveCount(1);

                var org = result.Organizations.Single();
                org.OrganizationId.Should().Be(allocationAmountsFact.OrganizationId);
                org.WaterAllocations.Should().HaveCount(1);
                org.WaterAllocations[0].AllocationAmountId.Should().Be(allocationAmountsFact.AllocationAmountId);
            }
            else
            {
                result.TotalWaterAllocationsCount.Should().Be(0);
                result.Organizations.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsAsync_Paging()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 15; i++)
                {
                    await AllocationAmountsFactBuilder.Load(db);
                }
            }

            var filters = new SiteAllocationAmountsFilters();

            var alreadyRetrieved = new List<string>();

            for (var i = 1; i <= 5; i++)
            {
                var sut = CreateWaterAllocationAccessor();
                var pagedResults = await sut.GetSiteAllocationAmountsAsync(filters, Utility.NthTriangle(i - 1), i);
                pagedResults.TotalWaterAllocationsCount.Should().Be(15);
                var waterAllocations = pagedResults.Organizations.SelectMany(a => a.WaterAllocations).Select(a => a.AllocationNativeID).ToList();
                waterAllocations.Should().HaveCount(i);
                foreach (var waterAllocation in waterAllocations)
                {
                    alreadyRetrieved.Should().NotContain(waterAllocation);
                    alreadyRetrieved.Add(waterAllocation);
                }
            }

            alreadyRetrieved.Should().OnlyHaveUniqueItems().And.HaveCount(15);
        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsAsync_Paging_Consistency()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 15; i++)
                {
                    await AllocationAmountsFactBuilder.Load(db);
                }
            }

            var filters = new SiteAllocationAmountsFilters();

            var sut = CreateWaterAllocationAccessor();
            var baseResults = (await sut.GetSiteAllocationAmountsAsync(filters, 0, 15)).Organizations.SelectMany(a => a.WaterAllocations).Select(a => a.AllocationNativeID).ToList();

            for (var i = 0; i < 14; i++)
            {
                var pagedResults = await sut.GetSiteAllocationAmountsAsync(filters, i, 2);
                var waterAllocations = pagedResults.Organizations.SelectMany(a => a.WaterAllocations).Select(a => a.AllocationNativeID).ToList();
                waterAllocations.Should().HaveCount(2);
                waterAllocations.Should().BeEquivalentTo(baseResults.Skip(i).Take(2));
            }
        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsAsync_Paging_RequestMoreThanExists()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 3; i++)
                {
                    await AllocationAmountsFactBuilder.Load(db);
                }
            }

            var filters = new SiteAllocationAmountsFilters();

            var sut = CreateWaterAllocationAccessor();
            var pagedResults = await sut.GetSiteAllocationAmountsAsync(filters, 1, 10);
            pagedResults.TotalWaterAllocationsCount.Should().Be(3);
            var waterAllocations = pagedResults.Organizations.SelectMany(a => a.WaterAllocations).Select(a => a.AllocationNativeID).ToList();
            waterAllocations.Should().HaveCount(2);
        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsAsync_Paging_RequestAfterLast()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 3; i++)
                {
                    await AllocationAmountsFactBuilder.Load(db);
                }
            }

            var filters = new SiteAllocationAmountsFilters();

            var sut = CreateWaterAllocationAccessor();
            var pagedResults = await sut.GetSiteAllocationAmountsAsync(filters, 4, 10);
            pagedResults.TotalWaterAllocationsCount.Should().Be(3);
            var waterAllocations = pagedResults.Organizations.SelectMany(a => a.WaterAllocations).Select(a => a.AllocationNativeID).ToList();
            waterAllocations.Should().HaveCount(0);
        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsDigestAsync_NoFilters()
        {
            var configuration = Configuration.GetConfiguration();
            AllocationAmountsFact allocationAmountsFact;
            using (var db = new WaDEContext(configuration))
            {
                allocationAmountsFact = await AllocationAmountsFactBuilder.Load(db);

                allocationAmountsFact.AllocationAmountId.Should().NotBe(0);
            }

            var filters = new SiteAllocationAmountsDigestFilters();

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.GetSiteAllocationAmountsDigestAsync(filters, 0, int.MaxValue);

            result.Count().Should().Be(1);
            result.First().Sites.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsDigestAsync_Paging()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 15; i++)
                {
                    await AllocationAmountsFactBuilder.Load(db);
                }
            }

            var filters = new SiteAllocationAmountsDigestFilters();

            var alreadyRetrieved = new List<long>();

            for (var i = 1; i <= 5; i++)
            {
                var sut = CreateWaterAllocationAccessor();
                var waterAllocations = (await sut.GetSiteAllocationAmountsDigestAsync(filters, Utility.NthTriangle(i - 1), i)).Select(a => a.AllocationAmountId).ToList();
                waterAllocations.Should().HaveCount(i);
                foreach (var waterAllocation in waterAllocations)
                {
                    alreadyRetrieved.Should().NotContain(waterAllocation);
                    alreadyRetrieved.Add(waterAllocation);
                }
            }

            alreadyRetrieved.Should().OnlyHaveUniqueItems().And.HaveCount(15);
        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsDigestAsync_Paging_Consistency()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 15; i++)
                {
                    await AllocationAmountsFactBuilder.Load(db);
                }
            }

            var filters = new SiteAllocationAmountsDigestFilters();

            var sut = CreateWaterAllocationAccessor();
            var baseResults = (await sut.GetSiteAllocationAmountsDigestAsync(filters, 0, 15)).Select(a => a.AllocationAmountId).ToList();

            for (var i = 0; i < 14; i++)
            {
                var pagedResults = await sut.GetSiteAllocationAmountsDigestAsync(filters, i, 2);
                var waterAllocations = pagedResults.Select(a => a.AllocationAmountId).ToList();
                waterAllocations.Should().HaveCount(2);
                waterAllocations.Should().BeEquivalentTo(baseResults.Skip(i).Take(2));
            }
        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsDigestAsync_Paging_RequestMoreThanExists()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 3; i++)
                {
                    await AllocationAmountsFactBuilder.Load(db);
                }
            }

            var filters = new SiteAllocationAmountsDigestFilters();

            var sut = CreateWaterAllocationAccessor();
            var waterAllocations = (await sut.GetSiteAllocationAmountsDigestAsync(filters, 1, 10)).Select(a => a.AllocationAmountId).ToList();
            waterAllocations.Should().HaveCount(2);

        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsDigestAsync_Paging_RequestAfterLast()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 3; i++)
                {
                    await AllocationAmountsFactBuilder.Load(db);
                }
            }

            var filters = new SiteAllocationAmountsDigestFilters();

            var sut = CreateWaterAllocationAccessor();
            var waterAllocations = (await sut.GetSiteAllocationAmountsDigestAsync(filters, 4, 10)).Select(a => a.AllocationAmountId).ToList();
            waterAllocations.Should().HaveCount(0);

        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsDigestAsync_NoFilters_WithSites()
        {
            var configuration = Configuration.GetConfiguration();
            AllocationAmountsFact allocationAmountsFact;
            AllocationBridgeSitesFact allocationBridgeSitesFact;
            using (var db = new WaDEContext(configuration))
            {
                allocationAmountsFact = await AllocationAmountsFactBuilder.Load(db);

                allocationAmountsFact.AllocationAmountId.Should().NotBe(0);

                allocationBridgeSitesFact = await AllocationBridgeSitesFactBuilder.Load(db,
                    new AllocationBridgeSitesFactBuilderOptions
                    {
                        AllocationAmountsFact = allocationAmountsFact
                    });

                allocationBridgeSitesFact.AllocationBridgeId.Should().NotBe(0);
            }

            var filters = new SiteAllocationAmountsDigestFilters();

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.GetSiteAllocationAmountsDigestAsync(filters, 0, int.MaxValue);

            result.Count().Should().Be(1);
            result.First().Sites.Should().NotBeEmpty();
            result.First().Sites.Count().Should().Be(1);
        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsDigestAsync_NoFilters_NoMatch()
        {
            var filters = new SiteAllocationAmountsDigestFilters();

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.GetSiteAllocationAmountsDigestAsync(filters, 0, int.MaxValue);

            result.Count().Should().Be(0);
        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsDigestAsync_NoFilters_WithSites_Many()
        {
            var configuration = Configuration.GetConfiguration();
            var count = 0;

            AllocationAmountsFact allocationAmountsFact;
            AllocationBridgeSitesFact allocationBridgeSitesFact;
            using (var db = new WaDEContext(configuration))
            {
                while (count++ < 10)
                {
                    allocationAmountsFact = await AllocationAmountsFactBuilder.Load(db);

                    allocationAmountsFact.AllocationAmountId.Should().NotBe(0);

                    allocationBridgeSitesFact = await AllocationBridgeSitesFactBuilder.Load(db,
                        new AllocationBridgeSitesFactBuilderOptions
                        {
                            AllocationAmountsFact = allocationAmountsFact
                        });

                    allocationBridgeSitesFact.AllocationBridgeId.Should().NotBe(0);
                }
            }

            var filters = new SiteAllocationAmountsDigestFilters();

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.GetSiteAllocationAmountsDigestAsync(filters, 0, int.MaxValue);

            result.Count().Should().Be(count - 1);

            result.ElementAt(6).Sites.Should().NotBeEmpty();
            result.ElementAt(6).Sites.Count().Should().Be(1);
        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsDigestAsync_OrganizationUuidFilter_WithSite()
        {
            var configuration = Configuration.GetConfiguration();
            AllocationAmountsFact allocationAmountsFact;
            SitesDim site;
            using (var db = new WaDEContext(configuration))
            {
                allocationAmountsFact = await AllocationAmountsFactBuilder.Load(db);
                allocationAmountsFact.AllocationAmountId.Should().NotBe(0);

                site = await SitesDimBuilder.Load(db);

                await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
                {
                    AllocationAmountsFact = allocationAmountsFact,
                    SitesDim = site
                });
            }

            var filters = new SiteAllocationAmountsDigestFilters
            {
                OrganizationUUID = allocationAmountsFact.Organization.OrganizationUuid
            };

            var sut = CreateWaterAllocationAccessor();
            var result = (await sut.GetSiteAllocationAmountsDigestAsync(filters, 0, int.MaxValue)).ToList();

            result.Should().HaveCount(1);
            result.First().Sites.Should().HaveCount(1);
            result.First().Sites.First().SiteUUID.Should().Be(site.SiteUuid);
        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsDigestAsync_OrganizationUuidFilter_NoMatch()
        {
            var configuration = Configuration.GetConfiguration();
            AllocationAmountsFact allocationAmountsFact;
            using (var db = new WaDEContext(configuration))
            {
                allocationAmountsFact = await AllocationAmountsFactBuilder.Load(db);
                allocationAmountsFact.AllocationAmountId.Should().NotBe(0);
            }

            var filters = new SiteAllocationAmountsDigestFilters
            {
                OrganizationUUID = new Faker().Random.AlphaNumeric(10)
            };

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.GetSiteAllocationAmountsDigestAsync(filters, 0, int.MaxValue);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetSiteAllocationAmountsDigestAsync_OrganizationUuidFilter_NoSites()
        {
            var configuration = Configuration.GetConfiguration();
            AllocationAmountsFact allocationAmountsFact;
            using (var db = new WaDEContext(configuration))
            {
                allocationAmountsFact = await AllocationAmountsFactBuilder.Load(db);

                allocationAmountsFact.AllocationAmountId.Should().NotBe(0);
            }

            var filters = new SiteAllocationAmountsDigestFilters
            {
                OrganizationUUID = allocationAmountsFact.Organization.OrganizationUuid
            };

            var sut = CreateWaterAllocationAccessor();
            var result = (await sut.GetSiteAllocationAmountsDigestAsync(filters, 0, int.MaxValue)).ToList();

            result.Should().HaveCount(1);
            result.First().Sites.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("2022-02-24", null, null, true)]
        [DataRow("2022-02-24", "2022-02-23", null, true)]
        [DataRow("2022-02-24", "2022-02-24", null, true)]
        [DataRow("2022-02-24", "2022-02-25", null, false)]
        [DataRow("2022-02-24", null, "2022-02-23", false)]
        [DataRow("2022-02-24", null, "2022-02-24", true)]
        [DataRow("2022-02-24", null, "2022-02-25", true)]
        [DataRow("2022-02-22", "2022-02-23", "2022-02-25", false)]
        [DataRow("2022-02-23", "2022-02-23", "2022-02-25", true)]
        [DataRow("2022-02-24", "2022-02-23", "2022-02-25", true)]
        [DataRow("2022-02-25", "2022-02-23", "2022-02-25", true)]
        [DataRow("2022-02-26", "2022-02-23", "2022-02-25", false)]
        public async Task GetSiteAllocationAmountsDigestAsync_Filters_DataPublicationDate(string dataPublicationDate, string startDataPublicationDate, string endDataPublicationDate, bool shouldMatch)
        {
            var configuration = Configuration.GetConfiguration();
            AllocationAmountsFact allocationAmountsFact;
            DateDim publicationDate;
            using (var db = new WaDEContext(configuration))
            {
                publicationDate = await DateDimBuilder.Load(db);
                publicationDate.Date = DateTime.Parse(dataPublicationDate);

                allocationAmountsFact = await AllocationAmountsFactBuilder.Load(db, new AllocationAmountsFactBuilderOptions
                {
                    DataPublicationDate = publicationDate
                });
            }

            var filters = new SiteAllocationAmountsDigestFilters
            {
                StartDataPublicationDate = startDataPublicationDate == null ? null : DateTime.Parse(startDataPublicationDate),
                EndDataPublicationDate = endDataPublicationDate == null ? null : DateTime.Parse(endDataPublicationDate),
            };

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.GetSiteAllocationAmountsDigestAsync(filters, 0, int.MaxValue);

            if (shouldMatch)
            {
                result.Should().HaveCount(1);
                result.Single().AllocationAmountId.Should().Be(allocationAmountsFact.AllocationAmountId);
            }
            else
            {
                result.Should().BeEmpty();
            }
        }
        
        [TestMethod]
        public async Task GetAllocationMetadata_ReturnsIntervalStartDate()
        {
            await using var db = new WaDEContext(Configuration.GetConfiguration());
            List<AllocationAmountsFact> timeSeries = new();
            for (var i = 0; i < 3; i++)
            {
                timeSeries.Add(await AllocationAmountsFactBuilder.Load(db));
            }

            var response = await CreateWaterAllocationAccessor().GetAllocationMetadata();
            response.IntervalStartDate.Should().BeSameDateAs(timeSeries.MinBy(ts => ts.AllocationPriorityDateNavigation.Date).AllocationPriorityDateNavigation.Date);
            response.IntervalEndDate.Should().BeNull();
        }

        private IWaterAllocationAccessor CreateWaterAllocationAccessor()
        {
            return new WaterAllocationAccessor(
                Configuration.GetConfiguration(), 
                LoggerFactory,
                Mock.Create<IAccessorRequestHandlerResolver>());
        }
    }
}