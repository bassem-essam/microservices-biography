using AnalyticsService.Models;

namespace AnalyticsService.Services;

public class CachedAnalyticsRepository : IAnalyticsRepository
{
    private readonly IAnalyticsRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedAnalyticsRepository> _logger;

    public CachedAnalyticsRepository(
        IAnalyticsRepository repository, 
        ICacheService cacheService,
        ILogger<CachedAnalyticsRepository> logger)
    {
        _repository = repository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<UserAnalytics?> GetUserAnalyticsAsync(string userId) => await _repository.GetUserAnalyticsAsync(userId);

    public async Task<UserAnalytics> CreateUserAnalyticsAsync(string userId, DateTime createdAt)
    {
        var analytics = await _repository.CreateUserAnalyticsAsync(userId, createdAt);

        if (await _cacheService.ShouldBeTopUser(analytics))
        {
            // Invalidate top users cache as rankings might change
            await _cacheService.InvalidateTopUsersAsync();
        }
        
        return analytics;
    }

    public async Task UpdateUserVisitAsync(string userId, DateTime visitedAt)
    {
        await _repository.UpdateUserVisitAsync(userId, visitedAt);
        
        var analytics = await _repository.GetUserAnalyticsAsync(userId);

        if (await _cacheService.ShouldBeTopUser(analytics))
        {
            // Invalidate top users cache as rankings might change
            await _cacheService.InvalidateTopUsersAsync();
        }
    }

    public async Task<List<UserAnalytics>> GetTopUsersAsync(int limit)
    {
        // Try cache first
        var cached = await _cacheService.GetTopUsersAsync(limit);
        if (cached != null)
        {
            return cached;
        }

        // Cache miss - get from database
        var topUsers = await _repository.GetTopUsersAsync(limit);
        
        // Cache the result
        await _cacheService.SetTopUsersAsync(limit, topUsers);
        
        return topUsers;
    }
}

