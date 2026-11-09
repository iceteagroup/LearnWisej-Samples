using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using AccessOps.Documents;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using AccessOps.Security;
using AccessOps.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The confidential-documents reader of the Module 3 walkthrough: the list Document · Title ·
    /// Folder · Last read, the "Identity trace" table (Document · Step · App subject · Request
    /// identity · Effective token · Outcome) and the status line. Clicking a document asks the
    /// <see cref="ScopedDocumentReader"/> for it by ID. The screen shows four distinct plain
    /// messages (allowed, ACL denial, policy denial, missing file) and never a path, SID or
    /// exception text; the trace log keeps the detail.
    /// </summary>
    public class DocumentsView : UserControl
    {
        private const int MaxTraceRows = 12;

        private readonly DocumentService _documents = AppHost.Get<DocumentService>();
        private readonly ConfidentialFileStore _files = AppHost.Get<ConfidentialFileStore>();
        private readonly IdentityTraceLog _traceLog = AppHost.Get<IdentityTraceLog>();
        private readonly AuditLog _audit = AppHost.Get<AuditLog>();
        private readonly IntranetUser _user;

        private readonly DataGridView grdDocuments = new DataGridView();
        private readonly DataGridView grdTrace = new DataGridView();
        private readonly StatusLine lblStatus = new StatusLine();
        private readonly Dictionary<string, (string Text, string Tone)> _lastRead = new();
        private readonly List<IdentityTraceRow> _traceRows = new();
        private List<DocumentRow> _rows;

        /// <summary>Builds the screen for the Windows user <c>Program.Main</c> found.</summary>
        public DocumentsView(IntranetUser user)
        {
            _user = user;
            this.Name = "DocumentsView";
            this.BackColor = Color.White;

            grdDocuments.Name = "grdDocuments";
            grdDocuments.Dock = DockStyle.Top;
            grdDocuments.Height = 30 + 4 * 36 + 2;
            DataGridStyle.Apply(grdDocuments, rowHeight: 36);
            grdDocuments.ColumnHeadersHeight = 30;
            grdDocuments.Columns.Add(DataGridStyle.MonoColumn("colDocument", "Document", nameof(DocumentRow.Document), 92, Theme.Accent, bold: true));
            grdDocuments.Columns.Add(DataGridStyle.Column("colTitle", "Title", nameof(DocumentRow.Title), 200, fill: true));
            grdDocuments.Columns.Add(DataGridStyle.MonoColumn("colFolder", "Folder", nameof(DocumentRow.Folder), 120, Theme.Body));
            var lastRead = DataGridStyle.Column("colLastRead", "Last read", nameof(DocumentRow.LastRead), 128);
            lastRead.AllowHtml = true;   // chip markup built here from fixed texts
            grdDocuments.Columns.Add(lastRead);
            grdDocuments.CellClick += grdDocuments_CellClick;

            var traceTitle = new Label
            {
                Name = "lblTraceTitle",
                Text = "Identity trace",
                Dock = DockStyle.Top,
                Height = 34,
                Padding = new Padding(14, 12, 14, 0),
                Font = Theme.Px(13f, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#0d1b2a")
            };

            grdTrace.Name = "grdTrace";
            grdTrace.Dock = DockStyle.Fill;
            DataGridStyle.Apply(grdTrace, rowHeight: 34);
            grdTrace.Font = Theme.Px(12f);
            grdTrace.Columns.Add(DataGridStyle.MonoColumn("colTraceDocument", "Document", nameof(TraceGridRow.Document), 110, Theme.Accent, bold: true));
            var step = DataGridStyle.Column("colStep", "Step", nameof(TraceGridRow.Step), 70);
            step.DefaultCellStyle.Font = Theme.Px(12f, FontStyle.Bold);
            grdTrace.Columns.Add(step);
            grdTrace.Columns.Add(DataGridStyle.Column("colAppSubject", "App subject", nameof(TraceGridRow.AppSubject), 110));
            grdTrace.Columns.Add(DataGridStyle.MonoColumn("colRequest", "Request identity", nameof(TraceGridRow.RequestIdentity), 150, Theme.Body));
            var effective = DataGridStyle.Column("colEffective", "Effective token", nameof(TraceGridRow.EffectiveToken), 200, fill: true);
            effective.AllowHtml = true;   // colour only; the name is HTML-encoded
            grdTrace.Columns.Add(effective);
            var outcome = DataGridStyle.Column("colOutcome", "Outcome", nameof(TraceGridRow.Outcome), 200);
            outcome.AllowHtml = true;
            grdTrace.Columns.Add(outcome);

            var tracePanel = new Panel { Name = "pnlTrace", Dock = DockStyle.Fill, Padding = new Padding(14, 0, 14, 12) };
            tracePanel.Controls.Add(grdTrace);
            var traceHost = new Panel { Name = "pnlTraceHost", Dock = DockStyle.Fill };
            traceHost.Controls.Add(tracePanel);
            traceHost.Controls.Add(traceTitle);

            // Dock order is the reverse of Controls.Add.
            this.Controls.Add(traceHost);
            this.Controls.Add(grdDocuments);
            this.Controls.Add(lblStatus);

            LoadDocuments();
            lblStatus.Show("Ready", StatusTone.Idle);
        }

        private void LoadDocuments()
        {
            // IDs, titles and fixture folders only: the path is resolved on the server, per read.
            _rows = _files.List().Select(f => new DocumentRow(f.DocumentId, f.Title, f.Folder)).ToList();
            RefreshGrids();
        }

        private void grdDocuments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && grdDocuments.Rows[e.RowIndex].DataBoundItem is DocumentRow row)
                OpenDocument(row.Document);
        }

        /// <summary>
        /// One scoped read, synchronously on the event's thread. The request identity comes from
        /// the current request (the WebSocket request, through <c>IHttpContextAccessor</c>), whose
        /// token is live while the socket is open - not from <c>Application.User</c>, which on a
        /// WebSocket event is the page-load identity (disposed when its connection closed).
        /// </summary>
        private void OpenDocument(string documentId)
        {
#if DEBUG
            LifecycleTrace.Write("Click · " + (Application.IsWebSocket ? "WebSocket" : "HTTP") + " (scoped read)");
            Application.StartTask(() => LifecycleTrace.Write("Click · StartTask (observer)"));
#endif
            if (!OperatingSystem.IsWindows())
            {
                lblStatus.Show("Windows sign-in is not available on this server. Nothing was opened.", StatusTone.Deny);
                return;
            }

            var context = AppHost.Get<IHttpContextAccessor>().HttpContext;
            var subject = _user.Subject;
            var trace = new IdentityTrace(documentId, subject, _traceLog);
            var catalog = new DocumentCatalog(subject, _files, _documents);
            var reader = new ScopedDocumentReader(catalog, trace);

            string status, chip, chipTone, outcome = null, reason = null;
            StatusTone tone;
            try
            {
                reader.Read(context, documentId);   // the bytes are not shown: the lab is about who opened the file
                status = $"{documentId} opened · effective identity back to the pool";
                tone = StatusTone.Ok;
                (chip, chipTone, outcome) = ("Read as user", "ok", "allowed");
            }
            catch (AccessDeniedException)
            {
                status = $"{documentId} is outside your access. Nothing was opened.";
                tone = StatusTone.Deny;
                (chip, chipTone) = ("Not allowed", "bad");     // audited by the policy itself
            }
            catch (UnauthorizedAccessException) when (trace.ScopeOpened)
            {
                status = $"{documentId} could not be opened with your Windows account. The request was logged.";
                tone = StatusTone.Deny;
                (chip, chipTone, outcome, reason) = ("Denied (ACL)", "bad", "failed", "acl");
            }
            catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
            {
                status = $"{documentId} could not be opened. The file is missing; the request was logged.";
                tone = StatusTone.Warn;
                (chip, chipTone, outcome, reason) = ("File missing", "warn", "failed", "missing");
            }
            catch (UnauthorizedAccessException)
            {
                status = "Windows sign-in is not available on this request. Nothing was opened.";
                tone = StatusTone.Deny;
                (chip, chipTone) = ("Not allowed", "bad");
            }
            catch (Exception ex)
            {
                AppHost.Get<Microsoft.Extensions.Logging.ILoggerFactory>()
                    .CreateLogger("AccessOps.Documents")
                    .LogError(ex, "Scoped read failed. correlation={CorrelationId}", catalog.CorrelationId);
                status = "The document could not be opened. Try again later.";
                tone = StatusTone.Warn;
                (chip, chipTone, outcome, reason) = ("Failed", "warn", "failed", "error");
            }

            if (outcome != null)
                _audit.Write(new AuditEvent(subject.Opaque, "file.read", documentId, null, outcome, reason, catalog.CorrelationId ?? AuditLog.NewCorrelationId()));

            var shown = documentId;
            if (trace.InsideFailure is FileNotFoundException)
                shown += " F03";
            foreach (var row in trace.Rows)
                _traceRows.Add(new IdentityTraceRow
                {
                    Document = shown,
                    Step = row.Step,
                    AppSubject = row.AppSubject,
                    RequestIdentity = row.RequestIdentity,
                    EffectiveToken = row.EffectiveToken,
                    Outcome = row.Outcome,
                    Tone = row.Tone
                });
            while (_traceRows.Count > MaxTraceRows)
                _traceRows.RemoveAt(0);

            _lastRead[documentId] = (chip, chipTone);
            RefreshGrids();
            Select(documentId);
            lblStatus.Show(status, tone);
        }

        private void RefreshGrids()
        {
            foreach (var row in _rows)
                row.LastRead = _lastRead.TryGetValue(row.Document, out var r) ? Chip(r.Text, r.Tone) : "<span style=\"color:#9aa8b6\">—</span>";
            grdDocuments.DataSource = null;
            grdDocuments.DataSource = _rows;
            grdTrace.DataSource = _traceRows.Select(r => new TraceGridRow(r)).ToList();
            foreach (DataGridViewRow row in grdTrace.Rows)
            {
                // the mock tints the "after" rows: the token is the pool's again
                if (row.DataBoundItem is TraceGridRow t && t.Step == "After")
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#f7fbf8");
            }
        }

        private void Select(string documentId)
        {
            foreach (DataGridViewRow row in grdDocuments.Rows)
            {
                if (row.DataBoundItem is DocumentRow d && d.Document == documentId)
                {
                    grdDocuments.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }

        private static readonly Dictionary<string, (string Fg, string Bg)> Tones = new()
        {
            ["ok"] = ("#1f9d6b", "#e8f7ef"),
            ["bad"] = ("#c0392b", "#fdecea"),
            ["warn"] = ("#b7791f", "#fdf3e1"),
            ["idle"] = ("#5a6b7d", "#eef2f6"),
        };

        /// <summary>The mock's chip; the text is HTML-encoded, the colours come from a fixed list.</summary>
        internal static string Chip(string text, string tone)
        {
            var (fg, bg) = Tones.TryGetValue(tone, out var t) ? t : Tones["idle"];
            return $"<span style=\"font-size:11.5px;font-weight:800;color:{fg};background:{bg};border:1px solid {fg}55;border-radius:999px;padding:2px 10px;white-space:nowrap\">{WebUtility.HtmlEncode(text)}</span>";
        }

        /// <summary>A row of the documents list.</summary>
        private sealed class DocumentRow
        {
            public DocumentRow(string document, string title, string folder)
            {
                this.Document = document;
                this.Title = title;
                this.Folder = folder;
            }

            public string Document { get; }
            public string Title { get; }
            public string Folder { get; }
            public string LastRead { get; set; }
        }

        /// <summary>A row of the trace grid (HTML for the two coloured cells, encoded).</summary>
        private sealed class TraceGridRow
        {
            public TraceGridRow(IdentityTraceRow r)
            {
                this.Document = r.Document;
                this.Step = r.Step;
                this.AppSubject = r.AppSubject;
                this.RequestIdentity = r.RequestIdentity;
                var user = r.EffectiveToken.Contains("Impersonation") || r.EffectiveToken.Contains("Delegation");
                this.EffectiveToken = $"<span style=\"font-family:Consolas,monospace;font-size:11.5px;font-weight:700;color:{(user ? "#b7791f" : "#1f2d3a")}\">{WebUtility.HtmlEncode(r.EffectiveToken)}</span>";
                this.Outcome = Chip(r.Outcome, r.Tone);
            }

            public string Document { get; }
            public string Step { get; }
            public string AppSubject { get; }
            public string RequestIdentity { get; }
            public string EffectiveToken { get; }
            public string Outcome { get; }
        }
    }
}
