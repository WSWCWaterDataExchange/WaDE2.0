using System.Linq;
using AutoMapper;
using WesternStatesWater.WaDE.Contracts.Api.Requests;

namespace WesternStatesWater.WaDE.Managers.Api.Mapping;

public class OgcApiProfile : Profile
{
    public OgcApiProfile()
    {
        CreateMap<string, double[][]>().ConvertUsing<StringToBoundingBoxConverter>();
        
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

        CreateMap<Contracts.Api.Requests.V2.SiteFeaturesSearchRequest,
                Engines.Contracts.Ogc.Requests.SiteFeaturesRequest>()
            .ForMember(dest => dest.BoundingBox, mem => mem.MapFrom(src => src.Bbox))
            .ForMember(dest => dest.LastSiteUuid, mem => mem.MapFrom(src => src.Next));

        CreateMap<Engines.Contracts.Ogc.Responses.SiteFeaturesResponse,
            Contracts.Api.Responses.V2.SiteFeaturesSearchResponse>();

        CreateMap<FeaturesSearchRequestBase,
            Engines.Contracts.Ogc.Requests.FeaturesRequestBase>();
    }
}

// This method is assuming Validators are in place to ensure the string is in the correct format.
public class StringToBoundingBoxConverter : ITypeConverter<string, double[][]>
{
    public double[][] Convert(string source, double[][] destination, ResolutionContext context)
    {
        if (source == null)
        {
            return null;
        }

        var bbox = source.Split(",").Select(double.Parse).ToArray();
        return [bbox];
    }
}