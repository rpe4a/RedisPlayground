using StackExchange.Redis;

namespace Publisher;

public static class Program
{
    private const string RedisChannel = "messages";

    static async Task Main(string[] _)
    {
        await using ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("localhost");

        ISubscriber channel = redis.GetSubscriber();

        for (int i = 0; i < 10; i++)
        {
            await channel.PublishAsync(RedisChannel, $"message-{i}");
            await Task.Delay(Random.Shared.Next(100, 1000));
        }
    }
}