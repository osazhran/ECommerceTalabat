using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInSeconds;

        public CachedAttribute(int timeToLiveInSeconds)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var responseCacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            // Ask CLR for Creating Object form "IResponseCacheService" Explicitly

            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            var respose = await responseCacheService.GetCachedResposeAsync(cacheKey);

            // Respose is Not Cached
            if (!string.IsNullOrEmpty(respose))
            {
                var result = new ContentResult()
                {
                    Content = respose,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                context.Result = result;
                return;
            }

            var executedActionContext = await next.Invoke(); // Will Execute The Next Action Filter Or Action Itself 

            if(executedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
            {
                await responseCacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds));
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            // {{url}}/api/products?pageIndex=1&pageSize=5&sort=name

            var keyBuilder = new StringBuilder();
            
            keyBuilder.Append(request.Path); // /api/products

            // pageIndex=1
            // pageSize=5
            // sort=name

            foreach(var (key, value) in request.Query.OrderBy(Q => Q.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
                // /api/products|pageIndex-1
                // /api/products|pageIndex-1|pageSize-5
                // /api/products|pageIndex-1|pageSize-5|sort-name
            }
            return keyBuilder.ToString();
        }
    }
}
