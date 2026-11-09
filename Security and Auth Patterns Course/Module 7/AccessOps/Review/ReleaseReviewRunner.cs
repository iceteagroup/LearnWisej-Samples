#if DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AccessOps.Documents;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using AccessOps.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wisej.Ext.WebAuthn;

namespace AccessOps.Review
{
    /// <summary>
    /// What the review screen captured on the Wisej.NET thread before the first <c>await</c>: the
    /// session's application principal and the current request (on the intranet route the
    /// WebSocket request, whose Windows token is live while the socket is open).
    /// </summary>
    public sealed class ReviewContext
    {
        /// <summary><c>Application.User</c> of the review session.</summary>
        public IPrincipal SessionPrincipal { get; init; }

        /// <summary><c>IHttpContextAccessor.HttpContext</c> of the click.</summary>
        public HttpContext Request { get; init; }
    }

    /// <summary>
    /// DEBUG ONLY - the AccessOps release review (Module 7 capstone). Runs the 12 cases of the
    /// capstone brief against the real services, the shared store and the running routes, and
    /// fills one <see cref="EvidenceCase"/> per case. Every case runs inside its own
    /// <see cref="Correlation"/> scope (<c>ILogger.BeginScope</c>), so the audit rows it causes carry
    /// the label shown in the matrix. A case never fakes a Live result: what needs IIS, the domain,
    /// a live provider or a real authenticator is labelled Fixture / Harness / Simulated with the
    /// exact rerun plan. Failures go to the server log and the audit table; the screen gets one plain
    /// sentence.
    /// </summary>
    public sealed class ReleaseReviewRunner
    {
        // Debug-only demo credential of the svc.reports fixture (invented; Release builds seed no password).
        private const string ServiceAccount = "svc.reports";
        private const string ServicePassword = "Reports-demo-31";

        private readonly IConfiguration _configuration;
        private readonly AccessOpsDatabase _db;
        private readonly IClock _clock;
        private readonly RecordPolicy _policy;
        private readonly CountingDocumentRepository _repository;
        private readonly GrantStore _grants;
        private readonly AccountStore _accounts;
        private readonly LoginService _login;
        private readonly SignOutService _signOut;
        private readonly ExportService _export;
        private readonly ExternalHandoffStore _handoffs;
        private readonly WisejExternalSignIn _external;
        private readonly PasskeyService _passkeys;
        private readonly PasskeyCredentialStore _credentials;
        private readonly ConfidentialFileStore _files;
        private readonly IdentityTraceLog _traceLog;
        private readonly AuditService _audit;
        private readonly ILoggerFactory _loggers;
        private readonly ILogger _log;

        /// <summary>Creates the runner from the process's services.</summary>
        public ReleaseReviewRunner(IServiceProvider services)
        {
            T Get<T>() => (T)services.GetService(typeof(T));
            _configuration = Get<IConfiguration>();
            _db = Get<AccessOpsDatabase>();
            _clock = Get<IClock>();
            _policy = Get<RecordPolicy>();
            _repository = Get<CountingDocumentRepository>();
            _grants = Get<GrantStore>();
            _accounts = Get<AccountStore>();
            _login = Get<LoginService>();
            _signOut = Get<SignOutService>();
            _export = Get<ExportService>();
            _handoffs = Get<ExternalHandoffStore>();
            _external = Get<WisejExternalSignIn>();
            _passkeys = Get<PasskeyService>();
            _credentials = Get<PasskeyCredentialStore>();
            _files = Get<ConfidentialFileStore>();
            _traceLog = Get<IdentityTraceLog>();
            _audit = Get<AuditService>();
            _loggers = Get<ILoggerFactory>();
            _log = _loggers.CreateLogger("AccessOps.ReleaseReview");
        }

        /// <summary>The intranet route's base URL (Kestrel Negotiate here, IIS in the lab).</summary>
        public string IntranetUrl => (_configuration["AccessOps:ReleaseReview:IntranetUrl"] ?? "http://localhost:6408").TrimEnd('/');

        /// <summary>The partner route's base URL the HTTP cases call.</summary>
        public string PartnerUrl => (_configuration["AccessOps:ReleaseReview:PartnerUrl"] ?? "https://localhost:6407").TrimEnd('/');

