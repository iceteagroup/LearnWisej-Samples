using EnterpriseOps.Diagnostics;
using EnterpriseOps.Services;

namespace EnterpriseOps.Security
{
    /// <summary>The outcome of a policy check: allowed or not, plus a reason that is safe to show to the user.</summary>
    public sealed class PolicyDecision
    {
        private PolicyDecision(bool allowed, string reason)
        {
            Allowed = allowed;
            Reason = reason;
        }

        public bool Allowed { get; }
        public string Reason { get; }

        public static PolicyDecision Allow(string reason) => new PolicyDecision(true, reason);
        public static PolicyDecision Deny(string reason) => new PolicyDecision(false, reason);
    }

    /// <summary>
    /// Who may open the Command Center. The policy is the first thing the workflow runs: when it denies,
    /// no integration is called and no data is read — the trace shows the short circuit.
    ///
    /// Screens never test roles themselves (rule S-4 in docs/CodingStandards.md); they show whatever the
    /// service decided.
    /// </summary>
    public sealed class DashboardPolicy
    {
        private readonly ActivityTrace _trace;

        public DashboardPolicy(ActivityTrace trace)
        {
            _trace = trace;
        }

        public PolicyDecision CanViewCommandCenter(CommandContext ctx)
        {
            bool allowed = ctx.Role == UserRole.Manager || ctx.Role == UserRole.Admin;

            _trace.Security($"DashboardPolicy.CanViewCommandCenter({ctx.UserName} · {ctx.Role}) → {(allowed ? "allowed" : "DENIED")}");

            return allowed
                ? PolicyDecision.Allow($"{ctx.Role} may view the Command Center")
                : PolicyDecision.Deny($"The Command Center is available to Managers and Admins. Signed in as {ctx.UserName} ({ctx.Role}).");
        }
    }
}
