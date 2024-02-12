using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shared.Extensions;

public static class CorsExtensions
{
    public static void AddCustomCors(this IServiceCollection services, IHostEnvironment environment)
    {
        services.AddCors(options =>
        {
            if (environment.IsDevelopment())
            {
                options.AddPolicy("EnableCORS", policyBuilder =>
                {
                    policyBuilder.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            }
            else if (environment.IsStaging())
            {
                options.AddPolicy("EnableCORS", policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyMethod() // 允许任何方法（例如GET, POST等）
                        .AllowAnyHeader() // 允许任何头
                        .AllowCredentials(); // 允许凭证
                });
            }
            else if (environment.IsProduction())
            {
                options.AddPolicy("EnableCORS", policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            }
        });
    }
}