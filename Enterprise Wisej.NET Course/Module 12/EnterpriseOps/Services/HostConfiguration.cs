using Microsoft.Extensions.Configuration;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The configuration this PROCESS started with — captured once in Startup.cs from
    /// builder.Configuration. It is process-wide by nature (the same for every session), which is
    /// why a static holder is right here and wrong for anything per user or per tenant.
    /// </summary>
    public static class HostConfiguration
    {
        public static IConfiguration Configuration { get; private set; }
        public static string EnvironmentName { get; private set; } = "(not captured)";
        public static string ContentRootPath { get; private set; } = ".";
        public static ValidationResult StartupValidation { get; private set; }

        public static string ReleaseVersion => Configuration?["EnterpriseOps:Release:Version"] ?? "?";
        public static string PreviousVersion => Configuration?["EnterpriseOps:Release:PreviousVersion"] ?? "?";
        public static string NodeName => Configuration?["EnterpriseOps:Release:NodeName"] ?? "app-node-A";
        public static string StorageRoot => Configuration?["EnterpriseOps:Storage:Root"] ?? "./_storage";

        public static void Capture(IConfiguration configuration, string environmentName, string contentRootPath)
        {
            Configuration = configuration;
            EnvironmentName = environmentName;
            ContentRootPath = contentRootPath;
        }

        public static void RecordStartupValidation(ValidationResult result) => StartupValidation = result;
    }
}
