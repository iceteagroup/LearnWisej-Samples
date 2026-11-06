using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AccessOps.Services
{
    /// <summary>
    /// The actions AccessOps performs. Code tests these - never a role name. Administrators assign
    /// roles; the mapping from roles to permissions is configuration (appsettings.json).
    /// </summary>
    public enum Permission
    {
        ViewDepartmentDocuments,
        ViewAllDocuments,
        UploadDocument,
        SubmitRequest,
        ApproveRequest,
        ViewAuditTrail,
        ManageUsers
    }

    /// <summary>
    /// A flat, sorted, immutable set of permissions. There is no Add: nothing downstream of
    /// <see cref="PermissionService"/> can widen it.
    /// </summary>
    public sealed class PermissionSet : IEnumerable<Permission>
    {
        private readonly Permission[] _items;

        private PermissionSet(IEnumerable<Permission> items)
            => _items = items.Distinct().OrderBy(p => p.ToString(), StringComparer.Ordinal).ToArray();

        public static PermissionSet Empty { get; } = new PermissionSet(Array.Empty<Permission>());

        public static PermissionSet Immutable(IEnumerable<Permission> items) => new PermissionSet(items ?? Array.Empty<Permission>());

        public bool Has(Permission permission) => Array.IndexOf(_items, permission) >= 0;

        public int Count => _items.Length;

        public IEnumerator<Permission> GetEnumerator() => ((IEnumerable<Permission>)_items).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public override string ToString() => string.Join(", ", _items);
    }

    /// <summary>Thrown by the authorization guard. Its message is safe to show; the details are logged.</summary>
    public sealed class NotAuthorizedException : Exception
    {
        public NotAuthorizedException(string message) : base(message) { }
    }
}
