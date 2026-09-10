using System.Collections.Generic;
using OperationsConsole.Models;
using OperationsConsole.Orders;
using OperationsConsole.Shell;

namespace OperationsConsole.Services
{
    /// <summary>
    /// The page cache that sits between the grid's <c>CellValueNeeded</c> and <see cref="OrderService"/>.
    /// <para>
    /// Virtual mode on its own only stops the grid from creating rows nobody looks at. <c>CellValueNeeded</c> still
    /// fires <b>once per visible cell</b> — fourteen rows of six columns is eighty-four events for one screen — so a
    /// handler that queries the service directly turns one scroll into hundreds of queries. This cache absorbs that:
    /// a cell is answered from a page already in memory, and only a row outside every held page causes a fetch, which
    /// then brings a whole page back.
    /// </para>
    /// <para>
    /// It keeps a small number of pages (<see cref="MaxPages"/>) rather than exactly one, because the visible block
    /// often straddles a page boundary and a single-page cache would then re-fetch on every second row. Pages are
    /// dropped oldest-first, and the whole cache is thrown away whenever the filter changes — a page only means
    /// anything together with the filter it was fetched for.
    /// </para>
    /// <para>
    /// The cache never throws: when the service fails it records the failure in <see cref="LastError"/>, answers null
    /// for those cells and lets the screen decide what to tell the user. An exception escaping a cell handler would
    /// leak internals into the browser, which the lab explicitly forbids.
    /// </para>
    /// </summary>
    public class OrderCache
    {
        private readonly OrderService _service;
        private readonly List<OrderPage> _pages = new List<OrderPage>();
        private OrderFilter _filter = new OrderFilter();

        public OrderCache(OrderService service, int pageSize = 200)
        {
            _service = service;
            PageSize = pageSize < 20 ? 20 : pageSize;
        }

        /// <summary>How many rows one fetch brings back.</summary>
        public int PageSize { get; }

        /// <summary>How many pages are held at once before the oldest is dropped.</summary>
        public int MaxPages { get; set; } = 4;

        /// <summary>Cells answered from memory.</summary>
        public int Hits { get; private set; }

        /// <summary>Cells that were outside every held page.</summary>
        public int Misses { get; private set; }

        /// <summary>Pages fetched from the service since the last <see cref="Reset"/>.</summary>
        public int Fetches { get; private set; }

        /// <summary>Total rows the current filter matches, as the last fetch reported it.</summary>
        public int TotalCount { get; private set; }

        /// <summary>The last failure message, or null. Cleared by <see cref="ClearError"/> and by <see cref="Reset"/>.</summary>
        public string LastError { get; private set; }

        /// <summary>Pages currently in memory — shown in the status strip.</summary>
        public int PagesInMemory => _pages.Count;

        /// <summary>
        /// Throws every page away and primes the cache for a filter. Called whenever the filter, the data or the mode
        /// changes: a stale page is worse than an empty cache.
        /// </summary>
        public void Reset(OrderFilter filter)
        {
            _filter = (filter ?? new OrderFilter()).Clone();
            _pages.Clear();
            Hits = 0;
            Misses = 0;
            Fetches = 0;
            TotalCount = 0;
            LastError = null;
        }

        /// <summary>Forgets the last failure so the next attempt can report a fresh outcome.</summary>
        public void ClearError()
        {
            LastError = null;
        }

        /// <summary>
        /// Serves one cell — this is the whole body of <c>ordersGrid_CellValueNeeded</c>.
        /// </summary>
        public object GetValue(int rowIndex, int columnIndex) => OrderColumns.ValueOf(RowAt(rowIndex), columnIndex);

        /// <summary>The row behind a grid row index, or null when it could not be fetched.</summary>
        public OrderRow RowAt(int rowIndex)
        {
            if (rowIndex < 0)
                return null;

            var page = Find(rowIndex);
            if (page != null)
            {
                Hits++;
                return page[rowIndex];
            }

            Misses++;
            page = Fetch(rowIndex);
            return page == null ? null : page[rowIndex];
        }

        /// <summary>
        /// Pre-loads the block the grid is about to read — the body of <c>ordersGrid_DataRead</c>.
        /// <c>DataRead</c> is the documented hook for this: it names the first and last row the client asked for, so
        /// the pages are already in memory before the first <c>CellValueNeeded</c> of that block arrives.
        /// Returns false when the service failed (see <see cref="LastError"/>).
        /// </summary>
        public bool Prefetch(int firstIndex, int lastIndex)
        {
            if (lastIndex < firstIndex)
                lastIndex = firstIndex;

            var index = firstIndex;
            while (index <= lastIndex)
            {
                var page = Find(index);
                if (page == null)
                {
                    page = Fetch(index);
                    if (page == null)
                        return false;               // the service is down; the screen reports it once
                }

                if (page.Rows.Count == 0)
                    break;                          // past the end of the result

                index = page.NextIndex;
            }

            return true;
        }

        /// <summary>A one-line summary for the Event log and the status strip.</summary>
        public string Describe() =>
            Fetches + " page fetch" + (Fetches == 1 ? "" : "es") + " · " +
            Hits + " cache hits · " + Misses + " misses · " +
            PagesInMemory + "/" + MaxPages + " pages in memory";

        // ------------------------------------------------------------------------------------------------------------

        private OrderPage Find(int rowIndex)
        {
            for (var i = 0; i < _pages.Count; i++)
            {
                if (_pages[i].Contains(rowIndex))
                    return _pages[i];
            }

            return null;
        }

        private OrderPage Fetch(int rowIndex)
        {
            var first = rowIndex / PageSize * PageSize;

            try
            {
                var page = _service.GetPage(_filter, first, PageSize);

                TotalCount = page.TotalCount;
                _pages.Add(page);
                Fetches++;
                LastError = null;

                if (_pages.Count > MaxPages)
                {
                    var dropped = _pages[0];
                    _pages.RemoveAt(0);
                    ConsoleLog.Add("OrderCache dropped the oldest page (" + dropped + ") — " + MaxPages + " pages is the ceiling");
                }

                ConsoleLog.Add("OrderCache miss on row " + rowIndex + " → fetched " + page + " (" + Describe() + ")");
                return page;
            }
            catch (OrderServiceException ex)
            {
                LastError = ex.Message;
                ConsoleLog.Add("✗ OrderCache could not fetch the page holding row " + rowIndex + " — " + ex.GetType().Name);
                ConsoleLog.Add("   " + ex.Message);
                return null;
            }
        }
    }
}
