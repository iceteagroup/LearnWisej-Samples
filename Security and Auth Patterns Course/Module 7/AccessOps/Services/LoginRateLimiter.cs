using System;
using System.Collections.Concurrent;
using System.Linq;

namespace AccessOps.Services
{
    /// <summary>
    /// A token bucket per (user name, client address) for the password sign-in. Middleware cannot do
    /// this: every Wisej.NET event - the Sign in click included - reaches the same application
    /// endpoint inside an existing session, so there is no "login URL" to limit. The bucket is taken
    /// in <see cref="LoginService"/>, before the password hash is computed.
    /// <para>
    /// The buckets live in this process. With more than one server they belong in a store every
    /// server shares (a distributed cache), or an attacker spreads the attempts across the farm.
    /// </para>
    /// </summary>
    public sealed class LoginRateLimiter
    {
        private const int MaxTrackedKeys = 50_000;

        private readonly int _capacity;
        private readonly TimeSpan _refillEvery;
        private readonly Func<DateTime> _clock;
        private readonly ConcurrentDictionary<string, Bucket> _buckets = new ConcurrentDictionary<string, Bucket>(StringComparer.Ordinal);

        public LoginRateLimiter(int capacity, TimeSpan refillEvery, Func<DateTime> clock)
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            _capacity = capacity;
            _refillEvery = refillEvery;
            _clock = clock;
        }

        /// <summary>Takes one token; false when the bucket for this name and address is empty.</summary>
        public bool TryTake(string userName, string clientAddress)
        {
            var now = _clock();
            if (_buckets.Count > MaxTrackedKeys)
                Prune(now);

            var bucket = _buckets.GetOrAdd((userName ?? "") + "|" + (clientAddress ?? ""), _ => new Bucket(_capacity, now));
            lock (bucket)
            {
                bucket.Refill(now, _capacity, _refillEvery);
                if (bucket.Tokens < 1)
                    return false;
                bucket.Tokens -= 1;
                return true;
            }
        }

        // a flood of invented user names must not grow the table without bound: drop the full buckets
        private void Prune(DateTime now)
        {
            foreach (var key in _buckets.Keys.ToArray())
            {
                if (_buckets.TryGetValue(key, out var b))
                {
                    lock (b)
                    {
                        b.Refill(now, _capacity, _refillEvery);
                        if (b.Tokens >= _capacity)
                            _buckets.TryRemove(key, out _);
                    }
                }
            }
        }

        private sealed class Bucket
        {
            public Bucket(int tokens, DateTime now)
            {
                this.Tokens = tokens;
                this.Updated = now;
            }

            public double Tokens;
            public DateTime Updated;

            public void Refill(DateTime now, int capacity, TimeSpan every)
            {
                var earned = (now - this.Updated).TotalSeconds / every.TotalSeconds;
                if (earned > 0)
                {
                    this.Tokens = Math.Min(capacity, this.Tokens + earned);
                    this.Updated = now;
                }
            }
        }
    }
}
