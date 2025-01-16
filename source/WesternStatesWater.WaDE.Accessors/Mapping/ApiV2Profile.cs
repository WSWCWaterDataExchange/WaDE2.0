using System.Linq;
using AutoMapper;
using NetTopologySuite.Operation.Union;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;
using EF = WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Mapping;

public class ApiV2Profile : Profile
{
    public ApiV2Profile()
    {
        AllowNullDestinationValues = true;
        CreateMap<EF.WaterSourcesDim, WaterSourceSummary>()
            .ForMember(dest => dest.WaterSourceUuId, opt => opt.MapFrom(src => src.WaterSourceUuid))
            .ForMember(dest => dest.NativeId, opt => opt.MapFrom(src => src.WaterSourceNativeId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.WaterSourceName))
            .ForMember(dest => dest.SourceType, opt => opt.MapFrom(src => src.WaterSourceTypeCvNavigation.WaDEName))
            .ForMember(dest => dest.WaterType, opt => opt.MapFrom(src => src.WaterQualityIndicatorCv));
        
        CreateMap<EF.SitesDim, SiteSearchItem>()
            .ForMember(a => a.SiteUuid, b => b.MapFrom(c => c.SiteUuid))
            .ForMember(a => a.SiteNativeId, b => b.MapFrom(c => c.SiteNativeId))
            .ForMember(a => a.SiteName, b => b.MapFrom(c => c.SiteName))
            .ForMember(a => a.UsgsSiteId, b => b.MapFrom(c => c.UsgssiteId))
            .ForMember(a => a.SiteType, b => b.MapFrom(c => c.SiteTypeCvNavigation.WaDEName))
            .ForMember(a => a.Location, b => b.MapFrom(c => c.Geometry != null ? c.Geometry : c.SitePoint))
            .ForMember(a => a.CoordinateMethod, b => b.MapFrom(c => c.CoordinateMethodCv))
            .ForMember(a => a.CoordinateAccuracy, b => b.MapFrom(c => c.CoordinateAccuracy))
            .ForMember(a => a.GnisCode, b => b.MapFrom(c => c.GniscodeCv))
            .ForMember(a => a.EpsgCode, b => b.MapFrom(c => c.EpsgcodeCv))
            .ForMember(a => a.NhdNetworkStatus, b => b.MapFrom(c => c.NhdnetworkStatusCv))
            .ForMember(a => a.NhdProduct, b => b.MapFrom(c => c.NhdproductCv))
            .ForMember(a => a.State, b => b.MapFrom(c => c.StateCv))
            .ForMember(a => a.Huc8, b => b.MapFrom(c => c.HUC8))
            .ForMember(a => a.Huc12, b => b.MapFrom(c => c.HUC12))
            .ForMember(a => a.County, b => b.MapFrom(c => c.County))
            .ForMember(a => a.RightUuids,
                b => b.MapFrom(c => c.AllocationBridgeSitesFact.Select(d => d.AllocationAmount.AllocationUUID)))
            .ForMember(a => a.IsTimeSeries, b => b.MapFrom(c => c.SiteVariableAmountsFact.Any()))
            .ForMember(a => a.PodOrPouSite, b => b.MapFrom(c => c.PODorPOUSite))
            .ForMember(a => a.WaterSources,
                b => b.MapFrom(c =>
                    c.WaterSourceBridgeSitesFact.Select(bridge => bridge.WaterSource)))
            .ForMember(a => a.Overlays,
                b => b.MapFrom(c =>
                    c.RegulatoryOverlayBridgeSitesFact.Select(bridge =>
                        bridge.RegulatoryOverlay.RegulatoryOverlayUuid)));

        CreateMap<EF.RegulatoryOverlayDim, OverlaySearchItem>()
            .ForMember(a => a.OverlayUuid, b => b.MapFrom(c => c.RegulatoryOverlayUuid))
            .ForMember(a => a.OverlayNativeId, b => b.MapFrom(c => c.RegulatoryOverlayNativeId))
            .ForMember(a => a.RegulatoryName, b => b.MapFrom(c => c.RegulatoryName))
            .ForMember(a => a.RegulatoryDescription, b => b.MapFrom(c => c.RegulatoryDescription))
            .ForMember(a => a.RegulatoryStatus, b => b.MapFrom(c => c.RegulatoryStatusCv))
            .ForMember(a => a.RegulatoryStatuteLink, b => b.MapFrom(c => c.RegulatoryStatuteLink))
            .ForMember(a => a.StatutoryEffectiveDate, b => b.MapFrom(c => c.StatutoryEffectiveDate))
            .ForMember(a => a.StatutoryEndDate, b => b.MapFrom(c => c.StatutoryEndDate))
            .ForMember(a => a.OverlayType, b => b.MapFrom(c => c.RegulatoryOverlayTypeCV))
            .ForMember(a => a.WaterSource, b => b.MapFrom(c => c.WaterSourceTypeCV))
            .ForMember(a => a.AreaNames,
                b => b.MapFrom(c =>
                    c.RegulatoryReportingUnitsFact.Select(fact => fact.ReportingUnit.ReportingUnitName)))
            .ForMember(a => a.AreaNativeIds,
                b => b.MapFrom(c =>
                    c.RegulatoryReportingUnitsFact.Select(fact => fact.ReportingUnit.ReportingUnitNativeId)))
            .ForMember(a => a.SiteUuids,
                b => b.MapFrom(c => c.RegulatoryOverlayBridgeSitesFact.Select(fact => fact.Site.SiteUuid)))
            .ForMember(a => a.Areas,
                b => b.MapFrom(c =>
                    UnaryUnionOp.Union(c.RegulatoryReportingUnitsFact.Select(fact => fact.ReportingUnit.Geometry))));

        CreateMap<EF.AllocationAmountsFact, AllocationSearchItem>()
            .ForMember(a => a.AllocationApplicationDate,
                b => b.MapFrom(c => c.AllocationApplicationDateNavigation.Date))
            .ForMember(a => a.AllocationPriorityDate, b => b.MapFrom(c => c.AllocationPriorityDateNavigation.Date))
            .ForMember(a => a.AllocationLegalStatusCodeCV, b => b.MapFrom(c => c.AllocationLegalStatusCv))
            .ForMember(a => a.AllocationExpirationDate,
                b => b.MapFrom(c => c.AllocationExpirationDateNavigation.Date))
            .ForMember(a => a.AllocationAcreage, b => b.MapFrom(c => c.IrrigatedAcreage))
            .ForMember(a => a.TimeframeStart, b => b.MapFrom(c => c.AllocationTimeframeStart))
            .ForMember(a => a.TimeframeEnd, b => b.MapFrom(c => c.AllocationTimeframeEnd))
            .ForMember(a => a.AllocationSDWISIdentifier, b => b.MapFrom(c => c.SdwisidentifierCV))
            .ForMember(a => a.MethodUUID, b => b.MapFrom(c => c.Method.MethodUuid))
            .ForMember(a => a.Method, b => b.MapFrom(c => c.Method.MethodName))
            .ForMember(a => a.Organization, b => b.MapFrom(c => c.Organization.OrganizationName))
            .ForMember(a => a.VariableSpecificTypeCV, b => b.MapFrom(c => c.VariableSpecific.VariableSpecificCv))
            .ForMember(a => a.States, b => b.MapFrom(c => c.AllocationBridgeSitesFact.Where(bridge => bridge.Site.StateCv != null).Select(bridge => bridge.Site.StateCv)))
            .ForMember(a => a.BeneficialUses,
                b => b.MapFrom(c => c.AllocationBridgeBeneficialUsesFact.Select(d => d.BeneficialUseCV)))
            .ForMember(a => a.DataPublicationDate, b => b.MapFrom(c => c.DataPublicationDate.Date))
            .ForMember(a => a.SitesUUIDs,
                b => b.MapFrom(c => c.AllocationBridgeSitesFact.Select(d => d.Site.SiteUuid)));

        CreateMap<EF.SiteVariableAmountsFact, TimeSeriesSearchItem>()
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
    }
}