using System;
using System.Text.Json;
using System.Threading.Tasks;
using Wisej.Web;

namespace TicketOps.Infrastructure
{
    /// <summary>What the browser answered when asked to copy: confirmed, or refused with a short reason.</summary>
    public sealed class ClipboardOutcome
    {
        public bool Copied { get; }
        public string Reason { get; }
        public string Detail { get; }

        private ClipboardOutcome(bool copied, string reason, string detail)
        {
            Copied = copied;
            Reason = reason;
            Detail = detail;
        }

        public static ClipboardOutcome Confirmed() => new ClipboardOutcome(true, null, null);

        public static ClipboardOutcome Refused(string reason, string detail) => new ClipboardOutcome(false, reason ?? "Error", detail ?? "");

        /// <summary>
        /// Reads the object the script resolved: <c>{ ok: true }</c> or <c>{ ok: false, reason, detail }</c>.
        /// Anything else (null, an unexpected shape) is treated as "not confirmed" — the server never assumes success.
        /// </summary>
        public static ClipboardOutcome From(object answer)
        {
            if (answer == null)
                return Refused("NoAnswer", "The browser returned nothing.");

            try
            {
                dynamic d = answer;
                bool ok = false;
                try { ok = Convert.ToBoolean(d.ok); } catch (Exception) { ok = false; }
                if (ok)
                    return Confirmed();

                string reason = null, detail = null;
                try { reason = Convert.ToString(d.reason); } catch (Exception) { reason = null; }
                try { detail = Convert.ToString(d.detail); } catch (Exception) { detail = null; }
                return Refused(string.IsNullOrEmpty(reason) ? "Unconfirmed" : reason, detail);
            }
            catch (Exception ex)
            {
                return Refused("UnreadableAnswer", ex.GetType().Name);
            }
        }

        public override string ToString() => Copied ? "copied" : $"not copied ({Reason})";
    }

    /// <summary>
    /// The one place that talks to browser-only APIs ("keep browser calls in one place"). Screens call a
    /// typed method; this class owns the script string, the escaping and the "the browser said no" path.
    ///
    /// Clipboard: the server already owns the text, so nothing coming back needs trusting — only the
    /// yes/no. <c>navigator.clipboard.writeText</c> needs a recent user gesture and a secure context
    /// (https or localhost); when it rejects, the screen shows a fallback instead of failing silently.
    /// <see cref="SimulateClipboardDenied"/> makes the script reject with NotAllowedError so the lab can
    /// show that path on demand.
    /// </summary>
    public sealed class BrowserApi
    {
        private readonly ILog _log;

        public BrowserApi(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        /// <summary>Lab switch: when true, the client script rejects the write as the browser would when permission is denied.</summary>
        public bool SimulateClipboardDenied { get; set; }

        /// <summary>
        /// Interop point: server → browser → server. Asks the browser to copy <paramref name="text"/> and awaits its
        /// answer. Runs <c>ticketOps.copyToClipboard(text, options)</c> from the embedded script (Platform/ticketops.interop.js);
        /// the expression returns a Promise that always resolves to <c>{ ok, reason, detail }</c>, and
        /// <see cref="Application.EvalAsync"/> hands the resolved value back here.
        /// </summary>
        public async Task<ClipboardOutcome> CopyToClipboardAsync(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            // Escaping: the text becomes a JavaScript string literal through the JSON serializer — quotes,
            // backslashes, line breaks and < > & are escaped, so nothing in it can become script.
            string literal = JsonSerializer.Serialize(text);
            string options = SimulateClipboardDenied ? "{ simulateDenied: true }" : "{}";
            string expression = "ticketOps.copyToClipboard(" + literal + ", " + options + ")";

            _log.Info(LogLayer.Client, "BrowserApi.CopyToClipboardAsync",
                $"→ ticketOps.copyToClipboard({text.Length} chars{(SimulateClipboardDenied ? ", simulateDenied" : "")}) — Application.EvalAsync, awaiting the browser");

            // Application.EvalAsync takes an EXPRESSION (never "return …"); a returned Promise is awaited.
            dynamic answer = await Application.EvalAsync(expression);
            var outcome = ClipboardOutcome.From((object)answer);

            if (outcome.Copied)
                _log.Info(LogLayer.Client, "navigator.clipboard.writeText", "resolved → ok (the browser confirmed the copy)");
            else
                _log.Warn(LogLayer.Client, "navigator.clipboard.writeText", $"rejected: {outcome.Reason} — {outcome.Detail}");

            return outcome;
        }
    }
}
