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
        Href = SwaggerDescription,
        Rel = "service-desc",
        Type = "application/vnd.oai.openapi+json;version=3.0",
        Title = "The API definition in JSON"
    };

    protected Link ServiceDocumentLink => new Link
    {
        Href = SwaggerDoc,
        Rel = "service-doc",
        Type = "text/html",
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
    /// URL to the Swagger file that describes the API.
    /// </summary>
    private string SwaggerDescription { get; }

    /// <summary>
    /// URL to the Swagger UI.
    /// </summary>
    private string SwaggerDoc { get; set; }

    /// <summary>
    /// Host name that comes from the OgcApi:Host configuration.
    /// Use this when you are referencing links in the OGC API.
    /// </summary>
    protected string OgcHost { get; }

    protected OgcFormattingHandlerBase(IConfiguration configuration)
    {
        Configuration = configuration;

        OgcHost = configuration["OgcApi:Host"] ??
                  throw new InvalidOperationException(
                      $"{nameof(OgcFormattingHandlerBase)} requires OgcApi:Host configuration to build the specification links.");

        SwaggerDoc = configuration["OgcApi:SwaggerDoc"] ??
                     throw new InvalidOperationException(
                         $"{nameof(OgcFormattingHandlerBase)} requires OgcApi:SwaggerDoc configuration to build the specification links.");

        SwaggerDescription = configuration["OgcApi:SwaggerDescription"] ??
                             throw new InvalidOperationException(
                                 $"{nameof(OgcFormattingHandlerBase)} requires OgcApi:SwaggerDescription configuration to build the specification links.");
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

        return request.Segments[collectionsIdx + 1].ToLower().TrimEnd('/');
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
            StorageCrs = collectionExtent?.Spatial != null ? metadata.BoundaryBox.Crs : null,
            ParameterNames = GetCollectionParameterNames(metadata),
            DataQueries = GetCollectionDataQueries(collectionId),
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
    /// Returns the data queries used in all collections.
    /// </summary>
    /// <param name="collectionId">Collection Id</param>
    /// <returns>Data queries for `area` and `items` in CRS84 detail.</returns>
    private Dictionary<string, DataQuery> GetCollectionDataQueries(string collectionId)
    {
        return new Dictionary<string, DataQuery>
        {
            {
                "area", new DataQuery
                {
                    Link = new Link
                    {
                        Href = $"{OgcHost}/collections/{collectionId}/area?coords={{coords}}",
                        Rel = "data",
                        Templated = true,
                        Variables = new Variable
                        {
                            Title = "Area query",
                            Description = "Area query",
                            QueryType = "area",
                            OutputFormats = ["GeoJSON"],
                            DefaultOutputFormat = ["GeoJSON"],
                            CrsDetails =
                            [
                                // This defines the WGS 84 (EPSG:4326) coordinate reference system, which is the standard for GPS and geospatial applications.
                                new CrsDetail
                                {
                                    Crs = "CRS84",
                                    Wkt =
                                        "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]"
                                }
                            ]
                        }
                    }
                }
            },
            {
                "items", new DataQuery
                {
                    Link = new Link
                    {
                        Href = $"{OgcHost}/collections/{collectionId}/items",
                        Rel = "data",
                        Templated = false,
                        Variables = new Variable
                        {
                            Title = "Items query",
                            Description = "Items query",
                            QueryType = "items",
                            OutputFormats = ["GeoJSON"],
                            DefaultOutputFormat = ["GeoJSON"],
                            CrsDetails =
                            [
                                // This defines the WGS 84 (EPSG:4326) coordinate reference system, which is the standard for GPS and geospatial applications.
                                new CrsDetail
                                {
                                    Crs = "CRS84",
                                    Wkt =
                                        "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]"
                                }
                            ]
                        }
                    }
                }
            }
        };
    }
    
    private static Dictionary<string, Parameter> GetCollectionParameterNames(MetadataBase metadata)
    {
        return metadata switch
        {
            SiteMetadata => SiteCollectionParameterNames(),
            AllocationMetadata => RightCollectionParametersNames(),
            OverlayMetadata => OverlayCollectionParameterNames(),
            SiteVariableAmountsMetadata => TimeSeriesCollectionParameterNames(),
            _ => throw new InvalidOperationException("Unknown metadata type.")
        };
    }

    private static Dictionary<string, Parameter> SiteCollectionParameterNames()
    {
        return new Dictionary<string, Parameter>
        {
            {
                "siteTypes", new Parameter
                {
                    Description = "Comma separated list of site types",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "siteTypes"
                    }
                }
            },
            {
                "states", new Parameter
                {
                    Description = "Comma separate list of abbreviated states",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "states"
                    }
                }
            },
            {
                "waterSourceTypes", new Parameter
                {
                    Description = "Comma separated list of water source types",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "waterSourceTypes"
                    }
                }
            }
        };
    }

    private static Dictionary<string, Parameter> RightCollectionParametersNames()
    {
        return new Dictionary<string, Parameter>
        {
            {
                "allocationUuids", new Parameter
                {
                    Description = "Comma separated list of allocation UUIDs",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "allocationUuids"
                    }
                }
            },
            {
                "siteUuids", new Parameter
                {
                    Description = "Comma separated list of site UUIDs",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "siteUuids"
                    }
                }
            },
            {
                "states", new Parameter
                {
                    Description = "Comma separated list of abbreviated states",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "states"
                    }
                }
            },
            {
                "waterSourceTypes", new Parameter
                {
                    Description = "Comma separated list of water source types",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "waterSourceTypes"
                    }
                }
            },
            {
                "beneficialUses", new Parameter
                {
                    Description = "Comma separated list of beneficial uses",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "beneficialUses"
                    }
                }
            }
        };
    }

    private static Dictionary<string, Parameter> OverlayCollectionParameterNames()
    {
        return new Dictionary<string, Parameter>
        {
            {
                "overlayUuids", new Parameter
                {
                    Description = "Comma separated list of overlay UUIDs",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "overlayUuids"
                    }
                }
            },
            {
                "siteUuids", new Parameter
                {
                    Description = "Comma separated list of site UUIDs",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "siteUuids"
                    }
                }
            },
            {
                "states", new Parameter
                {
                    Description = "Comma separated list of abbreviated states",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "states"
                    }
                }
            },
            {
                "overlayTypes", new Parameter
                {
                    Description = "Comma separated list of overlay types",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "overlayTypes"
                    }
                }
            },
            {
                "waterSourceTypes", new Parameter
                {
                    Description = "Comma separated list of water source types",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "waterSourceTypes"
                    }
                }
            }
        };
    }

    private static Dictionary<string, Parameter> TimeSeriesCollectionParameterNames()
    {
        return new Dictionary<string, Parameter>
        {
            {
                "siteUuids", new Parameter
                {
                    Description = "Comma separated list of site UUIDs",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "siteUuids"
                    }
                }
            },
            {
                "states", new Parameter
                {
                    Description = "Comma separated list of abbreviated states",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "states"
                    }
                }
            },
            {
                "waterSourceTypes", new Parameter
                {
                    Description = "Comma separated list of water source types",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "waterSourceTypes"
                    }
                }
            },
            {
                "variableTypes", new Parameter
                {
                    Description = "Comma separated list of variable types",
                    ObservedProperty = new ObservedProperty
                    {
                        Label = "variableTypes"
                    }
                }
            }
        };
    }
}