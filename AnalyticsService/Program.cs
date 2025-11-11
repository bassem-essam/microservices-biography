using AnalyticsService.Services;
using MongoDB.Driver;
using Serilog;
using StackExchange.Redis;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddGrpc();

var connectionString = builder.Configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
var databaseName = builder.Configuration["DatabaseName"] ?? "analytics_db";

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(databaseName);
});

var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(redisConnectionString);
});

builder.Services.AddSingleton<ICacheService, RedisCacheService>();


// builder.Services.AddSingleton<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddSingleton<AnalyticsRepository>();
builder.Services.AddSingleton<IAnalyticsRepository>(sp =>
{
    var baseRepository = sp.GetRequiredService<AnalyticsRepository>();
    var cacheService = sp.GetRequiredService<ICacheService>();
    var logger = sp.GetRequiredService<ILogger<CachedAnalyticsRepository>>();
    return new CachedAnalyticsRepository(baseRepository, cacheService, logger);
});




builder.Services.AddHostedService<RabbitMQService>();

builder.Services.AddDiscoveryClient(builder.Configuration);

var app = builder.Build();

app.MapGrpcService<AnalyticsGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

