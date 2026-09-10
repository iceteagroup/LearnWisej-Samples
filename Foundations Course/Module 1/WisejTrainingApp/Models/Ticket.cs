namespace WisejTrainingApp.Models
{
    /// <summary>
    /// A simple data class — the kind the lesson's "Inspect the solution structure" table puts under
    /// Models/. It holds the shape of one record and nothing else: no UI, no rules, no storage.
    /// </summary>
    public class Ticket
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public string Customer { get; set; } = "";

        public string Status { get; set; } = "Open";
    }
}
