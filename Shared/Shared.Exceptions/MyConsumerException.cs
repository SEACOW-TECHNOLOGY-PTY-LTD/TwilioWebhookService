namespace Shared.Exceptions;

public class MyConsumerException : Exception
{
    public MyConsumerException()
    {
    }

    public MyConsumerException(string message)
        : base(message)
    {
    }

    public MyConsumerException(string message, Exception inner)
        : base(message, inner)
    {
    }
}