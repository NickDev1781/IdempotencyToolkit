namespace Idempotency.Net.Abstractions;

/// <summary>
/// Abstraction for storing and retrieving idempotency records.
/// </summary>
public interface IdempotencyStore
{
    /// <summary>
    /// Retrieves an idempotency record by key.
    /// </summary>
    /// <param name="key">The unique idempotency key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stored record, or null if not found or expired.</returns>
    Task<IdempotencyRecord?> GetAsync(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves an idempotency record.
    /// </summary>
    /// <param name="record">The idempotency record to save.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SaveAsync(
        IdempotencyRecord record,
        CancellationToken cancellationToken = default);
}