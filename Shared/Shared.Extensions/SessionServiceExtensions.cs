using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Helpers;
using Shared.Models.Common;

namespace Shared.Extensions;

public static class SessionServiceExtensions
{
    public static IServiceCollection AddSessionHelper(this IServiceCollection services, IConfiguration configuration, string appInstanceName)
    {
        // Add services to the container.
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Cache:Redis"]; // 这是 Redis 的地址，您可能需要根据实际情况进行调整
            options.InstanceName = appInstanceName; // 这是实例名称的前缀，您可以根据需要更改
        });

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); // 设置 Session 的超时时间
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        return services;
    }
}