using System;
using System.IO;
using System.Xml.Linq;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// The desktop way to read configuration, copied into the web project to show why it breaks.
    /// LegacyOrderDesk called <c>ConfigurationManager.ConnectionStrings["OrderDesk"]</c>, which reads the
    /// <c>&lt;exe&gt;.config</c> file next to the executable. On the web host neither half exists:
    ///
    ///   ✕ System.Configuration.ConfigurationManager is not referenced by the web project (CS0103 if you try),
    ///   ✕ there is no App.config / LegacyOrderDesk.dll.config next to the running assembly — the host's
    ///     bin folder holds OrderDesk.dll and Kestrel, and configuration lives in Web.config in the content root.
    ///
    /// The class does what ConfigurationManager does under the hood (find the exe config, parse it) so the
    /// failure is a real FileNotFoundException the console can catch and explain.
    /// </summary>
    public static class AppConfig
    {
        /// <summary>The file ConfigurationManager would look for: &lt;assembly&gt;.dll.config next to the binary.</summary>
        public static string ExeConfigPath => Path.Combine(AppContext.BaseDirectory, "LegacyOrderDesk.dll.config");

        /// <summary>✕ ConfigurationManager.ConnectionStrings[name].ConnectionString — the desktop pattern.</summary>
        public static string ConnectionString(string name)
        {
            // ✕ return ConfigurationManager.ConnectionStrings[name].ConnectionString;   // CS0103 on the web project
            if (!File.Exists(ExeConfigPath))
                throw new FileNotFoundException($"App.config was deployed as '{Path.GetFileName(ExeConfigPath)}' next to the desktop .exe; the web host has no such file.", ExeConfigPath);

            var document = XDocument.Load(ExeConfigPath);
            foreach (var add in document.Root.Element("connectionStrings")?.Elements("add") ?? Array.Empty<XElement>())
                if ((string)add.Attribute("name") == name)
                    return (string)add.Attribute("connectionString");
            return null;
        }
    }
}
