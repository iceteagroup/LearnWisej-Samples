using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Security;
using Wisej.Web;

namespace EnterpriseOps.Services
{
    /// <summary>One HTML-capable surface found on the screen, and the verdict on it.</summary>
    public sealed class HtmlSurface
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool AllowHtml { get; set; }

        /// <summary>Where the text comes from: a constant, the services, the identity provider, or a user.</summary>
        public string TextSource { get; set; }

        /// <summary>What protects it: the framework's escaping, the sanitizer, or nothing.</summary>
        public string Handling { get; set; }

        public bool IsFinding { get; set; }

        public string Verdict => IsFinding ? "FINDING" : "ok";
    }

    public sealed class HtmlReviewResult : CommandResult
    {
        private HtmlReviewResult(string summary, string correlationId, IReadOnlyList<HtmlSurface> surfaces)
            : base(true, false, summary, correlationId, null)
        {
            Surfaces = surfaces;
        }

        public IReadOnlyList<HtmlSurface> Surfaces { get; }

        public IReadOnlyList<HtmlSurface> HtmlEnabled => Surfaces.Where(s => s.AllowHtml).ToList();
        public IReadOnlyList<HtmlSurface> Findings => Surfaces.Where(s => s.IsFinding).ToList();

        public static HtmlReviewResult Of(IReadOnlyList<HtmlSurface> surfaces, string correlationId)
            => new HtmlReviewResult($"{surfaces.Count} HTML-capable surfaces · {surfaces.Count(s => s.AllowHtml)} with AllowHtml = true · {surfaces.Count(s => s.IsFinding)} findings",
                correlationId, surfaces);
    }

    /// <summary>
    /// The AllowHtml review, run against the **live control tree** rather than a list someone typed.
    ///
    /// The lesson's instruction is that every place raw HTML is allowed in a control must be checked, because an
    /// unreviewed HTML surface is a cross-site-scripting risk. A written inventory answers that question on the
    /// day it is written; walking the running screen answers it on the day it is asked — including the control a
    /// colleague added last week, and the grid column whose AllowHtml was flipped in the designer.
    ///
    /// It finds surfaces by reflection: any control (or grid column) exposing a public <c>bool AllowHtml</c>. In
    /// Wisej.NET that is Label, ButtonBase, ListBox, ComboBox, GroupBox, TabPage, TreeNode, ToolTip,
    /// DataGridViewColumn and DataGridViewCell, among others — more surfaces than most teams expect, which is
    /// exactly why the review is automated.
    ///
    /// What reflection cannot know is where each surface's text comes from. That is a human judgement, so it is
    /// declared once in <see cref="TextSources"/> and reviewed like code.
    /// </summary>
    public sealed class SecurityReviewService
    {
        private readonly IPermissionService _permissions;
        private readonly ActivityTrace _trace;

        /// <summary>
        /// Where each named surface's text comes from. Anything not listed is treated as author-written constant
        /// text — and the review says so, so an unlisted control that renders HTML still shows up for judgement.
        /// </summary>
        private static readonly Dictionary<string, string> TextSources = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["lblNote"] = "UNTRUSTED — a customer note from a public portal",
            ["lblNoteRaw"] = "UNTRUSTED — the same note, shown as stored characters",
            ["lblNoteSource"] = "provider/user metadata (note author and source)",
            ["lstTrace"] = "service text that quotes provider group names and user ids",
            ["colDetail"] = "audit detail written by the services (quotes user ids and targets)",
            ["colUser"] = "user ids from the identity provider",
            ["lblUser"] = "display name from the 'name' claim",
            ["lblTenant"] = "tenant id from the 'tid' claim",
            ["lblBanner"] = "service messages, which quote permission names",
        };

        public SecurityReviewService(IPermissionService permissions, ActivityTrace trace)
        {
            _permissions = permissions;
            _trace = trace;
        }

        /// <summary>
        /// Walks the control tree under <paramref name="root"/>. <paramref name="sanitizedSurfaces"/> names the
        /// controls the screen is currently feeding through <see cref="HtmlText.Sanitize"/>, so a legitimately
        /// formatted surface is not reported as a finding.
        /// </summary>
        public HtmlReviewResult Review(CommandContext context, Control root, ISet<string> sanitizedSurfaces)
        {
            var surfaces = new List<HtmlSurface>();
            Walk(root, surfaces, sanitizedSurfaces ?? new HashSet<string>(StringComparer.Ordinal));

            surfaces = surfaces.OrderByDescending(s => s.IsFinding).ThenByDescending(s => s.AllowHtml)
                               .ThenBy(s => s.Name, StringComparer.Ordinal).ToList();

            var result = HtmlReviewResult.Of(surfaces, context.CorrelationId);
            _trace?.Security($"AllowHtml review — {result.Summary}");

            foreach (HtmlSurface finding in result.Findings)
                _trace?.Security($"AllowHtml FINDING — {finding.Name} ({finding.Type}): {finding.TextSource}, protected by: {finding.Handling}");

            return result;
        }

        /// <summary>The hardening checklist, counted by state, for the status line under the review.</summary>
        public string ChecklistSummary(CommandContext context)
        {
            int done = HardeningChecklist.Items.Count(i => i.State == ChecklistState.Done);
            int deployment = HardeningChecklist.Items.Count(i => i.State == ChecklistState.Deployment);
            int sample = HardeningChecklist.Items.Count(i => i.State == ChecklistState.SampleOnly);

            bool mayRunDiagnostics = _permissions.Has(context, Permission.AdminDiagnostics);
            _trace?.Security($"Hardening checklist — {done} done in code, {sample} sample-only, {deployment} owned by the deployment runbook"
                           + $" · AdminDiagnostics held by this caller: {mayRunDiagnostics}");

            return $"{HardeningChecklist.Items.Count} checklist items · {done} proven here · {deployment} deployment-owned";
        }

        private void Walk(Control control, List<HtmlSurface> surfaces, ISet<string> sanitized)
        {
            if (control == null) return;

            AddIfHtmlCapable(control, control.Name, control.GetType().Name, surfaces, sanitized);

            if (control is DataGridView grid)
                foreach (DataGridViewColumn column in grid.Columns)
                    AddIfHtmlCapable(column, column.Name, "DataGridViewColumn", surfaces, sanitized);

            foreach (Control child in control.Controls)
                Walk(child, surfaces, sanitized);
        }

        private static void AddIfHtmlCapable(object target, string name, string typeName, List<HtmlSurface> surfaces, ISet<string> sanitized)
        {
            PropertyInfo property = target.GetType().GetProperty("AllowHtml", BindingFlags.Public | BindingFlags.Instance);
            if (property == null || property.PropertyType != typeof(bool)) return;

            bool allowHtml = (bool)property.GetValue(target);
            string source = TextSources.TryGetValue(name ?? string.Empty, out string declared)
                ? declared
                : "constant text written by the developer";

            bool untrusted = source.StartsWith("UNTRUSTED", StringComparison.Ordinal)
                          || source.StartsWith("provider", StringComparison.Ordinal)
                          || source.StartsWith("service", StringComparison.Ordinal)
                          || source.StartsWith("user", StringComparison.Ordinal)
                          || source.StartsWith("audit", StringComparison.Ordinal)
                          || source.StartsWith("display", StringComparison.Ordinal)
                          || source.StartsWith("tenant", StringComparison.Ordinal);

            bool isSanitized = sanitized.Contains(name ?? string.Empty);

            string handling =
                !allowHtml ? "AllowHtml = false — the framework renders the characters" :
                isSanitized ? "HtmlText.Sanitize (allow-list, attributes dropped)" :
                untrusted ? "NOTHING — raw text on an HTML surface" :
                "author-written text only";

            surfaces.Add(new HtmlSurface
            {
                Name = string.IsNullOrEmpty(name) ? $"(unnamed {typeName})" : name,
                Type = typeName,
                AllowHtml = allowHtml,
                TextSource = source,
                Handling = handling,
                IsFinding = allowHtml && untrusted && !isSanitized,
            });
        }
    }
}
