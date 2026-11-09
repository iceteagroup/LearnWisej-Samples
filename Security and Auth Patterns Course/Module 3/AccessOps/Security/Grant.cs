using System;

namespace AccessOps.Security
{
    /// <summary>
    /// A revocable, expiring server-side record of what one account may do in one tenant. Every
    /// protected operation reads the grant's current state from <see cref="GrantStore"/>; a copy
    /// held by a screen or a job is never trusted.
    /// </summary>
    public sealed class Grant
    {
        /// <summary>Grant ID (the value of the principal's "grant" claim).</summary>
        public string Id { get; init; }

        /// <summary>The account the grant belongs to.</summary>
        public string AccountId { get; init; }

        /// <summary>The one tenant the grant covers.</summary>
        public string Tenant { get; init; }

        /// <summary>The grant's role name ("Reader", "Admin"). Display only.</summary>
        public string Role { get; init; }

        /// <summary>May read documents in <see cref="Tenant"/>.</summary>
        public bool CanRead { get; init; }

        /// <summary>May revoke and restore grants in the admin console.</summary>
        public bool CanAdminister { get; init; }

        /// <summary>Revoked by an administrator.</summary>
        public bool Revoked { get; init; }

        /// <summary>The grant stops working at this instant.</summary>
        public DateTimeOffset ExpiresAt { get; init; }
    }

    /// <summary>The clock the decisions read, so tests can move time.</summary>
    public interface IClock
    {
        /// <summary>The current instant.</summary>
        DateTimeOffset Now { get; }
    }

    /// <summary>The system clock (UTC).</summary>
    public sealed class SystemClock : IClock
    {
        /// <inheritdoc />
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }
}
