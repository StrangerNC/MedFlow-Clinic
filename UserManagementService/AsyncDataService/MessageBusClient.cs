using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using UserManagementService.Dtos;

namespace UserManagementService.AsyncDataService;

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
            Port = Convert.ToInt32(_configuration["RabbitMQPort"]),
        };
        try
        {
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(_configuration["RabbitMQChannel"], ExchangeType.Fanout);
            _connection.ConnectionShutdownAsync += async (sender, args) =>
                Console.WriteLine("-->[INFO] Connection to RabbitMQ established");
            Console.WriteLine("-->[INFO] Connection to RabbitMQ established");
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] RabbitMQ connection shutdown exception: {e}");
        }
    }

    private async Task SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        await _channel.BasicPublishAsync(_configuration["RabbitMQChannel"], string.Empty, body);
        Console.WriteLine("-->[INFO] Message sent {message}");
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

    public async Task PublishNewUserProfile(UserProfilePublishDto userProfile)
    {
        var message = JsonSerializer.Serialize(userProfile);
        if (_connection.IsOpen)
        {
            Console.WriteLine("-->[INFO] Connection to RabbitMQ established");
            await SendMessage(message);
        }
        else
        {
            Console.WriteLine("-->[ERROR] Connection to RabbitMQ established");
        }
    }
}