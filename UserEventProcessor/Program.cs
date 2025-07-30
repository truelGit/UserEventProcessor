using UserEventProcessor.Data;
using UserEventProcessor.Services;

namespace UserEventProcessor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Запуск UserEventProcessor");

            var observable = new EventObservable();
            var observer = new EventObserver(new DataStorage());
            observable.Subscribe(observer);

            var kafkaConsumer = new KafkaConsumer(observable);

            using var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Запрошена отмена");
                cts.Cancel();
                e.Cancel = true;
            };

            await kafkaConsumer.StartAsync(cts.Token);

            Console.WriteLine("UserEventProcessor остановлен");
        }
    }
}