using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WesternStatesWater.WaDE.Accessors.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyFilter<T>(IQueryable<T> query, List<string> values, Expression<Func<T, string>> column)
    {
        if (values != null && values.Any())
        {
            query = query.Where(x => values.Contains(column.Compile().Invoke(x)));
        }
        return query;
    }
}