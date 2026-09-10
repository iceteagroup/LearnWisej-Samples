namespace EnterpriseOps.Domain
{
    /// <summary>Work order priority. Used by the export policy: Critical rows count as sensitive data.</summary>
    public enum Priority
    {
        Low,
        Normal,
        High,
        Critical
    }
}
