using System;
using System.Text;
using System.Text.RegularExpressions;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// The two safe ways to put untrusted text on a screen, and the one place both are implemented.
    ///
    /// Server-side rendering removes a lot of browser-side risk, but a Wisej.NET application still ships HTML:
    /// any control with <c>AllowHtml = true</c> renders its Text as markup. If that Text ever contains something
    /// a user typed — a customer note, a file name, a search term, an error message quoting input — the control
    /// is an injection point.
    ///
    ///  · <see cref="Escape"/> — the default. The characters are shown, nothing is interpreted. Use it whenever
    ///    formatting is not required (which is almost always), or simply leave <c>AllowHtml = false</c>.
    ///  · <see cref="Sanitize"/> — for the rare surface that must keep a little formatting. Allow-list, never
    ///    deny-list: everything not explicitly permitted is escaped, so a payload nobody thought of is inert.
    ///
    /// The allow-list is deliberately tiny (b, i, em, strong, br). A sanitizer that grows tags on request stops
    /// being a security control; in production use a maintained library (HtmlSanitizer / AngleSharp) rather than
    /// a regular expression — this one is small enough to read in a lab, and it is documented as such.
    /// </summary>
    public static class HtmlText
    {
        /// <summary>The only tags <see cref="Sanitize"/> lets through. Attributes are dropped from all of them.</summary>
        public static readonly string[] AllowedTags = { "b", "i", "em", "strong", "br" };

        private static readonly Regex TagPattern = new Regex(@"</?\s*([a-zA-Z][a-zA-Z0-9]*)\b[^>]*>", RegexOptions.Compiled);

        /// <summary>Renders every character as itself: &lt; becomes &amp;lt;, quotes and ampersands too.</summary>
        public static string Escape(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var sb = new StringBuilder(text.Length + 16);
            foreach (char c in text)
            {
                switch (c)
                {
                    case '&': sb.Append("&amp;"); break;
                    case '<': sb.Append("&lt;"); break;
                    case '>': sb.Append("&gt;"); break;
                    case '"': sb.Append("&quot;"); break;
                    case '\'': sb.Append("&#39;"); break;
                    default: sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Escapes everything, then puts back only the allow-listed tags — without any attribute, so an
        /// <c>onerror</c>, a <c>style</c> or a <c>javascript:</c> href cannot survive the round trip.
        /// </summary>
        public static string Sanitize(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            string escaped = Escape(text);

            foreach (string tag in AllowedTags)
            {
                escaped = Regex.Replace(escaped, $@"&lt;\s*{tag}\s*/?&gt;", $"<{tag}>", RegexOptions.IgnoreCase);
                escaped = Regex.Replace(escaped, $@"&lt;\s*/\s*{tag}\s*&gt;", $"</{tag}>", RegexOptions.IgnoreCase);
            }

            return escaped;
        }

        /// <summary>True when the text contains anything that would be interpreted by a browser.</summary>
        public static bool LooksLikeMarkup(string text)
            => !string.IsNullOrEmpty(text) && TagPattern.IsMatch(text);

        /// <summary>
        /// The tags a reviewer wants named when a note is quarantined: what the payload actually tried to do.
        /// Used for the log line, never to decide whether to render — that decision is
        /// "escape it", always.
        /// </summary>
        public static string DescribeMarkup(string text)
        {
            if (string.IsNullOrEmpty(text)) return "no markup";

            var found = new System.Collections.Generic.List<string>();
            foreach (Match m in TagPattern.Matches(text))
            {
                string tag = m.Groups[1].Value.ToLowerInvariant();
                if (!found.Contains(tag)) found.Add(tag);
            }

            bool handler = Regex.IsMatch(text, @"\son[a-z]+\s*=", RegexOptions.IgnoreCase);
            bool jsUrl = text.IndexOf("javascript:", StringComparison.OrdinalIgnoreCase) >= 0;

            string tags = found.Count == 0 ? "no tags" : "<" + string.Join(">, <", found) + ">";
            if (handler) tags += " + an inline event handler";
            if (jsUrl) tags += " + a javascript: URL";
            return tags;
        }
    }
}
