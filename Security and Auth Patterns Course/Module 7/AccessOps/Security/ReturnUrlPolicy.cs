using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessOps.Security
{
    /// <summary>
    /// Continuation URLs are input. A <c>returnUrl</c> is accepted only when it is a local path -
    /// it starts with one "/" and not with "//" or "/\" (both mean "another host" to a browser) -
    /// or when it is exactly an entry of the server's allowlist. Anything else becomes "/".
    /// </summary>
    public static class ReturnUrlPolicy
    {
        /// <summary>Where every refused continuation goes.</summary>
        public const string Fallback = "/";

        /// <summary>The URL to continue to: <paramref name="returnUrl"/> when allowed, otherwise "/".</summary>
        public static string Resolve(string returnUrl, IEnumerable<string> allowlist = null)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
                return Fallback;

            // control characters (tab, CR, LF) are stripped by browsers and turn "/\t/evil" into "//evil"
            if (returnUrl.Any(char.IsControl))
                return Fallback;

            if (IsLocalPath(returnUrl))
                return returnUrl;

            if (allowlist != null && allowlist.Contains(returnUrl, StringComparer.Ordinal))
                return returnUrl;

            return Fallback;
        }

        /// <summary>True for "/path", false for "//host", "/\host", "https://host" or "path".</summary>
        public static bool IsLocalPath(string url)
        {
            if (string.IsNullOrEmpty(url) || url[0] != '/')
                return false;
            if (url.Length == 1)
                return true;
            return url[1] != '/' && url[1] != '\\';
        }
    }
}
