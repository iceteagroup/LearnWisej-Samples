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
    /// The <b>Dashboard</b> section. It answers one question — "are we on track for this month's ticket target?" —
    /// with a ChartJS bar chart for the pattern, a <c>ProgressBar</c> for the single status value, a <c>PdfViewer</c>
    /// and an <c>HtmlPanel</c> for content, an <c>Upload</c> for new reports and a reserved map slot.
    /// <see cref="RefreshDashboard"/> is the only method that writes to those controls, and it reads one
    /// <see cref="DashboardModel"/>.
    /// </summary>
    public partial class DashboardPage : UserControl, ISection
    {
        private static readonly Color OpenedColor = Color.FromArgb(21, 101, 216);
        private static readonly Color ClosedColor = Color.FromArgb(31, 157, 87);
        private static readonly Color OnTargetColor = Color.FromArgb(31, 157, 87);
        private static readonly Color BelowTargetColor = Color.FromArgb(232, 161, 60);
        private static readonly Color InkColor = Color.FromArgb(13, 27, 42);
        private static readonly Color GridColor = Color.FromArgb(228, 234, 241);
        private static readonly Color MutedColor = Color.FromArgb(90, 107, 125);
        private static readonly Color ErrorColor = Color.FromArgb(224, 86, 59);

        private readonly DocumentStore _documents = new DocumentStore();
        private readonly DashboardService _service;

        /// <summary>The last model that reached the screen — what stays visible when a refresh fails.</summary>
        private DashboardModel _model;

        private bool _refreshing;

        public DashboardPage()
        {
            InitializeComponent();

            _service = new DashboardService(_documents);

            ConfigureChart();
            ConfigureUpload();

            htmlPreview.Html = PreviewHtml.Empty("Loading…");
        }

        /// <inheritdoc/>
        public string Title => "Dashboard";

        /// <summary>The shell's Refresh runs the page's own Refresh — one refresh path.</summary>
        public void RefreshSection() => btnRefresh_Click(btnRefresh, EventArgs.Empty);

        /// <summary>The dashboard never opens empty: the first paint is an immediate snapshot.</summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RefreshDashboard(_service.GetInitialDashboard());
            ShellStatus.Show("Dashboard ready — " + DescribeCompletion(_model) + ".", StatusLevel.Ok);
        }

        // ================================================================================================
        // One refresh, one model
        // ================================================================================================

        /// <summary>Chart, indicator, preview and stamp, all from one <see cref="DashboardModel"/>.</summary>
        private void RefreshDashboard(DashboardModel model)
        {
            if (model == null)
                return;

            var firstFill = _model == null;
            _model = model;

            // the pattern — labels and DataSets together, in one place
            chartTickets.Labels = model.Months.ToArray();
            chartTickets.DataSets.Clear();
            chartTickets.DataSets.Add(BuildSeries("Opened", model.Opened, OpenedColor));
            chartTickets.DataSets.Add(BuildSeries("Closed", model.Closed, ClosedColor));

            if (!firstFill)
                chartTickets.UpdateData(400);

            // the single status value
            progressCompletion.Value = Math.Max(progressCompletion.Minimum,
                                       Math.Min(progressCompletion.Maximum, model.CompletionPercent));
            progressCompletion.BarColor = model.OnTarget ? OnTargetColor : BelowTargetColor;

            // the content
            ShowPreview(model.PreviewDocument);
            htmlPreview.Html = PreviewHtml.Summary(model, DashboardService.MonthlyTarget);

            // how fresh all of the above is
            lblLastRefreshed.Text = "Last refreshed " + model.GeneratedAt.ToString("HH:mm:ss");

            ShellStatus.Record(model.PreviewDocument.Id);
        }

        /// <summary>One series of the chart, with one deliberate colour per point.</summary>
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
        /// Points the <c>PdfViewer</c> at the document the model references: the report shipped in <c>wwwroot/</c>
        /// (a URL) or the last uploaded report (bytes <see cref="DocumentStore"/> holds, streamed to the viewer).
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
                                         " · uploaded " + document.ReceivedAt.ToString("HH:mm:ss");
            }
            else
            {
                pdfPreview.PdfStream = null;
                pdfPreview.PdfSource = document.Url;
                lblPreviewCaption.Text = document.Title;
            }

            lblPreviewCaption.ForeColor = MutedColor;
        }

        // ================================================================================================
        // Refresh
        // ================================================================================================

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            ShellStatus.Control(btnRefresh.Name);
            await RefreshFromServiceAsync();
        }

        private async Task RefreshFromServiceAsync()
        {
            if (_refreshing)
                return;

            SetLoading(true);
            ShellStatus.Show("Refreshing the dashboard…", StatusLevel.Ok);

            try
            {
                // An exception escaping a StartTask lambda is reported in Wisej.NET's error dialog even when the
                // awaited task is caught, so the lambda returns the error and it is re-thrown here.
                var outcome = await Application.StartTask(() =>
                {
                    try { return (Model: _service.GetDashboard(), Error: (Exception)null); }
                    catch (Exception ex) { return (Model: (DashboardModel)null, Error: ex); }
                });
                if (outcome.Error != null)
                    throw outcome.Error;
                var model = outcome.Model;

                RefreshDashboard(model);

                ShellStatus.Show("Dashboard refreshed at " + model.GeneratedAt.ToString("HH:mm:ss") + " — " +
                                 DescribeCompletion(model) + ".", model.OnTarget ? StatusLevel.Ok : StatusLevel.Warning);
                new Toast("Dashboard refreshed.", "icon-check")
                {
                    AutoCloseDelay = 2500,
                    Alignment = ContentAlignment.TopRight
                }.Show();
            }
            catch (Exception)
            {
                ShellStatus.Show("The dashboard could not be refreshed — the numbers on screen are the last good ones.",
                                 StatusLevel.Error);

                lblLastRefreshed.Text = _model == null
                    ? "Last refreshed —"
                    : "Last refreshed " + _model.GeneratedAt.ToString("HH:mm:ss") + " · refresh failed";

                AlertBox.Show(
                    "The dashboard could not be refreshed just now. The numbers on screen are the last good ones — please try again in a moment.",
                    MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
            }
            finally
            {
                SetLoading(false);
                Application.Update(this);
            }
        }

        /// <summary>The loading state: Refresh and Upload disabled, loaders on the controls that are about to change.</summary>
        private void SetLoading(bool loading)
        {
            _refreshing = loading;

            btnRefresh.Enabled = !loading;
            btnRefresh.ShowLoader = loading;
            uploadReport.Enabled = !loading;
            chartTickets.ShowLoader = loading;
            pdfPreview.ShowLoader = loading;

            if (loading)
                lblLastRefreshed.Text = "Refreshing…";
        }

        private void chkSimulateFailure_CheckedChanged(object sender, EventArgs e)
        {
            _service.SimulateFailure = chkSimulateFailure.Checked;
            _documents.SimulateFailure = chkSimulateFailure.Checked;
        }

        // ================================================================================================
        // Upload — browser → server, validated on the server, stored through a service
        // ================================================================================================

        /// <summary>
        /// The bytes have arrived: validate them again on the server, store them through <see cref="DocumentStore"/>,
        /// and rebuild the preview from the stored document.
        /// </summary>
        private void uploadReport_Uploaded(object sender, UploadedEventArgs e)
        {
            ShellStatus.Control(uploadReport.Name);

            // Wisej.Core.HttpFileCollection is indexed (Count / Get(i)), not enumerable.
            var files = e.Files;
            if (files == null || files.Count == 0)
            {
                RejectUpload("Nothing arrived on the server — please choose the report again.");
                return;
            }

            var file = files.Get(0);

            try
            {
                var result = _documents.Store(file.FileName, file.ContentType, file.InputStream);
                if (!result.Accepted)
                {
                    RejectUpload(result.RejectionReason);
                    return;
                }

                var stored = result.Document;

                lblUploadResult.ForeColor = OnTargetColor;
                lblUploadResult.Text = "✓ " + stored.FileName + " (" + DocumentStore.FormatSize(stored.SizeBytes) +
                                       ") uploaded at " + stored.ReceivedAt.ToString("HH:mm:ss") + ".";

                RefreshDashboard(_service.GetInitialDashboard());

                ShellStatus.Show("Report " + stored.FileName + " uploaded — preview updated.", StatusLevel.Ok);
                new Toast("Report uploaded and stored.", "icon-check")
                {
                    AutoCloseDelay = 3000,
                    Alignment = ContentAlignment.TopRight
                }.Show();
            }
            catch (Exception)
            {
                ShellStatus.Show("The report could not be stored — the previous preview is still shown.", StatusLevel.Error);

                lblUploadResult.ForeColor = ErrorColor;
                lblUploadResult.Text = "The report could not be stored just now. Nothing was changed — please try again.";

                AlertBox.Show("The report could not be stored just now. Nothing on the dashboard was changed — please try again.",
                    MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
            }
        }

        /// <summary>The browser refused the file before sending it (too large, wrong type, failed upload).</summary>
        private void uploadReport_Error(object sender, UploadErrorEventArgs e)
        {
            ShellStatus.Control(uploadReport.Name);

            var fileName = e.FileNames != null && e.FileNames.Length > 0 ? e.FileNames[0] : "that file";
            var size = e.FileSizes != null && e.FileSizes.Length > 0 ? DocumentStore.FormatSize(e.FileSizes[0]) : "unknown size";

            var reason = e.ErrorType == UploadErrorType.FileTooLarge
                ? fileName + " is " + size + " — reports have to be " + _documents.MaxSizeText + " or smaller."
                : fileName + " could not be uploaded — only " + string.Join(" / ", _documents.AllowedExtensions) +
                  " reports up to " + _documents.MaxSizeText + " are accepted.";

            RejectUpload(reason);
        }

        /// <summary>The file was not accepted: amber status, an AlertBox, and the card says why.</summary>
        private void RejectUpload(string reason)
        {
            lblUploadResult.ForeColor = ErrorColor;
            lblUploadResult.Text = "Not accepted: " + reason;

            ShellStatus.Show("Upload rejected — " + reason, StatusLevel.Warning);

            AlertBox.Show(reason, MessageBoxIcon.Warning, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
        }

        // ================================================================================================
        // One-time configuration
        // ================================================================================================

        /// <summary>
        /// Every chart decision in one place: bar, a legend for the two series, a fixed 0–100 y-axis so two refreshes
        /// stay comparable, named axes, quiet grid lines and the two colours. Data belongs to <see cref="RefreshDashboard"/>.
        /// </summary>
        private void ConfigureChart()
        {
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
            yAxis.Ticks.Min = 0;
            yAxis.Ticks.Max = 100;
            yAxis.Ticks.StepSize = 20;
            yAxis.Ticks.FontColor = MutedColor;
            yAxis.GridLines.Color = new[] { GridColor };

            chartTickets.Options.Scales.xAxes = new[] { xAxis };
            chartTickets.Options.Scales.yAxes = new[] { yAxis };
        }

        /// <summary>The Upload control mirrors the server's rules: one file, the extensions and the size from <see cref="DocumentStore"/>.</summary>
        private void ConfigureUpload()
        {
            uploadReport.AllowMultipleFiles = false;
            uploadReport.AllowedFileTypes = string.Join(",", _documents.AllowedExtensions);
            uploadReport.MaxFileSize = (int)_documents.MaxBytes;
            uploadReport.InvalidMessage = "Only " + string.Join(" / ", _documents.AllowedExtensions) +
                                          " reports up to " + _documents.MaxSizeText + " are accepted.";

            lblUploadHint.Text = string.Join(" / ", _documents.AllowedExtensions) + " only · max " + _documents.MaxSizeText;
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
