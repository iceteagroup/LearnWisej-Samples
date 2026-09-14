using System;
using System.Collections.Generic;
using System.Linq;
using WisejPerfLab.Models;

namespace WisejPerfLab.Services
{
    /// <summary>
    /// The page cache between the grid's <c>CellValueNeeded</c> and <see cref="TicketQueryService"/>.
    /// </summary>
    /// <remarks>
    /// <c>CellValueNeeded</c> fires once per visible cell — six times per row, dozens of rows on screen.
    /// A handler that queried the service directly would turn one scroll into hundreds of queries, which
    /// is the classic way of making a virtual grid slower than the bound one it replaced. Here a cell is
    /// answered from a page already in memory; only a row outside every held page causes a fetch, and a
    /// fetch brings back a whole page.
    /// <para>
    /// Three rules keep it honest: no database call per cell, no formatting in the handler (the rows
    /// arrive formatted), and no lock around the fetch — the browser can ask for several blocks of rows
    /// at once while scrolling, and a lock would serialise them into a stutter.
    /// </para>
    /// </remarks>
    public sealed class TicketPageCache
    {
        private readonly TicketQueryService _service;
        private readonly List<TicketPage> _pages = new List<TicketPage>();

        private TicketFilter _filter = new TicketFilter();

        public TicketPageCache(TicketQueryService service, int pageSize = 200)
        {
            _service = service;
            PageSize = pageSize < 20 ? 20 : pageSize;
        }

        /// <summary>How many rows one fetch brings back.</summary>
        public int PageSize { get; }

        /// <summary>How many pages are held before the oldest is dropped.</summary>
        public int MaxPages { get; set; } = 4;

        /// <summary>Pages fetched since the last <see cref="Reset"/>.</summary>
        public int Fetches { get; private set; }

        /// <summary>Statements issued since the last <see cref="Reset"/>.</summary>
        public int QueryCount { get; private set; }

        /// <summary>The last failure, or null. The cache never throws at the grid.</summary>
        public string LastError { get; private set; }

        /// <summary>Throws every page away. Called whenever the filter changes.</summary>
        public void Reset(TicketFilter filter)
        {
            _filter = filter ?? new TicketFilter();
            _pages.Clear();
            Fetches = 0;
            QueryCount = 0;
            LastError = null;
        }

        /// <summary>Serves one cell — the whole body of <c>CellValueNeeded</c>.</summary>
        public object GetValue(int rowIndex, int columnIndex)
        {
            var row = RowAt(rowIndex);
            if (row == null)
                return null;

            switch (columnIndex)
            {
                case 0: return row.Number;
                case 1: return row.Customer;
                case 2: return row.Status;
                case 3: return row.Priority;
                case 4: return row.AgeText;
                case 5: return row.UpdatedText;
                default: return null;
            }
        }

        /// <summary>The row at a grid index, fetching its page if no held page contains it.</summary>
        public TicketGridRow RowAt(int rowIndex)
        {
            if (rowIndex < 0)
                return null;

            var page = _pages.FirstOrDefault(p => rowIndex >= p.FirstIndex && rowIndex < p.FirstIndex + p.Rows.Count);
            if (page != null)
                return page.Rows[rowIndex - page.FirstIndex];

            var first = rowIndex / PageSize * PageSize;
            if (!Fetch(first))
                return null;

            page = _pages.FirstOrDefault(p => rowIndex >= p.FirstIndex && rowIndex < p.FirstIndex + p.Rows.Count);
            return page == null ? null : page.Rows[rowIndex - page.FirstIndex];
        }

        /// <summary>
        /// Fetches the pages covering a block of rows the client is about to read. The grid announces
        /// the block before it asks for the cells, so one fetch serves the whole visible window.
        /// </summary>
        public bool Prefetch(int firstIndex, int lastIndex)
        {
            var ok = true;
            for (var first = firstIndex / PageSize * PageSize; first <= lastIndex; first += PageSize)
                if (!_pages.Any(p => p.FirstIndex == first))
                    ok &= Fetch(first);

            return ok;
        }

        private bool Fetch(int firstIndex)
        {
            try
            {
                var page = _service.GetPage(_filter, firstIndex, PageSize);
                _pages.Add(page);
                Fetches++;
                QueryCount += page.QueryCount;

                while (_pages.Count > MaxPages)
                    _pages.RemoveAt(0);

                LastError = null;
                return true;
            }
            catch (Exception ex)
            {
                // A page that cannot be fetched leaves its cells empty and tells the screen why; it does
                // not throw out of CellValueNeeded, where an exception would take the whole grid down.
                LastError = ex.Message;
                return false;
            }
        }

        public string Describe()
            => $"{_pages.Count} page(s) held   {Fetches} fetch(es)   {QueryCount:N0} statements";
    }
}
