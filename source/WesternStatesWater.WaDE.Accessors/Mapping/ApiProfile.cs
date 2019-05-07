using AutoMapper;
using System.Linq;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using EF = WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Mapping
{
    internal class ApiProfile : Profile
    {
        public ApiProfile()
        {
            AllowNullDestinationValues = true;
            CreateMap<EF.AllocationAmountsFact, AccessorApi.Allocation>()
                .ForMember(a => a.AllocationNativeID, b => b.MapFrom(c => c.AllocationNativeId))
                .ForMember(a => a.AllocationPriorityDate, b => b.MapFrom(c => c.AllocationPriorityDateNavigation.Date))
                .ForMember(a => a.AllocationExpirationDate, b => b.MapFrom(c => c.AllocationExpirationDateNavigation.Date))
                .ForMember(a => a.AllocationApplicationDate, b => b.MapFrom(c => c.AllocationApplicationDateNavigation.Date))
                .ForMember(a => a.DataPublicationDate, b => b.MapFrom(c => c.DataPublicationDate.Date))
                .ForMember(a => a.AllocationLegalStatusCodeCV, b => b.MapFrom(c => c.AllocationLegalStatusCv))
                .ForMember(a => a.AllocationAcreage, b => b.Ignore())
                .ForMember(a => a.WaterSourceUUID, b => b.MapFrom(c => c.WaterSource.WaterSourceUuid))
                .ForMember(a => a.MethodUUID, b => b.MapFrom(c => c.Method.MethodUuid))
                .ForMember(a => a.VariableSpecificTypeCV, b => b.MapFrom(c => c.VariableSpecific.VariableSpecificCv))
                .ForMember(a => a.NativeSiteID, b => b.MapFrom(c => c.Site.SiteNativeId))
                .ForMember(a => a.Latitude, b => b.MapFrom(c => c.Site.Latitude))
                .ForMember(a => a.Longitude, b => b.MapFrom(c => c.Site.Longitude))
                .ForMember(a => a.CoordinateMethodCV, b => b.MapFrom(c => c.Site.CoordinateMethodCv))
                .ForMember(a => a.AllocationGNISIDCV, b => b.MapFrom(c => c.Site.GniscodeCv))
                .ForMember(a => a.SiteGeometry, b => b.MapFrom(c => c.Site.Geometry == null ? null : c.Site.Geometry.AsText()))
                .ForMember(a => a.NHDMetadata, b => b.Ignore())
                .ForMember(a => a.TimeframeStart, b => b.Ignore())
                .ForMember(a => a.TimeframeEnd, b => b.Ignore());
            CreateMap<EF.BeneficialUsesDim, AccessorApi.BeneficialUse>();
            CreateMap<EF.AllocationBridgeBeneficialUsesFact, AccessorApi.BeneficialUse>()
                .ForMember(a => a.BeneficialUseUUID, b => b.MapFrom(c => c.BeneficialUse.BeneficialUseUuid))
                .ForMember(a => a.BeneficialUseCategory, b => b.MapFrom(c => c.BeneficialUse.BeneficialUseCategory))
                .ForMember(a => a.USGSCategoryNameCV, b => b.MapFrom(c => c.BeneficialUse.UsgscategoryNameCv))
                .ForMember(a => a.NAICSCodeNameCV, b => b.MapFrom(c => c.BeneficialUse.NaicscodeNameCv));
            CreateMap<EF.MethodsDim, AccessorApi.Method>()
                .ForMember(a => a.ApplicableResourceType, b => b.MapFrom(c => c.ApplicableResourceTypeCv))
                .ForMember(a => a.DataQualityValue, b => b.MapFrom(c => c.DataQualityValueCv));
            CreateMap<EF.Nhdmetadata, AccessorApi.NHDMetadata>();
            CreateMap<EF.VariablesDim, AccessorApi.VariableSpecific>()
                .ForMember(a => a.VariableSpecificTypeCV, b => b.MapFrom(c => c.VariableSpecificCv));
            CreateMap<EF.WaterSourcesDim, AccessorApi.WaterSource>()
                .ForMember(a => a.WaterSourceNativeID, b => b.MapFrom(c => c.WaterSourceNativeId))
                .ForMember(a => a.FreshSalineIndicatorCV, b => b.MapFrom(c => c.WaterQualityIndicatorCv))
                .ForMember(a => a.WaterSourceGeometry, b => b.MapFrom(c => c.Geometry == null ? null : c.Geometry.AsText()));
            CreateMap<IGrouping<EF.OrganizationsDim, EF.AllocationAmountsFact>, AccessorApi.WaterAllocationOrganization>()
                .ForMember(a => a.OrganizationName, b => b.MapFrom(c => c.Key.OrganizationName))
                .ForMember(a => a.OrganizationPurview, b => b.MapFrom(c => c.Key.OrganizationPurview))
                .ForMember(a => a.OrganizationWebsite, b => b.MapFrom(c => c.Key.OrganizationWebsite))
                .ForMember(a => a.OrganizationPhoneNumber, b => b.MapFrom(c => c.Key.OrganizationPhoneNumber))
                .ForMember(a => a.OrganizationContactName, b => b.MapFrom(c => c.Key.OrganizationContactName))
                .ForMember(a => a.OrganizationContactEmail, b => b.MapFrom(c => c.Key.OrganizationContactEmail))
                .ForMember(a => a.OrganizationState, b => b.Ignore())
                .ForMember(a => a.WaterSources, b => b.MapFrom(c => c.Select(d => d.WaterSource).Distinct()))
                .ForMember(a => a.VariableSpecifics, b => b.MapFrom(c => c.Select(d => d.VariableSpecific).Distinct()))
                .ForMember(a => a.Methods, b => b.MapFrom(c => c.Select(d => d.Method).Distinct()))
                .ForMember(a => a.WaterAllocations, b => b.MapFrom(c => c));

            CreateMap<IGrouping<EF.OrganizationsDim, EF.AggregatedAmountsFact>, AccessorApi.AggregatedAmountsOrganization>()
                .ForMember(a => a.OrganizationName, b => b.MapFrom(c => c.Key.OrganizationName))
                .ForMember(a => a.OrganizationPurview, b => b.MapFrom(c => c.Key.OrganizationPurview))
                .ForMember(a => a.OrganizationWebsite, b => b.MapFrom(c => c.Key.OrganizationWebsite))
                .ForMember(a => a.OrganizationPhoneNumber, b => b.MapFrom(c => c.Key.OrganizationPhoneNumber))
                .ForMember(a => a.OrganizationContactName, b => b.MapFrom(c => c.Key.OrganizationContactName))
                .ForMember(a => a.OrganizationContactEmail, b => b.MapFrom(c => c.Key.OrganizationContactEmail))
                .ForMember(a => a.OrganizationState, b => b.Ignore())
                .ForMember(a => a.ReportingUnits, b => b.MapFrom(c => c.Select(d => d.ReportingUnit).Distinct()))
                .ForMember(a => a.WaterSources, b => b.MapFrom(c => c.Select(d => d.WaterSource).Distinct()))
                .ForMember(a => a.VariableSpecifics, b => b.MapFrom(c => c.Select(d => d.VariableSpecific).Distinct()))
                .ForMember(a => a.Methods, b => b.MapFrom(c => c.Select(d => d.Method).Distinct()))
                .ForMember(a => a.AggregatedAmounts, b => b.MapFrom(c => c));

            CreateMap<EF.AggregatedAmountsFact, AccessorApi.AggregatedAmount>()
                .ForMember(a => a.WaterSourceUUID, b => b.MapFrom(c => c.WaterSource.WaterSourceUuid))
                .ForMember(a => a.MethodUUID, b => b.MapFrom(c => c.Method.MethodUuid))
                .ForMember(a => a.VariableSpecificTypeCV, b => b.MapFrom(c => c.VariableSpecific.VariableSpecificCv))
                .ForMember(a => a.TimeframeStart, b => b.MapFrom(c => c.TimeframeStart.Date))
                .ForMember(a => a.TimeframeEnd, b => b.MapFrom(c => c.TimeframeEnd.Date))
                .ForMember(a => a.Variable, b => b.MapFrom(c => c.VariableSpecific.VariableCv))
                .ForMember(a => a.ReportYear, b => b.MapFrom(c => c.ReportYearCv))
                .ForMember(a => a.ReportingUnitUUID, b => b.MapFrom(c => c.ReportingUnit.ReportingUnitUuid))
                .ForMember(a => a.DataPublicationDate, b => b.MapFrom(c => c.DataPublicationDateNavigation.Date));

            CreateMap<EF.ReportingUnitsDim, AccessorApi.ReportingUnit>()
                .ForMember(a=>a.ReportingUnitGeometry, b=>b.MapFrom(c=> c.Geometry == null ? null : c.Geometry.AsText()));
        }
    }
}
