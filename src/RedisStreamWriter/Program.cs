using StackExchange.Redis;

namespace RedisStreamWriter;

public static class Program
{
    private const string EventStream = "event-stream";

    static async Task Main(string[] _)
    {
        await using ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("localhost");

        for (int i = 0; i < 1000; i++)
        {
            await redis.GetDatabase().StreamAddAsync(EventStream, "post", $"message-{i}");

            await Task.Delay(Random.Shared.Next(100, 1000));
        }
    }
}