        /// <summary>Grants the run issued through callbacks (cases 9 and 12): the rollback revokes them.</summary>
        public List<string> CallbackGrants { get; } = new List<string>();

        /// <summary>Opaque values the run handled (handles, credential IDs, user handles, grant IDs): the log scan looks for them.</summary>
        private readonly List<string> _secrets = new List<string>();

        /// <summary>The 12 cases, before a run.</summary>
        public static List<EvidenceCase> Definitions() => new List<EvidenceCase>
        {
            new EvidenceCase { Number = 1, Title = "Intranet Reader reads a permitted record", Route = "Intranet", Fixture = "F01", ExpectedOutcome = "Allow", Label = EvidenceLabel.Fixture,
                Expected = "401 with WWW-Authenticate Negotiate, then 200; the Reader reads A / 17 (one repository call)",
                RerunPlan = "Publish the intranet route to IIS with iis/windows-auth.location.xml on the canonical host; from a domain client sign in as LAB\\reader1, record 401 Negotiate -> 200 in the challenge trace and klist showing an HTTP/<host> ticket (provider = Kerberos only with that ticket), then open A / 17." },
            new EvidenceCase { Number = 2, Title = "Host-authenticated user without Reader", Route = "Intranet", Fixture = "—", ExpectedOutcome = "Deny", Label = EvidenceLabel.Harness,
                Expected = "Authenticated by the host, mapped to visitor1, no read right: deny at the policy, 0 repository calls",
                RerunPlan = "Sign in on the IIS intranet route as a second domain account outside the Reader group (LAB\\visitor1) and open A / 17: expect \"Access denied.\" and audit policy:role with 0 repository calls. Locally: start the intranet route with --AccessOps:CurrentWindowsUser=visitor1." },
            new EvidenceCase { Number = 3, Title = "Tenant A requests tenant B record 17", Route = "Partner", Fixture = "F02", ExpectedOutcome = "Deny", Label = EvidenceLabel.Live,
                Expected = "Tenant A Reader asks for B / 17: deny at the policy before the repository (0 calls)" },
            new EvidenceCase { Number = 4, Title = "Scoped read, forced error, token reverts", Route = "Intranet", Fixture = "F03", ExpectedOutcome = "Reverted", Label = EvidenceLabel.Fixture,
                Expected = "DOC-203 (approved, file missing) fails inside RunImpersonated; the effective token after equals the token before",
                RerunPlan = "On the IIS intranet route with tools/Setup-Fixtures.ps1 -PoolAccount \"IIS APPPOOL\\AccessOps Intranet\" -LabUser LAB\\dana.ortiz: DOC-201 denied before / allowed inside, DOC-203 FileNotFoundException inside, after = pool identity. Here pool and user are one account (NTLM loopback token)." },
            new EvidenceCase { Number = 5, Title = "Local share works, remote share denied", Route = "Intranet", Fixture = "F04", ExpectedOutcome = "Deny", Label = EvidenceLabel.Fixture,
                Expected = "DOC-201 (local) reads inside the scope; DOC-205 on \\\\files.contoso.test is refused before any scope (no approved delegation target)",
                RerunPlan = "On the domain: compare the service ticket, the allowed SPN (setspn -Q cifs/files.contoso.test) and the share ACL for DOC-205 with constrained delegation off and on; sign-off needs the decision in docs/route-matrix.md (restricted service identity, no delegation) to hold." },
            new EvidenceCase { Number = 6, Title = "Sign-in rotates the session ID", Route = "Partner", Fixture = "—", ExpectedOutcome = "Rotated", Label = EvidenceLabel.Live,
                Expected = "The last partner sign-in rotated the Wisej.NET session ID on its page load (audit session.rotate = rotated)" },
            new EvidenceCase { Number = 7, Title = "Revoked grant from a stale tab", Route = "Partner", Fixture = "F05", ExpectedOutcome = "Deny", Label = EvidenceLabel.Live,
                Expected = "Tab 2 signs out (grants revoked); tab 1's principal exports A / 17: deny, 0 repository calls, no export, no new grant" },
            new EvidenceCase { Number = 8, Title = "External return URL and forged post", Route = "Partner", Fixture = "F11", ExpectedOutcome = "Deny", Label = EvidenceLabel.Live,
                Expected = "returnUrl=https://evil.example -> \"/\"; POST /auth/cookie without antiforgery -> 400; WebSocket upgrade from a foreign Origin -> 403; no state change" },
            new EvidenceCase { Number = 9, Title = "Provider callback repeated", Route = "Partner", Fixture = "F07", ExpectedOutcome = "One grant", Label = EvidenceLabel.Fixture,
                Expected = "The validated callback's hand-off redeems once: one grant; the repeat is refused and issues none",
                RerunPlan = "Against the real provider: sign in through Contoso Identity, copy the /signin-oidc callback URL, replay it in the same browser: the handler refuses it (correlation) and the grant count stays +1. Then repeat with the shared-store hand-off on two nodes." },
            new EvidenceCase { Number = 10, Title = "Unapproved tenant or bad state", Route = "Partner", Fixture = "F06 F08", ExpectedOutcome = "Deny", Label = EvidenceLabel.Fixture,
                Expected = "A callback with a forged state fails in the handler (signin=failed); tenant C's mapped identity is refused; no principal, no grant",
                RerunPlan = "Against the real provider: start a sign-in in browser A and complete its callback in browser B (state from another attempt, F06); sign in as a user of an unapproved tenant (F08). Both must end on the fixed sentence with 0 new grants." },
            new EvidenceCase { Number = 11, Title = "Passkey replay, wrong owner, deleted", Route = "Partner", Fixture = "F09 F10", ExpectedOutcome = "Deny", Label = EvidenceLabel.Harness,
                Expected = "A valid assertion signs in once; the same response replayed, account A's credential for account B, and a deleted credential are refused with no commit and no grant",
                RerunPlan = "With Windows Hello or a security key on https://localhost:6407 (trusted dev certificate): enroll, sign in, replay the captured response, present the credential with another account's user handle, delete it on My passkeys and try again. Software authenticator here." },
            new EvidenceCase { Number = 12, Title = "Callback after node loss", Route = "Partner", Fixture = "F12", ExpectedOutcome = "Safe deny", Label = EvidenceLabel.Simulated,
                Expected = "A callback already consumed on node A and retried on node B is refused (no grant); a parked one resumes once on node B",
                RerunPlan = "Two partner nodes behind the balancer with the shared key ring and store: start a sign-in on node A, stop node A at the provider's login form, complete it so the callback lands on node B; then replay the callback. Recorded here by restarting the partner process mid-flow (docs/runbook.md)." },
        };

