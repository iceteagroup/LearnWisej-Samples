using System;

namespace AccessOps.Security
{
    /// <summary>
    /// A login grant: a revocable, expiring server-side record of what one account may do in one
    /// tenant, created by a successful sign-in. Every protected operation reads the grant's
    /// current state from <see cref="GrantStore"/>; a copy held by a screen, a claim or a job is
    /// never trusted. Revoking it is what sign-out means.
    /// </summary>
    public sealed class Grant
    {
        /// <summary>Opaque random grant ID ("G-" + 32 hex characters, 128 random bits).</summary>
        public string Id { get; init; }

        /// <summary>The account the grant belongs to.</summary>
        public string AccountId { get; init; }

        /// <summary>The one tenant the grant covers.</summary>
        public string Tenant { get; init; }

        /// <summary>The grant's role name ("Reader"). Display only.</summary>
        public string Role { get; init; }

        /// <summary>May read documents in <see cref="Tenant"/>.</summary>
        public bool CanRead { get; init; }

        /// <summary>When the sign-in created it.</summary>
        public DateTimeOffset CreatedAt { get; init; }

        /// <summary>The grant stops working at this instant.</summary>
        public DateTimeOffset ExpiresAt { get; init; }

        /// <summary>Revoked by a sign-out (or an administrator).</summary>
        public bool Revoked { get; init; }

        /// <summary>The grant ID as screens and traces show it ("G-7f3a…").</summary>
        public string Display => Shorten(this.Id);

        /// <summary>Shortens a grant ID to "G-" plus its first four characters.</summary>
        public static string Shorten(string grantId)
            => string.IsNullOrEmpty(grantId) ? "none" : grantId.Length <= 6 ? grantId : grantId[..6] + "…";
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
