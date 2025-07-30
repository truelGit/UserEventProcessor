# UserEventProcessor

## Описание

UserEventProcessor — это backend-сервис на C# (.NET 9), который:

- Подписывается на события пользователей из Apache Kafka (топик `user-events`).
- Обрабатывает эти события с использованием паттерна Observer (`IObservable` / `IObserver`).
- Считает статистику событий для каждого пользователя.
- Сохраняет агрегированные данные в JSON-файл (`user_event_stats.json`).

---

## Особенности

- Асинхронное чтение сообщений из Kafka с помощью библиотеки Confluent.Kafka.
- Реализация паттерна Observer через `EventObservable` и `EventObserver`.
- Фильтрация событий по временному диапазону.
- Легко расширяемая архитектура для сохранения данных (JSON-файл, PostgreSQL и т.д.).
- Конфигурация через переменные окружения.

---

## Требования

- .NET 9 SDK
- Docker и Docker Compose (для запуска Kafka и Zookeeper)
- Kafka (локально через Docker или удалённый)
- (Опционально) PostgreSQL, если расширять хранение в базу данных

---

## Переменные окружения

- `KAFKA_BOOTSTRAP_SERVERS` — адрес Kafka (например, `localhost:9092`)
- `KAFKA_TOPIC` — имя топика (по умолчанию `user-events`)
- `KAFKA_GROUP_ID` — ID группы потребителей Kafka
- (если используешь PostgreSQL) `POSTGRES_CONNECTION_STRING`

---

## Быстрый старт

1. Запустите Kafka и Zookeeper через Docker Compose:

```bash
docker-compose up -d
```
2. Создайте топик user-events:

```bash
docker exec kafka /usr/bin/kafka-topics --create --topic user-events --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1
```
3. Настройте переменные окружения.

4. Запустите сервис:
```bash
dotnet run --project UserEventProcessor
```
5. Отправляйте события в Kafka.
```json
Пример события
{
    "userId": 123,
    "eventType": "click",
    "timestamp": "2025-07-29T12:00:00Z",
    "data": {
    "buttonId": "submit"
    }
}
```
Тестирование
```bash
dotnet test
```
