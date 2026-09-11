using System;
using System.Collections.Generic;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    public sealed class NoteRenderResult : CommandResult
    {
        private NoteRenderResult(string summary, string correlationId, IReadOnlyList<string> errors, string html, bool allowHtml)
            : base(true, false, summary, correlationId, errors)
        {
            Html = html;
            AllowHtml = allowHtml;
        }

        /// <summary>Exactly what the control's Text must be set to.</summary>
        public string Html { get; }

        /// <summary>Exactly what the control's AllowHtml must be set to. The screen sets both, and nothing else.</summary>
        public bool AllowHtml { get; }

        public static NoteRenderResult Rendered(string html, bool allowHtml, string correlationId)
            => new NoteRenderResult(allowHtml ? "html" : "escaped", correlationId, null, html, allowHtml);
    }

    /// <summary>
    /// Deciding how untrusted text reaches the screen — in a service, because it is a security decision and not
    /// a formatting preference.
    ///
    /// The screen does two things and no more: <c>lbl.AllowHtml = result.AllowHtml</c> and
    /// <c>lbl.Text = result.Html</c>. A customer note is rendered with <c>AllowHtml = false</c>: the characters are
    /// shown and nothing is interpreted. A surface that genuinely needs formatting would go through
    /// <see cref="HtmlText.Sanitize"/> (allow-list, attributes dropped) instead — never raw.
    /// </summary>
    public sealed class NoteRenderService
    {
        private readonly ActivityTrace _trace;

        public NoteRenderService(ActivityTrace trace)
        {
            _trace = trace;
        }

        public NoteRenderResult Prepare(CommandContext context, WorkOrderNote note)
        {
            if (note == null) throw new ArgumentNullException(nameof(note));

            if (HtmlText.LooksLikeMarkup(note.Text))
                _trace?.Security($"NoteRenderService — note #{note.Id} from '{note.Source}' contains {HtmlText.DescribeMarkup(note.Text)}; rendered escaped");

            return NoteRenderResult.Rendered(note.Text, false, context.CorrelationId);
        }
    }
}
