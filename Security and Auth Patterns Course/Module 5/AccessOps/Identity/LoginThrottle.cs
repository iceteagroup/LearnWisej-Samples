using System;
using System.Collections.Generic;
using AccessOps.Security;

namespace AccessOps.Identity
{
    /// <summary>
    /// Counters of recent sign-in failures per account name and per client address. Five failures
    /// for one name, or twenty from one address, inside a 15-minute window lock that name or
    /// address for 15 minutes. Unknown names are counted exactly like real ones, so the throttle
    /// never tells anybody whether an account exists.
    /// <para>
    /// It lives in the login service because a Wisej.NET click arrives over the session's
    /// WebSocket, where ASP.NET Core rate limiting cannot see it. The counters are in process
    /// memory: correct for one server only - behind a load balancer they move to a shared store.
    /// </para>
    /// </summary>
    public sealed class LoginThrottle
    {
        /// <summary>Failures per account name before the name is locked.</summary>
        public const int AccountLimit = 5;

        /// <summary>Failures per client address before the address is locked.</summary>
        public const int AddressLimit = 20;

        /// <summary>The counting window, and the lockout period.</summary>
        public static readonly TimeSpan Window = TimeSpan.FromMinutes(15);

        private readonly IClock _clock;
        private readonly object _gate = new object();
        private readonly Dictionary<string, Counter> _accounts = new Dictionary<string, Counter>(StringComparer.Ordinal);
        private readonly Dictionary<string, Counter> _addresses = new Dictionary<string, Counter>(StringComparer.Ordinal);

        /// <summary>Creates the throttle on <paramref name="clock"/>.</summary>
        public LoginThrottle(IClock clock)
        {
            _clock = clock;
        }

        /// <summary>True when the account name or the address is locked right now.</summary>
        public bool IsLocked(string accountKey, string addressKey)
        {
            lock (_gate)
            {
                var now = _clock.Now;
                return Locked(_accounts, accountKey, now) || Locked(_addresses, addressKey, now);
            }
        }

        /// <summary>Counts one failure for the name and the address.</summary>
        public void RecordFailure(string accountKey, string addressKey)
        {
            lock (_gate)
            {
                var now = _clock.Now;
                Add(_accounts, accountKey, AccountLimit, now);
                Add(_addresses, addressKey, AddressLimit, now);
            }
        }

        /// <summary>A successful sign-in clears the name's counter (not the address's).</summary>
        public void Reset(string accountKey)
        {
            lock (_gate)
                _accounts.Remove(accountKey ?? "");
        }

        /// <summary>Failures counted for the name inside the current window.</summary>
        public int Failures(string accountKey)
        {
            lock (_gate)
            {
                if (!_accounts.TryGetValue(accountKey ?? "", out var counter))
                    return 0;
                counter.Trim(_clock.Now);
                return counter.Failures.Count;
            }
        }

        private static bool Locked(Dictionary<string, Counter> map, string key, DateTimeOffset now)
            => map.TryGetValue(key ?? "", out var counter) && counter.LockedUntil > now;

        private static void Add(Dictionary<string, Counter> map, string key, int limit, DateTimeOffset now)
        {
            key ??= "";
            if (!map.TryGetValue(key, out var counter))
                map[key] = counter = new Counter();

            counter.Trim(now);
            counter.Failures.Enqueue(now);
            if (counter.Failures.Count >= limit)
                counter.LockedUntil = now + Window;
        }

        private sealed class Counter
        {
            public readonly Queue<DateTimeOffset> Failures = new Queue<DateTimeOffset>();
            public DateTimeOffset LockedUntil;

            public void Trim(DateTimeOffset now)
            {
                while (this.Failures.Count > 0 && now - this.Failures.Peek() >= Window)
                    this.Failures.Dequeue();
            }
        }
    }
}
