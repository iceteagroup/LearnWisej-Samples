using System;
using System.Threading.Tasks;
using FieldOps.Services;
using Wisej.Hybrid;
using Wisej.Web;

namespace FieldOps
{
    /// <summary>
    /// Two gates in front of the camera and the location: FieldOps' one sentence of reason, then the
    /// platform prompt. Asked once per press - never in a loop - and every outcome is logged per
    /// feature. A refusal is a state of the screen, not an error.
    /// </summary>
    public sealed class PermissionGate
    {
        public enum Outcome { Granted, Denied, DeniedForGood }

        /// <summary>Shows a sentence on the screen ("info", "warn", "bad").</summary>
        private readonly Action<string, string> _say;

        public PermissionGate(Action<string, string> say)
        {
            _say = say;
        }

        public async Task<Outcome> AskAsync(PermissionType permission, string reason)
        {
            var status = await Device.Permissions.CheckAsync(permission);
            if (status == PermissionStatus.Granted)
            {
                Log(permission, "already granted");
                return Outcome.Granted;
            }

            _say(reason, "info");                        // gate 1: our sentence
            status = await Device.Permissions.RequestAsync(permission);   // gate 2: the platform
            if (status == PermissionStatus.Granted)
            {
                Log(permission, "granted after the reason");
                return Outcome.Granted;
            }

            // Denied - but for now, or for good? Android answers through the rationale flag;
            // Restricted (parental controls, MDM) is always for good.
            bool rationale = status != PermissionStatus.Restricted
                && await Device.Permissions.ShouldShowRationaleAsync(permission);
            Log(permission, $"{status}, rationale={rationale}");
            return rationale ? Outcome.Denied : Outcome.DeniedForGood;
        }

        /// <summary>The only cure for a permanent refusal: the app's own settings page.</summary>
        public static void OpenSettings()
        {
            OpsLog.Info("permission: Device.AppInfo.ShowSettingsUI()");
            Device.AppInfo.ShowSettingsUI();
        }

        private static void Log(PermissionType permission, string outcome)
        {
            OpsLog.Info($"permission: {permission} {outcome} (session {Application.SessionId[..8]})");
            UsageTelemetry.Record("permission." + permission.ToString().ToLowerInvariant(), outcome);
        }
    }
}
