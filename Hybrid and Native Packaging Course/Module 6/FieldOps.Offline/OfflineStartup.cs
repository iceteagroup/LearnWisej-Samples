using EmbedIO;
using Wisej.EmbedIO;
using System.Threading;

namespace FieldOps.Offline;

/// <summary>
/// Starts the embedded web server inside the Hybrid client and returns its URL. The client
/// registers this class with UseWisejOffline&lt;OfflineStartup&gt;(localUrl) and opens the URL
/// once it has been without internet for config.OfflineTimeout milliseconds.
///
/// Windows: WithWisej() resolves its web root from Windows.Storage.ApplicationData.Current, which
/// exists only for a PACKAGED app (MSIX identity). In an unpackaged client it throws
/// InvalidOperationException ("Operation is not valid due to the current state of the object"),
/// so FieldOps.Client registers this class only when it has a package identity (Startup.cs).
/// </summary>
public class OfflineStartup
{
	public static string Main(CancellationToken token, object[] args)
	{
		// The client passes WisejExtensions.GetLocalUrl(): a free port per desktop instance, so two
		// FieldOps windows on one laptop do not fight over localhost:5000.
		var url = args != null && args.Length > 0 && args[0] is string local && local.Length > 0
			? local
			: "http://localhost:5000";

		// The embedded server runs inside the package, where the machine's developer licence is out
		// of reach: hand Wisej.NET the framework licence explicitly (see WisejLicense.props.example).
		if (!string.IsNullOrEmpty(WisejLicense.Key))
			Wisej.Web.Application.LicenseKey = WisejLicense.Key;

		var server = new WebServer(url);   // EmbedIO
		server.WithWisej();                // Wisej.EmbedIO: the FieldOps Wisej.NET app, on the device
		server.Run(token);

		return url;
	}
}
