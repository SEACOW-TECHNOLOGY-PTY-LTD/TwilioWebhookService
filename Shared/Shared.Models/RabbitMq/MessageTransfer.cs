namespace Shared.Models.RabbitMq;

/// <summary>
/// 消息传输模型
/// </summary>
public class MessageTransfer
{
    /// <summary>
    /// 事件名称
    /// </summary>
    public string EventName { get; set; }

    /// <summary>
    /// 环境变量
    /// </summary>
    public string Environment { get; set; }

    /// <summary>
    /// 消息id
    /// </summary>
    public string MsgId { get; set; }

    /// <summary>
    /// 消息体, 事件内容序列化的内容
    /// </summary>
    public string MsgBody { get; set; }

    /// <summary>
    /// 附加数据
    /// </summary>
    public IDictionary<string, string> Items { get; set; }

    /// <summary>
    /// 发送时间
    /// </summary>
    public DateTimeOffset SendAt { get; set; }

    /// <summary>
    /// 延迟消费时间
    /// </summary>
    public DateTimeOffset? DelayAt { get; set; }
    
    /// <summary>
    /// 计划的发布时间。
    /// </summary>
    public DateTimeOffset? ScheduleTime { get; set; }

    public override string ToString()
    {
        return $"{EventName}-{MsgId}";
    }
}