using TicketOps.Domain;
using Wisej.Web;

namespace TicketOps.Controls
{
    /// <summary>
    /// One control, every screen: the status pill the dashboard's KPI cards, the detail strip and the
    /// WorkOrderDetail dialog all use. It is SET, not styled:
    ///
    ///  - <see cref="Status"/> picks the colour — through a theme STATE ("open", "inprogress", "blocked", "done")
    ///    on the "chip" appearance defined in Themes/TicketOps.mixin.theme. The palette lives in the theme
    ///    (chip-open → primary, chip-blocked → danger, chip-done → success …), so the same chip is right on
    ///    Bootstrap-4 and BootstrapDark-4 without a single conditional here;
    ///  - <see cref="Text"/> comes from the resources (Status.Open, Status.InProgress …) — localizable;
    ///  - padding, radius and font are fixed once, in the appearance and in the Designer file.
    ///
    /// There is no BackColor, ForeColor or hex value in this file: visual meaning is defined once in the theme
    /// layer and never recreated per screen.
    /// </summary>
    public partial class StatusChip : UserControl
    {
        /// <summary>The theme appearance key (Themes/TicketOps.mixin.theme → "appearances": { "chip": … }).</summary>
        public const string ChipAppearanceKey = "chip";

        private WorkOrderStatus _status;
        private string _currentState;

        public StatusChip()
        {
            InitializeComponent();

            // Join the theme system: background, radius and text colour come from the "chip" appearance + state.
            this.AppearanceKey = ChipAppearanceKey;
            ApplyState(StateFor(_status));
        }

        /// <summary>The status this chip shows. Setting it swaps the theme state; the theme decides what that looks like.</summary>
        public WorkOrderStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                ApplyState(StateFor(value));
            }
        }

        /// <summary>The caption — always a resource string (ILocalizationService.StatusText), never a literal.</summary>
        public override string Text
        {
            get => this.labelText.Text;
            set => this.labelText.Text = value;
        }

        /// <summary>Convenience for the screens: status + localized caption in one call.</summary>
        public void Show(WorkOrderStatus status, string text)
        {
            Status = status;
            Text = text;
        }

        /// <summary>WorkOrderStatus.InProgress → "inprogress": the state names used in the mixin.</summary>
        public static string StateFor(WorkOrderStatus status) => status.ToString().ToLowerInvariant();

        private void ApplyState(string state)
        {
            if (_currentState == state)
                return;

            if (_currentState != null)
                RemoveState(_currentState);

            AddState(state);
            _currentState = state;
        }
    }
}
