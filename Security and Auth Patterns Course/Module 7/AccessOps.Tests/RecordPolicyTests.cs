using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AccessOps.Documents;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using AccessOps.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AccessOps.Tests
{
    /// <summary>
    /// Module 7: the one <see cref="RecordPolicy"/> both routes call - allow, tenant, missing role,
    /// expired and revoked grants (the lab's five), plus the missing grant and another account's
    /// grant. Every deny: the reason, an audit row through <c>AuditService.Deny</c>, and 0
    /// repository calls. Also the shared-store pieces the capstone depends on (single-use hand-off
    /// handles across two "nodes", passkey challenges across instances) and the ingress switch.
    /// </summary>
    public class RecordPolicyTests
    {
        private static ClaimsPrincipal Principal(TestWorld w, string accountId, string tenant, string role, string grantId, string method)
            => ApplicationPrincipal.Build(new VerifiedSignIn
            {
                AccountId = accountId, TenantId = tenant, GrantId = grantId, Roles = new[] { role },
                DisplayName = "fixture", Method = method, VerifiedAt = w.Clock.Now
            });

        /// <summary>An intranet principal as Program.Main establishes it (reader1 or visitor1, grant by role).</summary>
        private static ClaimsPrincipal Intranet(TestWorld w, string userName, out Grant grant)
        {
            var account = w.Accounts.FindByUserName(userName);
            grant = w.Grants.Issue(account.Id, account.Tenant, account.Role, canRead: account.Role == "Reader");
            return Principal(w, account.Id, account.Tenant, account.Role, grant.Id, "windows");
        }

        private static long DenyRows(TestWorld w, string reason)
            => w.Dump("audit").Count(r => r.Contains("|denied|policy:" + reason + "|", StringComparison.Ordinal));

        [Fact]
        public void Allow_the_intranet_reader_reads_A17_with_one_repository_call()
        {
            using var w = new TestWorld();
            var reader = Intranet(w, "reader1", out _);

            var result = w.Policy.Read(reader, "A", 17);

            Assert.True(result.IsAllowed);
            Assert.Equal("Supplier contract", result.Document.Title);
            Assert.Equal(1, w.Repository.ReadCount);
        }

        [Fact]
        public void Allow_the_partner_route_reaches_the_same_decision()
        {
            using var w = new TestWorld();
            var partner = w.SignInPartner(out _);   // OpenID Connect hand-off principal (Dana)

            Assert.True(w.Policy.Read(partner, "A", 17).IsAllowed);
            Assert.Equal(1, w.Repository.ReadCount);
        }

        [Theory]
        [InlineData("reader1")]   // intranet route
        [InlineData("partner")]   // partner route
        public void Tenant_A_asking_for_tenant_B_record_17_is_denied_before_the_repository(string who)
        {
            using var w = new TestWorld();
            var principal = who == "partner" ? w.SignInPartner(out _) : Intranet(w, who, out _);

            var result = w.Policy.Read(principal, "B", 17);

            Assert.False(result.IsAllowed);
            Assert.Equal("tenant", result.Reason);
            Assert.Equal(DocumentResult.DeniedMessage, result.Message);
            Assert.Equal(0, w.Repository.ReadCount);
            Assert.Equal(1, DenyRows(w, "tenant"));
        }

        [Fact]
        public void Missing_role_a_host_authenticated_visitor_is_denied()
        {
            using var w = new TestWorld();
            var visitor = Intranet(w, "visitor1", out var grant);
            Assert.False(grant.CanRead);

            var result = w.Policy.Read(visitor, "A", 17);

            Assert.Equal("role", result.Reason);
            Assert.Equal(0, w.Repository.ReadCount);
            Assert.Equal(1, DenyRows(w, "role"));
        }

        [Fact]
        public void Expired_grant_is_denied_at_its_expiry()
        {
            using var w = new TestWorld();
            var reader = Intranet(w, "reader1", out var grant);
            w.Clock.Now = grant.ExpiresAt;

            var result = w.Policy.Read(reader, "A", 17);

            Assert.Equal("expired", result.Reason);
            Assert.Equal(DocumentResult.EndedMessage, result.Message);
            Assert.Equal(0, w.Repository.ReadCount);
        }

        [Fact]
        public void Revoked_grant_is_denied_even_though_the_principal_still_carries_it()
        {
            using var w = new TestWorld();
            var reader = Intranet(w, "reader1", out var grant);
            Assert.True(w.Grants.Revoke(grant.Id));   // admin console, sign-out elsewhere, incident

            var result = w.Policy.Read(reader, "A", 17);

            Assert.Equal("revoked", result.Reason);
            Assert.Equal(0, w.Repository.ReadCount);
            Assert.Equal(1, DenyRows(w, "revoked"));
        }

        [Fact]
        public void Missing_grant_an_unmapped_windows_account_is_denied()
        {
            using var w = new TestWorld();
            var unmapped = Principal(w, Subject.UnmappedWindowsAccountId("S-1-5-21-1-2-3-1188"), null, "", null, "windows");

            var result = w.Policy.Read(unmapped, "A", 17);

            Assert.Equal("missing", result.Reason);
            Assert.Equal(0, w.Repository.ReadCount);
        }

        [Fact]
        public void A_role_claim_decides_nothing()
        {
            using var w = new TestWorld();
            var visitor = w.Accounts.FindByUserName("visitor1");
            var grant = w.Grants.Issue(visitor.Id, "A", "Visitor", canRead: false);
            var claimsReader = Principal(w, visitor.Id, "A", "Reader", grant.Id, "windows");   // says Reader, grant says no

            Assert.Equal("role", w.Policy.Read(claimsReader, "A", 17).Reason);
            Assert.Equal(0, w.Repository.ReadCount);
        }

        [Fact]
        public void The_deny_audit_row_carries_no_grant_id_and_the_click_correlation()
        {
            using var w = new TestWorld();
            var reader = Intranet(w, "reader1", out var grant);
            using (Correlation.Begin(w.LoggerFactory.CreateLogger("test"), "click-abc123"))
                w.Policy.Read(reader, "B", 17);

            var row = w.Dump("audit").Single(r => r.Contains("policy:tenant"));
            Assert.Contains("click-abc123", row);
            Assert.DoesNotContain(grant.Id, row);
        }

        [Fact]
        public void Scoped_read_authorization_uses_the_same_decision_without_a_repository_call()
        {
            using var w = new TestWorld();
            var reader = Subject.From(Intranet(w, "reader1", out _));

            Assert.True(w.Policy.Authorize(reader, "A", "DOC-201", "scoped-read").IsAllowed);
            Assert.Equal("tenant", w.Policy.Authorize(reader, "B", "DOC-204", "scoped-read").Reason);
            Assert.Equal(0, w.Repository.ReadCount);
        }

        // ---- the shared store: what makes a second node or a restart safe ------------------------

        [Fact]
        public void F12_a_handoff_parked_on_node_A_redeems_once_on_node_B()
        {
            using var w = new TestWorld();
            var nodeA = new ExternalHandoffStore(w.Database, w.Clock);
            var nodeB = new ExternalHandoffStore(w.Database, w.Clock);   // another process over the same store
            var handle = nodeA.Park(TestWorld.Dana);

            Assert.True(nodeB.TryRedeem(handle, out var identity));
            Assert.Equal(TestWorld.Dana, identity);
            Assert.False(nodeA.TryRedeem(handle, out _));               // the original node cannot use it again
            Assert.False(nodeB.TryRedeem(handle, out _));
        }

        [Fact]
        public void F12_of_two_nodes_racing_for_one_handle_exactly_one_wins()
        {
            using var w = new TestWorld();
            var handle = new ExternalHandoffStore(w.Database, w.Clock).Park(TestWorld.Dana);
            var wins = 0;
            Parallel.For(0, 8, _ =>
            {
                if (new ExternalHandoffStore(w.Database, w.Clock).TryRedeem(handle, out var __))
                    System.Threading.Interlocked.Increment(ref wins);
            });
            Assert.Equal(1, wins);
        }

        [Fact]
        public void The_store_keeps_only_a_hash_of_the_handle()
        {
            using var w = new TestWorld();
            var handle = w.ExternalHandoffs.Park(TestWorld.Dana);
            Assert.DoesNotContain(w.Dump("signin_handoffs"), r => r.Contains(handle, StringComparison.Ordinal));
        }

        [Fact]
        public void An_expired_handoff_does_not_resume()
        {
            using var w = new TestWorld();
            var handle = w.ExternalHandoffs.Park(TestWorld.Dana);
            w.Clock.Now += ExternalHandoffStore.Lifetime;
            Assert.False(new ExternalHandoffStore(w.Database, w.Clock).TryRedeem(handle, out _));
        }

        [Fact]
        public void A_passkey_challenge_issued_on_one_instance_is_consumed_once_on_another()
        {
            using var w = new TestWorld();
            var challenge = new PasskeyChallengeStore(w.Database, w.Clock).Issue("webauthn.get");
            var other = new PasskeyChallengeStore(w.Database, w.Clock);
            var bytes = Convert.FromBase64String(challenge.Base64);

            Assert.True(other.TryConsume(challenge.Id, "webauthn.get", bytes));
            Assert.False(other.TryConsume(challenge.Id, "webauthn.get", bytes));
        }

        // ---- the ingress switch (the rollback's "partner route off") -----------------------------

        [Fact]
        public async Task Ingress_switch_off_answers_503_for_every_partner_request()
        {
            var settings = new Dictionary<string, string> { ["Ingress:PartnerRoute"] = "Off" };
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Development" });
            builder.WebHost.UseTestServer();
            builder.Logging.ClearProviders();
            builder.Configuration.AddInMemoryCollection(settings);
            var app = builder.Build();
            app.UseMiddleware<IngressSwitch>();
            app.MapFallback("{**path}", ctx => ctx.Response.WriteAsync("wisej"));
            await app.StartAsync();
            var client = app.GetTestClient();

            foreach (var path in new[] { "/Default.html", "/signin-oidc?code=x&state=y", "/auth/cookie" })
            {
                var response = await client.GetAsync(path);
                Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
                Assert.Equal(IngressSwitch.OffSentence, await response.Content.ReadAsStringAsync());
            }

            app.Configuration["Ingress:PartnerRoute"] = "On";
            Assert.Equal("wisej", await client.GetStringAsync("/Default.html"));
            await app.StopAsync();
        }
    }
}
