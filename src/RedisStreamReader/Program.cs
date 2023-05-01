using StackExchange.Redis;

namespace RedisStreamReader;

public static class Program
{
    private const string EventStream = "event-stream";

    private static async Task Main(string[] _)
    {
        await using ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("localhost");

        StreamInfo streamInfo = await redis.GetDatabase().StreamInfoAsync(EventStream);

        var position = streamInfo.FirstEntry.Id;
        while (true)
        {
            var messages = await redis.GetDatabase().StreamReadAsync(EventStream, position, 1);

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
                await Task.Delay(5000);
            }
        }
    }
}