        /// <summary>Runs case <paramref name="c"/> and fills it in. Never throws: a failure becomes a plain "Failed" row.</summary>
        public async Task RunAsync(EvidenceCase c, ReviewContext context)
        {
            c.Correlation = Correlation.NewLabel($"rr-{c.Number:00}");
            using var scope = Correlation.Begin(_log, c.Correlation);
            var readsBefore = _repository.ReadCount;
            try
            {
                switch (c.Number)
                {
                    case 1: await Case1(c, context); break;
                    case 2: Case2(c); break;
                    case 3: Case3(c); break;
                    case 4: Case4(c, context); break;
                    case 5: Case5(c, context); break;
                    case 6: Case6(c); break;
                    case 7: Case7(c); break;
                    case 8: await Case8(c); break;
                    case 9: Case9(c); break;
                    case 10: await Case10(c); break;
                    case 11: Case11(c); break;
                    case 12: Case12(c); break;
                }
                if (c.Number is not (4 or 5))
                    c.RepositoryCalls = _repository.ReadCount - readsBefore;
                c.Passed = c.Outcome == c.ExpectedOutcome && (!c.IsDeny || c.RepositoryCalls == 0) && c.StopCondition == null;
            }
            catch (Exception ex)
            {
                // details to the server log only; the row and the alert get a plain sentence
                _log.LogError(ex, "release review case {Case} failed. correlation={Correlation}", c.Number, c.Correlation);
                c.Outcome = "Failed";
                c.Actual = "The check could not be completed. The details are in the server log under " + c.Correlation + ".";
                c.Passed = false;
            }
            finally
            {
                c.Done = true;
                c.Running = false;
                // the evidence row as text (no handles, tokens, passwords or SIDs are ever in it)
                _log.LogInformation("evidence case={Case} title=\"{Title}\" route={Route} fixture={Fixture} alias=\"{Alias}\" entry=\"{Entry}\" expected=\"{Expected}\" actual=\"{Actual}\" outcome={Outcome} boundary={Boundary} repositoryCalls={Calls} label={Label} passed={Passed} stop={Stop}",
                    c.Number, c.Title, c.Route, c.Fixture, c.Alias, c.EntryPoint, c.Expected, c.Actual, c.Outcome, c.Boundary, c.RepositoryCalls, c.Label, c.Passed, c.StopCondition ?? "-");
                _audit.Write(new AuditEvent("review", "review.case", "case " + c.Number, "-",
                    c.Passed ? "passed" : "failed", (c.Outcome ?? "-").ToLowerInvariant().Replace(' ', '-') + "@" + (string.IsNullOrEmpty(c.Boundary) ? "none" : c.Boundary.ToLowerInvariant().Replace(' ', '-')), c.Correlation));
            }
        }

