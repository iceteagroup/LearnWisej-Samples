using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace TicketOps.Security
{
    /// <summary>
    /// The authenticated identity. Immutable: it is created by <see cref="AuthenticationService"/> after
    /// the credential was verified and never changes afterwards.
    ///
    /// It also implements <see cref="IPrincipal"/>/<see cref="IIdentity"/> (plain .NET, no Wisej type) so the
    /// Infrastructure layer can hand the same object to <c>Wisej.Web.Application.User</c>; the framework's
    /// <c>Application.IsAuthenticated</c> then answers from <see cref="IsAuthenticated"/>.
    /// </summary>
    public sealed class UserContext : IUserContext, IPrincipal, IIdentity
    {
        private readonly HashSet<string> _roles;

        public UserContext(string userName, string displayName, IEnumerable<string> roles)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("A user name is required.", nameof(userName));
            UserName = userName;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? userName : displayName;
            _roles = new HashSet<string>(roles ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
        }

        public string UserName { get; }
        public string DisplayName { get; }
        public IReadOnlyCollection<string> Roles => _roles;

        public bool IsInRole(string role) => role != null && _roles.Contains(role);

        // IIdentity — the identity IS this object.
        string IIdentity.Name => UserName;
        string IIdentity.AuthenticationType => "TicketOps-Demo";
        public bool IsAuthenticated => true;

        // IPrincipal
        IIdentity IPrincipal.Identity => this;

        public override string ToString() => $"{UserName} [{string.Join(", ", _roles)}]";
    }
}
