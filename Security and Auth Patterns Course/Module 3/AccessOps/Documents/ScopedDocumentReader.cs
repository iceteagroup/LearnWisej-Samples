using System;
using System.IO;
using System.Runtime.Versioning;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;

namespace AccessOps.Documents
{
    /// <summary>
    /// Reads one approved confidential file under the request's Windows token, in one bounded
    /// scope. Built from the course pack's <c>net8/WindowsScopedRead.cs</c>: require a
    /// <see cref="WindowsIdentity"/> from the request principal, then
    /// <c>WindowsIdentity.RunImpersonated(identity.AccessToken, () =&gt; File.ReadAllBytes(path))</c>.
    /// It receives a document ID, never a path: <see cref="DocumentCatalog"/> decides first.
    /// <c>RunImpersonated</c> restores the previous token when the delegate returns or throws, so
    /// the "after" row of the <see cref="IdentityTrace"/> is the process (pool) identity again
    /// even after the F03 <see cref="FileNotFoundException"/>, which escapes to the caller.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class ScopedDocumentReader
    {
        private readonly DocumentCatalog _catalog;
        private readonly IdentityTrace _trace;

        /// <summary>Creates the reader for one read.</summary>
        public ScopedDocumentReader(DocumentCatalog catalog, IdentityTrace trace)
        {
            _catalog = catalog;
            _trace = trace;
        }

        /// <summary>
        /// Run where the request token is live: <paramref name="context"/> is the current request
        /// (here the WebSocket request, through <c>IHttpContextAccessor</c>); its host owns the
        /// token's lifetime. The token is never stored.
        /// </summary>
        public byte[] Read(HttpContext context, string documentId)
        {
            if (context?.User?.Identity is not WindowsIdentity identity || !identity.IsAuthenticated)
                throw new UnauthorizedAccessException("No Windows identity on this request.");

            _trace.SetRequestIdentity(identity);

            // policy first: grant, tenant and record, then a canonical path under the root
            string path;
            try
            {
                path = _catalog.ResolveApprovedPath(documentId);
            }
            catch (AccessDeniedException denied)
            {
                using (var current = WindowsIdentity.GetCurrent())
                    _trace.Policy(current, denied.Reason);
                throw;
            }

            using (var before = WindowsIdentity.GetCurrent())
                _trace.Before(before, ProbeAsProcess(path));

            var failed = false;
            try
            {
                return WindowsIdentity.RunImpersonated(identity.AccessToken, () =>
                {
                    using var inside = WindowsIdentity.GetCurrent();
                    try
                    {
                        var bytes = File.ReadAllBytes(path);
                        _trace.Inside(inside, bytes.Length, null);
                        return bytes;
                    }
                    catch (Exception ex)
                    {
                        _trace.Inside(inside, 0, ex);
                        throw;   // escapes the scope; RunImpersonated reverts first
                    }
                });
            }
            catch
            {
                failed = true;
                throw;
            }
            finally
            {
                using (var after = WindowsIdentity.GetCurrent())
                    _trace.After(after, failed);
            }
        }

        /// <summary>
        /// LAB BASELINE for the "before" row: can the process (pool) token open the approved file?
        /// It opens and closes the file without reading content, so the ACL outcome outside the
        /// scope is evidence, not a second read path.
        /// </summary>
        private static Exception ProbeAsProcess(string path)
        {
            try
            {
                using (new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    return null;
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or FileNotFoundException or DirectoryNotFoundException)
            {
                return ex;
            }
        }
    }
}
