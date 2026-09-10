using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EnterpriseOps.Security
{
    public enum StaticStateVerdict
    {
        /// <summary>Documented with <see cref="SharedStateAttribute"/> and application/tenant scoped.</summary>
        Documented,
        /// <summary>static readonly and the value type is immutable (string, primitive, enum …).</summary>
        ImmutableReference,
        /// <summary>Per-user data in a static, an undocumented mutable static, or a static readonly to a mutable object.</summary>
        Finding,
    }

    public sealed class StaticStateFinding
    {
        public string Type { get; init; }
        public string Member { get; init; }
        public string FieldType { get; init; }
        public bool IsReadOnly { get; init; }
        public StaticStateVerdict Verdict { get; init; }
        public string Reason { get; init; }
        public string CurrentValue { get; init; }

        public override string ToString()
            => $"{(Verdict == StaticStateVerdict.Finding ? "FINDING" : Verdict == StaticStateVerdict.Documented ? "ok·doc " : "ok·imm ")} {Type}.{Member} : {FieldType} — {Reason}";
    }

    /// <summary>
    /// The static-state audit from the lesson, run live: reflects over every static field in the assembly and
    /// answers one question for each — what does it hold, and could that ever be per-user or per-tenant?
    /// A production team runs the same rule in code review; here it runs on the "Run static-state audit" button.
    /// </summary>
    public sealed class StaticStateAudit
    {
        // The audit audits itself: this static is reference data, so it says so in the same way it expects
        // every other static in the solution to.
        [SharedState(StateScope.Application,
            holds: "the value types the audit treats as immutable — a lookup table, no user or tenant data",
            synchronization: "never mutated after initialization; read-only use")]
        private static readonly Type[] ImmutableTypes =
        {
            typeof(string), typeof(bool), typeof(int), typeof(long), typeof(short), typeof(byte), typeof(double),
            typeof(float), typeof(decimal), typeof(char), typeof(Guid), typeof(DateTime), typeof(DateTimeOffset),
            typeof(TimeSpan), typeof(Type),
        };

        public IReadOnlyList<StaticStateFinding> Run(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            var findings = new List<StaticStateFinding>();

            foreach (Type type in assembly.GetTypes().OrderBy(t => t.FullName, StringComparer.Ordinal))
            {
                // Compiler-generated closures / lambda caches (<>c, <>c__DisplayClass) hold delegates, not state.
                if (type.Name.StartsWith("<", StringComparison.Ordinal) || type.IsEnum
                    || type.GetCustomAttribute<CompilerGeneratedAttribute>() != null)
                    continue;

                var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                                 .Where(f => !f.IsLiteral);       // const is not state

                foreach (FieldInfo field in fields)
                    findings.Add(Classify(type, field));
            }

            return findings;
        }

        private static StaticStateFinding Classify(Type type, FieldInfo field)
        {
            // A static auto-property shows up as its backing field: report it under the property's name.
            string member = field.Name.StartsWith("<", StringComparison.Ordinal)
                ? field.Name.Substring(1, field.Name.IndexOf('>') - 1) + " (property)"
                : field.Name;

            var documented = field.GetCustomAttribute<SharedStateAttribute>() ?? type.GetCustomAttribute<SharedStateAttribute>();
            bool immutable = ImmutableTypes.Contains(field.FieldType) || field.FieldType.IsEnum;

            StaticStateVerdict verdict;
            string reason;

            if (documented != null)
            {
                bool perUserScope = documented.Scope is StateScope.User or StateScope.Session or StateScope.Tab or StateScope.Request or StateScope.Workflow;
                verdict = perUserScope ? StaticStateVerdict.Finding : StaticStateVerdict.Documented;
                reason = perUserScope
                    ? $"documented as {documented.Scope}-scoped — a static can never own {documented.Scope} state"
                    : $"{documented.Scope} scope · holds {documented.Holds} · sync: {documented.Synchronization}";
            }
            else if (field.IsInitOnly && immutable)
            {
                verdict = StaticStateVerdict.ImmutableReference;
                reason = "static readonly, immutable value — configuration/reference data";
            }
            else if (field.IsInitOnly)
            {
                verdict = StaticStateVerdict.Finding;
                reason = "static readonly, but the object is mutable and undocumented — add [SharedState] with scope + synchronization, or remove";
            }
            else
            {
                verdict = StaticStateVerdict.Finding;
                reason = LooksPerUser(member)
                    ? "MUTABLE static that names a user/tenant/session — the last session to write wins; the next request runs as that person"
                    : "mutable static, undocumented — overwritten by any session at any time; no synchronization";
            }

            return new StaticStateFinding
            {
                Type = type.Name,
                Member = member,
                FieldType = FriendlyName(field.FieldType),
                IsReadOnly = field.IsInitOnly,
                Verdict = verdict,
                Reason = reason,
                CurrentValue = SafeValue(field),
            };
        }

        private static bool LooksPerUser(string member)
        {
            string m = member.ToLowerInvariant();
            return m.Contains("user") || m.Contains("tenant") || m.Contains("session") || m.Contains("current");
        }

        private static string SafeValue(FieldInfo field)
        {
            try
            {
                object value = field.GetValue(null);
                if (value == null) return "null";
                if (value is string s) return $"\"{s}\"";
                if (ImmutableTypes.Contains(value.GetType()) || value.GetType().IsEnum) return value.ToString();
                return "<object>";
            }
            catch
            {
                return "?";
            }
        }

        private static string FriendlyName(Type t)
        {
            if (!t.IsGenericType) return t.Name;
            string name = t.Name.Substring(0, t.Name.IndexOf('`'));
            return name + "<" + string.Join(",", t.GetGenericArguments().Select(FriendlyName)) + ">";
        }
    }
}
