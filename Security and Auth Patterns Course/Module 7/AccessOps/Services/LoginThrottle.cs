using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AccessOps.Services
{
    /// <summary>
    /// Refuses an account for a lockout period after five failures inside the window, and an
    /// address after twenty (credential stuffing tries many names from one place). Success resets
    /// the account's counter.
    ///
    /// Process memory: correct for one AccessOps server. Behind a load balancer the counters
    /// must live in a store every server shares, or the limit multiplies by the server count.
    /// </summary>
    public sealed class LoginThrottle
    {
        public const int MaxAccountFailures = 5;
        public const int MaxAddressFailures = 20;
        public static readonly TimeSpan Window = TimeSpan.FromMinutes(15);
        public static readonly TimeSpan Lockout = TimeSpan.FromMinutes(15);

        private readonly ConcurrentDictionary<string, Counter> _accounts = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, Counter> _addresses = new(StringComparer.Ordinal);
        private readonly Func<DateTime> _clock;

        public LoginThrottle() : this(() => DateTime.UtcNow) { }

        public LoginThrottle(Func<DateTime> clock) => _clock = clock;

        public bool IsLocked(string userName, string address)
        {
            var now = _clock();
            return IsLocked(_accounts, userName, now) || IsLocked(_addresses, address, now);
        }

        /// <summary>Failures counted against the account inside the current window.</summary>
        public int FailuresFor(string userName)
            => _accounts.TryGetValue(userName ?? "", out var c) ? c.Count(_clock()) : 0;

        public void RegisterFailure(string userName, string address)
        {
            var now = _clock();
            Register(_accounts, userName, now, MaxAccountFailures);
            Register(_addresses, address, now, MaxAddressFailures);
        }

        public void Reset(string userName) => _accounts.TryRemove(userName ?? "", out _);

        private static bool IsLocked(ConcurrentDictionary<string, Counter> map, string key, DateTime now)
            => map.TryGetValue(key ?? "", out var c) && c.IsLocked(now);

        private static void Register(ConcurrentDictionary<string, Counter> map, string key, DateTime now, int max)
            => map.GetOrAdd(key ?? "", _ => new Counter()).Add(now, max);

        private sealed class Counter
        {
            private readonly Queue<DateTime> _failures = new();
            private DateTime _lockedUntil;

            public bool IsLocked(DateTime now)
            {
                lock (this) return now < _lockedUntil;
            }

            public int Count(DateTime now)
            {
                lock (this)
                {
                    Trim(now);
                    return _failures.Count;
                }
            }

            public void Add(DateTime now, int max)
            {
                lock (this)
                {
                    Trim(now);
                    _failures.Enqueue(now);
                    if (_failures.Count >= max)
                        _lockedUntil = now + Lockout;
                }
            }

            private void Trim(DateTime now)
            {
                while (_failures.Count > 0 && now - _failures.Peek() > Window)
                    _failures.Dequeue();
            }
        }
    }
}
