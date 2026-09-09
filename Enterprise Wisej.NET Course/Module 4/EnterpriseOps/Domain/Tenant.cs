namespace EnterpriseOps.Domain
{
    /// <summary>A customer organisation. Every work order, query and command carries a TenantId.</summary>
    public class Tenant
    {
        public string Id { get; set; }        // "contoso", "fabrikam", "northwind"
        public string Name { get; set; }
    }
}
