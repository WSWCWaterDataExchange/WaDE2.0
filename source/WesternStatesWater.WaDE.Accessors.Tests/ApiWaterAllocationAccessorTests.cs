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
        public async Task GetAggregatedAmountsAsync_NoFilters()
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
        public async Task GetAggregatedAmountsDygestAsync_NoFilters()
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
        public async Task GetAggregatedAmountsDygestAsync_NoFilters_WithSites()
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
        public async Task GetAggregatedAmountsDygestAsync_NoFilters_NoMatch()
        {
            var filters = new SiteAllocationAmountsDigestFilters();

            var sut = CreateWaterAllocationAccessor();
            var result = await sut.GetSiteAllocationAmountsDigestAsync(filters, 0, int.MaxValue);

            result.Count().Should().Be(0);            
        }

        [TestMethod]
        public async Task GetAggregatedAmountsDygestAsync_NoFilters_WithSites_Many()
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

            result.Count().Should().Be(count-1);

            result.ElementAt(6).Sites.Should().NotBeEmpty();
            result.ElementAt(6).Sites.Count().Should().Be(1);
        }

        private IWaterAllocationAccessor CreateWaterAllocationAccessor()
        {
            return new WaterAllocationAccessor(Configuration.GetConfiguration(), LoggerFactory);
        }
    }
}