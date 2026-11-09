using System;
using System.Linq;
using System.Text.RegularExpressions;
using AccessOps.Documents;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using AccessOps.Security;
using Xunit;

namespace AccessOps.Tests
{
    /// <summary>GrantStore (opaque, expiring, revocable) and ExportService (grant, tenant, record before the repository).</summary>
    public class GrantAndExportTests
    {
        [Fact]
        public void Grant_ids_are_opaque_128_bit_random_values()
        {
            var ids = Enumerable.Range(0, 200).Select(_ => GrantStore.NewId()).ToList();
            Assert.All(ids, id => Assert.Matches(new Regex("^G-[0-9a-f]{32}$"), id));
            Assert.Equal(ids.Count, ids.Distinct().Count());
        }

        [Fact]
        public void A_grant_expires_and_can_be_revoked()
        {
            using var w = new TestWorld();
            var grant = w.Grants.Issue(AccessOpsDatabase.AnaAccountId, "A", "Reader", TimeSpan.FromMinutes(30));
            Assert.True(w.Grants.IsActive(grant.Id));

            w.Clock.Now += TimeSpan.FromMinutes(31);
            Assert.False(w.Grants.IsActive(grant.Id));

            var second = w.Grants.Issue(AccessOpsDatabase.AnaAccountId, "A", "Reader");
            Assert.True(w.Grants.Revoke(second.Id));
            Assert.False(w.Grants.Revoke(second.Id));          // already revoked
            Assert.False(w.Grants.IsActive(second.Id));
        }

        [Fact]
        public void Export_runs_the_repository_only_after_the_grant_and_the_tenant_pass()
        {
            using var w = new TestWorld();
            var principal = w.SignInPartner(out _);

            var allowed = w.Export.Export(principal, "A", 17);
            Assert.True(allowed.IsAllowed);
            Assert.Equal(1, w.Repository.ReadCount);

            var otherTenant = w.Export.Export(principal, "B", 17);   // fixture F02 on the partner route
            Assert.Equal("tenant", otherTenant.Reason);
            Assert.Equal(DocumentResult.DeniedMessage, otherTenant.Message);
            Assert.Equal(1, w.Repository.ReadCount);
        }

        [Fact]
        public void F05_a_stale_tab_is_denied_after_sign_out_in_another_tab_with_zero_reads_and_no_new_grant()
        {
            using var w = new TestWorld();
            var tab1 = w.SignInPartner(out var signIn1);
            var tab2 = w.SignInPartner(out _);                           // second profile, its own grant
            Assert.Equal(2, w.Grants.CountAll());

            var revoked = w.SignOut.SignOut(Subject.From(tab1), "127.0.0.1");
            Assert.Equal(2, revoked);                                 // sign-out revokes the account's grants everywhere

            var stale = w.Export.Export(tab2, "A", 17);
            Assert.False(stale.IsAllowed);
            Assert.Equal("revoked", stale.Reason);   // Module 7: RecordPolicy names the reason
            Assert.Equal(DocumentResult.EndedMessage, stale.Message);
            Assert.Equal(0, w.Repository.ReadCount);
            Assert.Equal(2, w.Grants.CountAll());                     // no new grant
            Assert.Null(w.Documents.List(Subject.From(tab2)));        // the list is refused too
        }

        [Fact]
        public void A_grant_claim_naming_another_accounts_grant_is_denied()
        {
            using var w = new TestWorld();
            w.SignInPartner(out var ana);
            var forged = ApplicationPrincipal.Build(new VerifiedSignIn
            {
                AccountId = AccessOpsDatabase.KaiAccountId,
                TenantId = "A",
                GrantId = ana.GrantId,           // someone else's grant ID in the claim
                Roles = new[] { "Reader" },
                VerifiedAt = w.Clock.Now
            });

            var result = w.Export.Export(forged, "A", 17);
            Assert.Equal("account", result.Reason);  // Module 7: RecordPolicy names the reason
            Assert.Equal(0, w.Repository.ReadCount);
        }

        [Fact]
        public void A_cookie_handle_redeems_once_and_expires()
        {
            using var w = new TestWorld();
            w.SignInPartner(out var signIn);
            var handle = w.Handoffs.Create(signIn);
            Assert.True(w.Handoffs.TryRedeem(handle, out var redeemed));
            Assert.Equal(signIn.GrantId, redeemed.GrantId);
            Assert.False(w.Handoffs.TryRedeem(handle, out _));        // single use

            var late = w.Handoffs.Create(signIn);
            w.Clock.Now += CookieHandoffStore.Lifetime;
            Assert.False(w.Handoffs.TryRedeem(late, out _));          // short-lived
        }
    }

    /// <summary>ReturnUrlPolicy: local paths or the allowlist, everything else "/" (fixture F11).</summary>
    public class ReturnUrlPolicyTests
    {
        [Theory]
        [InlineData("/Default.html?record=17", "/Default.html?record=17")]
        [InlineData("/", "/")]
        [InlineData("//evil.example", "/")]
        [InlineData("/\\evil.example", "/")]
        [InlineData("https://evil.example/x", "/")]
        [InlineData("evil.example", "/")]
        [InlineData("/\t/evil.example", "/")]
        [InlineData("", "/")]
        [InlineData(null, "/")]
        [InlineData("https://partners.accessops.example/welcome", "https://partners.accessops.example/welcome")]
        public void Resolve(string returnUrl, string expected)
        {
            var allowlist = new[] { "https://partners.accessops.example/welcome" };
            Assert.Equal(expected, ReturnUrlPolicy.Resolve(returnUrl, allowlist));
        }
    }
}
