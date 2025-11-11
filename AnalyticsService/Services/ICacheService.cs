using AnalyticsService.Models;

namespace AnalyticsService.Services;

public interface ICacheService
{
    // Task<UserAnalytics?> GetUserAnalyticsAsync(string userId);
    // Task SetUserAnalyticsAsync(string userId, UserAnalytics analytics);
    // Task InvalidateUserAnalyticsAsync(string userId);
    Task<List<UserAnalytics>?> GetTopUsersAsync(int limit);
    Task SetTopUsersAsync(int limit, List<UserAnalytics> users);
    Task InvalidateTopUsersAsync();
    Task<int?> GetLeastTopUserVisitCount();
    Task SetLeastTopUserVisitCount(int visitCount);
    Task<bool> ShouldBeTopUser(UserAnalytics userAnalytics);
}
