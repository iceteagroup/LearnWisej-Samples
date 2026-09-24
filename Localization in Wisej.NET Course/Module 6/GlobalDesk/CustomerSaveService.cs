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
        CodeFormatInvalid,
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
        private static readonly Regex CodePattern = new Regex("^[A-Za-z]{2}[0-9]{4}$", RegexOptions.Compiled);

        public static SaveResult Save(string name, string code)
        {
            if (string.IsNullOrWhiteSpace(name))
                return SaveResult.NameRequired;

            if (!string.IsNullOrWhiteSpace(code) && !CodePattern.IsMatch(code))
                return SaveResult.CodeFormatInvalid;

            // A real implementation would persist here.
            return SaveResult.Saved;
        }

        /// <summary>
        /// The one place a <see cref="SaveResult"/> becomes a resource key. Kept beside the enum so
        /// adding a result and forgetting its caption is a switch the compiler complains about,
        /// rather than a blank message a user reports.
        /// </summary>
        public static string ResourceKeyFor(SaveResult result)
        {
            switch (result)
            {
                case SaveResult.Saved: return "CustomerEditor.Saved";
                case SaveResult.NameRequired: return "Validation.Required";
                case SaveResult.CodeFormatInvalid: return "Validation.CodeFormat";
                default: throw new ArgumentOutOfRangeException(nameof(result), result, "no resource key for this result");
            }
        }
    }
}
