using System.Linq;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Extensions;

public static class SiteQueryableExtensions
{
    public static IQueryable<SitesDim> ApplyFilters(this IQueryable<SitesDim> query, SiteSearchRequest filters)
    {
        return query
            .ApplyIntersectFilters(filters)
            .ApplyPaging(filters);
    }

    public static IQueryable<SitesDim> ApplyIntersectFilters(this IQueryable<SitesDim> query, SiteSearchRequest filters)
    {
        if (filters.FilterBoundary != null && !filters.FilterBoundary.IsEmpty)
        {
            query = query.Where(s => s.Geometry.Intersects(filters.FilterBoundary));
        }

        return query;
    }

    public static IQueryable<SitesDim> ApplyPaging(this IQueryable<SitesDim> query, SiteSearchRequest filters)
    {
        if (!string.IsNullOrWhiteSpace(filters.LastSiteUuid))
        {
            query = query.Where(s => s.SiteUuid.CompareTo(filters.LastSiteUuid) > 0);
        }

        query = query.Take(filters.Limit);
        return query;
    }
}