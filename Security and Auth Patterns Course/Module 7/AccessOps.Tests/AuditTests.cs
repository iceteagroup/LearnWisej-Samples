using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AccessOps.Infrastructure;
using AccessOps.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AccessOps.Tests
{
    /// <summary>
    /// Module 6: the audit trail is append-only, gapless and hash-chained; the business change and its
    /// audit row commit together; denials are audited; logs carry the correlation ID and never a secret.
    /// </summary>
    public sealed class AuditTests
    {
        private const int JamieMercer = 2201;   // Finance manager, approver of REQ-4471
        private const int RuthOkoye = 2101;     // Finance employee, reports to Jamie
        private const int SofiaReyes = 3001;    // Administrator

        [Fact]
        public async Task Approval_and_its_audit_row_share_the_correlation_id()
        {
            using var host = new TestHost();
            host.SignInAs(JamieMercer);
            var ctx = host.Ctx();

            var seq = await host.Approvals.ApproveAsync(ctx, 4471, "ok");

            Assert.Equal("Approved", host.StatusOf(4471));
            var row = host.Audit.Latest(1).Single();
            Assert.Equal(seq, row.Seq);
            Assert.Equal("Approval.Approve", row.Action);
            Assert.Equal("u-2201", row.Actor);
            Assert.Equal("Allowed", row.Outcome);
            Assert.Equal(ctx.CorrelationId, row.CorrelationId);
        }

        [Fact]
        public async Task A_refused_approval_is_audited_as_denied_and_changes_nothing()
        {
            using var host = new TestHost();
            host.SignInAs(RuthOkoye);
            var ctx = host.Ctx();

            await Assert.ThrowsAsync<NotAuthorizedException>(() => host.Approvals.ApproveAsync(ctx, 4471, "self-approve"));

            Assert.Equal("Pending", host.StatusOf(4471));
            var row = host.Audit.Latest(1).Single();
            Assert.Equal("Denied", row.Outcome);
            Assert.Equal("u-2101", row.Actor);
            Assert.Equal(ctx.CorrelationId, row.CorrelationId);
        }

        [Fact]
        public async Task Deciding_an_already_decided_request_fails_without_an_allowed_row()
        {
            using var host = new TestHost();
            host.SignInAs(JamieMercer);
            await host.Approvals.ApproveAsync(host.Ctx(), 4471, "ok");
            var before = host.Audit.Latest(1).Single().Seq;

            await Assert.ThrowsAsync<InvalidOperationException>(() => host.Approvals.RejectAsync(host.Ctx(), 4471, "late"));

            Assert.Equal("Approved", host.StatusOf(4471));
            Assert.DoesNotContain(host.Audit.Latest(10), r => r.Seq > before && r.Outcome == "Allowed");
            Assert.Equal("Failed", host.Audit.Latest(1).Single().Outcome);
        }

        [Fact]
        public void Audit_rows_cannot_be_updated_or_deleted()
        {
            using var host = new TestHost();
            host.SignInAs(JamieMercer);
            host.Audit.Record(host.Ctx(), "Test.Action", "Test", "1", "Allowed");

            using var connection = host.Database.Open();
            var update = Assert.Throws<SqliteException>(() => Exec(connection, "UPDATE audit_entry SET outcome = 'Denied'"));
            var delete = Assert.Throws<SqliteException>(() => Exec(connection, "DELETE FROM audit_entry"));
            Assert.Contains("append-only", update.Message);
            Assert.Contains("append-only", delete.Message);
        }

        [Fact]
        public void The_chain_verifies_and_names_the_first_altered_row()
        {
            using var host = new TestHost();
            host.SignInAs(JamieMercer);
            for (var i = 0; i < 5; i++)
                host.Audit.Record(host.Ctx(), "Test.Action", "Test", i.ToString(), "Allowed");
            Assert.Null(host.Audit.VerifyChain());

            // someone with the database file drops the trigger and rewrites history
            using (var connection = host.Database.Open())
            {
                Exec(connection, "DROP TRIGGER audit_entry_no_update");
                Exec(connection, "UPDATE audit_entry SET outcome = 'Denied' WHERE seq = 3");
            }

            Assert.Equal(3, host.Audit.VerifyChain());
        }

        [Fact]
        public void Sequence_numbers_are_gapless()
        {
            using var host = new TestHost();
            host.SignInAs(JamieMercer);
            var first = host.Audit.Record(host.Ctx(), "Test.Action", "Test", "a", "Allowed");
            var second = host.Audit.Record(host.Ctx(), "Test.Action", "Test", "b", "Allowed");
            var third = host.Audit.Record(host.Ctx(), "Test.Action", "Test", "c", "Denied");
            Assert.Equal(first + 1, second);
            Assert.Equal(second + 1, third);
        }

        [Fact]
        public void Permission_grant_and_its_audit_row_commit_together_and_denials_are_audited()
        {
            using var host = new TestHost();
            var editor = new PermissionEditorService(host.Authorize, host.Database, host.Audit, new ListLogger<PermissionEditorService>());

            host.SignInAs(RuthOkoye);
            Assert.Throws<NotAuthorizedException>(() => editor.Grant(host.Ctx(), RuthOkoye, Permission.ManageUsers));
            Assert.Equal("Denied", host.Audit.Latest(1).Single().Outcome);

            host.SignInAs(SofiaReyes);
            var seq = editor.Grant(host.Ctx(), JamieMercer, Permission.ViewAuditTrail);
            var row = host.Audit.Latest(1).Single();
            Assert.Equal(seq, row.Seq);
            Assert.Equal("Permission.Grant", row.Action);
            Assert.Equal("Employee:2201/ViewAuditTrail", row.Target);
            Assert.True(host.Permissions.ResolveFor(host.Employees.Find(JamieMercer)).Has(Permission.ViewAuditTrail));

            editor.Revoke(host.Ctx(), JamieMercer, Permission.ViewAuditTrail);
            Assert.False(host.Permissions.ResolveFor(host.Employees.Find(JamieMercer)).Has(Permission.ViewAuditTrail));
            Assert.Null(host.Audit.VerifyChain());
        }

        [Fact]
        public void During_a_key_rotation_links_signed_with_the_previous_key_still_open()
        {
            var oldKey = System.Text.Encoding.UTF8.GetBytes("old-key-old-key-old-key-old-key-0123");
            var newKey = System.Text.Encoding.UTF8.GetBytes("new-key-new-key-new-key-new-key-4567");
            var now = DateTimeOffset.UtcNow;
            var before = new ShareLinkService(oldKey, TimeSpan.FromHours(1), () => now);
            var during = new ShareLinkService(newKey, TimeSpan.FromHours(1), () => now, previousKey: oldKey);
            var after = new ShareLinkService(newKey, TimeSpan.FromHours(1), () => now);

            var oldLink = Parse(before.Issue(4471, out _));
            var newLink = Parse(during.Issue(4471, out _));

            Assert.True(during.IsValid(4471, oldLink.Exp, oldLink.Sig));    // issued before the rotation
            Assert.True(during.IsValid(4471, newLink.Exp, newLink.Sig));
            Assert.False(before.IsValid(4471, newLink.Exp, newLink.Sig));   // new links are signed with the new key only
            Assert.False(after.IsValid(4471, oldLink.Exp, oldLink.Sig));    // previous key removed: old links stop
        }

        private static (long Exp, string Sig) Parse(string relative)
        {
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(new Uri("http://x/" + relative).Query);
            return (long.Parse(query["exp"]), query["sig"]);
        }

        [Fact]
        public void The_log_sink_redacts_secrets_and_keeps_the_correlation_scope()
        {
            var folder = Path.Combine(Path.GetTempPath(), "accessops-logs-" + Guid.NewGuid().ToString("N"));
            try
            {
                using var provider = new RedactingLoggerProvider(folder);
                using var factory = LoggerFactory.Create(b => b.AddProvider(provider));
                var log = factory.CreateLogger("AccessOps.Tests");

                using (log.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = "ab12-cd34", ["UserId"] = "u-2201" }))
                {
                    log.LogInformation("Sign-in for {UserName} with password={Password}", "jamie.mercer", "Demo-Not-A-Secret-91");
                    log.LogInformation("Token exchange returned access_token=eyJhbGciOiJSUzI1NiJ9.eyJzdWIiOiIyMjAxIn0.c2lnbmF0dXJl");
                    log.LogInformation("Calling HR with Authorization: Bearer abc.def.ghi and contact jamie.mercer@contoso.example");
                    log.LogWarning("Connection failed: Data Source=x.db;Password=Demo-Db-Secret-77");
                }

                var text = File.ReadAllText(provider.CurrentFile);
                Assert.DoesNotContain("Demo-Not-A-Secret-91", text);
                Assert.DoesNotContain("eyJhbGciOiJSUzI1NiJ9", text);
                Assert.DoesNotContain("abc.def.ghi", text);
                Assert.DoesNotContain("@contoso.example", text);
                Assert.DoesNotContain("Demo-Db-Secret-77", text);
                Assert.Equal(4, text.Split('\n', StringSplitOptions.RemoveEmptyEntries).Count(l => l.Contains("\"CorrelationId\":\"ab12-cd34\"")));
                Assert.Contains("jamie.mercer", text);
            }
            finally
            {
                try { Directory.Delete(folder, recursive: true); } catch (IOException) { }
            }
        }

        private static void Exec(SqliteConnection connection, string sql)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
