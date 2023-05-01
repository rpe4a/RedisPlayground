using StackExchange.Redis;

namespace RedisStreamConsumerGroup1Reader;

public static class Program
{
    private const string EventStream = "event-stream";
    private const string EventsConsumerGroup = "events-consumer-group";
    private const string EventConsumer = "event-consumer-1";

    private static async Task Main(string[] _)
    {
        await using ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("localhost");
        
        // await redis.GetDatabase().StreamCreateConsumerGroupAsync(EventStream, EventsConsumerGroup, "0-0");

        var r = redis.GetDatabase().StreamPending(EventStream, EventsConsumerGroup);
        
        var position = "0-0";
        while (true)
        {
            var messages = await redis.GetDatabase().StreamReadGroupAsync(EventStream, EventsConsumerGroup, EventConsumer, position, 1);

            if (messages.Length != 0)
            {
                foreach (StreamEntry streamEntry in messages)
                {
                    foreach (var streamEntryValue in streamEntry.Values)
                    {
                        Console.WriteLine($"{streamEntryValue.Name}-{streamEntryValue.Value}");
                    }
                }

                position = messages.First().Id;
            }
            else
            {
                Console.WriteLine("Have nothing to handle. Let wait.");
                await Task.Delay(1000);
                position = StreamPosition.NewMessages;
            }
        }
    }
}