using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.Architecture
{
    /// <summary>One event handler as the gate saw it.</summary>
    public sealed class HandlerCheck
    {
        public string Name { get; set; }
        public int Lines { get; set; }
        public bool CallsService { get; set; }
        public IReadOnlyList<string> Issues { get; set; } = Array.Empty<string>();
    }

    /// <summary>What the gate found in one source file.</summary>
    public sealed class ReviewGateReport : CommandResult
    {
        public string FileName { get; set; }
        public string ResolvedPath { get; set; }
        public bool FileFound { get; set; }
        public List<HandlerCheck> Handlers { get; } = new List<HandlerCheck>();
        public int TotalIssues { get; set; }

        public static ReviewGateReport NotFound(string correlationId, string fileName, string searched)
        {
            var report = new ReviewGateReport { Succeeded = false, CorrelationId = correlationId, FileName = fileName, ResolvedPath = searched, FileFound = false };
            report.Errors.Add($"Source file not found: {fileName}. Run the app from the project folder.");
            return report;
        }
    }

    /// <summary>
    /// Runs <see cref="ReviewGate.CheckEventHandler"/> over a real C# file: finds every `(object sender, … e)`
    /// handler, counts its lines from the signature to the closing brace, and looks for a call into a service
    /// (an injected `_field.Method(`, a `*Service` type, or an `…Async(` call).
    ///
    /// This is the pre-merge gate from the lesson, made runnable: point it at the legacy handler and it fails;
    /// point it at CommandCenterDashboard.cs and it passes. The same rules go into docs/CodeReviewChecklist.md.
    /// </summary>
    public sealed class ReviewGateService
    {
        private static readonly Regex HandlerSignature =
            new Regex(@"^\s*(?:private|protected|public|internal)\s+(?:async\s+)?void\s+(\w+)\s*\(\s*object\s+sender\s*,", RegexOptions.Compiled);

        private static readonly Regex ServiceCall =
            new Regex(@"\b_[a-z]\w*\.\w+\s*\(|\bService\b|\w+Async\s*\(", RegexOptions.Compiled);

        private readonly ActivityTrace _trace;

        public ReviewGateService(ActivityTrace trace)
        {
            _trace = trace;
        }

        public async Task<ReviewGateReport> InspectAsync(string relativePath, CommandContext ctx)
        {
            _trace.Architecture($"ReviewGateService.InspectAsync(\"{relativePath}\") corr={ctx.CorrelationId}");

            string fullPath = Resolve(relativePath);
            if (fullPath == null)
            {
                _trace.Architecture("source not found under the project folder (Application.StartupPath / working directory)");
                return ReviewGateReport.NotFound(ctx.CorrelationId, relativePath, Application.StartupPath);
            }

            string[] lines = await File.ReadAllLinesAsync(fullPath);
            var report = new ReviewGateReport { FileName = Path.GetFileName(relativePath), ResolvedPath = fullPath, FileFound = true, CorrelationId = ctx.CorrelationId };

            for (int i = 0; i < lines.Length; i++)
            {
                Match m = HandlerSignature.Match(lines[i]);
                if (!m.Success)
                    continue;

                int end = FindClosingBrace(lines, i);
                string body = string.Join("\n", lines, i, end - i + 1);

                var check = new HandlerCheck
                {
                    Name = m.Groups[1].Value,
                    Lines = end - i + 1,
                    CallsService = ServiceCall.IsMatch(body),
                };
                check.Issues = ReviewGate.CheckEventHandler(check.Name, check.Lines, check.CallsService);
                report.Handlers.Add(check);
                report.TotalIssues += check.Issues.Count;

                _trace.Architecture($"ReviewGate.CheckEventHandler(\"{check.Name}\", {check.Lines}, callsService: {check.CallsService.ToString().ToLowerInvariant()}) → {check.Issues.Count} issue(s)");
                foreach (string issue in check.Issues)
                    _trace.Architecture("  ✕ " + issue);
            }

            report.Succeeded = report.TotalIssues == 0;
            _trace.Architecture($"{report.FileName}: {report.Handlers.Count} handler(s), {report.TotalIssues} issue(s) → {(report.Succeeded ? "PASS — may merge" : "FAIL — blocked before merge")}");
            return report;
        }

        /// <summary>From the signature line, walk braces until the handler's block closes. Returns the closing line index.</summary>
        private static int FindClosingBrace(string[] lines, int start)
        {
            int depth = 0;
            bool opened = false;
            for (int i = start; i < lines.Length; i++)
            {
                foreach (char c in lines[i])
                {
                    if (c == '{') { depth++; opened = true; }
                    else if (c == '}') depth--;
                }
                if (opened && depth == 0)
                    return i;
            }
            return lines.Length - 1;
        }

        /// <summary>The project folder when run with `dotnet run` from it; otherwise walk up from the binaries to the .csproj.</summary>
        private static string Resolve(string relativePath)
        {
            string relative = relativePath.Replace('/', Path.DirectorySeparatorChar);
            var roots = new List<string> { Application.StartupPath, Directory.GetCurrentDirectory() };

            string dir = AppContext.BaseDirectory;
            for (int up = 0; up < 6 && dir != null; up++)
            {
                roots.Add(dir);
                dir = Path.GetDirectoryName(dir);
            }

            foreach (string root in roots)
            {
                if (string.IsNullOrEmpty(root))
                    continue;
                string candidate = Path.Combine(root, relative);
                if (File.Exists(candidate) && File.Exists(Path.Combine(root, "EnterpriseOps.csproj")))
                    return candidate;
            }
            return null;
        }
    }

    /// <summary>The two files the dashboard's review-gate buttons inspect.</summary>
    public static class SourceFiles
    {
        public const string CommandCenterDashboard = "UI/CommandCenterDashboard.cs";
        public const string LegacyOrderEntry = "Architecture/Samples/OrderEntryLegacy.cs.txt";
    }
}
