using System.Reflection;
using Shared.Helpers;
using Shared.Models.RabbitMq;
using Shared.RabbitMQ;

namespace WebhookService.BackgroundServices;

public class ConsumerHostedService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<IHostedService> _consumerServices;
    private readonly ILogger<ConsumerHostedService> _logger;

    public ConsumerHostedService(IServiceProvider serviceProvider, ILogger<ConsumerHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _consumerServices = new List<IHostedService>();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 获取当前程序集
        var assembly = Assembly.GetExecutingAssembly();

        // 使用 LINQ 查询找到需要注册的消费者类型
        var typesToRegister = assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(TopicConsumer).IsAssignableFrom(type));

        using var scope = _serviceProvider.CreateScope();
        foreach (var type in typesToRegister)
        {
            // 通过作用域创建消费者服务的实例
            if (ActivatorUtilities.CreateInstance(scope.ServiceProvider, type) is TopicConsumer consumerService)
            {
                // 获取ConsumerAttribute特性
                var consumerAttribute = type.GetCustomAttribute<ConsumerAttribute>();
                if (consumerAttribute != null)
                {
                    // 使用特性中的值设置描述符
                    consumerService.Initialize(new EventHandlerDescriptor
                    {
                        EventName = consumerAttribute.EventName,
                        EventHandlerName = consumerAttribute.EventHandlerName,
                        EventHandlerType = type
                    });

                    // 启动消费者服务
                    await consumerService.StartAsync(cancellationToken);
                    _consumerServices.Add(consumerService);

                    // Log the registration
                    _logger.LogInformation("Registered consumer: {FullName}", type.FullName);
                }
                else
                {
                    // Log a warning if the ConsumerAttribute is not found
                    _logger.LogError("ConsumerAttribute not found for consumer: {FullName}", type.FullName);
                }
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // 停止并释放所有消费者服务
        foreach (var service in _consumerServices)
        {
            await service.StopAsync(cancellationToken);
            if (service is IDisposable disposableService)
            {
                disposableService.Dispose();
            }
        }
    }

    public void Dispose()
    {
        foreach (var service in _consumerServices)
        {
            (service as IDisposable)?.Dispose();
        }
    }
}