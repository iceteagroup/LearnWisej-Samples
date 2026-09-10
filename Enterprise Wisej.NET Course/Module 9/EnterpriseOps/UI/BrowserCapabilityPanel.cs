using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// The browser capability panel (deliverable 4).
    ///
    /// It renders what <see cref="BrowserCapabilityService"/> accepted — never what the browser
    /// sent. The header says "detection only" because that is the whole contract of this card:
    /// a missing capability changes the OFFER (manual entry instead of a camera capture), and a
    /// present one changes nothing about what the user may do.
    ///
    /// The control is a plain composed UserControl: no JavaScript of its own, no state of its own,
    /// one <see cref="Render"/> method the page calls after every accepted report.
    /// </summary>
    public partial class BrowserCapabilityPanel : UserControl
    {
        public BrowserCapabilityPanel()
        {
            InitializeComponent();
        }

        public void Render(IReadOnlyList<CapabilityReading> readings, string summary, int ignoredKeys)
        {
            this.lstCapabilities.BeginUpdate();
            try
            {
                this.lstCapabilities.Items.Clear();
                if (readings == null || readings.Count == 0)
                {
                    this.lstCapabilities.Items.Add("… waiting for the host widget's first report");
                }
                else
                {
                    foreach (CapabilityReading reading in readings)
                        this.lstCapabilities.Items.Add(reading.ToString());
                }
            }
            finally
            {
                this.lstCapabilities.EndUpdate();
            }

            this.lblCapabilitySummary.Text = summary ?? "";
            this.lblCapabilitySummary.ForeColor = ignoredKeys > 0
                ? System.Drawing.Color.FromArgb(232, 161, 60)
                : System.Drawing.Color.FromArgb(106, 125, 146);

            List<CapabilityReading> missing = readings == null
                ? new List<CapabilityReading>()
                : readings.Where(r => !r.Available && !string.IsNullOrEmpty(r.Fallback)).ToList();

            this.lblCapabilityFallback.Text = missing.Count == 0
                ? "No fallback needed — every capability this screen uses is present."
                : $"{missing.Count} fallback(s) chosen by the server, e.g. \"{missing[0].Title} → {missing[0].Fallback}\".";
        }
    }
}
