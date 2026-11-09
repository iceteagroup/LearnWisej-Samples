using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using AccessOps.Documents;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using AccessOps.Security;
using Microsoft.Extensions.Logging;

namespace AccessOps.Tests
{
    /// <summary>A clock the tests move by hand.</summary>
    public sealed class FakeClock : IClock
    {
        /// <summary>The current instant.</summary>
        public DateTimeOffset Now { get; set; } = new DateTimeOffset(2026, 11, 9, 10, 41, 0, TimeSpan.Zero);
    }

    /// <summary>Collects every log line the services write, so tests can search them.</summary>
    public sealed class CapturingLoggerProvider : ILoggerProvider
    {
        /// <summary>Formatted messages plus exception text.</summary>
        public ConcurrentQueue<string> Lines { get; } = new ConcurrentQueue<string>();

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName) => new Logger(this);

        /// <inheritdoc />
        public void Dispose() { }

        private sealed class Logger : ILogger
        {
            private readonly CapturingLoggerProvider _owner;
            public Logger(CapturingLoggerProvider owner) => _owner = owner;
            public IDisposable BeginScope<TState>(TState state) => null;
            public bool IsEnabled(LogLevel logLevel) => true;
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                => _owner.Lines.Enqueue(formatter(state, exception) + (exception == null ? "" : " " + exception));
        }
    }

    /// <summary>
    /// A fresh AccessOps world per test: a temporary SQLite database with the Module 4 fixtures
    /// (the test project builds in Debug, so the demo password hashes are seeded), the services
    /// wired like Startup.cs, a fake clock and a capturing logger.
    /// </summary>
    public sealed class TestWorld : IDisposable
    {
        /// <summary>The demo password of partner.ana@fabrikam.test (local fixture only).</summary>
        public const string AnaPassword = "Fabrikam-demo-17";

        /// <summary>The demo password of the svc.reports service account (local fixture only).</summary>
        public const string ServicePassword = "Reports-demo-31";

        /// <summary>Dana Ellis at the lab's test provider.</summary>
        public static readonly ExternalIdentity Dana = new ExternalIdentity(AccessOpsDatabase.ContosoIssuer, "4f21-9ac0-dana-ellis");

        static TestWorld()
        {
            Subject.UseOpaqueKey(RandomNumberGenerator.GetBytes(32));
        }

        public TestWorld()
        {
            this.Folder = Path.Combine(Path.GetTempPath(), "accessops-tests-" + Guid.NewGuid().ToString("N")[..8]);
            this.Logs = new CapturingLoggerProvider();
            this.LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(b => b.AddProvider(this.Logs));
            this.Database = new AccessOpsDatabase(this.Folder);
            this.Hasher = new PasswordHasher(iterations: 1_000);
            this.Database.EnsureCreated(this.Hasher);
            this.Audit = new AuditLog(this.Database, this.LoggerFactory.CreateLogger<AuditLog>());
            this.LoginAudit = new LoginAudit(this.Database, this.LoggerFactory.CreateLogger<LoginAudit>());
            this.Accounts = new AccountStore(this.Database);
            this.Grants = new GrantStore(this.Database, this.Clock);
            this.Throttle = new LoginThrottle(this.Clock);
            this.Login = new LoginService(this.Accounts, this.Hasher, this.Throttle, this.Grants, this.LoginAudit, this.Clock);
            this.Repository = new CountingDocumentRepository(this.Database);
            this.Export = new ExportService(this.Grants, this.Repository, this.Repository, this.Audit, this.LoggerFactory.CreateLogger<ExportService>());
            this.Documents = new DocumentService(this.Grants, this.Repository, this.Audit);
            this.SignOut = new SignOutService(this.Grants, this.LoginAudit);
            this.Handoffs = new CookieHandoffStore(this.Clock);
            this.ExternalHandoffs = new ExternalHandoffStore(this.Clock);
            this.Mapper = new ExternalIdentityMapper(this.Accounts);
        }

        public string Folder { get; }
        public FakeClock Clock { get; } = new FakeClock();
        public CapturingLoggerProvider Logs { get; }
        public ILoggerFactory LoggerFactory { get; }
        public AccessOpsDatabase Database { get; }
        public PasswordHasher Hasher { get; }
        public AuditLog Audit { get; }
        public LoginAudit LoginAudit { get; }
        public AccountStore Accounts { get; }
        public GrantStore Grants { get; }
        public LoginThrottle Throttle { get; }
        public LoginService Login { get; }
        public CountingDocumentRepository Repository { get; }
        public ExportService Export { get; }
        public DocumentService Documents { get; }
        public SignOutService SignOut { get; }
        public CookieHandoffStore Handoffs { get; }
        public ExternalHandoffStore ExternalHandoffs { get; }
        public ExternalIdentityMapper Mapper { get; }

        /// <summary>
        /// Signs Dana in the way the page load does after the OpenID Connect hand-off (map issuer +
        /// subject, one grant) and returns the principal it would establish.
        /// </summary>
        public System.Security.Claims.ClaimsPrincipal SignInPartner(out VerifiedSignIn signIn)
        {
            var mapped = this.Mapper.Map(Dana);
            if (!mapped.Succeeded)
                throw new InvalidOperationException("fixture mapping failed: " + mapped.Reason);
            var grant = this.Grants.Issue(mapped.Account.Id, mapped.Account.Tenant, mapped.Account.Role);
            signIn = new VerifiedSignIn
            {
                AccountId = mapped.Account.Id,
                TenantId = mapped.Account.Tenant,
                GrantId = grant.Id,
                Roles = new[] { mapped.Account.Role },
                DisplayName = mapped.Account.DisplayName,
                Method = "oidc",
                VerifiedAt = this.Clock.Now
            };
            return ApplicationPrincipal.Build(signIn);
        }

        /// <summary>Every row of a table rendered as text, for "does this value appear anywhere" checks.</summary>
        public IEnumerable<string> Dump(string table)
        {
            using var db = this.Database.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT * FROM " + table;   // test-only, constant table names
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var values = new object[r.FieldCount];
                r.GetValues(values);
                yield return string.Join("|", values);
            }
        }

        public void Dispose()
        {
            this.LoggerFactory.Dispose();
            try { Directory.Delete(this.Folder, true); } catch (IOException) { }
        }
    }
}
