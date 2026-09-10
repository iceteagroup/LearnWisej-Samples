using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using OrderDesk.Domain;
using OrderDesk.Services;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk.Pages
{
    /// <summary>
    /// The orders grid ported the WinForms way — the OrdersForm grid moved to Wisej.Web with its columns
    /// kept and <c>DataSource = service.GetAll()</c> (✕ every row, like <c>OrdersForm.ReloadGrid</c>).
    /// Works with 100 rows in a lab; Module 5 measures what it does with 20,000 and 200,000.
    /// </summary>
    public sealed class NaiveOrdersPanel : Panel
    {
        private readonly DataGridView grid;
        private readonly Label footer;
        private readonly DataGridViewTextBoxColumn colId;
        private readonly DataGridViewTextBoxColumn colCustomer;
        private readonly DataGridViewTextBoxColumn colOwner;
        private readonly DataGridViewTextBoxColumn colTotal;
        private readonly DataGridViewTextBoxColumn colStatus;

        /// <summary>Raised on a row double-click (the WinForms edit gesture, kept).</summary>
        public event Action<Order> EditRequested;

        public NaiveOrdersPanel()
        {
            this.BackColor = Palette.CardBackground;

            colId = new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "Order", DataPropertyName = "Id", Width = 80 };
            colCustomer = new DataGridViewTextBoxColumn { Name = "colCustomer", HeaderText = "Customer", DataPropertyName = "CustomerName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            colOwner = new DataGridViewTextBoxColumn { Name = "colOwner", HeaderText = "Owner", DataPropertyName = "Owner", Width = 80 };
            colTotal = new DataGridViewTextBoxColumn { Name = "colTotal", HeaderText = "Total", DataPropertyName = "Total", Width = 110 };
            colTotal.DefaultCellStyle.Format = "C2";
            colTotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colStatus = new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Status", DataPropertyName = "Status", Width = 90 };

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None,
                NoDataMessage = "Not loaded — click  Naive load (20k) ✕  or  Bind all 200k ✕",
            };
            grid.Columns.AddRange(new DataGridViewColumn[] { colId, colCustomer, colOwner, colTotal, colStatus });
            grid.CellDoubleClick += Grid_CellDoubleClick;

            footer = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Bottom,
                Height = 26,
                Padding = new Padding(10, 0, 10, 0),
                ForeColor = Palette.MutedText,
                Font = new System.Drawing.Font("default", 9F),
                Text = "DataSource = service.GetAll()  ·  the WinForms way: nothing loaded yet",
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            };

            this.Controls.Add(grid);
            this.Controls.Add(footer);
        }

        public int RowsInGrid => grid.RowCount;

        /// <summary>The current row's order, else the first row, else null.</summary>
        public Order SelectedOrder
        {
            get
            {
                var current = grid.CurrentRow?.DataBoundItem as Order;
                if (current != null) return current;
                return grid.RowCount > 0 ? grid.Rows[0].DataBoundItem as Order : null;
            }
        }

        /// <summary>
        /// Binds the list exactly as OrdersForm did (<c>DataSource = rows</c>) and measures the server cost.
        /// <paramref name="rowsFetched"/> is how many rows the store handed back (GetAll = the whole table).
        /// </summary>
        public LoadMetrics Bind(List<Order> rows, string mode, int rowsFetched, double fetchMs)
        {
            long before = GC.GetTotalMemory(false);
            var sw = Stopwatch.StartNew();
            grid.BeginUpdate();
            try
            {
                grid.DataSource = null;
                grid.DataSource = rows;
            }
            finally
            {
                grid.EndUpdate();
            }
            sw.Stop();
            long after = GC.GetTotalMemory(false);

            var metrics = new LoadMetrics
            {
                Mode = mode,
                Query = "GetAll() → Take(" + rows.Count.ToString("N0") + ")",
                RowsFetched = rowsFetched,
                RowsInGrid = rows.Count,
                RowsShipped = rows.Count,
                ServerMs = fetchMs + sw.Elapsed.TotalMilliseconds,
                MemoryDeltaBytes = after - before,
            };
            footer.Text = "DataSource = list  ·  " + rows.Count.ToString("N0") + " of " + rowsFetched.ToString("N0") + " rows bound in "
                        + metrics.ServerMs.ToString("0", CultureInfo.InvariantCulture) + " ms  ·  Δ heap " + PayloadEstimate.Signed(metrics.MemoryDeltaBytes)
                        + "  ·  est. payload ~" + PayloadEstimate.Human(metrics.EstimatedPayloadBytes);
            return metrics;
        }

        public void Clear()
        {
            grid.DataSource = null;
            footer.Text = "DataSource = service.GetAll()  ·  cleared";
        }

        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var order = grid.Rows[e.RowIndex].DataBoundItem as Order;
            if (order != null) EditRequested?.Invoke(order);
        }
    }
}
