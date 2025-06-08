using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UserManagementService.EventProcessing;

namespace UserManagementService.AsyncDataService;

public class AuthMessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor) : BackgroundService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IEventProcessor _eventProcessor = eventProcessor;
    private string _authQueueName = "";
    private string _channelName = "";
    private IChannel _channel;
    private IConnection _connection;

    private async Task InitializeRabbitMQ()
    {
        _authQueueName = _configuration["RabbitMQQueueNames:UserManagement"];
        _channelName = _configuration["RabbitMQChannels:AuthService"];
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"] ?? string.Empty,
            Port = Convert.ToInt32(_configuration["RabbitMQPort"]),
        };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(_channelName,
            ExchangeType.Fanout);
        await _channel.QueueDeclareAsync(
            queue: _authQueueName,
            durable: false,
            exclusive: true,
            autoDelete: false,
            arguments: null
        );
        await _channel.QueueBindAsync(_authQueueName, _channelName, "");
        Console.WriteLine("-->[INFO] RabbitMQ subscriber is listening...]");
        _connection.ConnectionShutdownAsync += async (sender, @event) =>
            Console.WriteLine("-->[INFO] RabbitMQ subscriber connection shutdown...]");
    }

    public override void Dispose()
    {
        if (_channel?.IsOpen ?? false)
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeRabbitMQ();
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            Console.WriteLine("-->[INFO] Received message");
            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
            _eventProcessor.ProcessEvent(notificationMessage);
        };
        await _channel.BasicConsumeAsync(_authQueueName, true, consumer);
    }
}