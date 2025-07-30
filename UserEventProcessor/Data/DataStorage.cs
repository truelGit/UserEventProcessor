using System.Text.Json;
using System.Collections.Concurrent;

namespace UserEventProcessor.Data;

public interface IDataStorage
{
    void SaveEventStats(ConcurrentDictionary<(int userId, string eventType), int> stats);
}

public class DataStorage : IDataStorage
{
    private const string FilePath = "user_event_stats.json";

    public void SaveEventStats(ConcurrentDictionary<(int userId, string eventType), int> stats)
    {
        try
        {
            var result = stats.Select(kv => new UserEventStat
            {
                UserId = kv.Key.userId,
                EventType = kv.Key.eventType,
                Count = kv.Value
            }).ToList();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(result, options);
            File.WriteAllText(FilePath, json);

            Console.WriteLine($"Stats saved to {FilePath} ({result.Count} records)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save stats: {ex.Message}");
        }
    }
}

public class UserEventStat
{
    public int UserId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public int Count { get; set; }
}