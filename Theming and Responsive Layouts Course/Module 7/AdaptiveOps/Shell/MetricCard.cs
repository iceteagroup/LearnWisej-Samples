using System;
using System.Globalization;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// One metric card ("OPEN · 6"). The look is owned by the theme: the card is the <c>metric-card</c>
    /// appearance (surface, border, shadow, a dashed <c>stale</c> state), the two labels are the
    /// <c>metric-title</c> and <c>metric-value</c> appearances (font + colour tokens), and the scoped
    /// stylesheet adds the app-specific uppercase/letter-spacing through the <c>metric-card</c> /
    /// <c>metric-title</c> CSS classes. No BackColor, ForeColor or Font is set anywhere in this control.
    /// </summary>
    public partial class MetricCard : UserControl
    {
        public MetricCard()
        {
            InitializeComponent();
        }

        /// <summary>The caption shown in small caps ("OVERDUE").</summary>
        public string Title
        {
            get => this.lblTitle.Text;
            set
            {
                this.lblTitle.Text = value;
                this.AccessibleName = value + " metric";
                this.ToolTipText = value;
            }
        }

        /// <summary>The metric value as text.</summary>
        public string Value
        {
            get => this.lblValue.Text;
            set => this.lblValue.Text = value;
        }

        public void SetValue(int value) => Value = value.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Marks the card as stale (theme state <c>stale</c>: dashed warning border) or fresh.
        /// A custom theme state, not a colour set in code.
        /// </summary>
        public bool Stale
        {
            get => HasState("stale");
            set
            {
                if (value) AddState("stale"); else RemoveState("stale");
            }
        }
    }
}
