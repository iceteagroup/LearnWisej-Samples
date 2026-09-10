using System;
using System.Collections.Generic;
using System.Net;

namespace TicketOps.Security
{
    /// <summary>
    /// The safe text/HTML policy of the TicketOps Console (deliverable: docs/SafeHtmlPolicy.md).
    ///
    /// 1. Default: user text is rendered as TEXT. Labels keep <c>AllowHtml = false</c> (the Wisej.NET default),
    ///    so the framework escapes the string and a note containing "&lt;img onerror=…&gt;" shows those characters.
    /// 2. Any HTML-capable surface that must show user text (a label with AllowHtml = true, a tooltip, a grid
    ///    cell with HTML enabled) receives <see cref="Encode"/>d text. Surfaces that already escape — a Label with
    ///    AllowHtml = false, the trace/audit ListBox (verified at runtime) — receive plain text: encoding twice shows entities.
    /// 3. When formatting is genuinely required, <see cref="RenderWithAllowList"/> encodes EVERYTHING first and
    ///    then restores only the tags in <see cref="AllowedTags"/> — literal, attribute-free tags, so no
    ///    "onerror", no "href", no "style" can ever come back.
    ///
    /// Every <c>AllowHtml = true</c> in the code base is a reviewed decision with a comment saying why.
    /// </summary>
    public static class HtmlPolicy
    {
        /// <summary>The whole allow-list. Adding a tag here is a security review item.</summary>
        public static readonly IReadOnlyList<string> AllowedTags = new[] { "b", "i", "br" };

        /// <summary>Encode on output: '&lt;', '&gt;', '&amp;', quotes become entities, so markup displays instead of executing.</summary>
        public static string Encode(string userText)
            => userText == null ? string.Empty : WebUtility.HtmlEncode(userText);

        /// <summary>
        /// Encodes the text, then re-enables only the exact literal tags of the allow-list
        /// ("&lt;b&gt;", "&lt;/b&gt;", "&lt;i&gt;", "&lt;/i&gt;", "&lt;br&gt;"). A tag with attributes, a different case
        /// or any other element stays encoded, i.e. inert text.
        /// </summary>
        public static string RenderWithAllowList(string userText)
        {
            string safe = Encode(userText);
            foreach (var tag in AllowedTags)
            {
                safe = safe.Replace($"&lt;{tag}&gt;", $"<{tag}>", StringComparison.Ordinal);
                safe = safe.Replace($"&lt;/{tag}&gt;", $"</{tag}>", StringComparison.Ordinal);
            }
            return safe;
        }

        /// <summary>True when the text contains something that would be interpreted as markup on an HTML-capable surface.</summary>
        public static bool LooksLikeMarkup(string userText)
            => !string.IsNullOrEmpty(userText) && (userText.IndexOf('<') >= 0 || userText.IndexOf('&') >= 0);
    }
}
