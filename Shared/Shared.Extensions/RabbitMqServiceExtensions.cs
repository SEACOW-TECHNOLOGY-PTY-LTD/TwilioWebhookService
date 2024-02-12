using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using Shared.Models.RabbitMq;
using Shared.RabbitMQ;

namespace Shared.Extensions;

public static class RabbitMqServiceExtensions
{
    // public static IServiceCollection AddRabbitMqServices(this IServiceCollection services, IConfiguration configuration)
    // {
    //     services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
    //     services.AddScoped<IRabbitMqService, RabbitMqService>();
    //     
    //     // 添加连接池
    //     services.AddSingleton<RabbitMqConnectionPool>();
    //
    //     // 获取连接池服务
    //     services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
    //     services.AddSingleton<ObjectPool<IModel>>(provider =>
    //     {
    //         var rabbitMqConnectionPool = provider.GetRequiredService<RabbitMqConnectionPool>();
    //         var connection = rabbitMqConnectionPool.GetConnection();
    //
    //         var policy = new ChannelPooledObjectPolicy(connection);
    //         var poolProvider = provider.GetRequiredService<ObjectPoolProvider>();
    //         return poolProvider.Create(policy);
    //     });
    //
    //     return services;
    // }
    
    public static IServiceCollection AddRabbitMqServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.AddScoped<IRabbitMqService, RabbitMqService>();

        // 添加连接池
        services.AddSingleton<RabbitMqConnectionPool>();

        // 获取连接池服务
        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton<ObjectPool<IModel>>(provider =>
        {
            var rabbitMqConnectionPool = provider.GetRequiredService<RabbitMqConnectionPool>();
            var connection = rabbitMqConnectionPool.GetConnection();

            var policy = new ChannelPooledObjectPolicy(connection);

            // 设置最大通道数量为100
            return new DefaultObjectPool<IModel>(policy, 100); // 限制通道数量
        });

        return services;
    }

}