using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Wisej.Ext.WebAuthn;

namespace AccessOps.Tests
{
    /// <summary>
    /// A software authenticator for the tests: an ES256 (P-256) or RS256 key pair whose public half
    /// is stored the way AccessOps stores a real one, and which signs assertions exactly as the
    /// WebAuthn spec says (authenticatorData ‖ SHA-256(clientDataJSON)). The responses are built
    /// with the extension's own public types (<see cref="CredentialsResponse"/>, all setters public in 4.1.7),
    /// so <see cref="WebAuthn.Validate"/> checks a real signature.
    /// </summary>
    public sealed class SyntheticAuthenticator : IDisposable
    {
        private readonly ECDsa _ec;
        private readonly RSA _rsa;

        public SyntheticAuthenticator(COSEAlgorithmIdentifier algorithm = COSEAlgorithmIdentifier.ES256)
        {
            this.Algorithm = algorithm;
            this.CredentialId = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            if (algorithm == COSEAlgorithmIdentifier.RS256)
                _rsa = RSA.Create(2048);
            else
                _ec = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        }

        public COSEAlgorithmIdentifier Algorithm { get; }

        public string CredentialId { get; }

        /// <summary>The COSE public key as the browser hands it to the extension (labels as strings, coordinates base64).</summary>
        public string PublicKeyJson
        {
            get
            {
                object data;
                if (_rsa != null)
                {
                    var p = _rsa.ExportParameters(false);
                    data = new System.Collections.Generic.Dictionary<string, object> { ["1"] = 3, ["3"] = -257, ["-1"] = Convert.ToBase64String(p.Modulus), ["-2"] = Convert.ToBase64String(p.Exponent) };
                }
                else
                {
                    var p = _ec.ExportParameters(false);
                    data = new System.Collections.Generic.Dictionary<string, object> { ["1"] = 2, ["3"] = -7, ["-1"] = 1, ["-2"] = Convert.ToBase64String(p.Q.X), ["-3"] = Convert.ToBase64String(p.Q.Y) };
                }
                return JsonSerializer.Serialize(new { CredentialID = this.CredentialId, Data = data });
            }
        }

        /// <summary>The public key as the extension's type.</summary>
        public PublicKey PublicKey => new PublicKey(this.PublicKeyJson);

        /// <summary>A registration response (what CreateAsync returns): public key and client data, no signature.</summary>
        public CredentialsResponse Create(byte[] challenge, string origin, string type = "webauthn.create")
            => new CredentialsResponse
            {
                AuthenticatorData = new AuthenticatorData { PublicKey = this.PublicKey },
                ClientData = new ClientData { Challenge = challenge, Origin = origin, Type = type },
                SignatureCounter = 0
            };

        /// <summary>An authentication response (what GetAsync returns), signed with this key.</summary>
        public CredentialsResponse Get(byte[] challenge, string origin, string userHandle, uint counter = 0,
            string rpId = "localhost", bool userPresent = true, bool userVerified = true, bool backupEligible = false, string type = "webauthn.get")
        {
            var clientDataJson = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
            {
                type,
                challenge = Convert.ToBase64String(challenge).TrimEnd('=').Replace('+', '-').Replace('/', '_'),
                origin,
                crossOrigin = false
            }));

            var authData = new byte[37];
            SHA256.HashData(Encoding.UTF8.GetBytes(rpId)).CopyTo(authData, 0);
            authData[32] = (byte)((userPresent ? 0x01 : 0) | (userVerified ? 0x04 : 0) | (backupEligible ? 0x08 : 0));
            authData[33] = (byte)(counter >> 24);
            authData[34] = (byte)(counter >> 16);
            authData[35] = (byte)(counter >> 8);
            authData[36] = (byte)counter;

            var signed = new byte[authData.Length + 32];
            authData.CopyTo(signed, 0);
            SHA256.HashData(clientDataJson).CopyTo(signed, authData.Length);

            return new CredentialsResponse
            {
                AuthenticatorData = new AuthenticatorData
                {
                    Base64 = Convert.ToBase64String(authData),
                    RPIDHash = authData[..32],
                    UserPresent = userPresent,
                    UserVerified = userVerified,
                    BackupEligibility = backupEligible
                },
                ClientData = new ClientData { Base64 = Convert.ToBase64String(clientDataJson), Challenge = challenge, Origin = origin, Type = type },
                Signature = Sign(signed),
                UserHandle = userHandle,
                SignatureCounter = (int)counter
            };
        }

        private byte[] Sign(byte[] data)
        {
            if (_rsa != null)
                return _rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // plain DER, as an authenticator sends it - including the ~1 in 128 signatures whose r or s
            // is shorter than 32 bytes (see Es256Signature)
            return _ec.SignData(data, HashAlgorithmName.SHA256, DSASignatureFormat.Rfc3279DerSequence);
        }

        /// <summary>Signs arbitrary data (for the signature-encoding test).</summary>
        public byte[] SignRaw(byte[] data) => _ec.SignData(data, HashAlgorithmName.SHA256, DSASignatureFormat.Rfc3279DerSequence);

        public void Dispose()
        {
            _ec?.Dispose();
            _rsa?.Dispose();
        }
    }
}
