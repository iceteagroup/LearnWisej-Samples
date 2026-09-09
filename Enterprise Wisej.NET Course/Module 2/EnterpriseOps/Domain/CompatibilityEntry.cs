namespace EnterpriseOps.Domain
{
    /// <summary>
    /// One cell of the compatibility matrix: a component, the version it has now, the version the
    /// target needs, whether that combination is supported, and what to do about it if not.
    /// </summary>
    public class CompatibilityEntry
    {
        public string Component { get; set; }
        public string CurrentVersion { get; set; }
        public string TargetVersion { get; set; }
        public bool Supported { get; set; }
        public RiskLevel Risk { get; set; }
        public string Mitigation { get; set; }

        public string SupportedText => Supported ? "✓ supported" : "✕ blocked";
        public string RiskCode => Risk switch { RiskLevel.High => "H", RiskLevel.Medium => "M", _ => "L" };
    }
}
