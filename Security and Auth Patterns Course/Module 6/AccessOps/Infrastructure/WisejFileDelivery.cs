using System.IO;
using AccessOps.Services;
using Wisej.Web;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// Streams a file to the current session with Application.Download: no path in the markup, no
    /// URL in the history, nothing to forward. It only runs inside an event the service already
    /// authorized.
    /// </summary>
    public sealed class WisejFileDelivery : IFileDelivery
    {
        public void Send(byte[] content, string fileName) => Application.Download(new MemoryStream(content), fileName);
    }
}
