using AutoMapper;
using ApiRequest = WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using Accessor = WesternStatesWater.WaDE.Accessors.Contracts;

namespace WesternStatesWater.WaDE.Managers.Api.Mapping.v2;

public class ApiProfile : Profile
{
    public ApiProfile()
    {
        CreateMap<ApiRequest.SiteCollectionSearchRequest, Accessor.Api.Sites.Requests.SiteExtentSearchRequest>();
        CreateMap<Accessor.Api.Sites.Responses.SiteExtentSearchResponse,
            Accessor.Api.Sites.Responses.SiteExtentSearchResponse>();
    }
}