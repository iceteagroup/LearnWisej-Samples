namespace EnterpriseOps.Domain
{
    /// <summary>The categories the lesson says to inventory before touching code.</summary>
    public enum InventoryCategory
    {
        Project, Framework, Package, Startup, Theme, Resource, Component, Authentication, Deployment, Integration
    }

    /// <summary>
    /// One line of the migration inventory — current state and target state side by side, so nothing
    /// is "discovered halfway through the upgrade".
    /// </summary>
    public class InventoryItem
    {
        public InventoryCategory Category { get; set; }
        public string Name { get; set; }
        public string CurrentState { get; set; }
        public string TargetState { get; set; }
        public string Note { get; set; }

        public string CategoryText => Category.ToString();
    }
}
