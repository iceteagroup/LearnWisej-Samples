using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;

namespace TicketOps.Services
{
    /// <summary>
    /// The one place that knows which cultures the console ships and how a resource key becomes text.
    ///
    /// Text:        Resources/Strings.resx (neutral, English) + Strings.de.resx (German) through the
    ///              ResourceManager in <see cref="Strings"/>. The .NET fallback chain is de-DE → de → neutral,
    ///              so a key that is missing in German shows English (logged as untranslated) and a key that
    ///              is missing everywhere shows "[key]" (logged as missing). The user always sees SOMETHING
    ///              readable; the log tells the team what to fix.
    /// Formatting:  every date/number/currency goes through the session's CultureInfo explicitly —
    ///              6/14/2026 · $1,850.00 for en-US, 14.06.2026 · 1.850,00 € for de-DE — from the same value.
    ///
    /// Per session by construction (created in AppComposition); nothing static, because two operators may
    /// work in two languages at the same time.
    /// </summary>
    public sealed class LocalizationService : ILocalizationService
    {
        private static readonly string[] SupportedCultureNames = { "en-US", "de-DE" };

        private readonly SessionContext _session;
        private readonly ILog _log;
        private CultureInfo _culture;

        public LocalizationService(SessionContext session, ILog log)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            SupportedCultures = SupportedCultureNames.Select(CultureInfo.GetCultureInfo).ToList();
            _culture = CultureInfo.GetCultureInfo(_session.Culture ?? SupportedCultureNames[0]);
            ApplyToThread();
        }

        public IReadOnlyList<CultureInfo> SupportedCultures { get; }

        public CultureInfo Culture => _culture;

        public OperationResult<CultureInfo> SetCulture(string cultureName)
        {
            var requested = SupportedCultures.FirstOrDefault(c => string.Equals(c.Name, cultureName, StringComparison.OrdinalIgnoreCase));
            if (requested == null)
            {
                // Expected outcome: the operator picked a language we do not ship. Stay where we are and say so
                // (in the CURRENT culture — the requested one has no strings).
                return OperationResult<CultureInfo>.Fail(Format("Rule.CultureNotShipped", cultureName, _culture.Name));
            }

            _culture = requested;
            _session.Culture = requested.Name;
            ApplyToThread();
            return OperationResult<CultureInfo>.Ok(requested, Format("Message.CultureApplied", requested.NativeName));
        }

        public string Text(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            string text = Strings.ResourceManager.GetString(key, _culture);     // de-DE → de → neutral (English)
            if (text == null)
            {
                // Nothing to show for this key in any culture. Fall back to the key itself so the screen stays
                // readable and the gap is visible to the team, never an exception to the operator.
                _log.Warn(LogLayer.Service, "LocalizationService.Text", $"missing: key '{key}' has no value in {_culture.Name} nor in the neutral resources → fallback \"[{key}]\"");
                return "[" + key + "]";
            }

            if (!IsNeutral(_culture) && !HasOwnValue(key, _culture))
                _log.Warn(LogLayer.Service, "LocalizationService.Text", $"untranslated: key '{key}' missing in {_culture.Name} → neutral (English) value \"{text}\"");

            return text;
        }

        public string Format(string key, params object[] args)
        {
            string pattern = Text(key);
            try
            {
                return string.Format(_culture, pattern, args ?? new object[0]);
            }
            catch (FormatException ex)
            {
                _log.Error(LogLayer.Service, "LocalizationService.Format", ex, $"pattern for '{key}' is malformed — shown raw");
                return pattern;
            }
        }

        public string StatusText(WorkOrderStatus status) => Text("Status." + status);

        public string FormatDate(DateTime value) => value.ToString("d", _culture);
        public string FormatDateTime(DateTime value) => value.ToString("g", _culture);
        public string FormatCurrency(decimal value) => value.ToString("C", _culture);
        public string FormatNumber(double value) => value.ToString("N1", _culture);

        /// <summary>
        /// Sets the current request thread's cultures so that typed accessors (Strings.ActionFailed) and any
        /// ToString() without an explicit culture agree with the session. Wisej.NET carries the session culture
        /// across requests through Application.CurrentCulture, which the screen sets right after this call.
        /// </summary>
        private void ApplyToThread()
        {
            Thread.CurrentThread.CurrentCulture = _culture;
            Thread.CurrentThread.CurrentUICulture = _culture;
        }

        private static bool IsNeutral(CultureInfo culture) => culture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase);

        /// <summary>True if the key has a value in the culture's own resource set or its parent (de-DE → de), i.e. it is really translated.</summary>
        private static bool HasOwnValue(string key, CultureInfo culture)
        {
            for (var c = culture; c != null && !c.Equals(CultureInfo.InvariantCulture); c = c.Parent)
            {
                var set = Strings.ResourceManager.GetResourceSet(c, true, false);
                if (set != null && set.GetString(key) != null)
                    return true;
            }
            return false;
        }
    }
}
