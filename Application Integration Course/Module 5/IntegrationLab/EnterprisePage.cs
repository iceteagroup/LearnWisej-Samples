using System;
using System.Collections.Generic;
using System.Globalization;
using IntegrationLab.Controls;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// Operations Dashboard — Module 5 lab page.
    ///
    /// Every tile is the same custom control: IntegrationLab.Controls.SimpleGaugeControl
    /// (server class) rendered by integrationlab.controls.SimpleGaugeControl (client qx class)
    /// and styled by the "simplegauge" appearance key. No per-screen InitScript, no Options
    /// bag: typed properties in, .NET events out.
    ///
    /// Right card:  every message that crosses the wire, in both directions.
    /// Bottom bar:  progress path (stream), theme switch, failure path (invalid value),
    ///              recovery (reset), design-time notes, clear.
    /// </summary>
    public partial class EnterprisePage : Page
    {
        /// <summary>The built-in themes the "Switch theme" button cycles through (all ship inside Wisej.Framework.dll).</summary>
        private static readonly string[] ThemeNames = { "Bootstrap-4", "Material-3", "FluentDark-5" };

        /// <summary>One tile: the gauge, its UI, and the plausible drift it follows while streaming.</summary>
        private sealed class Channel
        {
            public string Name;
            public SimpleGaugeControl Gauge;
            public Label Banner;
            public double Base, Amplitude, Phase, Nominal;
        }

        private Channel[] _channels;
        private readonly List<string> _pendingRenderLines = new List<string>();
        private readonly Random _jitter = new Random(5);
        private double _clock;          // seconds of streaming
        private int _alerts;
        private int _themeIndex;

        public EnterprisePage()
        {
            InitializeComponent();

            _channels = new[]
            {
                new Channel { Name = "boiler1", Gauge = this.gaugeBoiler1, Banner = this.labelBanner1, Base = 70, Amplitude = 22, Phase = 0.0, Nominal = 70 },
                new Channel { Name = "boiler2", Gauge = this.gaugeBoiler2, Banner = this.labelBanner2, Base = 92, Amplitude = 16, Phase = 1.7, Nominal = 92 },
                new Channel { Name = "turbine", Gauge = this.gaugeTurbine, Banner = this.labelBanner3, Base = 64, Amplitude = 28, Phase = 3.1, Nominal = 64 },
                new Channel { Name = "coolant", Gauge = this.gaugeCoolant, Banner = this.labelBanner4, Base = 108, Amplitude = 9, Phase = 4.4, Nominal = 108 },
            };

            // Custom-control events → page logic (the same way a built-in control is consumed).
            foreach (var channel in _channels)
            {
                var c = channel;
                c.Gauge.ThresholdExceeded += (s, e) => Gauge_ThresholdExceeded(c, e);
                c.Gauge.Trace += (s, e) => Gauge_Trace(c, e);
            }
        }

        private void EnterprisePage_Load(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Server, "page load",
                $"4 × SimpleGaugeControl on one Page · theme={CurrentThemeName()} · client class {SimpleGaugeControl.ClientClassName}");
            SetStatus("idle", StatusKind.Normal);
            UpdateStateLabel();

            // The first OnWebRender of each tile happens while this response is being serialized;
            // its trace lines are queued and shown by the one-shot flush timer.
            this.timerFlush.Start();
        }

        private void timerFlush_Tick(object sender, EventArgs e)
        {
            this.timerFlush.Stop();
            FlushPendingRenders();
            UpdateStateLabel();
        }

        #region Progress path: the Timer streams plausible readings into all four tiles

        private void buttonStream_Click(object sender, EventArgs e)
        {
            FlushPendingRenders();

            if (this.timerStream.Enabled)
            {
                StopStream("stopped by operator");
                return;
            }

            this.buttonStream.Text = "■ Stop streaming";
            AddTrace(TraceDirection.Server, "stream", $"4 tiles · one Update() per changed gauge every {this.timerStream.Interval} ms");
            SetStatus("streaming", StatusKind.Normal);
            this.timerStream.Start();
            timerStream_Tick(sender, e);
        }

        private void timerStream_Tick(object sender, EventArgs e)
        {
            FlushPendingRenders();
            _clock += this.timerStream.Interval / 1000.0;

            foreach (var c in _channels)
            {
                double reading = c.Base + c.Amplitude * Math.Sin(0.9 * _clock + c.Phase) + (_jitter.NextDouble() - 0.5) * 1.5;
                reading = Math.Round(Math.Max(c.Gauge.Minimum, Math.Min(c.Gauge.Maximum, reading)), 1);
                if (!SetGauge(c, reading))
                {
                    StopStream("aborted after a rejected reading");
                    return;
                }
            }

            UpdateStateLabel();
        }

        private void StopStream(string reason)
        {
            this.timerStream.Stop();
            this.buttonStream.Text = "▶ Stream live";
            AddTrace(TraceDirection.Server, "stream", reason);
            SetStatus(AlarmCount() > 0 ? $"alarm ({AlarmCount()} tiles)" : "idle",
                      AlarmCount() > 0 ? StatusKind.Error : StatusKind.Normal);
        }

        #endregion

        #region Success path: the page sets a typed property; the control validates and re-renders

        /// <summary>
        /// The only way the page changes a reading. Validation happens in the control's setter,
        /// on the server, before anything is rendered. Returns false when the value was rejected.
        /// </summary>
        private bool SetGauge(Channel c, double value)
        {
            try
            {
                c.Gauge.Value = value;

                // A tile in alarm clears its banner once the reading falls below the threshold again.
                if (c.Banner.Visible && value < c.Gauge.Threshold)
                    c.Banner.Visible = false;

                return true;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // Failure path: the server refuses invalid state. Nothing is rendered, the browser
                // keeps the last good reading, the UI says why.
                AddTrace(TraceDirection.Server, $"rejected {c.Name}", ex.Message);
                ShowBanner(c, $"✖ Rejected on the server: {ex.Message}");
                SetStatus("rejected", StatusKind.Error);
                AlertBox.Show(ex.Message, MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return false;
            }
        }

        #endregion

        #region Theme: the same control restyled by another theme

        private void buttonTheme_Click(object sender, EventArgs e)
        {
            FlushPendingRenders();

            _themeIndex = (_themeIndex + 1) % ThemeNames.Length;
            string name = ThemeNames[_themeIndex];

            // The "simplegauge" appearance key (Themes/simplegauge.mixin.theme) is resolved against
            // whatever theme is active: background, border, radius, textColor and the accent colour
            // the client class reads all change together with every built-in control.
            Application.LoadTheme(name);

            AddTrace(TraceDirection.ServerToClient, "theme",
                $"Application.LoadTheme(\"{name}\") · appearance \"simplegauge\" + mixin restyle all 4 tiles");
            this.buttonTheme.Text = $"Switch theme → {ThemeNames[(_themeIndex + 1) % ThemeNames.Length]}";
            UpdateStateLabel();
        }

        #endregion

        #region Failure path and recovery

        private void buttonInvalid_Click(object sender, EventArgs e)
        {
            FlushPendingRenders();

            // 999 is above Maximum (150): the typed property throws before any JSON is rendered.
            SetGauge(_channels[0], 999);
            UpdateStateLabel();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            FlushPendingRenders();

            // Recovery: the server is the source of truth, so recovery is "set the nominal
            // state again" and let OnWebRender ship the difference.
            if (this.timerStream.Enabled)
                StopStream("stopped before reset");

            _clock = 0;
            foreach (var c in _channels)
            {
                SetGauge(c, c.Nominal);
                c.Banner.Visible = false;
            }

            AddTrace(TraceDirection.Server, "reset", "nominal readings restored on all 4 tiles; banners cleared");
            SetStatus("idle", StatusKind.Normal);
            UpdateStateLabel();
        }

        #endregion

        #region Design-time notes

        private void buttonNotes_Click(object sender, EventArgs e)
        {
            FlushPendingRenders();

            if (this.panelNotes.Visible)
            {
                this.panelNotes.Visible = false;
                return;
            }

            var g = this.gaugeBoiler1;
            this.labelNotes.Text =
                "The Wisej Designer instantiates IntegrationLab.Controls.SimpleGaugeControl from the built\n" +
                "assembly and renders it live, with the same client class and theme as at runtime.\n" +
                "\n" +
                "What OnWebRender writes when IsDesignMode() is true (Site.DesignMode or IWisejComponent.DesignMode):\n" +
                "  " + g.GetDesignTimeConfigJson() + "\n" +
                "\n" +
                "  • value      = 62% of the scale (" + F(g.DesignTimeSampleValue()) + ") — a needle, an arc and a readout, not a grey box\n" +
                "  • caption    = \"SimpleGauge (design)\" when Caption is empty\n" +
                "  • everything else is the real property state, so the Designer preview matches runtime\n" +
                "\n" +
                "Client side (Platform/SimpleGaugeControl.js):\n" +
                "  • the vendor gauge is created on the first \"appear\" from properties only — no server round trip needed\n" +
                "  • repeated appear / resize (dragging, resizing on the design surface) only calls resize()\n" +
                "  • wisej.web.DesignMode is true, so wired events are never attached at design time\n" +
                "  • destruct() destroys the vendor object when the Designer re-creates the control\n" +
                "\n" +
                "Right now (runtime): IsDesignMode() = " + g.IsDesignMode() + ", renders so far = " + g.RenderCount + "\n" +
                "\n" +
                "Restart discipline: Visual Studio caches the control assembly used by the Designer. After changing\n" +
                "SimpleGaugeControl.cs or the client class, rebuild and reopen EnterprisePage.cs [Design] (restart VS if the\n" +
                "preview does not change). Full notes: docs/DesignTimeNotes.md — take the screenshot there.";

            this.panelNotes.BringToFront();
            this.panelNotes.Visible = true;
            AddTrace(TraceDirection.Server, "design-time", $"IsDesignMode()={g.IsDesignMode()} · sample config shown");
        }

        #endregion

        #region Custom-control events → page

        private void Gauge_ThresholdExceeded(Channel c, GaugeThresholdEventArgs e)
        {
            _alerts++;
            this.labelKpiAlertsValue.Text = _alerts.ToString(CultureInfo.InvariantCulture);

            AddTrace(TraceDirection.Server, "ThresholdExceeded in C#",
                $"{c.Name} Value={F(e.Value)}{c.Gauge.Units.Trim()} ≥ {F(e.Threshold)} (client reported {F(e.ReportedValue)})");

            ShowBanner(c, $"⚠ ThresholdExceeded — {F(e.Value)}{c.Gauge.Units} (limit {F(e.Threshold)}) · operator notified");
            SetStatus($"alarm ({AlarmCount()} tiles)", StatusKind.Error);
            AlertBox.Show($"{this.TileTitle(c)}: {F(e.Value)}{c.Gauge.Units} — threshold {F(e.Threshold)} exceeded.",
                MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 3000);
            UpdateStateLabel();
        }

        private void Gauge_Trace(Channel c, TraceEventArgs e)
        {
            string line = FormatTrace(e.Direction, $"{e.Name} {c.Name}", e.Payload);

            // Lines raised inside OnWebRender arrive while Wisej.NET is serializing the response:
            // queue them and append them to the ListBox on the next request.
            if (e.FromRender)
                _pendingRenderLines.Add(line);
            else
                AppendTraceLine(line);
        }

        #endregion

        #region UI helpers

        private enum StatusKind { Normal, Warn, Error }

        private string TileTitle(Channel c)
        {
            if (c.Gauge == this.gaugeBoiler1) return this.labelTile1.Text;
            if (c.Gauge == this.gaugeBoiler2) return this.labelTile2.Text;
            if (c.Gauge == this.gaugeTurbine) return this.labelTile3.Text;
            return this.labelTile4.Text;
        }

        private int AlarmCount()
        {
            int n = 0;
            foreach (var c in _channels)
                if (c.Gauge.IsAlarm) n++;
            return n;
        }

        private string CurrentThemeName()
        {
            try { return Application.Theme?.Name ?? ThemeNames[_themeIndex]; }
            catch (Exception) { return ThemeNames[_themeIndex]; }
        }

        private void FlushPendingRenders()
        {
            if (_pendingRenderLines.Count == 0)
                return;

            foreach (var line in _pendingRenderLines)
                AppendTraceLine(line);
            _pendingRenderLines.Clear();
        }

        private void AddTrace(TraceDirection direction, string name, string payload)
            => AppendTraceLine(FormatTrace(direction, name, payload));

        private static string FormatTrace(TraceDirection direction, string name, string payload)
        {
            string prefix = direction switch
            {
                TraceDirection.ServerToClient => "→ .NET→JS ",
                TraceDirection.ClientToServer => "← JS→.NET ",
                _ => "• server  ",
            };
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            return $"{time}  {prefix} {name,-26} {payload}";
        }

        private void AppendTraceLine(string line)
        {
            // keep the list bounded while streaming
            while (this.listTrace.Items.Count >= 400)
                this.listTrace.Items.RemoveAt(0);

            this.listTrace.Items.Add(line);
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            _pendingRenderLines.Clear();
            this.listTrace.Items.Clear();
        }

        private void SetStatus(string text, StatusKind kind)
        {
            this.labelStatus.Text = "● " + text;
            this.labelStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
        }

        private void ShowBanner(Channel c, string text)
        {
            c.Banner.Text = text;
            c.Banner.Visible = true;
        }

        private void UpdateStateLabel()
        {
            int renders = 0;
            foreach (var c in _channels)
                renders += c.Gauge.RenderCount;

            var g = this.gaugeBoiler1;
            this.labelState.Text =
                $"SERVER STATE (authoritative)   theme={CurrentThemeName()}   renders={renders}   alerts={_alerts}\n" +
                $"boiler1 Value={F(g.Value)} Min={F(g.Minimum)} Max={F(g.Maximum)} Threshold={F(g.Threshold)} alarm={g.IsAlarm}\n" +
                $"last render: {Shorten(g.LastRenderedJson, 110)}";
        }

        private static string Shorten(string s, int max)
            => string.IsNullOrEmpty(s) ? "(not rendered yet)" : s.Length <= max ? s : s.Substring(0, max - 1) + "…";

        private static string F(double value) => value.ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}
