using System;
using System.Globalization;
using AdaptiveOps.Models;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// The aligned ticket editor: a two-column <see cref="TableLayoutPanel"/> (captions 84 px, editors
    /// 100 %) with Absolute rows for the single-line fields and a Percent row for the notes, a header and
    /// a footer with the Save command and the validation label. The same control is docked in the details
    /// region on desktop/tablet and hosted in <see cref="AdaptiveOps.Dialogs.DetailsDialog"/> on phone.
    ///
    /// Validation feedback is never colour alone: the failing editor gets <c>Invalid = true</c> with an
    /// <c>InvalidMessage</c> (theme <c>invalid</c> state + tooltip) and the footer label announces the same
    /// text with an error icon (theme state <c>error</c> on the <c>validation-label</c> appearance).
    /// </summary>
    public partial class DetailsEditor : UserControl
    {
        /// <summary>Raised when the user presses Save; the owner validates and stores on the server.</summary>
        public event EventHandler<Ticket> SaveRequested;

        private string _ticketId;
        private bool _compact;

        public DetailsEditor()
        {
            InitializeComponent();
            this.cboPriority.Items.AddRange(Enum.GetNames(typeof(TicketPriority)));
            this.cboStatus.Items.AddRange(Enum.GetNames(typeof(TicketStatus)));
            Clear();
        }

        /// <summary>Id of the ticket being edited, or null.</summary>
        public string TicketId => _ticketId;

        /// <summary>
        /// Compact mode for the phone dialog: the text editors take the <c>compact-editor</c> appearance
        /// (28 px, tighter padding) and the rows shrink. Idempotent.
        /// </summary>
        public bool Compact
        {
            get => _compact;
            set
            {
                _compact = value;
                string key = value ? "compact-editor" : string.Empty;
                this.txtTitle.AppearanceKey = key;
                this.txtOwner.AppearanceKey = key;
                this.txtNotes.AppearanceKey = key;
                float rowHeight = value ? 34F : 40F;
                for (int i = 0; i < 5; i++)
                    this.tableEditor.RowStyles[i].Height = rowHeight;
            }
        }

        /// <summary>Fills the editor from a repository copy of the ticket (never from grid cells).</summary>
        public void ShowTicket(Ticket t)
        {
            if (t == null)
            {
                Clear();
                return;
            }

            ClearValidation();
            _ticketId = t.Id;
            this.lblSubtitle.Text = $"{t.Id} · {t.Status} · due {t.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
            this.txtTitle.Text = t.Title;
            this.cboPriority.SelectedIndex = (int)t.Priority;
            this.cboStatus.SelectedIndex = (int)t.Status;
            this.txtOwner.Text = t.Owner;
            this.dtpDue.Value = t.DueDate;
            this.txtNotes.Text = t.Notes;
            this.btnSave.Enabled = true;
        }

        public void Clear()
        {
            ClearValidation();
            _ticketId = null;
            this.lblSubtitle.Text = "Select a ticket in the grid";
            this.txtTitle.Text = string.Empty;
            this.cboPriority.SelectedIndex = -1;
            this.cboStatus.SelectedIndex = -1;
            this.txtOwner.Text = string.Empty;
            this.dtpDue.Value = DateTime.Today;
            this.txtNotes.Text = string.Empty;
            this.btnSave.Enabled = false;
        }

        /// <summary>Builds the ticket the editor currently describes. Validation is NOT done here.</summary>
        public Ticket ReadTicket()
        {
            return new Ticket
            {
                Id = _ticketId,
                Title = this.txtTitle.Text,
                Priority = (TicketPriority)Math.Max(0, this.cboPriority.SelectedIndex),
                Status = (TicketStatus)Math.Max(0, this.cboStatus.SelectedIndex),
                Owner = this.txtOwner.Text,
                DueDate = this.dtpDue.Value,
                Notes = this.txtNotes.Text
            };
        }

        /// <summary>
        /// Shows a server-side rejection: the field named by the exception turns invalid (theme state +
        /// InvalidMessage tooltip) and the footer label announces the message with an icon.
        /// </summary>
        public void ShowValidationError(TicketValidationException error)
        {
            ClearValidation();
            if (error == null)
                return;

            switch (error.Field)
            {
                case "Title":
                    this.txtTitle.Invalid = true;
                    this.txtTitle.InvalidMessage = error.Message;
                    this.txtTitle.Focus();
                    break;
                case "Owner":
                    this.txtOwner.Invalid = true;
                    this.txtOwner.InvalidMessage = error.Message;
                    this.txtOwner.Focus();
                    break;
                case "DueDate":
                    this.dtpDue.Invalid = true;
                    this.dtpDue.InvalidMessage = error.Message;
                    this.dtpDue.Focus();
                    break;
            }

            this.lblValidation.ImageSource = "icon-error";
            this.lblValidation.Text = "Rejected: " + error.Message;
            this.lblValidation.AddState("error");
        }

        public void ClearValidation()
        {
            this.txtTitle.Invalid = false;
            this.txtTitle.InvalidMessage = string.Empty;
            this.txtOwner.Invalid = false;
            this.txtOwner.InvalidMessage = string.Empty;
            this.dtpDue.Invalid = false;
            this.dtpDue.InvalidMessage = string.Empty;
            this.lblValidation.ImageSource = string.Empty;
            this.lblValidation.Text = string.Empty;
            this.lblValidation.RemoveState("error");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveRequested?.Invoke(this, ReadTicket());
        }
    }
}
