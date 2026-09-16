using System;

namespace WisejPerfLab.Diagnostics
{
    /// <summary>
    /// One memory reading: the managed heap plus the two counters that say whether a form really went
    /// away. It is the cheap, in-app version of a Memory Usage snapshot — enough to see a leak and to
    /// see it fixed, not enough to find the path to root. That is what Visual Studio is for.
    /// </summary>
    public sealed class MemorySnapshot
    {
        public DateTime TakenAt { get; set; }

        /// <summary>Managed heap after one collection, which is what a profiler snapshot also does.</summary>
        public long HeapBytes { get; set; }

        /// <summary>Handlers on the static <c>GlobalTicketBus.TicketChanged</c> event.</summary>
        public int BusSubscribers { get; set; }

        /// <summary>Detail forms constructed since the process started.</summary>
        public int FormsCreated { get; set; }

        /// <summary>Detail forms that ran their <c>Dispose(true)</c>.</summary>
        public int FormsDisposed { get; set; }

        public int FormsOutstanding => FormsCreated - FormsDisposed;

        public double HeapMb => HeapBytes / 1024d / 1024d;

        public override string ToString()
            => $"heap {HeapMb:N1} MB   bus subscribers {BusSubscribers}   forms created {FormsCreated} / disposed {FormsDisposed}";
    }

    /// <summary>Takes <see cref="MemorySnapshot"/>s.</summary>
    public static class MemoryProbe
    {
        /// <summary>
        /// Reads the managed heap. <paramref name="collect"/> runs a full blocking collection first,
        /// because otherwise the number is "what has been allocated recently", not "what is retained" —
        /// a Memory Usage snapshot does the same thing before it counts.
        /// </summary>
        /// <remarks>
        /// Collecting <b>before a reading</b> is honest. Collecting repeatedly until the number looks
        /// good is not: the question is whether the objects can be collected at all, and an object that
        /// is still rooted survives any number of collections.
        /// </remarks>
        public static MemorySnapshot Take(bool collect = true)
        {
            if (collect)
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
                GC.WaitForPendingFinalizers();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
            }

            return new MemorySnapshot
            {
                TakenAt = DateTime.Now,
                HeapBytes = GC.GetTotalMemory(forceFullCollection: false),
                BusSubscribers = Services.GlobalTicketBus.SubscriberCount,
                FormsCreated = Forms.TicketDetailForm.CreatedCount,
                FormsDisposed = Forms.TicketDetailForm.DisposedCount
            };
        }

        /// <summary>"B - A: heap +24.6 MB, subscribers +50, forms outstanding +50".</summary>
        public static string Compare(MemorySnapshot a, MemorySnapshot b)
        {
            if (a == null || b == null)
                return "take snapshot A first";

            return
                $"B - A: heap {(b.HeapMb - a.HeapMb):+0.0;-0.0;0.0} MB   " +
                $"subscribers {(b.BusSubscribers - a.BusSubscribers):+0;-0;0}   " +
                $"forms outstanding {(b.FormsOutstanding - a.FormsOutstanding):+0;-0;0}";
        }
    }
}