        /// <summary>
        /// The fourth stop condition: credential material in the audit store. Looks for every full
        /// handle, credential ID, user handle and grant ID the run handled.
        /// </summary>
        public bool AuditHoldsCredentialMaterial()
        {
            var rows = _audit.AllText().ToList();
            return _secrets.Where(s => !string.IsNullOrEmpty(s) && s.Length >= 16).Any(s => rows.Any(r => r.Contains(s, StringComparison.Ordinal)));
        }

        // ---- the cases -------------------------------------------------------------------------

        private async Task Case1(EvidenceCase c, ReviewContext context)
        {
            c.Alias = "reader1 (Windows account mapped by SID)";
            c.EntryPoint = "intranet route · " + IntranetUrl;
            c.Boundary = "Ingress";

            using var anonymous = new HttpClient(new SocketsHttpHandler { AllowAutoRedirect = false, UseCookies = false }) { Timeout = TimeSpan.FromSeconds(10) };
            using var first = await anonymous.GetAsync(IntranetUrl + "/Default.html");
            var offered = string.Join(", ", first.Headers.WwwAuthenticate.Select(h => h.Scheme));
            using var negotiated = new HttpClient(new SocketsHttpHandler { AllowAutoRedirect = false, UseCookies = false, Credentials = CredentialCache.DefaultCredentials }) { Timeout = TimeSpan.FromSeconds(10) };
            using var second = await negotiated.GetAsync(IntranetUrl + "/Default.html");
            var provider = context.Request?.User?.Identity is WindowsIdentity w ? w.AuthenticationType : "(not on the intranet route)";

            var result = _policy.Read(context.SessionPrincipal, "A", 17);
            var challenged = (int)first.StatusCode == 401 && offered.Contains("Negotiate") && second.StatusCode == HttpStatusCode.OK;
            c.Actual = $"{(int)first.StatusCode} {offered} -> {(int)second.StatusCode} (provider {provider}, Kestrel); A / 17 {(result.IsAllowed ? "read" : "refused: " + result.Reason)}";
            c.Outcome = challenged && result.IsAllowed ? "Allow" : "Deny";
            if (!result.IsAllowed)
                c.Boundary = "Policy";
        }

        private void Case2(EvidenceCase c)
        {
            var visitor = _accounts.FindByUserName("visitor1");
            // what AccessGrants.FindForAccount issues for a mapped account without the Reader role
            var grant = _grants.Issue(visitor.Id, visitor.Tenant, visitor.Role, canRead: false);
            var principal = ApplicationPrincipal.Build(new VerifiedSignIn
            {
                AccountId = visitor.Id, TenantId = visitor.Tenant, GrantId = grant.Id, Roles = new[] { visitor.Role },
                DisplayName = visitor.DisplayName, Method = "windows", VerifiedAt = _clock.Now
            });
            _secrets.Add(grant.Id);

            var result = _policy.Read(principal, "A", 17);
            _grants.Revoke(grant.Id);
            c.Alias = "visitor1 (host-authenticated, no Reader)";
            c.EntryPoint = "intranet principal, built in-process";
            c.Boundary = "Policy";
            c.Actual = result.IsAllowed ? "A / 17 was read" : $"A / 17 refused: policy:{result.Reason}";
            c.Outcome = result.IsAllowed ? "Allow" : "Deny";
        }

