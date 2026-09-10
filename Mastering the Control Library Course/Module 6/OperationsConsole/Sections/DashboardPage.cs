using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OperationsConsole.Dashboard;
using OperationsConsole.Models;
using OperationsConsole.Services;
using OperationsConsole.Shell;
using Wisej.Web;
using Wisej.Web.Ext.ChartJS;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// The <b>Dashboard</b> section (Module 6 · Charts, Dashboards, Content, Media, and Documents).
    /// <para>
    /// The question this screen answers is written down first — <i>"are we on track for this month's ticket
    /// target?"</i> — and every control on it earns its place by helping answer that question: a ChartJS bar chart
    /// for the <b>pattern</b> (six months of tickets opened vs closed), a <c>ProgressBar</c> for the <b>single
    /// status value</b> (completion of this month's target), a <c>PdfViewer</c> and an <c>HtmlPanel</c> for
    /// <b>content</b>, an <c>Upload</c> for the browser → server file boundary, and a reserved panel where a
    /// <c>GoogleMaps</c> component would answer a <b>geographic</b> question.
    /// </para>
    /// <para>
    /// One rule holds the page together: <see cref="RefreshDashboard"/> is the only method that writes to those
    /// controls, and it reads exactly one object — the <see cref="DashboardModel"/> the service returned. No click
    /// handler builds chart data, so the chart, the gauge, the preview and the timestamp can never disagree.
    /// </para>
    /// </summary>
    public partial class DashboardPage : UserControl, ISection
    {
        // Chart / status colours, chosen once so the page reads as one screen (same palette as the shell).
        private static readonly Color OpenedColor = Color.FromArgb(21, 101, 216);    // blue  — tickets opened
        private static readonly Color ClosedColor = Color.FromArgb(31, 157, 87);     // green — tickets closed
        private static readonly Color OnTargetColor = Color.FromArgb(31, 157, 87);
        private static readonly Color BelowTargetColor = Color.FromArgb(232, 161, 60);
        private static readonly Color InkColor = Color.FromArgb(13, 27, 42);
        private static readonly Color GridColor = Color.FromArgb(228, 234, 241);
        private static readonly Color MutedColor = Color.FromArgb(90, 107, 125);
        private static readonly Color ErrorColor = Color.FromArgb(224, 86, 59);

        private readonly DocumentStore _documents = new DocumentStore();
        private readonly DashboardService _service;

        /// <summary>The last model that reached the screen — what the user is still looking at when a refresh fails.</summary>
        private DashboardModel _model;

        private bool _refreshing;

        public DashboardPage()
        {
            InitializeComponent();

            _service = new DashboardService(_documents);

            ConfigureChart();
            ConfigureUpload();

            htmlPreview.Html = PreviewHtml.Empty("Loading the first snapshot…");
        }

        /// <inheritdoc/>
        public string Title => "Dashboard";

        /// <summary>
        /// The shell's Refresh command. It is the same command as the page's own Refresh button — one refresh path,
        /// not two.
        /// </summary>
        public void RefreshSection() => btnRefresh_Click(btnRefresh, EventArgs.Empty);

        /// <summary>
        /// First paint. The dashboard must never open empty, so it takes an immediate snapshot (same aggregation,
        /// no simulated latency); every later refresh goes through <see cref="btnRefresh_Click"/>.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ConsoleLog.Add("DashboardPage · the question: " + DashboardService.Question);
            RefreshDashboard(_service.GetInitialDashboard());
            ConsoleLog.Add("initial snapshot — " + _service.TicketCount + " tickets aggregated into " +
                           _model.Months.Count + " monthly points (no service latency)");
            ConsoleLog.Status("Dashboard ready — " + DescribeCompletion(_model) + ".", StatusLevel.Ok);
        }

        // ================================================================================================
        // One refresh, one model
        // ================================================================================================

        /// <summary>
        /// <b>The</b> refresh method: chart, indicator, preview and stamp, all from one <see cref="DashboardModel"/>.
        /// Labels and DataSets are assigned together here — labels are part of chart data, not decoration — and this
        /// is the only place in the project that touches <c>chartTickets.Labels</c> or <c>chartTickets.DataSets</c>.
        /// </summary>
        private void RefreshDashboard(DashboardModel model)
        {
            if (model == null)
                return;

            var firstFill = _model == null;
            _model = model;

            // 1 · the pattern — labels and DataSets, set together, in one place
            chartTickets.Labels = model.Months.ToArray();
            chartTickets.DataSets.Clear();
            chartTickets.DataSets.Add(BuildSeries("Opened", model.Opened, OpenedColor));
            chartTickets.DataSets.Add(BuildSeries("Closed", model.Closed, ClosedColor));

            // The first fill is rendered from the control's own state when the chart appears; every later fill
            // asks the client for an animated transition from the previous data set to this one.
            if (!firstFill)
                chartTickets.UpdateData(400);

            // 2 · the single status value — a ProgressBar, not a second chart
            progressCompletion.Value = Math.Max(progressCompletion.Minimum,
                                       Math.Min(progressCompletion.Maximum, model.CompletionPercent));
            progressCompletion.BarColor = model.OnTarget ? OnTargetColor : BelowTargetColor;

            // 3 · the content — the document the model points at, and the model in words
            ShowPreview(model.PreviewDocument);
            htmlPreview.Html = PreviewHtml.Summary(model, DashboardService.MonthlyTarget);

            // 4 · how fresh all of the above is
            lblLastRefreshed.Text = "Last refreshed " + model.GeneratedAt.ToString("HH:mm:ss");

            ConsoleLog.Record(model.PreviewDocument.Id);
            ConsoleLog.Add("RefreshDashboard(model) → chartTickets (" + model.Months.Count + " labels, " +
                           chartTickets.DataSets.Count + " DataSets), progressCompletion " + model.CompletionPercent +
                           "%, " + (model.PreviewDocument.IsUploaded ? "pdfPreview.PdfStream" : "pdfPreview.PdfSource") +
                           ", htmlPreview, lblLastRefreshed");
        }

        /// <summary>
        /// One series of the chart. Bar data sets take a colour <i>per point</i>, so the array is built to the
        /// length of the series — one deliberate colour, repeated, instead of whatever Chart.js would pick.
        /// </summary>
        private static BarDataSet BuildSeries(string label, IReadOnlyList<int> values, Color color)
        {
            var colors = new Color[values.Count];
            for (var i = 0; i < colors.Length; i++)
                colors[i] = color;

            return new BarDataSet
            {
                Label = label,
                Data = values.Cast<object>().ToArray(),
                BackgroundColor = colors,
                BorderColor = colors,
                BorderWidth = 1
            };
        }

        /// <summary>
        /// Points the <c>PdfViewer</c> at the document the model references: the report that ships in
        /// <c>wwwroot/</c> (a URL the browser fetches) or the last uploaded report (bytes <c>DocumentStore</c>
        /// is holding in memory, streamed to the viewer). The model carries the reference; the bytes never do.
        /// </summary>
        private void ShowPreview(PreviewDocument document)
        {
            if (document == null)
                return;

            if (document.IsUploaded)
            {
                Stream content = _documents.OpenRead(document.Id);
                if (content != null)
                {
                    pdfPreview.PdfSource = null;
                    pdfPreview.FileName = document.Title;
                    pdfPreview.PdfStream = content;
                }

                lblPreviewCaption.Text = document.Title + " · " + DocumentStore.FormatSize(document.SizeBytes) +
                                         " · uploaded " + document.ReceivedAt.ToString("HH:mm:ss") + " · " + document.Id;
            }
            else
            {
                pdfPreview.PdfStream = null;
                pdfPreview.PdfSource = document.Url;
                lblPreviewCaption.Text = document.Title + " · shipped in " + document.Url + " · " + document.Id;
            }

            lblPreviewCaption.ForeColor = MutedColor;
        }

        // ================================================================================================
        // Commands — thin handlers that call named methods
        // ================================================================================================

        /// <summary>
        /// The lab's thin click handler: show the loading state, ask the service for one model, hand it to
        /// <see cref="RefreshDashboard"/>, and put the button back whatever happens.
        /// </summary>
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnRefresh.Name);
            await RefreshFromServiceAsync();
        }

        private async Task RefreshFromServiceAsync()
        {
            if (_refreshing)
                return;

            SetLoading(true);
            ConsoleLog.Add("btnRefresh → loading state on, DashboardService.GetDashboard() (~" + _service.LatencyMs + " ms)");
            ConsoleLog.Status("Refreshing the dashboard…", StatusLevel.Ok);

            try
            {
                // The aggregation runs off the request thread so the loading state is actually seen;
                // Application.Update below pushes the finished screen over the socket.
                // Verified: an exception that escapes the StartTask lambda is reported by Wisej.NET in its own
                // "Application Error" dialog (stack trace included) even when the awaited Task is caught here,
                // so the lambda catches it and hands it back; it is re-thrown on the request thread below.
                var outcome = await Application.StartTask(() =>
                {
                    try { return (Model: _service.GetDashboard(), Error: (Exception)null); }
                    catch (Exception ex) { return (Model: (DashboardModel)null, Error: ex); }
                });
                if (outcome.Error != null)
                    throw outcome.Error;
                var model = outcome.Model;

                RefreshDashboard(model);

                ConsoleLog.Add("✓ dashboard model " + model.GeneratedAt.ToString("HH:mm:ss") + " — " +
                               model.TicketsAggregated + " tickets aggregated into " + model.Months.Count +
                               " monthly points (the ticket table never leaves DashboardService)");
                ConsoleLog.Status("Dashboard refreshed at " + model.GeneratedAt.ToString("HH:mm:ss") + " — " +
                                  DescribeCompletion(model) + ".", model.OnTarget ? StatusLevel.Ok : StatusLevel.Warning);
                new Toast("Dashboard refreshed.", "icon-check")
                {
                    AutoCloseDelay = 2500,
                    Alignment = ContentAlignment.TopRight
                }.Show();
            }
            catch (Exception ex)
            {
                // Internals go to the Event log; the user gets a sentence and the last good numbers stay on screen.
                ConsoleLog.Add("✗ GetDashboard failed — " + ex.GetType().Name);
                ConsoleLog.Add("   " + ex.Message);
                ConsoleLog.Status("The dashboard could not be refreshed — the numbers on screen are the last good ones.",
                                  StatusLevel.Error);

                lblLastRefreshed.Text = _model == null
                    ? "Last refreshed — (the refresh failed)"
                    : "Last refreshed " + _model.GeneratedAt.ToString("HH:mm:ss") + " · retry failed";

                AlertBox.Show(
                    "The dashboard could not be refreshed just now. The numbers on screen are the last good ones — please try again in a moment.",
                    MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);

                // One-shot simulation: the recovery is simply pressing Refresh again.
                if (chkSimulateFailure.Checked)
                    chkSimulateFailure.Checked = false;
            }
            finally
            {
                SetLoading(false);
                Application.Update(this);   // we are past an await — push the result over the socket
            }
        }

        /// <summary>
        /// The loading state the lab asks for: the actions that cannot run during a refresh are disabled, the
        /// controls that are about to change show a loader, and the stamp says what is happening.
        /// </summary>
        private void SetLoading(bool loading)
        {
            _refreshing = loading;

            btnRefresh.Enabled = !loading;
            btnRefresh.ShowLoader = loading;
            btnSimulateOversized.Enabled = !loading;
            btnSimulateWrongType.Enabled = !loading;
            uploadReport.Enabled = !loading;
            chartTickets.ShowLoader = loading;
            pdfPreview.ShowLoader = loading;

            if (loading)
                lblLastRefreshed.Text = "Refreshing…";
        }

        /// <summary>Arms / disarms both services, so the failure paths are reproducible from the UI.</summary>
        private void chkSimulateFailure_CheckedChanged(object sender, EventArgs e)
        {
            var armed = chkSimulateFailure.Checked;

            _service.SimulateFailure = armed;
            _documents.SimulateFailure = armed;

            ConsoleLog.Control(chkSimulateFailure.Name);
            ConsoleLog.Add("chkSimulateFailure → dashboard service + document store " + (armed ? "armed to fail" : "back to normal"));
        }

        // ================================================================================================
        // Upload — browser → server, validated on the server, stored through a service
        // ================================================================================================

        /// <summary>
        /// The bytes have arrived. Everything from here on happens on the server: the file the browser sent is
        /// validated again, stored through <see cref="DocumentStore"/>, and the preview is rebuilt from the
        /// <b>stored</b> document — never from what the client claimed.
        /// </summary>
        private void uploadReport_Uploaded(object sender, UploadedEventArgs e)
        {
            ConsoleLog.Control(uploadReport.Name);

            // Wisej.Core.HttpFileCollection is indexed (Count / Get(i)), not enumerable.
            var files = e.Files;
            if (files == null || files.Count == 0)
            {
                RejectUpload("Nothing arrived on the server — please choose the report again.", "uploadReport.Uploaded with no files");
                return;
            }

            var file = files.Get(0);
            ConsoleLog.Add("uploadReport.Uploaded → " + file.FileName + " (" + DocumentStore.FormatSize(file.ContentLength) +
                           ", " + file.ContentType + ") — the bytes are on the server now");

            try
            {
                var result = _documents.Store(file.FileName, file.ContentType, file.InputStream);
                if (!result.Accepted)
                {
                    RejectUpload(result.RejectionReason, "DocumentStore.Validate rejected " + file.FileName);
                    return;
                }

                var stored = result.Document;
                ConsoleLog.Add("✓ stored as " + stored.Id + " in DocumentStore (in memory, " +
                               DocumentStore.FormatSize(stored.SizeBytes) + ") — rebuilding the preview from the stored bytes");

                lblUploadResult.ForeColor = OnTargetColor;
                lblUploadResult.Text = "✓ " + stored.FileName + " (" + DocumentStore.FormatSize(stored.SizeBytes) + ") stored as " +
                                       stored.Id + " at " + stored.ReceivedAt.ToString("HH:mm:ss") +
                                       ". The preview now shows the document the server received.";

                // The model is what feeds the preview, so take a fresh one instead of poking pdfPreview here.
                RefreshDashboard(_service.GetInitialDashboard());

                ConsoleLog.Status("Report " + stored.FileName + " uploaded — preview updated.", StatusLevel.Ok);
                new Toast("Report uploaded and stored.", "icon-check")
                {
                    AutoCloseDelay = 3000,
                    Alignment = ContentAlignment.TopRight
                }.Show();
            }
            catch (Exception ex)
            {
                ConsoleLog.Add("✗ DocumentStore.Store failed — " + ex.GetType().Name);
                ConsoleLog.Add("   " + ex.Message);
                ConsoleLog.Status("The report could not be stored — the previous preview is still shown.", StatusLevel.Error);

                lblUploadResult.ForeColor = ErrorColor;
                lblUploadResult.Text = "The report could not be stored just now. Nothing was changed — please try again.";

                AlertBox.Show("The report could not be stored just now. Nothing on the dashboard was changed — please try again.",
                    MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);

                if (chkSimulateFailure.Checked)
                    chkSimulateFailure.Checked = false;
            }
        }

        /// <summary>
        /// The client-side rejection: the browser refused the file before it was sent (too large for
        /// <c>MaxFileSize</c>, or an upload that failed on the way). The user gets a sentence, the Event log gets
        /// the error type and the framework's message.
        /// </summary>
        private void uploadReport_Error(object sender, UploadErrorEventArgs e)
        {
            ConsoleLog.Control(uploadReport.Name);

            var fileName = e.FileNames != null && e.FileNames.Length > 0 ? e.FileNames[0] : "that file";
            var size = e.FileSizes != null && e.FileSizes.Length > 0 ? DocumentStore.FormatSize(e.FileSizes[0]) : "unknown size";

            ConsoleLog.Add("✗ uploadReport.Error → " + e.ErrorType + " · " + fileName + " (" + size + ") · " + e.Message);

            var reason = e.ErrorType == UploadErrorType.FileTooLarge
                ? fileName + " is " + size + " — reports have to be " + _documents.MaxSizeText + " or smaller."
                : fileName + " could not be uploaded — please check the file and try again.";

            RejectUpload(reason, null);
        }

        /// <summary>
        /// Failure path of the <b>server-side</b> size check. The browser's <c>MaxFileSize</c> normally stops an
        /// oversized file before the server sees it, which is exactly why this button exists: the same rule has to
        /// hold on the server, where nothing the client says is trusted.
        /// </summary>
        private void btnSimulateOversized_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnSimulateOversized.Name);
            ValidateOnServer("sla-report-september-full.pdf", 5L * 1024 * 1024);
        }

        /// <summary>Failure path of the server-side <i>type</i> check (the browser file dialog filters to .pdf).</summary>
        private void btnSimulateWrongType_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnSimulateWrongType.Name);
            ValidateOnServer("tickets-export.xlsx", 240L * 1024);
        }

        private void ValidateOnServer(string fileName, long sizeBytes)
        {
            ConsoleLog.Add("DocumentStore.Validate(\"" + fileName + "\", " + DocumentStore.FormatSize(sizeBytes) +
                           ") — the check that runs on the server whatever the browser allowed");

            var rejection = _documents.Validate(fileName, sizeBytes);
            if (rejection == null)
            {
                ConsoleLog.Add("✓ accepted by the server-side check");
                ConsoleLog.Status("That report would be accepted.", StatusLevel.Ok);
                return;
            }

            RejectUpload(rejection, "rejected on the server before a byte was stored");
        }

        /// <summary>One place for "the file was not accepted": amber status, an AlertBox, and the card says why.</summary>
        private void RejectUpload(string reason, string logLine)
        {
            if (logLine != null)
                ConsoleLog.Add("✗ " + logLine);

            lblUploadResult.ForeColor = ErrorColor;
            lblUploadResult.Text = "Not accepted: " + reason;

            ConsoleLog.Add("   user sees: " + reason);
            ConsoleLog.Status("Upload rejected — " + reason, StatusLevel.Warning);

            AlertBox.Show(reason, MessageBoxIcon.Warning, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
        }

        // ================================================================================================
        // One-time configuration
        // ================================================================================================

        /// <summary>
        /// Every chart decision in one place: bar (a six-month comparison, not a continuous signal), a legend
        /// because there are two series, a fixed 0–100 y-axis so two refreshes can be compared without the scale
        /// moving under the user, named axes, quiet grid lines and the two brand colours. Data does not belong
        /// here — <see cref="RefreshDashboard"/> owns Labels and DataSets.
        /// </summary>
        private void ConfigureChart()
        {
            chartTickets.Options.Title.Display = true;
            chartTickets.Options.Title.Text = "Tickets opened vs closed — last six months";
            chartTickets.Options.Title.FontColor = InkColor;
            chartTickets.Options.Title.Position = HeaderPosition.Top;

            chartTickets.Options.Legend.Display = true;
            chartTickets.Options.Legend.Position = HeaderPosition.Top;
            chartTickets.Options.Legend.Labels.BoxWidth = 12;
            chartTickets.Options.Legend.Labels.Color = InkColor;

            chartTickets.Options.Tooltips.Enabled = true;

            var xAxis = new OptionScalesAxesX
            {
                Id = "months",
                Display = true,
                Type = ScaleType.Category,
                Position = HeaderPosition.Bottom
            };
            xAxis.ScaleLabel.Display = true;
            xAxis.ScaleLabel.LabelString = "Month";
            xAxis.ScaleLabel.FontColor = MutedColor;
            xAxis.GridLines.Display = false;
            xAxis.Ticks.FontColor = MutedColor;

            var yAxis = new OptionScalesAxesY
            {
                Id = "tickets",
                Display = true,
                Type = ScaleType.Linear,
                Position = HeaderPosition.Left
            };
            yAxis.ScaleLabel.Display = true;
            yAxis.ScaleLabel.LabelString = "Tickets";
            yAxis.ScaleLabel.FontColor = MutedColor;
            yAxis.Ticks.BeginAtZero = true;
            yAxis.Ticks.Min = 0;      // a fixed scale: the eye compares months, not axes
            yAxis.Ticks.Max = 100;
            yAxis.Ticks.StepSize = 20;
            yAxis.Ticks.FontColor = MutedColor;
            yAxis.GridLines.Color = new[] { GridColor };

            chartTickets.Options.Scales.xAxes = new[] { xAxis };
            chartTickets.Options.Scales.yAxes = new[] { yAxis };
        }

        /// <summary>
        /// The Upload control mirrors the server's rules instead of inventing its own: one file, the extensions
        /// and the size limit come from <see cref="DocumentStore"/>. The browser filter is a courtesy to the
        /// user; <c>DocumentStore.Validate</c> is the rule.
        /// </summary>
        private void ConfigureUpload()
        {
            uploadReport.AllowMultipleFiles = false;
            uploadReport.AllowedFileTypes = string.Join(",", _documents.AllowedExtensions);
            uploadReport.MaxFileSize = (int)_documents.MaxBytes;
            uploadReport.InvalidMessage = "Only " + string.Join(" / ", _documents.AllowedExtensions) +
                                          " reports up to " + _documents.MaxSizeText + " are accepted.";

            lblUploadHint.Text = string.Join(" / ", _documents.AllowedExtensions) + " only · max " + _documents.MaxSizeText +
                                 " · the browser sends the bytes to the server, which checks them again";
            lblUploadResult.ForeColor = MutedColor;
        }

        private static string DescribeCompletion(DashboardModel model)
        {
            if (model == null)
                return "no data yet";

            return model.CompletionPercent + "% of the " + model.TargetPercent + "% target (" +
                   model.ClosedThisMonth + " of " + DashboardService.MonthlyTarget + " tickets closed in " +
                   model.CurrentMonth + ")";
        }
    }
}
