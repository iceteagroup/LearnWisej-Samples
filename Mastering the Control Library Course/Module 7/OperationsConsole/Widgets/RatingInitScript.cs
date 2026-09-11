using System;
using System.IO;
using System.Reflection;
using Wisej.Web;

namespace OperationsConsole.Widgets
{
    /// <summary>
    /// Loads the text of <c>wwwroot/rating-init.js</c> for <c>ratingWidget.InitScript</c>. The adapter is compiled
    /// into the assembly as an embedded resource (see the <c>&lt;EmbeddedResource&gt;</c> entry in the csproj);
    /// <c>Widget.GetResourceString</c> is <c>protected</c>, so a plain <see cref="Widget"/> on a page reads the
    /// resource with reflection. Set <see cref="PreferFileWhileEditing"/> to read the file on disk instead while
    /// editing the adapter.
    /// </summary>
    public static class RatingInitScript
    {
        /// <summary>The logical name declared in the csproj. Must match exactly.</summary>
        public const string ResourceName = "OperationsConsole.wwwroot.rating-init.js";

        /// <summary>The same file on disk, relative to the project folder.</summary>
        public const string FilePath = "wwwroot/rating-init.js";

        /// <summary>Read the file on every page load instead of the compiled resource.</summary>
        public static bool PreferFileWhileEditing { get; set; }

        /// <summary>Returns the adapter source: the embedded resource first, then the file next to the project.</summary>
        /// <exception cref="InvalidOperationException">Neither the resource nor the file could be read.</exception>
        public static string Load()
        {
            if (!PreferFileWhileEditing)
            {
                string fromResource = TryReadResource();
                if (fromResource != null)
                    return fromResource;
            }

            string fromFile = TryReadFile();
            if (fromFile != null)
                return fromFile;

            throw new InvalidOperationException(
                "rating-init.js was found neither as the embedded resource '" + ResourceName +
                "' nor as the file '" + FilePath + "'.");
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
