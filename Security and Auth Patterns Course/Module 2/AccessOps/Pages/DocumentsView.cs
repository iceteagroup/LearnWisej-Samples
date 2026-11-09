using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using AccessOps.Documents;
using AccessOps.Identity;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The intranet "Confidential documents" screen of the Module 2 walkthrough: on the left the
    /// tenant's documents (Record · Title · Class) above the strip "Windows sign-in · Grant"; on
    /// the right the selected record with its Open button, the content box and the repository
    /// read count. The handlers only ask; <see cref="DocumentService"/> decides, and a deny after
    /// a successful Windows sign-in is shown with <c>AlertBox.Show(..., allowHtml: false)</c>.
    /// </summary>
    public class DocumentsView : UserControl
    {
        /// <summary>The one sentence the deny alert shows, whatever the reason.</summary>
        public const string NoAccessMessage = "You do not have access to this document.";

        private const string ContentPlaceholder = "Content appears here after the service allows the read.";

        private readonly DocumentService _documents = AppHost.Get<DocumentService>();
        private readonly CountingDocumentRepository _repository = AppHost.Get<CountingDocumentRepository>();
        private readonly IntranetUser _user;
        private readonly string _tenant;

        private readonly Label lblHeading = new Label();
        private readonly DataGridView grdDocuments = new DataGridView();
        private readonly Label lblRecord = new Label();
        private readonly Label lblRecordTitle = new Label();
        private readonly Button btnOpen = new Button();
        private readonly Label lblContent = new Label();
        private readonly Label lblReads = new Label();
        private int _recordId = 17;

        /// <summary>Builds the screen for the Windows user <c>Program.Main</c> found.</summary>
        public DocumentsView(IntranetUser user)
        {
            _user = user;
            _tenant = user.Subject.Tenant ?? "A";
            this.Name = "DocumentsView";
            this.BackColor = Color.White;

            // left: the tenant's documents
            var left = new Panel
            {
                Name = "pnlDocuments",
                Dock = DockStyle.Fill,
                CssStyle = "border-right:1px solid #e4eaf1;"
            };

            lblHeading.Name = "lblHeading";
            lblHeading.Text = $"Tenant {_tenant} · Documents";
            lblHeading.Dock = DockStyle.Top;
            lblHeading.Height = 44;
            lblHeading.Padding = new Padding(16, 0, 16, 0);
            lblHeading.Font = Theme.Px(14f, FontStyle.Bold);
            lblHeading.ForeColor = ColorTranslator.FromHtml("#0d1b2a");
            lblHeading.TextAlign = ContentAlignment.MiddleLeft;
            lblHeading.CssStyle = "border-bottom:1px solid #e4eaf1;";

            grdDocuments.Name = "grdDocuments";
            grdDocuments.Dock = DockStyle.Fill;
            DataGridStyle.Apply(grdDocuments, rowHeight: 34);
            grdDocuments.Columns.Add(DataGridStyle.MonoColumn("colRecord", "Record", nameof(GridRow.Record), 70, Theme.Accent, bold: true));
            grdDocuments.Columns.Add(DataGridStyle.Column("colTitle", "Title", nameof(GridRow.Title), 200, fill: true));
            var badge = DataGridStyle.Column("colClass", "Class", nameof(GridRow.ClassBadge), 120);
            badge.AllowHtml = true;   // the badge markup is built here from a fixed list, never from input
            grdDocuments.Columns.Add(badge);
            grdDocuments.SelectionChanged += grdDocuments_SelectionChanged;

            var footer = new FlowLayoutPanel
            {
                Name = "pnlSignedIn",
                Dock = DockStyle.Bottom,
                Height = 36,
                Padding = new Padding(16, 10, 16, 0),
                WrapContents = false,
                BackColor = Theme.Strip,
                CssStyle = "border-top:1px solid #e4eaf1;"
            };
            footer.Controls.Add(FooterLabel("lblSignInCaption", "Windows sign-in:", false, new Padding(0, 0, 4, 0)));
            footer.Controls.Add(FooterLabel("lblWindowsAccount", _user.MaskedAccount, true, new Padding(0, 0, 14, 0)));
            footer.Controls.Add(FooterLabel("lblGrantCaption", "Grant:", false, new Padding(0, 0, 4, 0)));
            footer.Controls.Add(FooterLabel("lblGrant", _user.GrantText, true, new Padding(0)));

            left.Controls.Add(grdDocuments);
            left.Controls.Add(footer);
            left.Controls.Add(lblHeading);

            // right: the selected record
            var right = new Panel
            {
                Name = "pnlRecord",
                Dock = DockStyle.Right,
                Width = 380,
                Padding = new Padding(16, 14, 16, 14)
            };

            lblRecord.Name = "lblRecord";
            lblRecord.Dock = DockStyle.Top;
            lblRecord.Height = 22;
            lblRecord.Font = Theme.Px(13f, FontStyle.Bold);
            lblRecord.ForeColor = ColorTranslator.FromHtml("#0d1b2a");

            lblRecordTitle.Name = "lblRecordTitle";
            lblRecordTitle.Dock = DockStyle.Top;
            lblRecordTitle.Height = 26;
            lblRecordTitle.Font = Theme.Px(12f);
            lblRecordTitle.ForeColor = Theme.Muted;

            btnOpen.Name = "btnOpen";
            btnOpen.Text = "Open";
            btnOpen.Dock = DockStyle.Left;
            btnOpen.Width = 72;
            Theme.StyleButton(btnOpen, primary: true);
            btnOpen.Click += btnOpen_Click;
            var buttonRow = new Panel { Name = "pnlOpen", Dock = DockStyle.Top, Height = 32 };
            buttonRow.Controls.Add(btnOpen);

            lblContent.Name = "lblContent";
            lblContent.Dock = DockStyle.Fill;
            lblContent.Padding = new Padding(12, 10, 12, 10);
            lblContent.Font = Theme.Px(12f);
            lblContent.TextAlign = ContentAlignment.TopLeft;
            lblContent.CssStyle = "border:1px solid #e0e7ef; border-radius:6px; line-height:1.5;";

            lblReads.Name = "lblReads";
            lblReads.Dock = DockStyle.Bottom;
            lblReads.Height = 26;
            lblReads.Font = Theme.Mono(11.5f);
            lblReads.ForeColor = Theme.Muted;
            lblReads.TextAlign = ContentAlignment.BottomLeft;

            // Dock order is the reverse of Controls.Add: add bottom-up.
            right.Controls.Add(lblContent);
            right.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 10 });
            right.Controls.Add(buttonRow);
            right.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 6 });
            right.Controls.Add(lblRecordTitle);
            right.Controls.Add(lblRecord);
            right.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 8 });
            right.Controls.Add(lblReads);

            this.Controls.Add(left);
            this.Controls.Add(right);

            LoadDocuments();
        }

        private void LoadDocuments()
        {
            // The list goes through the service too: no grant, no titles.
            var rows = _documents.List(_user.Subject);
            grdDocuments.DataSource = rows?.Select(r => new GridRow(r)).ToList() ?? new List<GridRow>();
            ShowRecord(17);
            ShowContent(null);
            ShowReads();
        }

        private void grdDocuments_SelectionChanged(object sender, EventArgs e)
        {
            if (grdDocuments.CurrentRow?.DataBoundItem is GridRow row && row.Record != _recordId)
            {
                ShowRecord(row.Record);
                ShowContent(null);
            }
        }

        // Open: the click hands the read to Application.StartTask; the subject is the one
        // Program.Main mapped from the Windows SID, and DocumentService reads the grant's current
        // state before the repository runs.
        private void btnOpen_Click(object sender, EventArgs e)
        {
#if DEBUG
            IdentityTrace.Write("Click · WebSocket (Open)");
#endif
            var subject = _user.Subject;
            var tenant = _tenant;
            var recordId = _recordId;
            Application.StartTask(() =>
            {
#if DEBUG
                IdentityTrace.Write("Click · StartTask (Open)");
#endif
                var result = _documents.Read(subject, tenant, recordId, "open");
                Application.Update(this, () => ShowOpened(result));
            });
        }

        private void ShowOpened(DocumentResult result)
        {
            ShowReads();
            if (result.IsAllowed)
            {
                ShowContent(result.Document.Body);
                return;
            }

            ShowContent(null);
            if (result.Decision == "failed")
                AlertBox.Show(DocumentResult.FailedMessage, MessageBoxIcon.Warning, allowHtml: false);
            else
                AlertBox.Show(NoAccessMessage, MessageBoxIcon.Warning, allowHtml: false);   // the reason stays in the audit
        }

        private void ShowRecord(int recordId)
        {
            _recordId = recordId;
            lblRecord.Text = $"Record {recordId}";
            var row = (grdDocuments.DataSource as List<GridRow>)?.FirstOrDefault(r => r.Record == recordId);
            lblRecordTitle.Text = row?.Title ?? "";
        }

        private void ShowContent(string body)
        {
            lblContent.Text = body ?? ContentPlaceholder;
            lblContent.ForeColor = body == null ? ColorTranslator.FromHtml("#9aa8b6") : Theme.Text;
            lblContent.BackColor = body == null ? ColorTranslator.FromHtml("#fafbfc") : Color.White;
        }

        private void ShowReads()
        {
            var reads = _repository.ReadCount;
            lblReads.Text = $"Repository reads: {reads}";
            lblReads.ForeColor = reads > 0 ? Theme.Ok : ColorTranslator.FromHtml("#b42318");
        }

        private static Label FooterLabel(string name, string text, bool value, Padding margin) => new Label
        {
            Name = name,
            Text = text,
            AutoSize = true,
            Margin = margin,
            Font = Theme.Px(11.5f, value ? FontStyle.Bold : FontStyle.Regular),
            ForeColor = value ? Theme.Text : Theme.Muted
        };

        /// <summary>A grid row: the list item plus the class badge markup.</summary>
        private sealed class GridRow
        {
            private static readonly Dictionary<string, string> BadgeColors = new()
            {
                ["Confidential"] = "#c2410c",
                ["Restricted"] = "#b42318",
                ["Internal"] = "#1565d8",
            };

            public GridRow(DocumentListItem item)
            {
                this.Record = item.Record;
                this.Title = item.Title;
                this.Classification = item.Classification;
            }

            public int Record { get; }
            public string Title { get; }
            public string Classification { get; }

            public string ClassBadge
            {
                get
                {
                    var color = BadgeColors.TryGetValue(this.Classification, out var c) ? c : "#5a6b7d";
                    return $"<span style=\"font-size:11px;font-weight:800;color:{color};background:{color}14;border:1px solid {color}55;border-radius:999px;padding:2px 9px\">{WebUtility.HtmlEncode(this.Classification)}</span>";
                }
            }
        }
    }
}
