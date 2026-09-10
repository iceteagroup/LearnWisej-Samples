using System;
using System.IO;
using System.Reflection;
using Wisej.Web;

namespace OperationsConsole.Widgets
{
    /// <summary>
    /// Loads the text of <c>wwwroot/rating-init.js</c> for <c>ratingWidget.InitScript</c>.
    /// <para>
    /// The module reading rules out one option and recommends the others: "Scripts in the startup
    /// HTML can be too early for Wisej.NET widgets; embedded resources, Packages, InitScript, and
    /// control-level calls are safer patterns." So the adapter is compiled into the assembly as an
    /// embedded resource (see the <c>&lt;EmbeddedResource&gt;</c> entry in OperationsConsole.csproj)
    /// and read from there — it cannot be missing at runtime, cannot be cached by the browser and
    /// cannot be edited on a deployed server.
    /// </para>
    /// <para>
    /// <b>Why not <c>Widget.GetResourceString(...)</c>?</b> That helper is <c>protected</c> on
    /// <see cref="Wisej.Web.Widget"/>, so it is only reachable from a Widget <i>subclass</i>. This
    /// module uses a plain <c>Wisej.Web.Widget</c> dropped on the page (the lab's shape), so the
    /// resource is read with ordinary reflection instead — same bytes, one less subclass.
    /// </para>
    /// <para>
    /// The file fallback exists for one reason: while editing the adapter you can change
    /// <c>wwwroot/rating-init.js</c> and press F5 instead of rebuilding — set
    /// <see cref="PreferFileWhileEditing"/> to true.
    /// </para>
    /// </summary>
    public static class RatingInitScript
    {
        /// <summary>The logical name declared in the csproj. Must match exactly.</summary>
        public const string ResourceName = "OperationsConsole.wwwroot.rating-init.js";

        /// <summary>The same file on disk, relative to the project folder.</summary>
        public const string FilePath = "wwwroot/rating-init.js";

        /// <summary>Read the file on every page load instead of the compiled resource (edit + F5 loop).</summary>
        public static bool PreferFileWhileEditing { get; set; }

        /// <summary>Where the script actually came from — logged to the Event log so the reviewer can see it.</summary>
        public static string LastSource { get; private set; } = "(not loaded yet)";

        /// <summary>
        /// Returns the adapter source. Tries the embedded resource first (unless
        /// <see cref="PreferFileWhileEditing"/> is set), then the file next to the project.
        /// </summary>
        /// <exception cref="InvalidOperationException">Neither the resource nor the file could be read.</exception>
        public static string Load()
        {
            if (!PreferFileWhileEditing)
            {
                string fromResource = TryReadResource();
                if (fromResource != null)
                {
                    LastSource = "embedded resource " + ResourceName;
                    return fromResource;
                }
            }

            string fromFile = TryReadFile();
            if (fromFile != null)
            {
                LastSource = "file " + FilePath;
                return fromFile;
            }

            throw new InvalidOperationException(
                "rating-init.js was found neither as the embedded resource '" + ResourceName +
                "' nor as the file '" + FilePath + "'. Check the <EmbeddedResource> entry in OperationsConsole.csproj.");
        }

        private static string TryReadResource()
        {
            try
            {
                Assembly assembly = typeof(RatingInitScript).Assembly;
                using (Stream stream = assembly.GetManifestResourceStream(ResourceName))
                {
                    if (stream == null) return null;
                    using (var reader = new StreamReader(stream))
                        return reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string TryReadFile()
        {
            try
            {
                // Application.MapPath resolves against the application's project directory
                // (Startup.cs serves the project folder, so wwwroot/ is right here).
                string path = Application.MapPath(FilePath);
                return File.Exists(path) ? File.ReadAllText(path) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
