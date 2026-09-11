using System.Drawing;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// One metric card (accent strip, caption, value). The card knows nothing about layout: it is
    /// 192 × 76 by default and the FlowLayoutPanel that hosts it wraps it with the other cards.
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
    }
}
