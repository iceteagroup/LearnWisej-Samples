using System;
using System.Linq;

namespace EnterpriseOps.Security
{
    public sealed class AccessDecision
    {
        public bool Allowed { get; init; }
        public string Reason { get; init; }
    }

    /// <summary>
    /// The diagnostics page is an attack surface, so it is role-protected: only operators (Manager, Admin)
    /// may open it. The decision is made here, on the server, never by hiding a button in the UI.
    /// </summary>
    public sealed class DiagnosticsAccessPolicy
    {
        public static readonly Role[] OperatorRoles = { Role.Manager, Role.Admin };

        public AccessDecision CanViewDiagnostics(AppUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            bool allowed = OperatorRoles.Contains(user.Role);
            return new AccessDecision
            {
                Allowed = allowed,
                Reason = allowed
                    ? $"{user.Role} is an operator role"
                    : $"{user.Role} is not an operator role (allowed: {string.Join(", ", OperatorRoles)})",
            };
        }
    }
}
