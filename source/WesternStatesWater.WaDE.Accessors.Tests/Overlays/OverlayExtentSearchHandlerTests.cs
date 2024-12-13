using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Geometries;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Overlays.Requests;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Overlays.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests.Overlays;

[TestClass]
public class OverlayExtentSearchHandlerTests : DbTestBase
{
    private static IConfiguration _configuration = Configuration.GetConfiguration();

    [TestMethod]
    public async Task OverlayExtentSearch_ReturnsSpatialExtentAndNoInterval()
    {
        await using var db = new WaDEContext(_configuration);

        // Arrange
        Coordinate[] coordinates =
        {
            new Coordinate(-2, 2),
            new Coordinate(2, 2),
            new Coordinate(2, -2),
            new Coordinate(-2, -2),
            new Coordinate(-2, 2)
        };
        GeometryFactory geometryFactory = new GeometryFactory();
        var polygon = geometryFactory.CreatePolygon(coordinates);
        var options = new ReportingUnitsDimBuilderOptions
        {
            Geometry = polygon
        };
        var dbReportingUnit = await ReportingUnitsDimBuilder.Load(db, options);
        var expectedBoundingBox = dbReportingUnit.Geometry.EnvelopeInternal;

        // Act
        var request = new OverlayExtentSearchRequest();
        var response = await CreateHandler().Handle(request);

        // Assert
        response.Should().NotBeNull();
        response.Extent.Spatial.Should().NotBeNull();
        response.Extent.Spatial.Bbox.Should().AllBeEquivalentTo(new[]
        {
            expectedBoundingBox.MinX,
            expectedBoundingBox.MinY,
            expectedBoundingBox.MaxX,
            expectedBoundingBox.MaxY
        });
    }

    private static OverlayExtentSearchHandler CreateHandler()
    {
        return new OverlayExtentSearchHandler(_configuration);
    }
}