using Redis.OM;
using WebApiRedisOM.Models;

namespace WebApiRedisOM.Services;

public class IndexCreationService : IHostedService
{
    private readonly RedisConnectionProvider _provider;

    public IndexCreationService(RedisConnectionProvider provider)
    {
        _provider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _provider.Connection.CreateIndexAsync(typeof(Person));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}