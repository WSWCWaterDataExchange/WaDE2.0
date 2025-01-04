using System;
using System.Linq;
using AutoMapper;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Common;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using EF = WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Mapping
{
    internal class ApiProfile : Profile
    {
        public const string PodPouKey = "PodPouKey";
        public const string PodValue = "POD";
        public const string PouValue = "POU";

        public ApiProfile()
        {
            AllowNullDestinationValues = true;
            CreateMap<EF.AllocationAmountsFact, AccessorApi.Allocation>()
                .ForMember(a => a.AllocationApplicationDate,
                    b => b.MapFrom(c => c.AllocationApplicationDateNavigation.Date))
                .ForMember(a => a.AllocationPriorityDate, b => b.MapFrom(c => c.AllocationPriorityDateNavigation.Date))
                .ForMember(a => a.AllocationLegalStatusCodeCV, b => b.MapFrom(c => c.AllocationLegalStatusCv))
                .ForMember(a => a.AllocationExpirationDate,
                    b => b.MapFrom(c => c.AllocationExpirationDateNavigation.Date))
                .ForMember(a => a.AllocationAcreage, b => b.MapFrom(c => c.IrrigatedAcreage))
                .ForMember(a => a.TimeframeStart, b => b.MapFrom(c => DateTime.Parse(c.AllocationTimeframeStart)))
                .ForMember(a => a.TimeframeEnd, b => b.MapFrom(c => DateTime.Parse(c.AllocationTimeframeEnd)))
                .ForMember(a => a.AllocationSDWISIdentifier, b => b.MapFrom(c => c.SDWISIdentifier))
                .ForMember(a => a.MethodUUID, b => b.MapFrom(c => c.Method.MethodUuid))
                .ForMember(a => a.VariableSpecificTypeCV, b => b.MapFrom(c => c.VariableSpecific.VariableSpecificCv))
                .ForMember(a => a.BeneficialUses,
                    b => b.MapFrom(c => c.AllocationBridgeBeneficialUsesFact.Select(d => d.BeneficialUseCV)))
                .ForMember(a => a.DataPublicationDate, b => b.MapFrom(c => c.DataPublicationDate.Date))
                .ForMember(a => a.SitesUUIDs,
                    b => b.MapFrom(c => c.AllocationBridgeSitesFact.Select(d => d.Site.SiteUuid)));

            CreateMap<WaterAllocationAccessor.AllocationHelper, AccessorApi.Allocation>()
                .ForMember(a => a.BeneficialUses, b => b.Ignore())
                .ForMember(a => a.SitesUUIDs, b => b.Ignore());
            CreateMap<EF.OrganizationsDim, AccessorApi.WaterAllocationOrganization>()
                .ForMember(a => a.OrganizationState, b => b.MapFrom(c => c.State))
                .ForMember(a => a.WaterSources, b => b.Ignore())
                .ForMember(a => a.VariableSpecifics, b => b.Ignore())
                .ForMember(a => a.Methods, b => b.Ignore())
                .ForMember(a => a.BeneficialUses, b => b.Ignore())
                .ForMember(a => a.WaterAllocations, b => b.Ignore())
                .ForMember(a => a.RegulatoryOverlays, b => b.Ignore())
                .ForMember(a => a.Sites, b => b.Ignore());

            CreateMap<EF.AllocationAmountsFact, WaterAllocationAccessor.AllocationHelper>()
                .ForMember(a => a.AllocationNativeID, b => b.MapFrom(c => c.AllocationNativeId))
                .ForMember(a => a.AllocationPriorityDate, b => b.MapFrom(c => c.AllocationPriorityDateNavigation.Date))
                .ForMember(a => a.AllocationExpirationDate,
                    b => b.MapFrom(c => c.AllocationExpirationDateNavigation.Date))
                .ForMember(a => a.AllocationApplicationDate,
                    b => b.MapFrom(c => c.AllocationApplicationDateNavigation.Date))
                .ForMember(a => a.DataPublicationDate, b => b.MapFrom(c => c.DataPublicationDate.Date))
                .ForMember(a => a.AllocationLegalStatusCodeCV, b => b.MapFrom(c => c.AllocationLegalStatusCv))
                .ForMember(a => a.AllocationSDWISIdentifier, b => b.MapFrom(c => c.SdwisidentifierCV))
                .ForMember(a => a.AllocationAcreage, b => b.Ignore())
                .ForMember(a => a.MethodUUID, b => b.MapFrom(c => c.Method.MethodUuid))
                .ForMember(a => a.VariableSpecificTypeCV, b => b.MapFrom(c => c.VariableSpecific.VariableSpecificCv))
                .ForMember(a => a.TimeframeStart, b => b.Ignore())
                .ForMember(a => a.TimeframeEnd, b => b.Ignore());

            CreateMap<EF.BeneficialUsesCV, AccessorApi.BeneficialUse>()
                .ForMember(a => a.USGSCategory, b => b.MapFrom(c => c.UsgscategoryNameCv))
                .ForMember(a => a.NAICSCode, b => b.MapFrom(c => c.NaicscodeNameCv));

            CreateMap<EF.SitesDim, AccessorApi.Site>()
                .ForMember(a => a.NativeSiteID, b => b.MapFrom(c => c.SiteNativeId))
                .ForMember(a => a.SiteUUID, b => b.MapFrom(c => c.SiteUuid))
                .ForMember(a => a.SiteName, b => b.MapFrom(c => c.SiteName))
                .ForMember(a => a.SiteTypeCV, b => b.MapFrom(c => c.SiteTypeCv))
                .ForMember(a => a.Latitude, b => b.MapFrom(c => c.Latitude))
                .ForMember(a => a.Longitude, b => b.MapFrom(c => c.Longitude))
                .ForMember(a => a.CoordinateMethodCV, b => b.MapFrom(c => c.CoordinateMethodCv))
                .ForMember(a => a.AllocationGNISIDCV, b => b.MapFrom(c => c.GniscodeCv))
                .ForMember(a => a.SiteGeometry, b => b.MapFrom(c => c.Geometry))
                .ForMember(a => a.County, b => b.MapFrom(c => c.County))
                .ForMember(a => a.HUC8, b => b.MapFrom(c => c.HUC8))
                .ForMember(a => a.HUC12, b => b.MapFrom(c => c.HUC12))
                .ForMember(a => a.WaterSourceUUIDs, b => b.Ignore())
                .ForMember(a => a.RelatedPODSites, b => b.MapFrom(c => c.PODSiteToPOUSitePODFact))
                .ForMember(a => a.RelatedPOUSites, b => b.MapFrom(c => c.PODSiteToPOUSitePOUFact));

            CreateMap<EF.PODSiteToPOUSiteFact, AccessorApi.PodToPouSiteRelationship>()
                .ForMember(a => a.SiteUUID, b => b.MapFrom<PODSiteToPOUSiteFactToSiteUuidResolver>());

            CreateMap<EF.AllocationBridgeBeneficialUsesFact, AccessorApi.BeneficialUse>()
                .ForMember(a => a.Term, b => b.MapFrom(c => c.BeneficialUse.Term))
                .ForMember(a => a.State, b => b.MapFrom(c => c.BeneficialUse.State))
                .ForMember(a => a.Definition, b => b.MapFrom(c => c.BeneficialUse.Definition))
                .ForMember(a => a.SourceVocabularyURI, b => b.MapFrom(c => c.BeneficialUse.SourceVocabularyUri))
                .ForMember(a => a.Name, b => b.MapFrom(c => c.BeneficialUse.Name))
                .ForMember(a => a.USGSCategory, b => b.MapFrom(c => c.BeneficialUse.UsgscategoryNameCv))
                .ForMember(a => a.NAICSCode, b => b.MapFrom(c => c.BeneficialUse.NaicscodeNameCv));

            CreateMap<EF.MethodsDim, AccessorApi.Method>()
                .ForMember(a => a.ApplicableResourceType, b => b.MapFrom(c => c.ApplicableResourceTypeCv))
                .ForMember(a => a.DataQualityValue, b => b.MapFrom(c => c.DataQualityValueCv));

            CreateMap<EF.VariablesDim, AccessorApi.VariableSpecific>()
                .ForMember(a => a.VariableSpecificUUID, b => b.MapFrom(c => c.VariableSpecificUuid))
                .ForMember(a => a.VariableSpecificTypeCV, b => b.MapFrom(c => c.VariableSpecificCv));

            CreateMap<EF.WaterSourcesDim, AccessorApi.WaterSource>()
                .ForMember(a => a.WaterSourceNativeID, b => b.MapFrom(c => c.WaterSourceNativeId))
                .ForMember(a => a.FreshSalineIndicatorCV, b => b.MapFrom(c => c.WaterQualityIndicatorCv))
                .ForMember(a => a.WaterSourceGeometry, b => b.MapFrom(c => c.Geometry));


            CreateMap<EF.OrganizationsDim, AccessorApi.AggregatedAmountsOrganization>()
                .ForMember(a => a.OrganizationState, b => b.MapFrom(c => c.State))
                .ForMember(a => a.ReportingUnits, b => b.Ignore())
                .ForMember(a => a.WaterSources, b => b.Ignore())
                .ForMember(a => a.VariableSpecifics, b => b.Ignore())
                .ForMember(a => a.Methods, b => b.Ignore())
                .ForMember(a => a.BeneficialUses, b => b.Ignore())
                .ForMember(a => a.AggregatedAmounts, b => b.Ignore())
                .ForMember(a => a.RegulatoryOverlays, b => b.Ignore());

            CreateMap<EF.AggregatedAmountsFact, AggregratedAmountsAccessor.AggregatedHelper>()
                .ForMember(a => a.WaterSourceUUID, b => b.MapFrom(c => c.WaterSource.WaterSourceUuid))
                .ForMember(a => a.MethodUUID, b => b.MapFrom(c => c.Method.MethodUuid))
                .ForMember(a => a.VariableSpecificTypeCV, b => b.MapFrom(c => c.VariableSpecific.VariableSpecificCv))
                .ForMember(a => a.TimeframeStart, b => b.MapFrom(c => c.TimeframeStart.Date))
                .ForMember(a => a.TimeframeEnd, b => b.MapFrom(c => c.TimeframeEnd.Date))
                .ForMember(a => a.Variable, b => b.MapFrom(c => c.VariableSpecific.VariableCv))
                .ForMember(a => a.ReportYear, b => b.MapFrom(c => c.ReportYearCv))
                .ForMember(a => a.ReportingUnitUUID, b => b.MapFrom(c => c.ReportingUnit.ReportingUnitUuid))
                .ForMember(a => a.DataPublicationDate, b => b.MapFrom(c => c.DataPublicationDateNavigation.Date))
                .ForMember(a => a.PrimaryUse, b => b.MapFrom(c => c.PrimaryBeneficialUse.Name));

            CreateMap<AggregratedAmountsAccessor.AggregatedHelper, AccessorApi.AggregatedAmount>()
                .ForMember(a => a.BeneficialUses, b => b.Ignore());

            CreateMap<EF.OrganizationsDim, AccessorApi.SiteVariableAmountsOrganization>()
                .ForMember(a => a.OrganizationState, b => b.MapFrom(c => c.State))
                .ForMember(a => a.WaterSources, b => b.Ignore())
                .ForMember(a => a.VariableSpecifics, b => b.Ignore())
                .ForMember(a => a.Methods, b => b.Ignore())
                .ForMember(a => a.BeneficialUses, b => b.Ignore())
                .ForMember(a => a.SiteVariableAmounts, b => b.Ignore())
                .ForMember(a => a.Sites, b => b.Ignore());

            CreateMap<SiteVariableAmountsAccessor.SiteVariableAmountHelper, AccessorApi.SiteVariableAmount>()
                .ForMember(a => a.BeneficialUses, b => b.Ignore());

            CreateMap<EF.SiteVariableAmountsFact, SiteVariableAmountsAccessor.SiteVariableAmountHelper>()
                .ForMember(a => a.WaterSourceUUID, b => b.MapFrom(c => c.WaterSource.WaterSourceUuid))
                .ForMember(a => a.SDWISIdentifier, b => b.MapFrom(c => c.SDWISIdentifierCv))
                .ForMember(a => a.MethodUUID, b => b.MapFrom(c => c.Method.MethodUuid))
                .ForMember(a => a.VariableSpecificUUID, b => b.MapFrom(c => c.VariableSpecific.VariableSpecificUuid))
                .ForMember(a => a.SiteUUID, b => b.MapFrom(c => c.Site.SiteUuid))
                .ForMember(a => a.AllocationCommunityWaterSupplySystem,
                    b => b.MapFrom(c => c.CommunityWaterSupplySystem))
                .ForMember(a => a.DataPublicationDate, b => b.MapFrom(c => c.DataPublicationDateNavigation.Date))
                .ForMember(a => a.TimeframeStart, b => b.MapFrom(c => c.TimeframeStartNavigation.Date))
                .ForMember(a => a.TimeframeEnd, b => b.MapFrom(c => c.TimeframeEndNavigation.Date))
                .ForMember(a => a.AllocationGNISIDCV, b => b.Ignore())
                .ForMember(a => a.AllocationCropDutyAmount, b => b.Ignore());

            CreateMap<EF.SiteVariableAmountsFact, AccessorApi.SiteVariableAmount>()
                .ForMember(a => a.WaterSourceUUID, b => b.MapFrom(c => c.WaterSource.WaterSourceUuid))
                .ForMember(a => a.AllocationGNISIDCV, b => b.MapFrom(c => c.Site.GniscodeCv))
                .ForMember(a => a.TimeframeStart, b => b.MapFrom(c => c.TimeframeStartNavigation.Date))
                .ForMember(a => a.TimeframeEnd, b => b.MapFrom(c => c.TimeframeEndNavigation.Date))
                .ForMember(a => a.DataPublicationDate, b => b.MapFrom(c => c.DataPublicationDateNavigation.Date))
                .ForMember(a => a.AllocationCropDutyAmount, b => b.MapFrom(c => c.AllocationCropDutyAmount))
                .ForMember(a => a.Amount, b => b.MapFrom(c => c.Amount))
                .ForMember(a => a.IrrigationMethodCV, b => b.MapFrom(c => c.IrrigationMethodCv))
                .ForMember(a => a.IrrigatedAcreage, b => b.MapFrom(c => c.IrrigatedAcreage))
                .ForMember(a => a.CropTypeCV, b => b.MapFrom(c => c.CropTypeCv))
                .ForMember(a => a.PopulationServed, b => b.MapFrom(c => c.PopulationServed))
                .ForMember(a => a.PowerGeneratedGWh, b => b.MapFrom(c => c.PowerGeneratedGwh))
                .ForMember(a => a.AllocationCommunityWaterSupplySystem,
                    b => b.MapFrom(c => c.CommunityWaterSupplySystem))
                .ForMember(a => a.SDWISIdentifier, b => b.MapFrom(c => c.SDWISIdentifierCv))
                .ForMember(a => a.DataPublicationDOI, b => b.MapFrom(c => c.DataPublicationDoi))
                .ForMember(a => a.ReportYearCV, b => b.MapFrom(c => c.ReportYearCv))
                .ForMember(a => a.MethodUUID, b => b.MapFrom(c => c.Method.MethodUuid))
                .ForMember(a => a.VariableSpecificUUID, b => b.MapFrom(c => c.VariableSpecific.VariableSpecificUuid))
                .ForMember(a => a.SiteUUID, b => b.MapFrom(c => c.Site.SiteUuid))
                .ForMember(a => a.AssociatedNativeAllocationIDs, b => b.MapFrom(c => c.AssociatedNativeAllocationIds))
                .ForMember(a => a.BeneficialUses,
                    b => b.MapFrom(c => c.SitesBridgeBeneficialUsesFact.Select(d => d.BeneficialUseCV)));

            CreateMap<EF.ReportingUnitsDim, AccessorApi.ReportingUnit>()
                .ForMember(a => a.ReportingUnitGeometry, b => b.MapFrom(c => c.Geometry))
                .ForMember(a => a.RegulatoryOverlayUUIDs, b => b.Ignore());

            CreateMap<EF.RegulatoryReportingUnitsFact, RegulatoryOverlayAccessor.ReportingUnitRegulatoryHelper>()
                .ForMember(a => a.ReportingUnitUUID, b => b.MapFrom(c => c.ReportingUnit.ReportingUnitUuid))
                .ForMember(a => a.ReportingUnitNativeID, b => b.MapFrom(c => c.ReportingUnit.ReportingUnitNativeId))
                .ForMember(a => a.ReportingUnitName, b => b.MapFrom(c => c.ReportingUnit.ReportingUnitName))
                .ForMember(a => a.ReportingUnitTypeCV, b => b.MapFrom(c => c.ReportingUnit.ReportingUnitTypeCv))
                .ForMember(a => a.ReportingUnitUpdateDate, b => b.MapFrom(c => c.ReportingUnit.ReportingUnitUpdateDate))
                .ForMember(a => a.ReportingUnitProductVersion,
                    b => b.MapFrom(c => c.ReportingUnit.ReportingUnitProductVersion))
                .ForMember(a => a.StateCV, b => b.MapFrom(c => c.ReportingUnit.StateCv))
                .ForMember(a => a.EPSGCodeCV, b => b.MapFrom(c => c.ReportingUnit.EpsgcodeCv))
                .ForMember(a => a.Geometry, b => b.MapFrom(c => c.ReportingUnit.Geometry));
            CreateMap<EF.OrganizationsDim, AccessorApi.RegulatoryReportingUnitsOrganization>()
                .ForMember(a => a.OrganizationState, b => b.MapFrom(c => c.State))
                .ForMember(a => a.RegulatoryOverlays, b => b.Ignore())
                .ForMember(a => a.ReportingUnitsRegulatory, b => b.Ignore());
            CreateMap<EF.RegulatoryOverlayDim, AccessorApi.RegulatoryOverlay>();
            CreateMap<RegulatoryOverlayAccessor.ReportingUnitRegulatoryHelper, AccessorApi.ReportingUnitRegulatory>();
        }

        private class PODSiteToPOUSiteFactToSiteUuidResolver : IValueResolver<EF.PODSiteToPOUSiteFact,
            AccessorApi.PodToPouSiteRelationship, string>
        {
            public string Resolve(PODSiteToPOUSiteFact podSiteToPouSiteFact, PodToPouSiteRelationship destination,
                string destMember, ResolutionContext context)
            {
                if (podSiteToPouSiteFact == null)
                {
                    return null;
                }

                if (!context.Items.TryGetValue(PodPouKey, out var obj))
                {
                    throw new WaDEException("POD/POU key must be specified");
                }

                if (obj is string value)
                {
                    if (value == PodValue)
                    {
                        return podSiteToPouSiteFact.PODSite.SiteUuid;
                    }

                    if (value == PouValue)
                    {
                        return podSiteToPouSiteFact.POUSite.SiteUuid;
                    }
                }

                throw new WaDEException($"POD/POU key must be either \"{PodValue}\" or \"{PouValue}\"");
            }
        }
    }
}