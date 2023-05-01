using StackExchange.Redis;

namespace Subscriber;

public static class Program
{
    private const string RedisChannel = "messages";

    static async Task Main(string[] _)
    {
        await using ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("localhost");

        ISubscriber channel = redis.GetSubscriber();

        ChannelMessageQueue queue = await channel.SubscribeAsync(RedisChannel);

        await foreach (ChannelMessage message in queue)
        {
            Console.WriteLine(message.Message);
            await Task.Delay(Random.Shared.Next(500, 2000));
        }

        Console.WriteLine("Finish!");
    }
}