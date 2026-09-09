namespace EnterpriseOps.Domain
{
    /// <summary>
    /// A customer of the field-service company. Tenant state (branding, plan limits) belongs to one
    /// customer and is reached only through the tenant context — never through a static.
    /// </summary>
    public sealed class Tenant
    {
        public string Id { get; }
        public string Name { get; }

        public Tenant(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => $"{Name} ({Id})";
    }
}
