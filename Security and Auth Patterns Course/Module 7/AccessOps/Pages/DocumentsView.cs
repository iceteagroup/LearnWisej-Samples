using System;
using System.Drawing;
using System.IO;
using System.Linq;
using AccessOps.Infrastructure;
using AccessOps.Services;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The confidential documents screen of the capstone. Every byte goes through
    /// <see cref="DocumentService"/>; on top of Module 5 the screen now says how it is being served:
    /// the banner and the footer strip are computed from the live request and the live Wisej.NET
    /// configuration (Application.IsSecure, Application.Configuration), not typed in.
    /// </summary>
    internal sealed class DocumentsView : UserControl
    {
        private readonly IAreaHost _host;
        private readonly DocumentService _documents = AppHost.Get<DocumentService>();

        private readonly Panel pnlToolbar = new Panel();
        private readonly Upload uploadDocument = new Upload();
        private readonly Button btnDownload = new Button();
        private readonly Button btnShareLink = new Button();
        private readonly MessageStrip pnlConnection = new MessageStrip();
        private readonly MessageStrip pnlNotice = new MessageStrip();
        private readonly DataGridView gridDocuments = new DataGridView();
        private readonly Label lblEmpty = new Label();
        private readonly Panel pnlFooter = new Panel();
        private readonly Label lblPosture = new Label();

        public DocumentsView(IAreaHost host)
        {
            _host = host;
            this.Name = "viewDocuments";
            this.BackColor = Color.White;
            var me = AppHost.Get<IAccessOpsSession>().Current;

            pnlToolbar.Name = "pnlToolbar";
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Height = 52;
            pnlToolbar.BackColor = Theme.Strip;
            pnlToolbar.Padding = new Padding(16, 10, 16, 10);
            pnlToolbar.CssStyle = "border-bottom:1px solid #e4eaf1;";

            pnlToolbar.Controls.Add(new Label
            {
                Name = "lblStore",
                Text = "store: the AccessOpsData folder — outside the web root",
                Dock = DockStyle.Right,
                AutoSize = true,
                Font = Theme.Mono(12f),
                ForeColor = Theme.Muted,
                TextAlign = ContentAlignment.MiddleRight
            });

            btnDownload.Name = "btnDownload";
            btnDownload.Text = "Download";
            btnDownload.Dock = DockStyle.Left;
            btnDownload.Width = 96;
            Theme.StyleButton(btnDownload, primary: false);
            btnDownload.Click += btnDownload_Click;

            // the Upload control posts the file to the server and raises Uploaded. AllowedFileTypes only
            // narrows the browser's picker; MaxFileSize is checked again by the gate on the server, and
            // the host refuses a larger request body before either (Kestrel MaxRequestBodySize).
            uploadDocument.Name = "uploadDocument";
            uploadDocument.Text = "Upload document";
            uploadDocument.Dock = DockStyle.Left;
            uploadDocument.Width = 140;
            uploadDocument.AllowedFileTypes = ".pdf,.docx,.xlsx";
            uploadDocument.MaxFileSize = (int)AppHost.Get<UploadGate>().MaxBytes;
            uploadDocument.AllowMultipleFiles = false;
            uploadDocument.BackColor = Theme.Accent;
            uploadDocument.ForeColor = Color.White;
            uploadDocument.Font = Theme.Px(12.5f, FontStyle.Bold);
            uploadDocument.CssStyle = "border-radius:7px;";
            uploadDocument.Uploaded += uploadDocument_Uploaded;
            uploadDocument.Error += uploadDocument_Error;

            pnlToolbar.Controls.Add(btnDownload);
            // hidden, not disabled: without UploadDocument this user can never upload in AccessOps
            if (me.Has(Permission.UploadDocument))
            {
                pnlToolbar.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 11 });
                pnlToolbar.Controls.Add(uploadDocument);
            }

            pnlConnection.Name = "pnlConnection";
            pnlNotice.Name = "pnlNotice";

            gridDocuments.Name = "gridDocuments";
            gridDocuments.Dock = DockStyle.Fill;
            DataGridStyle.Apply(gridDocuments);
            gridDocuments.Columns.Add(DataGridStyle.Column("colDocument", "Document", nameof(DocumentListItem.Reference), 118));
            gridDocuments.Columns.Add(DataGridStyle.Column("colTitle", "Title", nameof(DocumentListItem.Title), 300, fill: true));
            gridDocuments.Columns.Add(DataGridStyle.Column("colDepartment", "Department", nameof(DocumentListItem.Department), 140));
            gridDocuments.Columns.Add(DataGridStyle.Column("colClass", "Class", nameof(DocumentListItem.Classification), 128));
            gridDocuments.Columns["colDocument"].DefaultCellStyle.ForeColor = Theme.Accent;
            gridDocuments.Columns["colDocument"].DefaultCellStyle.Font = Theme.Mono(12.5f, FontStyle.Bold);
            gridDocuments.Columns["colClass"].DefaultCellStyle.ForeColor = Theme.Warn;
            gridDocuments.Columns["colClass"].DefaultCellStyle.Font = Theme.Px(12.5f, FontStyle.Bold);

            lblEmpty.Name = "lblEmpty";
            lblEmpty.Dock = DockStyle.Fill;
            lblEmpty.Text = "No documents you may open\nThe list only ever contains records the service already cleared.";
            lblEmpty.Font = Theme.Px(14f, FontStyle.Bold);
            lblEmpty.ForeColor = Theme.Muted;
            lblEmpty.TextAlign = ContentAlignment.MiddleCenter;
            lblEmpty.Visible = false;

            // footer strip: the session ID's home and the flags on the cookies AccessOps writes, plus Share link
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Height = 52;
            pnlFooter.BackColor = ColorTranslator.FromHtml("#f8fafc");
            pnlFooter.Padding = new Padding(16, 10, 16, 10);
            pnlFooter.CssStyle = "border-top:1px solid #e4eaf1;";

            lblPosture.Name = "lblPosture";
            lblPosture.Dock = DockStyle.Fill;
            lblPosture.Font = Theme.Mono(11.5f);
            lblPosture.ForeColor = ColorTranslator.FromHtml("#7d8ea0");
            lblPosture.TextAlign = ContentAlignment.MiddleLeft;

            btnShareLink.Name = "btnShareLink";
            btnShareLink.Text = "Share link";
            btnShareLink.Dock = DockStyle.Right;
            btnShareLink.Width = 110;
            Theme.StyleButton(btnShareLink, primary: true);
            btnShareLink.Click += btnShareLink_Click;

            pnlFooter.Controls.Add(lblPosture);
            pnlFooter.Controls.Add(btnShareLink);

            this.Controls.Add(gridDocuments);
            this.Controls.Add(lblEmpty);
            this.Controls.Add(pnlNotice);
            this.Controls.Add(pnlConnection);
            this.Controls.Add(pnlToolbar);
            this.Controls.Add(pnlFooter);

            ShowPosture();
            Reload("Ready", StatusTone.Idle);
        }

        /// <summary>The banner and the footer, from the live request and the live configuration.</summary>
        private void ShowPosture()
        {
            var config = Application.Configuration;
            var hardening = AppHost.Get<HardeningOptions>();
            var secure = Application.IsSecure;
            var validated = config.ValidateClient;
            var csp = hardening.CspReportOnly ? "CSP report-only" : "headers set";

            if (secure)
                pnlConnection.Show($"Encrypted over HTTPS and WSS · {csp} · {(validated ? "client validated" : "client NOT validated")} · own cookies flagged", validated ? StatusTone.Ok : StatusTone.Warn);
            else
                pnlConnection.Show($"Plain HTTP — development profile · {csp} · {(validated ? "client validated" : "client NOT validated")} · HTTPS and HSTS apply where Hardening:RequireHttps is on", StatusTone.Warn);

            var storage = config.SessionStorage.ToString();
            storage = storage.Equals("Session", StringComparison.OrdinalIgnoreCase) ? "sessionStorage"
                : storage.Equals("Local", StringComparison.OrdinalIgnoreCase) ? "localStorage" : storage.ToLowerInvariant();
            lblPosture.Text = $"session ID: {storage} · secure: {config.Secure.ToString().ToLowerInvariant()} · validateClient: {validated.ToString().ToLowerInvariant()} · " +
                $"own cookies: {(secure ? "Secure; " : "")}HttpOnly; SameSite=Strict";
        }

        private void Reload(string status, StatusTone tone)
        {
            var rows = _documents.List();
            gridDocuments.DataSource = rows;
            gridDocuments.Visible = rows.Count > 0;
            lblEmpty.Visible = rows.Count == 0;
            _host.Report(status, tone, rows.Count == 0 ? "0 documents" : $"{rows.Count} documents · streamed, never linked");
        }

        private DocumentListItem Selected => gridDocuments.CurrentRow?.DataBoundItem as DocumentListItem;

        private void uploadDocument_Uploaded(object sender, UploadedEventArgs e)
        {
            if (e.Files.Count == 0)
                return;

            var file = e.Files[0];
            _host.Report("Reading the file header…", StatusTone.Idle);
            Application.Update(this);

            byte[] content;
            using (var buffer = new MemoryStream())
            {
                file.InputStream.CopyTo(buffer);
                content = buffer.ToArray();
            }

            using var command = Command.Begin("Document.Upload");
            var result = _documents.Upload(command.Context, file.FileName, content);
            if (!result.Decision.Accepted)
            {
                // the rule they broke and the limits to meet; the file is gone from the server
                pnlNotice.Show(result.Decision.Message, StatusTone.Bad);
                _host.Report("Upload refused — " + result.Decision.FailedCheck.ToString().ToLowerInvariant() + " check · audited", StatusTone.Bad);
                return;
            }

            var me = AppHost.Get<IAccessOpsSession>().Current;
            pnlNotice.Show($"{result.Decision.DisplayName} accepted, scanned and filed under {me.Department}. It is stored under a name the server generated.", StatusTone.Ok);
            Reload("Upload accepted — quarantine clean", StatusTone.Ok);

            // safe interop: the file name is an argument to an existing page function, never script text
            Application.Call("showToast", "Filed: " + result.Decision.DisplayName);
        }

        // Module 7 review finding: a file over Upload.MaxFileSize was refused by the client with no
        // word to the user and no trace on the server. The control reports it here; nothing was sent.
        private void uploadDocument_Error(object sender, UploadErrorEventArgs e)
        {
            using var command = Command.Begin("Document.Upload");
            var name = e.FileNames?.FirstOrDefault() ?? "The file";
            var limitMb = AppHost.Get<UploadGate>().MaxBytes / (1024 * 1024);
            var message = e.ErrorType == UploadErrorType.FileTooLarge
                ? $"{name} was refused: it is larger than the {limitMb} MB limit. Nothing was uploaded."
                : $"{name} could not be uploaded. Reference {command.Context.CorrelationId}.";
            AppHost.Get<AuditService>().Record(command.Context, "Document.Upload", "Document", "-",
                e.ErrorType == UploadErrorType.FileTooLarge ? "Refused:Size(client)" : "Failed");
            pnlNotice.Show(message, StatusTone.Bad);
            _host.Report("Upload refused before it was sent — size check · audited", StatusTone.Bad);
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            var doc = Selected;
            if (doc == null)
            {
                _host.Report("Select a document first.", StatusTone.Idle);
                return;
            }

            using var command = Command.Begin("Document.Download " + doc.Id);
            try
            {
                _documents.Download(command.Context, doc.Id);
                pnlNotice.Clear();
                _host.Report($"{doc.Reference} streamed to this session · audited", StatusTone.Ok);
            }
            catch (NotAuthorizedException ex)
            {
                pnlNotice.Show(ex.Message, StatusTone.Bad);
                _host.Report("Download refused — audited", StatusTone.Bad);
            }
        }

        private void btnShareLink_Click(object sender, EventArgs e)
        {
            var doc = Selected;
            if (doc == null)
            {
                _host.Report("Select a document first.", StatusTone.Idle);
                return;
            }

            string absolute;
            DateTimeOffset expires;
            using (var command = Command.Begin("Document.Share " + doc.Id))
            {
                try
                {
                    // a link is issued only for a document this caller may read now
                    var relative = _documents.IssueShareLink(command.Context, doc.Id, out expires);
                    absolute = new Uri(new Uri(Application.Url), relative).ToString();
                }
                catch (NotAuthorizedException ex)
                {
                    pnlNotice.Show(ex.Message, StatusTone.Bad);
                    _host.Report("Share link refused — audited", StatusTone.Bad);
                    return;
                }
            }

            // the command ends when the link is issued, not when the user closes the dialog
            _host.Report($"Share link issued for {doc.Reference} · expires {expires:HH:mm} UTC", StatusTone.Ok);
            new ShareLinkDialog(doc.Title, absolute, expires).ShowDialog();
        }
    }
}
