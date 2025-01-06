using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc;
using Collection = WesternStatesWater.WaDE.Engines.Contracts.Ogc.Collection;
using Constants = WesternStatesWater.WaDE.Contracts.Api.OgcApi.Constants;

namespace WesternStatesWater.WaDE.Engines.Handlers;

public abstract class OgcFormattingHandlerBase(IConfiguration configuration)
{
    protected readonly string ServerUrl = $"{configuration["ServerUrl"]}";
    protected readonly string ApiPath = $"{configuration["ApiPath"]}";
    protected readonly GeometryFactory GeometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);

    protected Collection CreateCollection(MetadataBase metadata)
    {
        var collectionId = GetCollectionId(metadata);
        var collectionExtent = CreateExtentFromMetadata(metadata);

        return new Collection
        {
            Id = collectionId,
            Extent = collectionExtent,
            Links = new LinkBuilder(ServerUrl, ApiPath)
                .AddLandingPage()
                .AddCollection(collectionId)
                .Build(),
            ItemType = "feature",
            Crs = collectionExtent?.Spatial != null ? [metadata.BoundaryBox.Crs] : null,
            StorageCrs = collectionExtent?.Spatial != null ? metadata.BoundaryBox.Crs : null
        };
    }

    private static string GetCollectionId(MetadataBase metadataBase)
    {
        return metadataBase switch
        {
            SiteMetadata => Constants.SitesCollectionId,
            SiteVariableAmountsMetadata => Constants.TimeSeriesCollectionId,
            AllocationMetadata => Constants.RightsCollectionId,
            OverlayMetadata => Constants.OverlaysCollectionId,
            _ => throw new NotSupportedException($"Feature Collection {metadataBase.GetType().Name} is not supported")
        };
    }

    private static Extent? CreateExtentFromMetadata(MetadataBase metadata)
    {
        if (
            metadata.BoundaryBox == null &&
            !metadata.IntervalStartDate.HasValue &&
            !metadata.IntervalEndDate.HasValue)
        {
            return null;
        }

        Spatial? spatial = null;
        if (metadata.BoundaryBox != null)
        {
            spatial = new Spatial
            {
                Bbox = new double[][]
                {
                    [
                        metadata.BoundaryBox.MinX,
                        metadata.BoundaryBox.MinY,
                        metadata.BoundaryBox.MaxX,
                        metadata.BoundaryBox.MaxY
                    ]
                },
                Crs = metadata.BoundaryBox.Crs
            };
        }

        Temporal? temporal = null;
        if (metadata.IntervalStartDate.HasValue || metadata.IntervalEndDate.HasValue)
        {
            temporal = new Temporal
            {
                Trs = "http://www.opengis.net/def/uom/ISO-8601/0/Gregorian",
                Interval =
                [
                    [
                        metadata.IntervalStartDate?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        metadata.IntervalEndDate?.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    ]
                ]
            };
        }

        return new Extent
        {
            Spatial = spatial,
            Temporal = temporal
        };
    }
    
    /// <summary>
    /// Creates a Geometry from the bounding box. If the bounding box crosses the anti meridian, a multi polygon is created, else a single polygon is created.
    /// </summary>
    /// <param name="bbox">Bounding box that only supports 4 coordinates. [[minX,minY,maxX,maxY]]</param>
    /// <returns>SRID 4326 Polygon or MultiPolygon</returns>
    protected Geometry? ConvertBoundaryBoxToPolygon(double[][]? bbox)
    {
        if (bbox is not { Length: 1 } || bbox[0].Length != 4) return null;
        
        double left = bbox[0][0];
        double bottom = bbox[0][1];
        double right = bbox[0][2];
        double top = bbox[0][3];

        if (left > right) // Crosses the anti meridian
        {
            var box1 = GeometryFactory.CreatePolygon([
                new Coordinate(left, top),
                new Coordinate(180, top),
                new Coordinate(180, bottom),
                new Coordinate(left, bottom),
                new Coordinate(left, top)
            ]);

            var box2 = GeometryFactory.CreatePolygon([
                new Coordinate(-180, top),
                new Coordinate(right, top),
                new Coordinate(right, bottom),
                new Coordinate(-180, bottom),
                new Coordinate(-180, top)
            ]);

            return GeometryFactory.BuildGeometry([box1, box2]);
        }
        else
        {
            var box = GeometryFactory.CreatePolygon([
                new Coordinate(left, top),
                new Coordinate(right, top),
                new Coordinate(right, bottom),
                new Coordinate(left, bottom),
                new Coordinate(left, top)
            ]);

            return GeometryFactory.BuildGeometry([box]);
        }
    }
}