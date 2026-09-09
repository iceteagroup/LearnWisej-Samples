using System.ComponentModel;
using Wisej.Web;

namespace EnterpriseOps.Controls
{
    /// <summary>
    /// A shared control: one KPI card (caption + big number in an accent colour). The dashboard drops three of
    /// them from the Toolbox; every later screen that shows a number reuses the same tile, so the tiles look the
    /// same everywhere and are styled in one place.
    ///
    /// It holds UI state only (what to show) — it never computes a number.
    /// </summary>
    public partial class KpiTile : UserControl
    {
        public KpiTile()
        {
            InitializeComponent();
        }

        [Category("KPI"), Description("The small uppercase caption above the number.")]
        public string Caption
        {
            get { return lblCaption.Text; }
            set { lblCaption.Text = (value ?? "").ToUpperInvariant(); }
        }

        [Category("KPI"), Description("The big number.")]
        public string Value
        {
            get { return lblValue.Text; }
            set { lblValue.Text = value ?? ""; }
        }

        [Category("KPI"), Description("Colour of the big number.")]
        public System.Drawing.Color Accent
        {
            get { return lblValue.ForeColor; }
            set { lblValue.ForeColor = value; }
        }
    }
}
