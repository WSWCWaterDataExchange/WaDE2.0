using AutoMapper;

namespace WesternStatesWater.WaDE.Managers.Api.Mapping;

public class OgcApiProfile : Profile
{
    public OgcApiProfile()
    {
        CreateMap<Engines.Contracts.Ogc.Collection, Contracts.Api.OgcApi.Collection>();
        CreateMap<Engines.Contracts.Ogc.Link, Contracts.Api.OgcApi.Link>();
        CreateMap<Engines.Contracts.Ogc.Extent, Contracts.Api.OgcApi.Extent>();
        CreateMap<Engines.Contracts.Ogc.Spatial, Contracts.Api.OgcApi.Spatial>();
        CreateMap<Engines.Contracts.Ogc.Temporal, Contracts.Api.OgcApi.Temporal>();
        
        CreateMap<Engines.Contracts.Ogc.Responses.CollectionsResponse, Contracts.Api.OgcApi.CollectionsResponse>();

        CreateMap<Contracts.Api.Requests.V2.CollectionMetadataGetRequest,
            Engines.Contracts.Ogc.Requests.CollectionRequest>();
        CreateMap<Engines.Contracts.Ogc.Responses.CollectionResponse,
            Contracts.Api.Responses.V2.CollectionMetadataGetResponse>();
        CreateMap<Engines.Contracts.Ogc.Responses.CollectionsResponse,
            Contracts.Api.Responses.V2.CollectionsMetadataGetResponse>();
    }
}