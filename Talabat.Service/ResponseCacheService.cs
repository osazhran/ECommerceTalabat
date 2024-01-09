using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Services.Contract;

namespace Talabat.Service
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _databas;

        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            _databas = redis.GetDatabase();
        }
        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response is null)
                return;

            var serializeOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var serializedResponse = JsonSerializer.Serialize(response, serializeOptions);

            await _databas.StringSetAsync(cacheKey, serializedResponse, timeToLive);
        }

        public async Task<string?> GetCachedResposeAsync(string cacheKey)
        {
            var cachedRespose = await _databas.StringGetAsync(cacheKey);

            if (cachedRespose.IsNullOrEmpty)
                return null;

            return cachedRespose;
        }
    }
}
