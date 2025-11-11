using AnalyticsService.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace AnalyticsService.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly TimeSpan _defaultExpiry = TimeSpan.FromMinutes(15);

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;

        Task.Run(async () => await SetLeastTopUserVisitCount(int.MaxValue)).Wait();
    }

    public async Task<List<UserAnalytics>?> GetTopUsersAsync(int limit)
    {
        try
        {
            var cacheKey = $"top_users:{limit}";
            var cachedData = await _database.StringGetAsync(cacheKey);

            if (cachedData.HasValue)
            {
                _logger.LogInformation($"Cache hit for top users: {limit}");
                return JsonSerializer.Deserialize<List<UserAnalytics>>(cachedData);
            }

            _logger.LogInformation($"Cache miss for top users: {limit}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting top users from cache: {limit}");
            return null;
        }
    }

    public async Task SetTopUsersAsync(int limit, List<UserAnalytics> users)
    {
        try
        {
            var cacheKey = $"top_users:{limit}";
            var json = JsonSerializer.Serialize(users);
            await _database.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(5)); // Shorter expiry for rankings

            _logger.LogInformation($"Cached top users: {limit}");

            var newLeastVisitCount = users.LastOrDefault()?.VisitCount ?? 0;
            var leastTopUserVisitCount = await GetLeastTopUserVisitCount();

            _logger.LogInformation($"Least top user visit count: {leastTopUserVisitCount}");
            _logger.LogInformation($"New least top user visit count: {newLeastVisitCount}");

            if (leastTopUserVisitCount == null || newLeastVisitCount < leastTopUserVisitCount)
            {
                await SetLeastTopUserVisitCount(newLeastVisitCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error setting top users in cache: {limit}");
        }
    }

    public async Task InvalidateTopUsersAsync()
    {
        try
        {
            var pattern = "top_users:*";
            var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern);

            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }

            _logger.LogInformation("Invalidated all top users cache");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating top users cache");
        }
    }

    public async Task SetLeastTopUserVisitCount(int visitCount)
    {
        try
        {
            var cacheKey = "least_top_user_visit_count";
            await _database.StringSetAsync(cacheKey, visitCount, _defaultExpiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting least top user visit count in cache");
        }
    }

    public async Task<int?> GetLeastTopUserVisitCount()
    {
        try
        {
            var cacheKey = "least_top_user_visit_count";
            var cachedData = await _database.StringGetAsync(cacheKey);
            return cachedData.HasValue ? int.Parse(cachedData) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting least top user visit count from cache");
            return null;
        }
    }

    public async Task<bool> ShouldBeTopUser(UserAnalytics userAnalytics)
    {
        try
        {
            var leastTopUserVisitCount = await GetLeastTopUserVisitCount();
            _logger.LogInformation($"Least top user visit count: {leastTopUserVisitCount}, User visit count: {userAnalytics.VisitCount}");
            return userAnalytics.VisitCount >= leastTopUserVisitCount || leastTopUserVisitCount == null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user should be top user");
            return false;
        }
    }
}
