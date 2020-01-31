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
    public class ApiSiteVariableAmountsAccessorTests : DbTestBase
    {
        private readonly ILoggerFactory LoggerFactory = new LoggerFactory();

        [TestMethod]
        public async Task GetAggregatedAmountsAsync_NoFilters()
        {
            var configuration = Configuration.GetConfiguration();
            SiteVariableAmountsFact siteVariableAmountsFact;
            using (var db = new WaDEContext(configuration))
            {
                siteVariableAmountsFact = await SiteVariableAmountsFactBuilder.Load(db);

                siteVariableAmountsFact.SiteVariableAmountId.Should().NotBe(0);
            }

            var filters = new SiteVariableAmountsFilters();

            var sut = CreateSiteVariableAmountsAccessor();
            var result = await sut.GetSiteVariableAmountsAsync(filters, 0, int.MaxValue);

            result.TotalSiteVariableAmountsCount.Should().Be(1);
            result.Organizations.Should().HaveCount(1);

            var org = result.Organizations.Single();
            org.OrganizationId.Should().Be(siteVariableAmountsFact.OrganizationId);
            org.SiteVariableAmounts.Should().HaveCount(1);
            org.SiteVariableAmounts[0].SiteVariableAmountId.Should().Be(siteVariableAmountsFact.SiteVariableAmountId);
        }

        [TestMethod]
        public async Task GetAggregatedAmountsAsync_Paging()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 15; i++)
                {
                    await SiteVariableAmountsFactBuilder.Load(db);
                }
            }

            var filters = new SiteVariableAmountsFilters();

            var alreadyRetrieved = new List<long>();

            for (var i = 1; i <= 5; i++)
            {
                var sut = CreateSiteVariableAmountsAccessor();
                var pagedResults = await sut.GetSiteVariableAmountsAsync(filters, Utility.NthTriangle(i - 1), i);
                pagedResults.TotalSiteVariableAmountsCount.Should().Be(15);
                var waterAllocations = pagedResults.Organizations.SelectMany(a => a.SiteVariableAmounts).Select(a => a.SiteVariableAmountId).ToList();
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
        public async Task GetAggregatedAmountsAsync_Paging_RequestMoreThanExists()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 3; i++)
                {
                    await SiteVariableAmountsFactBuilder.Load(db);
                }
            }

            var filters = new SiteVariableAmountsFilters();

            var sut = CreateSiteVariableAmountsAccessor();
            var pagedResults = await sut.GetSiteVariableAmountsAsync(filters, 1, 10);
            pagedResults.TotalSiteVariableAmountsCount.Should().Be(3);
            var waterAllocations = pagedResults.Organizations.SelectMany(a => a.SiteVariableAmounts).Select(a => a.SiteVariableAmountId).ToList();
            waterAllocations.Should().HaveCount(2);
        }

        [TestMethod]
        public async Task GetAggregatedAmountsAsync_Paging_RequestAfterLast()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 3; i++)
                {
                    await SiteVariableAmountsFactBuilder.Load(db);
                }
            }

            var filters = new SiteVariableAmountsFilters();

            var sut = CreateSiteVariableAmountsAccessor();
            var pagedResults = await sut.GetSiteVariableAmountsAsync(filters, 3, 10);
            pagedResults.TotalSiteVariableAmountsCount.Should().Be(3);
            var waterAllocations = pagedResults.Organizations.SelectMany(a => a.SiteVariableAmounts).Select(a => a.SiteVariableAmountId).ToList();
            waterAllocations.Should().HaveCount(0);
        }

        private ISiteVariableAmountsAccessor CreateSiteVariableAmountsAccessor()
        {
            return new SiteVariableAmountsAccessor(Configuration.GetConfiguration(), LoggerFactory);
        }
    }
}