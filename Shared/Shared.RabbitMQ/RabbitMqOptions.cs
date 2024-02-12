using RabbitMQ.Client;

namespace Shared.RabbitMQ;

public class RabbitMqOptions
{
    /// <summary>
    /// RabbitMQ 服务器的主机名或 IP 地址。
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// RabbitMQ 服务的端口号。默认值是 5672。
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// 用于连接 RabbitMQ 服务的用户名。
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// 用于连接 RabbitMQ 服务的密码。
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// 虚拟主机，用于隔离连接在同一个 RabbitMQ 服务上的应用。默认值是 "/"。
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// 所有发送到这个交换机的消息都会被路由到所有与它绑定的队列。
    /// </summary>
    public string ExchangeName { get; set; }
    
    /// <summary>
    /// 发送消息确认超时时间,单位秒,default:5s
    /// </summary>
    public int ConfirmTimeout { get; set; } = 5;

    /// <summary>
    /// 自定义连接创建,优先使用此属性
    /// </summary>
    public Func<ConnectionFactory>? ConnectionFactory { get; set; }
}
