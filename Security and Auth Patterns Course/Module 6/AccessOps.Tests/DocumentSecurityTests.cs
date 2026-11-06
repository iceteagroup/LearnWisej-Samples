using System;
using System.IO;
using System.Linq;
using System.Text;
using AccessOps.Services;
using Xunit;

namespace AccessOps.Tests
{
    /// <summary>
    /// Module 5: the forged download, the share-link signature and expiry, the upload gate and the
    /// parameterised search. Demo people: Ruth Okoye (2101, Finance, UploadDocument granted),
    /// Jordan Marsh (2102, Facilities - no documents). Document 4471 is a Finance contract.
    /// </summary>
    public sealed class DocumentSecurityTests
    {
        private const int RuthOkoye = 2101;
        private const int JordanMarsh = 2102;
        private const int FinanceContract = 4471;

        [Fact]
        public void Forged_download_from_another_department_is_refused_and_nothing_is_streamed()
        {
            using var host = new TestHost();
            host.SignInAs(JordanMarsh);
            Assert.Empty(host.Documents.List());                                      // the button had nothing to offer

            var ex = Assert.Throws<NotAuthorizedException>(() => host.Documents.Download(host.Ctx(), FinanceContract));

            Assert.Equal(DocumentService.NotAvailable, ex.Message);
            Assert.Empty(host.Delivery.Sent);                                          // not one byte left
            Assert.Contains(host.DocumentServiceLog.Lines, l => l.Contains("Refused download of document 4471") && l.Contains("2102"));
        }

        [Fact]
        public void A_missing_document_gets_the_same_answer_as_a_forbidden_one()
        {
            using var host = new TestHost();
            host.SignInAs(JordanMarsh);

            var forbidden = Assert.Throws<NotAuthorizedException>(() => host.Documents.Download(host.Ctx(), FinanceContract));
            var missing = Assert.Throws<NotAuthorizedException>(() => host.Documents.Download(host.Ctx(), 999999));
            Assert.Equal(forbidden.Message, missing.Message);
        }

        [Fact]
        public void The_owner_department_downloads_through_the_service()
        {
            using var host = new TestHost();
            host.SignInAs(RuthOkoye);

            host.Documents.Download(host.Ctx(), FinanceContract);

            var (content, name) = Assert.Single(host.Delivery.Sent);
            Assert.Equal("Supplier contract 4471.pdf", name);
            Assert.Equal("%PDF-", Encoding.ASCII.GetString(content, 0, 5));
        }

        private static ShareLinkService Links(Func<DateTimeOffset> clock)
            => new ShareLinkService(Encoding.UTF8.GetBytes("test-key-test-key-test-key-test-key-0123"), TimeSpan.FromHours(1), clock);

        private static (int Id, long Exp, string Sig) Parse(string url)
        {
            var uri = new Uri("http://localhost/" + url);
            var q = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
            return (int.Parse(uri.Segments.Last()), long.Parse(q["exp"]), q["sig"]);
        }

        [Fact]
        public void A_share_link_is_valid_for_one_hour_and_only_as_issued()
        {
            var now = new DateTimeOffset(2026, 11, 6, 9, 12, 0, TimeSpan.Zero);
            var links = Links(() => now);
            var (id, exp, sig) = Parse(links.Issue(FinanceContract, out var expires));
            Assert.Equal(now.AddHours(1), expires);

            Assert.True(links.IsValid(id, exp, sig));                                  // as issued
            Assert.False(links.IsValid(id, exp + 2 * 86400, sig));                    // expiry edited by hand
            Assert.False(links.IsValid(4472, exp, sig));                              // signature belongs to 4471
            Assert.False(links.IsValid(id, exp, sig.Substring(1)));                   // signature truncated
            Assert.False(links.IsValid(id, exp, "not+base64url!"));
            Assert.False(links.IsValid(id, null, sig));
            Assert.False(Links(() => now.AddHours(2).AddMinutes(28)).IsValid(id, exp, sig));   // presented at 11:40
        }

        [Fact]
        public void A_renamed_executable_is_refused_by_the_signature_check()
        {
            var gate = new UploadGate(20 * 1024 * 1024);
            var exe = new byte[] { 0x4D, 0x5A, 0x90, 0x00, 0x03, 0x00, 0x00, 0x00 }.Concat(new byte[1024]).ToArray();

            var decision = gate.Check("payslip.pdf", exe);

            Assert.False(decision.Accepted);
            Assert.Equal(UploadCheck.Signature, decision.FailedCheck);
            Assert.Contains("4D 5A", decision.Steps.Last().Detail);
            Assert.Equal("payslip.pdf was refused: the file contents are not a PDF document. " + UploadGate.Rule, decision.Message);
        }

        [Fact]
        public void Size_extension_and_scan_are_checked_and_names_are_display_text_only()
        {
            var gate = new UploadGate(1024);
            Assert.Equal(UploadCheck.Size, gate.Check("big.pdf", DemoFiles.Pdf("x").Concat(new byte[2048]).ToArray()).FailedCheck);
            Assert.Equal(UploadCheck.Extension, new UploadGate(1 << 20).Check("tool.exe", new byte[] { 0x4D, 0x5A }).FailedCheck);
            var eicar = Encoding.ASCII.GetBytes("%PDF-1.4 X5O!P%@AP[4\\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*");
            Assert.Equal(UploadCheck.Scan, new UploadGate(1 << 20).Check("x.pdf", eicar).FailedCheck);

            Assert.Equal("passwd.pdf", UploadGate.DisplayName("..\\..\\etc/passwd.pdf"));
            Assert.Equal("ab.pdf", UploadGate.DisplayName("a\u0000b.pdf"));
        }

        [Fact]
        public void An_accepted_upload_is_stored_outside_the_web_root_under_a_generated_name_and_linked_to_the_department()
        {
            using var host = new TestHost();
            host.SignInAs(RuthOkoye);

            var result = host.Documents.Upload(host.Ctx(), "contract-4482.pdf", DemoFiles.Pdf("contract 4482"));

            Assert.True(result.Decision.Accepted);
            var record = host.DocumentRepository.FindById(result.DocumentId.Value);
            Assert.Equal("Finance", record.Department);
            Assert.Equal("contract-4482.pdf", record.Title);
            Assert.Matches("^[0-9a-f]{32}$", record.StoredName);
            Assert.True(File.Exists(Path.Combine(host.Store.DocumentsFolder, record.StoredName)));
            Assert.Empty(Directory.GetFiles(host.Store.QuarantineFolder));          // nothing left in quarantine
        }

        [Fact]
        public void Upload_needs_the_UploadDocument_permission()
        {
            using var host = new TestHost();
            host.SignInAs(JordanMarsh);

            Assert.Throws<NotAuthorizedException>(() => host.Documents.Upload(host.Ctx(), "a.pdf", DemoFiles.Pdf("a")));
        }

        [Fact]
        public void A_hostile_search_term_is_data_not_sql()
        {
            using var host = new TestHost();
            host.SignInAs(RuthOkoye);

            Assert.Empty(host.Documents.Search("' OR 1=1; DROP TABLE documents; --"));
            Assert.NotEmpty(host.Documents.Search("contract"));
            Assert.Equal(4, host.Documents.List().Count);                              // the table is still there, still scoped
        }
    }
}
