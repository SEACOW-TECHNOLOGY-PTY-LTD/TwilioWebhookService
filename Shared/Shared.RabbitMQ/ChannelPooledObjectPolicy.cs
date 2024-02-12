using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

namespace Shared.RabbitMQ;

public class ChannelPooledObjectPolicy : PooledObjectPolicy<IModel>
{
    private readonly IConnection _connection;

    public ChannelPooledObjectPolicy(IConnection connection)
    {
        _connection = connection;
    }

    public override IModel Create()
    {
        return _connection.CreateModel();
    }

    public override bool Return(IModel obj)
    {
        if (obj.IsOpen)
        {
            return true;
        }

        obj.Dispose();
        return false;
    }
}
