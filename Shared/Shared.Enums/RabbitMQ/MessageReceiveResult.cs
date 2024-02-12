namespace Shared.Enums.RabbitMQ;

public enum MessageReceiveResult
{
    Success,
    Failed,
    AlreadyExists,
    Exception,
    Ignore,
    Retry
}