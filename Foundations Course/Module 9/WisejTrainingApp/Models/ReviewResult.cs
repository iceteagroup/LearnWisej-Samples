namespace WisejTrainingApp.Models
{
    /// <summary>
    /// What the review service answers. <see cref="Message"/> is always safe to show to the user;
    /// <see cref="Summary"/> is the review package text (only filled on success).
    /// </summary>
    public class ReviewResult
    {
        public bool Success { get; set; }

        /// <summary>Why it failed: "permission", "checks", "version" — or null on success.</summary>
        public string Reason { get; set; }

        /// <summary>A user-facing message: "Review is restricted.", "Required checks incomplete.", "Package ready for deployment."</summary>
        public string Message { get; set; }

        public string Summary { get; set; }

        public static ReviewResult Fail(string reason, string message) =>
            new ReviewResult { Success = false, Reason = reason, Message = message };

        public static ReviewResult Ok(string summary) =>
            new ReviewResult { Success = true, Message = "Package ready for deployment.", Summary = summary };
    }
}
