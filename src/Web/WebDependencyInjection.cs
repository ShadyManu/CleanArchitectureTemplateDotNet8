using System.Threading.RateLimiting;
using Application.Common.Interfaces.Auth;
using Microsoft.AspNetCore.HttpOverrides;
using Web.Constants;
using Web.Services;

namespace Web;

public static class WebDependencyInjection
{
    public static void AddWebDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        // CORS policy
        services.AddCors(options =>
        {
            options.AddPolicy("LocalPolicy", b => b
                .WithOrigins([
                    "http://localhost:8100",
                    "http://host.docker.internal:8100",
                    "http://localhost:4200",
                    "http://host.docker.internal:4200",
                    "capacitor://localhost", // Capacitor ios
                    "https://localhost", // Capacitor android
                    "http://localhost"]) // Capacitor android
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
            options.AddPolicy("ProdPolicy", b => b
                .WithOrigins([
                    "capacitor://localhost", // Capacitor ios
                    "http://localhost"]) // Capacitor android
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
        });

        // Rate limiting
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(RateLimiterConstants.AnonymousUserPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = RateLimiterConstants.AnonymousUserPermitLimit,
                        Window = TimeSpan.FromSeconds(RateLimiterConstants.AnonymousUserWindowSeconds),
                    }));
        });

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        services.AddScoped<IUser, CurrentUser>();
        services.AddHttpContextAccessor();

        services.AddEndpointsApiExplorer();
    }
}