        private void Case3(EvidenceCase c)
        {
            var principal = PartnerSignIn(out var signIn);
            var result = _policy.Read(principal, "B", 17);
            _signOut.SignOut(Subject.From(principal), "review");
            c.Alias = "svc.reports · tenant A";
            c.EntryPoint = "partner sign-in (LoginService) · RecordPolicy.Read";
            c.Boundary = "Policy";
            c.Actual = result.IsAllowed ? "B / 17 was read" : $"B / 17 refused: policy:{result.Reason}";
            c.Outcome = result.IsAllowed ? "Allow" : "Deny";
            if (result.IsAllowed)
                c.StopCondition = "A cross-tenant read";
        }

        private void Case4(EvidenceCase c, ReviewContext context)
        {
            c.Alias = "reader1 (request Windows token)";
            c.EntryPoint = "intranet route · ScopedDocumentReader";
            c.Boundary = "OS token";
            if (!OperatingSystem.IsWindows() || context.Request?.User?.Identity is not WindowsIdentity)
            {
                c.Outcome = "Failed";
                c.Actual = "Run the review on the intranet route: this request carries no Windows identity.";
                return;
            }

            var subject = Subject.From(context.SessionPrincipal);
            var trace = new IdentityTrace("DOC-203", subject, _traceLog);
            var reader = new ScopedDocumentReader(new DocumentCatalog(subject, _files, _policy), trace);
            string before, after;
            using (var b = WindowsIdentity.GetCurrent()) before = b.Name + "/" + b.ImpersonationLevel;
            Exception failure = null;
            try
            {
                reader.Read(context.Request, "DOC-203");
            }
            catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
            {
                failure = ex;
            }
            using (var a = WindowsIdentity.GetCurrent()) after = a.Name + "/" + a.ImpersonationLevel;

            var inside = trace.Rows.FirstOrDefault(r => r.Step == "Inside");
            c.RepositoryCalls = trace.ScopeOpened ? 1 : 0;
            var reverted = failure != null && string.Equals(before, after, StringComparison.Ordinal);
            var insideLevel = inside == null ? "no scope" : inside.EffectiveToken.Contains("Impersonation") ? "request token (Impersonation)" : "process token";
            c.Actual = $"inside: {insideLevel} · {failure?.GetType().Name ?? "no error"}; after = before (name and level): {(reverted ? "yes" : "NO")}";
            c.Outcome = reverted ? "Reverted" : "Failed";
        }

        private void Case5(EvidenceCase c, ReviewContext context)
        {
            c.Alias = "reader1 (request Windows token)";
            c.EntryPoint = "intranet route · DocumentCatalog";
            c.Boundary = "Delegation";
            var subject = Subject.From(context.SessionPrincipal);
            var delegation = _configuration.GetSection("AccessOps:DelegationTargets").Get<string[]>();

            // the local share, inside the scope
            var local = "not run (no Windows request identity)";
            if (OperatingSystem.IsWindows() && context.Request?.User?.Identity is WindowsIdentity)
            {
                try
                {
                    var bytes = new ScopedDocumentReader(new DocumentCatalog(subject, _files, _policy, delegation), new IdentityTrace("DOC-201", subject, _traceLog))
                        .Read(context.Request, "DOC-201");
                    local = $"DOC-201 read inside the scope ({bytes.Length} bytes)";
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    local = "DOC-201 local read failed (" + ex.GetType().Name + "; run tools/Setup-Fixtures.ps1)";
                }
            }

            // the remote share: the catalog refuses the second hop before any scope or file call
            var remote = "DOC-205 opened";
            var calls = 0;
            try
            {
                new DocumentCatalog(subject, _files, _policy, delegation).ResolveApprovedPath("DOC-205");
                calls = 1;
            }
            catch (AccessDeniedException denied)
            {
                remote = "DOC-205 refused before any scope: " + denied.Reason;
            }

            c.RepositoryCalls = calls;
            c.Actual = local + "; " + remote;
            c.Outcome = calls == 0 ? "Deny" : "Allow";
        }

        private void Case6(EvidenceCase c)
        {
            c.Alias = "the last partner sign-in";
            c.EntryPoint = "partner route · page load (SignInHandoff)";
            c.Boundary = "Session";
            var evidence = _audit.Latest("session.rotate", _clock.Now.AddHours(-12));
            if (evidence == null)
            {
                c.Outcome = "Failed";
                c.Actual = "No partner sign-in in the last 12 hours. Sign in on the partner route, then run the review again.";
                return;
            }
            c.Actual = $"{evidence.Decision} at {evidence.Time} ({evidence.Reason} sign-in, audit {evidence.Correlation}); neither ID is stored";
            c.Outcome = evidence.Decision == "rotated" ? "Rotated" : "Failed";
        }

