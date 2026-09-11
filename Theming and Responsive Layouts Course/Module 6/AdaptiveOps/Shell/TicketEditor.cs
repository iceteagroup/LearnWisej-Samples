using System;
using System.Globalization;
using AdaptiveOps.Models;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// The ticket details editor as a UserControl (layout in TicketEditor.Designer.cs).
    ///
    /// The same instance lives docked inside the details region on desktop and tablet, or inside
    /// <see cref="Dialogs.TicketEditorForm"/> on the phone profiles, so whatever the operator has typed survives a
    /// profile change. The editor raises <see cref="SaveRequested"/>; the page saves (server-side validation) and
    /// reports back through <see cref="ShowMessage"/>, so the feedback is visible where the editor is.
    /// </summary>
    public partial class TicketEditor : UserControl
    {
        /// <summary>Raised when Save is clicked; the host reads <see cref="ReadTicket"/> and validates on the server.</summary>
        public event EventHandler SaveRequested;

        /// <summary>Raised by the Close button, which is visible only while the editor is hosted in a dialog.</summary>
        public event EventHandler CloseRequested;

        public TicketEditor()
        {
            InitializeComponent();

            // Drop-downs come from the enums; the theme owns how they look.
            this.cboPriority.Items.AddRange(Enum.GetNames(typeof(TicketPriority)));
            this.cboStatus.Items.AddRange(Enum.GetNames(typeof(TicketStatus)));
            Clear();
        }

        /// <summary>Id of the ticket being edited, or null.</summary>
        public string TicketId { get; private set; }

        /// <summary>True while the editor is hosted in the dialog: shows the Close button.</summary>
        public bool CloseButtonVisible
        {
            get => this.btnClose.Visible;
            set => this.btnClose.Visible = value;
        }

        /// <summary>Fills the editor from a repository copy of the ticket (never from grid cells).</summary>
        public void LoadTicket(Ticket t)
        {
            if (t == null)
            {
                Clear();
                return;
            }

            TicketId = t.Id;
            this.lblDetailsSubtitle.Text = $"{t.Id} · {t.Status} · due {t.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
            this.txtTitle.Text = t.Title;
            this.cboPriority.SelectedIndex = (int)t.Priority;
            this.cboStatus.SelectedIndex = (int)t.Status;
            this.txtOwner.Text = t.Owner;
            this.dtpDue.Value = t.DueDate;
            this.txtNotes.Text = t.Notes;
            ClearMessage();
        }

        public void Clear()
        {
            TicketId = null;
            this.lblDetailsSubtitle.Text = "Select a ticket in the grid";
            this.txtTitle.Text = string.Empty;
            this.cboPriority.SelectedIndex = -1;
            this.cboStatus.SelectedIndex = -1;
            this.txtOwner.Text = string.Empty;
            this.dtpDue.Value = DateTime.Today;
            this.txtNotes.Text = string.Empty;
            ClearMessage();
        }

        /// <summary>Builds the ticket the editor currently describes. Validation is NOT done here.</summary>
        public Ticket ReadTicket()
        {
            return new Ticket
            {
                Id = TicketId,
                Title = this.txtTitle.Text,
                Priority = (TicketPriority)Math.Max(0, this.cboPriority.SelectedIndex),
                Status = (TicketStatus)Math.Max(0, this.cboStatus.SelectedIndex),
                Owner = this.txtOwner.Text,
                DueDate = this.dtpDue.Value,
                Notes = this.txtNotes.Text
            };
        }

        /// <summary>Shows the save outcome inside the editor, so it is visible in the phone dialog too.</summary>
        public void ShowMessage(string text, bool isError)
        {
            this.lblMessage.Text = text;
            this.lblMessage.ForeColor = isError
                ? System.Drawing.Color.FromArgb(180, 35, 24)
                : System.Drawing.Color.FromArgb(2, 122, 72);
            this.lblMessage.Visible = true;
        }

        public void ClearMessage()
        {
            this.lblMessage.Text = string.Empty;
            this.lblMessage.Visible = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
