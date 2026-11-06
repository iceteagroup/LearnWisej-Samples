using System;
using System.Security.Cryptography;
using System.Text;

namespace AccessOps.Services
{
    /// <summary>
    /// What AccessOps stores instead of a password: the algorithm, the iteration count, a random
    /// per-employee salt and the derived bytes. Nothing here can be turned back into a password.
    /// </summary>
    public sealed class StoredCredential
    {
        public int EmployeeId { get; init; }
        public string Algorithm { get; init; } = PasswordService.AlgorithmName;
        public int Iterations { get; init; }
        public byte[] Salt { get; init; }
        public byte[] Hash { get; init; }

        /// <summary>
        /// A credential that matches no password, verified when the user name does not exist so
        /// that "no such user" costs exactly as much time as "wrong password".
        /// </summary>
        public static StoredCredential Dummy { get; } = new StoredCredential
        {
            EmployeeId = 0,
            Iterations = PasswordService.DefaultIterations,
            Salt = RandomNumberGenerator.GetBytes(PasswordService.SaltBytes),
            Hash = RandomNumberGenerator.GetBytes(PasswordService.HashBytes)
        };
    }

    /// <summary>
    /// The only place a password is turned into stored bytes or compared with them.
    /// PBKDF2 (RFC 2898) with HMAC-SHA256, a 16-byte random salt, a 32-byte output and
    /// 210,000 iterations; comparison in constant time. The password lives for one method call.
    /// </summary>
    public sealed class PasswordService
    {
        public const string AlgorithmName = "PBKDF2-SHA256";
        public const int DefaultIterations = 210_000;
        public const int SaltBytes = 16;
        public const int HashBytes = 32;

        public StoredCredential Hash(int employeeId, string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("A password is required.", nameof(password));

            var salt = RandomNumberGenerator.GetBytes(SaltBytes);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                DefaultIterations,
                HashAlgorithmName.SHA256,
                HashBytes);

            return new StoredCredential
            {
                EmployeeId = employeeId,
                Iterations = DefaultIterations,
                Salt = salt,
                Hash = hash
            };
        }

        public bool Verify(string password, StoredCredential stored)
        {
            if (password == null || stored == null)
                return false;

            var candidate = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                stored.Salt,
                stored.Iterations,
                HashAlgorithmName.SHA256,
                stored.Hash.Length);

            return CryptographicOperations.FixedTimeEquals(candidate, stored.Hash);
        }
    }
}
