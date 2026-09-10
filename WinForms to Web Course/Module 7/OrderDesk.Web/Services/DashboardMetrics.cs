using System;
using System.Collections.Generic;
using System.Linq;
using OrderDesk.Domain;

namespace OrderDesk.Services
{
    /// <summary>
    /// The KPI numbers of the operations dashboard, computed from the same OrderService the WinForms app
    /// called. Pure C# — no control involved — so the same class could feed a report or an API.
    /// "Today" is the store's business date (the newest order date), so the numbers are deterministic.
    /// </summary>
    public sealed class DashboardMetrics
    {
        public int OpenOrders { get; private set; }
        public int OrdersToday { get; private set; }
        public decimal RevenueToday { get; private set; }
        public DateTime BusinessDate { get; private set; }
        public int Invoiced { get; private set; }
        public int Shipped { get; private set; }
        public int Hold { get; private set; }
        public double OnTimePercent { get; private set; }
        public Dictionary<OrderStatus, int> ByStatus { get; private set; }
        public int TotalOrders { get; private set; }

        public static DashboardMetrics Compute(OrderService service)
        {
            var all = service.GetAll();
            var byStatus = service.CountByStatus();
            var m = new DashboardMetrics
            {
                ByStatus = byStatus,
                TotalOrders = all.Count,
                OpenOrders = byStatus.TryGetValue(OrderStatus.Open, out var open) ? open : 0,
                Invoiced = byStatus.TryGetValue(OrderStatus.Invoiced, out var inv) ? inv : 0,
                Shipped = byStatus.TryGetValue(OrderStatus.Shipped, out var sh) ? sh : 0,
                Hold = byStatus.TryGetValue(OrderStatus.Hold, out var hold) ? hold : 0,
            };
            m.BusinessDate = all.Count == 0 ? DateTime.Today : all.Max(o => o.Date);
            var today = all.Where(o => o.Date == m.BusinessDate).ToList();
            m.OrdersToday = today.Count;
            m.RevenueToday = today.Sum(o => o.Total);
            int done = m.Shipped + m.Invoiced;
            m.OnTimePercent = done + m.Hold == 0 ? 100 : Math.Round(100.0 * done / (done + m.Hold), 1);
            return m;
        }

        public int Count(OrderStatus status) => ByStatus != null && ByStatus.TryGetValue(status, out var n) ? n : 0;
    }
}
