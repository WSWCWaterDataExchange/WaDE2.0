using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace WaDEImportFunctions
{
    public class HttpContextAccessorMiddleware(IHttpContextAccessor httpContextAccessor) : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            httpContextAccessor.HttpContext = context.GetHttpContext();
            await next(context);
        }
    }
}