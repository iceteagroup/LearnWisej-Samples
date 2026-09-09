using System;
using System.Threading.Tasks;
using SupportDesk.Data.Diagnostics;
using SupportDesk.Services;
using Wisej.Services;
using Wisej.Web;

namespace SupportDesk.Web
{
    /// <summary>
    /// Module 1 lab page — the first async query.
    ///
    /// Left card:  the lab controls (countButton, statusLabel), the friendly error banner and a live
    ///             table of the four lifetimes (session, UI object, request, unit of work).
    /// Right card: everything EF Core does inside one click — context created, SQL, context disposed.
    /// Bottom bar: progress path (slow count / three rapid clicks against the loading guard),
    ///             failure path (outage → friendly message), recovery, and the shared-context anti-pattern.
    ///
    /// The page is session state: it lives on the server for as long as the browser tab does.
    /// The services it holds are stateless; every DbContext is created and disposed inside one handler.
    /// </summary>
    public partial class TicketBrowserPage : Page
    {
        // Resolved through Microsoft DI: Startup.cs registered app.Services with Wisej.NET.
        [Inject]
        private TicketQueryService TicketQueries { get; set; }

        [Inject]
        private SharedContextAntiPattern AntiPattern { get; set; }

        [Inject]
        private DevelopmentOutageSwitch Outage { get; set; }

        private readonly DateTime _createdAt = DateTime.Now;
        private readonly object _traceLock = new object();

        // The loading guard: one database operation per page at a time.
        private bool _loading;

        private int _handlerRuns;
        private int _contextsCreated;
        private int _contextsDisposed;

        public TicketBrowserPage()
        {
            InitializeComponent();
        }

        private void TicketBrowserPage_Load(object sender, EventArgs e)
        {
            AddTrace(Glyph.Server, "session",
                $"page created for session {ShortSessionId()} · TicketQueryService through [Inject] → " +
                (TicketQueries != null ? "resolved from Microsoft DI" : "NULL — the IServiceProvider bridge is missing"));
            UpdateLifetimes();
            SetState("● idle", StateKind.Ok);
        }

        #region Success path: the lab handler

        private async void countButton_Click(object sender, EventArgs e)
        {
            await CountAsync(TimeSpan.Zero, "countButton_Click");
        }

        /// <summary>
        /// The pattern of the whole course: guard → busy UI → one awaited service call → result,
        /// friendly message in catch, UI restored in finally. No DbContext is visible here at all.
        /// </summary>
        private async Task CountAsync(TimeSpan latency, string origin)
        {
            _handlerRuns++;

            if (_loading)
            {
                AddTrace(Glyph.Server, "guard", $"{origin}: a count is already running — this click is ignored");
                UpdateLifetimes();
                return;
            }

            using var trace = QueryTrace.Begin(OnTrace);
            try
            {
                _loading = true;
                SetBusy(true);
                HideBanner();
                SetState("● counting", StateKind.Normal);
                statusLabel.Text = latency > TimeSpan.Zero
                    ? $"Counting tickets… (simulated {latency.TotalSeconds:0.#} s latency)"
                    : "Counting tickets…";
                AddTrace(Glyph.Server, origin, latency > TimeSpan.Zero
                    ? $"TicketQueryService.CountTicketsSlowlyAsync({latency.TotalSeconds:0.#} s)"
                    : "TicketQueryService.CountTicketsAsync()");

                var count = latency > TimeSpan.Zero
                    ? await TicketQueries.CountTicketsSlowlyAsync(latency)
                    : await TicketQueries.CountTicketsAsync();

                statusLabel.Text = $"{count} tickets in the Support Desk database";
                AddTrace(Glyph.Result, "result", $"{count} tickets · {trace.Commands} statement(s) · {trace.Milliseconds:0.0} ms in the database · {trace.ContextsCreated} context created, {trace.ContextsDisposed} disposed");
                SetState("● ok", StateKind.Ok);
            }
            catch (DatabaseUnavailableException ex)
            {
                Fail("The Support Desk database is not reachable right now. Nothing was changed — please try again in a moment.", ex);
            }
            catch (Exception ex)
            {
                Fail("The ticket count is not available right now. Please try again in a moment.", ex);
            }
            finally
            {
                _loading = false;
                SetBusy(false);
                UpdateLifetimes();
                Application.Update(this);
            }
        }

        #endregion

        #region Progress path: the loading guard

        private async void buttonSlowCount_Click(object sender, EventArgs e)
        {
            await CountAsync(TimeSpan.FromSeconds(2.5), "buttonSlowCount_Click");
        }

        private async void buttonRapid_Click(object sender, EventArgs e)
        {
            // Three counts requested in one go: the first one runs, the guard drops the other two.
            var first = CountAsync(TimeSpan.FromSeconds(2.5), "rapid click 1");
            await CountAsync(TimeSpan.Zero, "rapid click 2");
            await CountAsync(TimeSpan.Zero, "rapid click 3");
            await first;
        }

        #endregion

        #region Failure path and recovery: the database goes away

        private async void buttonBreak_Click(object sender, EventArgs e)
        {
            Outage.IsDown = true;
            AddTrace(Glyph.Server, "outage", "DevelopmentOutageSwitch.IsDown = true — every connection open now fails (lab prop, development only)");
            await CountAsync(TimeSpan.Zero, "buttonBreak_Click");
        }

