using AccessOps.Services;
using Xunit;

namespace AccessOps.Tests
{
    /// <summary>
    /// The AccessOps side of the external sign-in: the one-time hand-off, the mapping by issuer +
    /// subject, first-login provisioning with no rights, the disabled account, and the password
    /// path kept for service accounts only. (The protocol side - PKCE, state, nonce, signature,
    /// issuer, audience, expiry - is the ASP.NET Core OpenID Connect handler's, exercised end to end
    /// in the browser against the test identity provider; see the module README.)
    /// </summary>
    public sealed class ExternalSignInTests
    {
        private const string Issuer = "http://localhost:6417";

        private static ExternalIdentity Identity(string subject, string given = "Test", string family = "Person")
            => new ExternalIdentity("Contoso Identity", Issuer, subject, subject + "@contoso.example", given + " " + family, given, family);

        [Fact]
        public void A_hand_off_can_be_redeemed_once()
        {
            var store = new ExternalHandoffStore();
            var handle = store.Park(Identity("s-1"), new ExternalTokenSet());

            Assert.True(store.TryRedeem(handle, out var first, out _));
            Assert.Equal("s-1", first.Subject);
            Assert.False(store.TryRedeem(handle, out _, out _));      // a replayed hand-off finds nothing
            Assert.False(store.TryRedeem("forged", out _, out _));
        }

        [Fact]
        public void A_linked_identity_maps_to_its_employee()
        {
            using var host = new TestHost();
            var mapper = new ExternalIdentityMapper(host.Database, host.Audit);

            var result = mapper.Map(Identity("4f21-9ac0-dana-ellis", "Dana", "Ellis"), "10.4.2.61");

            Assert.Equal(ExternalMapOutcome.Linked, result.Outcome);
            Assert.Equal(2001, result.EmployeeId);
        }

        [Fact]
        public void A_first_login_is_provisioned_with_no_roles_and_therefore_no_permissions()
        {
            using var host = new TestHost();
            var mapper = new ExternalIdentityMapper(host.Database, host.Audit);

            var result = mapper.Map(Identity("new-subject-1", "Jo", "Newman"), "10.4.2.61");
            var again = mapper.Map(Identity("new-subject-1", "Jo", "Newman"), "10.4.2.61");

            Assert.Equal(ExternalMapOutcome.Provisioned, result.Outcome);
            Assert.Equal(ExternalMapOutcome.Linked, again.Outcome);         // linked by issuer + subject from now on
            Assert.Equal(result.EmployeeId, again.EmployeeId);
            var employee = host.Employees.Find(result.EmployeeId);
            Assert.Equal("", employee.Roles);
            Assert.Equal(0, host.Permissions.ResolveFor(employee).Count);
        }

        [Fact]
        public void The_same_subject_from_another_issuer_is_a_different_person()
        {
            using var host = new TestHost();
            var mapper = new ExternalIdentityMapper(host.Database, host.Audit);

            var foreign = new ExternalIdentity("Other", "https://other.example", "4f21-9ac0-dana-ellis", "dana.ellis@contoso.example", "Dana Ellis", "Dana", "Ellis");
            var result = mapper.Map(foreign, "10.4.2.61");

            Assert.Equal(ExternalMapOutcome.Provisioned, result.Outcome);   // never matched by email or subject alone
            Assert.NotEqual(2001, result.EmployeeId);
        }

        [Fact]
        public void A_disabled_employee_is_rejected()
        {
            using var host = new TestHost();
            var mapper = new ExternalIdentityMapper(host.Database, host.Audit);

            Assert.Equal(ExternalMapOutcome.Rejected, mapper.Map(Identity("c901-5e2a-theo-grant", "Theo", "Grant"), "10.4.2.61").Outcome);
        }

        [Fact]
        public void The_password_path_signs_in_a_service_account_only()
        {
            using var host = new TestHost();
            var login = new LoginService(host.Employees, new PasswordService(), new LoginThrottle(), new LoginAuditService(host.Audit, host.Session), host.Session, host.Permissions, new LoginRateLimiter(5, System.TimeSpan.FromSeconds(12), () => System.DateTime.UtcNow));

            var service = login.SignIn("svc.reports", "Demo-Service-2026");
            Assert.True(service.Succeeded);
            Assert.Equal("svc.reports", host.Session.Current.UserName);
        }
    }
}
