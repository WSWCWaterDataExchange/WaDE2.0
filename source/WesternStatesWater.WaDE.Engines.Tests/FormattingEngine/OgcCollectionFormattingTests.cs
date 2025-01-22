using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Common.Ogc;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Engines.Tests.FormattingEngine;

// OGC API Spec for Feature Collection `/collections/{collectionId}` endpoint.
// https://docs.ogc.org/is/17-069r4/17-069r4.html#_feature_collection_rootcollectionscollectionid
[TestClass]
public class OgcCollectionFormattingTests : OgcFormattingTestBase
{
    private readonly IRegulatoryOverlayAccessor _regulatoryOverlayAccessorMock =
        Mock.Create<IRegulatoryOverlayAccessor>(Behavior.Strict);

    private readonly ISiteVariableAmountsAccessor _siteVariableAmountsAccessorMock =
        Mock.Create<ISiteVariableAmountsAccessor>(Behavior.Strict);

    private readonly IWaterAllocationAccessor _allocationAccessorMock =
        Mock.Create<IWaterAllocationAccessor>(Behavior.Strict);

    private readonly ISiteAccessor _siteAccessorMock = Mock.Create<ISiteAccessor>(Behavior.Strict);
    
    [DataTestMethod]
    [DataRow("/collections/sites", "sites")]
    [DataRow("/collections/timeseries", "timeseries")]
    [DataRow("/collections/rights", "rights")]
    [DataRow("/collections/overlays", "overlays")]
    public async Task Handler_CallsAccessor_WithCorrectCollectionId(string apiPath, string collectionId)
    {
        // Arrange
        MockApiContextRequest(apiPath);
        
        _regulatoryOverlayAccessorMock.Arrange(mock => mock.GetOverlayMetadata())
            .ReturnsAsync(new OverlayMetadata());
        _siteVariableAmountsAccessorMock.Arrange(mock => mock.GetSiteVariableAmountsMetadata())
            .ReturnsAsync(new SiteVariableAmountsMetadata());
        _allocationAccessorMock.Arrange(mock => mock.GetAllocationMetadata())
            .ReturnsAsync(new AllocationMetadata());
        _siteAccessorMock.Arrange(mock => mock.GetSiteMetadata())
            .ReturnsAsync(new SiteMetadata());

        // Act
        var request = new CollectionRequest();
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
        MockApiContextRequest("/collections/unknown");
        
        var request = new CollectionRequest();

        // Act
        var handler = CreateHandler();
        var action = new Func<Task>(() => handler.Handle(request));

        // Assert
        await action.Should().ThrowAsync<NotSupportedException>();
    }

    [TestMethod]
    public async Task Handler_CollectionResponse_IncludesSelfReference()
    {
        // Arrange
        MockApiContextRequest("/collections/sites");
        
        _siteAccessorMock.Arrange(mock => mock.GetSiteMetadata())
            .ReturnsAsync(new SiteMetadata());
        
        // Act
        var request = new CollectionRequest();
        var response = await CreateHandler().Handle(request);
        
        // Assert
        response.Collection.Links.Should().NotBeNullOrEmpty();
        response.Collection.Links.Should().ContainSingle(l => l.Rel == "self");
        response.Collection.Links.First(l => l.Rel == "self")
            .Should().BeEquivalentTo(new Link
            {
                Href = "https://proxy.example.com/api/collections/sites",
                Rel = "self",
                Type = "application/json",
                Title = "This document as JSON"
            });


    }

    private OgcCollectionFormattingHandler CreateHandler()
    {
        return new OgcCollectionFormattingHandler(
            Configuration.GetConfiguration(),
            _regulatoryOverlayAccessorMock,
            _siteVariableAmountsAccessorMock,
            _allocationAccessorMock,
            _siteAccessorMock,
           _contextUtilityMock
        );
    }
}