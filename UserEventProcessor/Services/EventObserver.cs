using System.Collections.Concurrent;
using UserEventProcessor.Data;
using UserEventProcessor.Models;

namespace UserEventProcessor.Services;

public class EventObserver : IObserver<UserEvent>
{
    private readonly IDataStorage _dataStorage;
    private readonly ConcurrentDictionary<(int userId, string eventType), int> _eventCounts = new();
    
    private readonly DateTime? _startFilter;
    private readonly DateTime? _endFilter;

    public EventObserver(IDataStorage dataStorage, DateTime? startFilter = null, DateTime? endFilter = null)
    {
        _dataStorage = dataStorage;
        _startFilter = startFilter;
        _endFilter = endFilter;
    }

    public void OnNext(UserEvent value)
    {
        //фильтрация по времени
        if (_startFilter.HasValue && value.Timestamp < _startFilter.Value)
            return;

        if (_endFilter.HasValue && value.Timestamp > _endFilter.Value)
            return;
        
        if (string.IsNullOrWhiteSpace(value.EventType))
        {
            Console.WriteLine("Пропускаем событие без типа");
            return;
        }

        var key = (value.UserId, value.EventType.ToLowerInvariant());
        _eventCounts.AddOrUpdate(key, 1, (_, old) => old + 1);

        Console.WriteLine($"Обработано: Пользователь {key.Item1}, Событие '{key.Item2}'  Количество {_eventCounts[key]}");

        _dataStorage.SaveEventStats(_eventCounts);
    }

    public void OnError(Exception error)
    {
        Console.WriteLine($"Ошибка обзервера: {error.Message}");
    }

    public void OnCompleted()
    {
        Console.WriteLine("Стрим окончен.");
    }
}