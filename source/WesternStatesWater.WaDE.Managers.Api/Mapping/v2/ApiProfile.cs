using AutoMapper;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;
using ApiRequest = WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using ApiResponse = WesternStatesWater.WaDE.Contracts.Api.Responses.V2;
using Accessor = WesternStatesWater.WaDE.Accessors.Contracts;

namespace WesternStatesWater.WaDE.Managers.Api.Mapping.v2;

public class ApiProfile : Profile
{
    public ApiProfile()
    {
        CreateMap<ApiRequest.SiteCollectionSearchRequest, Accessor.Api.Sites.Requests.SiteExtentSearchRequest>();
        CreateMap<Accessor.Api.Sites.Responses.SiteExtentSearchResponse, Extent>()
            .ForPath(dest => dest.Spatial.Bbox, opt => opt.MapFrom(src => new[]
            {
                new[] { src.BoundaryBox.MinX, src.BoundaryBox.MinY, src.BoundaryBox.MaxX, src.BoundaryBox.MaxY }
            }));    
    }
}