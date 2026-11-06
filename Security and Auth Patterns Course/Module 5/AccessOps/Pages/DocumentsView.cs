using System;
using System.Drawing;
using System.IO;
using System.Linq;
using AccessOps.Services;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The confidential documents screen. Every byte goes through <see cref="DocumentService"/>:
    /// uploads through the gate (size, extension, signature, scan, quarantine), downloads through
    /// the permission check and Application.Download, share links only for documents the caller may
    /// read. The handlers only call the service; the screen holds no path and no URL to a file.
    /// </summary>
    internal sealed class DocumentsView : UserControl
    {
        private readonly IAreaHost _host;
        private readonly DocumentService _documents = AppHost.Get<DocumentService>();
        private readonly ShareLinkService _links = AppHost.Get<ShareLinkService>();

        private readonly Panel pnlToolbar = new Panel();
        private readonly Upload uploadDocument = new Upload();
        private readonly Button btnDownload = new Button();
        private readonly Button btnShareLink = new Button();
        private readonly MessageStrip pnlNotice = new MessageStrip();
        private readonly DataGridView gridDocuments = new DataGridView();
        private readonly Label lblEmpty = new Label();

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

            btnShareLink.Name = "btnShareLink";
            btnShareLink.Text = "Share link (1 h)";
            btnShareLink.Dock = DockStyle.Left;
            btnShareLink.Width = 128;
            Theme.StyleButton(btnShareLink, primary: false);
            btnShareLink.Click += btnShareLink_Click;

            btnDownload.Name = "btnDownload";
            btnDownload.Text = "Download";
            btnDownload.Dock = DockStyle.Left;
            btnDownload.Width = 96;
            Theme.StyleButton(btnDownload, primary: false);
            btnDownload.Click += btnDownload_Click;

            // the Upload control posts the file to the server and raises Uploaded. AllowedFileTypes only
            // narrows the browser's picker; MaxFileSize is checked again by the gate on the server.
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

            pnlToolbar.Controls.Add(btnShareLink);
            pnlToolbar.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 11 });
            pnlToolbar.Controls.Add(btnDownload);
            // hidden, not disabled: without UploadDocument this user can never upload in AccessOps
            if (me.Has(Permission.UploadDocument))
            {
                pnlToolbar.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 11 });
                pnlToolbar.Controls.Add(uploadDocument);
            }

            pnlNotice.Name = "pnlNotice";

            gridDocuments.Name = "gridDocuments";
            gridDocuments.Dock = DockStyle.Fill;
            DataGridStyle.Apply(gridDocuments);
            gridDocuments.Columns.Add(DataGridStyle.Column("colTitle", "Document", nameof(DocumentListItem.Title), 300, fill: true));
            gridDocuments.Columns.Add(DataGridStyle.Column("colDepartment", "Department", nameof(DocumentListItem.Department), 140));
            gridDocuments.Columns.Add(DataGridStyle.Column("colClass", "Class", nameof(DocumentListItem.Classification), 120));
            gridDocuments.Columns.Add(DataGridStyle.Column("colSize", "Size", nameof(DocumentListItem.Size), 90));
            gridDocuments.Columns.Add(DataGridStyle.Column("colUpdated", "Updated", nameof(DocumentListItem.Updated), 110));
            gridDocuments.Columns["colTitle"].DefaultCellStyle.ForeColor = Theme.Accent;
            gridDocuments.Columns["colTitle"].DefaultCellStyle.Font = Theme.Px(12.5f, FontStyle.Bold);
            gridDocuments.Columns["colClass"].DefaultCellStyle.ForeColor = Theme.Warn;
            gridDocuments.Columns["colClass"].DefaultCellStyle.Font = Theme.Px(12.5f, FontStyle.Bold);
            gridDocuments.Columns["colSize"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            lblEmpty.Name = "lblEmpty";
            lblEmpty.Dock = DockStyle.Fill;
            lblEmpty.Text = "No documents you may open\nThe list only ever contains records the service already cleared.";
            lblEmpty.Font = Theme.Px(14f, FontStyle.Bold);
            lblEmpty.ForeColor = Theme.Muted;
            lblEmpty.TextAlign = ContentAlignment.MiddleCenter;
            lblEmpty.Visible = false;

            this.Controls.Add(gridDocuments);
            this.Controls.Add(lblEmpty);
            this.Controls.Add(pnlNotice);
            this.Controls.Add(pnlToolbar);

            Reload("Ready", StatusTone.Idle);
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

            var result = _documents.Upload(file.FileName, content);
            if (!result.Decision.Accepted)
            {
                // the rule they broke and the limits to meet; the file is gone from the server
                pnlNotice.Show(result.Decision.Message, StatusTone.Bad);
                _host.Report("Upload refused — " + result.Decision.FailedCheck.ToString().ToLowerInvariant() + " check · audited in the server log", StatusTone.Bad);
                return;
            }

            var me = AppHost.Get<IAccessOpsSession>().Current;
            pnlNotice.Show($"{result.Decision.DisplayName} accepted, scanned and filed under {me.Department}. It is stored under a name the server generated.", StatusTone.Ok);
            Reload("Upload accepted — quarantine clean", StatusTone.Ok);

            // safe interop: the file name is an argument to an existing page function, never script text
            Application.Call("showToast", "Filed: " + result.Decision.DisplayName);
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            var doc = Selected;
            if (doc == null)
            {
                _host.Report("Select a document first.", StatusTone.Idle);
                return;
            }

            try
            {
                _documents.Download(doc.Id);
                pnlNotice.Clear();
                _host.Report($"{doc.Title} streamed to this session · audited in the server log", StatusTone.Ok);
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

            try
            {
                // a link is issued only for a document this caller may read now
                _documents.Authorized(doc.Id, "share");
                var relative = _links.Issue(doc.Id, out var expires);
                var absolute = new Uri(new Uri(Application.Url), relative).ToString();
                new ShareLinkDialog(doc.Title, absolute, expires).ShowDialog();
                _host.Report($"Share link issued for {doc.Title} · expires {expires:HH:mm} UTC", StatusTone.Ok);
            }
            catch (NotAuthorizedException ex)
            {
                pnlNotice.Show(ex.Message, StatusTone.Bad);
                _host.Report("Share link refused — audited", StatusTone.Bad);
            }
        }
    }
}
