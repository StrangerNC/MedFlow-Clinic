using System.Text;
using System.Text.Json;
using AuthService.Dtos;
using RabbitMQ.Client;

namespace AuthService.AsyncDataService;

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

    public async Task PublishNewUser(UserPublishDto user)
    {
        var message = JsonSerializer.Serialize(user);
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
            _connection.ConnectionShutdownAsync +=
                async (sender, @event) => Console.WriteLine("-->[INFO] RabbitMQ connection shutdown");
            Console.WriteLine("-->[INFO] Connected to RabbitMQ");
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] RabbitMQ connection shutdown exception {e}");
        }
    }

    private async Task SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        await _channel.BasicPublishAsync(_configuration["RabbitMQChannel"],
            string.Empty, body);
        Console.WriteLine($"-->[INFO] Message sent {message}");
    }

    public void Dispose()
    {
        Console.WriteLine("-->[INFO] RabbitMQ disposing");
        if (_connection.IsOpen)
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}