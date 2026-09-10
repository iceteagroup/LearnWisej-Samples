using System;
using System.Collections.Generic;
using System.Globalization;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// Everything culture-related a screen needs, behind one contract: which cultures ship, the current one,
    /// the text for a resource key (with a safe, traced fallback) and culture-aware formatting of dates,
    /// numbers and currency. Plain .NET (ResourceManager + CultureInfo), no Wisej.NET type: a unit test can
    /// assert "de-DE formats 1850 as 1.850,00 €" without a browser.
    /// </summary>
    public interface ILocalizationService
    {
        IReadOnlyList<CultureInfo> SupportedCultures { get; }

        CultureInfo Culture { get; }

        /// <summary>Switches the session's culture. An unsupported culture is an expected outcome: a failed result, not an exception.</summary>
        OperationResult<CultureInfo> SetCulture(string cultureName);

        /// <summary>Resolves a resource key for the current culture. A missing key never throws: it falls back and is traced.</summary>
        string Text(string key);

        /// <summary>Resolves a resource key and formats it with the culture (string.Format with culture-aware arguments).</summary>
        string Format(string key, params object[] args);

        string StatusText(WorkOrderStatus status);

        string FormatDate(DateTime value);
        string FormatDateTime(DateTime value);
        string FormatCurrency(decimal value);
        string FormatNumber(double value);
    }
}
