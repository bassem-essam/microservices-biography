using AnalyticsService.Models;
using MongoDB.Driver;

namespace AnalyticsService.Services;

public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly IMongoCollection<UserAnalytics> _collection;

    public AnalyticsRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<UserAnalytics>("user_analytics");
        
        // Create indexes for better performance
        var indexBuilder = Builders<UserAnalytics>.IndexKeys;
        _collection.Indexes.CreateOneAsync(new CreateIndexModel<UserAnalytics>(
            indexBuilder.Ascending(x => x.UserId)));
        _collection.Indexes.CreateOneAsync(new CreateIndexModel<UserAnalytics>(
            indexBuilder.Descending(x => x.VisitCount)));
    }

    public async Task<UserAnalytics?> GetUserAnalyticsAsync(string userId)
    {
        return await _collection.Find(x => x.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task<UserAnalytics> CreateUserAnalyticsAsync(string userId, DateTime createdAt)
    {
        var analytics = new UserAnalytics
        {
            UserId = userId,
            VisitCount = 0,
            CreatedAt = createdAt,
            VisitDates = new List<DateTime>()
        };

        await _collection.InsertOneAsync(analytics);
        return analytics;
    }

    public async Task UpdateUserVisitAsync(string userId, DateTime visitedAt)
    {
        var filter = Builders<UserAnalytics>.Filter.Eq(x => x.UserId, userId);
        var update = Builders<UserAnalytics>.Update
            .Inc(x => x.VisitCount, 1)
            .Set(x => x.LastVisit, visitedAt)
            .Push(x => x.VisitDates, visitedAt);

        // Set FirstVisit if it's null
        var user = await GetUserAnalyticsAsync(userId);
        if (user?.FirstVisit == null)
        {
            update = update.Set(x => x.FirstVisit, visitedAt);
        }

        await _collection.UpdateOneAsync(filter, update);
    }

    public async Task<List<UserAnalytics>> GetTopUsersAsync(int limit)
    {
        return await _collection
            .Find(Builders<UserAnalytics>.Filter.Empty)
            .SortByDescending(x => x.VisitCount)
            .Limit(limit)
            .ToListAsync();
    }
}

