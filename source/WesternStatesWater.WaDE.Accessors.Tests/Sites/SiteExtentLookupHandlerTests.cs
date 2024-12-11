using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Sites.Requests;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Sites.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests.Sites;

[TestClass]
public class SiteExtentLookupHandlerTests : DbTestBase
{
    private static IConfiguration _configuration = Configuration.GetConfiguration();

    [TestMethod]
    public async Task SiteExtentLookup_EmptyRequest_UnitedStatesBoundaryBox()
    {
        // Act
        var request = new SiteExtentSearchRequest();
        var response = await CreateHandler().Handle(request);

        // Assert
        response.Should().NotBeNull();
        response.BoundaryBox.MinX.Should().Be(-125);
        response.BoundaryBox.MaxX.Should().Be(32);
        response.BoundaryBox.MinY.Should().Be(-100);
        response.BoundaryBox.MaxY.Should().Be(49);
        response.TimeframeStart.Should().BeNull();
        response.TimeframeEnd.Should().BeNull();
    }

    [TestMethod]
    public async Task SiteExtentLookup_WithSiteVariableAmountsFact_ReturnsTimeframeStartAndEnd()
    {
        // Arrange
        List<SiteVariableAmountsFact> timeSeries = new List<SiteVariableAmountsFact>();
        await using var db = new WaDEContext(_configuration);
        for (var i = 0; i < 2; i++)
        {
            timeSeries.Add(await SiteVariableAmountsFactBuilder.Load(db));
        }

        // Act
        var request = new SiteExtentSearchRequest();
        var response = await CreateHandler().Handle(request);

        // Assert
        response.Should().NotBeNull();
        response.TimeframeStart.Should().BeSameDateAs(timeSeries.Select(x => x.TimeframeStartNavigation.Date).Min());
        response.TimeframeEnd.Should().BeSameDateAs(timeSeries.Select(x => x.TimeframeEndNavigation.Date).Max());
    }

    private static SiteExtentLookupHandler CreateHandler()
    {
        return new SiteExtentLookupHandler(_configuration);
    }
}