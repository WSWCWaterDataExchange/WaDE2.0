using System.Linq;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<SitesDim> ApplySearchFilters(this IQueryable<SitesDim> query, SiteSearchRequest filters)
    {
        if (filters.GeometrySearch?.Geometry != null && !filters.GeometrySearch.Geometry.IsEmpty)
        {
            query = filters.GeometrySearch.SpatialRelationType switch
            {
                SpatialRelationType.Intersects => query.Where(s =>
                    (s.Geometry.IsValid && s.Geometry.Intersects(filters.GeometrySearch.Geometry)) ||
                    (s.SitePoint.IsValid && s.SitePoint.Intersects(filters.GeometrySearch.Geometry))),
                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(filters.LastSiteUuid))
        {
            query = query.Where(s => s.SiteUuid.CompareTo(filters.LastSiteUuid) > 0);
        }
        
        if (filters.SiteTypes != null && filters.SiteTypes.Count != 0)
        {
            query = query.Where(s => filters.SiteTypes.Contains(s.SiteTypeCvNavigation.WaDEName));
        }

        if (filters.States != null && filters.States.Count != 0)
        {
            query = query.Where(s => filters.States.Contains(s.StateCv));
        }
        
        if (filters.WaterSourcesTypes != null && filters.WaterSourcesTypes.Count != 0)
        {
            query = query.Where(s => s.WaterSourceBridgeSitesFact.Any(fact => filters.WaterSourcesTypes.Contains(fact.WaterSource.WaterSourceTypeCvNavigation.WaDEName)));
        }

        if (filters.SiteUuids != null && filters.SiteUuids.Count != 0)
        {
            query = query.Where(s => filters.SiteUuids.Contains(s.SiteUuid));
        }
        
        if (filters.OverlayUuids != null && filters.OverlayUuids.Count != 0)
        {
            query = query.Where(s => s.RegulatoryOverlayBridgeSitesFact.Any(fact => filters.OverlayUuids.Contains(fact.RegulatoryOverlay.RegulatoryOverlayUuid)));
        }
        
        if (filters.AllocationUuids != null && filters.AllocationUuids.Count != 0)
        {
            query = query.Where(s => s.AllocationBridgeSitesFact.Any(fact => filters.AllocationUuids.Contains(fact.AllocationAmount.AllocationUUID)));
        }

        return query;
    }

    public static IQueryable<AllocationAmountsFact> ApplySearchFilters(this IQueryable<AllocationAmountsFact> query,
        AllocationSearchRequest filters)
    {
        if (filters.GeometrySearch?.Geometry != null && !filters.GeometrySearch.Geometry.IsEmpty)
        {
            query = filters.GeometrySearch.SpatialRelationType switch
            {
                SpatialRelationType.Intersects => query.Where(x => x.AllocationBridgeSitesFact.Any(
                    bridge => (bridge.Site.Geometry.IsValid && bridge.Site.Geometry.Intersects(filters.GeometrySearch.Geometry)) ||
                              (bridge.Site.SitePoint.IsValid && bridge.Site.SitePoint.Intersects(filters.GeometrySearch.Geometry)))),
                _ => query
            };

        }
        
        if (filters.AllocationUuid != null && filters.AllocationUuid.Any())
        {
            query = query.Where(x => filters.AllocationUuid.Contains(x.AllocationUUID));
        }

        if (filters.SiteUuid != null && filters.SiteUuid.Count != 0)
        {
            query = query.Where(x => x.AllocationBridgeSitesFact.Any(
                bridge => filters.SiteUuid.Contains(bridge.Site.SiteUuid)));
        }

        if (filters.States != null && filters.States.Count != 0)
        {
            query = query.Where(x =>
                x.AllocationBridgeSitesFact.Any(bridge => filters.States.Contains(bridge.Site.StateCv)));
        }

        if (filters.WaterSourceTypes != null && filters.WaterSourceTypes.Count != 0)
        {
            query = query.Where(x => x.AllocationBridgeSitesFact.Any(
                bridge => bridge.Site.WaterSourceBridgeSitesFact.Any(
                    ws => filters.WaterSourceTypes.Contains(ws.WaterSource.WaterSourceTypeCvNavigation.WaDEName))));
        }
        
        if (filters.BeneficialUses != null && filters.BeneficialUses.Count != 0)
        {
            query = query.Where(x => x.AllocationBridgeBeneficialUsesFact.Any(
                bridge => filters.BeneficialUses.Contains(bridge.BeneficialUse.WaDEName)));
        }

        if (!string.IsNullOrWhiteSpace(filters.LastKey))
        {
            query = query.Where(x => x.AllocationUUID.CompareTo(filters.LastKey) > 0);
        }

        return query;
    }

    public static IQueryable<SiteVariableAmountsFact> ApplySearchFilters(this IQueryable<SiteVariableAmountsFact> query,
        TimeSeriesSearchRequest filters)
    {
        if (filters.SiteVariableAmountId.HasValue)
        {
            query = query.Where(x => x.SiteVariableAmountId == filters.SiteVariableAmountId);
        }
        
        if (filters.GeometrySearch?.Geometry != null && !filters.GeometrySearch.Geometry.IsEmpty)
        {
            query = filters.GeometrySearch.SpatialRelationType switch
            {
                SpatialRelationType.Intersects => query.Where(s =>
                    (s.Site.Geometry.IsValid && s.Site.Geometry.Intersects(filters.GeometrySearch.Geometry)) ||
                    (s.Site.SitePoint.IsValid && s.Site.SitePoint.Intersects(filters.GeometrySearch.Geometry))),
                _ => query
            };
        }
        
        if (filters.DateRange?.StartDate != null)
        {
            query = query.Where(x =>
                    // Check if filtered start date is within the time series date range
                    (filters.DateRange.StartDate >= x.TimeframeStartNavigation.Date &&
                     filters.DateRange.StartDate <= x.TimeframeEndNavigation.Date) ||
                    // Else check if filtered start date is before the time series start date
                    filters.DateRange.StartDate <= x.TimeframeStartNavigation.Date);
        }

        if (filters.DateRange?.EndDate != null)
        {
            query = query.Where(x =>
                    // Check if filtered end date is within the time series date range
                    (filters.DateRange.EndDate >= x.TimeframeStartNavigation.Date &&
                     filters.DateRange.EndDate <= x.TimeframeEndNavigation.Date) ||
                    // Else check if filtered end date is after the time series end date
                    x.TimeframeEndNavigation.Date <= filters.DateRange.EndDate);
        }

        if (filters.SiteUuids != null && filters.SiteUuids.Count != 0)
        {
            query = query.Where(x => filters.SiteUuids.Contains(x.Site.SiteUuid));
        }

        if (filters.States != null && filters.States.Count != 0)
        {
            query = query.Where(x => filters.States.Contains(x.Site.StateCv));
        }

        if (filters.VariableTypes != null && filters.VariableTypes.Count != 0)
        {
            query = query.Where(x => filters.VariableTypes.Contains(x.VariableSpecific.VariableCv));
        }

        if (filters.WaterSourceTypes != null && filters.WaterSourceTypes.Count != 0)
        {
            query = query.Where(x => filters.WaterSourceTypes.Contains(x.WaterSource.WaterSourceTypeCv));
        }

        if (!string.IsNullOrWhiteSpace(filters.LastKey))
        {
            query = query.Where(x => x.SiteVariableAmountId > long.Parse(filters.LastKey));
        }

        return query;
    }

    public static IQueryable<RegulatoryOverlayDim> ApplySearchFilters(this IQueryable<RegulatoryOverlayDim> query,
        OverlaySearchRequest filters)
    {
        if (filters.OverlayUuids != null && filters.OverlayUuids.Count != 0)
        {
            query = query.Where(o => filters.OverlayUuids.Contains(o.RegulatoryOverlayUuid));
        }

        if (filters.SiteUuids != null && filters.SiteUuids.Count != 0)
        {
            query = query.Where(o =>
                o.RegulatoryOverlayBridgeSitesFact.Any(sf =>
                    filters.SiteUuids.Contains(sf.Site.SiteUuid)));
        }

        if (filters.GeometrySearch?.Geometry != null && !filters.GeometrySearch.Geometry.IsEmpty)
        {
            query = filters.GeometrySearch.SpatialRelationType switch
            {
                SpatialRelationType.Intersects => query.Where(o =>
                    o.RegulatoryReportingUnitsFact.Any(fact =>
                        fact.ReportingUnit.Geometry.Intersects(filters.GeometrySearch.Geometry))),
                _ => query
            };
        }
        
        if (filters.States != null && filters.States.Count != 0)
        {
            query = query.Where(o => o.RegulatoryReportingUnitsFact.Any(fact => filters.States.Contains(fact.ReportingUnit.StateCv)));
        }
        
        if (filters.OverlayTypes != null && filters.OverlayTypes.Count != 0)
        {
            query = query.Where(o => filters.OverlayTypes.Contains(o.RegulatoryOverlayType.WaDEName));
        }
        
        if (filters.WaterSourceTypes != null && filters.WaterSourceTypes.Count != 0)
        {
            query = query.Where(o => filters.WaterSourceTypes.Contains(o.WaterSourceType.WaDEName));
        }

        if (!string.IsNullOrWhiteSpace(filters.LastKey))
        {
            query = query.Where(o => o.RegulatoryOverlayUuid.CompareTo(filters.LastKey) > 0);
        }

        return query;
    }
    
    /// <summary>
    /// Sets the Take limit plus one on the query. This is used to determine if there are more results.
    /// </summary>
    /// <param name="query">Queryable DbSet.</param>
    /// <param name="filters">SearchRequestBase</param>
    /// <typeparam name="T">DbSet</typeparam>
    /// <returns></returns>
    public static IQueryable<T> ApplyLimit<T>(this IQueryable<T> query, SearchRequestBase filters)
    {
        // Adding one to limit to determine if there are more results
        query = query.Take(filters.Limit + 1);
        return query;
    }
}