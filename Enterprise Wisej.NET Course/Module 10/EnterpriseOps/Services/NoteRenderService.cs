using System;
using System.Collections.Generic;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>The three ways this application will put a note on screen. Two are safe; one is the failure path.</summary>
    public enum NoteRenderMode
    {
        /// <summary>AllowHtml = false. The characters are shown. The default, and the right answer almost always.</summary>
        Escaped,

        /// <summary>AllowHtml = true, text through the allow-list sanitizer. For the rare surface that needs &lt;b&gt;.</summary>
        Sanitized,

        /// <summary>AllowHtml = true, raw untrusted text. This is the bug the AllowHtml review exists to find.</summary>
        RawHtml
    }

    public sealed class NoteRenderResult : CommandResult
    {
        private NoteRenderResult(bool succeeded, string summary, string correlationId, IReadOnlyList<string> errors,
            NoteRenderMode mode, string html, bool allowHtml, string rawText, string markup, string warning)
            : base(succeeded, false, summary, correlationId, errors)
        {
            Mode = mode;
            Html = html;
            AllowHtml = allowHtml;
            RawText = rawText;
            Markup = markup;
            Warning = warning;
        }

        public NoteRenderMode Mode { get; }

        /// <summary>Exactly what the control's Text must be set to.</summary>
        public string Html { get; }

        /// <summary>Exactly what the control's AllowHtml must be set to. The screen sets both, and nothing else.</summary>
        public bool AllowHtml { get; }

        public string RawText { get; }

        /// <summary>What the payload tried to do, named — for the trace, the audit detail and the banner.</summary>
        public string Markup { get; }

        public string Warning { get; }

        public static NoteRenderResult Rendered(NoteRenderMode mode, string html, bool allowHtml, string raw, string markup,
            string warning, string correlationId)
            => new NoteRenderResult(true, mode.ToString(), correlationId, null, mode, html, allowHtml, raw, markup, warning);
    }

    /// <summary>
    /// Deciding how untrusted text reaches the screen — in a service, because it is a security decision and not
    /// a formatting preference.
    ///
    /// The screen ends up doing two things and no more: <c>lbl.AllowHtml = result.AllowHtml</c> and
    /// <c>lbl.Text = result.Html</c>. It never chooses the encoding itself, which means a reviewer can answer
    /// "which UI elements render HTML, and who decided what goes into them?" by reading one file.
    /// </summary>
    public sealed class NoteRenderService
    {
        private readonly IAuditLog _audit;
        private readonly ActivityTrace _trace;

        public NoteRenderService(IAuditLog audit, ActivityTrace trace)
        {
            _audit = audit;
            _trace = trace;
        }

        public NoteRenderResult Prepare(CommandContext context, WorkOrderNote note, NoteRenderMode mode)
        {
            if (note == null) throw new ArgumentNullException(nameof(note));

            string markup = HtmlText.DescribeMarkup(note.Text);
            bool dangerous = HtmlText.LooksLikeMarkup(note.Text);

            switch (mode)
            {
                case NoteRenderMode.Sanitized:
                    _trace?.Security($"NoteRenderService — note #{note.Id} sanitized against the allow-list ({string.Join(", ", HtmlText.AllowedTags)}); attributes dropped");
                    return NoteRenderResult.Rendered(mode, HtmlText.Sanitize(note.Text), true, note.Text, markup,
                        dangerous ? $"Sanitized: {markup} — only {string.Join(", ", HtmlText.AllowedTags)} survive, and never an attribute." : null,
                        context.CorrelationId);

                case NoteRenderMode.RawHtml:
                    // The deliberate failure path. It is audited, because in a real system this is a finding:
                    // untrusted text reached an HTML-capable surface, and someone must know it happened.
                    _audit.Write(context, "UnsafeHtmlRender", AuditResult.Failed, $"note #{note.Id}",
                        $"untrusted text from '{note.Source}' rendered with AllowHtml = true ({markup})");
                    _trace?.Security($"NoteRenderService — note #{note.Id} rendered RAW: {markup}. This is the finding, not the feature.");
                    return NoteRenderResult.Rendered(mode, note.Text, true, note.Text, markup,
                        $"Raw HTML from '{note.Source}': {markup} is now live markup in the page.",
                        context.CorrelationId);

                default:
                    // AllowHtml = false, raw text: the framework renders the characters and interprets nothing.
                    // HtmlText.Escape produces the same result for surfaces where AllowHtml cannot be turned off
                    // (a grid column, a tooltip, a toast) — the trace shows what it would have produced.
                    _trace?.Security($"NoteRenderService — note #{note.Id} escaped; {markup} shown as characters, interpreted as nothing");
                    _trace?.Security($"NoteRenderService — HtmlText.Escape equivalent: {Preview(HtmlText.Escape(note.Text))}");
                    return NoteRenderResult.Rendered(NoteRenderMode.Escaped, note.Text, false, note.Text, markup,
                        null, context.CorrelationId);
            }
        }

        /// <summary>A short, single-line preview for a trace entry — logs never carry a whole payload.</summary>
        private static string Preview(string text)
            => text.Length <= 70 ? text.Replace("\n", " ") : text.Substring(0, 70).Replace("\n", " ") + "…";
    }
}
