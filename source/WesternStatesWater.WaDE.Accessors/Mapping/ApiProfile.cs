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
                .ForMember(a => a.AllocationSDWISIdentifier, b => b.MapFrom(c => c.SdwisidentifierCV))
                .ForMember(a => a.AllocationAcreage, b => b.Ignore())
                .ForMember(a => a.WaterSourceUUID, b => b.MapFrom(c => c.WaterSource.WaterSourceUuid))
                .ForMember(a => a.MethodUUID, b => b.MapFrom(c => c.Method.MethodUuid))
                .ForMember(a => a.VariableSpecificTypeCV, b => b.MapFrom(c => c.VariableSpecific.VariableSpecificCv))
                .ForMember(a => a.TimeframeStart, b => b.Ignore())
                .ForMember(a => a.TimeframeEnd, b => b.Ignore())
                .ForMember(a => a.BeneficialUses, b => b.Ignore())
                .ForMember(a => a.Sites, b => b.Ignore());

            CreateMap<EF.BeneficialUsesCV, AccessorApi.BeneficialUse>()
                .ForMember(a => a.USGSCategory, b => b.MapFrom(c => c.UsgscategoryNameCv))
                .ForMember(a => a.NAICSCode, b => b.MapFrom(c => c.NaicscodeNameCv));

            CreateMap<EF.SitesDim, AccessorApi.Site>()
                 .ForMember(a => a.NativeSiteID, b => b.MapFrom(c => c.SiteNativeId))
                 .ForMember(a => a.Latitude, b => b.MapFrom(c => c.Latitude))
                 .ForMember(a => a.Longitude, b => b.MapFrom(c => c.Longitude))
                 .ForMember(a => a.CoordinateMethodCV, b => b.MapFrom(c => c.CoordinateMethodCv))
                 .ForMember(a => a.AllocationGNISIDCV, b => b.MapFrom(c => c.GniscodeCv))
                 .ForMember(a => a.SiteGeometry, b => b.MapFrom(c => c.Geometry == null ? null : c.Geometry.AsText()))
                 .ForMember(a => a.County, b => b.MapFrom(c => c.County))
                 .ForMember(a => a.HUC8, b => b.MapFrom(c => c.HUC8))
                 .ForMember(a => a.HUC12, b => b.MapFrom(c => c.HUC12));

            CreateMap<EF.AllocationBridgeBeneficialUsesFact, AccessorApi.BeneficialUse>()
                .ForMember(a => a.Term, b => b.MapFrom(c => c.BeneficialUse.Term))
                .ForMember(a => a.State, b => b.MapFrom(c => c.BeneficialUse.State))
                .ForMember(a => a.Definition, b => b.MapFrom(c => c.BeneficialUse.Definition))
                .ForMember(a => a.SourceVocabularyURI, b => b.MapFrom(c => c.BeneficialUse.SourceVocabularyURI))

                .ForMember(a => a.Name, b => b.MapFrom(c => c.BeneficialUse.Name))
                .ForMember(a => a.USGSCategory, b => b.MapFrom(c => c.BeneficialUse.UsgscategoryNameCv))
                .ForMember(a => a.NAICSCode, b => b.MapFrom(c => c.BeneficialUse.NaicscodeNameCv));

            CreateMap<EF.MethodsDim, AccessorApi.Method>()
                .ForMember(a => a.ApplicableResourceType, b => b.MapFrom(c => c.ApplicableResourceTypeCv))
                .ForMember(a => a.DataQualityValue, b => b.MapFrom(c => c.DataQualityValueCv));

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
                .ForMember(a => a.BeneficialUses, b => b.MapFrom(c => c.Where(d => d.PrimaryUseCategoryCV != null).Select(d => d.PrimaryBeneficialUse).Union(c.SelectMany(d => d.AllocationBridgeBeneficialUsesFact.Select(e => e.BeneficialUse))).Distinct()))
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
                .ForMember(a => a.BeneficialUses, b => b.MapFrom(c => c.Select(d => d.BeneficialUse).Union(c.SelectMany(d => d.AggBridgeBeneficialUsesFact.Select(e => e.BeneficialUse))).Distinct()))
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
                .ForMember(a => a.DataPublicationDate, b => b.MapFrom(c => c.DataPublicationDateNavigation.Date))
                .ForMember(a => a.BeneficialUses, b => b.Ignore())
                .ForMember(a => a.PrimaryUse, b => b.MapFrom(c => c.BeneficialUse.Name));

            CreateMap<IGrouping<EF.OrganizationsDim, EF.SiteVariableAmountsFact>, AccessorApi.SiteVariableAmountsOrganization>()
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
                .ForMember(a => a.BeneficialUses, b => b.MapFrom(c => c.SelectMany(d => d.SitesBridgeBeneficialUsesFact.Select(e => e.BeneficialUse)).Distinct()))
                .ForMember(a => a.SiteVariableAmounts, b => b.MapFrom(c => c));

            CreateMap<EF.SiteVariableAmountsFact, AccessorApi.SiteVariableAmount>()
                .ForMember(a => a.SiteName, b => b.MapFrom(c => c.Site.SiteName))
                .ForMember(a => a.SDWISIdentifier, b => b.MapFrom(c => c.SDWISIdentifierCv))
                .ForMember(a => a.NativeSiteID, b => b.MapFrom(c => c.Site.SiteNativeId))
                .ForMember(a => a.SiteTypeCV, b => b.MapFrom(c => c.Site.SiteTypeCv))
                .ForMember(a => a.Longitude, b => b.MapFrom(c => c.Site.Longitude))
                .ForMember(a => a.Latitude, b => b.MapFrom(c => c.Site.Latitude))
                .ForMember(a => a.CoordinateMethodCV, b => b.MapFrom(c => c.Site.CoordinateMethodCv))
                .ForMember(a => a.MethodUUID, b => b.MapFrom(c => c.Method.MethodUuid))
                .ForMember(a => a.VariableSpecificTypeCV, b => b.MapFrom(c => c.VariableSpecificId))
                .ForMember(a => a.SiteUUID, b => b.MapFrom(c => c.Site.SiteUuid))
                .ForMember(a => a.AllocationCommunityWaterSupplySystem, b => b.MapFrom(c => c.CommunityWaterSupplySystem))
                .ForMember(a => a.DataPublicationDate, b => b.MapFrom(c => c.DataPublicationDateNavigation.Date))
                .ForMember(a => a.TimeframeStart, b => b.MapFrom(c => c.TimeframeStartNavigation.Date))
                .ForMember(a => a.TimeframeEnd, b => b.MapFrom(c => c.TimeframeEndNavigation.Date))
                .ForMember(a => a.SiteGeometry, b => b.MapFrom(c => c.Geometry == null ? null : c.Geometry.AsText()))
                .ForMember(a => a.AllocationGNISIDCV, b => b.Ignore())
                .ForMember(a => a.AllocationCropDutyAmount, b => b.Ignore())
                .ForMember(a => a.BeneficialUses, b => b.Ignore())
                .ForMember(a => a.HUC8, b => b.MapFrom(c => c.Site.HUC8))
                .ForMember(a => a.HUC12, b => b.MapFrom(c => c.Site.HUC12))
                .ForMember(a => a.County, b => b.MapFrom(c => c.Site.County));

            CreateMap<EF.ReportingUnitsDim, AccessorApi.ReportingUnit>()
                .ForMember(a => a.ReportingUnitGeometry, b => b.MapFrom(c => c.Geometry == null ? null : c.Geometry.AsText()));
        }
    }
}
