using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Services
{
    /// <summary>One line of docs/EnvironmentConfiguration.md — a setting, its value per environment, and its owner.</summary>
    public class ConfigurationRow
    {
        public string Setting { get; set; }
        public string Key { get; set; }
        public string Development { get; set; }
        public string Staging { get; set; }
        public string Production { get; set; }
        public string Secret { get; set; }          // "no" · "YES" · "PARTLY"
        public string Owner { get; set; }           // platform · ops · product · security · release
    }

    /// <summary>What a "boot this environment" preview found: the values, the contract errors, the fix.</summary>
    public class EnvironmentPreview
    {
        public string Environment;
        public bool SecretsInjected;                // false = only the committed files, as a clean checkout would boot
        public ValidationResult Validation;
        public bool WouldStart => Validation != null && Validation.Succeeded;
    }

    /// <summary>
    /// Configuration as a contract. Reads the SAME appsettings files the host reads, one environment at a
    /// time, so the dashboard can show the environment configuration table and — the failure path — what
    /// happens when a required setting is missing: StartupValidation refuses the boot and the node never
    /// joins the load balancer.
    ///
    /// Secrets are never in the files. "Injecting" them here is the in-process stand-in for the platform
    /// putting EnterpriseOps__ConnectionString / EnterpriseOps__IdentityProvider__ClientSecret in the
    /// environment at deployment time (vault, Key Vault, Secrets Manager, docker secret…).
    /// </summary>
    public class ConfigurationService
    {
        public const string Development = "Development";
        public const string Staging = "Staging";
        public const string Production = "Production";

        public static readonly string[] Environments = { Development, Staging, Production };

        private const string Prefix = "EnterpriseOps:";
        private const string Masked = "●●●●●●●●";

        private readonly ActivityTrace _trace;

        public ConfigurationService(ActivityTrace trace)
        {
            _trace = trace;
        }

        /// <summary>Deliverable "Environment configuration table" — built from the real files, not retyped.</summary>
        public List<ConfigurationRow> BuildTable()
        {
            var dev = BuildForEnvironment(Development, injectSecrets: false);
            var stg = BuildForEnvironment(Staging, injectSecrets: false);
            var prd = BuildForEnvironment(Production, injectSecrets: false);

            var rows = new List<ConfigurationRow>
            {
                Row("Connection string",  "ConnectionString",                dev, stg, prd, secret: "YES",    owner: "platform"),
                Row("Logging level",      "Logging:Level",                   dev, stg, prd, secret: "no",     owner: "ops"),
                Row("Upload limit (MB)",  "UploadLimitMb",                   dev, stg, prd, secret: "no",     owner: "product"),
                Row("Feature flags",      "FeatureFlags",                    dev, stg, prd, secret: "no",     owner: "product"),
                Row("Identity provider",  "IdentityProvider:Authority",      dev, stg, prd, secret: "PARTLY", owner: "security"),
                Row("IdP client secret",  "IdentityProvider:ClientSecret",   dev, stg, prd, secret: "YES",    owner: "security"),
                Row("Allowed hosts",      "AllowedHosts",                    dev, stg, prd, secret: "no",     owner: "security"),
                Row("Release version",    "Release:Version",                 dev, stg, prd, secret: "no",     owner: "release"),
                Row("Storage root",       "Storage:Root",                    dev, stg, prd, secret: "no",     owner: "platform"),
            };

            _trace.Service($"ConfigurationService.BuildTable → {rows.Count} settings × 3 environments, read from appsettings*.json");
            int secrets = rows.Count(r => r.Secret != "no");
            _trace.Service($"{secrets} settings are secret or partly secret — none of them has a value committed for Production");
            return rows;
        }

        /// <summary>
        /// "Boot" an environment with only what a clean checkout contains (or with the platform's secrets
        /// injected) and run the same StartupValidation rules Startup.cs runs before the host is built.
        /// </summary>
        public EnvironmentPreview Preview(string environmentName, bool injectSecrets)
        {
            _trace.Ui($"ConfigurationService.Preview(\"{environmentName}\", secretsInjected: {injectSecrets.ToString().ToLowerInvariant()})");
            var configuration = BuildForEnvironment(environmentName, injectSecrets);
            var validation = StartupValidation.Validate(configuration, environmentName);

            _trace.Host($"appsettings.json + appsettings.{environmentName}.json layered" +
                        (injectSecrets ? " + environment variables (platform secret store)" : " (no environment variables — a clean checkout)"));

            if (validation.Succeeded)
                _trace.Service($"StartupValidation: {validation.Passed.Count} rules passed — the node would start and join the balancer");
            else
                foreach (var error in validation.Errors)
                    _trace.Service("StartupValidation REJECTED: " + error);

            return new EnvironmentPreview
            {
                Environment = environmentName,
                SecretsInjected = injectSecrets,
                Validation = validation,
            };
        }

        /// <summary>The environment variables the platform injects at deployment time — never a committed file.</summary>
        public static Dictionary<string, string> PlatformSecrets(string environmentName) => new Dictionary<string, string>
        {
            [Prefix + "ConnectionString"] = $"Server=sql-{environmentName.ToLowerInvariant()};Database=EnterpriseOps;User Id=eops;Password=(from vault)",
            [Prefix + "IdentityProvider:ClientSecret"] = "(from vault)",
        };

        private static IConfiguration BuildForEnvironment(string environmentName, bool injectSecrets)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(HostConfiguration.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true);

            if (injectSecrets)
                builder.AddInMemoryCollection(PlatformSecrets(environmentName));

            return builder.Build();
        }

        private static ConfigurationRow Row(string caption, string key, IConfiguration dev, IConfiguration stg, IConfiguration prd, string secret, string owner)
            => new ConfigurationRow
            {
                Setting = caption,
                Key = Prefix + key,
                Development = Display(dev[Prefix + key], secret, Development),
                Staging = Display(stg[Prefix + key], secret, Staging),
                Production = Display(prd[Prefix + key], secret, Production),
                Secret = secret,
                Owner = owner,
            };

        /// <summary>Values are shown, secrets are masked, and a deliberately absent secret says where it comes from.</summary>
        private static string Display(string value, string secret, string environmentName)
        {
            bool isDev = string.Equals(environmentName, Development, StringComparison.Ordinal);
            if (string.IsNullOrWhiteSpace(value))
                return secret == "no" ? "(inherits base)" : "→ environment variable";
            if (secret == "YES" && !isDev)
                return Masked;
            return value.Length <= 46 ? value : value.Substring(0, 43) + "…";
        }
    }
}
