using EnterpriseOps.Services;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// Who may press Deploy and Rollback. The runbook names "the person who may invoke it"; the code
    /// enforces it server-side (a hidden button is not a permission). ana.ops (Manager) and
    /// cara.admin (Admin) may release; ben.tech (Technician) may only watch.
    /// </summary>
    public static class ReleaseAuthorization
    {
        public static bool CanDeploy(SessionContext ctx) => ctx.Role == "Manager" || ctx.Role == "Admin";

        public static bool CanRollback(SessionContext ctx) => CanDeploy(ctx);

        public static string Explain(SessionContext ctx, string action)
            => $"Security: {ctx.User} ({ctx.Role}) {(CanDeploy(ctx) ? "may" : "may NOT")} {action} — rule: Manager or Admin";
    }
}
