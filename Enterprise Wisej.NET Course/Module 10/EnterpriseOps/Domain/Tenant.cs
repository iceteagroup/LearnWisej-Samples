namespace EnterpriseOps.Domain
{
    /// <summary>A customer of the field-service company. Every record and every permission check is tenant-scoped.</summary>
    public sealed class Tenant
    {
        public Tenant(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }

        public override string ToString() => $"{Name} ({Id})";
    }

    /// <summary>The three tenants the course uses.</summary>
    public static class Tenants
    {
        public static readonly Tenant Contoso = new Tenant("contoso", "Contoso Facilities");
        public static readonly Tenant Fabrikam = new Tenant("fabrikam", "Fabrikam Industrial");
        public static readonly Tenant Northwind = new Tenant("northwind", "Northwind Utilities");

        public static Tenant Find(string id)
        {
            if (id == Contoso.Id) return Contoso;
            if (id == Fabrikam.Id) return Fabrikam;
            if (id == Northwind.Id) return Northwind;
            return null;
        }
    }
}
