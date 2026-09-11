using System;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using OperationsConsole.Models;
using OperationsConsole.Services;
using OperationsConsole.Shell;
using OperationsConsole.Widgets;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// The <b>Widgets</b> section: a <see cref="Wisej.Web.Widget"/> named <c>ratingWidget</c> wraps the
    /// <c>wwwroot/rating.js</c> star-rating library. A star click raises <c>ratingChanged { value }</c>, handled by
    /// <see cref="ratingWidget_WidgetEvent"/>, validated and stored by <see cref="RatingService"/>, and confirmed
    /// back with <c>CallAsync("setSaved", value)</c>.
    /// </summary>
    public partial class WidgetsPage : UserControl, ISection
    {
        /// <summary>Every rule about a rating lives here, never in JavaScript.</summary>
        private readonly RatingService _ratings = new RatingService();

        public WidgetsPage()
        {
            InitializeComponent();

            ConfigureRatingWidget();
            ApplyConsoleStyleSheet();

            // Application events outlive the page: unsubscribe on disposal.
            Application.ResponsiveProfileChanged += Application_ResponsiveProfileChanged;
            this.Disposed += WidgetsPage_Disposed;

            ShowRating(_ratings.Current);
        }

        /// <inheritdoc/>
        public string Title => "Widgets";

        /// <inheritdoc/>
        public void RefreshSection()
        {
            RatingModel model = _ratings.Reload();
            PushOptionsToWidget(model);
            ShowRating(model);
        }

        // ============================================================================================
        // Wiring the Widget: packages and WiredEvents (designer), InitScript and Options (here)
        // ============================================================================================

        private void ConfigureRatingWidget()
        {
            // From the embedded resource, not from Default.html: a script in the startup HTML runs before the
            // Wisej.NET widget exists.
            ratingWidget.InitScript = RatingInitScript.Load();

            PushOptionsToWidget(_ratings.Current, updateClient: false);
        }

        /// <summary>
        /// Copies the server-side model into <c>ratingWidget.Options</c>. With <paramref name="updateClient"/>,
        /// <c>Update()</c> runs <c>update(options, old)</c> in the browser, which applies the value silently.
        /// </summary>
        private void PushOptionsToWidget(RatingModel model, bool updateClient = true)
        {
            dynamic options = ratingWidget.Options;
            options.value = model.Value;
            options.max = model.Max;
            options.label = model.CustomerName;
            options.saved = model.IsSaved;
            options.theme = "bootstrap";
            options.profile = Application.ActiveProfile?.Name ?? "Desktop";
            options.name = ratingWidget.Name;

            if (updateClient)
                ratingWidget.Update();
        }

        /// <summary>
        /// The application-level override layer on top of the packaged <c>rating.css</c>: the
        /// <see cref="Wisej.Web.StyleSheet"/> extender adds console-specific CSS without touching the widget's files.
        /// </summary>
        private void ApplyConsoleStyleSheet()
        {
            styleSheet.Styles =
                ".opc-console-rating .opc-rating { --opc-rating-radius: 10px; }\n" +
                ".opc-console-rating .opc-rating__label { letter-spacing: .01em; }\n";

            styleSheet.SetCssClass(ratingWidget, "opc-console-rating");
        }

        // ============================================================================================
        // client → server
        // ============================================================================================

        /// <summary>Check the type, validate and normalise the payload, save it, confirm back to the browser.</summary>
        private async void ratingWidget_WidgetEvent(object sender, WidgetEventArgs e)
        {
            ShellStatus.Control(ratingWidget.Name);

            if (e.Type != "ratingChanged")
                return;

            object raw = ReadValueField(e.Data);
            Trace("← JS→.NET", "ratingChanged " + PayloadJson(raw));

            // The payload came from the browser: nothing below runs until it is a whole number from 1 to 5.
            if (!_ratings.TryNormalize(raw, out int value, out string error))
            {
                RejectPayload(error);
                return;
            }

            await SaveRatingAsync(value);
        }

        private async Task SaveRatingAsync(int value)
        {
            RatingModel model;
            try
            {
                model = _ratings.Save(value);
            }
            catch (Exception)
            {
                ShowSaveFailure(value);
                return;
            }

            ShowRating(model);
            ShellStatus.Record(model.CustomerId);
            ShellStatus.Show("Rating " + value + "/" + model.Max + " saved for " + model.CustomerName + ".", StatusLevel.Ok);

            new Toast("Rating " + value + " saved for " + model.CustomerName + ".", "icon-check")
            {
                AutoCloseDelay = 3000,
                Alignment = ContentAlignment.TopRight
            }.Show();

            // server → client: the widget switches to its saved state
            Trace("→ .NET→JS", "setSaved " + value.ToString(CultureInfo.InvariantCulture));
            await ratingWidget.CallAsync("setSaved", value);
        }

        /// <summary>The save failed: a friendly error, nothing stored, and the widget stays editable.</summary>
        private void ShowSaveFailure(int value)
        {
            _ratings.MarkUnsaved(value);

            ratingWidget.Call("ratingClearSaved");
            Trace("→ .NET→JS", "ratingClearSaved");

            ShellStatus.Show("The rating could not be saved — the ratings service is not answering. Try again.", StatusLevel.Error);

            lblSavedState.Text = "Not saved: the ratings service is not answering. The stars are still editable — try again.";
            lblSavedState.ForeColor = Color.FromArgb(224, 86, 59);

            AlertBox.Show(
                "The rating could not be saved. Your click was not lost — try again in a moment.",
                MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
        }

        /// <summary>The payload was rejected: nothing is stored, the user is told why, the widget stays editable.</summary>
        private void RejectPayload(string error)
        {
            ratingWidget.Call("ratingSetBusy", false);
            Trace("→ .NET→JS", "ratingSetBusy false");

            ShellStatus.Show("That rating was rejected: " + error, StatusLevel.Warning);

            lblSavedState.Text = "Rejected: " + error + " Nothing was saved.";
            lblSavedState.ForeColor = Color.FromArgb(232, 161, 60);

            AlertBox.Show("That rating was not accepted. " + error,
                MessageBoxIcon.Warning, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
        }

        private void chkSimulateServiceFailure_CheckedChanged(object sender, EventArgs e)
        {
            _ratings.SimulateFailure = chkSimulateServiceFailure.Checked;
        }

        // ============================================================================================
        // Responsive profile
        // ============================================================================================

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            if (this.IsDisposed) return;
            PushOptionsToWidget(_ratings.Current);
        }

        private void WidgetsPage_Disposed(object sender, EventArgs e)
        {
            Application.ResponsiveProfileChanged -= Application_ResponsiveProfileChanged;
        }

        // ============================================================================================
        // Helpers
        // ============================================================================================

        /// <summary>Reflects the server-side model in the card. The browser never writes these labels.</summary>
        private void ShowRating(RatingModel model)
        {
            lblCustomer.Text = model.CustomerName + " · customer " + model.CustomerId;
            lblSavedState.Text = model.Describe();
            lblSavedState.ForeColor = model.IsSaved
                ? Color.FromArgb(31, 157, 87)
                : Color.FromArgb(90, 107, 125);
        }

        /// <summary>One line of the message trace, in either direction.</summary>
        private void Trace(string direction, string message)
        {
            lstMessageTrace.Items.Add(DateTime.Now.ToString("HH:mm:ss") + "  " + direction + " " + message);
            lstMessageTrace.SelectedIndex = lstMessageTrace.Items.Count - 1;
        }

        /// <summary>Reads the <c>value</c> field of the payload without letting a bad shape throw.</summary>
        private static object ReadValueField(object data)
        {
            if (data == null) return null;
            try { return ((dynamic)data).value; }
            catch (Exception) { return null; }
        }

        /// <summary>The payload the way it arrived.</summary>
        private static string PayloadJson(object raw)
        {
            if (raw == null) return "{\"value\":null}";
            if (raw is string s) return "{\"value\":\"" + s + "\"}";
            return "{\"value\":" + Convert.ToString(raw, CultureInfo.InvariantCulture) + "}";
        }
    }
}
