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

// OGC API Spec for Feature Collections `/collections` endpoint.
// https://docs.ogc.org/is/17-069r4/17-069r4.html#_collections_

[TestClass]
public class OgcCollectionsFormattingTests : OgcFormattingTestBase
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
        MockApiContextRequest("/collections");
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
            .BeEquivalentTo(Constants.SitesCollectionId, Constants.RightsCollectionId, Constants.OverlaysCollectionId, Constants.TimeSeriesCollectionId);
        response.Links.Should().NotBeNullOrEmpty();
        response.Links.Should().BeEquivalentTo([
            new Link
            {
                Href = $"{ApiHostName}/collections",
                Rel = "self",
                Type = "application/json",
                Title = "This document as JSON"
            },
            new Link
            {
                Href = $"{ApiHostName}",
                Rel = "root",
                Type = "application/json",
                Title = "The API landing page"
            }
        ]);
    }

    [TestMethod]
    public async Task OgcSpec_ResponseCollection_ShouldHaveLinks()
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
        foreach (var collection in response.Collections)
        {
            var collectionId = collection.Id;
            collection.Links.Should().BeEquivalentTo([
                new Link
                {
                    Href = $"{ApiHostName}/collections/{collectionId}",
                    Rel = "self",
                    Type = "application/json",
                    Title = "This document as JSON"
                },
                new Link
                {
                    Href = $"{ApiHostName}/collections/{collectionId}/items",
                    Rel = "data",
                    Type = "application/geo+json",
                    Title = "Items as GeoJSON"
                }
            ]);
        }
    }

    [TestMethod]
    public async Task Metadata_NoBoundaryBoxOrTimeframe_ExtentIsNull()
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
        foreach (var collection in response.Collections)
        {
            collection.Extent.Should().BeNull();
        }
    }

    [TestMethod]
    public async Task Metadata_HasBoundaryBox_ExtentIsSet()
    {
        // Arrange
        _regulatoryOverlayAccessorMock.Arrange(mock => mock.GetOverlayMetadata())
            .ReturnsAsync(new OverlayMetadata());
        _siteVariableAmountsAccessorMock.Arrange(mock => mock.GetSiteVariableAmountsMetadata())
            .ReturnsAsync(new SiteVariableAmountsMetadata());
        _allocationAccessorMock.Arrange(mock => mock.GetAllocationMetadata())
            .ReturnsAsync(new AllocationMetadata());
        _siteAccessorMock.Arrange(mock => mock.GetSiteMetadata())
            .ReturnsAsync(new SiteMetadata
            {
                BoundaryBox = new BoundaryBox
                {
                    Crs = "http://www.opengis.net/def/crs/OGC/1.3/CRS84",
                    MinX = -180,
                    MinY = 180,
                    MaxX = -90,
                    MaxY = 90
                }
            });

        // Act
        var request = new CollectionsRequest();
        var response = await CreateHandler().Handle(request);

        // Assert
        response.Should().NotBeNull();
        var sitesCollection = response.Collections.First(c => c.Id == "sites");
        sitesCollection.Crs.Should().BeEquivalentTo("http://www.opengis.net/def/crs/OGC/1.3/CRS84");
        sitesCollection.StorageCrs.Should().Be("http://www.opengis.net/def/crs/OGC/1.3/CRS84");
        sitesCollection.Extent.Spatial.Crs.Should().Be("http://www.opengis.net/def/crs/OGC/1.3/CRS84");
        sitesCollection.Extent.Spatial.Bbox[0][0].Should().Be(-180);
        sitesCollection.Extent.Spatial.Bbox[0][1].Should().Be(180);
        sitesCollection.Extent.Spatial.Bbox[0][2].Should().Be(-90);
        sitesCollection.Extent.Spatial.Bbox[0][3].Should().Be(90);
        sitesCollection.Extent.Temporal.Should().BeNull();
    }

    [TestMethod]
    [DataRow("2020-01-01T00:00:00Z", "2020-12-31T23:59:59Z")]
    [DataRow("2020-01-01T00:00:00Z", null)]
    [DataRow(null, "2020-12-31T23:59:59Z")]
    public async Task Metadata_HasTimeframe_ExtentIsSet(string startDate, string endDate)
    {
        // Arrange
        _regulatoryOverlayAccessorMock.Arrange(mock => mock.GetOverlayMetadata())
            .ReturnsAsync(new OverlayMetadata());
        _siteVariableAmountsAccessorMock.Arrange(mock => mock.GetSiteVariableAmountsMetadata())
            .ReturnsAsync(new SiteVariableAmountsMetadata());
        _allocationAccessorMock.Arrange(mock => mock.GetAllocationMetadata())
            .ReturnsAsync(new AllocationMetadata());
        _siteAccessorMock.Arrange(mock => mock.GetSiteMetadata())
            .ReturnsAsync(new SiteMetadata
            {
                IntervalStartDate = string.IsNullOrEmpty(startDate)
                    ? null
                    : DateTime.Parse(startDate,null, System.Globalization.DateTimeStyles.RoundtripKind),
                IntervalEndDate = string.IsNullOrEmpty(endDate)
                    ? null
                    : DateTime.Parse(endDate,null, System.Globalization.DateTimeStyles.RoundtripKind)
            });

        // Act
        var request = new CollectionsRequest();
        var response = await CreateHandler().Handle(request);

        // Assert
        response.Should().NotBeNull();
        var sitesCollection = response.Collections.First(c => c.Id == "sites");
        sitesCollection.Extent.Temporal.Interval.Should().NotBeNull();
        sitesCollection.Extent.Temporal.Trs.Should().Be("http://www.opengis.net/def/uom/ISO-8601/0/Gregorian");
        sitesCollection.Extent.Temporal.Interval[0][0].Should().Be(startDate);
        sitesCollection.Extent.Temporal.Interval[0][1].Should().Be(endDate);
    }

    private OgcCollectionsFormattingHandler CreateHandler()
    {
        return new OgcCollectionsFormattingHandler(
            Configuration.GetConfiguration(),
            _regulatoryOverlayAccessorMock,
            _siteVariableAmountsAccessorMock,
            _allocationAccessorMock,
            _siteAccessorMock,
            _contextUtilityMock);
    }
}