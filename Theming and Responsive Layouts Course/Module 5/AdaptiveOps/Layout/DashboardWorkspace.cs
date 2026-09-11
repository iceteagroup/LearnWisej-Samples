using System.Drawing;
using AdaptiveOps.Shell;
using Wisej.Web;
using Wisej.Web.Markup;

namespace AdaptiveOps.Layout
{
    /// <summary>
    /// The list / details split of the workspace as a FlexLayoutPanel, built entirely in code with the
    /// Wisej.Web.Markup fluent extensions. There is no Designer file for this class on purpose: the
    /// container reads top to bottom as one chain and nothing in it uses a second notation for the same
    /// properties.
    ///
    /// The list region takes two thirds of the spare width (FillWeight 2), the details region one third
    /// (FillWeight 1). Both carry a MinimumSize, so a narrow browser stops the details region at 280 px
    /// instead of shrinking it to zero; the details region also has a MaximumSize (520 px) and AlignY Top.
    /// </summary>
    public class DashboardWorkspace : FlexLayoutPanel
    {
        private readonly Panel pnlList;
        private readonly Panel detailsPanel;

        /// <summary>"Tickets": retitled by the navigation rail.</summary>
        public Label ListTitle { get; }

        /// <summary>The ticket grid (filled by the page from the repository).</summary>
        public DataGridView Grid { get; }

        /// <summary>The ticket editor (Shell/TicketEditor, a TableLayoutPanel form).</summary>
        public TicketEditor Editor { get; }

        public DashboardWorkspace()
        {
            // The list region. The grid is added before the title so it docks last and fills what the title leaves.
            this.Grid = CreateGrid();
            // AutoSize and BorderStyle have no unambiguous Markup extension for Label / Panel in Wisej-4 4.1.0,
            // so those two are object-initialiser properties; everything layout-related stays in the chain.
            this.ListTitle = new Label { AutoSize = false }
                .Name("lblWorkspaceTitle")
                .Text("Tickets")
                .Dock(DockStyle.Top)
                .Size(300, 26)
                .Font(new Font("default", 11F, FontStyle.Bold))
                .TextAlign(ContentAlignment.MiddleLeft);
            this.pnlList = new Panel { BorderStyle = BorderStyle.Solid }
                .Name("pnlList")
                .BackColor(Color.White)
                .Padding(new Padding(8))
                .MinimumSize(300, 200)
                .Controls(new Control[] { this.Grid, this.ListTitle });

            // The details region: the TableLayoutPanel editor fills a scrollable white card.
            this.Editor = new TicketEditor()
                .Name("ticketEditor")
                .Dock(DockStyle.Fill);
            this.detailsPanel = new Panel { BorderStyle = BorderStyle.Solid }
                .Name("detailsPanel")
                .BackColor(Color.White)
                .AutoScroll(true)
                .MinimumSize(280, 200)
                .MaximumSize(520, 0)
                .Controls(new Control[] { this.Editor });

            // The container itself: one row, weights 2 : 1, bounded by the sizes above.
            this
                .Name("dashboard")
                .Dock(DockStyle.Fill)
                .MinimumSize(320, 240)
                .LayoutStyle(FlexLayoutStyle.Horizontal)
                .Spacing(8)
                .Controls(new Control[] { this.pnlList, this.detailsPanel })
                .FillWeight(this.pnlList, 2)
                .FillWeight(this.detailsPanel, 1)
                .AlignY(this.detailsPanel, VerticalAlignment.Top);
        }

        private static DataGridView CreateGrid()
        {
            var grid = new DataGridView
            {
                Name = "gridTickets",
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Dock = DockStyle.Fill,
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            grid.Columns.AddRange(new DataGridViewColumn[]
            {
                Column("colId", "Id", 60F, 64),
                Column("colTitle", "Title", 240F, 160),
                Column("colPriority", "Priority", 70F, 64),
                Column("colStatus", "Status", 70F, 64),
                Column("colOwner", "Owner", 70F, 64),
                Column("colDue", "Due", 90F, 90)
            });

            return grid;
        }

        private static DataGridViewTextBoxColumn Column(string name, string header, float fillWeight, int minimumWidth)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                FillWeight = fillWeight,
                MinimumWidth = minimumWidth,
                ReadOnly = true
            };
        }
    }
}
