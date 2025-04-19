using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;

namespace Handson.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        private readonly int _requestLimit;
        private readonly TimeSpan _timeWindow;

        public RateLimitingMiddleware(
            RequestDelegate next,
            ILogger<RateLimitingMiddleware> logger,
            IMemoryCache cache,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _cache = cache;
            _configuration = configuration;

            _requestLimit = _configuration.GetValue<int>("RateLimiting:RequestLimit", 100);
            _timeWindow = TimeSpan.FromSeconds(_configuration.GetValue<int>("RateLimiting:TimeWindowSeconds", 60));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            
            // Skip rate limiting if endpoint has DisableRateLimiting attribute
            if (endpoint?.Metadata.GetMetadata<DisableRateLimitingAttribute>() != null)
            {
                await _next(context);
                return;
            }

            var requestKey = GenerateClientKey(context);
            
            // Get current request count
            _cache.TryGetValue(requestKey, out RequestTracker tracker);
            
            if (tracker == null)
            {
                tracker = new RequestTracker
                {
                    Count = 0,
                    FirstRequest = DateTime.UtcNow
                };
            }

            // Check if time window has elapsed
            if (DateTime.UtcNow > tracker.FirstRequest.Add(_timeWindow))
            {
                // Reset if outside time window
                tracker.Count = 0;
                tracker.FirstRequest = DateTime.UtcNow;
            }

            // Increment request count
            tracker.Count++;

            // Store updated tracker
            _cache.Set(requestKey, tracker, _timeWindow);

            // Check if limit is exceeded
            if (tracker.Count > _requestLimit)
            {
                _logger.LogWarning("Rate limit exceeded for client: {ClientIP}", GetClientIpAddress(context));
                
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers.Add("Retry-After", _timeWindow.TotalSeconds.ToString());
                
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Rate limit exceeded. Try again later."
                });
                
                return;
            }

            await _next(context);
        }

        private string GenerateClientKey(HttpContext context)
        {
            var clientIp = GetClientIpAddress(context);
            
            // You might want to include the user ID if authenticated
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
            
            // Include the path to scope rate limiting to specific endpoints if needed
            var path = context.Request.Path.ToString();
            
            return $"rate_limit_{clientIp}_{userId}_{path}";
        }

        private string GetClientIpAddress(HttpContext context)
        {
            // Get client IP - checking forwarded headers for clients behind proxies
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private class RequestTracker
        {
            public int Count { get; set; }
            public DateTime FirstRequest { get; set; }
        }
}

// Attribute to disable rate limiting for specific endpoints
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class DisableRateLimitingAttribute : Attribute
{
}

// Extension method for easy registration in Startup
public static class RateLimitingMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitingMiddleware>();
    }
}