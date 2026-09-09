using System;

namespace EnterpriseOps.Domain
{
    /// <summary>
    /// The version a client loaded and expects to still be current when it saves. The walkthrough wraps a
    /// SQL <c>rowversion</c> (<c>byte[] RowVersion</c>); this in-memory sample wraps the integer
    /// <see cref="WorkOrder.Version"/> — the contract is identical: opaque to the UI, compared by the store.
    /// </summary>
    public sealed record ConcurrencyToken(int Version)
    {
        public string ToDisplayText() => "v" + Version.ToString();

        public bool Matches(int currentVersion) => currentVersion == Version;

        public static ConcurrencyToken From(WorkOrder workOrder)
        {
            if (workOrder == null) throw new ArgumentNullException(nameof(workOrder));
            return new ConcurrencyToken(workOrder.Version);
        }
    }
}
