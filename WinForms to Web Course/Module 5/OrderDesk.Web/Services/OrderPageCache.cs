using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using OrderDesk.Domain;

namespace OrderDesk.Services
{
    /// <summary>
    /// The page cache behind the VirtualMode grid. The grid asks for one cell at a time
    /// (<c>CellValueNeeded</c>); this class turns that into one <see cref="OrderService.Search"/> call per
    /// block of <see cref="BlockSize"/> rows (<c>Skip/Take</c> on the server-side <see cref="OrderQuery"/>)
    /// and remembers the blocks it already has. One cache per query: change the filter, sort or search
    /// and MainPage builds a new one — nothing per-user is ever kept in a static.
    /// </summary>
    public sealed class OrderPageCache
    {
        private readonly OrderService _service;
        private readonly OrderQuery _query;
        private readonly Dictionary<int, List<Order>> _blocks = new Dictionary<int, List<Order>>();
        private readonly Action<string, string> _log;

        public OrderPageCache(OrderService service, OrderQuery query, int blockSize, Action<string, string> log)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _query = (query ?? new OrderQuery()).Clone();
            _query.Skip = 0;
            _query.Take = int.MaxValue;
            BlockSize = Math.Max(1, blockSize);
            _log = log ?? ((n, p) => { });
        }

        public int BlockSize { get; }
        public OrderQuery Query => _query;

        /// <summary>Rows matching the query (from <see cref="Count"/>).</summary>
        public int TotalRows { get; private set; }
        public double CountMs { get; private set; }
        public int BlocksFetched { get; private set; }
        public int RowsFetched { get; private set; }
        public double FetchMs { get; private set; }
        public int CachedBlocks => _blocks.Count;

        /// <summary>Server-side count → becomes the grid's RowCount.</summary>
        public int Count()
        {
            var sw = Stopwatch.StartNew();
            TotalRows = _service.Count(_query);
            sw.Stop();
            CountMs = sw.Elapsed.TotalMilliseconds;
            _log("OrderService.Count(query)", _query + " → " + TotalRows.ToString("N0") + " rows · " + CountMs.ToString("0.0", CultureInfo.InvariantCulture) + " ms");
            return TotalRows;
        }

        /// <summary>The order at a virtual row index, fetching its block on first use.</summary>
        public Order Get(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= TotalRows) return null;
            int block = rowIndex / BlockSize;
            var rows = GetBlock(block);
            int i = rowIndex - block * BlockSize;
            return i < rows.Count ? rows[i] : null;
        }

        /// <summary>Called from the grid's DataRead event: fetch every block the client is about to show.</summary>
        public void Prefetch(int firstIndex, int lastIndex)
        {
            if (TotalRows == 0) return;
            firstIndex = Math.Max(0, firstIndex);
            lastIndex = Math.Min(TotalRows - 1, lastIndex);
            for (int b = firstIndex / BlockSize; b <= lastIndex / BlockSize; b++)
                GetBlock(b);
        }

        private List<Order> GetBlock(int block)
        {
            if (_blocks.TryGetValue(block, out var cached)) return cached;

            var q = _query.Clone();
            q.Skip = block * BlockSize;
            q.Take = BlockSize;

            var sw = Stopwatch.StartNew();
            var rows = _service.Search(q);
            sw.Stop();

            BlocksFetched++;
            RowsFetched += rows.Count;
            FetchMs += sw.Elapsed.TotalMilliseconds;
            _blocks[block] = rows;
            _log("OrderService.Search(query)", q + " → " + rows.Count + " rows · " + sw.Elapsed.TotalMilliseconds.ToString("0.0", CultureInfo.InvariantCulture) + " ms · block " + block);
            return rows;
        }

        /// <summary>
        /// The summary the footer shows. Computed on the server over EVERY matching row — a client-side sum
        /// over the 50 rows that happen to be visible would be wrong, which is why the summary is a query, not a grid feature, in VirtualMode.
        /// </summary>
        public decimal SumTotal(out double ms)
        {
            var sw = Stopwatch.StartNew();
            var all = _service.Search(_query);
            var sum = all.Sum(o => o.Total);
            sw.Stop();
            ms = sw.Elapsed.TotalMilliseconds;
            return sum;
        }
    }
}
