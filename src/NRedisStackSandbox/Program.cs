using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

namespace NRedisStackSandbox;

public static class Program
{
    static async Task Main(string[] _)
    {
        await using ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("localhost");
        var db = redis.GetDatabase();
        //JSON & Search
        var ft = db.FT();
        IJsonCommands json = db.JSON();
        json.Set("product:15970", "$", new
        {
            id = 15970,
            gender = "Men",
            season = new[] {"Fall", "Winter"},
            description = "Turtle Check Men Navy Blue Shirt",
            price = 34.95,
            city = "Boston",
            coords = "-71.057083, 42.361145"
        });
        json.Set("product:59263", "$", new
        {
            id = 59263,
            gender = "Women",
            season = new[] {"Fall", "Winter", "Spring", "Summer"},
            description = "Titan Women Silver Watch",
            price = 129.99,
            city = "Dallas",
            coords = "-96.808891, 32.779167"
        });
        json.Set("product:46885", "$", new
        {
            id = 46885,
            gender = "Boys",
            season = new[] {"Fall"},
            description = "Ben 10 Boys Navy Blue Slippers",
            price = 45.99,
            city = "Denver",
            coords = "-104.991531, 39.742043"
        });

        try
        {
            ft.DropIndex("idx1");
        }
        catch
        {
        }

        ft.Create("idx1", new FTCreateParams().On(IndexDataType.JSON)
                .Prefix("product:"),
            new Schema().AddNumericField(new FieldName("$.id", "id"))
                .AddTagField(new FieldName("$.gender", "gender"))
                .AddTagField(new FieldName("$.season.*", "season"))
                .AddTextField(new FieldName("$.description", "description"))
                .AddNumericField(new FieldName("$.price", "price"))
                .AddTextField(new FieldName("$.city", "city"))
                .AddGeoField(new FieldName("$.coords", "coords")));

        foreach (var doc in ft.Search("idx1", new Query("*")).ToJson())
        {
            Console.WriteLine(doc);
        }

        foreach (var doc in ft.Search("idx1", new Query("@price:[40, 100] @description:Blue"))
                     .ToJson())
        {
            Console.WriteLine(doc);
        }

        //Graph
        var graph = db.GRAPH();
        var graphName = "road";
        try
        {
            graph.Delete(graphName);
        }
        catch
        {
        }

        graph.Query("road",
            "CREATE (a:City{name:'A'}), (b:City{name:'B'}), (c:City{name:'C'}), (d:City{name:'D'}), (e:City{name:'E'}), (f:City{name:'F'}), (g:City{name:'G'}), (a)-[:Road{time:4, dist:3}]->(b), (a)-[:Road{time:3, dist:8}]->(c), (a)-[:Road{time:4, dist:2}]->(d), (b)-[:Road{time:5, dist:7}]->(e), (b)-[:Road{time:5, dist:5}]->(d), (d)-[:Road{time:4, dist:5}]->(e), (c)-[:Road{time:3, dist:6}]->(f), (d)-[:Road{time:1, dist:4}]->(c), (d)-[:Road{time:2, dist:12}]->(f), (e)-[:Road{time:5, dist:5}]->(g), (f)-[:Road{time:4, dist:2}]->(g)");
    }
}