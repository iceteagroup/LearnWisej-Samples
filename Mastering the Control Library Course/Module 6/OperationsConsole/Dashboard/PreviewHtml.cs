using System.Net;
using System.Text;
using OperationsConsole.Models;

namespace OperationsConsole.Dashboard
{
    /// <summary>
    /// Builds the small HTML fragment shown in the dashboard's <c>htmlPreview</c> (<see cref="Wisej.Web.HtmlPanel"/>).
    /// <para>
    /// This class exists because "embed some HTML" is where dashboards get themselves into trouble. The rules it
    /// enforces are the module's rules: <b>the markup is a fixed template written here</b>, every value that comes
    /// from data or from a user (a document title, an uploaded file name) goes through
    /// <see cref="WebUtility.HtmlEncode"/>, there is no script, no external stylesheet and no link. An
    /// <c>HtmlPanel</c> is a content boundary, not a place to concatenate whatever the model happens to hold.
    /// </para>
    /// </summary>
    public static class PreviewHtml
    {
        private const string Ink = "#0d1b2a";
        private const string Muted = "#5a6b7d";
        private const string Green = "#1f9d57";
        private const string Amber = "#e8a13c";

        /// <summary>
        /// The completion summary under the ProgressBar: the single status value in words, the target, and where
        /// the numbers came from. Everything interpolated below is encoded first.
        /// </summary>
        public static string Summary(DashboardModel model, int monthlyTarget)
        {
            if (model == null)
                return Empty("No dashboard model yet — press Refresh.");

            var accent = model.OnTarget ? Green : Amber;
            var html = new StringBuilder();

            html.Append("<div style=\"font-family:sans-serif;color:").Append(Ink).Append(";font-size:12px;line-height:1.5\">");
            html.Append("<div style=\"font-size:26px;font-weight:700;color:").Append(accent).Append("\">")
                .Append(Encode(model.CompletionPercent + "%"))
                .Append("</div>");
            html.Append("<div style=\"color:").Append(Muted).Append("\">")
                .Append(Encode(model.ClosedThisMonth + " of " + monthlyTarget + " tickets closed in " + model.CurrentMonth))
                .Append(" · target ").Append(Encode(model.TargetPercent + "%"))
                .Append("</div>");
            html.Append("<div style=\"margin-top:6px;color:").Append(accent).Append(";font-weight:600\">")
                .Append(Encode(model.OnTarget ? "On target." : "Below target — " + (model.TargetPercent - model.CompletionPercent) + " points to go."))
                .Append("</div>");
            html.Append("<div style=\"margin-top:8px;color:").Append(Muted).Append("\">")
                .Append(Encode(model.TicketsAggregated + " tickets aggregated into " + model.Months.Count + " monthly points"))
                .Append("</div>");
            html.Append("<div style=\"color:").Append(Muted).Append("\">")
                .Append(Encode("Preview: " + model.PreviewDocument.Title + " (" + model.PreviewDocument.Id + ")"))
                .Append("</div>");
            html.Append("<div style=\"color:").Append(Muted).Append("\">")
                .Append(Encode("Model generated " + model.GeneratedAt.ToString("HH:mm:ss")))
                .Append("</div>");
            html.Append("</div>");

            return html.ToString();
        }

        /// <summary>A neutral message in the same frame — used before the first refresh and after a failure.</summary>
        public static string Empty(string message)
        {
            return "<div style=\"font-family:sans-serif;color:" + Muted + ";font-size:12px;line-height:1.5\">" +
                   Encode(message) + "</div>";
        }

        /// <summary>
        /// The single place where text becomes markup. If a value did not pass through here it does not go into
        /// the panel — an uploaded file called <c>&lt;script&gt;alert(1)&lt;/script&gt;.pdf</c> must read as text.
        /// </summary>
        private static string Encode(string value) => WebUtility.HtmlEncode(value ?? string.Empty);
    }
}
