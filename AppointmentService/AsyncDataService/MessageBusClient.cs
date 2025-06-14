using System.Text;
using System.Text.Json;
using AppointmentService.Dtos;
using RabbitMQ.Client;

namespace AppointmentService.AsyncDataService;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private IChannel _channel;
    private IConnection _connection;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;
        _ = ConfigureRabbit();
    }

    private async Task ConfigureRabbit()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"] ?? string.Empty,
            Port = Convert.ToInt32(_configuration["RabbitMQPort"])
        };
        try
        {
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(_configuration["RabbitMQChannel"], ExchangeType.Fanout);
            Console.WriteLine("-->[INFO] Connected to RabbitMQ");
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[INFO] RabbitMQ connection could not be initialized. {e}");
        }
    }

    private async Task SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        await _channel.BasicPublishAsync(_configuration["RabbitMQChannel"], string.Empty, body);
        Console.WriteLine($"-->[INFO] Message sent {message}");
    }

    public void Dispose()
    {
        Console.WriteLine("-->[INFO] RabbitMQ dispose");
        if (_connection.IsOpen)
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }

    public async Task PublishNewAppointment(AppointmentPublishDto appointment)
    {
        var message = JsonSerializer.Serialize(appointment);
        if (_connection.IsOpen)
        {
            Console.WriteLine("-->[INFO] RabbitMQ connection opened, sending message...");
            await SendMessage(message);
        }
        else
        {
            Console.WriteLine("-->[ERROR] RabbitMQ connection could not be opened, not sending message");
        }
    }
}