using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests
{
    [TestClass]
    public class ApiWaterAllocationAccessorTests : DbTestBase
    {
        private readonly ILoggerFactory LoggerFactory = new LoggerFactory();

        [TestMethod]
        public async Task GetSiteAllocationAmountsAsync_NoFilters()
        {
            var configuration = Configuration.GetConfiguration();
            AllocationAmountsFact allocationAmountsFact;
            using (var db = new WaDEContext(configuration))
            {
                allocationAmountsFact = await AllocationAmountsFactBuilder.Load(db);

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
                var waterAllocations = pagedResults.Organizations.SelectMany(a => a.WaterAllocations).Select(a => a.WaterSourceUUID).ToList();
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
            var baseResults = (await sut.GetSiteAllocationAmountsAsync(filters, 0, 15)).Organizations.SelectMany(a => a.WaterAllocations).Select(a => a.WaterSourceUUID).ToList();

            for (var i = 0; i < 14; i++)
            {
                var pagedResults = await sut.GetSiteAllocationAmountsAsync(filters, i, 2);
                var waterAllocations = pagedResults.Organizations.SelectMany(a => a.WaterAllocations).Select(a => a.WaterSourceUUID).ToList();
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
            var waterAllocations = pagedResults.Organizations.SelectMany(a => a.WaterAllocations).Select(a => a.WaterSourceUUID).ToList();
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
            var waterAllocations = pagedResults.Organizations.SelectMany(a => a.WaterAllocations).Select(a => a.WaterSourceUUID).ToList();
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

        private IWaterAllocationAccessor CreateWaterAllocationAccessor()
        {
            return new WaterAllocationAccessor(Configuration.GetConfiguration(), LoggerFactory);
        }
    }
}