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
        // DEMO ONLY: used when the environment supplies no key, and logged loudly. Never in production.
        private static readonly byte[] DemoKey = Encoding.UTF8.GetBytes("DEMO-ONLY-accessops-share-link-key-not-for-production");

        private readonly byte[] _key;
        private readonly Func<DateTimeOffset> _clock;

        public ShareLinkService(IConfiguration configuration, ILogger<ShareLinkService> log)
            : this(ReadKey(configuration, log), TimeSpan.FromMinutes(configuration.GetValue("ShareLinks:LifetimeMinutes", 60)), () => DateTimeOffset.UtcNow)
        {
        }

        public ShareLinkService(byte[] key, TimeSpan lifetime, Func<DateTimeOffset> clock)
        {
            if (key == null || key.Length < 32)
                throw new ArgumentException("The share-link signing key must be at least 32 bytes.", nameof(key));
            _key = key;
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

        public byte[] Sign(int documentId, long exp)
        {
            using var hmac = new HMACSHA256(_key);
            return hmac.ComputeHash(Encoding.ASCII.GetBytes(documentId.ToString(CultureInfo.InvariantCulture) + "|" + exp.ToString(CultureInfo.InvariantCulture)));
        }

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

            var expected = Sign(documentId, exp.Value);
            var signatureOk = presented.Length == expected.Length && CryptographicOperations.FixedTimeEquals(expected, presented);
            return signatureOk && _clock().ToUnixTimeSeconds() <= exp.Value;
        }

        private static byte[] ReadKey(IConfiguration configuration, ILogger log)
        {
            var configured = configuration["ShareLinks:SigningKey"];
            if (!string.IsNullOrEmpty(configured))
                return Convert.FromBase64String(configured);

            log.LogWarning("ShareLinks:SigningKey is not configured - using the DEMO share-link key. Supply ShareLinks__SigningKey from the environment before any real use.");
            return DemoKey;
        }
    }
}
