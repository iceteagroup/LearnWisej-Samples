using System.Text;

namespace FieldOps.Client
{
	/// <summary>
	/// The client half of crash telemetry. Wisej.NET Hybrid ships no crash-reporting API, so this is
	/// where your team's service is initialised (its SDK call replaces Report below). What it already
	/// does: catch an unhandled native exception, keep a local copy in the app's data folder, and POST
	/// a short line (platform, client version and build, exception type and message - no user data)
	/// to the FieldOps server's /client-log, the same endpoint HybridConfig.LogUrl uses.
	/// </summary>
	internal static class CrashReporting
	{
		public static void Install()
		{
			AppDomain.CurrentDomain.UnhandledException += (s, e) =>
				Report(e.ExceptionObject as Exception, "AppDomain");
			TaskScheduler.UnobservedTaskException += (s, e) =>
			{
				Report(e.Exception, "Task");
				e.SetObserved();
			};
		}

		private static void Report(Exception ex, string source)
		{
			if (ex == null) return;
			var line = $"crash {source} {DeviceInfo.Current.Platform} {AppInfo.Current.VersionString} ({AppInfo.Current.BuildString}): {ex.GetType().Name}: {ex.Message}";
			try
			{
				File.AppendAllText(Path.Combine(FileSystem.AppDataDirectory, "crash.log"), DateTime.UtcNow.ToString("o") + " " + line + Environment.NewLine);
				using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
				var url = FieldOpsServer.Url.Split('?')[0].TrimEnd('/') + "/client-log";
				http.PostAsync(url, new StringContent(line, Encoding.UTF8, "text/plain")).Wait(3000);
			}
			catch
			{
				// The crash reporter must never crash.
			}
		}
	}
}
