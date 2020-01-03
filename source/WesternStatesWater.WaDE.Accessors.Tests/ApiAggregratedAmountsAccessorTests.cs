using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests
{
    [TestClass]
    public class AggregratedAmountsAccessorTests : DbTestBase
    {
        private readonly ILoggerFactory loggerFactory = new LoggerFactory();

        [TestMethod]
        public async Task GetAggregatedAmountsAsync_NoFilters()
        {
            var configuration = Configuration.GetConfiguration();
            AggregatedAmountsFact aggregatedAmount;
            using (var db = new WaDEContext(configuration))
            {
                aggregatedAmount = await AggregatedAmountsFactBuilder.Load(db);

                aggregatedAmount.AggregatedAmountId.Should().NotBe(0);
            }
            
            var filters = new AggregatedAmountsFilters();

            var sut = CreateAggregatedAmountsAccessor();
            var result = await sut.GetAggregatedAmountsAsync(filters, 0, int.MaxValue);

            result.TotalAggregatedAmountsCount.Should().Be(1);
            result.Organizations.Should().HaveCount(1);

            var org = result.Organizations.Single();
            org.OrganizationId.Should().Be(aggregatedAmount.OrganizationId);
            org.AggregatedAmounts.Should().HaveCount(1);
            org.AggregatedAmounts[0].AggregatedAmountId.Should().Be(aggregatedAmount.AggregatedAmountId);
        }

        private IAggregatedAmountsAccessor CreateAggregatedAmountsAccessor()
        {
            return new AggregratedAmountsAccessor(Configuration.GetConfiguration(), loggerFactory);
        }
    }

    [TestClass]
    public class WaterAllocationAccessorTests : DbTestBase
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

        private IWaterAllocationAccessor CreateWaterAllocationAccessor()
        {
            return new WaterAllocationAccessor(Configuration.GetConfiguration(), LoggerFactory);
        }
    }
}
