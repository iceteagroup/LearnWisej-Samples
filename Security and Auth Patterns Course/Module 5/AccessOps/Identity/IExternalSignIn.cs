namespace AccessOps.Identity
{
    /// <summary>The outcome of the page-load half of an external sign-in.</summary>
    public enum ExternalCompletion
    {
        /// <summary>No hand-off cookie on this load: nothing to do.</summary>
        None,
        /// <summary>The identity was mapped, one grant was created and the principal set.</summary>
        SignedIn,
        /// <summary>A hand-off arrived and was refused (replayed, unknown, disabled, unapproved tenant).</summary>
        Rejected
    }

    /// <summary>
    /// The one seam the Wisej.NET pages use for partner federation. The sign-in button calls
    /// <see cref="Begin"/>; <c>Program.Main</c> and <c>Application.ApplicationRefresh</c> call
    /// <see cref="TryCompleteOnPageLoad"/>.
    /// </summary>
    public interface IExternalSignIn
    {
        /// <summary>The provider's display name ("Contoso Identity").</summary>
        string ProviderName { get; }

        /// <summary>Sends the tab to <c>/account/signin</c> (a top-level navigation; the challenge happens there).</summary>
        void Begin();

        /// <summary>Redeems a hand-off on this page load, if there is one.</summary>
        ExternalCompletion TryCompleteOnPageLoad();
    }
}
