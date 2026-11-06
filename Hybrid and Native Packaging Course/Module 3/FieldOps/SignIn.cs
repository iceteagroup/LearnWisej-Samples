using System;
using FieldOps.Model;
using FieldOps.Services;
using Wisej.Hybrid;
using Wisej.Web;

namespace FieldOps
{
    /// <summary>
    /// What happens once a technician is signed in: the session remembers who it is, the desktop
    /// window says whose session it holds, and the support log records which client opened it.
    /// </summary>
    internal static class SignIn
    {
        public static void Complete(Technician technician)
        {
            Session.Technician = technician;
            OpsLog.Info($"session {Application.SessionId[..8]} signed in {technician.Name}");
            SignIn_Completed(technician);
        }

        /// <summary>
        /// Called by the list page once it is loaded: the window says whose session it holds.
        /// In 4.1.7 on Windows, Device.TitleBar is a title band the client draws INSIDE its window,
        /// hidden until Visible is set: Text alone changes nothing you can see (verified by capturing
        /// the client window). The operating-system caption is the client's own (App.xaml.cs).
        /// </summary>
        public static void ApplyTitle(Technician tech)
        {
            // A browser tab has no title bar to set; the Hybrid client's window does.
            if (Device.Valid && !string.IsNullOrEmpty(tech?.Name))
            {
                Device.TitleBar.Text = "FieldOps — " + tech.Name;
                Device.TitleBar.Visible = true;
                OpsLog.Info($"support: session {Application.SessionId[..8]} title bar \"{Device.TitleBar.Text}\"");
            }
        }

        private static void SignIn_Completed(Technician tech)
        {
            if (!Device.Valid)
            {
                OpsLog.Info($"support: session {Application.SessionId[..8]} Device.Valid=False (browser or PWA)");
                return;
            }

            // The support log: which client, which install, which mode.
            OpsLog.Info(
                $"support: session {Application.SessionId[..8]} Device.Valid=True " +
                $"Platform={Device.System.Platform} Idiom={Device.System.Idiom} IsDesktop={Device.System.IsDesktop} " +
                $"PackagingModel={Device.System.PackagingModel} " +
                $"OfflineUrl={(string.IsNullOrEmpty(Device.System.OfflineUrl) ? "(not started)" : Device.System.OfflineUrl)} " +
                $"Mode={(AppMode.IsLocal ? "Local" : "Remote")} License={Device.License.LicenseType} " +
                $"Client={Device.Versioning.CurrentVersion} App={Device.System.AppName} OS={Device.System.Version}");

            // Changes made in the Local app while the network was down go to the server now.
            if (!AppMode.IsLocal)
            {
                try
                {
                    var outcome = DeviceSync.ReplayPending(WorkOrderService.Current, tech);
                    if (outcome != null) Application.Session["SyncOutcome"] = outcome;
                }
                catch (Exception ex)
                {
                    OpsLog.Error("replaying offline changes", ex);
                }
            }
        }
    }
}
