using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.Models.Log;
using Shared.Models.RabbitMq;
using Shared.RabbitMQ;
using SqlSugar;

namespace WebhookService.Producers;

public class TwilioEventProducer(
    ITopicProducer topicProducer,
    ISqlSugarClient db,
    ILogger<TwilioEventProducer> logger,
    IOptionsMonitor<RabbitMqOptions> options,
    IWebHostEnvironment env)
{
    public async Task Send(Dictionary<string, string> message, string eventName, string eventHandlerName)
    {
        var eventSid = message.ContainsKey("Sid") && !string.IsNullOrEmpty(message["Sid"]) 
            ? message["Sid"] 
            : Guid.NewGuid().ToString();

        var twilioEventMessageLog = new TwilioEventMessageLog()
        {
            Message = JsonConvert.SerializeObject(message),
            Exchange = options.CurrentValue.ExchangeName,
            RoutingKey = eventName,
            Queue = eventHandlerName,
            EventSid = eventSid,
            PublishAt = DateTime.Now,
            ProcessResult = "Published",
        };

        message.Add("TrackingId", eventSid);

        // 步骤1: 将消息写入数据库
        await db.Insertable(twilioEventMessageLog).ExecuteCommandAsync();

        // 步骤2: 尝试执行发送消息
        const int maxRetryCount = 3;
        bool success = false;
        for (int retry = 0; retry < maxRetryCount; retry++)
        {
            try
            {
                success = await topicProducer.Publish(new MessageTransfer()
                {
                    EventName = eventName,
                    Environment = env.EnvironmentName,
                    MsgId = eventSid,
                    MsgBody = eventSid,
                    Items = message,
                    SendAt = DateTimeOffset.Now
                }, CancellationToken.None);

                if (success)
                {
                    // 步骤3: 如果发送成功，则更新数据库记录
                    break;
                }
            }
            catch (Exception publishEx)
            {
                logger.LogError(publishEx, "Publishing message failed on retry {Retry}", retry);
            }

            // 步骤4: 如果发送失败，则更新数据库记录，等待60秒后，重新执行步骤2
            twilioEventMessageLog.ProcessResult = "Failed";
            twilioEventMessageLog.ErrorMessages = "Publish failed on retry " + retry;
            await db.Updateable(twilioEventMessageLog).ExecuteCommandAsync();
            await Task.Delay(TimeSpan.FromSeconds(60)); // Retry delay
        }

        // 步骤5: 如果重试3次，依然失败，则更新数据库记录
        if (!success)
        {
            twilioEventMessageLog.ProcessResult = "Failed";
            twilioEventMessageLog.ErrorMessages = "Publish failed after " + maxRetryCount + " retries";
            await db.Updateable(twilioEventMessageLog).ExecuteCommandAsync();
        }
    }
}