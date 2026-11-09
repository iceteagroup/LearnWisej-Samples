using System;
using System.Text.Json;
using AccessOps.Identity;
using AccessOps.Security;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// Single-use, short-lived handles the Wisej.NET session creates so the browser can ask the
    /// HTTPS endpoint <c>/auth/cookie</c> for the host authentication cookie. A handle names a
    /// verified sign-in on the server; it is not a credential the browser could reuse: it works
    /// once and for 60 seconds. Module 7: in the shared store (<see cref="HandoffTable"/>), so the
    /// endpoint can run on any node.
    /// </summary>
    public sealed class CookieHandoffStore
    {
        /// <summary>The kind of row in <c>signin_handoffs</c>.</summary>
        public const string Kind = "cookie";

        /// <summary>How long a handle can be redeemed.</summary>
        public static readonly TimeSpan Lifetime = TimeSpan.FromSeconds(60);

        private readonly HandoffTable _table;

        /// <summary>Creates the store over the shared database.</summary>
        public CookieHandoffStore(AccessOpsDatabase db, IClock clock)
        {
            _table = new HandoffTable(db, clock);
        }

        /// <summary>Parks <paramref name="signIn"/> and returns a new random handle (256 bits).</summary>
        public string Create(VerifiedSignIn signIn)
            => _table.Park(Kind, JsonSerializer.Serialize(signIn), Lifetime);

        /// <summary>Redeems a handle once. False for an unknown, expired or already redeemed handle.</summary>
        public bool TryRedeem(string handle, out VerifiedSignIn signIn)
        {
            signIn = null;
            if (!_table.TryRedeem(Kind, handle, out var json))
                return false;
            signIn = JsonSerializer.Deserialize<VerifiedSignIn>(json);
            return true;
        }
    }
}
