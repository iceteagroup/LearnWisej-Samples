using Wisej.Hybrid.Native;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FieldOps.Client.WinUI
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public partial class App : HybridWinUIApplication
	{
		/// <summary>
		/// One FieldOps per laptop, if the depot wants it: build with
		/// -p:DefineConstants=FIELDOPS_SINGLE_INSTANCE. By default a Hybrid desktop app allows
		/// several instances, and Startup.cs gives each one its own embedded-server port
		/// (WisejExtensions.GetLocalUrl()), so the default is safe for the Local fallback.
		/// </summary>
#if FIELDOPS_SINGLE_INSTANCE
		private static System.Threading.Mutex _single;
#endif

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
#if FIELDOPS_SINGLE_INSTANCE
			_single = new System.Threading.Mutex(true, @"Local\Contoso.FieldOps", out bool first);
			if (!first)
			{
				// Another FieldOps is already running for this user: leave quietly.
				System.Environment.Exit(0);
			}
#endif
			InitializeComponent();
		}

		protected override MauiApp CreateMauiApp() => Startup.Main();
	}
}
