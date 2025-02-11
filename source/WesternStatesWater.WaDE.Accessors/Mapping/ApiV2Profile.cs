using System.Linq;
using AutoMapper;
using NetTopologySuite.Operation.Union;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;
using EF = WesternStatesWater.WaDE.Accessors.EntityFramework;
using Method = WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Method;
using Site = WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Site;
using VariableSpecific = WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.VariableSpecific;

namespace WesternStatesWater.WaDE.Accessors.Mapping;

public class ApiV2Profile : Profile
{
    public ApiV2Profile()
    {
        AllowNullDestinationValues = true;
        CreateMap<EF.WaterSourcesDim, WaterSourceSummary>();

        CreateMap<EF.SitesDim, SiteSearchItem>()
            .ForMember(a => a.SiteTypeWaDEName, b => b.MapFrom(c => c.SiteTypeCvNavigation.WaDEName))
            .ForMember(a => a.Location, b => b.MapFrom(c => c.Geometry != null ? c.Geometry : c.SitePoint))
            .ForMember(a => a.WaterSources,
                b => b.MapFrom(c =>
                    c.WaterSourceBridgeSitesFact.Select(bridge => bridge.WaterSource)));

        CreateMap<EF.RegulatoryOverlayDim, OverlaySearchItem>()
            .ForMember(a => a.OverlayUuid, b => b.MapFrom(c => c.RegulatoryOverlayUuid))
            .ForMember(a => a.OverlayNativeId, b => b.MapFrom(c => c.RegulatoryOverlayNativeId))
            .ForMember(a => a.RegulatoryName, b => b.MapFrom(c => c.RegulatoryName))
            .ForMember(a => a.RegulatoryDescription, b => b.MapFrom(c => c.RegulatoryDescription))
            .ForMember(a => a.RegulatoryStatus, b => b.MapFrom(c => c.RegulatoryStatusCv))
            .ForMember(a => a.RegulatoryStatuteLink, b => b.MapFrom(c => c.RegulatoryStatuteLink))
            .ForMember(a => a.StatutoryEffectiveDate, b => b.MapFrom(c => c.StatutoryEffectiveDate))
            .ForMember(a => a.StatutoryEndDate, b => b.MapFrom(c => c.StatutoryEndDate))
            .ForMember(a => a.OverlayType, b => b.MapFrom(c => c.RegulatoryOverlayType.WaDEName))
            .ForMember(a => a.WaterSource, b => b.MapFrom(c => c.WaterSourceType.WaDEName))
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
                    UnaryUnionOp.Union(c.RegulatoryReportingUnitsFact.Where(fact => fact.ReportingUnit.Geometry != null)
                        .Select(fact => fact.ReportingUnit.Geometry))))
            .ForMember(a => a.States,
                b => b.MapFrom(
                    c => c.RegulatoryReportingUnitsFact.Select(fact => fact.ReportingUnit.StateCv).Distinct()));

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
            .ForMember(a => a.States,
                b => b.MapFrom(c =>
                    c.AllocationBridgeSitesFact.Where(bridge => bridge.Site.StateCv != null)
                        .Select(bridge => bridge.Site.StateCv)))
            .ForMember(a => a.BeneficialUses,
                b => b.MapFrom(c =>
                    c.AllocationBridgeBeneficialUsesFact.Select(d => d.BeneficialUse.WaDEName).Distinct()))
            .ForMember(a => a.DataPublicationDate, b => b.MapFrom(c => c.DataPublicationDate.Date))
            .ForMember(a => a.SitesUUIDs,
                b => b.MapFrom(c => c.AllocationBridgeSitesFact.Select(d => d.Site.SiteUuid)))
            .ForMember(a => a.WaterSources,
                b => b.MapFrom(c => c.AllocationBridgeSitesFact
                    .SelectMany(bridge => bridge.Site.WaterSourceBridgeSitesFact.Select(ws => ws.WaterSource))));

        CreateMap<EF.SitesDim, Site>()
            .ForMember(a => a.SiteUuid, b => b.MapFrom(c => c.SiteUuid))
            .ForMember(a => a.SiteNativeId, b => b.MapFrom(c => c.SiteNativeId))
            .ForMember(a => a.SiteName, b => b.MapFrom(c => c.SiteName))
            .ForMember(a => a.UsgsSiteId, b => b.MapFrom(c => c.UsgssiteId))
            .ForMember(a => a.SiteTypeWaDEName, b => b.MapFrom(c => c.SiteTypeCvNavigation.WaDEName))
            .ForMember(a => a.Location, b => b.MapFrom(c => c.Geometry != null ? c.Geometry : c.SitePoint))
            .ForMember(a => a.CoordinateMethodCv, b => b.MapFrom(c => c.CoordinateMethodCv))
            .ForMember(a => a.CoordinateAccuracy, b => b.MapFrom(c => c.CoordinateAccuracy))
            .ForMember(a => a.GnisCodeCv, b => b.MapFrom(c => c.GniscodeCv))
            .ForMember(a => a.EpsgCodeCv, b => b.MapFrom(c => c.EpsgcodeCv))
            .ForMember(a => a.NhdNetworkStatusCv, b => b.MapFrom(c => c.NhdnetworkStatusCv))
            .ForMember(a => a.NhdProductCv, b => b.MapFrom(c => c.NhdproductCv))
            .ForMember(a => a.StateCv, b => b.MapFrom(c => c.StateCv))
            .ForMember(a => a.Huc8, b => b.MapFrom(c => c.HUC8))
            .ForMember(a => a.Huc12, b => b.MapFrom(c => c.HUC12))
            .ForMember(a => a.County, b => b.MapFrom(c => c.County))
            .ForMember(a => a.PodOrPouSite, b => b.MapFrom(c => c.PODorPOUSite))
            .ForMember(a => a.WaterSources,
                b => b.MapFrom(c =>
                    c.WaterSourceBridgeSitesFact.Select(bridge => bridge.WaterSource)));
        
        CreateMap<EF.OrganizationsDim, Organization>()
            .ForMember(a => a.Uuid, b => b.MapFrom(c => c.OrganizationUuid))
            .ForMember(a => a.Purview, b => b.MapFrom(c => c.OrganizationPurview))
            .ForMember(a => a.Website, b => b.MapFrom(c => c.OrganizationWebsite))
            .ForMember(a => a.PhoneNumber, b => b.MapFrom(c => c.OrganizationPhoneNumber))
            .ForMember(a => a.ContactName, b => b.MapFrom(c => c.OrganizationContactName))
            .ForMember(a => a.ContactEmail, b => b.MapFrom(c => c.OrganizationContactEmail))
            .ForMember(a => a.State, b => b.MapFrom(c => c.State));

        CreateMap<EF.VariablesDim, VariableSpecific>()
            .ForMember(a => a.Uuid, b => b.MapFrom(c => c.VariableSpecificUuid))
            .ForMember(a => a.VariableSpecificCv, b => b.MapFrom(c => c.VariableSpecificCv))
            .ForMember(a => a.VariableSpecificWaDEName, b => b.MapFrom(c => c.VariableSpecificCvNavigation.WaDEName))
            .ForMember(a => a.VariableCv, b => b.MapFrom(c => c.VariableCv))
            .ForMember(a => a.VariableWaDEName, b => b.MapFrom(c => c.VariableCvNavigation.WaDEName))
            .ForMember(a => a.AggregationStatistic, b => b.MapFrom(c => c.AggregationStatisticCv))
            .ForMember(a => a.AggregationInterval, b => b.MapFrom(c => c.AggregationInterval))
            .ForMember(a => a.AggregationIntervalUnit, b => b.MapFrom(c => c.AggregationIntervalUnitCv))
            .ForMember(a => a.ReportYearStartMonth, b => b.MapFrom(c => c.ReportYearStartMonth))
            .ForMember(a => a.ReportYearType, b => b.MapFrom(c => c.ReportYearTypeCv))
            .ForMember(a => a.AmountUnit, b => b.MapFrom(c => c.AmountUnitCv))
            .ForMember(a => a.MaximumAmountUnit, b => b.MapFrom(c => c.MaximumAmountUnitCv));

        CreateMap<EF.MethodsDim, Method>()
            .ForMember(a => a.Uuid, b => b.MapFrom(c => c.MethodUuid))
            .ForMember(a => a.Name, b => b.MapFrom(c => c.MethodName))
            .ForMember(a => a.Description, b => b.MapFrom(c => c.MethodDescription))
            .ForMember(a => a.Type, b => b.MapFrom(c => c.MethodTypeCv))
            .ForMember(a => a.NemiLink, b => b.MapFrom(c => c.MethodNemilink))
            .ForMember(a => a.ApplicableResourceType, b => b.MapFrom(c => c.ApplicableResourceTypeCv));
    }
}