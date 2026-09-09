namespace WisejTrainingApp.Services
{
    /// <summary>
    /// Lesson s42 §3 — the junior-level checks per deployment target, as the default text of the
    /// deployment notes box. The window swaps the text when cboTarget changes.
    /// </summary>
    public static class DeploymentNotes
    {
        public static readonly string[] Targets = { "IIS", "Kestrel", "Cloud" };

        public static string For(string target)
        {
            switch (target)
            {
                case "IIS":
                    return "IIS — publish the app folder (dotnet publish -c Release), review Web.config, "
                         + "check the ASP.NET Core hosting bundle if needed, confirm the app pool (No Managed Code, "
                         + "correct identity), test the URL.";

                case "Kestrel":
                    return "Kestrel / self-host — confirm environment variables (WISEJ_LICENSE_KEY, ASPNETCORE_ENVIRONMENT), "
                         + "the port (--urls), the reverse proxy (nginx / IIS ARR), HTTPS certificates and the JSON config "
                         + "files (Default.json, appsettings.json).";

                case "Cloud":
                    return "Cloud — confirm app settings and secrets in the portal / key vault, static files "
                         + "(themes, Widgets/*), logging destination, and the deployment slot / staging setup before swapping.";

                default:
                    return "Pick a deployment target to load its checks.";
            }
        }
    }
}
