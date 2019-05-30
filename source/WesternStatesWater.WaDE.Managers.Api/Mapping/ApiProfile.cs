using AutoMapper;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using ManagerApi = WesternStatesWater.WaDE.Contracts.Api;

namespace WesternStatesWater.WaDE.Managers.Mapping
{
    internal class ApiProfile : Profile
    {
        public ApiProfile()
        {
            CreateMap<AccessorApi.BeneficialUse, ManagerApi.BeneficialUse>();
            CreateMap<AccessorApi.Method, ManagerApi.Method>();
            CreateMap<AccessorApi.WaterAllocationOrganization, ManagerApi.WaterAllocationOrganization>();
            CreateMap<AccessorApi.VariableSpecific, ManagerApi.VariableSpecific>();
            CreateMap<AccessorApi.WaterSource, ManagerApi.WaterSource>();

            CreateMap<AccessorApi.AggregatedAmountsOrganization, ManagerApi.AggregatedAmountsOrganization>();
            CreateMap<AccessorApi.AggregatedAmount, ManagerApi.AggregatedAmount>();
            CreateMap<AccessorApi.ReportingUnit, ManagerApi.ReportingUnit>();

            CreateMap<AccessorApi.SiteVariableAmountsOrganization, ManagerApi.SiteVariableAmountsOrganization>();
            CreateMap<AccessorApi.SiteVariableAmount, ManagerApi.SiteVariableAmount>();
        }
    }
}
