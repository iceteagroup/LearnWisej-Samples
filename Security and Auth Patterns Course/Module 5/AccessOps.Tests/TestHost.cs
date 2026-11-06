using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using AccessOps.Infrastructure;
using AccessOps.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AccessOps.Tests
{
    /// <summary>A session the test controls: no browser, no Wisej.NET.</summary>
    public sealed class TestSession : IAccessOpsSession
    {
        public SessionContext Context { get; set; }

        public SessionContext Current => this.Context ?? throw new NotSignedInException();
        public SessionContext TryGetCurrent() => this.Context;
        public string ClientAddress => "10.4.2.61";
        public string UserAgent => "xunit";
        public void Establish(SessionContext context) => this.Context = context;
        public void Replace(SessionContext context) => this.Context = context;
        public void Clear() => this.Context = null;
        public void End() { }
    }

    /// <summary>Records what would have been streamed to the browser.</summary>
    public sealed class RecordingDelivery : IFileDelivery
    {
        public List<(byte[] Content, string FileName)> Sent { get; } = new List<(byte[], string)>();
        public void Send(byte[] content, string fileName) => this.Sent.Add((content, fileName));
    }

    /// <summary>Collects log lines so a test can assert what the server recorded.</summary>
    public sealed class ListLogger<T> : ILogger<T>
    {
        public ConcurrentQueue<string> Lines { get; } = new ConcurrentQueue<string>();
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            => this.Lines.Enqueue(logLevel + " " + formatter(state, exception));
    }

    /// <summary>
    /// A fresh AccessOps database per test, seeded from the same Data/*.json files the application
    /// uses, in a temporary folder. The services are the real ones.
    /// </summary>
    public sealed class TestHost : IDisposable
    {
        private readonly string _folder = Path.Combine(Path.GetTempPath(), "accessops-tests-" + Guid.NewGuid().ToString("N"));

        public TestHost()
        {
            this.Database = new AccessOpsDatabase(_folder, Path.Combine(AppContext.BaseDirectory, "Data"));
            this.Database.EnsureCreated(new PasswordService());

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"))
                .Build();

            this.Permissions = new PermissionService(configuration, this.Database);
            this.Employees = new EmployeeRepository(this.Database);
            this.Authorize = new Authorize(this.Session, this.AuthorizeLog);
            this.DocumentRepository = new DocumentRepository(this.Database, this.DocumentLog);
            this.Store = new DocumentStore(Path.Combine(_folder, "documents"), Path.Combine(_folder, "quarantine"));
            this.DocumentRepository.EnsureSeedFiles(this.Store);
            this.Documents = new DocumentService(this.Authorize, this.DocumentRepository, this.Store, new UploadGate(20 * 1024 * 1024), this.Delivery, this.DocumentServiceLog);
            this.ApprovalRepository = new ApprovalRepository(this.Database);
            this.Approvals = new ApprovalService(this.Authorize, this.ApprovalRepository, new ListLogger<ApprovalService>());
        }

        public AccessOpsDatabase Database { get; }
        public TestSession Session { get; } = new TestSession();
        public ListLogger<Authorize> AuthorizeLog { get; } = new ListLogger<Authorize>();
        public ListLogger<DocumentRepository> DocumentLog { get; } = new ListLogger<DocumentRepository>();
        public PermissionService Permissions { get; }
        public DocumentRepository DocumentRepository { get; }
        public DocumentStore Store { get; }
        public RecordingDelivery Delivery { get; } = new RecordingDelivery();
        public ListLogger<DocumentService> DocumentServiceLog { get; } = new ListLogger<DocumentService>();
        public EmployeeRepository Employees { get; }
        public Authorize Authorize { get; }
        public DocumentService Documents { get; }
        public ApprovalRepository ApprovalRepository { get; }
        public ApprovalService Approvals { get; }

        /// <summary>Builds the SessionContext the login flow would build for this employee.</summary>
        public SessionContext SignInAs(int employeeId)
        {
            var e = this.Employees.Find(employeeId);
            this.Session.Context = new SessionContext(
                e.Id, e.UserName, e.DisplayName, e.Department, e.IsManager,
                e.Roles.Split(';', StringSplitOptions.RemoveEmptyEntries), DateTime.UtcNow, "10.4.2.61", "xunit", "Test",
                this.Permissions.ResolveFor(e), this.Employees.DirectReports(e.Id), DateTime.UtcNow);
            return this.Session.Context;
        }

        public string StatusOf(int requestId) => this.ApprovalRepository.GetAsync(requestId).GetAwaiter().GetResult().Status;

        public void Dispose()
        {
            try { Directory.Delete(_folder, recursive: true); } catch (IOException) { }
        }
    }
}
