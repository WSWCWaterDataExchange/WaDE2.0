using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Extensions;
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
        db.Database.SetCommandTimeout(300);
        
        // Performing a spatial search from SiteVariableAmountsFact to SitesDim causes queries to be very slow when the Geometry does not match any records.
        // To improve performance, we first query SitesDim to get the SiteIds that match the spatial search.
        List<long> siteIds = null;
        if (request.GeometrySearch?.Geometry != null && !request.GeometrySearch.Geometry.IsEmpty)
        {
            siteIds = await db.SiteVariableAmountsFact
                .AsNoTracking()
                .Where(s =>
                    s.Site.Geometry.Intersects(request.GeometrySearch.Geometry) ||
                    s.Site.SitePoint.Intersects(request.GeometrySearch.Geometry))
                .ApplyLimit(request) // This must happen before OrderBy otherwise the query gets very slow and causes timeouts.
                .OrderBy(s => s.SiteVariableAmountId)
                .Select(x => x.SiteId)
                .ToListAsync();

            // If no siteIds are found, return an empty response, no need to query the rest of the data.
            if (siteIds.Count == 0)
            {
                return new TimeSeriesSearchResponse
                {
                    Sites = new List<TimeSeriesSearchItem>()
                };
            }
        }

        var sites = await db.SiteVariableAmountsFact
            .AsNoTracking()
            .ApplySearchFilters(request, siteIds)
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
                    Location = ts.Site.Geometry == null ? ts.Site.SitePoint : ts.Site.Geometry,
                    CoordinateMethodCv = ts.Site.CoordinateMethodCv,
                    GnisCodeCv = ts.Site.GniscodeCv,
                    EpsgCodeCv = ts.Site.EpsgcodeCv,
                    NhdNetworkStatusCv = ts.Site.NhdnetworkStatusCv,
                    NhdProductCv = ts.Site.NhdproductCv,
                    StateCv = ts.Site.StateCv,
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
                    OrganizationContactEmail = ts.Organization.OrganizationContactEmail,
                    OrganizationContactName = ts.Organization.OrganizationName,
                    OrganizationPhoneNumber = ts.Organization.OrganizationPhoneNumber,
                    OrganizationPurview = ts.Organization.OrganizationPurview,
                    State = ts.Organization.State,
                    OrganizationUuid = ts.Organization.OrganizationUuid,
                    OrganizationWebsite = ts.Organization.OrganizationWebsite
                },
                Method = new Method
                {
                    ApplicableResourceTypeCv = ts.Method.ApplicableResourceTypeCv,
                    MethodDescription = ts.Method.MethodDescription,
                    MethodName = ts.Method.MethodName,
                    MethodNemiLink = ts.Method.MethodNemilink,
                    MethodTypeCv = ts.Method.MethodTypeCv,
                    MethodUuid = ts.Method.MethodUuid
                },
                VariableSpecific = new VariableSpecific
                {
                    AggregationInterval = ts.VariableSpecific.AggregationInterval,
                    AggregationIntervalUnitCv = ts.VariableSpecific.AggregationIntervalUnitCv,
                    AggregationStatisticCv = ts.VariableSpecific.AggregationStatisticCv,
                    AmountUnitCv = ts.VariableSpecific.AmountUnitCv,
                    MaximumAmountUnitCv = ts.VariableSpecific.MaximumAmountUnitCv,
                    VariableSpecificCv = ts.VariableSpecific.VariableSpecificCv,
                    VariableSpecificWaDEName = ts.VariableSpecific.VariableSpecificCvNavigation.WaDEName,
                    ReportYearStartMonth = ts.VariableSpecific.ReportYearStartMonth,
                    ReportYearTypeCv = ts.VariableSpecific.ReportYearTypeCv,
                    VariableSpecificUuid = ts.VariableSpecific.VariableSpecificUuid,
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