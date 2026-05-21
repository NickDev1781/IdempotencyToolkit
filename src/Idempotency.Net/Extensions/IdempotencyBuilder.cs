using Idempotency.Net.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Idempotency.Net.Extensions;

/// <summary>
/// Builder for configuring idempotency services.
/// </summary>
public sealed class IdempotencyBuilder
{
    /// <summary>
    /// Gets the service collection being configured.
    /// </summary>
    public IServiceCollection Services { get; }

    internal IdempotencyBuilder(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// Configures <see cref="IdempotencyOptions"/>.
    /// </summary>
    /// <param name="configure">A delegate to configure the options.</param>
    /// <returns>The builder for chaining.</returns>
    public IdempotencyBuilder Configure(Action<IdempotencyOptions> configure)
    {
        Services.Configure(configure);
        return this;
    }
}