using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Integration.Tests.WaterResources;

[TestClass]
public class AggregatedAmountsIntegrationTests : IntegrationTestsBase
{
    private IAggregatedAmountsManager _manager = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _manager = Services.GetRequiredService<IAggregatedAmountsManager>();
    }

    [DataTestMethod]
    [DataRow(1, 0, 1, 1, DisplayName = "Should return the total count requested (1)")]
    [DataRow(3, 0, 3, 3, DisplayName = "Should return the total count requested (3)")]
    [DataRow(2, 0, 1, 1, DisplayName = "Should return the max records if less than requested")]
    [DataRow(4, 3, 10, 1, DisplayName = "Should start at the requested index")]
    [DataRow(5, 6, 1, 0, DisplayName = "Should return no records if start index is greater than total count")]
    public async Task Load_AggregatedAmountsSearchRequest_PagingSmokeTests(
        int saveCount,
        int requestStartIndex,
        int requestCount,
        int expectedResultCount
    )
    {
        var facts = await SaveAggregatedAmountsFacts(saveCount);

        var request = new AggregatedAmountsSearchRequest
        {
            Filters = new AggregatedAmountsFilters(),
            StartIndex = requestStartIndex,
            RecordCount = requestCount
        };

        var response = await _manager.Load(request);

        // The total is always the total number of facts saved.
        response.AggregatedAmounts.TotalAggregatedAmountsCount.Should().Be(saveCount);

        // The number of organizations returned should match the record count requested.
        response.AggregatedAmounts.Organizations.Should().HaveCount(expectedResultCount);

        // The aggregated amounts should be returned in the order they were saved (i.e., ordered on ID).
        var expectedLastOrgName = facts
            .ElementAtOrDefault(requestStartIndex + expectedResultCount - 1)?
            .Organization
            .OrganizationName;

        response.AggregatedAmounts
            .Organizations
            .LastOrDefault()?
            .OrganizationName
            .Should()
            .Be(expectedLastOrgName);
    }

    private async Task<EF.AggregatedAmountsFact[]> SaveAggregatedAmountsFacts(int count)
    {
        await using var db = new EF.WaDEContext(Services.GetRequiredService<IConfiguration>());

        var facts = new List<EF.AggregatedAmountsFact>(count);

        for (var i = 0; i < count; i++)
        {
            var fact = await AggregatedAmountsFactBuilder.Load(db);
            facts.Add(fact);
        }

        return facts.ToArray();
    }
}