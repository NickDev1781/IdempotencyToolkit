namespace Idempotency.Net.Abstractions;

/// <summary>
/// Represents a cached idempotency response record.
/// </summary>
public sealed class IdempotencyRecord
{
    /// <summary>
    /// The unique idempotency key.
    /// </summary>
    public string Key { get; init; } = default!;

    /// <summary>
    /// The HTTP status code of the original response.
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// The cached response body (if any).
    /// </summary>
    public string? ResponseBody { get; init; }

    /// <summary>
    /// The content type of the cached response body.
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// The time when the record was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// The time after which the record is considered expired.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; init; }
}