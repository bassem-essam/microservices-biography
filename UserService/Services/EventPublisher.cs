using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace UserService.Services;

public class EventPublisher : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public EventPublisher()
    {
        var factory = new ConnectionFactory()
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
            Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672"),
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare the exchange (should match AnalyticsService)
        _channel.ExchangeDeclare("user_events", ExchangeType.Topic);
    }

    public void PublishUserCreated(string username)
    {
        var eventData = new Events.UserCreatedEvent
        {
            UserId = username,
            CreatedAt = DateTime.UtcNow
        };

        var message = JsonSerializer.Serialize(eventData);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: "user_events",
            routingKey: "user.created",
            basicProperties: null,
            body: body);

        Console.WriteLine($"✓ Published UserCreated event for user: {username}");
    }

    public void PublishUserVisited(string username)
    {
        var eventData = new Events.UserVisitedEvent
        {
            UserId = username,
            VisitedAt = DateTime.UtcNow,
        };

        var message = JsonSerializer.Serialize(eventData);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: "user_events",
            routingKey: "user.visited",
            basicProperties: null,
            body: body);

        Console.WriteLine($"✓ Published UserVisited event for user: {username}");
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}

