// DiagnosticsPerformancePatterns.cs
//
// The two types the lesson resource shows, verbatim. OperationTimer is the "timing as a using block"
// pattern: wrap any operation in `using` and the elapsed time logs itself, with the operation name and the
// correlation id. DiagnosticSnapshot is "safe by type": it has no field that could carry a connection
// string, a key or an exception message, so the page that binds to it cannot leak them.
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EnterpriseOps.Diagnostics;

public sealed class OperationTimer : IDisposable
{
    private readonly Stopwatch _watch = Stopwatch.StartNew();
    private readonly string _operation;
    private readonly string _correlationId;
    private readonly Action<string, TimeSpan, string> _log;

    public OperationTimer(string operation, string correlationId,
        Action<string, TimeSpan, string> log)
    {
        _operation = operation;
        _correlationId = correlationId;
        _log = log;
    }

    public void Dispose()
    {
        _watch.Stop();
        _log(_operation, _watch.Elapsed, _correlationId);
    }
}

public sealed record DiagnosticSnapshot(
    string Version,
    string Environment,
    string NodeName,
    string ActiveTheme,
    IReadOnlyDictionary<string, string> FeatureFlags,
    IReadOnlyList<string> SafeRecentEvents);
