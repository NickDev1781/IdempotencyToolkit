using Idempotency.Net.Abstractions;
using Idempotency.Net.Extensions;
using Idempotency.Net.Redis.IntegrationTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Idempotency.Net.Redis.IntegrationTests;

public sealed class RedisIdempotentFilterConcurrencyTests : IClassFixture<RedisContainerFixture>
{
    private readonly RedisContainerFixture _fixture;

    public RedisIdempotentFilterConcurrencyTests(RedisContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ConcurrentRequests_WithSameKey_ExecuteLogicOnlyOnce()
    {
        // Arrange
        const int database = 6;
        var keyPrefix = $"concurrency:{Guid.NewGuid():N}:";
        var requestKey = $"request:{Guid.NewGuid():N}";
        await _fixture.FlushDatabaseAsync(database);

        await using var provider = BuildProvider(database, keyPrefix);
        var store = provider.GetRequiredService<IdempotencyStore>();
        var mux = provider.GetRequiredService<IConnectionMultiplexer>();

        int executionCount = 0;

        async Task HandleRequest()
        {
           
            var cached = await store.GetAsync(requestKey);
            if (cached is not null) return;

            
            var db = mux.GetDatabase();
            var lockKey = "lock:" + requestKey;
            var lockToken = Guid.NewGuid().ToString();
            bool acquired = await db.LockTakeAsync(lockKey, lockToken, TimeSpan.FromSeconds(10));
            if (!acquired)
            {
                await Task.Delay(100);
                return; 
            }

            try
            {

                cached = await store.GetAsync(requestKey);
                if (cached is not null) return;


                Interlocked.Increment(ref executionCount);
                var record = new IdempotencyRecord
                {
                    Key = requestKey,
                    StatusCode = 200,
                    ResponseBody = "{\"result\":\"ok\"}",
                    ContentType = "application/json; charset=utf-8",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(5)
                };
                await store.SaveAsync(record);
            }
            finally
            {
                await db.LockReleaseAsync(lockKey, lockToken);
            }
        }

        // Act 
        var tasks = Enumerable.Range(0, 10).Select(_ => Task.Run(HandleRequest));
        await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(1, executionCount); 
    }

    private ServiceProvider BuildProvider(int database, string keyPrefix)
    {
        var services = new ServiceCollection();
        services.AddIdempotency()
                .UseRedis(options =>
                {
                    options.ConnectionString = _fixture.ConnectionString;
                    options.Database = database;
                    options.KeyPrefix = keyPrefix;
                    options.AbortOnConnectFail = false;
                });
        return services.BuildServiceProvider();
    }
}