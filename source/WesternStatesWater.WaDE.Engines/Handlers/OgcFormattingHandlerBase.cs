using Microsoft.Extensions.Configuration;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc;

namespace WesternStatesWater.WaDE.Engines.Handlers;

public abstract class OgcFormattingHandlerBase(IConfiguration configuration)
{
    protected readonly string ServerUrl = $"{configuration["ServerUrl"]}";
    protected readonly string ApiPath = $"{configuration["ApiPath"]}";

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
            SiteMetadata => "sites",
            SiteVariableAmountsMetadata => "timeSeries",
            AllocationMetadata => "rights",
            OverlayMetadata => "overlays",
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
}