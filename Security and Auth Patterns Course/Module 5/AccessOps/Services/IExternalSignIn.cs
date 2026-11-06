using System;
using System.Threading.Tasks;

namespace AccessOps.Services
{
    public enum ExternalCompletion { None, SignedIn, Rejected }

    /// <summary>Who signed in through the provider, kept in the session next to the SessionContext.</summary>
    public sealed record ExternalSessionInfo(string Provider, string Issuer, string Subject, bool Provisioned);

    /// <summary>
    /// The seam between AccessOps' pages and the external sign-in mechanism. Pages call only this.
    /// Today it is implemented with ASP.NET Core's OpenID Connect handler (WisejExternalSignIn);
    /// replacing it with the Wisej.NET 4.2 OAuthClient component is a change behind this interface.
    /// </summary>
    public interface IExternalSignIn
    {
        /// <summary>The provider's display name ("Contoso Identity").</summary>
        string ProviderName { get; }

        /// <summary>Sends the browser to the provider (authorization code flow with PKCE).</summary>
        void Begin();

        /// <summary>
        /// On a page load: redeems a hand-off from the provider callback, maps it to an employee
        /// and establishes the SessionContext. None when there is nothing to redeem.
        /// </summary>
        ExternalCompletion TryCompleteOnPageLoad();

        /// <summary>The external sign-in of this session, or null for a password (service account) session.</summary>
        ExternalSessionInfo Current { get; }

        /// <summary>
        /// A valid access token for a downstream API, refreshed in the back channel when it is
        /// within 60 seconds of expiry. Null when it can no longer be renewed - the token has expired
        /// for good and the caller signs the session out (see DashboardView).
        /// </summary>
        Task<string> GetAccessTokenAsync();
    }
}
