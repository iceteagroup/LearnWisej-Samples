using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace EnterpriseOps.Services
{
    public class ValidationResult
    {
        public bool Succeeded => Errors.Count == 0;
        public List<string> Errors = new List<string>();
        public List<string> Passed = new List<string>();
        public string Environment;
    }

    /// <summary>
    /// The configuration contract, enforced. Startup.cs runs it before the host is built (fail fast:
    /// a misconfigured node must never join the balancer); the Release dashboard runs the same rules
    /// against any environment preview so the learner can see the failure without restarting.
    ///
    /// Rules are deliberately readable: one line per requirement, with WHERE the value should come from.
    /// </summary>
    public static class StartupValidation
    {
        private const string Prefix = "EnterpriseOps:";

        public static ValidationResult Validate(IConfiguration configuration, string environmentName)
        {
            var result = new ValidationResult { Environment = environmentName };
            bool isProduction = string.Equals(environmentName, "Production", StringComparison.OrdinalIgnoreCase);
            bool isStaging = string.Equals(environmentName, "Staging", StringComparison.OrdinalIgnoreCase);

            Require(configuration, result, "Release:Version", "appsettings.json (set by the pipeline)");
            Require(configuration, result, "Release:PreviousVersion", "appsettings.json — rollback needs a named previous artifact");
            Require(configuration, result, "ConnectionString",
                isProduction ? "environment variable EnterpriseOps__ConnectionString (vault / platform secret store)"
                             : "appsettings.{Environment}.json");
            Require(configuration, result, "IdentityProvider:Authority", "appsettings.{Environment}.json");

            if (isProduction || isStaging)
                Require(configuration, result, "IdentityProvider:ClientSecret", "environment variable EnterpriseOps__IdentityProvider__ClientSecret (vault)");

            string hosts = configuration[Prefix + "AllowedHosts"];
            if (isProduction && (string.IsNullOrWhiteSpace(hosts) || hosts.Trim() == "*"))
                result.Errors.Add("EnterpriseOps:AllowedHosts must name the public host in Production (\"*\" accepts host-header spoofing)");
            else
                result.Passed.Add("EnterpriseOps:AllowedHosts = " + hosts);

            string level = configuration[Prefix + "Logging:Level"];
            if (isProduction && string.Equals(level, "Debug", StringComparison.OrdinalIgnoreCase))
                result.Errors.Add("EnterpriseOps:Logging:Level must not be Debug in Production (log volume + data exposure)");
            else
                result.Passed.Add("EnterpriseOps:Logging:Level = " + level);

            return result;
        }

        private static void Require(IConfiguration configuration, ValidationResult result, string key, string expectedSource)
        {
            string value = configuration[Prefix + key];
            if (string.IsNullOrWhiteSpace(value))
                result.Errors.Add($"missing {Prefix}{key} — expected from {expectedSource}");
            else
                result.Passed.Add($"{Prefix}{key} present");
        }
    }
}
