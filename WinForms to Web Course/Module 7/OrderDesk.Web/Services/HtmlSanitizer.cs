using System.Text.RegularExpressions;

namespace OrderDesk.Services
{
    /// <summary>
    /// The only way user-entered text may reach a control with AllowHtml = true: an allow-list sanitizer.
    /// Keeps a handful of formatting tags WITHOUT attributes (b, i, u, strong, em, br) and removes every
    /// other tag, every attribute (so no onerror/onclick/href) and every script/style block.
    /// Default Wisej.NET rendering encodes text; this is for the rare field where a little markup is wanted.
    /// </summary>
    public static class HtmlSanitizer
    {
        private static readonly Regex ScriptBlocks = new Regex(@"<\s*(script|style)[^>]*>.*?<\s*/\s*\1\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex Tags = new Regex(@"<\s*(/?)\s*([a-zA-Z][a-zA-Z0-9]*)[^>]*>", RegexOptions.Compiled);
        private static readonly string[] Allowed = { "b", "i", "u", "strong", "em", "br" };

        public static string Strip(string html)
        {
            if (string.IsNullOrEmpty(html)) return "";
            var text = ScriptBlocks.Replace(html, "");
            text = Tags.Replace(text, m =>
            {
                var closing = m.Groups[1].Value == "/";
                var tag = m.Groups[2].Value.ToLowerInvariant();
                if (System.Array.IndexOf(Allowed, tag) < 0) return "";           // unknown tag: dropped entirely
                if (tag == "br") return "<br>";
                return closing ? "</" + tag + ">" : "<" + tag + ">";             // allowed tag: kept, attributes dropped
            });
            return text;
        }

        /// <summary>Lists what <see cref="Strip"/> removed, for the trace.</summary>
        public static string Describe(string html)
        {
            if (string.IsNullOrEmpty(html)) return "nothing";
            int removedTags = 0, keptTags = 0, attributes = 0;
            foreach (Match m in Tags.Matches(html))
            {
                var tag = m.Groups[2].Value.ToLowerInvariant();
                if (System.Array.IndexOf(Allowed, tag) < 0) removedTags++; else keptTags++;
                attributes += Regex.Matches(m.Value, @"\s[a-zA-Z-]+\s*=").Count;
            }
            return "kept " + keptTags + " formatting tag(s), removed " + removedTags + " tag(s) and " + attributes + " attribute(s)";
        }
    }
}
