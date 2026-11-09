using System;
using System.Security.Cryptography;
using System.Text;

namespace AccessOps.Identity
{
    /// <summary>
    /// Password hashing with a framework derivation: PBKDF2-HMAC-SHA256 through
    /// <c>Rfc2898DeriveBytes.Pbkdf2</c>, a 16-byte random salt per account and a constant-time
    /// comparison. The stored form is <c>pbkdf2-sha256$iterations$salt$hash</c> (base64).
    /// </summary>
    public sealed class PasswordHasher
    {
        private const string Scheme = "pbkdf2-sha256";
        private readonly int _iterations;

        /// <summary>
        /// <paramref name="iterations"/> defaults to 210,000 (the OWASP figure for
        /// PBKDF2-HMAC-SHA256); tests pass a small number to stay fast.
        /// </summary>
        public PasswordHasher(int iterations = 210_000)
        {
            _iterations = iterations;
            // A hash of a random password nobody knows: unknown user names are verified against it
            // so "no such user" costs the same time as "wrong password".
            this.DummyHash = Hash(Convert.ToBase64String(RandomNumberGenerator.GetBytes(24)));
        }

        /// <summary>The dummy credential checked for unknown user names.</summary>
        public string DummyHash { get; }

        /// <summary>Hashes <paramref name="password"/> with a new random salt.</summary>
        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var hash = Derive(password, salt, _iterations);
            return $"{Scheme}${_iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        /// <summary>True when <paramref name="password"/> matches <paramref name="stored"/>.</summary>
        public bool Verify(string password, string stored)
        {
            var parts = (stored ?? "").Split('$');
            if (parts.Length != 4 || parts[0] != Scheme || !int.TryParse(parts[1], out var iterations))
                return false;

            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Derive(password, salt, iterations);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }

        private static byte[] Derive(string password, byte[] salt, int iterations)
            => Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password ?? ""), salt, iterations, HashAlgorithmName.SHA256, 32);
    }
}
