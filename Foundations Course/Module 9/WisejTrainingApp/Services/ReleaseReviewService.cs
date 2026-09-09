using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// The server-side half of the permission gate (lesson s42 §1: "never rely only on hiding a button —
    /// server-side logic should also protect the action"). The window may disable btnReviewPackage for
    /// the wrong role, but this service repeats the role check, the required-checks check and the version
    /// check on its own, from the request it is given — it never looks at a control.
    /// </summary>
    public class ReleaseReviewService
    {
        public static readonly string[] AllowedRoles = { "Admin", "Team Lead" };

        private static readonly Regex VersionPattern = new Regex(@"^\d+\.\d+\.\d+$", RegexOptions.Compiled);

        public static bool IsAllowed(string role) =>
            Array.IndexOf(AllowedRoles, role ?? string.Empty) >= 0;

        /// <summary>
        /// Creates the review package or explains why not. Throws only for a real packaging failure
        /// (the lab simulates one with <see cref="ReleaseRequest.SimulateError"/>); the caller turns
        /// that into a safe message and a redacted log line.
        /// </summary>
        public ReviewResult TryCreatePackage(ReleaseRequest req)
        {
            if (req == null)
                throw new ArgumentNullException(nameof(req));

            // 1. Authorization — the same rule as the UI, evaluated again here.
            if (!IsAllowed(req.Role))
                return ReviewResult.Fail("permission", "Review is restricted.");

            // 2. All required checks complete?
            if (req.RequiredChecked < req.RequiredTotal)
                return ReviewResult.Fail("checks", "Required checks incomplete.");

            // 3. Version format major.minor.patch.
            if (!VersionPattern.IsMatch(req.Version ?? string.Empty))
                return ReviewResult.Fail("version", "Version must look like major.minor.patch (e.g. 1.4.0).");

            // 4. Packaging — this is where a real app would zip the publish folder.
            if (req.SimulateError)
                throw new IOException("Access to path 'D:\\deploy\\ServiceDesk.zip' is denied");

            return ReviewResult.Ok(BuildSummary(req));
        }

        /// <summary>The "safe summary" from the walkthrough: what, where, who, status, open items, rollback — no secrets.</summary>
        private static string BuildSummary(ReleaseRequest req)
        {
            var sb = new StringBuilder();
            sb.AppendLine("DEPLOYMENT REVIEW PACKAGE");
            sb.AppendLine("Environment : " + req.Environment);
            sb.AppendLine("Target      : " + req.Target);
            sb.AppendLine("Version     : " + req.Version);
            sb.AppendLine("Reviewer    : " + (string.IsNullOrWhiteSpace(req.Reviewer) ? "(not entered)" : req.Reviewer.Trim()));
            sb.AppendLine("Role        : " + req.Role + " (server check passed)");
            sb.AppendLine("Status      : READY for deployment review");
            sb.AppendLine($"Required    : {req.RequiredChecked} / {req.RequiredTotal} complete");
            sb.AppendLine($"Optional    : {req.OptionalChecked} / {req.OptionalTotal} complete");
            sb.AppendLine("Secrets     : license key " + req.LicenseKeyStatus + "; connection " + req.ConnectionStatus);
            sb.AppendLine("Known issues: " + (req.OpenOptionalItems.Count == 0
                ? "none open"
                : string.Join("; ", req.OpenOptionalItems)));
            sb.AppendLine("Rollback    : " + (req.RollbackPlanWritten
                ? "plan written — redeploy the previous package and restore the last known-good config"
                : "NOT documented — write the rollback plan before go-live"));
            sb.AppendLine("Notes       : " + FirstLine(req.Notes));
            sb.AppendLine("Reviewed at : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture));
            return sb.ToString().TrimEnd();
        }

        private static string FirstLine(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "(none)";

            string line = text.Trim().Split('\n')[0].Trim();
            return line.Length > 110 ? line.Substring(0, 107) + "…" : line;
        }
    }
}
