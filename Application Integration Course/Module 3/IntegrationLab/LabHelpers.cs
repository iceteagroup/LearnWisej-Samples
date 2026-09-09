using System;
using System.IO;
using System.Reflection;
using Wisej.Core;

namespace IntegrationLab
{
    /// <summary>
    /// Reads an InitScript that ships inside this assembly (see the EmbeddedResource
    /// entries in IntegrationLab.csproj).
    ///
    /// Widget.GetResourceString(name) does the same thing, but it is protected: a page
    /// configuring a plain Wisej.Web.Widget (no subclass) cannot call it, so the lab
    /// reads the manifest resource directly. The logical name is the one in the csproj.
    /// </summary>
    internal static class EmbeddedScript
    {
        public static string Read(string logicalName)
        {
            var assembly = typeof(EmbeddedScript).Assembly;
            using (var stream = assembly.GetManifestResourceStream(logicalName))
            {
                if (stream == null)
                    throw new FileNotFoundException(
                        $"Embedded resource '{logicalName}' not found. Check the <EmbeddedResource LogicalName=…> entry in IntegrationLab.csproj.");

                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }
    }

    /// <summary>
    /// JSON for the live trace. Uses the framework's own serializer (the one that renders
    /// Widget.Options), so what the trace prints is what travels: anonymous objects and
    /// classes come out camel-cased (C# MinValue → JSON minValue), which is the
    /// camel-casing evidence the lesson asks for.
    /// </summary>
    internal static class LabJson
    {
        public static string Of(object value)
        {
            try
            {
                return WisejSerializer.Serialize(value, WisejSerializerOptions.CamelCase | WisejSerializerOptions.IgnoreNulls);
            }
            catch (Exception ex)
            {
                return $"<not serializable: {ex.Message}>";
            }
        }
    }
}
