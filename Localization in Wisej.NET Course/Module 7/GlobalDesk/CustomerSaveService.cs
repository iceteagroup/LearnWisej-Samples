using System;
using System.Text.RegularExpressions;

namespace GlobalDesk
{
    /// <summary>
    /// The outcome of a save, as a value the UI can translate.
    ///
    /// Not a sentence. A service that returns "Customer saved." has made a language decision on
    /// behalf of every caller - the web UI, a background job's log, a unit test - and only one of
    /// them wanted it.
    /// </summary>
    public enum SaveResult
    {
        Saved,
        NameRequired,
        EmailInvalid,
    }

    /// <summary>
    /// The domain side of saving a customer. It validates, it decides, and it says what happened
    /// in terms the domain owns. It knows nothing about resources, cultures or Wisej.NET.
    ///
    /// The rule this class exists to demonstrate: <b>domain code returns codes, the UI turns codes
    /// into words.</b> Everything below can be unit-tested with no session and no culture, and the
    /// day a second front end arrives it does not have to be rewritten.
    /// </summary>
    public static class CustomerSaveService
    {
        // The rule itself, not a message about the rule. It is not translated, because it is not
        // words: it is the definition of a valid address.
        private static readonly Regex EmailPattern =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public static SaveResult Save(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                return SaveResult.NameRequired;

            if (!string.IsNullOrWhiteSpace(email) && !EmailPattern.IsMatch(email))
                return SaveResult.EmailInvalid;

            // A real implementation would persist here.
            return SaveResult.Saved;
        }

        /// <summary>
        /// The one place a <see cref="SaveResult"/> becomes a resource key. Kept beside the enum so
        /// adding a result and forgetting its caption is a switch the compiler complains about,
        /// rather than a blank message a user reports.
        ///
        /// A <c>switch</c> rather than <c>"Validation." + result</c> on purpose: string
        /// concatenation compiles whatever you rename the enum to and fails at run time with a
        /// missing key, while this fails at the point of the change.
        /// </summary>
        public static string ResourceKeyFor(SaveResult result)
        {
            switch (result)
            {
                case SaveResult.Saved: return "CustomerEditor.Saved";
                case SaveResult.NameRequired: return "Validation.Required";
                case SaveResult.EmailInvalid: return "Validation.Email";
                default: throw new ArgumentOutOfRangeException(nameof(result), result, "no resource key for this result");
            }
        }
    }
}
