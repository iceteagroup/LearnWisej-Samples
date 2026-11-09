using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AccessOps.Documents;
using AccessOps.Identity;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The Documents screen of the walkthrough: the request bar (Tenant, Record, Open, Export),
    /// the grid Record · Title · Classification · Tenant and the status line. The handlers only
    /// ask; <see cref="DocumentService"/> decides. Open runs through
    /// <c>Application.StartTask</c> (the session is restored on that thread); Export runs on a
    /// raw <c>Task.Run</c> with an explicit subject (<see cref="ExportJob"/>).
    /// </summary>
    public class DocumentsView : UserControl
    {
        private readonly DocumentService _documents = AppHost.Get<DocumentService>();

        private readonly TextBox txtTenant = new TextBox();
        private readonly TextBox txtRecord = new TextBox();
        private readonly Button btnOpen = new Button();
        private readonly Button btnExport = new Button();
        private readonly DataGridView grdDocuments = new DataGridView();
        private readonly StatusLine lblStatus = new StatusLine();

        public DocumentsView()
        {
            this.Name = "DocumentsView";
            this.BackColor = Color.White;

            // request bar
            var bar = new FlowLayoutPanel
            {
                Name = "pnlRequest",
                Dock = DockStyle.Top,
                Height = 54,
                Padding = new Padding(16, 12, 16, 12),
                WrapContents = false,
                CssStyle = "border-bottom:1px solid #e8edf3;"
            };
            var tenantLabel = Theme.FieldLabel("Tenant");
            tenantLabel.Margin = new Padding(0, 7, 10, 0);
            txtTenant.Name = "txtTenant";
            Theme.StyleInput(txtTenant, 52);
            txtTenant.Margin = new Padding(0, 0, 16, 0);
            txtTenant.MaxLength = 8;
            var recordLabel = Theme.FieldLabel("Record");
            recordLabel.Margin = new Padding(0, 7, 10, 0);
            txtRecord.Name = "txtRecord";
            Theme.StyleInput(txtRecord, 52);
            txtRecord.MaxLength = 9;
            txtRecord.Margin = new Padding(0);
            bar.Controls.AddRange(new Control[] { tenantLabel, txtTenant, recordLabel, txtRecord });

            var actions = new FlowLayoutPanel
            {
                Name = "pnlActions",
                Dock = DockStyle.Right,
                Width = 180,
                Padding = new Padding(0, 12, 16, 0),
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false
            };
            btnOpen.Name = "btnOpen";
            btnOpen.Text = "Open";
            btnOpen.Width = 70;
            btnOpen.Margin = new Padding(0);
            Theme.StyleButton(btnOpen, primary: true);
            btnOpen.Click += btnOpen_Click;
            btnExport.Name = "btnExport";
            btnExport.Text = "Export";
            btnExport.Width = 80;
            btnExport.Margin = new Padding(8, 0, 0, 0);
            Theme.StyleButton(btnExport, primary: false);
            btnExport.Click += btnExport_Click;
            actions.Controls.Add(btnExport);
            actions.Controls.Add(btnOpen);

            var top = new Panel { Dock = DockStyle.Top, Height = 54 };
            top.Controls.Add(bar);
            top.Controls.Add(actions);
            bar.Dock = DockStyle.Fill;

            // grid
            grdDocuments.Name = "grdDocuments";
            grdDocuments.Dock = DockStyle.Fill;
            DataGridStyle.Apply(grdDocuments);
            grdDocuments.Columns.Add(DataGridStyle.MonoColumn("colRecord", "Record", nameof(DocumentListItem.Record), 70, Theme.Accent, bold: true));
            grdDocuments.Columns.Add(DataGridStyle.Column("colTitle", "Title", nameof(DocumentListItem.Title), 200, fill: true));
            var classification = DataGridStyle.Column("colClassification", "Classification", nameof(GridRow.ClassificationBadge), 130);
            classification.AllowHtml = true;   // the badge markup is built here from a fixed list, never from input
            grdDocuments.Columns.Add(classification);
            grdDocuments.Columns.Add(DataGridStyle.MonoColumn("colTenant", "Tenant", nameof(DocumentListItem.Tenant), 70, Theme.Body));
            grdDocuments.CellClick += grdDocuments_CellClick;

            this.Controls.Add(grdDocuments);
            this.Controls.Add(lblStatus);
            this.Controls.Add(top);

            LoadDocuments();
        }

        private void LoadDocuments()
        {
            var subject = Subject.From(Application.User);
            txtTenant.Text = subject.Tenant ?? "";
            txtRecord.Text = "17";

            var rows = _documents.List(subject);
            if (rows == null)
            {
                grdDocuments.DataSource = null;
                lblStatus.Show(DocumentResult.DeniedMessage, StatusTone.Deny);
                return;
            }

            grdDocuments.DataSource = rows.Select(r => new GridRow(r)).ToList();
            var role = subject.Roles.FirstOrDefault() ?? "member";
            lblStatus.Show($"Tenant {subject.Tenant} · {rows.Count} documents · signed in as a {role}", StatusTone.Idle);
        }

        private void grdDocuments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || grdDocuments.Rows[e.RowIndex].DataBoundItem is not GridRow row)
                return;

            txtTenant.Text = row.Tenant;
            txtRecord.Text = row.Record.ToString();
            lblStatus.Show($"Tenant {row.Tenant} · record {row.Record} selected", StatusTone.Idle);
        }

        // Open: the click hands the read to Application.StartTask. That pool thread has the
        // session restored, so Application.User answers there - the "BuildPreview" pattern.
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (!TryReadRequest(out var tenant, out var recordId))
                return;
