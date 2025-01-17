using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Common.Context;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Engines.Tests.FormattingEngine;

// OGC API Spec for Feature Collection `/collections/{collectionId}` endpoint.
// https://docs.ogc.org/is/17-069r4/17-069r4.html#_feature_collection_rootcollectionscollectionid
[TestClass]
public class OgcCollectionFormattingTests
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

    [DataTestMethod]
    [DataRow(Constants.SitesCollectionId)]
    [DataRow(Constants.TimeSeriesCollectionId)]
    [DataRow(Constants.RightsCollectionId)]
    [DataRow(Constants.OverlaysCollectionId)]
    public async Task Handler_CallsAccessor_WithCorrectCollectionId(string collectionId)
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
        var request = new CollectionRequest { CollectionId = collectionId };
        var response = await CreateHandler().Handle(request);

        // Assert
        response.Collection.Id.Should().Be(collectionId);
        _siteAccessorMock.Assert(mock => mock.GetSiteMetadata(),
            collectionId == Constants.SitesCollectionId ? Occurs.Once() : Occurs.Never());
        _siteVariableAmountsAccessorMock.Assert(mock => mock.GetSiteVariableAmountsMetadata(),
            collectionId == Constants.TimeSeriesCollectionId ? Occurs.Once() : Occurs.Never());
        _allocationAccessorMock.Assert(mock => mock.GetAllocationMetadata(),
            collectionId == Constants.RightsCollectionId ? Occurs.Once() : Occurs.Never());
        _regulatoryOverlayAccessorMock.Assert(mock => mock.GetOverlayMetadata(),
            collectionId == Constants.OverlaysCollectionId ? Occurs.Once() : Occurs.Never());
    }
    
    [TestMethod]
    public async Task Handler_CollectionIdNotFound_ThrowsNotSupportedException()
    {
        // Arrange
        var request = new CollectionRequest { CollectionId = "unknown" };

        // Act
        var handler = CreateHandler();
        var action = new Func<Task>(() => handler.Handle(request));

        // Assert
        await action.Should().ThrowAsync<NotSupportedException>();
    }

    private OgcCollectionFormattingHandler CreateHandler()
    {
        return new OgcCollectionFormattingHandler(
            Configuration.GetConfiguration(),
            _regulatoryOverlayAccessorMock,
            _siteVariableAmountsAccessorMock,
            _allocationAccessorMock,
            _siteAccessorMock,
            Mock.Create<IContextUtility>()
        );
    }
}