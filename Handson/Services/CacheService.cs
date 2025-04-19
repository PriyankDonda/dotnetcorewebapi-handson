using System.Text.Json;
using Handson.IServices;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Handson.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (_cache.TryGetValue(key, out T value))
                {
                    return Task.FromResult<T?>(value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cached data for key {Key}", key);
            }
            
            return Task.FromResult<T?>(default);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var options = new MemoryCacheEntryOptions();
                
                if (expiry.HasValue)
                {
                    options.AbsoluteExpirationRelativeToNow = expiry;
                }
                else
                {
                    // Default expiry of 1 hour
                    options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                }
                
                _cache.Set(key, value, options);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching data for key {Key}", key);
            }
            
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _cache.Remove(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached data for key {Key}", key);
            }
            
            return Task.CompletedTask;
        }
}

//when using distributedcache like redis use below code
//
// public class CacheService : ICacheService
//     {
//         private readonly IDistributedCache _cache;
//         private readonly ILogger<CacheService> _logger;
//
//         public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
//         {
//             _cache = cache;
//             _logger = logger;
//         }
//
//         public async Task<T?> GetAsync<T>(string key)
//         {
//             try
//             {
//                 var cachedData = await _cache.GetStringAsync(key);
//                 
//                 if (string.IsNullOrEmpty(cachedData))
//                 {
//                     return default;
//                 }
//                 
//                 return JsonSerializer.Deserialize<T>(cachedData);
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error retrieving cached data for key {Key}", key);
//                 return default;
//             }
//         }
//
//         public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
//         {
//             try
//             {
//                 var options = new DistributedCacheEntryOptions();
//                 
//                 if (expiry.HasValue)
//                 {
//                     options.AbsoluteExpirationRelativeToNow = expiry;
//                 }
//                 else
//                 {
//                     // Default expiry of 1 hour
//                     options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
//                 }
//                 
//                 var serializedData = JsonSerializer.Serialize(value);
//                 await _cache.SetStringAsync(key, serializedData, options);
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error caching data for key {Key}", key);
//             }
//         }
//
//         public async Task RemoveAsync(string key)
//         {
//             try
//             {
//                 await _cache.RemoveAsync(key);
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error removing cached data for key {Key}", key);
//             }
//         }
//     }