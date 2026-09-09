using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Diagnostics
{
    public enum BudgetStatus { NotMeasured, Ok, Over }

    /// <summary>One line of the performance budget table: the agreed limit and the last measurement.</summary>
    public sealed class BudgetRow
    {
        public string Operation { get; init; }
        public int BudgetMs { get; init; }
        public long? MeasuredMs { get; private set; }
        public BudgetStatus Status { get; private set; } = BudgetStatus.NotMeasured;
        public string CorrelationId { get; private set; }
        public DateTime? MeasuredUtc { get; private set; }

        public bool IsOver => Status == BudgetStatus.Over;

        public string BudgetText => $"≤ {BudgetMs:N0} ms";
        public string MeasuredText => MeasuredMs.HasValue ? $"{MeasuredMs.Value:N0} ms" : "—";
        public string StatusText => Status switch
        {
            BudgetStatus.Ok => "OK ✓",
            BudgetStatus.Over => "OVER ✕",
            _ => "not measured",
        };

        internal void Record(long measuredMs, string correlationId)
        {
            MeasuredMs = measuredMs;
            Status = measuredMs <= BudgetMs ? BudgetStatus.Ok : BudgetStatus.Over;
            CorrelationId = correlationId;
            MeasuredUtc = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// The performance budget table: numbers the team agreed to keep under, one per point where a user waits.
    /// Record() is the only way a measurement gets in, and it is where OK / OVER is decided — not in the UI.
    /// </summary>
    public sealed class PerformanceBudget
    {
        public const string Startup = "Startup";
        public const string ScreenLoad = "Screen load — Diagnostics";
        public const string QuerySearchWorkOrders = "Query — SearchWorkOrders";
        public const string BindingRefresh = "Binding refresh";
        public const string BackgroundJobTick = "Background job tick";

        private readonly List<BudgetRow> _rows = new List<BudgetRow>
        {
            new BudgetRow { Operation = Startup,               BudgetMs = 800 },
            new BudgetRow { Operation = ScreenLoad,            BudgetMs = 400 },
            new BudgetRow { Operation = QuerySearchWorkOrders, BudgetMs = 400 },
            new BudgetRow { Operation = BindingRefresh,        BudgetMs = 150 },
            new BudgetRow { Operation = BackgroundJobTick,     BudgetMs = 250 },
        };

        public IReadOnlyList<BudgetRow> Rows => _rows;

        public bool AnyOver => _rows.Any(r => r.IsOver);

        public IEnumerable<BudgetRow> OverBudget => _rows.Where(r => r.IsOver);

        public BudgetRow Find(string operation)
        {
            BudgetRow row = _rows.FirstOrDefault(r => r.Operation == operation);
            if (row == null) throw new ArgumentException($"No budget is defined for '{operation}'.", nameof(operation));
            return row;
        }

        public BudgetRow Record(string operation, long measuredMs, string correlationId)
        {
            BudgetRow row = Find(operation);
            row.Record(measuredMs, correlationId);
            return row;
        }
    }
}
