using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AccessOps.Services
{
    /// <summary>
    /// Signed, expiring share links: a document id, an expiry (Unix seconds) and an HMAC-SHA256 over
    /// both with a key only the server holds. Editing the id or pushing the expiry out changes the
    /// signature; the endpoint recomputes it and compares in fixed time before it loads anything.
    /// </summary>
    public sealed class ShareLinkService
    {
        private readonly byte[] _key;
        private readonly byte[] _previousKey;
        private readonly Func<DateTimeOffset> _clock;

        public ShareLinkService(IConfiguration configuration, ILogger<ShareLinkService> log)
            : this(ReadKey(configuration, "ShareLinks:SigningKey", required: true),
                   TimeSpan.FromMinutes(configuration.GetValue("ShareLinks:LifetimeMinutes", 60)),
                   () => DateTimeOffset.UtcNow,
                   ReadKey(configuration, "ShareLinks:PreviousSigningKey", required: false))
        {
        }

        /// <param name="previousKey">
        /// During a rotation, the key being retired: links it signed keep working until they expire,
        /// new links are signed with <paramref name="key"/> only. Null outside a rotation.
        /// </param>
        public ShareLinkService(byte[] key, TimeSpan lifetime, Func<DateTimeOffset> clock, byte[] previousKey = null)
        {
            if (key == null || key.Length < 32)
                throw new ArgumentException("The share-link signing key must be at least 32 bytes.", nameof(key));
            if (previousKey != null && previousKey.Length < 32)
                throw new ArgumentException("The previous share-link signing key must be at least 32 bytes.", nameof(previousKey));
            _key = key;
            _previousKey = previousKey;
            this.Lifetime = lifetime;
            _clock = clock;
        }

        public TimeSpan Lifetime { get; }

        /// <summary>The relative URL of a share link for the document, valid for <see cref="Lifetime"/>.</summary>
        public string Issue(int documentId, out DateTimeOffset expires)
        {
            expires = _clock().Add(this.Lifetime);
            var exp = expires.ToUnixTimeSeconds();
            return $"share/{documentId.ToString(CultureInfo.InvariantCulture)}?exp={exp.ToString(CultureInfo.InvariantCulture)}&sig={WebEncoders.Base64UrlEncode(Sign(documentId, exp))}";
        }

        public byte[] Sign(int documentId, long exp) => Sign(_key, documentId, exp);

        /// <summary>True only for an untampered, unexpired link. Every other case looks the same to the caller.</summary>
        public bool IsValid(int documentId, long? exp, string sig)
        {
            if (exp == null || string.IsNullOrEmpty(sig))
                return false;

            byte[] presented;
            try
            {
                presented = WebEncoders.Base64UrlDecode(sig);
            }
            catch (FormatException)
            {
                return false;
            }

            var signatureOk = Matches(_key, documentId, exp.Value, presented)
                || (_previousKey != null && Matches(_previousKey, documentId, exp.Value, presented));
            return signatureOk && _clock().ToUnixTimeSeconds() <= exp.Value;
        }

        private static byte[] Sign(byte[] key, int documentId, long exp)
        {
            using var hmac = new HMACSHA256(key);
            return hmac.ComputeHash(Encoding.ASCII.GetBytes(documentId.ToString(CultureInfo.InvariantCulture) + "|" + exp.ToString(CultureInfo.InvariantCulture)));
        }

        private static bool Matches(byte[] key, int documentId, long exp, byte[] presented)
        {
            var expected = Sign(key, documentId, exp);
            return presented.Length == expected.Length && CryptographicOperations.FixedTimeEquals(expected, presented);
        }

        // Module 6: no demo fallback any more - the key comes from user secrets (Development) or the
        // environment (ShareLinks__SigningKey), or the application does not start.
        // ShareLinks:PreviousSigningKey is set only while a key is being rotated (docs/SecretRotation.md).
        private static byte[] ReadKey(IConfiguration configuration, string name, bool required)
        {
            var configured = configuration[name];
            if (string.IsNullOrEmpty(configured))
            {
                if (!required)
                    return null;
                throw new InvalidOperationException($"{name} is not configured. Supply it with 'dotnet user-secrets' (Development) or the {name.Replace(":", "__")} environment variable.");
            }
            return Convert.FromBase64String(configured);
        }
    }
}
