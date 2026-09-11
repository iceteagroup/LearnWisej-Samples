using System.Collections.Generic;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// The browser capability panel.
    ///
    /// It renders what <see cref="BrowserCapabilityService"/> accepted — never what the browser
    /// sent. A missing capability changes the OFFER (manual entry instead of a camera capture);
    /// a present one changes nothing about what the user may do.
    ///
    /// A plain composed UserControl: no JavaScript of its own, no state of its own, one
    /// <see cref="Render"/> method the page calls after every accepted report.
    /// </summary>
    public partial class BrowserCapabilityPanel : UserControl
    {
        public BrowserCapabilityPanel()
        {
            InitializeComponent();
        }

        public void Render(IReadOnlyList<CapabilityReading> readings)
        {
            this.lstCapabilities.BeginUpdate();
            try
            {
                this.lstCapabilities.Items.Clear();
                if (readings == null || readings.Count == 0)
                {
                    this.lstCapabilities.Items.Add("Detecting…");
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
        }
    }
}
