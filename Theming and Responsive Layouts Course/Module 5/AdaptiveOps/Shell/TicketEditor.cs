using System;
using System.Globalization;
using System.Linq;
using AdaptiveOps.Models;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// The ticket details editor as a structured form: a two-column TableLayoutPanel declared in
    /// TicketEditor.Designer.cs. ColumnStyles hold the proportions — Absolute 140 for the caption
    /// column, Percent 100 for the editor column — so the editors grow with the panel while the captions
    /// stay put, every row is AutoSize except the Notes row (Percent 100, so it takes the remaining
    /// height, never below the editor's MinimumSize of 120), and the Notes editor spans both columns
    /// (SetColumnSpan). Each editor is docked Fill inside its cell: a child of a TableLayoutPanel may
    /// still dock, unlike a child of a FlowLayoutPanel. Nothing here positions a control.
    /// </summary>
    public partial class TicketEditor : UserControl
    {
        /// <summary>Raised when the Save button is clicked; the page owns the repository and the write path.</summary>
        public event EventHandler SaveClick;

        public TicketEditor()
        {
            InitializeComponent();

            // Editor drop-downs come from the enums; the base theme owns how they look.
            this.cboPriority.Items.AddRange(Enum.GetNames(typeof(TicketPriority)));
            this.cboStatus.Items.AddRange(Enum.GetNames(typeof(TicketStatus)));
        }

        /// <summary>The TableLayoutPanel that owns the form's proportions (exposed for the trace and the acceptance check).</summary>
        public TableLayoutPanel Table => this.table;

        /// <summary>The Notes editor (exposed for the acceptance check: it must span both columns).</summary>
        public TextBox NotesEditor => this.txtNotes;

        /// <summary>Second line under the title: "T-1042 · Open · due 2026-09-12".</summary>
        public string Subtitle
        {
            get => this.lblSubtitle.Text;
            set => this.lblSubtitle.Text = value;
        }

        /// <summary>Fills the editors from a repository copy of the ticket.</summary>
        public void ShowTicket(Ticket t)
        {
            this.Subtitle = $"{t.Id} · {t.Status} · due {t.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
            this.txtTitle.Text = t.Title;
            this.cboPriority.SelectedIndex = (int)t.Priority;
            this.cboStatus.SelectedIndex = (int)t.Status;
            this.txtOwner.Text = t.Owner;
            this.dtpDue.Value = t.DueDate;
            this.txtNotes.Text = t.Notes;
        }

        public void Clear()
        {
            this.Subtitle = "Select a ticket in the grid";
            this.txtTitle.Text = string.Empty;
            this.cboPriority.SelectedIndex = -1;
            this.cboStatus.SelectedIndex = -1;
            this.txtOwner.Text = string.Empty;
            this.dtpDue.Value = DateTime.Today;
            this.txtNotes.Text = string.Empty;
        }

        /// <summary>Builds the ticket the editors currently describe. Validation is NOT done here (the repository does it).</summary>
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

        /// <summary>
        /// One trace line with the styles the form is built from and the widths/heights the engine produced:
        /// "TableLayoutPanel 2 cols [Absolute 140 | Percent 100] · 10 rows · notes span 2 · col widths 140,176 · row heights …".
        /// </summary>
        public string Describe()
        {
            string cols = string.Join(" | ", Enumerable.Range(0, this.table.ColumnStyles.Count)
                .Select(i => DescribeStyle(this.table.ColumnStyles[i])));
            string widths = string.Join(",", this.table.GetColumnWidths());
            string heights = string.Join(",", this.table.GetRowHeights());
            return $"TableLayoutPanel {this.table.ColumnCount} cols [{cols}] · {this.table.RowCount} rows · notes span {this.table.GetColumnSpan(this.txtNotes)} · col widths {widths} · row heights {heights} · panel {this.table.Width}×{this.table.Height}";
        }

        private static string DescribeStyle(TableLayoutStyle style)
        {
            var column = style as ColumnStyle;
            var row = style as RowStyle;
            float size = column != null ? column.Width : row != null ? row.Height : 0F;
            return style.SizeType == SizeType.AutoSize ? "AutoSize" : $"{style.SizeType} {size:0}";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
