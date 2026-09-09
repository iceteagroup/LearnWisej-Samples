namespace EnterpriseOps.Domain
{
    /// <summary>A customer organisation whose work orders are isolated from every other tenant's.</summary>
    public sealed class Tenant
    {
        public Tenant(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }

        public override string ToString() => Id;
    }

    /// <summary>The three tenants every module of the course seeds.</summary>
    public static class Tenants
    {
        public static readonly Tenant Contoso = new Tenant("contoso", "Contoso Field Services");
        public static readonly Tenant Fabrikam = new Tenant("fabrikam", "Fabrikam Utilities");
        public static readonly Tenant Northwind = new Tenant("northwind", "Northwind Facilities");
    }
}
