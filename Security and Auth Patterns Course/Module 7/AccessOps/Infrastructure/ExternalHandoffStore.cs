using System;
using System.Text.Json;
using AccessOps.Identity;
using AccessOps.Security;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// Where <c>OnTokenValidated</c> parks a validated <see cref="ExternalIdentity"/> until the
    /// Wisej.NET page load picks it up. The browser only holds a random 256-bit handle in the
    /// HttpOnly <see cref="CookieName"/> cookie; the handle redeems once and for two minutes.
    /// Module 7 moved it from process memory to the shared store (<see cref="HandoffTable"/>): a
    /// restarted process or a second node resumes the hand-off once, and a repeat fails safely.
    /// </summary>
    public sealed class ExternalHandoffStore
    {
        /// <summary>The hand-off cookie.</summary>
        public const string CookieName = "AccessOps.Handoff";

        /// <summary>The kind of row in <c>signin_handoffs</c>.</summary>
        public const string Kind = "external";

        /// <summary>How long a parked identity waits for its page load.</summary>
        public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(2);

        private readonly HandoffTable _table;

        /// <summary>Creates the store over the shared database.</summary>
        public ExternalHandoffStore(AccessOpsDatabase db, IClock clock)
        {
            _table = new HandoffTable(db, clock);
        }

        /// <summary>Parks <paramref name="identity"/> and returns its new handle.</summary>
        public string Park(ExternalIdentity identity)
            => _table.Park(Kind, JsonSerializer.Serialize(new Payload(identity.Issuer, identity.Subject)), Lifetime);

        /// <summary>Redeems a handle once. False for an unknown, expired or already redeemed handle.</summary>
        public bool TryRedeem(string handle, out ExternalIdentity identity)
        {
            identity = null;
            if (!_table.TryRedeem(Kind, handle, out var json))
                return false;
            var payload = JsonSerializer.Deserialize<Payload>(json);
            identity = new ExternalIdentity(payload.Issuer, payload.Subject);
            return true;
        }

        private sealed record Payload(string Issuer, string Subject);
    }
}
