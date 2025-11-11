using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using AnalyticsService.Events;

namespace AnalyticsService.Services;

public class RabbitMQService : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMQService> _logger;

    public RabbitMQService(IServiceProvider serviceProvider, ILogger<RabbitMQService> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var factory = new ConnectionFactory()
        {
            HostName = configuration["RabbitMQ:Host"],
            Port = configuration.GetValue<int>("RabbitMQ:Port"),
            UserName = configuration["RabbitMQ:Username"],
            Password = configuration["RabbitMQ:Password"]
        };

        Console.WriteLine("RabbitMQService started with options: " + factory.ToString() + ", host: " + factory.HostName);

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare exchanges and queues
        _channel.ExchangeDeclare("user_events", ExchangeType.Topic);
        _channel.QueueDeclare("analytics_user_created", false, false, false, null);
        _channel.QueueDeclare("analytics_user_visited", false, false, false, null);
        
        _channel.QueueBind("analytics_user_created", "user_events", "user.created");
        _channel.QueueBind("analytics_user_visited", "user_events", "user.visited");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(() =>
        {
            var userCreatedConsumer = new EventingBasicConsumer(_channel);
            userCreatedConsumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await HandleUserCreatedEvent(message);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            var userVisitedConsumer = new EventingBasicConsumer(_channel);
            userVisitedConsumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await HandleUserVisitedEvent(message);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("analytics_user_created", false, userCreatedConsumer);
            _channel.BasicConsume("analytics_user_visited", false, userVisitedConsumer);

            _logger.LogInformation("RabbitMQ consumers started");

            while (!stoppingToken.IsCancellationRequested)
            {
                Thread.Sleep(1000);
            }
        }, stoppingToken);
    }

    private async Task HandleUserCreatedEvent(string message)
    {
        try
        {
            // Console.WriteLine("UserCreated Event Message: " + message);
            // return;
            var userCreatedEvent = JsonSerializer.Deserialize<UserCreatedEvent>(message);
            if (userCreatedEvent != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IAnalyticsRepository>();
                
                await repository.CreateUserAnalyticsAsync(userCreatedEvent.UserId, userCreatedEvent.CreatedAt);
                _logger.LogInformation($"User analytics created for user: {userCreatedEvent.UserId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing UserCreated event");
        }
    }

    private async Task HandleUserVisitedEvent(string message)
    {
        try
        {
            // Console.WriteLine("User Visited Event Message: " + message);
            // return;
            var userVisitedEvent = JsonSerializer.Deserialize<UserVisitedEvent>(message);
            if (userVisitedEvent != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IAnalyticsRepository>();
                
                // Ensure user analytics exists
                var userAnalytics = await repository.GetUserAnalyticsAsync(userVisitedEvent.UserId);
                if (userAnalytics == null)
                {
                    await repository.CreateUserAnalyticsAsync(userVisitedEvent.UserId, userVisitedEvent.VisitedAt);
                }
                
                await repository.UpdateUserVisitAsync(userVisitedEvent.UserId, userVisitedEvent.VisitedAt);
                _logger.LogInformation($"Visit recorded for user: {userVisitedEvent.UserId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing UserVisited event");
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
