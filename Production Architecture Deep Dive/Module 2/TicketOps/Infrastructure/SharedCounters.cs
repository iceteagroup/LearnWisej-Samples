using System.Threading;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one piece of <b>mutable</b> application-scope state in this sample, kept deliberately small and
    /// thread-safe (Interlocked). It counts how many SessionContexts this process has created — a number
    /// that is the same for everyone by definition, so sharing it is correct. Note what is not here: no
    /// "last user", no "current tenant" — anything that would differ per session belongs on SessionContext.
    /// </summary>
    public static class SharedCounters
    {
        private static long _contextsCreated;

        public static long ContextsCreated => Interlocked.Read(ref _contextsCreated);

        public static long ContextCreated() => Interlocked.Increment(ref _contextsCreated);
    }
}
