using Microsoft.AspNetCore.Http;
using WesternStatesWater.WaDE.Common.Contexts;

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
        Uri requestUri;
        
#if DEBUG
        var scheme = httpContextAccessor.HttpContext.Request.Scheme;
        var host = httpContextAccessor.HttpContext.Request.Host.Value;
        var path = httpContextAccessor.HttpContext.Request.Path.Value;
        var query = httpContextAccessor.HttpContext.Request.QueryString.Value;
        requestUri = new Uri($"{scheme}://{host}{path}{query}");
#else
        if (httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-WaDE-OriginalUrl", out var originalUrl))
        {
            var builder = new UriBuilder(originalUrl.First()!)
            {
                Port = -1 // Remove port because header passes along port :443
            };
            requestUri = builder.Uri;
        }
        else
        {
            throw new KeyNotFoundException("X-WaDE-OriginalUrl header not found.");
        }
#endif
        return new ApiContext
        {
            RequestUri = requestUri
        };
    }
    
}