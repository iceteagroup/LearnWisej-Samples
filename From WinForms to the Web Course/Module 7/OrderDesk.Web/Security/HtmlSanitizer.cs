using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace OrderDesk.Security
{
    /// <summary>What the sanitizer did to one input — the console shows it next to the result.</summary>
    public sealed class SanitizeResult
    {
        public string Html { get; set; }
        public int TagsKept { get; set; }
        public int TagsRemoved { get; set; }
        public List<string> Removed { get; } = new List<string>();
    }

    /// <summary>
    /// A whitelist sanitizer for the one place OrderDesk renders user-entered HTML (order notes with bold,
    /// italic and line breaks). Everything not on the list is removed — tag, attributes, event handlers,
    /// styles — and all text is HTML-encoded. Wisej.NET encodes Label.Text by default; this is the only
    /// path allowed to set AllowHtml = true on user data.
    /// </summary>
    public static class HtmlSanitizer
    {
        private static readonly HashSet<string> Whitelist = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "b", "i", "br" };

        // <tag ...>, </tag>, <tag/>; group 1 = "/", group 2 = name, group 3 = attributes / trailing slash.
        private static readonly Regex TagPattern = new Regex(@"<(/?)([a-zA-Z][a-zA-Z0-9]*)([^>]*)>", RegexOptions.Compiled);

        /// <summary>The default and safest option: no markup survives. Same as AllowHtml = false.</summary>
        public static string Encode(string text) => WebUtility.HtmlEncode(text ?? "");

        /// <summary>Keeps &lt;b&gt;, &lt;i&gt; and &lt;br&gt; without attributes; encodes and strips everything else.</summary>
        public static SanitizeResult Sanitize(string input)
        {
            var result = new SanitizeResult();
            var text = input ?? "";
            var sb = new StringBuilder();
            int position = 0;
            foreach (Match match in TagPattern.Matches(text))
            {
                sb.Append(WebUtility.HtmlEncode(text.Substring(position, match.Index - position)));
                position = match.Index + match.Length;

                string name = match.Groups[2].Value;
                string attributes = match.Groups[3].Value.Trim().TrimEnd('/').Trim();
                bool allowed = Whitelist.Contains(name) && attributes.Length == 0;
                if (allowed)
                {
                    result.TagsKept++;
                    bool isBreak = name.Equals("br", StringComparison.OrdinalIgnoreCase);
                    sb.Append('<').Append(match.Groups[1].Value).Append(name.ToLowerInvariant()).Append(isBreak ? "/>" : ">");
                }
                else
                {
                    result.TagsRemoved++;
                    result.Removed.Add(match.Value);
                }
            }
            sb.Append(WebUtility.HtmlEncode(text.Substring(position)));
            result.Html = sb.ToString();
            return result;
        }
    }
}
