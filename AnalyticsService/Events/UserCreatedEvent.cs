namespace AnalyticsService.Events;

public class UserCreatedEvent
{
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

