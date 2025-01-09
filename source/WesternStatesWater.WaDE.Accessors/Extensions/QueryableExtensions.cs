using System.Linq;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<SitesDim> ApplySearchFilters(this IQueryable<SitesDim> query, SiteSearchRequest filters)
    {
        if (filters.FilterBoundary != null && !filters.FilterBoundary.IsEmpty)
        {
            query = query.Where(s => 
                                     (s.Geometry.IsValid && s.Geometry.Intersects(filters.FilterBoundary)) ||
                                     (s.SitePoint.IsValid && s.SitePoint.Intersects(filters.FilterBoundary)));
        }

        if (!string.IsNullOrWhiteSpace(filters.LastSiteUuid))
        {
            query = query.Where(s => s.SiteUuid.CompareTo(filters.LastSiteUuid) > 0);
        }

        return query;
    }

    public static IQueryable<AllocationAmountsFact> ApplySearchFilters(this IQueryable<AllocationAmountsFact> query,
        AllocationSearchRequest filters)
    {
        if (filters.AllocationUuid != null && filters.AllocationUuid.Any())
        {
            query = query.Where(x => filters.AllocationUuid.Contains(x.AllocationUUID));
        }

        if (filters.SiteUuid != null && filters.SiteUuid.Count != 0)
        {
            query = query.Where(x => x.AllocationBridgeSitesFact.Any(
                bridge => filters.SiteUuid.Contains(bridge.Site.SiteUuid)));
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
        if (filters.StartDate.HasValue)
        {
            query = query.Where(x =>
                (filters.StartDate >= x.TimeframeStartNavigation.Date &&
                 filters.StartDate <= x.TimeframeEndNavigation.Date) ||
                filters.StartDate <= x.TimeframeStartNavigation.Date);
        }

        if (filters.EndDate.HasValue)
        {
            query = query.Where(x =>
                (filters.EndDate >= x.TimeframeStartNavigation.Date &&
                 filters.EndDate <= x.TimeframeEndNavigation.Date) ||
                x.TimeframeEndNavigation.Date <= filters.EndDate);
        }

        if (filters.SiteUuids != null && filters.SiteUuids.Count != 0)
        {
            query = query.Where(x => filters.SiteUuids.Contains(x.Site.SiteUuid));
        }

        if (filters.LastKey.HasValue)
        {
            query = query.Where(x => x.SiteVariableAmountId > filters.LastKey);
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

        if (filters.FilterBoundary != null && !filters.FilterBoundary.IsEmpty)
        {
            query = query.Where(o =>
                o.RegulatoryReportingUnitsFact.Any(fact =>
                    fact.ReportingUnit.Geometry.Intersects(filters.FilterBoundary)));
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