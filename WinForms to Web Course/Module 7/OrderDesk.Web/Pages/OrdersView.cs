using OrderDesk.Domain;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk.Pages
{
    /// <summary>
    /// The Orders screen of the capstone: the five walkthrough orders first, then history, fetched through
    /// the server-side <see cref="OrderQuery"/> (Module 5) — never the whole table. Responsive: the Owner
    /// column hides on Tablet/Phone, Date and PO hide on the Phone.
    /// </summary>
    public sealed class OrdersView : ViewBase
    {
        private readonly Panel _card = Card();
        private readonly Label _title = CardTitle("Orders");
        private readonly Label _note = Lbl("", F(9F), Palette.MutedText);
        private readonly DataGridView _grid = OrdersGrid(true);

        public OrdersView()
        {
            _card.Controls.Add(_title);
            _card.Controls.Add(_note);
            _card.Controls.Add(_grid);
            this.Controls.Add(_card);
        }

        public int RowCount => _grid.Rows.Count;

        public void Load(OrderService service, int take)
        {
            var query = new OrderQuery { Take = take };
            var rows = service.Search(query);
            FillOrders(_grid, rows, true);
            _title.Text = "Orders · " + rows.Count + " of " + service.Store.Count;
            _note.Text = "OrderService.Search(" + query + ") — the grid never binds the full table (Module 5).";
        }

        public override void ApplyLayout(LayoutMode mode)
        {
            SetColumnVisible(_grid, "Owner", mode == LayoutMode.Desktop);
            SetColumnVisible(_grid, "Date", mode != LayoutMode.Phone);
            SetColumnVisible(_grid, "PO", mode != LayoutMode.Phone);
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
