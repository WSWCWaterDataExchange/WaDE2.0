using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Sites.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Sites.Responses;
using WesternStatesWater.WaDE.Accessors.Sites;

namespace WesternStatesWater.WaDE.Accessors.Tests;

[TestClass]
public class SiteAccessorTests : AccessorTestBase
{
    [TestMethod]
    public async Task SiteAccessor_SiteExtentSearch_ReturnsData()
    {
        // Arrange
        SiteExtentSearchRequest request = new();

        // Act
        var response = await ServiceProvider
            .GetRequiredService<ISiteAccessor>()
            .Search<SiteExtentSearchRequest, SiteExtentSearchResponse>(request);

        // Assert
        response.Should().NotBeNull();
    }
}