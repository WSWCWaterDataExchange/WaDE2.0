using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;

namespace WesternStatesWater.WaDE.Managers.Api.Mapping;

public class OgcApiProfile : Profile
{
    public OgcApiProfile()
    {
        // Managers -> Engines
        CreateMap<Contracts.Api.Requests.V2.CollectionMetadataGetRequest,
            Engines.Contracts.Ogc.Requests.CollectionRequest>();
        CreateMap<Contracts.Api.Requests.V2.DiscoveryMetadataGetRequest,
            Engines.Contracts.Ogc.Requests.DiscoveryRequest>();
        CreateMap<Contracts.Api.Requests.V2.ConformanceMetadataGetRequest,
            Engines.Contracts.Ogc.Requests.ConformanceRequest>();

        // Engines -> Managers
        CreateMap<Engines.Contracts.Ogc.Responses.DiscoveryResponse,
            Contracts.Api.Responses.V2.DiscoveryMetadataGetResponse>();
        CreateMap<Engines.Contracts.Ogc.Responses.ConformanceResponse,
            Contracts.Api.Responses.V2.ConformanceMetadataGetResponse>();
        CreateMap<Engines.Contracts.Ogc.Responses.CollectionResponse,
            Contracts.Api.Responses.V2.CollectionMetadataGetResponse>();
        CreateMap<Engines.Contracts.Ogc.Responses.CollectionsResponse,
            Contracts.Api.Responses.V2.CollectionsMetadataGetResponse>();
        CreateMap<Engines.Contracts.Ogc.Responses.OgcFeaturesFormattingResponse,
            Contracts.Api.Responses.V2.SiteFeaturesSearchResponse>();
        CreateMap<Engines.Contracts.Ogc.Responses.OgcFeaturesFormattingResponse,
            Contracts.Api.Responses.V2.OverlayFeaturesSearchResponse>();
        CreateMap<Engines.Contracts.Ogc.Responses.OgcFeaturesFormattingResponse,
            Contracts.Api.Responses.V2.RightFeaturesSearchResponse>();
        CreateMap<Engines.Contracts.Ogc.Responses.OgcFeaturesFormattingResponse,
            Contracts.Api.Responses.V2.TimeSeriesFeaturesSearchResponse>();
        CreateMap<Engines.Contracts.Ogc.Responses.OgcFeaturesFormattingResponse,
                Contracts.Api.Responses.V2.SiteFeatureItemGetResponse>()
            .ForMember(dest => dest.Feature, mem => mem.MapFrom(src => src.Features[0]));
        CreateMap<Engines.Contracts.Ogc.Responses.OgcFeaturesFormattingResponse,
                Contracts.Api.Responses.V2.RightFeatureItemGetResponse>()
            .ForMember(dest => dest.Feature, mem => mem.MapFrom(src => src.Features[0]));
        CreateMap<Engines.Contracts.Ogc.Responses.OgcFeaturesFormattingResponse,
                Contracts.Api.Responses.V2.OverlayFeatureItemGetResponse>()
            .ForMember(dest => dest.Feature, mem => mem.MapFrom(src => src.Features[0]));
        CreateMap<Engines.Contracts.Ogc.Responses.OgcFeaturesFormattingResponse,
                Contracts.Api.Responses.V2.TimeSeriesFeatureItemGetResponse>()
            .ForMember(dest => dest.Feature, mem => mem.MapFrom(src => src.Features[0]));

        // Managers -> Accessors
        CreateMap<Contracts.Api.Requests.V2.SiteFeaturesItemRequest,
                Accessors.Contracts.Api.SpatialSearchCriteria>()
            .ForMember(dest => dest.Geometry, mem => mem.ConvertUsing(new BoundingBoxConverter(), src => src.Bbox))
            .ForMember(dest => dest.SpatialRelationType,
                mem => mem.MapFrom(src => AccessorApi.SpatialRelationType.Intersects));

        CreateMap<Contracts.Api.Requests.V2.SiteFeaturesAreaRequest,
                Accessors.Contracts.Api.SpatialSearchCriteria>()
            .ForMember(dest => dest.Geometry, mem => mem.ConvertUsing(new WktToGeometryConverter(), src => src.Coords))
            .ForMember(dest => dest.SpatialRelationType,
                mem => mem.MapFrom(src => AccessorApi.SpatialRelationType.Intersects));

        CreateMap<Contracts.Api.Requests.V2.SiteFeaturesItemRequest,
                Accessors.Contracts.Api.V2.Requests.SiteSearchRequest>()
            .ForMember(dest => dest.GeometrySearch, mem => mem.MapFrom(src => src))
            .ForMember(dest => dest.LastSiteUuid, mem => mem.MapFrom(src => src.Next))
            .ForMember(dest => dest.SiteTypes,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.SiteTypes))
            .ForMember(dest => dest.States,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.States))
            .ForMember(dest => dest.WaterSourcesTypes,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.WaterSourceTypes))
            .ForMember(dest => dest.SiteUuids,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.SiteUuids))
            .ForMember(dest => dest.OverlayUuids,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.OverlayUuids))
            .ForMember(dest => dest.AllocationUuids,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.AllocationUuids));

        CreateMap<Contracts.Api.Requests.V2.SiteFeaturesAreaRequest,
                Accessors.Contracts.Api.V2.Requests.SiteSearchRequest>()
            .ForMember(dest => dest.SiteTypes, mem => mem.Ignore())
            .ForMember(dest => dest.States, mem => mem.Ignore())
            .ForMember(dest => dest.WaterSourcesTypes, mem => mem.Ignore())
            .ForMember(dest => dest.SiteUuids, mem => mem.Ignore())
            .ForMember(dest => dest.OverlayUuids, mem => mem.Ignore())
            .ForMember(dest => dest.AllocationUuids, mem => mem.Ignore())
            .ForMember(dest => dest.GeometrySearch, mem => mem.MapFrom(src => src))
            .ForMember(dest => dest.LastSiteUuid, mem => mem.MapFrom(src => src.Next));

        CreateMap<Contracts.Api.Requests.V2.OverlayFeaturesItemRequest,
                Accessors.Contracts.Api.SpatialSearchCriteria>()
            .ForMember(dest => dest.Geometry, mem => mem.ConvertUsing(new BoundingBoxConverter(), src => src.Bbox))
            .ForMember(dest => dest.SpatialRelationType,
                mem => mem.MapFrom(src => AccessorApi.SpatialRelationType.Intersects));

        CreateMap<Contracts.Api.Requests.V2.OverlayFeaturesAreaRequest,
                Accessors.Contracts.Api.SpatialSearchCriteria>()
            .ForMember(dest => dest.Geometry, mem => mem.ConvertUsing(new WktToGeometryConverter(), src => src.Coords))
            .ForMember(dest => dest.SpatialRelationType,
                mem => mem.MapFrom(src => AccessorApi.SpatialRelationType.Intersects));

        CreateMap<Contracts.Api.Requests.V2.OverlayFeaturesItemRequest,
                Accessors.Contracts.Api.V2.Requests.OverlaySearchRequest>()
            .ForMember(dest => dest.GeometrySearch, mem => mem.MapFrom(src => src))
            .ForMember(dest => dest.OverlayUuids,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.OverlayUuids))
            .ForMember(dest => dest.SiteUuids,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.SiteUuids))
            .ForMember(dest => dest.States,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.States))
            .ForMember(dest => dest.WaterSourceTypes,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.WaterSourceTypes))
            .ForMember(dest => dest.OverlayTypes,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.OverlayTypes))
            .ForMember(dest => dest.LastKey, mem => mem.MapFrom(src => src.Next));

        CreateMap<Contracts.Api.Requests.V2.OverlayFeaturesAreaRequest,
                Accessors.Contracts.Api.V2.Requests.OverlaySearchRequest>()
            .ForMember(dest => dest.OverlayUuids, mem => mem.Ignore())
            .ForMember(dest => dest.SiteUuids, mem => mem.Ignore())
            .ForMember(dest => dest.States, mem => mem.Ignore())
            .ForMember(dest => dest.WaterSourceTypes, mem => mem.Ignore())
            .ForMember(dest => dest.OverlayTypes, mem => mem.Ignore())
            .ForMember(dest => dest.GeometrySearch, mem => mem.MapFrom(src => src))
            .ForMember(dest => dest.LastKey, mem => mem.MapFrom(src => src.Next));

        CreateMap<Contracts.Api.Requests.V2.RightFeaturesItemRequest,
                Accessors.Contracts.Api.SpatialSearchCriteria>()
            .ForMember(dest => dest.Geometry, mem => mem.ConvertUsing(new BoundingBoxConverter(), src => src.Bbox))
            .ForMember(dest => dest.SpatialRelationType,
                mem => mem.MapFrom(src => AccessorApi.SpatialRelationType.Intersects));

        CreateMap<Contracts.Api.Requests.V2.RightFeaturesAreaRequest,
                Accessors.Contracts.Api.SpatialSearchCriteria>()
            .ForMember(dest => dest.Geometry, mem => mem.ConvertUsing(new WktToGeometryConverter(), src => src.Coords))
            .ForMember(dest => dest.SpatialRelationType,
                mem => mem.MapFrom(src => AccessorApi.SpatialRelationType.Intersects));

        CreateMap<Contracts.Api.Requests.V2.RightFeaturesItemRequest,
                Accessors.Contracts.Api.V2.Requests.AllocationSearchRequest>()
            .ForMember(dest => dest.GeometrySearch, mem => mem.MapFrom(src => src))
            .ForMember(dest => dest.AllocationUuid,
                opt => opt.ConvertUsing(new CommaStringToListConverter(), src => src.AllocationUuids))
            .ForMember(dest => dest.WaterSourceTypes,
                opt => opt.ConvertUsing(new CommaStringToListConverter(), src => src.WaterSourceTypes))
            .ForMember(dest => dest.BeneficialUses,
                opt => opt.ConvertUsing(new CommaStringToListConverter(), src => src.BeneficialUses))
            .ForMember(dest => dest.SiteUuid,
                opt => opt.ConvertUsing(new CommaStringToListConverter(), src => src.SiteUuids))
            .ForMember(dest => dest.States,
                opt => opt.ConvertUsing(new CommaStringToListConverter(), src => src.States))
            .ForMember(dest => dest.LastKey, opt => opt.MapFrom(src => src.Next));

        CreateMap<Contracts.Api.Requests.V2.RightFeaturesAreaRequest,
                Accessors.Contracts.Api.V2.Requests.AllocationSearchRequest>()
            .ForMember(dest => dest.AllocationUuid, mem => mem.Ignore())
            .ForMember(dest => dest.SiteUuid, mem => mem.Ignore())
            .ForMember(dest => dest.States, mem => mem.Ignore())
            .ForMember(dest => dest.WaterSourceTypes, mem => mem.Ignore())
            .ForMember(dest => dest.BeneficialUses, mem => mem.Ignore())
            .ForMember(dest => dest.GeometrySearch, mem => mem.MapFrom(src => src))
            .ForMember(dest => dest.LastKey, opt => opt.MapFrom(src => src.Next));

        CreateMap<Contracts.Api.Requests.V2.TimeSeriesFeaturesItemRequest,
                Accessors.Contracts.Api.SpatialSearchCriteria>()
            .ForMember(dest => dest.Geometry, mem => mem.ConvertUsing(new BoundingBoxConverter(), src => src.Bbox))
            .ForMember(dest => dest.SpatialRelationType,
                mem => mem.MapFrom(src => AccessorApi.SpatialRelationType.Intersects));

        CreateMap<Contracts.Api.Requests.V2.TimeSeriesFeaturesAreaRequest,
                Accessors.Contracts.Api.SpatialSearchCriteria>()
            .ForMember(dest => dest.Geometry, mem => mem.ConvertUsing(new WktToGeometryConverter(), src => src.Coords))
            .ForMember(dest => dest.SpatialRelationType,
                mem => mem.MapFrom(src => AccessorApi.SpatialRelationType.Intersects));

        CreateMap<Contracts.Api.Requests.V2.TimeSeriesFeaturesItemRequest,
                Accessors.Contracts.Api.V2.Requests.TimeSeriesSearchRequest>()
            .ForMember(dest => dest.GeometrySearch, mem => mem.MapFrom(src => src))
            .ForMember(dest => dest.DateRange, mem => mem.ConvertUsing(new OgcDateTimeConverter(), src=>src.DateTime))
            .ForMember(dest => dest.SiteUuids,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.SiteUuids))
            .ForMember(dest => dest.States,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.States))
            .ForMember(dest => dest.VariableTypes,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.VariableTypes))
            .ForMember(dest => dest.WaterSourceTypes,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.WaterSourceTypes))
            .ForMember(dest => dest.PrimaryUses,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.PrimaryUses))
            .ForMember(dest => dest.LastKey, opt => opt.MapFrom(src => src.Next));

        CreateMap<Contracts.Api.Requests.V2.TimeSeriesFeaturesAreaRequest,
                Accessors.Contracts.Api.V2.Requests.TimeSeriesSearchRequest>()
            .ForMember(dest => dest.SiteUuids, mem => mem.Ignore())
            .ForMember(dest => dest.States, mem => mem.Ignore())
            .ForMember(dest => dest.VariableTypes, mem => mem.Ignore())
            .ForMember(dest => dest.WaterSourceTypes, mem => mem.Ignore())
            .ForMember(dest => dest.PrimaryUses, mem => mem.Ignore())
            .ForMember(dest => dest.DateRange, mem => mem.Ignore())
            .ForMember(dest => dest.GeometrySearch, mem => mem.MapFrom(src => src))
            .ForMember(dest => dest.LastKey, mem => mem.MapFrom(src => src.Next));

        CreateMap<Contracts.Api.Requests.V2.SiteFeatureItemGetRequest,
                Accessors.Contracts.Api.V2.Requests.SiteSearchRequest>()
            .ForMember(dest => dest.GeometrySearch, mem => mem.Ignore())
            .ForMember(dest => dest.LastSiteUuid, mem => mem.Ignore())
            .ForMember(dest => dest.SiteTypes, mem => mem.Ignore())
            .ForMember(dest => dest.States, mem => mem.Ignore())
            .ForMember(dest => dest.WaterSourcesTypes, mem => mem.Ignore())
            .ForMember(dest => dest.OverlayUuids, mem => mem.Ignore())
            .ForMember(dest => dest.AllocationUuids, mem => mem.Ignore())
            .ForMember(dest => dest.Limit, mem => mem.MapFrom(src => 1))
            .ForMember(dest => dest.SiteUuids, mem => mem.MapFrom(src => new List<string> { src.Id }));

        CreateMap<Contracts.Api.Requests.V2.RightFeatureItemGetRequest,
                Accessors.Contracts.Api.V2.Requests.AllocationSearchRequest>()
            .ForMember(dest => dest.SiteUuid, mem => mem.Ignore())
            .ForMember(dest => dest.States, mem => mem.Ignore())
            .ForMember(dest => dest.WaterSourceTypes, mem => mem.Ignore())
            .ForMember(dest => dest.BeneficialUses, mem => mem.Ignore())
            .ForMember(dest => dest.GeometrySearch, mem => mem.Ignore())
            .ForMember(dest => dest.LastKey, mem => mem.Ignore())
            .ForMember(dest => dest.Limit, mem => mem.MapFrom(src => 1))
            .ForMember(dest => dest.AllocationUuid, mem => mem.MapFrom(src => new List<string> { src.Id }));

        CreateMap<Contracts.Api.Requests.V2.OverlayFeatureItemGetRequest,
                Accessors.Contracts.Api.V2.Requests.OverlaySearchRequest>()
            .ForMember(dest => dest.SiteUuids, mem => mem.Ignore())
            .ForMember(dest => dest.States, mem => mem.Ignore())
            .ForMember(dest => dest.WaterSourceTypes, mem => mem.Ignore())
            .ForMember(dest => dest.OverlayTypes, mem => mem.Ignore())
            .ForMember(dest => dest.LastKey, mem => mem.Ignore())
            .ForMember(dest => dest.GeometrySearch, mem => mem.Ignore())
            .ForMember(dest => dest.Limit, mem => mem.MapFrom(src => 1))
            .ForMember(dest => dest.OverlayUuids, mem => mem.MapFrom(src => new List<string> { src.Id }));

        CreateMap<Contracts.Api.Requests.V2.TimeSeriesFeatureItemGetRequest,
                Accessors.Contracts.Api.V2.Requests.TimeSeriesSearchRequest>()
            .ForMember(dest => dest.GeometrySearch, mem => mem.Ignore())
            .ForMember(dest => dest.DateRange, mem => mem.Ignore())
            .ForMember(dest => dest.SiteUuids, mem => mem.Ignore())
            .ForMember(dest => dest.States, mem => mem.Ignore())
            .ForMember(dest => dest.VariableTypes, mem => mem.Ignore())
            .ForMember(dest => dest.WaterSourceTypes, mem => mem.Ignore())
            .ForMember(dest => dest.PrimaryUses, mem => mem.Ignore())
            .ForMember(dest => dest.LastKey, mem => mem.Ignore())
            .ForMember(dest => dest.Limit, mem => mem.MapFrom(src => 1))
            .ForMember(dest => dest.SiteUuids,
                mem => mem.MapFrom(src => new List<string> { src.Id }));

        // Accessor -> Engines
        CreateMap<Accessors.Contracts.Api.V2.Responses.SiteSearchResponse,
                Engines.Contracts.Ogc.Requests.OgcFeaturesFormattingRequest>()
            .ForMember(dest => dest.Items,
                opt => opt.MapFrom((src, a, b, c) =>
                    c.Mapper.Map<List<Engines.Contracts.SiteFeature>>(src.Sites)));

        CreateMap<Accessors.Contracts.Api.V2.Responses.OverlaySearchResponse,
                Engines.Contracts.Ogc.Requests.OgcFeaturesFormattingRequest>()
            .ForMember(dest => dest.Items,
                opt => opt.MapFrom((src, a, b, c) =>
                    c.Mapper.Map<List<Engines.Contracts.OverlayFeature>>(src.Overlays)));

        CreateMap<Accessors.Contracts.Api.V2.Responses.AllocationSearchResponse,
                Engines.Contracts.Ogc.Requests.OgcFeaturesFormattingRequest>()
            .ForMember(dest => dest.Items,
                opt => opt.MapFrom((src, a, b, c) =>
                    c.Mapper.Map<List<Engines.Contracts.RightFeature>>(src.Allocations)));

        CreateMap<Accessors.Contracts.Api.V2.Responses.TimeSeriesSearchResponse,
                Engines.Contracts.Ogc.Requests.OgcFeaturesFormattingRequest>()
            .ForMember(dest => dest.Items,
                opt => opt.MapFrom((src, a, b, c) =>
                    c.Mapper.Map<List<Engines.Contracts.TimeSeriesFeature>>(src.Sites)));

        CreateMap<Accessors.Contracts.Api.V2.SiteSearchItem,
                Engines.Contracts.SiteFeature>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SiteUuid))
            .ForMember(dest => dest.Geometry, opt => opt.MapFrom(src => src.Location));

        CreateMap<Accessors.Contracts.Api.V2.OverlaySearchItem,
                Engines.Contracts.OverlayFeature>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OverlayUuid))
            .ForMember(dest => dest.Geometry, opt => opt.MapFrom(src => src.Areas));

        CreateMap<Accessors.Contracts.Api.V2.AllocationSearchItem,
                Engines.Contracts.RightFeature>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AllocationUUID))
            .ForMember(dest => dest.Geometry, opt => opt.Ignore());

        CreateMap<Accessors.Contracts.Api.V2.TimeSeriesSearchItem,
                Engines.Contracts.TimeSeriesFeature>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SiteUuid))
            .ForMember(dest => dest.TimeSeries, opt => opt.MapFrom(src => src.TimeSeries))
            .ForMember(dest => dest.Geometry, opt => opt.MapFrom(src => src.Location));

        CreateMap<Accessors.Contracts.Api.V2.TimeSeries,
            Engines.Contracts.TimeSeries>();

        CreateMap<Accessors.Contracts.Api.WaterSourceSummary,
            Engines.Contracts.WaterSourceSummary>();

        CreateMap<Accessors.Contracts.Api.V2.Organization,
            Engines.Contracts.Organization>();

        CreateMap<Accessors.Contracts.Api.V2.VariableSpecific,
            Engines.Contracts.VariableSpecific>();

        CreateMap<Accessors.Contracts.Api.V2.Method,
            Engines.Contracts.Method>();
    }
}