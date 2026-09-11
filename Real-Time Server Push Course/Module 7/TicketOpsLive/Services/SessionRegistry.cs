using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TicketOpsLive
{
    /// <summary>
    /// A GLOBAL service: one instance for the whole server process, shared by every session.
    ///
    /// Rules it follows (lesson: "global services must be thread-safe and must not store session controls
    /// without a defined subscription lifecycle"):
    ///   - it stores values only (client id + start time), never a MainPage, a control or an IWisejComponent;
    ///   - every access to the dictionary is under a lock;
    ///   - SessionsChanged is raised on a thread-pool thread, outside the lock and outside any session
    ///     context — the subscriber has to restore its own context with Application.Update(context, …),
    ///     exactly like it would for a queue, a hub or a file watcher;
    ///   - a failing subscriber is logged and does not stop the others.
    ///
    /// Subscribers own their subscription: MainPage subscribes on Load and unsubscribes on
    /// ApplicationExit / Disposed (see Cleanup() in MainPage.cs).
    /// </summary>
    public sealed class SessionRegistry
    {
        private static readonly Lazy<SessionRegistry> _instance =
            new Lazy<SessionRegistry>(() => new SessionRegistry(), LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>The single, lazily created, thread-safe instance.</summary>
        public static SessionRegistry Instance => _instance.Value;

        private readonly object _gate = new object();
        private readonly Dictionary<string, SessionInfo> _sessions = new Dictionary<string, SessionInfo>(StringComparer.Ordinal);

        private SessionRegistry()
        {
        }

        /// <summary>Raised after a session joined or left. Runs on a thread-pool thread — no session context.</summary>
        public event EventHandler<SessionsChangedEventArgs> SessionsChanged;

        /// <summary>Number of sessions currently registered.</summary>
        public int Count
        {
            get { lock (_gate) return _sessions.Count; }
        }

        /// <summary>Registers a session by its client id. Returns false when the id was already known.</summary>
        public bool Register(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                return false;

            int count;
            lock (_gate)
            {
                if (_sessions.ContainsKey(sessionId))
                    return false;

                _sessions.Add(sessionId, new SessionInfo(sessionId, DateTime.Now));
                count = _sessions.Count;
            }

            Raise(new SessionsChangedEventArgs(sessionId, SessionChange.Joined, count));
            return true;
        }

        /// <summary>Removes a session. Safe to call twice (ApplicationExit and Disposed both call it).</summary>
        public bool Unregister(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                return false;

            int count;
            lock (_gate)
            {
                if (!_sessions.Remove(sessionId))
                    return false;

                count = _sessions.Count;
            }

            Raise(new SessionsChangedEventArgs(sessionId, SessionChange.Left, count));
            return true;
        }

        /// <summary>A copy of the registered sessions, oldest first. The caller can never touch the live dictionary.</summary>
        public SessionInfo[] GetSnapshot()
        {
            lock (_gate)
                return _sessions.Values.OrderBy(s => s.StartedAt).ToArray();
        }

        private void Raise(SessionsChangedEventArgs e)
        {
            var handlers = SessionsChanged;
            if (handlers == null)
                return;

            // Outside the lock and off the caller's request thread: the session that joined must not wait for
            // every other session to render, and no session lock is held while foreign sessions are updated.
            Task.Run(() =>
            {
                foreach (EventHandler<SessionsChangedEventArgs> handler in handlers.GetInvocationList())
                {
                    try
                    {
                        handler(this, e);
                    }
                    catch (Exception ex)
                    {
                        // One broken subscriber must not break the others: log and continue.
                        Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} SessionRegistry: a subscriber failed while handling {e.Change} of {e.SessionId}: {ex}");
                    }
                }
            });
        }
    }
}
