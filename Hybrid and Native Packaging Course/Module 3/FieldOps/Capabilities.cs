using Wisej.Hybrid;

namespace FieldOps
{
    /// <summary>
    /// The capability gate every screen asks. Never the user agent: a phone browser and the
    /// Hybrid client's WebView look alike, only <see cref="Device.Valid"/> knows the client is there.
    ///
    /// Order matters. <c>Device.Valid</c> first ("is a Hybrid client present at all?"), then the
    /// feature flag ("can THIS client do it?"); the short-circuit means <c>Device.Media</c> is only
    /// consulted once a client is there. Every gated feature has a browser answer.
    /// </summary>
    public static class Capabilities
    {
        /// <summary>Camera capture through the Hybrid client; otherwise the Upload control.</summary>
        public static bool CanTakePhoto => Device.Valid && Device.Media.CaptureSupported;
    }
}
