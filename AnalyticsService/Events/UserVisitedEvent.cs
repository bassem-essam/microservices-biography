namespace AnalyticsService.Events;

public class UserVisitedEvent
{
    public string UserId { get; set; } = string.Empty;
    public DateTime VisitedAt { get; set; }
}

