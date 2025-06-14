using System.Text;
using AppointmentService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AppointmentService.AsyncDataService;

public class PatientMessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    : BackgroundService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IEventProcessor _eventProcessor = eventProcessor;
    private string _patientQueueName = "";
    private string _channelName = "";
    private IChannel _channel;
    private IConnection _connection;

    private async Task InitializeRabbitMQ()
    {
        _patientQueueName = _configuration["RabbitMQQueueNames:Patient"];
        _channelName = _configuration["RabbitMQChannels:Patient"];
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"] ?? string.Empty,
            Port = Convert.ToInt32(_configuration["RabbitMQPort"]),
        };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(_channelName, ExchangeType.Fanout);
        await _channel.QueueDeclareAsync(
            queue: _patientQueueName,
            durable: false,
            exclusive: true,
            autoDelete: false,
            arguments: null);
        await _channel.QueueBindAsync(_patientQueueName, _channelName, "");
        Console.WriteLine("-->[INFO] RabbitMQ subscriber is listening...");
        _connection.ConnectionShutdownAsync += async (sender, @event) =>
            Console.WriteLine("-->[INFO] RabbitMQ subscriber connection shutdown...");
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
            Console.WriteLine("-->[INFO] Patient Received message");
            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
            _eventProcessor.ProcessEvent(notificationMessage, "patient");
        };
        await _channel.BasicConsumeAsync(_patientQueueName, false, consumer);
    }
}