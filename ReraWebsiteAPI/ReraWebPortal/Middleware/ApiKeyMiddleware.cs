using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ReraWebPortal.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEY_HEADER = "X-API-KEY";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration config)
        {
            var endpoint = context.GetEndpoint();
            var hasAuth = endpoint?.Metadata.GetMetadata<ApiKeyAuthAttribute>();

            if (hasAuth == null)
            {
                await _next(context);
                return;
            }

            // 1️⃣ API KEY CHECK
            if (!context.Request.Headers.TryGetValue(APIKEY_HEADER, out var apiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API Key missing");
                return;
            }

            var validKey = config["ApiKeySettings:ApiKey"];

            if (!apiKey.Equals(validKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            // 2️⃣ CLIENT IP CHECK
            var remoteIp = context.Connection.RemoteIpAddress;

            if (remoteIp == null)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("IP not detected");
                return;
            }

            var allowedIps = config
                .GetSection("ApiKeySettings:AllowedIPs")
                .Get<string[]>();

            var clientIp = remoteIp.MapToIPv4().ToString();

            //if (!allowedIps.Contains(clientIp))
            //{
            //    context.Response.StatusCode = StatusCodes.Status403Forbidden;
            //    await context.Response.WriteAsync($"IP {clientIp} not allowed");
            //    return;
            //}

            await _next(context);
        }
    }

}
