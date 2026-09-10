using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OrderDesk.Diagnostics
{
    /// <summary>
    /// Readiness item 2, checked by reflection instead of by promise: every static field in the OrderDesk
    /// assembly is listed, and any that is writable (not readonly, not const) is an "open item" — a slot one
    /// session can overwrite for every other session, exactly what LegacyOrderDesk's AppState.CurrentUser was.
    /// Readonly statics that reference collections (AuditLog, the shared repository) are allowed because they
    /// are shared by design and guarded by a lock; the audit names them so the reviewer can check.
    /// </summary>
    public static class StaticStateAudit
    {
        public sealed class Finding
        {
            public string Type { get; set; }
            public string Field { get; set; }
            public bool Writable { get; set; }
            public override string ToString() => $"{(Writable ? "✕ writable" : "✓ readonly")} {Type}.{Field}";
        }

        public static IList<Finding> Run()
        {
            var findings = new List<Finding>();
            foreach (var type in typeof(StaticStateAudit).Assembly.GetTypes())
            {
                if (type.IsEnum || type.GetCustomAttribute<CompilerGeneratedAttribute>() != null) continue;
                if (!(type.Namespace ?? "").StartsWith("OrderDesk", StringComparison.Ordinal)) continue;

                foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (field.IsLiteral) continue;                                   // const
                    if (field.Name.Contains("<") || field.Name.Contains("__")) continue;  // compiler-generated (lambdas, switch tables)
                    findings.Add(new Finding
                    {
                        Type = type.Name,
                        Field = field.Name,
                        Writable = !field.IsInitOnly
                    });
                }
            }
            return findings.OrderByDescending(f => f.Writable).ThenBy(f => f.Type).ThenBy(f => f.Field).ToList();
        }

        public static int OpenItems(IList<Finding> findings) => findings.Count(f => f.Writable && !IsAllowedCounter(f));

        /// <summary>
        /// The one writable static the design accepts: AuditLog._version, a counter mutated only inside
        /// AuditLog's lock. Everything else writable is an open item.
        /// </summary>
        private static bool IsAllowedCounter(Finding f) => f.Type == "AuditLog" && f.Field == "_version";

        public static string Summary()
        {
            var findings = Run();
            int open = OpenItems(findings);
            var shared = findings.Where(f => !f.Writable).Select(f => f.Type).Distinct().OrderBy(t => t).ToList();
            return $"{open} open item(s) · {findings.Count} static field(s) scanned · readonly shared state in: {string.Join(", ", shared)}";
        }
    }
}
