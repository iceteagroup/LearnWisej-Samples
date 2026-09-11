using System;
using System.Globalization;
using AdaptiveOps.Models;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// The ticket editor as a structured form: a two-column TableLayoutPanel (TicketEditor.Designer.cs).
    /// ColumnStyles hold the proportions (Absolute 140 for captions, Percent 100 for editors), every row
    /// is AutoSize except Notes (Percent 100, never below the editor's MinimumSize of 120), the Notes
    /// editor spans both columns, and each editor is docked Fill inside its cell.
    /// </summary>
    public partial class TicketEditor : UserControl
    {
        /// <summary>Raised when the Save button is clicked; the page owns the repository and the write path.</summary>
        public event EventHandler SaveClick;

        public TicketEditor()
        {
            InitializeComponent();

            this.cboPriority.Items.AddRange(Enum.GetNames(typeof(TicketPriority)));
            this.cboStatus.Items.AddRange(Enum.GetNames(typeof(TicketStatus)));
        }

        /// <summary>Fills the editors from a repository copy of the ticket.</summary>
        public void ShowTicket(Ticket t)
        {
            this.lblSubtitle.Text = $"{t.Id} · {t.Status} · due {t.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
            this.txtTitle.Text = t.Title;
            this.cboPriority.SelectedIndex = (int)t.Priority;
            this.cboStatus.SelectedIndex = (int)t.Status;
            this.txtOwner.Text = t.Owner;
            this.dtpDue.Value = t.DueDate;
            this.txtNotes.Text = t.Notes;
        }

        public void Clear()
        {
            this.lblSubtitle.Text = "Select a ticket in the grid";
            this.txtTitle.Text = string.Empty;
            this.cboPriority.SelectedIndex = -1;
            this.cboStatus.SelectedIndex = -1;
            this.txtOwner.Text = string.Empty;
            this.dtpDue.Value = DateTime.Today;
            this.txtNotes.Text = string.Empty;
        }

        /// <summary>Builds the ticket the editors currently describe. Validation is done by the repository.</summary>
        public Ticket Read(string id)
        {
            return new Ticket
            {
                Id = id,
                Title = this.txtTitle.Text,
                Priority = (TicketPriority)Math.Max(0, this.cboPriority.SelectedIndex),
                Status = (TicketStatus)Math.Max(0, this.cboStatus.SelectedIndex),
                Owner = this.txtOwner.Text,
                DueDate = this.dtpDue.Value,
                Notes = this.txtNotes.Text
            };
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
