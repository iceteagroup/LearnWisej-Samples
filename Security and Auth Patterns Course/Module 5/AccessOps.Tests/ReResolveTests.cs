using System.Threading;
using AccessOps.Services;
using Xunit;

namespace AccessOps.Tests
{
    public sealed class ReResolveTests
    {
        [Fact]
        public void A_granted_permission_takes_effect_at_the_next_action_after_force_re_resolve()
        {
            using var host = new TestHost();
            var refresh = new PermissionRefreshService(host.Authorize, host.Session, host.Permissions, host.Employees);

            // Dana's session starts without ApproveRequest
            var dana = host.SignInAs(3081);
            Assert.False(dana.Has(Permission.ApproveRequest));

            // an administrator grants it in the identity store ...
            using (var db = host.Database.Open())
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO employee_claims (employee_id, kind, permission) VALUES (3081, 'grant', 'ApproveRequest')";
                cmd.ExecuteNonQuery();
            }
            Thread.Sleep(20);

            // ... and asks for a re-resolve (needs ManageUsers)
            host.SignInAs(3001);
            refresh.Request(3081);

            // Dana's still-open session: unchanged until its next action, then re-resolved
            host.Session.Context = dana;
            Assert.False(host.Session.Current.Has(Permission.ApproveRequest));
            Assert.True(refresh.RefreshIfRequested());
            Assert.True(host.Session.Current.Has(Permission.ApproveRequest));
            Assert.False(refresh.RefreshIfRequested());   // once
        }

        [Fact]
        public void Only_a_ManageUsers_holder_can_ask_for_a_re_resolve()
        {
            using var host = new TestHost();
            var refresh = new PermissionRefreshService(host.Authorize, host.Session, host.Permissions, host.Employees);
            host.SignInAs(3050);   // a manager, not an administrator

            Assert.Throws<NotAuthorizedException>(() => refresh.Request(3081));
        }
    }
}
