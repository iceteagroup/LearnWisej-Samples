using System.Collections.Generic;
using OrderDesk.Domain;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// The two data-screen habits copied out of LegacyOrderDesk (OrdersForm.ReloadGrid and
    /// EditOrderDialog.saveButton_Click) so the Module 5 console can run them against the
    /// production-sized store and show what breaks. Both were harmless on one desktop with one user
    /// and a LAN database; neither survives a shared server and a browser.
    /// </summary>
    public static class DesktopGridHabits
    {
        /// <summary>
        /// ✕ OrdersForm.ReloadGrid: <c>ordersGrid.DataSource = _orderService.Search(filter)</c> — the
        /// whole result set is materialised and handed to the grid. With 200,000 rows every session
        /// clones the table, sorts it and ships it to a browser that can show 12 rows at a time.
        /// </summary>
        public static IList<Order> LoadWholeTable(OrderService service) => service.GetOrders();

        /// <summary>
        /// ✕ EditOrderDialog.saveButton_Click: the only rule LegacyOrderDesk had lived inside the form
        /// (<c>MessageBox.Show("Select a customer.")</c>) — one message, blocking, unreachable from a
        /// batch import or an API. Returns the desktop message, or null when that single rule passes.
        /// </summary>
        public static string ValidateInsideTheForm(Order order) =>
            order.Customer == null ? "Select a customer." : null;
    }
}
