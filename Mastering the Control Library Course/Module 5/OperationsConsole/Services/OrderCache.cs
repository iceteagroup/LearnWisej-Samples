using System.Collections.Generic;
using OperationsConsole.Models;
using OperationsConsole.Orders;

namespace OperationsConsole.Services
{
    /// <summary>
    /// The page cache between the grid's <c>CellValueNeeded</c> and <see cref="OrderService"/>.
    /// <para>
    /// <c>CellValueNeeded</c> fires once per visible cell, so a handler that queried the service directly would turn one
    /// scroll into hundreds of queries. Here a cell is answered from a page already in memory, and only a row outside
    /// every held page causes a fetch, which brings a whole page back. A few pages are kept (oldest dropped first),
    /// and the whole cache is thrown away when the filter changes.
    /// </para>
    /// <para>
    /// The cache never throws: a failure is recorded in <see cref="LastError"/>, those cells answer null, and the
    /// screen decides what to tell the user.
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

        /// <summary>Pages fetched from the service since the last <see cref="Reset"/>.</summary>
        public int Fetches { get; private set; }

        /// <summary>The last failure message, or null.</summary>
        public string LastError { get; private set; }

        /// <summary>Throws every page away and primes the cache for a filter.</summary>
        public void Reset(OrderFilter filter)
        {
            _filter = (filter ?? new OrderFilter()).Clone();
            _pages.Clear();
            Fetches = 0;
            LastError = null;
        }

        /// <summary>Forgets the last failure so the next attempt can report a fresh outcome.</summary>
        public void ClearError()
        {
            LastError = null;
        }

        /// <summary>Serves one cell — the whole body of <c>ordersGrid_CellValueNeeded</c>.</summary>
        public object GetValue(int rowIndex, int columnIndex) => OrderColumns.ValueOf(RowAt(rowIndex), columnIndex);

        /// <summary>The row behind a grid row index, or null when it could not be fetched.</summary>
        public OrderRow RowAt(int rowIndex)
        {
            if (rowIndex < 0)
                return null;

            var page = Find(rowIndex) ?? Fetch(rowIndex);
            return page == null ? null : page[rowIndex];
        }

        /// <summary>
        /// Pre-loads the block the grid is about to read — the body of <c>ordersGrid_DataRead</c>.
        /// Returns false when the service failed.
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
                        return false;
                }

                if (page.Rows.Count == 0)
                    break;

                index = page.NextIndex;
            }

            return true;
        }

        /// <summary>A one-line summary for the status strip.</summary>
        public string Describe() =>
            Fetches + " page fetch" + (Fetches == 1 ? "" : "es") + " · " + _pages.Count + " of " + MaxPages + " pages in memory";

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

                _pages.Add(page);
                Fetches++;
                LastError = null;

                if (_pages.Count > MaxPages)
                    _pages.RemoveAt(0);

                return page;
            }
            catch (OrderServiceException ex)
            {
                LastError = ex.Message;
                return null;
            }
        }
    }
}
