using System;
using System.Globalization;
using AdaptiveOps.Models;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>Argument of <see cref="DetailsEditor.Saved"/>: the ticket as the editor describes it.</summary>
    public sealed class TicketSaveEventArgs : EventArgs
    {
        public TicketSaveEventArgs(Ticket ticket)
        {
            Ticket = ticket;
        }

        public Ticket Ticket { get; }
    }

    /// <summary>
    /// Right details region of the console. Its local layout (DetailsEditor.Designer.cs) is a header
    /// (Dock Top), the scrolling fields (Dock Fill, AutoScroll, ScrollBars.Hidden) and a command bar
    /// (Dock Bottom) with Save and Cancel anchored Bottom|Right. The shell sees only
    /// <see cref="Ticket"/> and <see cref="Saved"/>; every field is private.
    /// </summary>
    public partial class DetailsEditor : UserControl
    {
        private Ticket _original;

        public DetailsEditor()
        {
            InitializeComponent();

            this.cboPriority.Items.AddRange(Enum.GetNames(typeof(TicketPriority)));
            this.cboStatus.Items.AddRange(Enum.GetNames(typeof(TicketStatus)));
        }

        /// <summary>Raised when the user clicks Save; the shell validates and stores the ticket.</summary>
        public event EventHandler<TicketSaveEventArgs> Saved;

        /// <summary>
        /// The ticket shown in the editor (null clears it). The getter builds a new Ticket from the
        /// fields; validation is done by the repository.
        /// </summary>
        public Ticket Ticket
        {
            get
            {
                if (_original == null)
                    return null;

                return new Ticket
                {
                    Id = _original.Id,
                    Title = this.txtTitle.Text,
                    Priority = (TicketPriority)Math.Max(0, this.cboPriority.SelectedIndex),
                    Status = (TicketStatus)Math.Max(0, this.cboStatus.SelectedIndex),
                    Owner = this.txtOwner.Text,
                    DueDate = this.dtpDue.Value,
                    Notes = this.txtNotes.Text
                };
            }
            set
            {
                if (value == null)
                {
                    _original = null;
                    this.lblDetailsSubtitle.Text = "Select a ticket in the grid";
                    this.txtId.Text = string.Empty;
                    this.txtTitle.Text = string.Empty;
                    this.cboPriority.SelectedIndex = -1;
                    this.cboStatus.SelectedIndex = -1;
                    this.txtOwner.Text = string.Empty;
                    this.dtpDue.Value = DateTime.Today;
                    this.txtNotes.Text = string.Empty;
                    return;
                }

                _original = value.Clone();
                this.lblDetailsSubtitle.Text = $"{value.Id} · {value.Status} · due {value.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
                this.txtId.Text = value.Id;
                this.txtTitle.Text = value.Title;
                this.cboPriority.SelectedIndex = (int)value.Priority;
                this.cboStatus.SelectedIndex = (int)value.Status;
                this.txtOwner.Text = value.Owner;
                this.dtpDue.Value = value.DueDate;
                this.txtNotes.Text = value.Notes;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var ticket = Ticket;
            if (ticket != null)
                Saved?.Invoke(this, new TicketSaveEventArgs(ticket));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_original != null)
                Ticket = _original;
        }
    }
}
