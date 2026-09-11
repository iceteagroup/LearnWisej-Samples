using System;
using System.Globalization;
using TicketOps.Diagnostics;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The Infrastructure adapter over <c>Wisej.Web.Application</c>. Every member is read fresh on each call
    /// (Wisej updates ActiveProfile and Theme per request) and guarded, so a diagnostics refresh never throws
    /// because one runtime property was unavailable. Nothing is cached here: caching a per-session value in
    /// an object that might outlive the session is exactly the leak this module is about.
    /// </summary>
    public sealed class WisejRuntimeInfo : IRuntimeInfo
    {
        public int? ActiveSessionCount
        {
            get { try { return Application.SessionCount; } catch (Exception) { return null; } }
        }

        public int? ConfiguredSessionTimeoutSeconds
        {
            get { try { return Application.Configuration != null ? Application.Configuration.SessionTimeout : (int?)null; } catch (Exception) { return null; } }
        }

        public string ConfiguredThemeName
        {
            get { try { return Application.Configuration != null ? Application.Configuration.ThemeName : null; } catch (Exception) { return null; } }
        }

        public string ActiveThemeName
        {
            get { try { return Application.Theme != null ? Application.Theme.Name : null; } catch (Exception) { return null; } }
        }

        public string ActiveClientProfile
        {
            get
            {
                try
                {
                    string profile = Application.ActiveProfile != null ? Application.ActiveProfile.Name : "Default";
                    string device = Application.Browser != null ? Application.Browser.Device : null;
                    return string.IsNullOrEmpty(device) ? profile : $"{profile} · {device}";
                }
                catch (Exception) { return null; }
            }
        }

        public string Server
        {
            get
            {
                try { return $"{Application.ServerName}:{Application.ServerPort.ToString(CultureInfo.InvariantCulture)}"; }
                catch (Exception) { return null; }
            }
        }

        /// <summary>The values AppComposition copies into a brand-new SessionContext when a session starts.</summary>
        public static string ReadSessionId()
        {
            try { return Application.SessionId; } catch (Exception) { return Guid.NewGuid().ToString("N"); }
        }
    }
}
