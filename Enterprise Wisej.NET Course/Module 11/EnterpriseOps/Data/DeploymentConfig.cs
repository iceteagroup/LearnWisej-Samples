using System;
using System.Collections.Generic;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// What the deployment hands the process: the environment, the logical node label, the feature flags —
    /// and the secrets that must never reach a screen. The values are fakes; what matters is the *types* of
    /// things a configuration carries, because DiagnosticsService decides by type what the snapshot may show.
    /// </summary>
    public sealed class DeploymentConfig
    {
        private DeploymentConfig() { }

        /// <summary>Safe to show. Read from ASPNETCORE_ENVIRONMENT; "Development" when it is not set.</summary>
        public string Environment { get; private set; }

        /// <summary>
        /// Safe to show: the logical node label the deployment assigns (the lesson's "app-node-B"). The raw
        /// machine name is deliberately not used — it is an infrastructure detail, not a support answer.
        /// </summary>
        public string NodeName { get; private set; }

        /// <summary>Secret. Excluded from the snapshot by type: no snapshot field can hold it.</summary>
        public string ConnectionString { get; private set; }

        /// <summary>Sensitive. Masked before anything derived from it is displayed.</summary>
        public string StorageAccountName { get; private set; }

        /// <summary>Secret. Excluded from the snapshot.</summary>
        public string ApiKey { get; private set; }

        /// <summary>Safe to show: which features are on for this deployment.</summary>
        public IReadOnlyDictionary<string, string> FeatureFlags { get; private set; }

        public static DeploymentConfig Load()
        {
            return new DeploymentConfig
            {
                Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                NodeName = System.Environment.GetEnvironmentVariable("ENTERPRISEOPS_NODE") ?? "app-node-B",
                ConnectionString = "Server=sql-prod-02;Database=WorkOrders;User Id=ops;Password=not-a-real-secret",
                StorageAccountName = "stenterpriseopsprod",
                ApiKey = "sk-live-not-a-real-key-8f31c2",
                FeatureFlags = new Dictionary<string, string>
                {
                    ["workqueue.serverPaging"] = "on",
                    ["escalation.wizard"] = "on",
                    ["diagnostics.liveRefresh"] = "on",
                    ["reports.cache"] = "off",
                },
            };
        }
    }
}
