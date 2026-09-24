namespace GlobalDesk
{
    /// <summary>
    /// The one place the application asks for a piece of text.
    ///
    /// Every user-visible string in GlobalDesk comes through here, which buys three things: one
    /// decision about what a missing key looks like, one place to change if the resource layout
    /// ever moves, and a single name to search for when somebody asks "where does this wording
    /// come from?".
    ///
    /// From Module 7 it is a forwarder: <see cref="LocalizationService"/> holds the
    /// implementation, the missing-key policy and the date, number and currency formatting.
    /// </summary>
    public static class Texts
    {
        /// <summary>
        /// Kept as a thin forwarder so the six modules' call sites still read the same, while
        /// there is now exactly one implementation behind them. Delete it once every caller has
        /// moved to <see cref="LocalizationService"/> - a shim that survives is a second API.
        /// </summary>
        public static string Get(string key) => LocalizationService.Text(key);
    }
}
