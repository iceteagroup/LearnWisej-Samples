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
    /// Everything here reads <c>Application.CurrentCulture</c>, which is per session. Nothing in
    /// this class is static state, and that is deliberate - see Module 4's note.
    /// </summary>
    public static class LocalizationService
    {
        private static readonly ResourceManager Resources =
            new ResourceManager("GlobalDesk.Resources.Strings", typeof(LocalizationService).Assembly);

        /// <summary>
        /// Every key the application asked for and did not get, for this session. The UI can show
        /// it; a production build would send the same list to the log.
        /// </summary>
        private static readonly List<string> MissingKeys = new List<string>();

        /// <summary>
        /// The missing-key policy, in one flag.
        ///
        /// <b>Development</b> - return the key in brackets, so a gap is visible on screen and a
        /// tester can describe it: <c>[Ticket.Waiting]</c>.
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

        // ── text ────────────────────────────────────────────────────────────────

        /// <summary>The text for <paramref name="key"/> under the session's culture.</summary>
        public static string Text(string key)
        {
            var value = Resources.GetString(key);
            if (value != null)
                return value;

            if (!MissingKeys.Contains(key))
                MissingKeys.Add(key);

            return DevelopmentMode ? $"[{key}]" : key;
        }

        /// <summary>
        /// A composed sentence. The culture goes to <c>string.Format</c> as well as to whatever
        /// formatted the arguments - without it, <c>string.Format</c> falls back to the thread's
        /// culture, which in a server application is whatever that thread last did.
        /// </summary>
        public static string Text(string key, params object[] args) =>
            string.Format(Application.Culture, Text(key), args);

        // ── values ──────────────────────────────────────────────────────────────

        /// <summary>A date, in the session's culture. "D" is long, "d" is short.</summary>
        public static string Date(DateTime value, string format = "D") =>
            value.ToString(format, Application.Culture);

        /// <summary>A number. "N0" for whole units, "N2" for two decimals.</summary>
        public static string Number(IFormattable value, string format = "N0") =>
            value.ToString(format, Application.Culture);

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
            value.ToString("C", Application.Culture);

        /// <summary>The culture every method above uses, in one place.</summary>
        private static class Application
        {
            public static CultureInfo Culture => Wisej.Web.Application.CurrentCulture;
        }
    }
}
