using System.Diagnostics;
using OrderDesk.Domain;

namespace OrderDesk.Services
{
    /// <summary>
    /// The production-like data set of Module 5: 200,000 orders in a PRIVATE <see cref="OrderStore"/>
    /// (<c>OrderStore.Create(200000, 42)</c>, not <c>OrderStore.Shared()</c>), so the 5,000-row shared store
    /// the other modules use is never touched. MainPage builds it inside <c>Application.StartTask</c> so the
    /// page opens instantly, and the seed time is measured and written to the trace.
    ///
    /// The first five orders are still the walkthrough orders (1042 Northwind Traders … 1038 Globex Corp);
    /// the generated history runs downwards from 1037 (the shared seed keeps counting below zero for a
    /// data set this large — harmless, but you will see negative order numbers deep in the list).
    /// </summary>
    public sealed class BigOrderData
    {
        public const int OrderCount = 200_000;
        public const int Seed = 42;

        public OrderStore Store { get; private set; }
        public OrderService Service { get; private set; }
        public long SeedMs { get; private set; }
        public long HeapAfterSeedBytes { get; private set; }

        public bool IsReady => Service != null;

        /// <summary>Seeds the store. Call from a background task; it takes a few hundred milliseconds.</summary>
        public void Build()
        {
            var sw = Stopwatch.StartNew();
            var store = OrderStore.Create(OrderCount, Seed);
            sw.Stop();

            SeedMs = sw.ElapsedMilliseconds;
            HeapAfterSeedBytes = System.GC.GetTotalMemory(false);
            Store = store;
            Service = new OrderService(store);
        }
    }
}
