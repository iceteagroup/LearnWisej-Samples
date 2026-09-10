using System.Collections.Generic;

namespace EnterpriseOps.Security
{
    public enum ChecklistState
    {
        /// <summary>Done in this sample, and the evidence says where to look.</summary>
        Done,

        /// <summary>Demonstrated in the sample but only meaningful once the app is hosted for real.</summary>
        SampleOnly,

        /// <summary>Deliberately not done here — a deployment concern the runbook owns (Module 12).</summary>
        Deployment
    }

    public sealed class ChecklistItem
    {
        public ChecklistItem(string area, string item, ChecklistState state, string evidence)
        {
            Area = area;
            Item = item;
            State = state;
            Evidence = evidence;
        }

        public string Area { get; }
        public string Item { get; }
        public ChecklistState State { get; }

        /// <summary>Where a reviewer looks to confirm the item — a file, a control, or the runbook that owns it.</summary>
        public string Evidence { get; }
    }

    /// <summary>
    /// The security hardening checklist as code, so the document in <c>docs/SecurityHardeningChecklist.md</c> and
    /// the running application cannot drift apart. The lesson's instruction is to treat it as living code: add an
    /// item every time a review finds a gap, and record who checked each item for each release.
    ///
    /// The list covers the surfaces a Wisej.NET application actually has: HTML, uploads, cookies, CSP and the
    /// other response headers, logs, secrets, and the reverse proxy in front of it.
    /// </summary>
    public static class HardeningChecklist
    {
        public static IReadOnlyList<ChecklistItem> Items { get; } = new List<ChecklistItem>
        {
            new ChecklistItem("HTML", "Every control with AllowHtml = true is inventoried and its text source is known",
                ChecklistState.Done, "Run 'AllowHtml review' — it walks the live control tree, it is not a list someone typed"),
            new ChecklistItem("HTML", "Untrusted text is escaped, or sanitized against an allow-list, before it reaches an HTML surface",
                ChecklistState.Done, "Security/HtmlText.cs · the safe/unsafe note buttons on the page"),
            new ChecklistItem("HTML", "Grid cells and tooltips count as HTML surfaces (DataGridViewColumn.AllowHtml, ToolTip.AllowHtml)",
                ChecklistState.Done, "Services/SecurityReviewService.cs inspects grid columns as well as controls"),

            new ChecklistItem("Uploads", "Size limit, content-type allow-list, extension check, and a stored name the server chose",
                ChecklistState.Deployment, "No upload surface in this module — the item stays on the list so the next screen inherits it"),
            new ChecklistItem("Uploads", "Files are stored outside the web root and never served back by original name",
                ChecklistState.Deployment, "docs/SecurityHardeningChecklist.md"),

            new ChecklistItem("Cookies", "Session cookie is Secure, HttpOnly and SameSite=Lax (or Strict)",
                ChecklistState.Deployment, "Set on the host — Web.config / Kestrel cookie policy; verified per environment"),

            new ChecklistItem("Headers", "Content-Security-Policy, Strict-Transport-Security, X-Content-Type-Options, Referrer-Policy",
                ChecklistState.Deployment, "Startup.cs in the deployment module (12); CSP must allow the Wisej *.wx endpoints"),

            new ChecklistItem("Logs", "No password, token, cookie, claim value or full personal record is ever written to a log",
                ChecklistState.Done, "Diagnostics/ActivityTrace.cs writes subject ids, tenants and permission names only"),
            new ChecklistItem("Logs", "Every sensitive command writes an audit entry with a correlation id, including denials",
                ChecklistState.Done, "Security/PermissionService.Demand + Security/AuditLog.cs · visible in dgvAudit"),

            new ChecklistItem("Secrets", "Connection strings, keys and client secrets come from protected configuration or a vault",
                ChecklistState.SampleOnly, "This sample has no secret at all — the simulated provider needs none, which is the point"),
            new ChecklistItem("Secrets", "Nothing secret is in Default.json or any file the static file server can reach",
                ChecklistState.Done, "Startup.cs refuses to serve *.json; Default.json holds theme and startup only"),

            new ChecklistItem("Identity", "The gate sits in front of every entry point, not only the main page",
                ChecklistState.Done, "UI/AuditLogPage.RequireSignInAsync + every service call takes a CommandContext"),
            new ChecklistItem("Identity", "Services read identity from the session context, never from a control, query string or hidden field",
                ChecklistState.Done, "Security/SessionContext.BeginCommand is the only source of a CommandContext"),
            new ChecklistItem("Identity", "A long-running session can reload roles without a new login",
                ChecklistState.Done, "Security/SessionContext.RefreshPermissions"),

            new ChecklistItem("Proxy", "The reverse proxy forwards the original scheme and client address (X-Forwarded-Proto / -For)",
                ChecklistState.Deployment, "Otherwise the app logs the proxy's address in every audit entry — docs/SecurityHardeningChecklist.md"),
        };
    }
}
