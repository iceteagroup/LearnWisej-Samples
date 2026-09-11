using System;
using System.IO;
using Wisej.Web;

namespace TicketOps.Infrastructure
{
    /// <summary>The import file that ships with the project (relative to the project folder).</summary>
    public static class SampleFiles
    {
        public const string Tickets = "Data/sample-tickets.csv";
    }

    /// <summary>
    /// Turns a project-relative path into the file the import service opens. The file lives in the project
    /// folder (where Default.json is — <c>Application.MapPath</c>) and is also copied next to the binaries
    /// by the csproj, so both "dotnet run" from the project folder and a published output find it.
    /// Infrastructure may know about Wisej.NET; the import service that receives the path does not.
    /// </summary>
    public sealed class ImportFileLocator
    {
        private readonly ILog _log;

        public ImportFileLocator(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public string Resolve(string relativePath)
        {
            string[] candidates =
            {
                MapPathOrNull(relativePath),
                Path.Combine(AppContext.BaseDirectory, relativePath),
                Path.Combine(Directory.GetCurrentDirectory(), relativePath)
            };

            foreach (string candidate in candidates)
            {
                if (!string.IsNullOrEmpty(candidate) && File.Exists(candidate))
                    return Path.GetFullPath(candidate);
            }

            // Not found anywhere: hand back the first guess so the service reports "file not found" as a result.
            _log.Warn(LogLayer.Infrastructure, "ImportFileLocator.Resolve", $"{relativePath} not found in the project folder, the output folder or the working directory");
            return candidates[1];
        }

        private static string MapPathOrNull(string relativePath)
        {
            try
            {
                return Application.MapPath(relativePath);
            }
            catch (Exception)
            {
                return null;    // outside a session (e.g. a unit test) there is no application root to map
            }
        }
    }
}
