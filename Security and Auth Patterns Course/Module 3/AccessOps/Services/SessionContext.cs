using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessOps.Services
{
    /// <summary>
    /// Who is on the other end of this session and what they may do - created only after a
    /// credential has been verified, stored once in <c>Application.Session</c>, and the only place
    /// any AccessOps code asks. Immutable: nothing downstream can change the identity or widen the
    /// permission set it carries.
    /// </summary>
    public sealed class SessionContext
    {
        public SessionContext(
            int employeeId, string userName, string displayName, string department, bool isManager,
            IEnumerable<string> roles, DateTime signedInUtc, string clientAddress, string userAgent, string signInMethod,
            PermissionSet permissions, IEnumerable<int> directReports, DateTime permissionsResolvedUtc)
        {
            this.EmployeeId = employeeId;
            this.UserName = userName;
            this.DisplayName = displayName;
            this.Department = department;
            this.IsManager = isManager;
            this.Roles = (roles ?? Enumerable.Empty<string>()).ToArray();
            this.SignedInUtc = signedInUtc;
            this.ClientAddress = clientAddress;
            this.UserAgent = userAgent;
            this.SignInMethod = signInMethod;
            this.Permissions = permissions ?? PermissionSet.Empty;
            this.DirectReports = (directReports ?? Enumerable.Empty<int>()).ToArray();
            this.PermissionsResolvedUtc = permissionsResolvedUtc;
        }

        public int EmployeeId { get; }
        public string UserName { get; }
        public string DisplayName { get; }

        /// <summary>The department this employee belongs to (the department id of the lab).</summary>
        public string Department { get; }

        /// <summary>True when at least one employee reports to this one.</summary>
        public bool IsManager { get; }

        public IReadOnlyList<string> Roles { get; }
        public DateTime SignedInUtc { get; }

        /// <summary>Captured at sign-in from Application.UserHostAddress.</summary>
        public string ClientAddress { get; }

        /// <summary>Captured at sign-in from Application.UserAgent.</summary>
        public string UserAgent { get; }

        /// <summary>"Password" or "RememberedDevice".</summary>
        public string SignInMethod { get; }

        /// <summary>Resolved once by PermissionService at sign-in.</summary>
        public PermissionSet Permissions { get; }

        /// <summary>The employees who report directly to this one (manager scope).</summary>
        public IReadOnlyList<int> DirectReports { get; }

        /// <summary>When <see cref="Permissions"/> was resolved; a revoked permission takes effect at the next resolve.</summary>
        public DateTime PermissionsResolvedUtc { get; }

        public bool Has(Permission permission) => this.Permissions.Has(permission);

        /// <summary>The same identity with a freshly resolved permission set (admin "force re-resolve").</summary>
        public SessionContext WithPermissions(PermissionSet permissions, IEnumerable<int> directReports)
            => new SessionContext(EmployeeId, UserName, DisplayName, Department, IsManager, Roles, SignedInUtc,
                ClientAddress, UserAgent, SignInMethod, permissions, directReports, DateTime.UtcNow);
    }

    /// <summary>Thrown by the typed accessor when a screen asks for the identity and there is none.</summary>
    public sealed class NotSignedInException : InvalidOperationException
    {
        public NotSignedInException() : base("Nobody is signed in to this session.") { }
    }

    /// <summary>
    /// The session boundary as the services see it. The Wisej.NET implementation reads and writes
    /// <c>Application.Session</c>; a test supplies its own, so no service needs a browser.
    /// </summary>
    public interface IAccessOpsSession
    {
        /// <summary>The verified identity; throws <see cref="NotSignedInException"/> when there is none.</summary>
        SessionContext Current { get; }

        /// <summary>The verified identity or null.</summary>
        SessionContext TryGetCurrent();

        /// <summary>The caller's address for this request (Application.UserHostAddress).</summary>
        string ClientAddress { get; }

        /// <summary>The caller's browser (Application.UserAgent).</summary>
        string UserAgent { get; }

        /// <summary>
        /// Stores a verified identity: first a new session ID (Application.RefreshSessionId), then
        /// the context. The order is the fixation defence.
        /// </summary>
        void Establish(SessionContext context);

        /// <summary>Replaces the context of an already signed-in session (re-resolved permissions).</summary>
        void Replace(SessionContext context);

        /// <summary>Removes the identity from the session.</summary>
        void Clear();

        /// <summary>Ends the session on the server (Application.Exit).</summary>
        void End();
    }
}
