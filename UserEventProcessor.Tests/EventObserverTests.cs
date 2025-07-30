using UserEventProcessor.Models;
using UserEventProcessor.Services;

namespace UserEventProcessor.Tests;

public class EventObserverTests
{
    [Fact]
    public void OnNext_ShouldCountEventsCorrectly()
    {
        // Arrange
        var dataStorage = new DataStorageMock();
        var observer = new EventObserver(dataStorage);

        var event1 = new UserEvent
        {
            UserId = 1,
            EventType = "click",
            Timestamp = DateTime.UtcNow
        };
        var event2 = new UserEvent
        {
            UserId = 1,
            EventType = "click",
            Timestamp = DateTime.UtcNow
        };
        var event3 = new UserEvent
        {
            UserId = 2,
            EventType = "hover",
            Timestamp = DateTime.UtcNow
        };

        // Act
        observer.OnNext(event1);
        observer.OnNext(event2);
        observer.OnNext(event3);

        // Assert
        Assert.NotNull(dataStorage.LastSavedStats);
        Assert.Equal(2, dataStorage.LastSavedStats![(1, "click")]);
        Assert.Equal(1, dataStorage.LastSavedStats![(2, "hover")]);
    }

    [Fact]
    public void OnNext_ShouldFilterByTimestamp()
    {
        // Arrange
        var dataStorage = new DataStorageMock();

        var startFilter = DateTime.UtcNow.AddMinutes(-10);
        var endFilter = DateTime.UtcNow.AddMinutes(10);

        var observer = new EventObserver(dataStorage, startFilter, endFilter);

        var insideEvent = new UserEvent
        {
            UserId = 1,
            EventType = "click",
            Timestamp = DateTime.UtcNow
        };

        var beforeEvent = new UserEvent
        {
            UserId = 1,
            EventType = "click",
            Timestamp = DateTime.UtcNow.AddHours(-1)
        };

        var afterEvent = new UserEvent
        {
            UserId = 1,
            EventType = "click",
            Timestamp = DateTime.UtcNow.AddHours(1)
        };

        // Act
        observer.OnNext(insideEvent);
        observer.OnNext(beforeEvent);
        observer.OnNext(afterEvent);

        // Assert
        Assert.NotNull(dataStorage.LastSavedStats);
        Assert.Equal(1, dataStorage.LastSavedStats![(1, "click")]);
    }
}