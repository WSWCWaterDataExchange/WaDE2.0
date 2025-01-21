using System.Collections.Generic;
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

        // Managers -> Accessors
        CreateMap<Contracts.Api.Requests.V2.SiteFeaturesItemRequest,
            Accessors.Contracts.Api.SpatialSearchCriteria>()
            .ForMember(dest => dest.Geometry, mem => mem.ConvertUsing(new BoundingBoxConverter(), src => src.Bbox))
            .ForMember(dest => dest.SpatialRelationType, mem => mem.MapFrom(src => AccessorApi.SpatialRelationType.Intersects));
        
        CreateMap<Contracts.Api.Requests.V2.SiteFeaturesAreaRequest,
        Accessors.Contracts.Api.SpatialSearchCriteria>()
            .ForMember(dest => dest.Geometry, mem => mem.ConvertUsing(new WktToGeometryConverter(), src => src.Coords))
            .ForMember(dest => dest.SpatialRelationType, mem => mem.MapFrom(src=> AccessorApi.SpatialRelationType.Intersects));
        
        CreateMap<Contracts.Api.Requests.V2.SiteFeaturesItemRequest,
                Accessors.Contracts.Api.V2.Requests.SiteSearchRequest>()
            .ForMember(dest => dest.GeometrySearch, mem => mem.MapFrom(src => src))
            .ForMember(dest => dest.LastSiteUuid, mem => mem.MapFrom(src => src.Next))
            .ForMember(dest => dest.SiteTypes, mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.SiteTypes))
            .ForMember(dest => dest.States, mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.States))
            .ForMember(dest => dest.WaterSourcesTypes, mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.WaterSourceTypes));
        
        CreateMap<Contracts.Api.Requests.V2.SiteFeaturesAreaRequest,
                Accessors.Contracts.Api.V2.Requests.SiteSearchRequest>()
            .ForMember(dest => dest.SiteTypes, mem => mem.Ignore())
            .ForMember(dest => dest.States, mem => mem.Ignore())
            .ForMember(dest => dest.WaterSourcesTypes, mem => mem.Ignore())
            .ForMember(dest => dest.GeometrySearch, mem => mem.MapFrom(src => src))
            .ForMember(dest => dest.LastSiteUuid, mem => mem.MapFrom(src => src.Next));
        
        CreateMap<Contracts.Api.Requests.V2.OverlayFeaturesItemRequest,
                Accessors.Contracts.Api.SpatialSearchCriteria>()
            .ForMember(dest => dest.Geometry, mem => mem.ConvertUsing(new BoundingBoxConverter(), src => src.Bbox))
            .ForMember(dest => dest.SpatialRelationType, mem => mem.MapFrom(src => AccessorApi.SpatialRelationType.Intersects));
        
        CreateMap<Contracts.Api.Requests.V2.OverlayFeaturesAreaRequest,
        Accessors.Contracts.Api.SpatialSearchCriteria>()
            .ForMember(dest => dest.Geometry, mem => mem.ConvertUsing(new WktToGeometryConverter(), src => src.Coords))
            .ForMember(dest => dest.SpatialRelationType, mem => mem.MapFrom(src=> AccessorApi.SpatialRelationType.Intersects));

        CreateMap<Contracts.Api.Requests.V2.OverlayFeaturesItemRequest,
                Accessors.Contracts.Api.V2.Requests.OverlaySearchRequest>()
            .ForMember(dest => dest.GeometrySearch, mem => mem.MapFrom(src => src))
            .ForMember(dest => dest.OverlayUuids,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.OverlayUuids))
            .ForMember(dest => dest.SiteUuids,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.SiteUuids))
            .ForMember(dest => dest.States, mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.States))
            .ForMember(dest => dest.WaterSourceTypes,
                mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.WaterSourceTypes))
            .ForMember(dest => dest.OverlayTypes, mem => mem.ConvertUsing(new CommaStringToListConverter(), src => src.OverlayTypes))
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
            .ForMember(dest => dest.SpatialRelationType, mem => mem.MapFrom(src => AccessorApi.SpatialRelationType.Intersects));
        
        CreateMap<Contracts.Api.Requests.V2.RightFeaturesAreaRequest,
        Accessors.Contracts.Api.SpatialSearchCriteria>()
            .ForMember(dest => dest.Geometry, mem => mem.ConvertUsing(new WktToGeometryConverter(), src => src.Coords))
            .ForMember(dest => dest.SpatialRelationType, mem => mem.MapFrom(src=> AccessorApi.SpatialRelationType.Intersects));
        
        CreateMap<Contracts.Api.Requests.V2.RightFeaturesItemRequest,
        Accessors.Contracts.Api.V2.Requests.AllocationSearchRequest>()
            .ForMember(dest => dest.GeometrySearch, mem => mem.MapFrom(src => src))
            .ForMember(dest => dest.AllocationUuid,
                opt => opt.ConvertUsing(new CommaStringToListConverter(), src => src.AllocationUuids))
            .ForMember(dest => dest.WaterSourceTypes,
                opt => opt.ConvertUsing(new CommaStringToListConverter(), src => src.WaterSourceTypes))
            .ForMember(dest => dest.BeneficialUses,
                opt => opt.ConvertUsing(new CommaStringToListConverter(), src => src.BeneficialUses))
            .ForMember(dest => dest.SiteUuid, opt => opt.ConvertUsing(new CommaStringToListConverter(), src => src.SiteUuids))
            .ForMember(dest => dest.States, opt => opt.ConvertUsing(new CommaStringToListConverter(), src => src.States))
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

        // Accessor -> Engines
        CreateMap<Accessors.Contracts.Api.V2.Responses.SiteSearchResponse,
                Engines.Contracts.Ogc.Requests.OgcFeaturesFormattingRequest>()
            .ForMember(dest => dest.CollectionId, opt => opt.MapFrom(src => Constants.SitesCollectionId))
            .ForMember(dest => dest.Items,
                opt => opt.MapFrom((src, a, b, c) =>
                    c.Mapper.Map<List<Engines.Contracts.SiteFeature>>(src.Sites)));

        CreateMap<Accessors.Contracts.Api.V2.Responses.OverlaySearchResponse,
                Engines.Contracts.Ogc.Requests.OgcFeaturesFormattingRequest>()
            .ForMember(dest => dest.CollectionId, opt => opt.MapFrom(src => Constants.OverlaysCollectionId))
            .ForMember(dest => dest.Items,
                opt => opt.MapFrom((src, a, b, c) =>
                    c.Mapper.Map<List<Engines.Contracts.OverlayFeature>>(src.Overlays)));

        CreateMap<Accessors.Contracts.Api.V2.Responses.AllocationSearchResponse,
                Engines.Contracts.Ogc.Requests.OgcFeaturesFormattingRequest>()
            .ForMember(dest => dest.CollectionId, opt => opt.MapFrom(src => Constants.RightsCollectionId))
            .ForMember(dest => dest.Items,
                opt => opt.MapFrom((src, a, b, c) =>
                    c.Mapper.Map<List<Engines.Contracts.RightFeature>>(src.Allocations)));

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

        CreateMap<Accessors.Contracts.Api.WaterSourceSummary,
            Engines.Contracts.WaterSourceSummary>();
    }
}