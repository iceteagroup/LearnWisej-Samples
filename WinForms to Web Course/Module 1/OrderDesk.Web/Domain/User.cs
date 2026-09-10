namespace OrderDesk.Domain
{
    public sealed class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Role { get; set; } = "Clerk";
        public string Company { get; set; } = "Acme";

        public override string ToString() => UserName;
    }
}
