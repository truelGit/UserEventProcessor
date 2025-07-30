using Confluent.Kafka;
using System.Text.Json;
using UserEventProcessor.Models;

namespace UserEventProcessor.Services;

public class KafkaConsumer
{
    private readonly IObservable<UserEvent> _observable;
    private readonly string _bootstrapServers;
    private readonly string _topic;
    private readonly string _groupId;

    public KafkaConsumer(IObservable<UserEvent> observable)
    {
        _observable = observable;

        // Получение конфигурации из переменных окружения
        _bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS")
                             ?? throw new InvalidOperationException("KAFKA_BOOTSTRAP_SERVERS is not set");
        _topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC") ?? "user-events";
        _groupId = Environment.GetEnvironmentVariable("KAFKA_GROUP_ID") ?? "user-event-processor";
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_topic);

        Console.WriteLine($"Kafka consumer запущен. Ожидание сообщений из топика '{_topic}'");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(cancellationToken);

                    var json = result.Message.Value;
                    var userEvent = JsonSerializer.Deserialize<UserEvent>(json);

                    if (userEvent != null)
                    {
                        if (_observable is EventObservable observableImpl)
                        {
                            observableImpl.Publish(userEvent);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Не удалось десериализовать сообщение.");
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Ошибка при чтении из Kafka: {ex.Error.Reason}");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Ошибка парсинга JSON: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Работа Kafka-консьюмера отменена.");
        }
        finally
        {
            consumer.Close();
        }
    }
}