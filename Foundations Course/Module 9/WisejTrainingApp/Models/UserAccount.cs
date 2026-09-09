namespace WisejTrainingApp.Models
{
    /// <summary>
    /// The signed-in user as Module 9 sees it: authentication already answered "who is this?"
    /// (Name), authorization asks "what may they do?" (Role).
    /// Roles used by the course: "Support Agent", "Team Lead", "Admin".
    /// One instance per session — it lives in the window, never in a static field.
    /// </summary>
    public class UserAccount
    {
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
