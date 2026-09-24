namespace GlobalDesk
{
    /// <summary>
    /// The one place the application used to ask for a piece of text.
    ///
    /// From Module 7 it is a thin forwarder: <see cref="LocalizationService"/> holds the
    /// implementation, the missing-key policy and the date, number and currency formatting, and a
    /// solution-wide search for <c>ResourceManager</c> returns that one file.
    ///
    /// It is kept so the earlier modules' call sites still read the same. Delete it once every
    /// caller has moved across - a shim that survives is a second API, and a second API is a
    /// second place for the missing-key policy to be decided.
    /// </summary>
    public static class Texts
    {
        public static string Get(string key) => LocalizationService.Text(key);
    }
}
