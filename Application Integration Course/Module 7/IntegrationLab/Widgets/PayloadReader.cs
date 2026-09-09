using System;
using System.Globalization;

namespace IntegrationLab.Widgets
{
    /// <summary>
    /// Small helpers that read ONE field out of an untrusted dynamic payload.
    /// Every read is guarded: a missing member, a null, the wrong type or a non-finite
    /// number all come back as "false" so the caller can reject the payload with a reason.
    /// </summary>
    internal static class PayloadReader
    {
        /// <summary>Runs a dynamic member read and swallows binder/null failures.</summary>
        public static object Get(Func<object> read)
        {
            try { return read(); }
            catch { return null; }
        }

        public static bool TryDouble(object raw, out double value)
        {
            value = double.NaN;
            if (raw == null || raw is bool || raw is string) return false;
            try { value = Convert.ToDouble(raw, CultureInfo.InvariantCulture); }
            catch { return false; }
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        public static bool TryInt(object raw, out int value)
        {
            value = 0;
            if (!TryDouble(raw, out double d)) return false;
            if (d != Math.Floor(d) || d < int.MinValue || d > int.MaxValue) return false;
            value = (int)d;
            return true;
        }

        public static bool TryString(object raw, out string value)
        {
            value = raw as string;
            return value != null;
        }

        /// <summary>Compact JSON rendering of a payload for the trace (never used for logic).</summary>
        public static string ToJson(object data)
        {
            if (data == null) return "null";
            try
            {
                if (data is Wisej.Core.DynamicObject d) return d.ToJSON(false);
                return data.ToString();
            }
            catch { return data.ToString(); }
        }

        public static string F(double value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
