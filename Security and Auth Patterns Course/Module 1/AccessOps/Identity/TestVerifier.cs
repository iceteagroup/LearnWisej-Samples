#if DEBUG
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using AccessOps.Infrastructure;

namespace AccessOps.Identity
{
    /// <summary>
    /// TRAINING MOCK - NEVER SHIPS. A stand-in for a real credential check so Module 1 can
    /// establish a session principal without a password store or identity provider. It exists
    /// only in Debug builds (the whole file is inside <c>#if DEBUG</c>): a Release build has no
    /// verifier at all and refuses every sign-in.
    /// <para>
    /// It keeps no plain-text secrets: each demo account has the SHA-256 of its demo passphrase
    /// (listed in README.md, local use only), compared in constant time. A real verifier uses a
    /// slow password hash with a per-account salt (Module 4).
    /// </para>
    /// </summary>
    public sealed class TestVerifier : IAccountVerifier
    {
        // user name -> SHA-256 (hex) of the demo passphrase
        private static readonly Dictionary<string, string> DemoSecrets = new(StringComparer.Ordinal)
        {
            ["reader.a"] = Sha256Hex("reader-demo-17"),
            ["unassigned"] = Sha256Hex("unassigned-demo"),
            ["admin"] = Sha256Hex("admin-demo-21"),
        };

        private readonly AccountStore _accounts;

        public TestVerifier(AccountStore accounts)
        {
            _accounts = accounts;
        }

        /// <inheritdoc />
        public Account Verify(string userName, string secret)
        {
            userName = (userName ?? "").Trim();
            var expected = DemoSecrets.TryGetValue(userName, out var hash) ? hash : Sha256Hex("no-such-account");
            var matches = CryptographicOperations.FixedTimeEquals(
                Encoding.ASCII.GetBytes(expected),
                Encoding.ASCII.GetBytes(Sha256Hex(secret ?? "")));

            return matches && DemoSecrets.ContainsKey(userName)
                ? _accounts.FindByUserName(userName)
                : null;
        }

        private static string Sha256Hex(string text)
            => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text)));
    }
}
#endif
