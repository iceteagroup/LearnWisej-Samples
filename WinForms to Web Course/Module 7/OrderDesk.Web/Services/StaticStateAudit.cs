using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OrderDesk.Services
{
    /// <summary>One mutable static field found in the OrderDesk assembly and how the review classifies it.</summary>
    public sealed class StaticFieldFinding
    {
        public string Type { get; set; }
        public string Field { get; set; }
        public string FieldType { get; set; }
        public bool Allowed { get; set; }
        public string Why { get; set; }
        public override string ToString() => Type + "." + Field + " : " + FieldType + " → " + (Allowed ? "allowed — " : "✖ per-user? — ") + Why;
    }

    /// <summary>
    /// Readiness check #2 ("no per-user state remains in unsafe static fields"), automated: reflect over
    /// every type of the OrderDesk assembly and list the static fields that are neither readonly nor const.
    /// Compiler-generated fields (lambda caches, anonymous types) are skipped. Whatever is left must be on
    /// the allow-list with a reason (shared reference data behind a lock), or the check fails.
    /// </summary>
    public static class StaticStateAudit
    {
        private static readonly Dictionary<string, string> AllowList = new Dictionary<string, string>
        {
            { "OrderDesk.Domain.OrderStore._shared", "process-wide reference data, created once under a lock (OrderStore.Shared)" },
            { "OrderDesk.Services.AppConfig._settings", "Web.config cache, loaded once under a lock, same for every user" },
            { "OrderDesk.Services.AppConfig._connectionStrings", "Web.config cache, loaded once under a lock, same for every user" },
            { "OrderDesk.Services.AppConfig._loadedFrom", "Web.config cache, loaded once under a lock, same for every user" },
        };

        public static List<StaticFieldFinding> Scan()
        {
            var findings = new List<StaticFieldFinding>();
            var asm = typeof(StaticStateAudit).Assembly;
            foreach (var type in asm.GetTypes().OrderBy(t => t.FullName))
            {
                if (type.Name.Contains("<")) continue;                       // compiler-generated (lambdas, anonymous types)
                foreach (var f in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (f.IsInitOnly || f.IsLiteral) continue;               // readonly / const: cannot be re-assigned per user
                    if (f.Name.Contains("<") || f.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false)) continue;
                    if (typeof(Delegate).IsAssignableFrom(f.FieldType)) continue; // static event backing fields
                    var key = type.FullName + "." + f.Name;
                    findings.Add(new StaticFieldFinding
                    {
                        Type = type.FullName,
                        Field = f.Name,
                        FieldType = f.FieldType.Name,
                        Allowed = AllowList.TryGetValue(key, out var why),
                        Why = why ?? "mutable static not on the allow-list — move it to Application.Session (UserContext) or justify it",
                    });
                }
            }
            return findings;
        }

        /// <summary>Readiness check #3: the desktop-only dependencies must not be referenced any more.</summary>
        public static List<string> DesktopReferences()
        {
            var suspects = new[] { "System.Windows.Forms", "System.Drawing.Printing", "Microsoft.Win32.Registry", "Microsoft.Office.Interop.Excel", "System.Drawing.Common" };
            var refs = typeof(StaticStateAudit).Assembly.GetReferencedAssemblies().Select(a => a.Name).ToList();
            return suspects.Where(s => refs.Any(r => string.Equals(r, s, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        /// <summary>True when no type of the migrated app still lives in the LegacyOrderDesk plumbing namespace.</summary>
        public static bool LegacyNamespaceCompiled()
            => typeof(StaticStateAudit).Assembly.GetTypes().Any(t => t.Namespace != null && t.Namespace.StartsWith("OrderDesk.Legacy", StringComparison.Ordinal));
    }
}