        private void Case7(EvidenceCase c)
        {
            var staleTab = PartnerSignIn(out _);                       // tab 1 keeps this principal
            var revoked = _signOut.SignOut(Subject.From(staleTab), "review");   // tab 2 signs out
            var grantsBefore = _grants.CountAll();
            var result = _export.Export(staleTab, "A", 17);            // tab 1 clicks Export
            var newGrants = _grants.CountAll() - grantsBefore;
            c.Alias = "svc.reports · two tabs";
            c.EntryPoint = "partner route · ExportService (stale principal)";
            c.Boundary = "Policy";
            c.Actual = result.IsAllowed
                ? "the stale tab exported A / 17"
                : $"sign-out revoked {revoked} grant(s); export refused: policy:{result.Reason}; new grants {newGrants}; no file";
            c.Outcome = result.IsAllowed ? "Allow" : "Deny";
            if (result.IsAllowed)
                c.StopCondition = "A deleted credential or revoked grant still works";
        }

        private async Task Case8(EvidenceCase c)
        {
            c.Alias = "a page on https://evil.example";
            c.EntryPoint = "partner route · " + PartnerUrl;
            c.Boundary = "Origin";
            var grantsBefore = _grants.CountAll();
            using var http = PartnerClient();

            using var redirect = await http.GetAsync(PartnerUrl + "/auth/continue?returnUrl=" + Uri.EscapeDataString("https://evil.example/steal"));
            var location = redirect.Headers.Location?.OriginalString ?? "(none)";

            using var post = new HttpRequestMessage(HttpMethod.Post, PartnerUrl + "/auth/cookie")
            {
                Content = new StringContent("{\"handle\":\"forged\"}", Encoding.UTF8, "application/json")
            };
            post.Headers.TryAddWithoutValidation("Origin", "https://evil.example");
            using var forged = await http.SendAsync(post);

            using var upgrade = new HttpRequestMessage(HttpMethod.Get, PartnerUrl + "/");
            upgrade.Headers.TryAddWithoutValidation("Origin", "https://evil.example");
            upgrade.Headers.TryAddWithoutValidation("Connection", "Upgrade");
            upgrade.Headers.TryAddWithoutValidation("Upgrade", "websocket");
            upgrade.Headers.TryAddWithoutValidation("Sec-WebSocket-Version", "13");
            upgrade.Headers.TryAddWithoutValidation("Sec-WebSocket-Key", Convert.ToBase64String(Guid.NewGuid().ToByteArray()));
            using var socket = await http.SendAsync(upgrade);

            var newGrants = _grants.CountAll() - grantsBefore;
            var refused = location == "/" && forged.StatusCode == HttpStatusCode.BadRequest && socket.StatusCode == HttpStatusCode.Forbidden && newGrants == 0;
            c.Actual = $"returnUrl -> \"{location}\"; forged POST -> {(int)forged.StatusCode}; foreign-origin upgrade -> {(int)socket.StatusCode}; new grants {newGrants}";
            c.Outcome = refused ? "Deny" : "Allow";
        }

        private void Case9(EvidenceCase c)
        {
            var grantsBefore = _grants.CountAll();
            var handle = _handoffs.Park(new ExternalIdentity(Issuer, "4f21-9ac0-dana-ellis"));   // what OnTokenValidated parks
            _secrets.Add(handle);
            var first = _external.Complete(handle, "a-review");
            var second = _external.Complete(handle, "a-review");                                   // the same callback again
            if (first.Succeeded) { CallbackGrants.Add(first.SignIn.GrantId); _secrets.Add(first.SignIn.GrantId); }
            if (second.Succeeded) { CallbackGrants.Add(second.SignIn.GrantId); }
            var issued = _grants.CountAll() - grantsBefore;
            c.Alias = "Dana Ellis (Contoso Identity fixture)";
            c.EntryPoint = "partner route · hand-off (WisejExternalSignIn.Complete)";
            c.Boundary = "Hand-off";
            c.Actual = $"first: {(first.Succeeded ? "1 grant" : first.Reason)}; repeat: {(second.Succeeded ? "ANOTHER grant" : second.Reason)}; grants +{issued}";
            c.Outcome = issued == 1 && !second.Succeeded ? "One grant" : "Failed";
            if (second.Succeeded)
                c.StopCondition = "A replay accepted as a fresh sign-in";
        }

