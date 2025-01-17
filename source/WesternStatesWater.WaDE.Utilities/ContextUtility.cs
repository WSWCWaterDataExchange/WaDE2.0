using Microsoft.AspNetCore.Http;
using WesternStatesWater.WaDE.Common.Context;

namespace WesternStatesWater.WaDE.Utilities;

public class ContextUtility(IHttpContextAccessor httpContextAccessor) : IContextUtility
{
    public ContextBase GetContext() => Build();

    public TContext GetRequiredContext<TContext>() where TContext : ContextBase
    {
        if (GetContext() is not TContext context)
        {
            throw new InvalidOperationException(
                $"Context is of type '{GetContext().GetType().Name}', not of the " +
                $"requested type '{typeof(TContext).Name}'."
            );
        }

        return context;
    }

    private ContextBase Build()
    {
        return new ApiContext
        {
            Scheme = httpContextAccessor.HttpContext.Request.Scheme,
            Host = httpContextAccessor.HttpContext.Request.Host.Value,
            PathBase = httpContextAccessor.HttpContext.Request.PathBase,
            Path = httpContextAccessor.HttpContext.Request.Path,
            QueryString = httpContextAccessor.HttpContext.Request.QueryString
        };
    }
}