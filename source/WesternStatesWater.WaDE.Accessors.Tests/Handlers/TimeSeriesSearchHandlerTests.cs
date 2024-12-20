using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests.Handlers;

[TestClass]
public class TimeSeriesSearchHandlerTests : DbTestBase
{
    [TestMethod]
    public async Task Handler_LimitSet_ReturnsCorrectAmount()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        for (var i = 0; i < 5; i++)
        {
            await SiteVariableAmountsFactBuilder.Load(db);
        }

        var request = new TimeSeriesSearchRequest
        {
            Limit = 3
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.TimeSeries.Should().HaveCount(3);
    }

    [TestMethod]
    public async Task Handler_LastKeySet_PagesNextSet()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        List<SiteVariableAmountsFact> timeSeries = new();
        for (var i = 0; i < 10; i++)
        {
            timeSeries.Add(await SiteVariableAmountsFactBuilder.Load(db));
        }

        var sortedTimeSeries = timeSeries.OrderBy(x => x.SiteVariableAmountId).ToList();

        var request = new TimeSeriesSearchRequest
        {
            Limit = 3,
            LastKey = sortedTimeSeries[2].SiteVariableAmountId
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.TimeSeries.Should().HaveCount(3);
        response.TimeSeries.Select(a => a.SiteVariableAmountId).Should().BeEquivalentTo(
            sortedTimeSeries.Skip(3).Take(3).Select(a => a.SiteVariableAmountId));
    }

    [TestMethod]
    public async Task Handler_FilerSiteUuids_ReturnsRequestedTimeSeries()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var siteA = await SitesDimBuilder.Load(db);
        var siteB = await SitesDimBuilder.Load(db);
        await SitesDimBuilder.Load(db);

        var timeSeriesA = await SiteVariableAmountsFactBuilder.Load(db, new SiteVariableAmountsFactBuilderOptions
        {
            SiteDim = siteA
        });

        var timeSeriesB = await SiteVariableAmountsFactBuilder.Load(db, new SiteVariableAmountsFactBuilderOptions
        {
            SiteDim = siteB
        });

        await SiteVariableAmountsFactBuilder.Load(db);

        var request = new TimeSeriesSearchRequest
        {
            SiteUuids = [siteA.SiteUuid, siteB.SiteUuid],
            Limit = 10
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.TimeSeries.Should().HaveCount(2);
        response.TimeSeries.Select(a => a.SiteVariableAmountId).Should().BeEquivalentTo(
            [timeSeriesA.SiteVariableAmountId, timeSeriesB.SiteVariableAmountId]
        );
    }

    [TestMethod]
    [DataRow(null, null, 1)]
    [DataRow("4042-01-01", null, 1)]
    [DataRow(null, "4042-03-01", 1)]
    [DataRow("4042-01-01", "4042-12-31", 1)]
    [DataRow("4042-02-15", "4042-03-15", 1)]
    [DataRow("4042-01-15", "4042-02-15", 1)]
    public async Task Handler_FilterDates_ReturnsTimeSeriesBetweenRanges(string startDate, string endDate,
        int expectedCount)
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        await SiteVariableAmountsFactBuilder.Load(db, new SiteVariableAmountsFactBuilderOptions
        {
            TimeframeStart = await DateDimBuilder.Load(db, new DateDimBuilderOptions
            {
                Date = DateTime.Parse("4042-02-01", CultureInfo.InvariantCulture)
            }),
            TimeframeEnd = await DateDimBuilder.Load(db, new DateDimBuilderOptions
            {
                Date = DateTime.Parse("4042-02-28", CultureInfo.InvariantCulture)
            })
        });

        var request = new TimeSeriesSearchRequest
        {
            StartDate = startDate != null ? DateTime.Parse(startDate) : null,
            EndDate = endDate != null ? DateTime.Parse(endDate) : null,
            Limit = 10
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.TimeSeries.Should().HaveCount(expectedCount);
    }

    private async Task<TimeSeriesSearchResponse> ExecuteHandler(TimeSeriesSearchRequest request)
    {
        var handler = new TimeSeriesSearchHandler(Configuration.GetConfiguration());
        return await handler.Handle(request);
    }
}