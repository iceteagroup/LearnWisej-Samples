using EnterpriseOps.Services;

namespace EnterpriseOps.Security
{
    /// <summary>The actions the migration screens and the migrated screens gate.</summary>
    public enum Permission
    {
        ViewDossier,
        RunHarness,
        ApproveWorkOrder,
        MapThemeMixin,
        RollbackMigrationStep
    }

    public class PermissionResult
    {
        public bool Allowed { get; set; }
        public string Reason { get; set; }
    }

    /// <summary>
    /// Server-side permission rules — the "same behavior" the Authentication dossier row promises.
    /// Rolling back a migration step and mapping the theme change the build for everyone, so they
    /// need a Manager or an Admin; a Technician gets a visible, explained denial.
    /// </summary>
    public class PermissionService
    {
        private readonly ActivityTrace _trace;

        public PermissionService(ActivityTrace trace)
        {
            _trace = trace;
        }

        public PermissionResult Check(CommandContext ctx, Permission permission)
        {
            bool allowed = permission switch
            {
                Permission.ViewDossier => true,
                Permission.RunHarness => true,
                Permission.ApproveWorkOrder => ctx.Role != Role.Technician,
                Permission.MapThemeMixin => ctx.Role != Role.Technician,
                Permission.RollbackMigrationStep => ctx.Role != Role.Technician,
                _ => false
            };

            var result = new PermissionResult
            {
                Allowed = allowed,
                Reason = allowed
                    ? $"{permission} for {ctx.UserName} ({ctx.Role}) → allowed"
                    : $"{permission} for {ctx.UserName} ({ctx.Role}) → DENIED (requires Manager or Admin)"
            };

            _trace.Security(result.Reason);
            return result;
        }

        /// <summary>Silent variant for the harness, which reports its own verdict.</summary>
        public bool IsAllowed(Role role, Permission permission)
        {
            return permission switch
            {
                Permission.ViewDossier => true,
                Permission.RunHarness => true,
                _ => role != Role.Technician
            };
        }
    }
}
