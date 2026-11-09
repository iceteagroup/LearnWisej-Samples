using System;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// The correlation ID of one click or one review case. Over a Wisej.NET WebSocket the ASP.NET
    /// Core logging scope belongs to the connection (<c>/ws.wx</c>), not to the click, so AccessOps
    /// opens its own scope per click with <see cref="Begin"/>: every log line written inside carries
    /// <c>Correlation=…</c> and every audit event takes the same ID (<see cref="Current"/>). The
    /// value is random and opaque; it never contains a session ID, a name or a token.
    /// </summary>
    public static class Correlation
    {
        private static readonly AsyncLocal<string> _current = new AsyncLocal<string>();

        /// <summary>The correlation ID of the scope this code runs in, or null.</summary>
        public static string Current => _current.Value;

        /// <summary>A new random label: <paramref name="prefix"/> + "-" + 6 hex characters.</summary>
        public static string NewLabel(string prefix)
            => prefix + "-" + Convert.ToHexString(RandomNumberGenerator.GetBytes(3)).ToLowerInvariant();

        /// <summary>
        /// Opens <c>ILogger.BeginScope</c> with <c>Correlation = label</c> and makes
        /// <paramref name="label"/> the <see cref="Current"/> ID until the returned scope is disposed.
        /// </summary>
        public static IDisposable Begin(ILogger logger, string label)
        {
            var previous = _current.Value;
            _current.Value = label;
            // a message-template scope: the console formatter prints "Correlation=rr-07-3f9a1c" (a dictionary
            // state prints as its type name), structured sinks still get the Correlation property
            var scope = logger?.BeginScope("Correlation={Correlation}", label);
            return new Scope(scope, previous);
        }

        private sealed class Scope : IDisposable
        {
            private readonly IDisposable _inner;
            private readonly string _previous;
            private bool _disposed;

            public Scope(IDisposable inner, string previous)
            {
                _inner = inner;
                _previous = previous;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;
                _disposed = true;
                _current.Value = _previous;
                _inner?.Dispose();
            }
        }
    }
}