        private async Task Case10(EvidenceCase c)
        {
            var grantsBefore = _grants.CountAll();
            using var http = PartnerClient();
            using var callback = await http.GetAsync(PartnerUrl + "/signin-oidc?code=forged-code&state=forged-state");
            var location = callback.Headers.Location?.OriginalString ?? "(none)";

            var kai = _external.Complete(_handoffs.Park(new ExternalIdentity(Issuer, "c77d-21b4-kai-berg")), "a-review");
            var issued = _grants.CountAll() - grantsBefore;
            c.Alias = "Kai Berg (tenant C) · a forged callback";
            c.EntryPoint = "partner route · /signin-oidc + hand-off";
            c.Boundary = "Callback";
            c.Actual = $"forged state -> {(int)callback.StatusCode} {location}; tenant C identity -> {(kai.Succeeded ? "GRANT" : kai.Reason)}; grants +{issued}";
            c.Outcome = location.Contains("signin=failed") && !kai.Succeeded && issued == 0 ? "Deny" : "Allow";
        }

        private void Case11(EvidenceCase c)
        {
            var rp = _passkeys.RelyingParty;
            var origin = rp.AllowedOrigins.First();
            var dana = AccessOpsDatabase.DanaAccountId;
            using var key = new SyntheticAuthenticator();
            var credential = new PasskeyCredential
            {
                RowId = Guid.NewGuid().ToString("N"), RpId = rp.RpId, CredentialId = key.CredentialId, AccountId = dana,
                UserHandle = _credentials.UserHandleFor(dana), PublicKeyJson = key.PublicKeyJson,
                Algorithm = (int)key.Algorithm, SignCount = 0, Label = "Release review · software key"
            };
            _credentials.Add(credential);
            _secrets.AddRange(new[] { key.CredentialId, credential.UserHandle });
            var grantsBefore = _grants.CountAll();

            // the verified sign-in (allowed), then the same response again (replay)
            var ticket = _passkeys.BeginSignIn();
            var response = key.Get(ticket.Bytes, origin, credential.UserHandle, counter: 1, rpId: rp.RpId);
            var valid = _passkeys.CompleteSignIn(ticket.Id, response);
            var replay = _passkeys.CompleteSignIn(ticket.Id, response);

            // account A's credential presented with account B's user handle (F09)
            var ticketB = _passkeys.BeginSignIn();
            var wrongOwner = _passkeys.CompleteSignIn(ticketB.Id, key.Get(ticketB.Bytes, origin, _credentials.UserHandleFor(AccessOpsDatabase.JoAccountId), counter: 2, rpId: rp.RpId));

            // deleted after it was read for verification (F10): delete, then sign in with it
            var owner = ApplicationPrincipal.Build(new VerifiedSignIn { AccountId = dana, TenantId = "A", GrantId = valid.SignIn?.GrantId, Roles = new[] { "Reader" }, DisplayName = "Dana Ellis", Method = "passkey", VerifiedAt = _clock.Now });
            _passkeys.Delete(owner, credential.RowId);
            var ticketC = _passkeys.BeginSignIn();
            var deleted = _passkeys.CompleteSignIn(ticketC.Id, key.Get(ticketC.Bytes, origin, credential.UserHandle, counter: 3, rpId: rp.RpId));

            var issued = _grants.CountAll() - grantsBefore;
            if (valid.SignIn != null)
            {
                _secrets.Add(valid.SignIn.GrantId);
                _grants.Revoke(valid.SignIn.GrantId);
            }
            c.Alias = "Dana Ellis + Jo Newman (software authenticator)";
            c.EntryPoint = "partner route · PasskeyService.CompleteSignIn";
            c.Boundary = "Passkey";
            c.Actual = $"valid: {valid.Outcome}; replay: {replay.Outcome}; wrong owner: {wrongOwner.Outcome}; deleted: {deleted.Outcome}; grants +{issued} (the valid one only)";
            var denied = !replay.Succeeded && !wrongOwner.Succeeded && !deleted.Succeeded;
            c.Outcome = valid.Succeeded && denied && issued == 1 ? "Deny" : "Failed";
            if (replay.Succeeded)
                c.StopCondition = "A replay accepted as a fresh sign-in";
            else if (deleted.Succeeded)
                c.StopCondition = "A deleted credential or revoked grant still works";
        }

