using System.Drawing;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// One metric card (accent strip, caption, value). The card knows nothing about layout: it is
    /// 192 × 76 by default and is placed by whichever engine hosts it — the FlowLayoutPanel wraps it,
    /// the TableLayoutPanel stretches it inside its cell (Dock = Fill in the cell), the FlexLayoutPanel
    /// shares the row width by FillWeight. MinimumSize keeps it usable, MaximumSize keeps the height at
    /// 76 so the flex engine has something to align with AlignY.
    /// </summary>
    public partial class MetricCard : UserControl
    {
        public MetricCard()
        {
            InitializeComponent();
        }

        /// <summary>Caption in small caps ("OPEN", "OVERDUE" …).</summary>
        public string Title
        {
            get => this.lblTitle.Text;
            set => this.lblTitle.Text = (value ?? string.Empty).ToUpperInvariant();
        }

        /// <summary>The metric value as text ("12", "–").</summary>
        public string Value
        {
            get => this.lblValue.Text;
            set => this.lblValue.Text = value ?? string.Empty;
        }

        /// <summary>Colour of the 4-px strip along the top edge.</summary>
        public Color Accent
        {
            get => this.strip.BackColor;
            set => this.strip.BackColor = value;
        }

        /// <summary>"Open 192×76 @(4,4)" — used by the trace to compare what each engine did with the card.</summary>
        public string DescribeBounds()
        {
            return $"{this.Title.ToLowerInvariant()} {this.Width}×{this.Height} @({this.Left},{this.Top})";
        }
    }
}
