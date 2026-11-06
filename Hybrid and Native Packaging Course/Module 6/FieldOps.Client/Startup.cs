using FieldOps.Offline;
using Microsoft.Extensions.Logging;
using Wisej.EmbedIO;
using Wisej.Hybrid.Native;

namespace FieldOps.Client
{
	public static class Startup
	{

		public static MauiApp Main()
		{
			// Crash reporting: plug your team's service in CrashReporting (no third-party SDK ships here).
			CrashReporting.Install();

			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

			// The Local app (FieldOps.Offline) on an embedded EmbedIO server inside this client.
			// Every desktop instance gets its own port for it.
			if (CanHostLocalApp())
				builder.UseWisejOffline<OfflineStartup>(WisejExtensions.GetLocalUrl());

			builder.UseWisejHybrid((config) =>
			{
				// Remote first...
				config.StartupUrl = FieldOpsServer.Url;   // dev, staging or production

				// The default, stated on purpose: every return to the foreground reloads FieldOps,
				// so the page restores the open order from the saved draft (Module 4).
				config.ReloadOnForeground = true;

				// ...and the Local app 5 s after the connection is lost (-1 disables the switch).
				config.OfflineTimeout = 5000;

				// DebugMode is left at its default: on in Debug builds (F12 opens the WebView2
				// developer tools on the live FieldOps page), off in Release.

				// Client POST logging (Module 6): the FieldOps server's /client-log endpoint.
				config.LogUrl = FieldOpsServer.Url.Split('?')[0].TrimEnd('/') + "/client-log";

				// The licence key comes from outside the repository (see the .csproj).
				if (!string.IsNullOrEmpty(HybridLicense.Key))
					config.LicenseKey = HybridLicense.Key;
			});

#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}

		/// <summary>
		/// On Windows the embedded server needs a package identity: Wisej's WithWisej() reads
		/// Windows.Storage.ApplicationData.Current, which throws in an unpackaged process and the
		/// window shows "The embedded application could not start". So the Local fallback is
		/// registered only for the MSIX-packaged client (F5 with the "Windows Machine" profile, or an
		/// installed MSIX). An unpackaged run is Remote only - the server's support log shows it as
		/// PackagingModel=Unpackaged, OfflineUrl=(not started).
		/// </summary>
		private static bool CanHostLocalApp()
		{
#if WINDOWS
			try
			{
				return Windows.ApplicationModel.Package.Current != null;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
#else
			return true;
#endif
		}
	}
}
