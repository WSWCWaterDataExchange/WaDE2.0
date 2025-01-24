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
    [DataTestMethod]
    [DataRow(5)]
    [DataRow(10)]
    public async Task Handler_LimitSet_EndOfRecordsNoLastUuid(int limit)
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        
        for (var i = 0; i < 5; i++)
        {
            await SiteVariableAmountsFactBuilder.Load(db);
        }

        var request = new TimeSeriesSearchRequest
        {
            Limit = limit
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.TimeSeries.Should().HaveCount(5);
        response.LastUuid.Should().BeNull();
    }
    
    [TestMethod]
    public async Task Handler_LimitSet_ReturnsCorrectAmount()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        List<SiteVariableAmountsFact> dbTimeSeries = new();
        for (var i = 0; i < 5; i++)
        {
            dbTimeSeries.Add(await SiteVariableAmountsFactBuilder.Load(db));
        }

        var request = new TimeSeriesSearchRequest
        {
            Limit = 3
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.TimeSeries.Should().HaveCount(3);
        response.LastUuid.Should()
            .Be(dbTimeSeries.OrderBy(ts => ts.SiteVariableAmountId)
                .Select(ts => ts.SiteVariableAmountId)
                .ElementAt(3)
                .ToString());
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
    public async Task Handler_FilterSiteUuids_ReturnsRequestedTimeSeries()
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

    [TestMethod]
    public async Task Handler_FilterStates_ReturnsTimeSeriesThatContainSitesInState()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var stateA = await StateBuilder.Load(db);
        var stateB = await StateBuilder.Load(db);
        
        var siteA = await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            StateCVNavigation = stateA
        });
        
        var siteB = await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            StateCVNavigation = stateB
        });
        
        await SiteVariableAmountsFactBuilder.Load(db, new SiteVariableAmountsFactBuilderOptions
        {
            SiteDim = siteA
        });
        
        await SiteVariableAmountsFactBuilder.Load(db, new SiteVariableAmountsFactBuilderOptions
        {
            SiteDim = siteB
        });
        
        var request = new TimeSeriesSearchRequest
        {
            States = [stateA.Name],
            Limit = 10
        };
        
        var response = await ExecuteHandler(request);
        
        response.TimeSeries.Should().HaveCount(1);
        response.TimeSeries[0].SiteUUID.Should().BeEquivalentTo(siteA.SiteUuid);
        response.TimeSeries[0].State.Should().Be(stateA.Name);
    }
    
    [TestMethod]
    public async Task Handler_FilterVariableTypes_ReturnsTimeSeriesThatContainVariableTypes()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var variableA = await VariableBuilder.Load(db);
        var variableB = await VariableBuilder.Load(db);
        
        var variableDimA = await VariablesDimBuilder.Load(db, new VariablesDimBuilderOptions
        {
            Variable = variableA
        });
        
        var variableDimB = await VariablesDimBuilder.Load(db, new VariablesDimBuilderOptions
        {
            Variable = variableB
        });
        
        var siteA = await SitesDimBuilder.Load(db);
        var siteB = await SitesDimBuilder.Load(db);
        
        await SiteVariableAmountsFactBuilder.Load(db, new SiteVariableAmountsFactBuilderOptions
        {
            SiteDim = siteA,
            VariablesDim = variableDimA
        });
        
        await SiteVariableAmountsFactBuilder.Load(db, new SiteVariableAmountsFactBuilderOptions
        {
            SiteDim = siteB,
            VariablesDim = variableDimB
        });
        
        var request = new TimeSeriesSearchRequest
        {
            VariableTypes = [variableA.WaDEName],
            Limit = 10
        };
        
        var response = await ExecuteHandler(request);
        
        response.TimeSeries.Should().HaveCount(1);
        response.TimeSeries[0].VariableType.Should().Be(variableA.WaDEName);
    }
    
    [TestMethod]
    public async Task Handler_FilterWaterSourceTypes_ReturnsTimeSeriesThatContainWaterSourceTypes()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var waterSourceTypeA = await WaterSourceTypeBuilder.Load(db);
        var waterSourceTypeB = await WaterSourceTypeBuilder.Load(db);

        var waterSourceA = await WaterSourcesDimBuilder.Load(db, new WaterSourcesDimBuilderOptions
        {
            WaterSourceType = waterSourceTypeA
        });
        var waterSourceB = await WaterSourcesDimBuilder.Load(db, new WaterSourcesDimBuilderOptions
        {
            WaterSourceType = waterSourceTypeB
        });
        
        var siteA = await SitesDimBuilder.Load(db);
        
        await SiteVariableAmountsFactBuilder.Load(db, new SiteVariableAmountsFactBuilderOptions
        {
            SiteDim = siteA,
            WaterSourcesDim = waterSourceA
        });
        
        await SiteVariableAmountsFactBuilder.Load(db, new SiteVariableAmountsFactBuilderOptions
        {
            SiteDim = siteA,
            WaterSourcesDim = waterSourceB
        });
        
        var request = new TimeSeriesSearchRequest
        {
            WaterSourceTypes = [waterSourceTypeA.WaDEName],
            Limit = 10
        };
        
        var response = await ExecuteHandler(request);
        
        response.TimeSeries.Should().HaveCount(1);
        response.TimeSeries[0].WaterSource.SourceType.Should().Be(waterSourceTypeA.WaDEName);
    }

    private async Task<TimeSeriesSearchResponse> ExecuteHandler(TimeSeriesSearchRequest request)
    {
        var handler = new TimeSeriesSearchHandler(Configuration.GetConfiguration());
        return await handler.Handle(request);
    }
}