using System.Reflection;
using Wisej.Resources;

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
    /// <see cref="ResourceManager"/> does the culture work. It reads
    /// <c>Application.CurrentCulture</c> - which is per session - and walks .NET's fallback chain:
    /// the most specific resource first (<c>fr-CA</c>), then the language (<c>fr</c>), then the
    /// neutral file. The first value it finds wins, which is why the neutral file must be complete:
    /// it is the last place the search looks.
    /// </summary>
    public static class Texts
    {
        /// <summary>
        /// The base name is the resource's manifest name without the culture or the extension:
        /// <c>Resources/Strings.resx</c> in a project whose root namespace is <c>GlobalDesk</c>.
        /// </summary>
        private static readonly ResourceManager Resources =
            new ResourceManager("GlobalDesk.Resources.Strings", typeof(Texts).Assembly);

        /// <summary>
        /// Returns the text for <paramref name="key"/>, or the key in brackets if there is none.
        ///
        /// The bracketed marker is the point of the method. Returning an empty string hides a
        /// missing key behind a blank caption that nobody reports; <c>[Dashboard.Welcome]</c> on
        /// screen is a defect anyone can see and describe.
        /// </summary>
        public static string Get(string key) => Resources.GetString(key) ?? $"[{key}]";
    }
}
