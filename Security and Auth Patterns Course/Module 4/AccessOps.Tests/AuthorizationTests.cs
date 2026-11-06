using System.Linq;
using System.Threading.Tasks;
using AccessOps.Services;
using Xunit;

namespace AccessOps.Tests
{
    /// <summary>
    /// The negative tests reviewers actually check: skip the UI entirely and call the services
    /// with a forged request. Demo people: d.ortiz (3081, Operations employee), m.keller (3050,
    /// Operations manager of 3081, 3092, 3077), s.reyes (3001, Administrator).
    /// </summary>
    public sealed class AuthorizationTests
    {
        private const int DanaOrtiz = 3081;
        private const int MiriamKeller = 3050;
        private const int SofiaReyes = 3001;

        [Fact]
        public async Task Forged_approve_from_a_non_manager_is_refused_and_nothing_changes()
        {
            using var host = new TestHost();
            var me = host.SignInAs(DanaOrtiz);
            Assert.False(me.Has(Permission.ApproveRequest));

            // the forged call: no screen, no button - straight to the service, with a request id
            // from another department's employee
            var ex = await Assert.ThrowsAsync<NotAuthorizedException>(() => host.Approvals.ApproveAsync(4115, "ok"));

            Assert.Equal(Authorize.RefusalMessage, ex.Message);
            Assert.Equal("Pending", host.StatusOf(4115));          // the second assertion: nothing was written
            Assert.Contains(host.AuthorizeLog.Lines, l => l.Contains("DENIED ApproveRequest") && l.Contains("3081") && l.Contains("REQ-4115"));
        }

        [Fact]
        public async Task A_manager_cannot_approve_a_request_outside_their_reports()
        {
            using var host = new TestHost();
            host.SignInAs(MiriamKeller);

            // 4115 is charged to Operations (so it is in her queue) but Sam Whitfield reports to Legal
            await Assert.ThrowsAsync<NotAuthorizedException>(() => host.Approvals.ApproveAsync(4115, "ok"));
            Assert.Equal("Pending", host.StatusOf(4115));
        }

        [Fact]
        public async Task A_manager_can_approve_a_request_from_a_direct_report()
        {
            using var host = new TestHost();
            host.SignInAs(MiriamKeller);

            await host.Approvals.ApproveAsync(4118, "ok");

            var request = await host.ApprovalRepository.GetAsync(4118);
            Assert.Equal("Approved", request.Status);
            Assert.Equal(MiriamKeller, request.DecidedBy);
        }

        [Fact]
        public async Task An_unknown_request_id_is_refused_exactly_like_an_out_of_scope_one()
        {
            using var host = new TestHost();
            host.SignInAs(MiriamKeller);

            var missing = await Assert.ThrowsAsync<NotAuthorizedException>(() => host.Approvals.ApproveAsync(999999, "ok"));
            var outside = await Assert.ThrowsAsync<NotAuthorizedException>(() => host.Approvals.ApproveAsync(4115, "ok"));
            Assert.Equal(outside.Message, missing.Message);
        }

        [Fact]
        public async Task The_queue_marks_what_the_manager_may_approve()
        {
            using var host = new TestHost();
            host.SignInAs(MiriamKeller);

            var rows = await host.Approvals.ListAsync();

            Assert.True(rows.Single(r => r.Id == 4118).CanApprove);
            Assert.True(rows.Single(r => r.Id == 4117).CanApprove);
            Assert.Equal("not your report", rows.Single(r => r.Id == 4115).WhyNot);
            Assert.DoesNotContain(rows, r => r.Id == 4104);       // Finance: not in her department's queue
        }

        [Fact]
        public void An_employee_sees_only_their_department_and_the_filter_is_in_the_sql()
        {
            using var host = new TestHost();
            host.SignInAs(DanaOrtiz);

            var docs = host.Documents.List();

            Assert.NotEmpty(docs);
            Assert.All(docs, d => Assert.Equal("Operations", d.Department));
            Assert.Contains(host.DocumentLog.Lines, l => l.Contains("WHERE department = $department") && l.Contains("department=Operations"));
        }

        [Fact]
        public void An_administrator_with_ViewAllDocuments_sees_every_department()
        {
            using var host = new TestHost();
            host.SignInAs(SofiaReyes);

            var departments = host.Documents.List().Select(d => d.Department).Distinct().ToList();

            Assert.True(departments.Count > 1);
            Assert.Contains(host.DocumentLog.Lines, l => l.Contains("department=(all)"));
        }

        [Fact]
        public async Task An_administrator_without_SubmitRequest_or_ApproveRequest_cannot_list_approvals()
        {
            using var host = new TestHost();
            host.SignInAs(SofiaReyes);

            await Assert.ThrowsAsync<NotAuthorizedException>(() => host.Approvals.ListAsync());
        }

        [Fact]
        public async Task Without_a_signed_in_session_every_service_refuses()
        {
            using var host = new TestHost();

            Assert.Throws<NotSignedInException>(() => host.Documents.List());
            await Assert.ThrowsAsync<NotSignedInException>(() => host.Approvals.ApproveAsync(4118, "ok"));
            Assert.Equal("Pending", host.StatusOf(4118));
        }
    }
}
