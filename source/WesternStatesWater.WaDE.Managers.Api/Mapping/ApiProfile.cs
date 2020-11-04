using AutoMapper;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using ManagerApi = WesternStatesWater.WaDE.Contracts.Api;

namespace WesternStatesWater.WaDE.Managers.Mapping
{
    internal class ApiProfile : Profile
    {
        public ApiProfile()
        {

            CreateMap<ManagerApi.AggregatedAmountsFilters, AccessorApi.AggregatedAmountsFilters>();
            CreateMap<ManagerApi.SiteAllocationAmountsFilters, AccessorApi.SiteAllocationAmountsFilters>();
            CreateMap<ManagerApi.SiteVariableAmountsFilters, AccessorApi.SiteVariableAmountsFilters>();
            CreateMap<ManagerApi.RegulatoryOverlayFilters, AccessorApi.RegulatoryOverlayFilters>();
            CreateMap<ManagerApi.SiteAllocationAmountsDigestFilters, AccessorApi.SiteAllocationAmountsDigestFilters>();

            CreateMap<AccessorApi.BeneficialUse, ManagerApi.BeneficialUse>();
            CreateMap<AccessorApi.Site, ManagerApi.Site>();
            CreateMap<AccessorApi.Method, ManagerApi.Method>();
            CreateMap<AccessorApi.VariableSpecific, ManagerApi.VariableSpecific>();
            CreateMap<AccessorApi.WaterSource, ManagerApi.WaterSource>();
            CreateMap<AccessorApi.ReportingUnit, ManagerApi.ReportingUnit>();

            CreateMap<AccessorApi.WaterAllocations, ManagerApi.WaterAllocations>();
            CreateMap<AccessorApi.WaterAllocationOrganization, ManagerApi.WaterAllocationOrganization>();
            CreateMap<AccessorApi.Allocation, ManagerApi.Allocation>();

            CreateMap<AccessorApi.AggregatedAmounts, ManagerApi.AggregatedAmounts>();
            CreateMap<AccessorApi.AggregatedAmountsOrganization, ManagerApi.AggregatedAmountsOrganization>();
            CreateMap<AccessorApi.AggregatedAmount, ManagerApi.AggregatedAmount>();

            CreateMap<AccessorApi.SiteVariableAmounts, ManagerApi.SiteVariableAmounts>();
            CreateMap<AccessorApi.SiteVariableAmountsOrganization, ManagerApi.SiteVariableAmountsOrganization>();
            CreateMap<AccessorApi.SiteVariableAmount, ManagerApi.SiteVariableAmount>();

            CreateMap<AccessorApi.RegulatoryReportingUnitsOrganization, ManagerApi.RegulatoryReportingUnitsOrganization>();
            CreateMap<AccessorApi.RegulatoryOverlay, ManagerApi.RegulatoryOverlay>();
            CreateMap<AccessorApi.ReportingUnitRegulatory, ManagerApi.ReportingUnitRegulatory>();

            CreateMap<AccessorApi.RegulatoryReportingUnits, ManagerApi.RegulatoryReportingUnits>();

            CreateMap<AccessorApi.WaterAllocationsDigest, ManagerApi.WaterAllocationDigest>();
            CreateMap<AccessorApi.SiteDigest, ManagerApi.SiteDigest>();
        }
    }
}
