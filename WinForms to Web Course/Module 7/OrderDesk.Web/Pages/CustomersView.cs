using System.Collections.Generic;
using OrderDesk.Domain;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk.Pages
{
    /// <summary>
    /// Customers: Name · Country · Tier · Credit limit. <see cref="Customer.TaxId"/> is sensitive and is never
    /// put in a grid row (it would travel to every browser); the only way out is the Manager-guarded download.
    /// </summary>
    public sealed class CustomersView : ViewBase
    {
        private readonly Panel _card = Card();
        private readonly Label _title = CardTitle("Customers");
        private readonly Label _note = Lbl("TaxId stays on the server — not a column here; export needs the Manager role (Security review → Guarded download).", F(9F), Palette.MutedText);
        private readonly DataGridView _grid = new DataGridView { ReadOnly = true, RowHeadersVisible = false, AllowUserToResizeColumns = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, BorderStyle = BorderStyle.None, Font = F(9F) };

        public CustomersView()
        {
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Customer", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, MinimumWidth = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Country", HeaderText = "Country", Width = 80 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Tier", HeaderText = "Tier", Width = 90 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Credit", HeaderText = "Credit limit", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
            _card.Controls.Add(_title);
            _card.Controls.Add(_note);
            _card.Controls.Add(_grid);
            this.Controls.Add(_card);
        }

        public void Load(IReadOnlyList<Customer> customers)
        {
            _grid.Rows.Clear();
            foreach (var c in customers)
            {
                int i = _grid.Rows.Add(c.Name, c.Country, c.Tier, c.CreditLimit.ToString("C0", Us));
                if (c.Tier == "Gold") _grid.Rows[i].Cells[2].Style.ForeColor = Palette.Warn;
            }
            _title.Text = "Customers · " + customers.Count;
        }

        public override void ApplyLayout(LayoutMode mode)
        {
            SetColumnVisible(_grid, "Country", mode != LayoutMode.Phone);
            base.ApplyLayout(mode);
        }

        protected override void Relayout()
        {
            int w = this.Width - 2 * Pad, h = this.Height - 2 * Pad;
            if (w < 100 || h < 100) return;
            _card.SetBounds(Pad, Pad, w, h);
            _title.SetBounds(12, 6, w - 24, 22);
            _note.SetBounds(12, 28, w - 24, 18);
            _grid.SetBounds(0, 50, w - 2, h - 52);
        }
    }
}
