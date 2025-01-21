using Microsoft.Extensions.Configuration;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Common.Ogc;

namespace WesternStatesWater.WaDE.Engines.Handlers;

public abstract class OgcFormattingHandlerBase
{
    protected const string ContentTypeJson = "application/json";
    protected readonly IConfiguration Configuration;

    protected Link ServiceDescriptionLink => new Link
    {
        Href = $"{SwaggerHost}/swagger.json",
        Rel = "service-desc",
        Type = ContentTypeJson,
        Title = "The API definition in JSON"
    };

    protected Link ServiceDocumentLink => new Link
    {
        Href = $"{SwaggerHost}/swagger/ui",
        Rel = "service-doc",
        Type = ContentTypeJson,
        Title = "Swagger UI"
    };
    
    protected Link ConformanceLink => new Link
    {
        Href = $"{OgcHost}/conformance",
        Rel = "conformance",
        Type = ContentTypeJson,
        Title = "OGC API conformance declaration"
    };
    
    protected Link CollectionsLink => new Link
    {
        Href = $"{OgcHost}/collections",
        Rel = "data",
        Type = ContentTypeJson,
        Title = "Resource collections"
    };

    /// <summary>
    /// Swagger host name that comes from the OpenApi:HostNames configuration.
    /// Use this when you are referencing links in the Swagger specification.
    /// </summary>
    private string SwaggerHost { get; }
    
    /// <summary>
    /// Host name that comes from the OgcApi:Host configuration.
    /// Use this when you are referencing links in the OGC API.
    /// </summary>
    protected string OgcHost { get; }

    protected OgcFormattingHandlerBase(IConfiguration configuration)
    {
        Configuration = configuration;
        
        OgcHost = configuration["OgcApi:Host"] ?? 
                  throw new InvalidOperationException($"{nameof(OgcFormattingHandlerBase)} requires OgcApi:Host configuration to build the specification links.");
        
        // Uses the OpenApi:HostNames configuration to know the host name of the server.
        // Open API supports multiple host names, but this engine will only support one at this time.
        var swaggerHostNames = configuration["OpenApi:HostNames"] ??
                               throw new InvalidOperationException($"{nameof(OgcFormattingHandlerBase)} requires OpenApi:HostNames configuration to determine swagger links.");
        
        var hostNames = swaggerHostNames.Split(',');
        if (hostNames.Length > 1)
        {
            throw new InvalidOperationException($"{nameof(OgcFormattingHandlerBase)} currently only supports one Swagger host name.");
        }

        SwaggerHost = hostNames[0].Trim();
    }

    /// <summary>
    /// Uses the Uri to extract the collection id.
    /// </summary>
    /// <param name="request">Uri of the request to the server.</param>
    /// <returns>Lowercase collection id.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected static string GetCollectionId(Uri request)
    {
        var collectionsIdx = Array.FindIndex(request.Segments,
            segment => segment.Equals("collections/", StringComparison.OrdinalIgnoreCase));
        if (collectionsIdx == -1)
        {
            throw new InvalidOperationException("Request URI does not contain a collections segment.");
        }

        return request.Segments[collectionsIdx + 1].ToLower();
    }

    protected Collection CreateCollection(MetadataBase metadata, string collectionId)
    {
        var collectionExtent = CreateExtentFromMetadata(metadata);

        return new Collection
        {
            Id = collectionId,
            Extent = collectionExtent,
            Links =
            [
                new Link
                {
                    Href = $"{OgcHost}/collections/{collectionId}", Rel = "self",
                    Type = "application/json", Title = "This document as JSON"
                },
                new Link
                {
                    Href = $"{OgcHost}/collections/{collectionId}/items", Rel = "items",
                    Type = "application/geo+json", Title = "Items as GeoJSON"
                }
            ],
            ItemType = "feature",
            Crs = collectionExtent?.Spatial != null ? [metadata.BoundaryBox.Crs] : null,
            StorageCrs = collectionExtent?.Spatial != null ? metadata.BoundaryBox.Crs : null
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