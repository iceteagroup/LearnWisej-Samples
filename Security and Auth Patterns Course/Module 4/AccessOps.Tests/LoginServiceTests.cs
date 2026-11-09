using System;
using System.Linq;
using AccessOps.Identity;
using AccessOps.Security;
using Xunit;

namespace AccessOps.Tests
{
    /// <summary>LoginService and LoginThrottle: generic failures, throttling, no password anywhere.</summary>
    public class LoginServiceTests
    {
        [Theory]
        [InlineData("partner.ana@fabrikam.test", "wrong-password", LoginOutcome.BadCredential)]
        [InlineData("nobody@fabrikam.test", "Fabrikam-demo-17", LoginOutcome.UnknownUser)]
        [InlineData("partner.old@fabrikam.test", "Fabrikam-demo-old", LoginOutcome.Disabled)]
        [InlineData("partner.kai@northwind.test", "Northwind-demo-23", LoginOutcome.TenantNotApproved)]
        public void Every_refusal_shows_the_same_sentence_and_creates_no_grant(string user, string password, LoginOutcome expected)
        {
            using var w = new TestWorld();
            var result = w.Login.SignIn(user, password, "10.0.0.5");

            Assert.False(result.Succeeded);
            Assert.Equal(expected, result.Outcome);
            Assert.Equal("That user name and password combination is not valid.", result.Message);
            Assert.Null(result.SignIn);
            Assert.Equal(0, w.Grants.CountAll());
        }

        [Fact]
        public void A_good_password_creates_exactly_one_grant_before_the_principal_exists()
        {
            using var w = new TestWorld();
            var result = w.Login.SignIn("  Partner.Ana@Fabrikam.test ", TestWorld.AnaPassword, "10.0.0.5");

            Assert.True(result.Succeeded);
            Assert.Equal(1, w.Grants.CountAll());
            Assert.True(w.Grants.IsActive(result.SignIn.GrantId));
            Assert.Equal("A", result.SignIn.TenantId);
            Assert.Equal(new[] { "Reader" }, result.SignIn.Roles);
        }

        [Fact]
        public void The_fifth_failure_locks_the_name_and_then_even_the_right_password_is_refused()
        {
            using var w = new TestWorld();
            for (int i = 1; i <= 5; i++)
            {
                var r = w.Login.SignIn("partner.ana@fabrikam.test", "bad-" + i, "10.0.0.5");
                Assert.Equal(i, r.Failures);
            }

            var locked = w.Login.SignIn("partner.ana@fabrikam.test", TestWorld.AnaPassword, "10.0.0.5");
            Assert.False(locked.Succeeded);
            Assert.Equal(LoginOutcome.LockedOut, locked.Outcome);
            Assert.Equal(SignInResult.GenericFailure, locked.Message);
            Assert.Equal(0, w.Grants.CountAll());

            // fifteen minutes later the lock has passed
            w.Clock.Now += LoginThrottle.Window;
            Assert.True(w.Login.SignIn("partner.ana@fabrikam.test", TestWorld.AnaPassword, "10.0.0.5").Succeeded);
        }

        [Fact]
        public void Twenty_failures_from_one_address_lock_the_address_for_every_name()
        {
            using var w = new TestWorld();
            for (int i = 0; i < 20; i++)
                w.Login.SignIn($"spray{i}@example.test", "Password1", "203.0.113.7");

            var fromSameAddress = w.Login.SignIn("partner.ana@fabrikam.test", TestWorld.AnaPassword, "203.0.113.7");
            Assert.Equal(LoginOutcome.LockedOut, fromSameAddress.Outcome);

            var fromElsewhere = w.Login.SignIn("partner.ana@fabrikam.test", TestWorld.AnaPassword, "198.51.100.9");
            Assert.True(fromElsewhere.Succeeded);
        }

        [Fact]
        public void Unknown_names_are_counted_like_real_ones()
        {
            using var w = new TestWorld();
            var unknown = w.Login.SignIn("ghost@fabrikam.test", "x", "10.0.0.5");
            var known = w.Login.SignIn("partner.ana@fabrikam.test", "x", "10.0.0.6");
            Assert.Equal(unknown.Failures, known.Failures);
            Assert.Equal(unknown.Message, known.Message);
        }

        [Fact]
        public void No_password_value_reaches_the_login_audit_the_audit_or_a_log_line()
        {
            using var w = new TestWorld();
            const string typedWrong = "Sup3r-Secret-Typo!";
            w.Login.SignIn("partner.ana@fabrikam.test", typedWrong, "10.0.0.5");
            w.Login.SignIn("partner.ana@fabrikam.test", TestWorld.AnaPassword, "10.0.0.5");

            var everything = w.Dump("login_audit").Concat(w.Dump("audit")).Concat(w.Logs.Lines).ToList();
            Assert.NotEmpty(everything);
            Assert.DoesNotContain(everything, line => line.Contains(typedWrong, StringComparison.Ordinal));
            Assert.DoesNotContain(everything, line => line.Contains(TestWorld.AnaPassword, StringComparison.Ordinal));
            // the typed user name is not stored in clear either: the audit has an opaque "u-" value
            Assert.DoesNotContain(everything, line => line.Contains("partner.ana", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void The_parked_sign_in_has_no_place_for_a_password_or_a_token()
        {
            var names = typeof(VerifiedSignIn).GetProperties().Select(p => p.Name.ToLowerInvariant()).ToList();
            Assert.DoesNotContain(names, n => n.Contains("password") || n.Contains("secret") || n.Contains("token") || n.Contains("session"));
        }
    }
}
