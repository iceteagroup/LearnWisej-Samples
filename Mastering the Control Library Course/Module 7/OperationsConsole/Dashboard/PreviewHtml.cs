using System.Net;
using System.Text;
using OperationsConsole.Models;

namespace OperationsConsole.Dashboard
{
    /// <summary>
    /// Builds the small HTML fragment shown in the dashboard's <c>htmlPreview</c>. The markup is a fixed template,
    /// and every value that comes from data goes through <see cref="WebUtility.HtmlEncode"/>: no script, no external
    /// stylesheet, no link.
    /// </summary>
    public static class PreviewHtml
    {
        private const string Ink = "#0d1b2a";
        private const string Muted = "#5a6b7d";
        private const string Green = "#1f9d57";
        private const string Amber = "#e8a13c";

        /// <summary>The completion value in words, next to the ProgressBar.</summary>
        public static string Summary(DashboardModel model, int monthlyTarget)
        {
            if (model == null)
                return Empty("No data yet — press Refresh.");

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
            html.Append("</div>");

            return html.ToString();
        }

        /// <summary>A neutral message in the same frame.</summary>
        public static string Empty(string message)
        {
            return "<div style=\"font-family:sans-serif;color:" + Muted + ";font-size:12px;line-height:1.5\">" +
                   Encode(message) + "</div>";
        }

        private static string Encode(string value) => WebUtility.HtmlEncode(value ?? string.Empty);
    }
}
