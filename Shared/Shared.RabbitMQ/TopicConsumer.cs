using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Shared.Enums.RabbitMQ;
using Shared.Exceptions;
using Shared.Models.Log;
using Shared.Models.RabbitMq;
using SqlSugar;

namespace Shared.RabbitMQ;

/// <summary>
/// 定义事件订阅者的接口。任何需要订阅事件的服务都应该实现此接口。
/// </summary>
public interface ITopicConsumer : IHostedService
{
    /// <summary>
    /// 异步订阅指定类型的事件。当有此类型的事件被发布时，指定的事件处理器会被调用。
    /// </summary>
    /// <param name="descriptor">事件处理器描述符，包括事件名称和处理器名称等信息。</param>
    /// <returns>表示异步订阅操作的任务。</returns>
    // Task SubscribeAsync(EventHandlerDescriptor descriptor, CancellationToken cancellationToken)
    // {
    //     throw new NotImplementedException();
    // }
    void Initialize(EventHandlerDescriptor descriptor);
}

public abstract class TopicConsumer : ITopicConsumer
{
    private readonly IOptionsMonitor<RabbitMqOptions> _options;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly ILogger<TopicConsumer> _logger;
    private readonly IMessageSerializer _messageSerializer;
    private readonly ISqlSugarClient _db;
    private EventHandlerDescriptor _descriptor;
    
    protected TopicConsumer(IOptionsMonitor<RabbitMqOptions> options, IRabbitMqService rabbitMqService, ILogger<TopicConsumer> logger, IMessageSerializer messageSerializer, ISqlSugarClient db)
    {
        _options = options;
        _rabbitMqService = rabbitMqService;
        _logger = logger;
        _messageSerializer = messageSerializer;
        _db = db;
    }
    
    public void Initialize(EventHandlerDescriptor descriptor)
    {
        _descriptor = descriptor;
    }
    
    /// <summary>
    /// 子类必须实现的消息处理方法。
    /// </summary>
    /// <param name="message">接收到的消息。</param>
    /// <param name="cancellationToken">用于取消操作的取消标记。</param>
    /// <returns>处理结果。</returns>
    protected abstract Task<MessageReceiveResult> HandleMessageAsync(MessageTransfer message, CancellationToken cancellationToken);

    /// <summary>
    /// 异步订阅指定类型的事件。
    /// </summary>
    /// <param name="descriptor">事件处理器描述符，包括事件名称和事件处理器名称。</param>
    /// <param name="cancellationToken">用于取消操作的取消标记。</param>
    /// <returns>表示异步操作的任务。</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        // 获取事件名称和事件处理器名称
        var eventName = _descriptor.EventName;
        var eventHandlerName = _descriptor.EventHandlerName;

        // 获取 RabbitMQ 的通道
        var channel = _rabbitMqService.GetChannel();

        // 注册基础通信交换机，类型为 topic
        channel.ExchangeDeclare(exchange: _options.CurrentValue.ExchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);

        // 定义死信交换机和队列
        var deadLetterExchange = _options.CurrentValue.ExchangeName + ".dead";
        var deadLetterQueue = eventHandlerName + ".dead";
        channel.ExchangeDeclare(exchange: deadLetterExchange, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);
        channel.QueueDeclare(queue: deadLetterQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue: deadLetterQueue, exchange: deadLetterExchange, routingKey: eventName, arguments: null);

