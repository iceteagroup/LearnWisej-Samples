using System;
using System.Collections.Generic;
using System.Globalization;
using Wisej.Resources;

namespace GlobalDesk
{
    /// <summary>
    /// The one place the application asks for a piece of text.
    ///
    /// <see cref="ResourceManager"/> does the culture work. It reads
    /// <c>Application.CurrentCulture</c> - which is per session - and walks .NET's fallback chain:
    /// the most specific resource first (<c>fr-CA</c>), then the language (<c>fr</c>), then the
    /// neutral file. The first value it finds wins, which is why the neutral file must be
    /// complete: it is the last place the search looks.
    /// </summary>
    public static class Texts
    {
        private static readonly ResourceManager Resources =
            new ResourceManager("GlobalDesk.Resources.Strings", typeof(Texts).Assembly);

        private static readonly HashSet<string> Resolved = new HashSet<string>();
        private static readonly HashSet<string> Missing = new HashSet<string>();

        /// <summary>
        /// The keys whose value is the same in every language on purpose - product and
        /// third-party names, a literal format, and the entries in a language list, which every
        /// product shows in the language they name.
        ///
        /// This mirrors the <c>&lt;metadata name="Key.IsInvariant"&gt;</c> entries the neutral
        /// <c>.resx</c> carries for the translation tool. The tool uses those to keep the keys
        /// away from translators; this set keeps the same keys out of the untranslated report,
        /// which would otherwise call every one of them a gap.
        /// </summary>
        private static readonly HashSet<string> Invariant = new HashSet<string>
        {
            "App.ProductName", "Product.Framework", "Format.CustomerCode",
            "Language.en-US", "Language.de-DE", "Language.it-IT",
            "Language.fr-CA", "Language.ar-SA", "Language.qps-ploc",
        };

        /// <summary>
        /// Returns the text for <paramref name="key"/>, or the key in brackets if there is none.
        ///
        /// An empty string would hide a missing key behind a blank caption that nobody reports;
        /// <c>[Dashboard.Welcome]</c> on screen is a defect anyone can see and describe.
        /// </summary>
        public static string Get(string key)
        {
            var value = Resources.GetString(key);
            if (value == null)
            {
                Missing.Add(key);
                return "[" + key + "]";
            }

            Resolved.Add(key);
            return value;
        }

        public static int ResolvedCount => Resolved.Count;

        public static int MissingCount => Missing.Count;

        /// <summary>
        /// The untranslated filter, asked of the running application instead of the grid: every
        /// key this session has looked up whose value under the current culture is still the
        /// neutral one.
        ///
        /// It is a developer's report, not a user's. It answers the question the translation round
        /// is measured by - <i>how many keys are still English?</i> - on the screen rather than in
        /// the tool, which matters because fallback is silent: a key nobody translated renders in
        /// English and the page looks finished.
        ///
        /// Two things it cannot see, both by design. Keys this session never asked for are not in
        /// the list, because the list is built from real lookups. And a value a translator
        /// legitimately left identical - <i>Name</i> in German - looks exactly like one nobody
        /// touched, which is what the invariant markers and a human review are for.
        /// </summary>
        public static IReadOnlyList<string> UntranslatedKeys()
        {
            var culture = Wisej.Web.Application.CurrentCulture;
            var gaps = new List<string>();

            if (culture.TwoLetterISOLanguageName == "en")
                return gaps;   // the neutral file IS this culture's file

            foreach (var key in Resolved)
            {
                if (Invariant.Contains(key))
                    continue;

                var translated = Resources.GetString(key, culture);
                var neutral = Resources.GetString(key, CultureInfo.InvariantCulture);

                if (translated != null && neutral != null &&
                    string.Equals(translated, neutral, StringComparison.Ordinal))
                {
                    gaps.Add(key);
                }
            }

            gaps.Sort(StringComparer.Ordinal);
            return gaps;
        }
    }
}