#if DEBUG
            IdentityTrace.Write("Click · WebSocket (Open)");
#endif
            Application.StartTask(() =>
            {
#if DEBUG
                IdentityTrace.Write("Click · StartTask (Open)");
#endif
                var subject = Subject.From(Application.User);
                var result = _documents.Read(subject, tenant, recordId, "open");
                Application.Update(this, () => ShowOpened(result));
            });
        }

        // Export: the subject is captured as data on the click; the job runs on Task.Run, where
        // there is no session, and DocumentService rechecks the grant when it runs.
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (!TryReadRequest(out var tenant, out var recordId))
                return;
#if DEBUG
            IdentityTrace.Write("Click · WebSocket (Export)");
#endif
            var subject = Subject.From(Application.User);
            ExportJob.Queue(_documents, subject, tenant, recordId,
                result => Application.Update(this, () => ShowExported(result)));
        }

        private bool TryReadRequest(out string tenant, out int recordId)
        {
            tenant = (txtTenant.Text ?? "").Trim();
            if (!int.TryParse((txtRecord.Text ?? "").Trim(), out recordId) || recordId <= 0)
            {
                lblStatus.Show("Enter a record number.", StatusTone.Idle);
                return false;
            }
            return true;
        }

        private void ShowOpened(DocumentResult result)
        {
            if (result.IsAllowed)
            {
                SelectRecord(result.Document);
                lblStatus.Show($"Record {result.Document.Id} opened · {result.Document.Title}", StatusTone.Ok);
            }
            else
            {
                ShowRefusal(result);
            }
        }

        private void ShowExported(DocumentResult result)
        {
            if (!result.IsAllowed)
            {
                ShowRefusal(result);
                return;
            }

            var d = result.Document;
            var text = $"{d.Title}\r\nTenant {d.Tenant} · record {d.Id} · {d.Classification}\r\n\r\n{d.Body}\r\n";
            Application.Download(new MemoryStream(Encoding.UTF8.GetBytes(text)), $"record-{d.Tenant}-{d.Id}.txt");
            lblStatus.Show($"Record {d.Id} exported · {d.Title}", StatusTone.Ok);
        }

        // One generic sentence per outcome: the reason stays in the audit event.
        private void ShowRefusal(DocumentResult result)
        {
            if (result.Decision == "failed")
                lblStatus.Show(DocumentResult.FailedMessage, StatusTone.Warn);
            else
                lblStatus.Show(DocumentResult.DeniedMessage, StatusTone.Deny);
        }

        private void SelectRecord(Document document)
        {
            foreach (DataGridViewRow row in grdDocuments.Rows)
            {
                if (row.DataBoundItem is GridRow item && item.Record == document.Id && item.Tenant == document.Tenant)
                {
                    grdDocuments.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }

        /// <summary>A grid row: the list item plus the classification badge markup.</summary>
        private sealed class GridRow
        {
            public GridRow(DocumentListItem item)
            {
                this.Record = item.Record;
                this.Title = item.Title;
                this.Classification = item.Classification;
                this.Tenant = item.Tenant;
            }

            public int Record { get; }
            public string Title { get; }
            public string Classification { get; }
            public string Tenant { get; }

            public string ClassificationBadge
            {
                get
                {
                    var confidential = this.Classification == "Confidential";
                    var color = confidential ? "#c62828" : "#1565d8";
                    var wash = confidential ? "#fdecec" : "#eaf3ff";
                    return $"<span style=\"font-size:11px;font-weight:800;color:{color};background:{wash};border-radius:999px;padding:2px 9px\">{WebUtility.HtmlEncode(this.Classification)}</span>";
                }
            }
        }
    }
}