        private void Case12(EvidenceCase c)
        {
            var grantsBefore = _grants.CountAll();

            // node A: two callbacks were validated and parked; the first one's page load completed on A
            var nodeA = NewNode();
            var consumed = nodeA.Handoffs.Park(new ExternalIdentity(Issuer, "4f21-9ac0-dana-ellis"));
            var parked = nodeA.Handoffs.Park(new ExternalIdentity(Issuer, "4f21-9ac0-dana-ellis"));
            _secrets.AddRange(new[] { consumed, parked });
            var onA = nodeA.SignIn.Complete(consumed, "a-node-a");
            if (onA.Succeeded) CallbackGrants.Add(onA.SignIn.GrantId);
            nodeA = default;   // node A is lost: its memory (sessions, objects) is gone, the store is not

            // node B: brand-new instances over the shared store
            var nodeB = NewNode();
            var retried = nodeB.SignIn.Complete(consumed, "a-node-b");   // the browser retries the consumed callback
            var resumed = nodeB.SignIn.Complete(parked, "a-node-b");      // the parked hand-off resumes once
            var again = nodeB.SignIn.Complete(parked, "a-node-b");        // and only once
            foreach (var r in new[] { retried, resumed, again })
                if (r.Succeeded) CallbackGrants.Add(r.SignIn.GrantId);

            var issued = _grants.CountAll() - grantsBefore;
            c.Alias = "Dana Ellis · node A lost, node B";
            c.EntryPoint = "two node instances over the shared store";
            c.Boundary = "Hand-off";
            c.Actual = $"consumed on A, retried on B: {(retried.Succeeded ? "GRANT" : retried.Reason)}; parked on A, resumed on B: {(resumed.Succeeded ? "1 grant" : resumed.Reason)}, again: {(again.Succeeded ? "GRANT" : again.Reason)}; grants +{issued}";
            c.Outcome = !retried.Succeeded && !again.Succeeded && issued == 2 ? "Safe deny" : "Failed";
            if (retried.Succeeded || again.Succeeded)
                c.StopCondition = "A replay accepted as a fresh sign-in";
        }

        // ---- helpers ---------------------------------------------------------------------------

        private string Issuer => _configuration["Oidc:Authority"]?.TrimEnd('/') ?? AccessOpsDatabase.ContosoIssuer;

        /// <summary>Signs svc.reports in through the partner route's real password sign-in and returns the principal it establishes.</summary>
        private ClaimsPrincipal PartnerSignIn(out VerifiedSignIn signIn)
        {
            var result = _login.SignIn(ServiceAccount, ServicePassword, "review");
            if (!result.Succeeded)
                throw new InvalidOperationException("The svc.reports fixture could not sign in (Release build or changed fixture).");
            signIn = result.SignIn;
            _secrets.Add(signIn.GrantId);
            return ApplicationPrincipal.Build(signIn);
        }

        private (ExternalHandoffStore Handoffs, WisejExternalSignIn SignIn) NewNode()
        {
            var handoffs = new ExternalHandoffStore(_db, _clock);
            var signIn = new WisejExternalSignIn(handoffs, new ExternalIdentityMapper(_accounts), _grants,
                (LoginAudit)AppHost.Services.GetService(typeof(LoginAudit)), _clock, _configuration, _loggers.CreateLogger<WisejExternalSignIn>());
            return (handoffs, signIn);
        }

        /// <summary>
        /// HTTP client for the partner route. Only for https://localhost: the development
        /// certificate is accepted when it is the localhost certificate - nothing else is.
        /// </summary>
        private static HttpClient PartnerClient()
        {
            var handler = new SocketsHttpHandler { AllowAutoRedirect = false, UseCookies = false };
            handler.SslOptions.RemoteCertificateValidationCallback = (sender, certificate, chain, errors) =>
                errors == System.Net.Security.SslPolicyErrors.None
                || (certificate is X509Certificate2 cert && cert.GetNameInfo(X509NameType.DnsName, false) == "localhost");
            return new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(15) };
        }
    }
}
#endif
