using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AnalyticsService.Models;

public class UserAnalytics
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("visitCount")]
    public int VisitCount { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("firstVisit")]
    public DateTime? FirstVisit { get; set; }

    [BsonElement("lastVisit")]
    public DateTime? LastVisit { get; set; }

    [BsonElement("visitDates")]
    public List<DateTime> VisitDates { get; set; } = new();
}