        // 定义队列
        var args = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", deadLetterExchange }, // 死信交换机
            { "x-dead-letter-routing-key", eventName } // 死信路由键
        };
        channel.QueueDeclare(queue: eventHandlerName, durable: true, exclusive: false, autoDelete: false, arguments: args);

        // 绑定队列到交换机
        channel.QueueBind(queue: eventHandlerName, exchange: _options.CurrentValue.ExchangeName, routingKey: eventName, arguments: null);

        // 创建消费者
        var consumer = new EventingBasicConsumer(channel);
        
        // prefetchCount:1来告知RabbitMQ,不要同时给一个消费者推送多于 N 个消息，也确保了消费速度和性能
        // global：是否设为全局的
        // prefetchSize:单条消息大小，通常设0，表示不做限制
        // 是autoAck=false才会有效
        // _channel?.BasicQos(prefetchSize: 0, prefetchCount:1, global: true);

        consumer.Received += async (_, ea) =>
        {
            try
            {
                // 检查取消请求
                if (cancellationToken.IsCancellationRequested) return;

                // 尝试反序列化消息
                MessageTransfer? message;
                try
                {
                    message = _messageSerializer.Deserialize<MessageTransfer>(ea.Body.ToArray());
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "[EventBus-RabbitMQ] Deserialize message from rabbit error");
                    return;
                }

                // 验证消息有效性
                if (message is null || message.EventName != eventName)
                {
                    _logger.LogError("[EventBus-RabbitMQ] Received invalid event name \"{EventName}\", expect \"{ExpectedEventName}\"", message?.EventName, eventName);
                    return;
                }
                
                // 定义重试策略
                var retryPolicy = Policy
                    .Handle<AlreadyClosedException>() // 连接已关闭
                    .Or<OperationInterruptedException>() // 操作中断
                    .Or<IOException>() // I/O错误
                    .Or<TimeoutException>() // 超时
                    .Or<BrokerUnreachableException>() // 无法连接到服务器
                    .Or<ChannelAllocationException>() // 无法分配通道
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(60), (ex, time, retryAttempt, ctx) =>
                    {
                        _logger.LogWarning(ex, "[EventBus-RabbitMQ] Delaying for {Delay} seconds, then making retry {Retry}", time.TotalSeconds, retryAttempt);
                    });
                
                // 使用Polly重试策略
                await retryPolicy.ExecuteAsync(async () =>
                {
                    var res = await HandleMessageAsync(message, cancellationToken).ConfigureAwait(false);
                    switch (res)
                    {
                        case MessageReceiveResult.Success:
                            _logger.LogInformation("[EventBus-RabbitMQ] Handle message success");
                            channel.BasicAck(ea.DeliveryTag, false); // 确认消息
                            break;
                        
                        case MessageReceiveResult.Failed:
                            _logger.LogError("MessageReceiveResult.Failed");
                            channel.BasicReject(ea.DeliveryTag, false); // 拒绝消息，消息将被发送到死信队列
                            break;
                        
                        case MessageReceiveResult.Ignore:
                            _logger.LogError("MessageReceiveResult.Ignore");
                            channel.BasicAck(ea.DeliveryTag, false); // 确认消息
                            break;
                        
                        case MessageReceiveResult.Retry:
                            _logger.LogError("MessageReceiveResult.Retry");
                            channel.BasicReject(ea.DeliveryTag, true); // 放回队列重试
                            break;
                        
                        case MessageReceiveResult.Exception:
                            _logger.LogError("MessageReceiveResult.Exception");
                            throw new BusinessLogicException("Handle message failed"); // 自定义业务逻辑异常
                        
                        case MessageReceiveResult.AlreadyExists:
                            _logger.LogError("MessageReceiveResult.AlreadyExists");
                            channel.BasicAck(ea.DeliveryTag, false); // 确认消息
                            break;
                        
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
            }
            catch (BusinessLogicException ex)
            {
                _logger.LogError(ex, "[EventBus-RabbitMQ] Business logic error, not retrying");
                channel.BasicReject(ea.DeliveryTag, false); // 拒绝消息，消息将被发送到死信队列
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EventBus-RabbitMQ] Unexpected error while receiving message");
                channel.BasicReject(ea.DeliveryTag, false); // 拒绝消息，消息将被发送到死信队列
            }
        };

        // 定义消费者注册和关闭事件
        consumer.Registered += (_, _) => _logger.LogInformation("[EventBus-RabbitMQ] Event handler \"{EventHandlerName}\" has been registered", eventHandlerName);
        consumer.Shutdown += (_, _) => _logger.LogWarning("[EventBus-RabbitMQ] Event handler \"{EventHandlerName}\" has been shutdown", eventHandlerName);

        // 开始消费
        channel.BasicConsume(queue: eventHandlerName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
    private async Task UpdateTwilioEventMessageLogAsync(MessageTransfer message, MessageReceiveResult res, CancellationToken cancellationToken)
    {
        // 更新TwilioEventMessageLog的逻辑
        var log = await _db.Queryable<TwilioEventMessageLog>().FirstAsync(l => l.EventSid != null && l.EventSid.Equals(message.MsgId), cancellationToken);

        if (log is null) return;
        
        log.ConsumedAt = DateTime.Now;
        switch (res)
        {
            case MessageReceiveResult.Success:
                log.ProcessResult = "Success";
                break;
            case MessageReceiveResult.Failed:
                log.ProcessResult = "Failed";
                break;
            case MessageReceiveResult.AlreadyExists:
                log.ProcessResult = "AlreadyExists";
                break;
            case MessageReceiveResult.Exception:
                log.ProcessResult = "Exception";
                break;
        }

        await _db.Updateable(log).ExecuteCommandAsync(cancellationToken);
    }

    private async Task UpdateRabbitMqMessageLogAsync(MessageTransfer message, MessageReceiveResult res, CancellationToken cancellationToken)
    {
        // 更新TwilioEventMessageLog的逻辑
        var log = await _db.CopyNew().Queryable<RabbitMqMessageLog>().FirstAsync(l => l.TrackingId.Equals(message.MsgId), cancellationToken);
        
        if (log is null) return;
        
        log.ConsumedAt = DateTime.Now;
        switch (res)
        {
            case MessageReceiveResult.Success:
                log.ProcessResult = "Success";
                break;
            case MessageReceiveResult.Failed:
                log.ProcessResult = "Failed";
                break;
            case MessageReceiveResult.AlreadyExists:
                log.ProcessResult = "AlreadyExists";
                break;
            case MessageReceiveResult.Exception:
                log.ProcessResult = "Exception";
                break;
        }

        await _db.Updateable(log).ExecuteCommandAsync(cancellationToken);
    }
}