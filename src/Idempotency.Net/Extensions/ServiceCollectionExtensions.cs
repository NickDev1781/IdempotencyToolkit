using Idempotency.Net.Abstractions;
using Idempotency.Net.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Idempotency.Net.Extensions;

/// <summary>
/// Extension methods for registering idempotency services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the core idempotency services in the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional action to configure <see cref="IdempotencyOptions"/>.</param>
    /// <returns>An <see cref="IdempotencyBuilder"/> for further storage configuration.</returns>
    public static IdempotencyBuilder AddIdempotency(
        this IServiceCollection services,
        Action<IdempotencyOptions>? configure = null)
    {
        services.AddOptions<IdempotencyOptions>();

        if (configure != null)
            services.Configure(configure);
        return new IdempotencyBuilder(services);
    }
}