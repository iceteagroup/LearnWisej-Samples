namespace FieldOps.Client
{
	/// <summary>
	/// Every environment's FieldOps address in one place. Debug points at the developer's machine
	/// (the Android emulator reaches it at 10.0.2.2, Windows at localhost), Release at production.
	/// FIELDOPS_URL overrides it without a rebuild (a Dev Tunnels URL gives every platform one HTTPS
	/// address). Changing a constant here is a client change: rebuild and, for Release, a store release.
	/// </summary>
	public static class FieldOpsServer
	{
		public const string Development =
#if ANDROID
			"http://10.0.2.2:6304/";
#else
			"http://localhost:6304/";
#endif

		public const string Staging = "https://staging.fieldops.example.com/";

		public const string Production = "https://fieldops.example.com/";

		public static string Url =>
			Environment.GetEnvironmentVariable("FIELDOPS_URL") ??
#if DEBUG
			Development;
#else
			Production;
#endif
	}
}
