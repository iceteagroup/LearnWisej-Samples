using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Wisej.Web;

namespace OrderDesk.Configuration
{
    /// <summary>
    /// ✓ The web-side replacement for <c>ConfigurationManager</c>: App.config's <c>&lt;appSettings&gt;</c> and
    /// <c>&lt;connectionStrings&gt;</c> moved into <c>Web.config</c> in the content root, and the project reads them
    /// with <c>System.Xml.Linq</c>. Nothing else is needed — <c>System.Configuration.ConfigurationManager</c> is not
    /// referenced by the web project, and <c>Application.StartupPath</c> is the project folder under
    /// <c>dotnet run</c> / F5 (the same file the host reads for its own <c>Wisej.*</c> keys).
    /// </summary>
    public static class WebConfig
    {
        /// <summary>The file the web host reads: Web.config in the content root (never copied to bin).</summary>
        public static string FilePath => Path.Combine(Application.StartupPath, "Web.config");

        /// <summary><c>&lt;connectionStrings&gt;/add[@name=…]/@connectionString</c>, or null when the name is missing.</summary>
        public static string ConnectionString(string name)
        {
            var document = XDocument.Load(FilePath);
            return document.Root?.Element("connectionStrings")?.Elements("add")
                .FirstOrDefault(add => (string)add.Attribute("name") == name)
                ?.Attribute("connectionString")?.Value;
        }

        /// <summary><c>&lt;appSettings&gt;/add[@key=…]/@value</c>, or null when the key is missing.</summary>
        public static string AppSetting(string key)
        {
            var document = XDocument.Load(FilePath);
            return document.Root?.Element("appSettings")?.Elements("add")
                .FirstOrDefault(add => (string)add.Attribute("key") == key)
                ?.Attribute("value")?.Value;
        }

        /// <summary>The connection string with any Password=/Pwd= value replaced by dots — safe for a banner or a log.</summary>
        public static string Mask(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return connectionString;
            return Regex.Replace(connectionString, @"(?i)\b(password|pwd)\s*=\s*[^;]*", "$1=•••••");
        }
    }
}
