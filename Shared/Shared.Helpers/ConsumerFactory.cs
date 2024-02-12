using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.RabbitMQ;

namespace Shared.Helpers;

public class ConsumerFactory
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ConsumerFactory(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public IHostedService CreateConsumer(Type consumerType)
    {
        using var scope = _scopeFactory.CreateScope(); // 创建一个新的作用域
        return (IHostedService)scope.ServiceProvider.GetRequiredService(consumerType); // 从该作用域的服务提供者解析服务
    }
}