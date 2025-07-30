namespace UserEventProcessor.Models;

public class UserEvent
{
    public int UserId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string>? Data { get; set; }
}