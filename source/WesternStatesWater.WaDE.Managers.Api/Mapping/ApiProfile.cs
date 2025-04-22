using AutoMapper;

namespace WesternStatesWater.WaDE.Managers.Api.Mapping
{
    internal class ApiProfile : Profile
    {
        public const string GeometryFormatKey = "GeometryFormatKey";

        public ApiProfile()
        {
            CreateMap<ManagerApi.AggregatedAmountsFilters, AccessorApi.AggregatedAmountsFilters>()
                .ForMember(dest => dest.Geometry,
                    opt => opt.ConvertUsing(new StringToGeometryConverter(), src => src.Geometry));
            CreateMap<ManagerApi.SiteAllocationAmountsFilters, AccessorApi.SiteAllocationAmountsFilters>()
                .ForMember(dest => dest.Geometry,
                    opt => opt.ConvertUsing(new StringToGeometryConverter(), src => src.Geometry));;
            CreateMap<ManagerApi.SiteVariableAmountsFilters, AccessorApi.SiteVariableAmountsFilters>()
                .ForMember(dest => dest.Geometry,
                    opt => opt.ConvertUsing(new StringToGeometryConverter(), src => src.Geometry));;
            CreateMap<ManagerApi.RegulatoryOverlayFilters, AccessorApi.RegulatoryOverlayFilters>()
                .ForMember(dest => dest.Geometry,
                    opt => opt.ConvertUsing(new StringToGeometryConverter(), src => src.Geometry));;
            CreateMap<ManagerApi.SiteAllocationAmountsDigestFilters, AccessorApi.SiteAllocationAmountsDigestFilters>()
                .ForMember(dest => dest.Geometry,
                    opt => opt.ConvertUsing(new StringToGeometryConverter(), src => src.Geometry));;

            CreateMap<AccessorApi.BeneficialUse, ManagerApi.BeneficialUse>();
            CreateMap<AccessorApi.Site, ManagerApi.Site>()
                .ForMember(dest => dest.SiteGeometry, opt => opt.MapFrom(new GeometryFormatterResolver<AccessorApi.Site, ManagerApi.Site>(GeometryFormatKey, nameof(AccessorApi.Site.SiteGeometry))));
            CreateMap<AccessorApi.Method, ManagerApi.Method>();
            CreateMap<AccessorApi.VariableSpecific, ManagerApi.VariableSpecific>();
            CreateMap<AccessorApi.WaterSource, ManagerApi.WaterSource>()
                .ForMember(dest => dest.WaterSourceGeometry, opt => opt.MapFrom(new GeometryFormatterResolver<AccessorApi.WaterSource, ManagerApi.WaterSource>(GeometryFormatKey, nameof(AccessorApi.WaterSource.WaterSourceGeometry))));
            CreateMap<AccessorApi.ReportingUnit, ManagerApi.ReportingUnit>()
                .ForMember(dest => dest.ReportingUnitGeometry, opt => opt.MapFrom(new GeometryFormatterResolver<AccessorApi.ReportingUnit, ManagerApi.ReportingUnit>(GeometryFormatKey, nameof(AccessorApi.ReportingUnit.ReportingUnitGeometry))));

            CreateMap<AccessorApi.WaterAllocations, ManagerApi.WaterAllocations>();
            CreateMap<AccessorApi.WaterAllocationOrganization, ManagerApi.WaterAllocationOrganization>();
            CreateMap<AccessorApi.Allocation, ManagerApi.Allocation>();

            CreateMap<AccessorApi.AggregatedAmounts, ManagerApi.AggregatedAmounts>();
            CreateMap<AccessorApi.AggregatedAmountsOrganization, ManagerApi.AggregatedAmountsOrganization>();
            CreateMap<AccessorApi.AggregatedAmount, ManagerApi.AggregatedAmount>();

            CreateMap<AccessorApi.SiteVariableAmounts, ManagerApi.SiteVariableAmounts>();
            CreateMap<AccessorApi.SiteVariableAmountsOrganization, ManagerApi.SiteVariableAmountsOrganization>();
            CreateMap<AccessorApi.SiteVariableAmount, ManagerApi.SiteVariableAmount>();

            CreateMap<AccessorApi.RegulatoryReportingUnitsOrganization,
                ManagerApi.RegulatoryReportingUnitsOrganization>();
            CreateMap<AccessorApi.RegulatoryOverlay, ManagerApi.RegulatoryOverlay>()
                .ForMember(dest => dest.RegulatoryOverlayID, opt => opt.MapFrom(src => src.OverlayID))
                .ForMember(dest => dest.RegulatoryOverlayUUID, opt => opt.MapFrom(src => src.OverlayUUID))
                .ForMember(dest => dest.RegulatoryDescription, opt => opt.MapFrom(src => src.OverlayDescription))
                .ForMember(dest => dest.RegulatoryOverlayTypeCV, opt => opt.MapFrom(src => src.OverlayTypeCV))
                .ForMember(dest => dest.RegulatoryStatuteLink, opt => opt.MapFrom(src => src.StatuteLink));
            CreateMap<AccessorApi.ReportingUnitRegulatory, ManagerApi.ReportingUnitRegulatory>();

            CreateMap<AccessorApi.RegulatoryReportingUnits, ManagerApi.RegulatoryReportingUnits>();

            CreateMap<AccessorApi.WaterAllocationsDigest, ManagerApi.WaterAllocationDigest>();
            CreateMap<AccessorApi.SiteDigest, ManagerApi.SiteDigest>();

            CreateMap<AccessorApi.PodToPouSiteRelationship, ManagerApi.PodToPouSiteRelationship>();
        }
    }
}