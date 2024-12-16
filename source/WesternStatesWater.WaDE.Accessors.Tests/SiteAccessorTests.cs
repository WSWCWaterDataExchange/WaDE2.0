using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Accessors.Tests;

[TestClass]
public class SiteAccessorTests : AccessorTestBase
{
    [TestMethod]
    public async Task SiteAccessor_SiteExtentSearch_ReturnsData()
    {
        // Act
        var response = await ServiceProvider
            .GetRequiredService<ISiteAccessor>()
            .GetSiteMetadata();

        // Assert
        response.Should().NotBeNull();
        response.BoundaryBox.Should().NotBeNull();
        response.BoundaryBox.Crs.Should().Be("http://www.opengis.net/def/crs/OGC/1.3/CRS84");
        response.BoundaryBox.MinX.Should().Be(-180);
        response.BoundaryBox.MinY.Should().Be(18);
        response.BoundaryBox.MaxX.Should().Be(-93);
        response.BoundaryBox.MaxY.Should().Be(72);
    }
}