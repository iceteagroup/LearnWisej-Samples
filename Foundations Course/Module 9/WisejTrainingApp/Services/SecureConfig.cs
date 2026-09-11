using System;
using System.IO;
using System.Xml;
using Wisej.Web;

namespace WisejTrainingApp.Services
{
    // Secrets come from the environment first, then from Web.config — never from code or the UI.
    public class SecureConfig
    {
        public bool IsLicenseKeyConfigured
        {
            get { return !string.IsNullOrWhiteSpace(Read("WISEJ_LICENSE_KEY", "Wisej.LicenseKey")); }
        }

        private static string Read(string environmentVariable, string appSettingKey)
        {
            string value = Environment.GetEnvironmentVariable(environmentVariable);
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            string path = Path.Combine(Application.StartupPath, "Web.config");
            if (!File.Exists(path))
                return null;

            var doc = new XmlDocument();
            doc.Load(path);
            return doc.SelectSingleNode($"/configuration/appSettings/add[@key='{appSettingKey}']/@value")?.Value;
        }
    }
}
