using AnalyticsService.Models;

namespace AnalyticsService.Services;

public interface IAnalyticsRepository
{
    Task<UserAnalytics?> GetUserAnalyticsAsync(string userId);
    Task<UserAnalytics> CreateUserAnalyticsAsync(string userId, DateTime createdAt);
    Task UpdateUserVisitAsync(string userId, DateTime visitedAt);
    Task<List<UserAnalytics>> GetTopUsersAsync(int limit);
}

