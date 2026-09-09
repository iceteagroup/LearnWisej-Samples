using System.Collections.Generic;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// Permissions as a design concept (lesson s11 §5): no login system yet, just the role matrix.
    ///
    ///   Role           Dashboard  Tickets       Customers     Settings
    ///   Support Agent  View       Create/close  View          View only
    ///   Manager        View       Create/close  Create/edit   Save settings
    ///
    /// The shell calls these to decide what to enable; the views call them AGAIN before doing the
    /// protected action, so a disabled button is never the only thing standing between a user and a save.
    /// Later this grows into real authentication and authorization.
    /// </summary>
    public class PermissionService
    {
        public const string SupportAgent = "Support Agent";
        public const string Manager = "Manager";

        public static readonly string[] Roles = { SupportAgent, Manager };

        /// <summary>Only a Manager may save settings; a Support Agent sees them read-only.</summary>
        public bool CanSaveSettings(string role)
        {
            return role == Manager;
        }

        /// <summary>Only a Manager may add or edit customers; a Support Agent can view them.</summary>
        public bool CanEditCustomers(string role)
        {
            return role == Manager;
        }

        /// <summary>Both roles create and close tickets.</summary>
        public bool CanManageTickets(string role)
        {
            return role == SupportAgent || role == Manager;
        }

        /// <summary>The matrix row for one role, page by page — what the Settings view prints.</summary>
        public IReadOnlyList<KeyValuePair<string, string>> DescribeRole(string role)
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Dashboard", "View"),
                new KeyValuePair<string, string>("Tickets",   CanManageTickets(role) ? "Create / close" : "View"),
                new KeyValuePair<string, string>("Customers", CanEditCustomers(role) ? "Create / edit" : "View"),
                new KeyValuePair<string, string>("Settings",  CanSaveSettings(role) ? "Save settings" : "View only"),
            };
        }
    }
}
