using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace EventBus.RabbitMQ;

public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
    private readonly int _retryCount;
    private IConnection? _connection;
    public bool Disposed;

    readonly object _syncRoot = new();

    public DefaultRabbitMQPersistentConnection(IConnectionFactory connectionFactory, ILogger<DefaultRabbitMQPersistentConnection> logger, int retryCount = 5)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        _retryCount = retryCount;
    }

    public bool IsConnected => _connection is { IsOpen: true } && !Disposed;

    public IModel CreateModel()
    {
        if (!IsConnected || _connection == null)
            throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");

        return _connection.CreateModel();
    }

    public void Dispose()
    {
        if (Disposed)
            return;

        Disposed = true;

        try
        {
            if (_connection == null)
                throw new NullReferenceException("IConnection is null");

            _connection.ConnectionShutdown += OnConnectionShutdown!;
            _connection.CallbackException += OnCallbackException!;
            _connection.ConnectionBlocked += OnConnectionBlocked!;
            _connection.Dispose();
        }
        catch (IOException ex)
        {
            _logger.LogCritical(ex.ToString());
        }
    }

    public bool TryConnect()
    {
        _logger.LogInformation("RabbitMQ Client is trying to connect");

        lock (_syncRoot)
        {
            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                });

            policy.Execute(() => _connection = _connectionFactory.CreateConnection());

            if (IsConnected && _connection != null)
            {
                _connection.ConnectionShutdown += OnConnectionShutdown!;
                _connection.CallbackException += OnCallbackException!;
                _connection.ConnectionBlocked += OnConnectionBlocked!;

                _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.Endpoint.HostName);

                return true;
            }
            else
            {
                _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");
                return false;
            }
        }
    }

    private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
    {
        if (Disposed)
            return;

        _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

        TryConnect();
    }

    private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
    {
        if (Disposed)
            return;

        _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

        TryConnect();
    }

    private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
    {
        if (Disposed)
            return;

        _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

        TryConnect();
    }
}
