using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Shared.Models.RabbitMq;
using SqlSugar;

namespace Shared.RabbitMQ;

public interface ITopicProducer
{
    Task<bool> Publish(MessageTransfer message, CancellationToken cancellationToken);
    Task<bool> PublishToSpecificExchange(MessageTransfer message, string exchangeName, CancellationToken cancellationToken);
}

public class TopicProducer : ITopicProducer
{
    private readonly IOptionsMonitor<RabbitMqOptions> _options;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly ILogger<TopicProducer> _logger;
    private readonly IMessageSerializer _messageSerializer;
    private readonly ISqlSugarClient _db;

    public TopicProducer(IRabbitMqService rabbitMqService, ILogger<TopicProducer> logger, IOptionsMonitor<RabbitMqOptions> options, IMessageSerializer messageSerializer, ISqlSugarClient db)
    {
        _rabbitMqService = rabbitMqService;
        _logger = logger;
        _options = options;
        _messageSerializer = messageSerializer;
        _db = db;
    }
    
    public async Task<bool> Publish(MessageTransfer message, CancellationToken cancellationToken)
    {
        bool returnResult = false;
        var channel = _rabbitMqService.GetChannel();
        channel.ConfirmSelect();

        // 创建交换机, Topic类型
        channel.ExchangeDeclare(exchange: _options.CurrentValue.ExchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);
        
        // 死信交换机
        var delayExchange = _options.CurrentValue.ExchangeName + ".delay";
        var delayQueue = message.EventName + ".delay";

        channel.ExchangeDeclare(exchange: delayExchange, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);

        var delayQueueArguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", _options.CurrentValue.ExchangeName }, // 主交换机
            { "x-dead-letter-routing-key", message.EventName } // 主路由键
        };

        channel.QueueDeclare(queue: delayQueue, durable: true, exclusive: false, autoDelete: false, arguments: delayQueueArguments);
        channel.QueueBind(queue: delayQueue, exchange: delayExchange, routingKey: message.EventName);
        
        // 定义重试策略
        var retryPolicy = Policy
            .Handle<TimeoutException>()
            .Or<AlreadyClosedException>()
            .Or<AuthenticationFailureException>()
            .Or<BrokerUnreachableException>()
            .Or<ConnectFailureException>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(60), (ex, time, retryAttempt, ctx) =>
            {
                _logger.LogWarning(ex, "[EventBus-RabbitMQ] Delaying for {Delay} seconds, then making retry {Retry} of {MaxRetry}", time.TotalSeconds, retryAttempt, 5);
            });

        // 使用Polly重试策略
        await retryPolicy.ExecuteAsync(() =>
        {
            var body = _messageSerializer.SerializeToBytes(message);

            // 创建消息属性
            var properties = channel.CreateBasicProperties();
            properties.ContentType = "application/json";
            properties.DeliveryMode = 2;
            properties.MessageId = message.MsgId;
            properties.Persistent = true;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.Now.ToUnixTimeSeconds());

            if (message.DelayAt.HasValue)
            {
                var delayMilliseconds = (message.DelayAt.Value - DateTimeOffset.Now).TotalMilliseconds;
                properties.Expiration = delayMilliseconds.ToString(CultureInfo.InvariantCulture); // 设置TTL
                channel.BasicPublish(exchange: delayExchange, routingKey: message.EventName, mandatory: false, basicProperties: properties, body: body); // 发布到延迟交换机
            }
            else
            {
                channel.BasicPublish(exchange: _options.CurrentValue.ExchangeName, routingKey: message.EventName, mandatory: false, basicProperties: properties, body: body); // 发布到主交换机
            }

            // 等待消费端ack消息
            channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(_options.CurrentValue.ConfirmTimeout));

            _logger.LogDebug("[EventBus-RabbitMQ] send msg success: {Message}", message);
            returnResult = true;
            return Task.CompletedTask;
        });

        // 确保总是将通道返回到池中，无论成功还是出现异常
        _rabbitMqService.ReturnChannel(channel);

        return returnResult; // 此行代码不应该被执行，但作为附加的安全措施返回失败
    }
    
    public async Task<bool> PublishToSpecificExchange(MessageTransfer message, string exchangeName, CancellationToken cancellationToken)
    {
        bool returnResult = false;
        var channel = _rabbitMqService.GetChannel();
        channel.ConfirmSelect();

        // 创建交换机, Topic类型
        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);
        
        // 死信交换机
        var delayExchange = exchangeName + ".delay";
        var delayQueue = message.EventName + ".delay";

        channel.ExchangeDeclare(exchange: delayExchange, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);

        var delayQueueArguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", exchangeName }, // 主交换机
            { "x-dead-letter-routing-key", message.EventName } // 主路由键
        };

        channel.QueueDeclare(queue: delayQueue, durable: true, exclusive: false, autoDelete: false, arguments: delayQueueArguments);
        channel.QueueBind(queue: delayQueue, exchange: delayExchange, routingKey: message.EventName);
        
        // 定义重试策略
        var retryPolicy = Policy
            .Handle<TimeoutException>()
            .Or<AlreadyClosedException>()
            .Or<AuthenticationFailureException>()
            .Or<BrokerUnreachableException>()
            .Or<ConnectFailureException>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(60), (ex, time, retryAttempt, ctx) =>
            {
                _logger.LogWarning(ex, "[EventBus-RabbitMQ] Delaying for {Delay} seconds, then making retry {Retry} of {MaxRetry}", time.TotalSeconds, retryAttempt, 5);
            });

        // 使用Polly重试策略
        await retryPolicy.ExecuteAsync(() =>
        {
            var body = _messageSerializer.SerializeToBytes(message);

            // 创建消息属性
            var properties = channel.CreateBasicProperties();
            properties.ContentType = "application/json";
            properties.DeliveryMode = 2;
            properties.MessageId = message.MsgId;
            properties.Persistent = true;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.Now.ToUnixTimeSeconds());

            if (message.DelayAt.HasValue)
            {
                var delayMilliseconds = (message.DelayAt.Value - DateTimeOffset.Now).TotalMilliseconds;
                properties.Expiration = delayMilliseconds.ToString(CultureInfo.InvariantCulture); // 设置TTL
                channel.BasicPublish(exchange: delayExchange, routingKey: message.EventName, mandatory: false, basicProperties: properties, body: body); // 发布到延迟交换机
            }
            else
            {
                channel.BasicPublish(exchange: exchangeName, routingKey: message.EventName, mandatory: false, basicProperties: properties, body: body); // 发布到主交换机
            }

            // 等待消费端ack消息
            channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(_options.CurrentValue.ConfirmTimeout));

            _logger.LogDebug("[EventBus-RabbitMQ] send msg success: {Message}", message);
            returnResult = true;
            
            return Task.CompletedTask;
        });

        // 确保总是将通道返回到池中，无论成功还是出现异常
        _rabbitMqService.ReturnChannel(channel);

        return returnResult; // 此行代码不应该被执行，但作为附加的安全措施返回失败
    }
}