namespace Shared.Helpers;

[AttributeUsage(AttributeTargets.Class)]
public class ConsumerAttribute : Attribute
{
    public string EventName { get; }
    public string EventHandlerName { get; }

    public ConsumerAttribute(string eventName, string eventHandlerName)
    {
        EventName = eventName;
        EventHandlerName = eventHandlerName;
    }
}
