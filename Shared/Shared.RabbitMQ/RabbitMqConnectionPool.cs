using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;

namespace Shared.RabbitMQ;

public class RabbitMqConnectionPool
{
    private readonly ConcurrentBag<IConnection> _connections;
    private readonly ConnectionFactory _connectionFactory;
    private readonly Policy _retryPolicy;
    private const int MaxConnections = 50;

    public RabbitMqConnectionPool(IOptionsMonitor<RabbitMqOptions> options)
    {
        _connections = new ConcurrentBag<IConnection>();
        _connectionFactory = new ConnectionFactory
        {
            Password = options.CurrentValue.Password,
            HostName = options.CurrentValue.HostName,
            UserName = options.CurrentValue.UserName,
            Port = options.CurrentValue.Port,
            VirtualHost = options.CurrentValue.VirtualHost
        };

        _retryPolicy = Policy.Handle<Exception>()
            .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public IConnection GetConnection()
    {
        if (_connections.Count >= MaxConnections) throw new InvalidOperationException("Max connections reached in pool.");
        
        if (_connections.TryTake(out var connection) && connection.IsOpen)
        {
            return connection;
        }

        var newConnection = _retryPolicy.Execute(() => _connectionFactory.CreateConnection());
        _connections.Add(newConnection);
        return newConnection;
    }
}