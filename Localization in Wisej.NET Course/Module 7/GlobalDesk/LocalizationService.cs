using System;
using System.Collections.Generic;
using System.Globalization;
using Wisej.Resources;

namespace GlobalDesk
{
    /// <summary>
    /// The only place the application touches a <see cref="ResourceManager"/> or a
    /// <see cref="CultureInfo"/>.
    ///
    /// Six modules produced six habits - <c>Texts.Get</c>, <c>ToString("D", culture)</c>,
    /// <c>string.Format(culture, ...)</c> - and the capstone collapses them into one surface. The
    /// value is not tidiness. It is that every localization decision now has exactly one place to
    /// be made, audited and changed: what a missing key does, which culture is used, whether a
    /// lookup is recorded.
    ///
    /// Everything here reads <c>Application.CurrentCulture</c> at the moment of the call, which is
    /// per session. Nothing in this class caches a culture, and that is deliberate - see Module 4.
    /// </summary>
    public static class LocalizationService
    {
        private static readonly ResourceManager Resources =
            new ResourceManager("GlobalDesk.Resources.Strings", typeof(LocalizationService).Assembly);

        /// <summary>Keys this session asked for and got, used by the untranslated report.</summary>
        private static readonly HashSet<string> Resolved = new HashSet<string>();

        /// <summary>
        /// Every key the application asked for and did not get, for this session. The QA
        /// checklist reads it; a production build would send the same list to the log.
        /// </summary>
        private static readonly List<string> MissingKeys = new List<string>();

        /// <summary>
        /// The keys whose value is the same in every language on purpose. Mirrors the
        /// <c>IsInvariant</c> metadata the neutral <c>.resx</c> carries for the translation tool.
        /// </summary>
        private static readonly HashSet<string> Invariant = new HashSet<string>
        {
            "App.ProductName", "Product.Framework", "Format.CustomerCode",
            "Language.en-US", "Language.de-DE", "Language.it-IT",
            "Language.fr-CA", "Language.ar-SA", "Language.qps-ploc",
        };

        /// <summary>
        /// The missing-key policy, in one flag.
        ///
        /// <b>Development</b> - return the key in brackets, so a gap is visible on screen and a
        /// tester can describe it: <c>[TicketStatus.Closed]</c>.
        ///
        /// <b>Production</b> - return the key itself without brackets and record it. A user should
        /// never be shown square brackets, but a blank caption is worse: it reads as a design
        /// choice and nobody reports it. The key at least tells a support engineer what to search
        /// for, and the log is what actually gets the gap fixed.
        ///
        /// What this must never do is return an empty string or throw. A missing translation is a
        /// content defect, not a crash, and one absent key must not take a page down.
        /// </summary>
        public static bool DevelopmentMode { get; set; } = true;

        public static IReadOnlyList<string> MissingKeysThisSession => MissingKeys;

        public static void ClearMissingKeys() => MissingKeys.Clear();

        /// <summary>The culture every method here uses, read at call time, never cached.</summary>
        private static CultureInfo Culture => Wisej.Web.Application.CurrentCulture;

        // ── text ────────────────────────────────────────────────────────────────

        /// <summary>The text for <paramref name="key"/> under the session's culture.</summary>
        public static string Text(string key)
        {
            var value = Resources.GetString(key);
            if (value != null)
            {
                Resolved.Add(key);
                return value;
            }

            if (!MissingKeys.Contains(key))
                MissingKeys.Add(key);

            return DevelopmentMode ? "[" + key + "]" : key;
        }

        /// <summary>
        /// A composed sentence. The culture goes to <c>string.Format</c> as well as to whatever
        /// formatted the arguments - without it, <c>string.Format</c> falls back to the thread's
        /// culture, which in a server application is whatever that thread last did.
        /// </summary>
        public static string Text(string key, params object[] args) =>
            string.Format(Culture, Text(key), args);

        // ── values ──────────────────────────────────────────────────────────────

        /// <summary>A date, in the session's culture. "D" is long, "d" is short.</summary>
        public static string Date(DateTime value, string format = "d") =>
            value.ToString(format, Culture);

        /// <summary>A number. "N0" for whole units, "N2" for two decimals, "P1" for a percentage.</summary>
        public static string Number(IFormattable value, string format = "N0") =>
            value.ToString(format, Culture);

        /// <summary>
        /// An amount, with the session culture's currency symbol and placement.
        ///
        /// Worth being explicit about what this does and does not do: it formats the number the
        /// way the reader expects to see money written. It does <b>not</b> convert currencies. An
        /// amount in euros shown to a US session renders as <c>$1,850.75</c> - the right shape and
        /// the wrong currency. If an application handles more than one currency, the currency is
        /// part of the data and the format has to carry it explicitly.
        /// </summary>
        public static string Currency(decimal value) =>
            value.ToString("C", Culture);

        // ── the QA reports ──────────────────────────────────────────────────────

        /// <summary>
        /// Keys this session looked up whose value under the current culture is still the neutral
        /// one - the untranslated filter, asked of the running application instead of the tool.
        ///
        /// A missing translation and a missing key are different failures and neither report sees
        /// the other: this one finds a key that exists but was never translated, and
        /// <see cref="MissingKeysThisSession"/> finds a key nobody ever wrote. Run both.
        /// </summary>
        public static IReadOnlyList<string> UntranslatedKeys()
        {
            var culture = Culture;
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
