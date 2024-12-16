using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;

namespace WesternStatesWater.WaDE.Engines.Tests.FormattingEngine;

// https://docs.ogc.org/is/17-069r4/17-069r4.html#_collections_

[TestClass]
public class OgcCollectionsFormattingTests
{
    private readonly IRegulatoryOverlayAccessor _regulatoryOverlayAccessorMock =
        Mock.Create<IRegulatoryOverlayAccessor>(Behavior.Strict);

    private readonly ISiteVariableAmountsAccessor _siteVariableAmountsAccessorMock =
        Mock.Create<ISiteVariableAmountsAccessor>(Behavior.Strict);

    private readonly IWaterAllocationAccessor _allocationAccessorMock =
        Mock.Create<IWaterAllocationAccessor>(Behavior.Strict);

    private readonly ISiteAccessor _siteAccessorMock = Mock.Create<ISiteAccessor>(Behavior.Strict);

    [TestInitialize]
    public void TestInitialize()
    {
        Environment.SetEnvironmentVariable("ServerUrl", "http://localhost");
        Environment.SetEnvironmentVariable("ApiPath", "/api");
    }

    [TestMethod]
    public async Task OgcSpec_Response_ShouldHaveLinksAndCollections()
    {
        // Arrange
        _regulatoryOverlayAccessorMock.Arrange(mock => mock.GetOverlayMetadata())
            .ReturnsAsync(new OverlayMetadata());
        _siteVariableAmountsAccessorMock.Arrange(mock => mock.GetSiteVariableAmountsMetadata())
            .ReturnsAsync(new SiteVariableAmountsMetadata());
        _allocationAccessorMock.Arrange(mock => mock.GetAllocationMetadata())
            .ReturnsAsync(new AllocationMetadata());
        _siteAccessorMock.Arrange(mock => mock.GetSiteMetadata())
            .ReturnsAsync(new SiteMetadata());

        // Act
        var request = new CollectionsRequest();
        var response = await CreateHandler().Handle(request);

        // Assert
        response.Should().NotBeNull();
        response.Collections.Select(c => c.Id).Should()
            .BeEquivalentTo("sites", "waterRights", "overlays", "timeSeries");
        response.Links.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public async Task OgcSpec_ResponseCollection_ShouldHaveSchema()
    {
        // Arrange
        _regulatoryOverlayAccessorMock.Arrange(mock => mock.GetOverlayMetadata())
            .ReturnsAsync(new OverlayMetadata());
        _siteVariableAmountsAccessorMock.Arrange(mock => mock.GetSiteVariableAmountsMetadata())
            .ReturnsAsync(new SiteVariableAmountsMetadata());
        _allocationAccessorMock.Arrange(mock => mock.GetAllocationMetadata())
            .ReturnsAsync(new AllocationMetadata());
        _siteAccessorMock.Arrange(mock => mock.GetSiteMetadata())
            .ReturnsAsync(new SiteMetadata());

        // Act
        var request = new CollectionsRequest();
        var response = await CreateHandler().Handle(request);

        // Assert
        response.Should().NotBeNull();
        response.Collections.Select(c => c.Links).Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    private OgcCollectionsFormattingHandler CreateHandler()
    {
        return new OgcCollectionsFormattingHandler(
            Configuration.GetConfiguration(),
            _regulatoryOverlayAccessorMock,
            _siteVariableAmountsAccessorMock,
            _allocationAccessorMock,
            _siteAccessorMock);
    }
}