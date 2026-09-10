using System;
using System.Globalization;
using AdaptiveOps.Models;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// Argument of <see cref="DetailsEditor.Saved"/>. The editor fills <see cref="Ticket"/>; the
    /// shell saves it on the server and, when the repository refuses it or throws, writes the
    /// reason into <see cref="Error"/> so the editor can show the outcome next to its buttons.
    /// </summary>
    public sealed class TicketSaveEventArgs : EventArgs
    {
        public TicketSaveEventArgs(Ticket ticket)
        {
            Ticket = ticket;
        }

        /// <summary>The ticket as the editor describes it (not validated).</summary>
        public Ticket Ticket { get; }

        /// <summary>Set by the shell when the save failed; null means saved.</summary>
        public string Error { get; set; }
    }

    /// <summary>
    /// Right details region of the console (Dock = Right in MainPage). Its local layout is three
    /// nested containers declared in DetailsEditor.Designer.cs — a header (Dock Top), the scrolling
    /// fields (Dock Fill, AutoScroll, ScrollBars.Hidden, AutoScrollMargin 0×24) and a command bar
    /// (Dock Bottom) whose Save / Cancel buttons are anchored Bottom|Right. Captions are AutoSize
    /// labels anchored Top|Left, the title / id / notes editors stretch with Top|Left|Right, and the
    /// second column (Status, Due) keeps its distance from the right edge with Top|Right.
    ///
    /// The shell sees a small public surface: <see cref="Ticket"/>, <see cref="Saved"/> and
    /// <see cref="Cancelled"/>. Every field is private, so Module 6 can dock this region under the
    /// grid on a tablet or open it in a Form on a phone without touching a text box.
    /// </summary>
    public partial class DetailsEditor : UserControl
    {
        /// <summary>The ticket as the shell last set it; Cancel restores it.</summary>
        private Ticket _original;

        public DetailsEditor()
        {
            InitializeComponent();

            // Drop-down entries come from the enums; the base theme owns how they look.
            this.cboPriority.Items.AddRange(Enum.GetNames(typeof(TicketPriority)));
            this.cboStatus.Items.AddRange(Enum.GetNames(typeof(TicketStatus)));
        }

        /// <summary>Raised when the user clicks Save; the shell writes the ticket and reports through e.Error.</summary>
        public event EventHandler<TicketSaveEventArgs> Saved;

        /// <summary>Raised after Cancel restored the ticket the shell last set.</summary>
        public event EventHandler Cancelled;

        /// <summary>
        /// The ticket shown in the editor. Set null to clear the editor. The getter returns a new
        /// Ticket built from the fields (validation is NOT done here: the repository does it).
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
                    this.lblHint.Text = "Server validates.";
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

        /// <summary>
        /// Height of the scrolling content (last field's bottom + AutoScrollMargin), from the layout
        /// the designer declared; compared with the visible height it tells whether the fields scroll.
        /// </summary>
        public int ContentHeight => this.txtNotes.Bottom + this.fieldsPanel.AutoScrollMargin.Height;

        /// <summary>One line for the trace: "details 332×588 · fields 476 visible / 454 content → fits" or "… → scrolls by N px (AutoScroll, ScrollBars=Hidden)".</summary>
        public string DescribeScroll()
        {
            int content = ContentHeight;
            int visible = this.fieldsPanel.ClientSize.Height;
            return $"details {this.Width}×{this.Height} · fields {visible} visible / {content} content → " +
                   (content > visible
                        ? $"scrolls by {content - visible} px (AutoScroll, ScrollBars={this.fieldsPanel.ScrollBars})"
                        : "fits");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var ticket = Ticket;
            if (ticket == null)
            {
                this.lblHint.Text = "Select a ticket first.";
                return;
            }

            var args = new TicketSaveEventArgs(ticket);
            Saved?.Invoke(this, args);

            // The shell owns the repository; it tells us how it went. An exception on the server
            // becomes a visible message here, never a broken layout.
            this.lblHint.Text = args.Error == null
                ? "✓ saved " + DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                : "✖ " + args.Error;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_original != null)
                Ticket = _original;

            this.lblHint.Text = "Edits discarded.";
            Cancelled?.Invoke(this, EventArgs.Empty);
        }
    }
}
