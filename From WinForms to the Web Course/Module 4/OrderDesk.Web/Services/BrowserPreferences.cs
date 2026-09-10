using System.Threading.Tasks;
using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>
    /// ✓ Device-bound UI preferences in the browser's localStorage, through Application.Eval /
    /// EvalAsync. The third destination of the lesson: right for things like grid density or a
    /// collapsed panel that may legitimately differ per device, wrong for anything a business
    /// process depends on (the server never sees it unless it asks, another device starts empty,
    /// and the user can clear it). Use it only if that is operationally acceptable.
    /// </summary>
    public static class BrowserPreferences
    {
        public const string DensityKey = "orderdesk.density";

        /// <summary>The JavaScript that stores the value — shown in the trace so the round trip is visible.</summary>
        public static string SaveDensityScript(string density) => $"localStorage.setItem('{DensityKey}', '{Escape(density)}')";

        /// <summary>The JavaScript expression that reads it back (an expression, never a return statement).</summary>
        public static string ReadDensityScript => $"localStorage.getItem('{DensityKey}')";

        public static void SaveDensity(string density) => Application.Eval(SaveDensityScript(density));

        /// <summary>Null when the key was never written in this browser profile.</summary>
        public static async Task<string> ReadDensityAsync()
        {
            object value = await Application.EvalAsync(ReadDensityScript);
            return value == null ? null : value.ToString();
        }

        public static void ClearDensity() => Application.Eval($"localStorage.removeItem('{DensityKey}')");

        private static string Escape(string value) => (value ?? "").Replace("\\", "\\\\").Replace("'", "\\'");
    }
}
