using System;
using System.IO;
using System.Xml;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// Lesson s41 §3 — secrets come from secure configuration or environment variables, never from
    /// UI code. This class is the only place that touches the raw values, and it never hands them out:
    /// callers get a <see cref="SecretStatus"/> with a masked description (last 4 characters) or a
    /// "MISSING" hint that names the variable to set.
    ///
    /// Lookup order: environment variable → Web.config appSettings (parsed with XmlDocument, so no
    /// System.Configuration dependency) → missing.
    /// </summary>
    public class SecureConfig
    {
        public const string LicenseKeyVariable = "WISEJ_LICENSE_KEY";
        public const string LicenseKeyAppSetting = "Wisej.LicenseKey";
        public const string ConnectionVariable = "TRAINING_CONNECTION_STRING";
        public const string ConnectionAppSetting = "Training.Connection";

        private readonly string _webConfigPath;

        /// <param name="startupPath">The folder the app runs from (Application.StartupPath); Web.config is read from there.</param>
        public SecureConfig(string startupPath)
        {
            _webConfigPath = Path.Combine(startupPath ?? string.Empty, "Web.config");
        }

        public SecretStatus LicenseKey => Describe(LicenseKeyVariable, LicenseKeyAppSetting);

        public SecretStatus Connection => Describe(ConnectionVariable, ConnectionAppSetting);

        /// <summary>Reads the value and immediately reduces it to a safe description.</summary>
        public SecretStatus Describe(string environmentVariable, string appSettingKey)
        {
            string value = Environment.GetEnvironmentVariable(environmentVariable);
            string source = "environment variable " + environmentVariable;

            if (string.IsNullOrWhiteSpace(value))
            {
                value = ReadAppSetting(appSettingKey);
                source = "Web.config appSettings " + appSettingKey;
            }

            if (string.IsNullOrWhiteSpace(value))
                return SecretStatus.Missing(environmentVariable);

            return SecretStatus.Configured(Mask(value), source);
        }

        /// <summary>"••••1234" — only the last four characters survive. Shorter values are fully masked.</summary>
        public static string Mask(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "••••";

            string tail = value.Length > 4 ? value.Substring(value.Length - 4) : string.Empty;
            return "••••" + tail;
        }

        private string ReadAppSetting(string key)
        {
            try
            {
                if (!File.Exists(_webConfigPath))
                    return null;

                var doc = new XmlDocument();
                doc.Load(_webConfigPath);
                XmlNode node = doc.SelectSingleNode($"/configuration/appSettings/add[@key='{key}']/@value");
                return node?.Value;
            }
            catch (Exception)
            {
                // A broken Web.config counts as "missing" — the reviewer sees a hint, never an exception.
                return null;
            }
        }
    }

    /// <summary>A secret described without its value.</summary>
    public class SecretStatus
    {
        public bool IsConfigured { get; private set; }
        public string Masked { get; private set; }
        public string Source { get; private set; }
        public string VariableName { get; private set; }

        /// <summary>"configured (masked ••••1234)" or "MISSING — set WISEJ_LICENSE_KEY".</summary>
        public string Display => IsConfigured
            ? $"configured (masked {Masked})"
            : $"MISSING — set {VariableName}";

        public static SecretStatus Configured(string masked, string source) =>
            new SecretStatus { IsConfigured = true, Masked = masked, Source = source };

        public static SecretStatus Missing(string variableName) =>
            new SecretStatus { IsConfigured = false, VariableName = variableName };
    }
}
