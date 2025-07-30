using UserEventProcessor.Models;

namespace UserEventProcessor.Services;

public class EventObservable : IObservable<UserEvent>
{
    private readonly List<IObserver<UserEvent>> _observers = new();

    public IDisposable Subscribe(IObserver<UserEvent> observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);

        return new Unsubscriber(_observers, observer);
    }

    public void Publish(UserEvent userEvent)
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(userEvent);
        }
    }

    private class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<UserEvent>> _observers;
        private readonly IObserver<UserEvent> _observer;

        public Unsubscriber(List<IObserver<UserEvent>> observers, IObserver<UserEvent> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}