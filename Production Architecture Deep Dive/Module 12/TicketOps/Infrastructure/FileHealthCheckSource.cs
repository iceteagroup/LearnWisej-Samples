using System;
using System.IO;
using TicketOps.Domain;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// Reads HealthCheck.json from the application folder. The same file is what a load-balancer probe fetches
    /// over HTTP (GET /HealthCheck.json, served by UseFileServer — see Startup.cs), so the build ships one
    /// manifest and both the machine and the diagnostics page read it.
    /// Application.MapPath resolves a file relative to the application's root directory.
    /// </summary>
    public sealed class FileHealthCheckSource : IHealthCheckSource
    {
        public const string FileName = "HealthCheck.json";

        /// <summary>The URL a load-balancer probe hits (relative to the site root).</summary>
        public const string ProbeUrl = "/" + FileName;

        private readonly ILog _log;

        public FileHealthCheckSource(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public string Location => Application.MapPath(FileName);

        public HealthReport Load()
        {
            _log.Info(LogLayer.Infrastructure, "FileHealthCheckSource.Load", $"File.ReadAllText(Application.MapPath(\"{FileName}\"))");

            string json = File.ReadAllText(Location);
            var report = HealthCheckJson.Parse(json);

            _log.Info(LogLayer.Infrastructure, "FileHealthCheckSource.Load",
                $"manifest: {report.App} v{report.Version} build {report.Build} ({report.Environment}) · {report.Checks.Count} dependencies declared");
            return report;
        }
    }
}
