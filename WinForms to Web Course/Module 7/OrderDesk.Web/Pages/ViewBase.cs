using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using OrderDesk.Domain;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk.Pages
{
    /// <summary>The three layouts the capstone supports — one per client profile family (docs/client-profiles.md).</summary>
    public enum LayoutMode
    {
        Desktop,
        Tablet,
        Phone
    }

    /// <summary>
    /// Base of the five navigation views (Dashboard · Orders · Customers · Reports · Settings). Each view is a
    /// Panel hosted Dock=Fill in MainPage's view area; <see cref="ApplyLayout"/> is the code form of the
    /// designer's responsive properties (docs/client-profiles.md lists what each profile changes).
    /// </summary>
    public abstract class ViewBase : Panel
    {
        public static readonly CultureInfo Us = CultureInfo.GetCultureInfo("en-US");

        protected const int Pad = 10;
        protected LayoutMode Mode = LayoutMode.Desktop;

        protected ViewBase()
        {
            this.BackColor = Palette.PageBackground;
            this.Dock = DockStyle.Fill;
        }

        /// <summary>Applies the layout for a profile family and re-lays the view out.</summary>
        public virtual void ApplyLayout(LayoutMode mode)
        {
            Mode = mode;
            Relayout();
        }

        /// <summary>Positions the children for the current size and mode. Called on Resize and on profile changes.</summary>
        protected abstract void Relayout();

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            Relayout();
        }

        // ---- small factories so the views read like a designer file --------------------------------------

        protected static Font F(float size, bool bold = false)
            => new Font("default", size, bold ? FontStyle.Bold : FontStyle.Regular);

        protected static Font Mono(float size, bool bold = false)
            => new Font("monospace", size, bold ? FontStyle.Bold : FontStyle.Regular);

        protected static Label Lbl(string text, Font font, Color fore, ContentAlignment align = ContentAlignment.MiddleLeft)
            => new Label { AutoSize = false, Text = text, Font = font, ForeColor = fore, TextAlign = align };

        protected static Panel Card()
            => new Panel { BackColor = Palette.CardBackground, BorderStyle = BorderStyle.Solid };

        protected static Label CardTitle(string text)
            => Lbl(text, F(11F, true), Palette.Ink);

        protected static Button Btn(string text, int width)
            => new Button { Text = text, Size = new Size(width, 30) };

        /// <summary>The Recent-orders / Orders grid: Order · Customer · Owner · Total · Status (+ Date · PO when asked).</summary>
        protected static DataGridView OrdersGrid(bool withDateAndPo)
        {
            var grid = new DataGridView
            {
                ReadOnly = true,
                RowHeadersVisible = false,
                AllowUserToResizeColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None,
                Font = F(9F),
            };
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Order", HeaderText = "Order", Width = 64 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Customer", HeaderText = "Customer", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, MinimumWidth = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Owner", HeaderText = "Owner", Width = 70 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Total", Width = 96, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 88, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            if (withDateAndPo)
            {
                grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "Date", Width = 90 });
                grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "PO", HeaderText = "PO", Width = 90 });
            }
            return grid;
        }

        /// <summary>Fills the grid; the Status cell is styled as a chip (soft background, status colour, bold).</summary>
        protected static void FillOrders(DataGridView grid, IEnumerable<Order> orders, bool withDateAndPo)
        {
            grid.Rows.Clear();
            foreach (var o in orders)
            {
                int index = withDateAndPo
                    ? grid.Rows.Add(o.Id, o.CustomerName, o.Owner ?? "—", o.Total.ToString("C2", Us), StatusText(o.Status), o.Date.ToString("yyyy-MM-dd"), o.PoNumber)
                    : grid.Rows.Add(o.Id, o.CustomerName, o.Owner ?? "—", o.Total.ToString("C2", Us), StatusText(o.Status));
                var row = grid.Rows[index];
                row.Tag = o;
                var chip = row.Cells[4].Style;
                chip.ForeColor = Palette.StatusColor(o.Status);
                chip.BackColor = SoftColor(o.Status);
                chip.Font = F(9F, true);
                row.Cells[3].Style.Font = Mono(9F);
                row.Cells[0].Style.ForeColor = Palette.Accent;
                row.Cells[0].Style.Font = Mono(9F, true);
            }
        }

        public static string StatusText(OrderStatus s) => s == OrderStatus.InProgress ? "In progress" : s.ToString();

        public static Color SoftColor(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Shipped: return Palette.GoodSoft;
                case OrderStatus.Invoiced: return Color.FromArgb(240, 236, 252);
                case OrderStatus.InProgress: return Palette.WarnSoft;
                case OrderStatus.Hold: return Palette.BadSoft;
                default: return Palette.AccentSoft;
            }
        }

        protected static void SetColumnVisible(DataGridView grid, string name, bool visible)
        {
            var col = grid.Columns[name];
            if (col != null && col.Visible != visible) col.Visible = visible;
        }
    }
}
