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
            .ForMember(a => a.ReportingAreas,
                b => b.MapFrom(c =>
                    c.RegulatoryReportingUnitsFact.Select(fact => fact.ReportingUnit)))
            .ForMember(a => a.Areas,
                b => b.MapFrom(c =>
                    UnaryUnionOp.Union(c.RegulatoryReportingUnitsFact.Where(fact => fact.ReportingUnit.Geometry != null)
                        .Select(fact => fact.ReportingUnit.Geometry))));

        CreateMap<EF.AllocationAmountsFact, AllocationSearchItem>()
            .ForMember(a => a.AllocationApplicationDate,
                b => b.MapFrom(c => c.AllocationApplicationDateNavigation.Date))
            .ForMember(a => a.AllocationPriorityDate, b => b.MapFrom(c => c.AllocationPriorityDateNavigation.Date))
            .ForMember(a => a.AllocationExpirationDate,
                b => b.MapFrom(c => c.AllocationExpirationDateNavigation.Date))
            .ForMember(a => a.Method, b => b.MapFrom(c => c.Method))
            .ForMember(a => a.Organization, b => b.MapFrom(c => c.Organization))
            .ForMember(a => a.VariableSpecific, b => b.MapFrom(c => c.VariableSpecific))
            .ForMember(a => a.BeneficialUses,
                b => b.MapFrom(c =>
                    c.AllocationBridgeBeneficialUsesFact.Select(d => d.BeneficialUse.WaDEName).Distinct()))
            .ForMember(a => a.DataPublicationDate, b => b.MapFrom(c => c.DataPublicationDate.Date));

        CreateMap<EF.SitesDim, Site>()
            .ForMember(a => a.SiteTypeWaDEName, b => b.MapFrom(c => c.SiteTypeCvNavigation.WaDEName))
            .ForMember(a => a.Location, b => b.MapFrom(c => c.Geometry != null ? c.Geometry : c.SitePoint))
            .ForMember(a => a.WaterSources,
                b => b.MapFrom(c =>
                    c.WaterSourceBridgeSitesFact.Select(bridge => bridge.WaterSource)));
        
        CreateMap<EF.OrganizationsDim, Organization>()
            .ForMember(a => a.OrganizationUuid, b => b.MapFrom(c => c.OrganizationUuid))
            .ForMember(a => a.OrganizationPurview, b => b.MapFrom(c => c.OrganizationPurview))
            .ForMember(a => a.OrganizationWebsite, b => b.MapFrom(c => c.OrganizationWebsite))
            .ForMember(a => a.OrganizationPhoneNumber, b => b.MapFrom(c => c.OrganizationPhoneNumber))
            .ForMember(a => a.OrganizationContactName, b => b.MapFrom(c => c.OrganizationContactName))
            .ForMember(a => a.OrganizationContactEmail, b => b.MapFrom(c => c.OrganizationContactEmail))
            .ForMember(a => a.State, b => b.MapFrom(c => c.State));

        CreateMap<EF.VariablesDim, VariableSpecific>()
            .ForMember(a => a.VariableSpecificUuid, b => b.MapFrom(c => c.VariableSpecificUuid))
            .ForMember(a => a.VariableSpecificWaDEName, b => b.MapFrom(c => c.VariableSpecificCvNavigation.WaDEName))
            .ForMember(a => a.VariableWaDEName, b => b.MapFrom(c => c.VariableCvNavigation.WaDEName));

        CreateMap<EF.MethodsDim, Method>()
            .ForMember(a => a.MethodUuid, b => b.MapFrom(c => c.MethodUuid))
            .ForMember(a => a.MethodName, b => b.MapFrom(c => c.MethodName))
            .ForMember(a => a.MethodDescription, b => b.MapFrom(c => c.MethodDescription))
            .ForMember(a => a.MethodTypeCv, b => b.MapFrom(c => c.MethodTypeCv))
            .ForMember(a => a.MethodNemiLink, b => b.MapFrom(c => c.MethodNemilink))
            .ForMember(a => a.ApplicableResourceTypeCv, b => b.MapFrom(c => c.ApplicableResourceTypeCv));

        CreateMap<EF.ReportingUnitsDim, ReportingArea>()
            .ForMember(a => a.State, b => b.MapFrom(c => c.StateCv));
    }
}