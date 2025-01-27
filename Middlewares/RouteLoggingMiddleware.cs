using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trabahuso_api.Middlewares
{
    public class RouteLoggingMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<RouteLoggingMiddleware> _logger;

        public RouteLoggingMiddleware(RequestDelegate next, ILogger<RouteLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("------------------------------");

            _logger.LogInformation("Incoming Request: {Method} {Url}", context.Request.Method, context.Request.Path);

            if (context.Request.QueryString.HasValue)
            {
                _logger.LogInformation("Query Parameters: {Query}", context.Request.QueryString.Value);
            }

            var clientIpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            _logger.LogInformation("Client IP Address: {ClientIP}", clientIpAddress);

            _logger.LogInformation("------------------------------");

            await _next(context);
        }

    }
    public static class RouteLoggingMiddlewareExtension
    {
        public static IApplicationBuilder UseRouteLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RouteLoggingMiddleware>();
        }
    }
}