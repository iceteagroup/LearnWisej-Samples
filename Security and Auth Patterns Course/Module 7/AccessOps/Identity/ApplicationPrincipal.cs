using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using Wisej.Web;

namespace AccessOps.Identity
{
    /// <summary>
    /// Establishes the Wisej.NET application principal (the session override of
    /// <c>Application.User</c>). Adapted from the course pack's <c>common/ApplicationPrincipal.cs</c>:
    /// as the Module 4 lesson says, this copy assigns <c>Application.User</c> and does not rotate -
    /// the page load rotates the session ID once, before the identity exists
    /// (<see cref="SignInHandoff.CompleteOnPageLoad"/>).
    /// </summary>
    public static class ApplicationPrincipal
    {
        /// <summary>The authentication type of every principal AccessOps establishes.</summary>
        public const string AuthenticationType = "Portal";

        /// <summary>The claim that carries the grant ID every protected operation checks.</summary>
        public const string GrantClaim = "grant";

        /// <summary>The claim that carries the tenant.</summary>
        public const string TenantClaim = "tenant";

        /// <summary>The claim that says how the account signed in ("password").</summary>
        public const string MethodClaim = "amr";

        /// <summary>The claim that says when the credential was verified (unix seconds).</summary>
        public const string AuthTimeClaim = "auth_time";

        /// <summary>
        /// Builds the claims principal. Invoke only after credential verification, account and
        /// tenant policy and grant creation.
        /// </summary>
        public static ClaimsPrincipal Build(VerifiedSignIn signIn)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, signIn.AccountId),
                new Claim(ClaimTypes.Name, signIn.DisplayName ?? string.Empty),
                new Claim(TenantClaim, signIn.TenantId ?? string.Empty),
                new Claim(GrantClaim, signIn.GrantId ?? string.Empty),
                new Claim(MethodClaim, signIn.Method ?? "password"),
                new Claim(AuthTimeClaim, signIn.VerifiedAt.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture))
            };

            foreach (var role in signIn.Roles ?? Array.Empty<string>())
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationType));
        }

        /// <summary>
        /// Sets <c>Application.User</c> for this Wisej.NET session. Session state: it issues no
        /// cookie and creates no Windows token.
        /// </summary>
        public static void Establish(VerifiedSignIn signIn)
        {
            Application.User = Build(signIn);
        }
    }
}
