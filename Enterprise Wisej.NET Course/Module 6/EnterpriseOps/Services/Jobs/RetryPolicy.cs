using System;

namespace EnterpriseOps.Services.Jobs
{
    /// <summary>A timeout writing one row: retry it, a bounded number of times, with a delay.</summary>
    public sealed class TransientRowException : Exception
    {
        public TransientRowException(string message) : base(message) { }
    }

    /// <summary>A row that can never succeed (bad asset code, unknown site): record it, move on, never retry.</summary>
    public sealed class TerminalRowException : Exception
    {
        public TerminalRowException(string message) : base(message) { }
    }

    /// <summary>A file that cannot be read at all: the whole job fails immediately.</summary>
    public sealed class MalformedFileException : Exception
    {
        public MalformedFileException(string message) : base(message) { }
    }

    /// <summary>
    /// The retry policy: which errors are retryable, how many times, how quickly. Transient errors get
    /// <see cref="MaxAttempts"/> attempts with exponential backoff; everything else is terminal.
    /// Retries are safe only because row writes are idempotent (upsert by ExternalRef).
    /// </summary>
    public sealed class RetryPolicy
    {
        public int MaxAttempts { get; init; } = 3;
        public int BaseDelayMs { get; init; } = 100;

        /// <summary>Transient: a timeout writing one row. Terminal: a malformed row or file, or anything unknown.</summary>
        public bool IsTransient(Exception ex) => ex is TransientRowException;

        /// <summary>100 ms, 200 ms, 400 ms … so a flaky downstream system is not hammered.</summary>
        public int DelayFor(int attempt) => BaseDelayMs * (1 << (attempt - 1));

        public override string ToString() => $"max {MaxAttempts} attempts, backoff {BaseDelayMs} ms × 2^n; transient = TransientRowException, terminal = everything else";
    }

    /// <summary>
    /// The cancellation policy, written down so the job and the reviewer agree on it:
    /// a cancel request sets a flag; the job checks it between batches, finishes the current batch cleanly,
    /// records what was done and moves to Canceled. It never stops mid-row.
    /// </summary>
    public static class CancellationPolicy
    {
        public const bool CheckBetweenBatches = true;
        public const bool FinishCurrentBatch = true;
        public const string Description = "checked between batches · current batch finishes · result records rows imported · status Canceled";
    }
}
