using Foundation;
using Wisej.Hybrid.Native;

namespace FieldOps.Client
{
	[Register("AppDelegate")]
	public class AppDelegate : HybridUIApplicationDelegate
	{
		protected override MauiApp CreateMauiApp() => Startup.Main();
	}
}