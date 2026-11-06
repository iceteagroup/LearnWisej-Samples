using Microsoft.Extensions.Logging;

namespace AccessOps.Services
{
    /// <summary>The authorization guard every service method calls on its first line.</summary>
    public interface IAuthorize
    {
        /// <summary>
        /// Throws <see cref="NotAuthorizedException"/> unless the signed-in caller holds the
        /// permission. Reads the caller from the session - never from a parameter - and logs the
        /// user, the permission and the target before throwing.
        /// </summary>
        SessionContext Require(Permission permission, string target = null);

        /// <summary>Like <see cref="Require"/>, satisfied by any one of the permissions.</summary>
        SessionContext RequireAny(params Permission[] permissions);

        /// <summary>Records a refusal decided after the permission check (record outside the caller's scope).</summary>
        NotAuthorizedException Refuse(Permission permission, string target, string reason);
    }

    public sealed class Authorize : IAuthorize
    {
        public const string RefusalMessage = "You are not allowed to do that. If you think that is wrong, ask your department administrator.";

        private readonly IAccessOpsSession _session;
        private readonly ILogger<Authorize> _log;

        public Authorize(IAccessOpsSession session, ILogger<Authorize> log)
        {
            _session = session;
            _log = log;
        }

        public SessionContext Require(Permission permission, string target = null)
        {
            var me = _session.Current;   // throws NotSignedInException when nobody is signed in
            if (me.Has(permission))
                return me;

            throw Refuse(permission, target, "permission not in session permission set");
        }

        public SessionContext RequireAny(params Permission[] permissions)
        {
            var me = _session.Current;
            foreach (var p in permissions)
                if (me.Has(p))
                    return me;

            throw Refuse(permissions[permissions.Length - 1], null, "none of " + string.Join("/", permissions) + " in session permission set");
        }

        public NotAuthorizedException Refuse(Permission permission, string target, string reason)
        {
            var me = _session.TryGetCurrent();
            _log.LogWarning(
                // a reference (the employee id), never a name or an e-mail address
                "DENIED {Permission} for employee {EmployeeId} ({Department}) on {Target}: {Reason}",
                permission, me?.EmployeeId, me?.Department, target ?? "-", reason);
            return new NotAuthorizedException(RefusalMessage);
        }
    }
}
