using System.Collections.Specialized;
using System.Globalization;
using Wisej.Web;

namespace NorthwindDashboard
{
	internal static class SessionStartup
	{
		// Called by Wisej for each new session, as configured in Default.json.
		public static void Start(NameValueCollection args)
		{
			// Northwind amounts are US dollars; keep currency and dates consistent for every browser locale.
			Application.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

			Application.MainPage = new MainPage();
		}
	}
}
