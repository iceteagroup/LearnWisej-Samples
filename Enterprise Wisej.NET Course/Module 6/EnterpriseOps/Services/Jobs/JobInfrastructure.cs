using System;
using EnterpriseOps.Data;

namespace EnterpriseOps.Services.Jobs
{
    /// <summary>
    /// The process-wide job infrastructure: the queue, the status store, the notification store and the
    /// work-order repository the imports write to. These are the ONLY statics in the sample, and they are
    /// static on purpose: they stand in for the durable queue / job table / notification table that outlive
    /// every session. They hold no user or tenant state of their own — every read is tenant-scoped through
    /// the CommandContext (Module 3's static-state audit still passes).
    /// </summary>
    public static class JobInfrastructure
    {
        private static readonly Lazy<Bundle> _bundle = new Lazy<Bundle>(Build, isThreadSafe: true);

        public static IJobStatusStore Store => _bundle.Value.Store;
        public static IJobQueue Queue => _bundle.Value.Queue;
        public static INotificationService Notifications => _bundle.Value.Notifications;
        public static FakeWorkOrderRepository WorkOrders => _bundle.Value.WorkOrders;
        public static IImportFileSource Files => _bundle.Value.Files;
        public static RetryPolicy Retry => _bundle.Value.Retry;

        private sealed class Bundle
        {
            public IJobStatusStore Store;
            public IJobQueue Queue;
            public INotificationService Notifications;
            public FakeWorkOrderRepository WorkOrders;
            public IImportFileSource Files;
            public RetryPolicy Retry;
        }

        private static Bundle Build()
        {
            var store = new InMemoryJobStatusStore();
            var notifications = new InMemoryNotificationService();
            var bundle = new Bundle
            {
                Store = store,
                Notifications = notifications,
                Queue = new InMemoryJobQueue(store, notifications),
                WorkOrders = new FakeWorkOrderRepository(),
                Files = new FakeImportFileSource(),
                Retry = new RetryPolicy(),
            };
            SeedHistory(store);
            return bundle;
        }

        /// <summary>Yesterday's jobs, so the grid and the tenant filter have something to show on the first open.</summary>
        private static void SeedHistory(IJobStatusStore store)
        {
            var t0 = DateTime.UtcNow.AddMinutes(-42);

            store.Create(new JobRecord
            {
                JobId = Guid.NewGuid(), Type = "ImportAssets", Description = "Asset import — sensors.csv", Input = "sensors.csv",
                TenantId = "contoso", StartedBy = "ben.tech", CorrelationId = "seed0001",
                Status = JobStatus.Completed, Percent = 100, Message = "Import completed: 240 rows imported.",
                CreatedUtc = t0, StartedUtc = t0.AddSeconds(2), FinishedUtc = t0.AddSeconds(31),
                Result = new JobResultSummary { TotalRows = 240, BatchesCompleted = 3, Imported = 240, Created = 240 }
            });

            store.Create(new JobRecord
            {
                JobId = Guid.NewGuid(), Type = "GenerateReport", Description = "Report generation — SLA Q2", Input = "sla-q2",
                TenantId = "contoso", StartedBy = "cara.admin", CorrelationId = "seed0002",
                Status = JobStatus.Completed, Percent = 100, Message = "Report generated (14 pages).",
                CreatedUtc = t0.AddMinutes(12), StartedUtc = t0.AddMinutes(12), FinishedUtc = t0.AddMinutes(13)
            });

            // Another tenant's job: it exists in the store and must never reach a contoso screen.
            store.Create(new JobRecord
            {
                JobId = Guid.NewGuid(), Type = "ImportWorkOrders", Description = "Work order import — fabrikam_q1.csv", Input = "fabrikam_q1.csv",
                TenantId = "fabrikam", StartedBy = "m.weber", CorrelationId = "seed0003",
                Status = JobStatus.CompletedWithErrors, Percent = 100, Message = "Completed with errors: 4,980 imported · 18 retried ✓ · 2 terminal.",
                CreatedUtc = t0.AddMinutes(20), StartedUtc = t0.AddMinutes(20), FinishedUtc = t0.AddMinutes(41)
            });
        }
    }
}
