using Idempotency.Net.Abstractions;
using Idempotency.Net.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using StackExchange.Redis;

namespace Idempotency.Net.Redis;

/// <summary>
/// Extension methods for configuring Redis-based idempotency.
/// </summary>
public static class RedisIdempotencyBuilderExtensions
{
    /// <summary>
    /// Configures the idempotency library to use Redis as the storage and locking provider.
    /// </summary>
    /// <param name="builder">The <see cref="IdempotencyBuilder"/>.</param>
    /// <param name="configure">Optional delegate to configure <see cref="RedisIdempotencyOptions"/>.</param>
    /// <returns>The builder for chaining.</returns>
    public static IdempotencyBuilder UseRedis(
        this IdempotencyBuilder builder,
        Action<RedisIdempotencyOptions>? configure = null)
    {
        builder.Services.AddOptions<RedisIdempotencyOptions>();

        if (configure is not null)
            builder.Services.Configure(configure);

        builder.Services.TryAddSingleton<IConnectionMultiplexer>(static serviceProvider =>
        {
            var options = serviceProvider
                .GetRequiredService<Microsoft.Extensions.Options.IOptions<RedisIdempotencyOptions>>()
                .Value;

            string? configuration = options.ConnectionString;
            if (string.IsNullOrWhiteSpace(configuration))
                configuration = options.Configuration;

            if (string.IsNullOrWhiteSpace(configuration))
                throw new InvalidOperationException("Redis configuration is required.");

            var redisOptions = ConfigurationOptions.Parse(configuration, true);
            redisOptions.ConnectTimeout = (int)options.ConnectTimeout.TotalMilliseconds;
            redisOptions.SyncTimeout = (int)options.SyncTimeout.TotalMilliseconds;
            redisOptions.AbortOnConnectFail = options.AbortOnConnectFail;

            return ConnectionMultiplexer.Connect(redisOptions);
        });

        builder.Services.AddScoped<RedisIdempotencyStore>();
        builder.Services.AddScoped<IdempotencyStore>(serviceProvider =>
            serviceProvider.GetRequiredService<RedisIdempotencyStore>());
        builder.Services.TryAddScoped<IIdempotencyLock, RedisIdempotencyLock>();

        return builder;
    }
}