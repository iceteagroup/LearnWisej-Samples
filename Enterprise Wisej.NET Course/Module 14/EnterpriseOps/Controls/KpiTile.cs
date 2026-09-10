using System.ComponentModel;
using Wisej.Web;

namespace EnterpriseOps.Controls
{
    /// <summary>
    /// One KPI card: a small uppercase caption over a big number, in an accent colour. The Command Center
    /// drops five of them from the Toolbox.
    ///
    /// It holds UI state only — what to show. It never computes a number: that is
    /// <see cref="Services.DashboardService"/>'s job, which is why the definition of "overdue" can be
    /// reviewed without opening the Designer.
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

        [Category("KPI"), Description("The small line under the number (what the KPI means).")]
        public string Footnote
        {
            get { return lblFootnote.Text; }
            set { lblFootnote.Text = value ?? ""; }
        }
    }
}
