using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Extensions;
using WesternStatesWater.WaDE.Accessors.Mapping;
using Method = WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Method;
using Site = WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Site;
using VariableSpecific = WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.VariableSpecific;

namespace WesternStatesWater.WaDE.Accessors.Handlers;

public class TimeSeriesSearchHandler(IConfiguration configuration)
    : IRequestHandler<TimeSeriesSearchRequest, TimeSeriesSearchResponse>
{
    public async Task<TimeSeriesSearchResponse> Handle(TimeSeriesSearchRequest request)
    {
        await using var db = new WaDEContext(configuration);

        var sites = await db.SiteVariableAmountsFact
            .AsNoTracking()
            .ApplySearchFilters(request)
            .OrderBy(s => s.SiteVariableAmountId)
            .ApplyLimit(request)
            // Using Select because Automapper ProjectTo created complex queries that were not performant.
            .Select(ts => new TimeSeriesSearchItem
            {
                Site = new Site
                {
                    SiteUuid = ts.Site.SiteUuid,
                    SiteNativeId = ts.Site.SiteNativeId,
                    SiteName = ts.Site.SiteName,
                    Location = ts.Site.Geometry ?? ts.Site.SitePoint,
                    CoordinateMethod = ts.Site.CoordinateMethodCv,
                    GnisCode = ts.Site.GniscodeCv,
                    EpsgCode = ts.Site.EpsgcodeCv,
                    NhdNetworkStatus = ts.Site.NhdnetworkStatusCv,
                    NhdProduct = ts.Site.NhdproductCv,
                    State = ts.Site.StateCv,
                    Huc8 = ts.Site.HUC8,
                    Huc12 = ts.Site.HUC12,
                    County = ts.Site.County,
                    PodOrPouSite = ts.Site.PODorPOUSite,
                    WaterSources = ts.Site.WaterSourceBridgeSitesFact.Select(ws => new WaterSourceSummary
                    {
                        WaterSourceName = ws.WaterSource.WaterSourceName,
                        WaterSourceNativeId = ws.WaterSource.WaterSourceNativeId,
                        WaterSourceTypeCv = ws.WaterSource.WaterSourceTypeCv,
                        WaterSourceUuid = ws.WaterSource.WaterSourceUuid,
                        WaterQualityIndicatorCv = ws.WaterSource.WaterSourceTypeCv
                    }).ToArray()
                },
                Organization = new Organization
                {
                    ContactEmail = ts.Organization.OrganizationContactEmail,
                    ContactName = ts.Organization.OrganizationName,
                    PhoneNumber = ts.Organization.OrganizationPhoneNumber,
                    Purview = ts.Organization.OrganizationPurview,
                    State = ts.Organization.State,
                    Uuid = ts.Organization.OrganizationUuid,
                    Website = ts.Organization.OrganizationWebsite
                },
                Method = new Method
                {
                    ApplicableResourceType = ts.Method.ApplicableResourceTypeCv,
                    Description = ts.Method.MethodDescription,
                    Name = ts.Method.MethodName,
                    NemiLink = ts.Method.MethodNemilink,
                    Type = ts.Method.MethodTypeCv,
                    Uuid = ts.Method.MethodUuid
                },
                VariableSpecific = new VariableSpecific
                {
                    AggregationInterval = ts.VariableSpecific.AggregationInterval,
                    AggregationIntervalUnit = ts.VariableSpecific.AggregationIntervalUnitCv,
                    AggregationStatistic = ts.VariableSpecific.AggregationStatisticCv,
                    AmountUnit = ts.VariableSpecific.AmountUnitCv,
                    MaximumAmountUnit = ts.VariableSpecific.MaximumAmountUnitCv,
                    VariableSpecificCv = ts.VariableSpecific.VariableSpecificCv,
                    VariableSpecificWaDEName = ts.VariableSpecific.VariableSpecificCvNavigation.WaDEName,
                    ReportYearStartMonth = ts.VariableSpecific.ReportYearStartMonth,
                    ReportYearType = ts.VariableSpecific.ReportYearTypeCv,
                    Uuid = ts.VariableSpecific.VariableSpecificUuid,
                    VariableCv = ts.VariableSpecific.VariableCv,
                    VariableWaDEName = ts.VariableSpecific.VariableCvNavigation.WaDEName
                },
                WaterSource = new WaterSourceSummary
                {
                    WaterSourceUuid = ts.WaterSource.WaterSourceUuid,
                    WaterSourceNativeId = ts.WaterSource.WaterSourceNativeId,
                    WaterSourceName = ts.WaterSource.WaterSourceName,
                    WaterSourceTypeCv = ts.WaterSource.WaterSourceTypeCv,
                    WaterQualityIndicatorCv = ts.WaterSource.WaterSourceTypeCv
                },
                SiteVariableAmountId = ts.SiteVariableAmountId.ToString(),
                TimeframeStart = ts.TimeframeStartNavigation.Date,
                TimeframeEnd = ts.TimeframeEndNavigation.Date,
                ReportYear = ts.ReportYearCv,
                Amount = ts.Amount,
                PopulationServed = ts.PopulationServed,
                PowerGeneratedGWh = ts.PowerGeneratedGwh,
                IrrigatedAcreage = ts.IrrigatedAcreage,
                IrrigationMethod = ts.IrrigationMethodCv,
                CropType = ts.CropTypeCv,
                CommunityWaterSupplySystem = ts.CommunityWaterSupplySystem,
                SdwisIdentifier = ts.SDWISIdentifierCv,
                AssociatedNativeAllocationIDs = ts.AssociatedNativeAllocationIds,
                PrimaryUseCategoryCv = ts.PrimaryUseCategoryCV,
                PrimaryUseCategoryWaDEName = ts.PrimaryBeneficialUse.WaDEName,
                PowerType = ts.PowerType
            })
            .ToListAsync();

        string lastUuid = null;
        // Only set lastUuid if more than one item was returned.
        // Requests looking up a specific record will only have count of 1 or 0.
        if (sites.Count > 1)
        {
            // Get the last UUID of the page (not the first one on the next page).
            lastUuid = sites.Count <= request.Limit ? null : sites[^2].SiteVariableAmountId;
        }

        return new TimeSeriesSearchResponse
        {
            LastUuid = lastUuid,
            Sites = sites.Take(request.Limit).ToList()
        };
    }
}