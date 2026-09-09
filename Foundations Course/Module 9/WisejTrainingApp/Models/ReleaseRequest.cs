using System.Collections.Generic;

namespace WisejTrainingApp.Models
{
    /// <summary>
    /// Everything the review service needs to decide whether a review package can be created.
    /// The window fills it from the controls; the service trusts nothing else (not the button state).
    /// </summary>
    public class ReleaseRequest
    {
        public string Environment { get; set; }     // Debug | Staging | Production
        public string Target { get; set; }          // IIS | Kestrel | Cloud
        public string Version { get; set; }         // major.minor.patch
        public string Reviewer { get; set; }
        public string Role { get; set; }            // the role the server checks

        public int RequiredChecked { get; set; }
        public int RequiredTotal { get; set; }
        public int OptionalChecked { get; set; }
        public int OptionalTotal { get; set; }

        /// <summary>Optional items still unchecked — reported as open items in the summary.</summary>
        public List<string> OpenOptionalItems { get; set; } = new List<string>();

        public bool RollbackPlanWritten { get; set; }

        /// <summary>Safe, masked descriptions of the secrets (never the values).</summary>
        public string LicenseKeyStatus { get; set; }
        public string ConnectionStatus { get; set; }

        public string Notes { get; set; }

        /// <summary>Lab prop: makes the packaging step throw so the safe-message path can be seen.</summary>
        public bool SimulateError { get; set; }
    }
}
