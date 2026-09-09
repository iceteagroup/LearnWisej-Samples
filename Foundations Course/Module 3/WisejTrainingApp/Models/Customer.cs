namespace WisejTrainingApp.Models
{
    /// <summary>A customer of the ServiceDesk — the list CustomersView shows.</summary>
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string City { get; set; }
    }
}