        private async void buttonRestore_Click(object sender, EventArgs e)
        {
            Outage.IsDown = false;
            AddTrace(Glyph.Server, "outage", "DevelopmentOutageSwitch.IsDown = false — the next operation gets a fresh context and a working connection");
            await CountAsync(TimeSpan.Zero, "buttonRestore_Click");
        }

        #endregion

        #region Anti-pattern: what a shared DbContext does

        private async void buttonAntiPattern_Click(object sender, EventArgs e)
        {
            AddTrace(Glyph.Server, "anti-pattern", "one context, two concurrent CountAsync calls — what a static/shared DbContext does when two sessions use it");
            using var trace = QueryTrace.Begin(OnTrace);
            try
            {
                await AntiPattern.RunTwoOperationsOnOneContextAsync();
                AddTrace(Glyph.Server, "anti-pattern", "no exception this time: the first count finished before the second started — the race is timing-dependent, which is exactly why it is unsafe");
                SetState("● anti-pattern: race not hit", StateKind.Warn);
            }
            catch (InvalidOperationException ex)
            {
                AddTrace(Glyph.Server, "caught", $"InvalidOperationException: {FirstSentence(ex.Message)}");
                ShowBanner("EF Core refused: a second operation was started on the same DbContext before the first completed. A static or shared context makes every user run into this.");
                SetState("● anti-pattern caught", StateKind.Warn);
            }
            catch (Exception ex)
            {
                Fail("The demo could not run.", ex);
            }
            finally
            {
                UpdateLifetimes();
                Application.Update(this);
            }
        }

        #endregion

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.listTrace.Items.Clear();
        }

        #region Helpers

        private void Fail(string friendlyMessage, Exception ex)
        {
            statusLabel.Text = "Count unavailable";
            ShowBanner(friendlyMessage);
            AddTrace(Glyph.Server, "caught", $"{ex.GetType().Name}: {FirstSentence(ex.Message)} → friendly message shown, full exception logged server-side");
            SetState("● fault", StateKind.Error);
        }

        private void SetBusy(bool busy)
        {
            this.countButton.Enabled = !busy;
            this.buttonSlowCount.Enabled = !busy;
            this.buttonRapid.Enabled = !busy;
        }

        /// <summary>Receives the EF Core trace for the current operation (see QueryTrace).</summary>
        private void OnTrace(TraceEntry entry)
        {
            switch (entry.Kind)
            {
                case TraceKind.Context:
                    if (entry.Text.Contains("created")) _contextsCreated++;
                    if (entry.Text.Contains("disposed")) _contextsDisposed++;
                    AddTrace(Glyph.Context, "context", entry.Text);
                    break;
                case TraceKind.Command:
                    AddTrace(Glyph.Sql, "SQL", $"{entry.Text}   ({entry.Milliseconds:0.0} ms)");
                    break;
                case TraceKind.Failure:
                    AddTrace(Glyph.Sql, "SQL failed", entry.Text);
                    break;
            }
        }

        private enum Glyph { Server, Context, Sql, Result }

        private void AddTrace(Glyph glyph, string label, string text)
        {
            var symbol = glyph switch
            {
                Glyph.Server => "•",
                Glyph.Context => "◦",
                Glyph.Sql => "→",
                _ => "←"
            };

            lock (_traceLock)
            {
                this.listTrace.Items.Add($"{DateTime.Now:HH:mm:ss.fff}  {symbol} {label,-12} {text}");
                this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
            }
        }

        private void UpdateLifetimes()
        {
            this.labelLifetimes.Text =
                "lifetime           owner            this session, right now\n" +
                "session state      Wisej.NET        id " + ShortSessionId() + " · started " + _createdAt.ToString("HH:mm:ss") + "\n" +
                "UI object          this Page        1 page, its labels and this trace list — alive until the tab closes\n" +
                "request / thread   one click        " + _handlerRuns + " handler run(s) · after an await the continuation may resume on any thread\n" +
                "unit of work       DbContext        " + _contextsCreated + " created · " + _contextsDisposed + " disposed · " + (_contextsCreated - _contextsDisposed) + " alive between clicks";
        }

        private enum StateKind { Normal, Ok, Warn, Error }

        private void SetState(string text, StateKind kind)
        {
            this.labelState.Text = text;
            this.labelState.ForeColor = kind switch
            {
                StateKind.Ok => System.Drawing.Color.FromArgb(31, 157, 87),
                StateKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                StateKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                _ => System.Drawing.Color.FromArgb(90, 107, 125)
            };
        }

        private void ShowBanner(string text)
        {
            this.labelBanner.Text = text;
            this.labelBanner.Visible = true;
        }

        private void HideBanner()
        {
            this.labelBanner.Visible = false;
        }

        private static string ShortSessionId()
        {
            var id = Application.SessionId ?? "";
            return id.Length > 8 ? id.Substring(0, 8) : id;
        }

        private static string FirstSentence(string message)
        {
            var i = message.IndexOf(". ", StringComparison.Ordinal);
            return i > 0 ? message.Substring(0, i + 1) : message;
        }

        #endregion
    }
}
