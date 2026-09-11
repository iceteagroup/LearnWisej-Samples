using System;
using System.Globalization;
using IntegrationLab.Controls;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// Operations Dashboard. Every tile is the same custom control, IntegrationLab.Controls.SimpleGaugeControl,
    /// styled by the "simplegauge" appearance key: typed properties in, .NET events out, no per-screen InitScript.
    /// </summary>
    public partial class EnterprisePage : Page
    {
        /// <summary>One tile: the gauge, its banner, and the drift it follows while streaming.</summary>
        private sealed class Channel
        {
            public SimpleGaugeControl Gauge;
            public Label Banner;
            public double Base, Amplitude, Phase;
        }

        private readonly Channel[] _channels;
        private readonly Random _jitter = new Random(5);
        private double _clock;
        private int _alerts;

        public EnterprisePage()
        {
            InitializeComponent();

            _channels = new[]
            {
                new Channel { Gauge = this.gaugeBoiler1, Banner = this.labelBanner1, Base = 70, Amplitude = 22, Phase = 0.0 },
                new Channel { Gauge = this.gaugeBoiler2, Banner = this.labelBanner2, Base = 92, Amplitude = 16, Phase = 1.7 },
                new Channel { Gauge = this.gaugeTurbine, Banner = this.labelBanner3, Base = 64, Amplitude = 28, Phase = 3.1 },
                new Channel { Gauge = this.gaugeCoolant, Banner = this.labelBanner4, Base = 108, Amplitude = 9, Phase = 4.4 },
            };

            foreach (var channel in _channels)
            {
                var c = channel;
                c.Gauge.ThresholdExceeded += (s, e) => Gauge_ThresholdExceeded(c, e);
            }
        }

        private void EnterprisePage_Load(object sender, EventArgs e)
        {
            this.timerStream.Start();
        }

        private void timerStream_Tick(object sender, EventArgs e)
        {
            _clock += this.timerStream.Interval / 1000.0;

            foreach (var c in _channels)
            {
                double reading = c.Base + c.Amplitude * Math.Sin(0.9 * _clock + c.Phase) + (_jitter.NextDouble() - 0.5) * 1.5;
                reading = Math.Round(Math.Max(c.Gauge.Minimum, Math.Min(c.Gauge.Maximum, reading)), 1);
                if (!SetGauge(c, reading))
                {
                    this.timerStream.Stop();
                    SetStatus("stopped", System.Drawing.Color.FromArgb(224, 86, 59));
                    return;
                }
            }

            UpdateStatus();
        }

        /// <summary>
        /// The only way the page changes a reading. The control validates the value on the server
        /// before anything is rendered. Returns false when the value was rejected.
        /// </summary>
        private bool SetGauge(Channel c, double value)
        {
            try
            {
                c.Gauge.Value = value;

                if (c.Banner.Visible && value < c.Gauge.Threshold)
                    c.Banner.Visible = false;

                return true;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ShowBanner(c, "✖ " + ex.Message);
                AlertBox.Show(ex.Message, MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return false;
            }
        }

        private void Gauge_ThresholdExceeded(Channel c, GaugeThresholdEventArgs e)
        {
            _alerts++;
            this.labelKpiAlertsValue.Text = _alerts.ToString(CultureInfo.InvariantCulture);

            ShowBanner(c, $"⚠ {F(e.Value)}{c.Gauge.Units} — above {F(e.Threshold)}");
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            int alarms = 0;
            foreach (var c in _channels)
                if (c.Gauge.IsAlarm) alarms++;

            if (alarms > 0)
                SetStatus($"alarm ({alarms})", System.Drawing.Color.FromArgb(224, 86, 59));
            else
                SetStatus("live", System.Drawing.Color.FromArgb(31, 157, 87));
        }

        private void SetStatus(string text, System.Drawing.Color color)
        {
            this.labelStatus.Text = "● " + text;
            this.labelStatus.ForeColor = color;
        }

        private static void ShowBanner(Channel c, string text)
        {
            c.Banner.Text = text;
            c.Banner.Visible = true;
        }

        private static string F(double value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
