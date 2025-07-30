using System.Collections.Concurrent;
using UserEventProcessor.Data;

namespace UserEventProcessor.Tests;

// Заглушка для DataStorage, чтобы тестировать без файлов и БД
public class DataStorageMock : IDataStorage
{
    public ConcurrentDictionary<(int userId, string eventType), int>? LastSavedStats { get; private set; }

    public void SaveEventStats(ConcurrentDictionary<(int userId, string eventType), int> stats)
    {
        LastSavedStats = new ConcurrentDictionary<(int, string), int>(stats);
    }
}