namespace EnterpriseOps.Domain
{
    /// <summary>Business priority of a work order. The dashboard shows Normal as "Medium" (a UI projection, not a domain rename).</summary>
    public enum Priority
    {
        Low,
        Normal,
        High,
        Critical
    }
}
