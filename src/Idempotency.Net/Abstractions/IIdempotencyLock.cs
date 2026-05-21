using System;
using System.Collections.Generic;
using System.Text;

namespace Idempotency.Net.Abstractions
{
    /// <summary>
    /// Provides distributed lock acquisition and release for idempotency keys.
    /// </summary>
    public interface IIdempotencyLock
    {
        /// <summary>
        /// Attempts to acquire a distributed lock for the specified key.
        /// </summary>
        /// <param name="key">The unique idempotency key.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the lock was acquired; otherwise false.</returns>
        Task<bool> AcquireAsync(string key, CancellationToken cancellationToken = default);

        //// <summary>
        /// Releases the distributed lock associated with the specified key.
        /// </summary>
        /// <param name="key">The unique idempotency key.</param>
        Task ReleaseAsync(string key);
    }
}
