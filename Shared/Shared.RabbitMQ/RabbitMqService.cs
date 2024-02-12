using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Models.RabbitMq;

namespace Shared.RabbitMQ;

public interface IRabbitMqService : IDisposable
{
    IModel GetChannel();
    void ReturnChannel(IModel channel);
}

public sealed class RabbitMqService : IRabbitMqService
{
    private readonly ObjectPool<IModel> _channelPool;

    public RabbitMqService(ObjectPool<IModel> channelPool)
    {
        _channelPool = channelPool;
    }

    public IModel GetChannel()
    {
        return _channelPool.Get();
    }

    public void ReturnChannel(IModel channel)
    {
        _channelPool.Return(channel);
    }

    public void Dispose()
    {
        // Optionally close and dispose resources.
        // Note: The ObjectPool will manage the disposal of its pooled objects,
        // so we don't need to dispose the channel pool here.
    }
}
