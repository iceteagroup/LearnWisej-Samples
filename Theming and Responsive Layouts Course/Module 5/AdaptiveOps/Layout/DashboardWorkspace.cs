using System.Drawing;
using AdaptiveOps.Shell;
using Wisej.Web;
using Wisej.Web.Markup;

namespace AdaptiveOps.Layout
{
    /// <summary>
    /// The list / details split of the workspace as a FlexLayoutPanel — the one region of the console
    /// built entirely in code with the Wisej.Web.Markup fluent extensions. There is no Designer file
    /// for this class on purpose: the container reads top to bottom as one chain
    /// (LayoutStyle → Spacing → Controls → FillWeight → FillWeight → AlignY) and nothing in it uses a
    /// second notation (no SetFillWeight / SetAlignY calls, no designer serialisation) for the same
    /// properties.
    ///
    /// Behaviour: the list region takes two thirds of the spare width (FillWeight 2), the details
    /// region one third (FillWeight 1). Both carry a MinimumSize, so a narrow browser stops the details
    /// region at 280 px instead of shrinking it to zero; the details region also carries a MaximumSize
    /// (520 px wide) so a very wide monitor does not stretch the form into whitespace, and AlignY Top so
    /// that, whenever it does not fill the height, it hugs the top edge instead of centring. The flex
    /// engine honours each child's Margin; the gap between the two regions is the container's Spacing.
    ///
    /// The DataGridView columns are plain object initialisers: Wisej.Web.Markup has no grid extensions,
    /// and the grid is content, not layout.
    /// </summary>
    public class DashboardWorkspace : FlexLayoutPanel
    {
        /// <summary>The list region: a white card with the workspace title and the ticket grid.</summary>
        public Panel ListRegion { get; }

        /// <summary>"Tickets" — retitled by the navigation rail.</summary>
        public Label ListTitle { get; }

        /// <summary>The ticket grid (filled by the page from the repository).</summary>
        public DataGridView Grid { get; }

        /// <summary>The details region: a white card hosting the TableLayoutPanel editor; scrolls when it is shorter than the form.</summary>
        public Panel DetailsRegion { get; }

        /// <summary>The ticket editor (Shell/TicketEditor, a TableLayoutPanel form).</summary>
        public TicketEditor Editor { get; }

        public DashboardWorkspace()
        {
            // The list region. The grid is added before the title so it docks LAST and fills what the title leaves.
            this.Grid = CreateGrid();
            // (AutoSize and BorderStyle have no unambiguous Markup extension for Label / Panel in Wisej-4 4.1.0 —
            //  AutoSize is defined by both ControlExtensions and LabelExtensions, BorderStyle only for Button —
            //  so those two are object-initialiser properties; everything layout-related stays in the chain.)
            this.ListTitle = new Label { AutoSize = false }
                .Name("lblWorkspaceTitle")
                .Text("Tickets")
                .Dock(DockStyle.Top)
                .Size(300, 26)
                .Font(new Font("default", 11F, FontStyle.Bold))
                .TextAlign(ContentAlignment.MiddleLeft);
            this.ListRegion = new Panel { BorderStyle = BorderStyle.Solid }
                .Name("listRegion")
                .BackColor(Color.White)
                .Padding(new Padding(8))
                .MinimumSize(300, 200)
                .Controls(new Control[] { this.Grid, this.ListTitle });

            // The details region: the TableLayoutPanel editor fills a scrollable white card.
            this.Editor = new TicketEditor()
                .Name("ticketEditor")
                .Dock(DockStyle.Fill);
            this.DetailsRegion = new Panel { BorderStyle = BorderStyle.Solid }
                .Name("detailsRegion")
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
                .Controls(new Control[] { this.ListRegion, this.DetailsRegion })
                .FillWeight(this.ListRegion, 2)
                .FillWeight(this.DetailsRegion, 1)
                .AlignY(this.DetailsRegion, VerticalAlignment.Top);
        }

        /// <summary>True when the flex engine has pushed the details region down to its MinimumSize (the phone-width case).</summary>
        public bool DetailsAtMinimum => this.DetailsRegion.Width <= this.DetailsRegion.MinimumSize.Width;

        /// <summary>True when the details region is held at its MaximumSize (the 4K-monitor case).</summary>
        public bool DetailsAtMaximum => this.DetailsRegion.MaximumSize.Width > 0 && this.DetailsRegion.Width >= this.DetailsRegion.MaximumSize.Width;

        /// <summary>
        /// One trace line with the extended properties read back from the container and the sizes the engine produced:
        /// "FlexLayoutPanel Horizontal · Spacing 8 · list FillWeight 2 min 300×200 → 484×372 · details FillWeight 1 min 280×200 max 520 AlignY Top → 280×372 (at minimum) · workspace 780×372".
        /// </summary>
        public string Describe()
        {
            string detailsNote = DetailsAtMinimum ? " (at MinimumSize)" : DetailsAtMaximum ? " (at MaximumSize)" : string.Empty;
            return $"FlexLayoutPanel {this.LayoutStyle} · Spacing {this.Spacing} · "
                 + $"list FillWeight {this.GetFillWeight(this.ListRegion)} min {this.ListRegion.MinimumSize.Width}×{this.ListRegion.MinimumSize.Height} → {this.ListRegion.Width}×{this.ListRegion.Height} · "
                 + $"details FillWeight {this.GetFillWeight(this.DetailsRegion)} min {this.DetailsRegion.MinimumSize.Width}×{this.DetailsRegion.MinimumSize.Height} max {this.DetailsRegion.MaximumSize.Width} AlignY {this.GetAlignY(this.DetailsRegion)} → {this.DetailsRegion.Width}×{this.DetailsRegion.Height}{detailsNote} · "
                 + $"workspace {this.Width}×{this.Height}";
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
