using System.Text.Json;
using MyApp.Model;
using StackExchange.Redis;

namespace Sandbox // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            await TrySimpleRedisClientCommands();
        }

        private static async Task TrySimpleRedisClientCommands()
        {
            await using ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("localhost");

            //Simple value
            string customerOneNameKey = "customer:1:name";

            IDatabase db = redis.GetDatabase();
            await db.StringSetAsync(customerOneNameKey, Guid.NewGuid().ToString());

            RedisValue value = await db.StringGetAsync(customerOneNameKey);
            Console.WriteLine($"Customer id - {value}.");

            //Set
            string setKey = "colours";

            await db.SetAddAsync(setKey, "green");
            await db.SetAddAsync(setKey, "blue");
            await db.SetAddAsync(setKey, "yellow");
            await db.SetAddAsync(setKey, "yellow");
            await db.SetAddAsync(setKey, "red");

            RedisValue[] colours = await db.SetMembersAsync(setKey);
            Console.WriteLine($"Colours: {string.Join(", ", colours)}.");

            //List
            string listKey = "to-do";

            // await db.ListRightPushAsync(listKey, "watch tv");
            // await db.ListRightPushAsync(listKey, "sleep");
            // await db.ListRightPushAsync(listKey, "write blog");
            // await db.ListRightPushAsync(listKey, "play with kids");
            // await db.ListRightPushAsync(listKey, "sleep");

            var toDos = await db.ListRangeAsync(listKey);
            Console.WriteLine($"TODOs: {string.Join(", ", toDos)}.");

            //Hash
            string hashKey = "customer:2";
            var hashEntries = new HashEntry[]
            {
                new("name", "John"),
                new("surname", "Smith"),
                new("company", "Redis"),
                new("age", "29"),
            };

            await db.HashSetAsync(hashKey, hashEntries);
            HashEntry[] customer = await db.HashGetAllAsync(hashKey);
            foreach (var hashEntry in customer)
            {
                Console.WriteLine($"{hashEntry.Name}-{hashEntry.Value}");
            }

            // Text.Json
            string personKey = "person:1";
            Person person = new Person()
            {
                Id = Guid.NewGuid(),
                IsFriend = true,
                Name = "Elvis Presley",
                Address = new Address() {City = "Memphis", Number = 1, Street = "Graceland"}
            };

            await db.StringSetAsync(personKey, JsonSerializer.Serialize(person, JsonSerializerOptions.Default));

            var personRedis = await db.StringGetAsync(personKey);
            var personDes = JsonSerializer.Deserialize<Person>(personRedis);
            Console.WriteLine($"Person - {personDes}");
            
            
            
        }
    }
}