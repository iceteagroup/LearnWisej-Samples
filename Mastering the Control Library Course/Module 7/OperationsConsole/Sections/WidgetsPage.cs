using System;
using System.Drawing;
using System.Globalization;
using OperationsConsole.Models;
using OperationsConsole.Services;
using OperationsConsole.Shell;
using OperationsConsole.Widgets;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// The <b>Widgets</b> section (Module 7 · Custom Widgets, Extensions, Theming, and Capstone).
    /// <para>
    /// One <see cref="Wisej.Web.Widget"/> named <c>ratingWidget</c> wraps the small
    /// <c>wwwroot/rating.js</c> star-rating library and carries the bridge in both directions:
    /// </para>
    /// <list type="bullet">
    ///   <item><description><b>client → server</b>: a star click raises <c>ratingChanged { value }</c>,
    ///   handled by <see cref="ratingWidget_WidgetEvent"/>, validated by <see cref="RatingService.TryNormalize"/>
    ///   and stored by <see cref="RatingService.Save"/>;</description></item>
    ///   <item><description><b>server → client</b>: <c>Call("setSaved", value)</c> confirms,
    ///   <c>CallAsync("getState")</c> reads the client back, and <c>Options</c> + <c>Update()</c>
    ///   pushes a value in silently.</description></item>
    /// </list>
    /// <para>
    /// The page is deliberately self-contained: it is hosted docked <c>Fill</c> (in the Module 1
    /// <c>contentPanel</c>, in the Module 3 <c>TabControl</c>) and talks to the shell only through
    /// <see cref="ConsoleLog"/>.
    /// </para>
    /// </summary>
    public partial class WidgetsPage : UserControl, ISection
    {
        /// <summary>Every rule about a rating lives here, never in JavaScript.</summary>
        private readonly RatingService _ratings = new RatingService();

        /// <summary>The two themes the demo switches between (both ship with Wisej.NET 4.1).</summary>
        private const string ThemeBootstrap = "Bootstrap-4";
        private const string ThemeMaterial = "Material-3";

        private string _currentTheme = ThemeBootstrap;

        public WidgetsPage()
        {
            InitializeComponent();

            ConfigureRatingWidget();
            ApplyConsoleStyleSheet();

            // The narrow profile is a server-side decision (ClientProfiles.json), pushed into the
            // widget as an option. Unsubscribe on disposal: Application events outlive the page.
            Application.ResponsiveProfileChanged += Application_ResponsiveProfileChanged;
            this.Disposed += WidgetsPage_Disposed;

            ShowRating(_ratings.Current, "page opened");
            ConsoleLog.Add("WidgetsPage ready · InitScript from " + RatingInitScript.LastSource);
            ConsoleLog.Add("ratingWidget packages: rating.css → rating.js (stylesheet first, so the first paint is themed)");
        }

        /// <inheritdoc/>
        public string Title => "Widgets";

        /// <inheritdoc/>
        public void RefreshSection()
        {
            RatingModel model = _ratings.Reload();
            PushOptionsToWidget(model);
            ShowRating(model, "RefreshSection()");
            ConsoleLog.Add("WidgetsPage.RefreshSection() → re-read the rating and re-pushed Options to ratingWidget");
        }

        // ============================================================================================
        // Wiring the Widget: packages (designer), adapter, options, events
        // ============================================================================================

        /// <summary>
        /// Finishes what the designer started. <c>Packages</c>, <c>WiredEvents</c>, the accessible
        /// label and the <c>WidgetEvent</c> handler are declared in <c>WidgetsPage.Designer.cs</c>;
        /// the two things that need code — the adapter source and the initial <c>Options</c> — are here.
        /// </summary>
        private void ConfigureRatingWidget()
        {
            // The InitScript comes from the embedded resource, not from Default.html: a <script> in
            // the startup HTML runs before the Wisej.NET widget exists (module reading, "Debugging").
            ratingWidget.InitScript = RatingInitScript.Load();

            // State in. Options is a dynamic object; these are all first-level fields, so a later
            // assignment + Update() reaches the client's update(options, old).
            PushOptionsToWidget(_ratings.Current, updateClient: false);
        }

        /// <summary>Copies the server-side model into <c>ratingWidget.Options</c>.</summary>
        /// <param name="updateClient">
        /// True after the widget exists: <c>Update()</c> runs <c>update(options, old)</c> in the
        /// browser, which applies the value <b>silently</b> so a server-driven change never bounces
        /// back to the server as a <c>ratingChanged</c> event.
        /// </param>
        private void PushOptionsToWidget(RatingModel model, bool updateClient = true)
        {
            dynamic options = ratingWidget.Options;
            options.value = model.Value;
            options.max = model.Max;
            options.label = model.CustomerName;
            options.saved = model.IsSaved;
            options.theme = ThemeKey(_currentTheme);
            options.profile = Application.ActiveProfile?.Name ?? "Desktop";
            options.name = ratingWidget.Name;

            if (updateClient)
                ratingWidget.Update();
        }

        /// <summary>
        /// The application-level override layer the lab mentions next to packaged CSS: the
        /// <see cref="Wisej.Web.StyleSheet"/> extender injects console-specific CSS without touching
        /// <c>rating.css</c> (which belongs to the widget) or <c>rating.js</c> (which must stay
        /// colour-free). Everything here is still a CSS custom property, never a hard-coded colour
        /// inside JavaScript.
        /// </summary>
        private void ApplyConsoleStyleSheet()
        {
            styleSheet.Styles =
                "/* Operations Console overrides for the packaged rating widget. */\n" +
                ".opc-console-rating .opc-rating { --opc-rating-radius: 10px; }\n" +
                ".opc-console-rating .opc-rating__label { letter-spacing: .01em; }\n";

            // Tags the widget's element so the rules above only reach this console's copy.
            styleSheet.SetCssClass(ratingWidget, "opc-console-rating");
        }

        // ============================================================================================
        // client → server: the one wired event
        // ============================================================================================

        /// <summary>
        /// Handles every event the client raises on this widget.
        /// <para>
        /// The order matters and is the lesson of the module: check the <b>type</b>, log what
        /// arrived, <b>validate and normalise</b> the payload before anything else touches it, then
        /// call the service, then confirm back to the browser and tell the user.
        /// </para>
        /// </summary>
        private void ratingWidget_WidgetEvent(object sender, WidgetEventArgs e)
        {
            ConsoleLog.Control(ratingWidget.Name);

            // 1. A widget can raise several events; only the contract's own is handled here.
            if (e.Type != "ratingChanged")
            {
                LogIn(e.Type, "(not part of the contract)");
                ConsoleLog.Add("   ignored — WiredEvents declares ratingChanged only");
                return;
            }

            object raw = ReadValueField(e.Data);
            LogIn("ratingChanged", PayloadJson(raw));

            // 2. The payload came from the browser. This is the line that stops a hand-edited
            //    fireWidgetEvent: nothing below it runs until the value is a real 1..5 rating.
            if (!_ratings.TryNormalize(raw, out int value, out string error))
            {
                RejectPayload(error);
                return;
            }

            // 3. The rule passed — hand it to the service.
            SaveRating(value);
        }

        /// <summary>Stores the value and confirms back to the widget. The only place <c>Save</c> is called.</summary>
        private void SaveRating(int value)
        {
            try
            {
                RatingModel model = _ratings.Save(value);

                // Server → client: the confirmation half of the bridge. Call() is one-way and is
                // flushed with this response — the widget switches to its .saved state.
                ratingWidget.Call("setSaved", value);
                LogOut("setSaved", value.ToString(CultureInfo.InvariantCulture));

                ShowRating(model, "saved");
                ConsoleLog.Record(model.CustomerId);
                ConsoleLog.Status("Rating " + value + "/" + model.Max + " saved for " + model.CustomerName + ".", StatusLevel.Ok);

                new Toast("Rating " + value + " saved for " + model.CustomerName + ".", "icon-check")
                {
                    AutoCloseDelay = 3000,
                    Alignment = ContentAlignment.TopRight
                }.Show();
            }
            catch (Exception ex)
            {
                // The failure path: friendly words for the user, details for the Event log, and the
                // widget stays editable — clearing the saved state is the honest thing to show.
                _ratings.MarkUnsaved(value);

                ratingWidget.Call("ratingClearSaved");
                LogOut("ratingClearSaved", "(the value was not stored)");

                ConsoleLog.Add("✗ RatingService.Save(" + value + ") failed — " + ex.GetType().Name);
                ConsoleLog.Add("   " + ex.Message);
                ConsoleLog.Status("The rating could not be saved — the ratings service is not answering. Try again.", StatusLevel.Error);

                lblSavedState.Text = "Not saved: the ratings service refused the write. The stars are still editable — try again.";
                lblSavedState.ForeColor = Color.FromArgb(224, 86, 59);

                AlertBox.Show(
                    "The rating could not be saved. Your click was not lost — try again in a moment.",
                    MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
            }
        }

        /// <summary>The rejection path: nothing is stored, the user is told why, the widget stays editable.</summary>
        private void RejectPayload(string error)
        {
            ratingWidget.Call("ratingSetBusy", false);
            LogOut("ratingSetBusy", "false (rejected, nothing stored)");

            ConsoleLog.Add("✗ payload rejected — " + error);
            ConsoleLog.Status("That rating was rejected: " + error, StatusLevel.Warning);

            lblSavedState.Text = "Rejected: " + error + " Nothing was saved.";
            lblSavedState.ForeColor = Color.FromArgb(232, 161, 60);

            AlertBox.Show("That rating was not accepted. " + error,
                MessageBoxIcon.Warning, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
        }

        // ============================================================================================
        // server → client: the command row
        // ============================================================================================

        /// <summary>Options + <c>Update()</c>: the client applies the value silently, no event comes back.</summary>
        private void btnPushFromServer_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnPushFromServer.Name);

            const int pushed = RatingService.MaxRating;
            _ratings.MarkUnsaved(pushed);
            PushOptionsToWidget(_ratings.Current);
            LogOut("Options.value", pushed + " (via update(options, old) — applied silently)");

            lblSavedState.Text = "The server pushed " + pushed + "/" + RatingService.MaxRating +
                                 " into the widget. It is shown but NOT saved — click a star to save.";
            lblSavedState.ForeColor = Color.FromArgb(90, 107, 125);
            ConsoleLog.Status("Pushed " + pushed + " into the widget through Options — nothing was stored.", StatusLevel.Ok);
        }

        /// <summary><c>CallAsync</c>: the server-to-client call that comes back with a value.</summary>
        private async void btnReadClientState_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnReadClientState.Name);

            if (!ratingWidget.IsLoaded)
            {
                ConsoleLog.Add("→ .NET→JS getState — skipped, the client widget has not initialised yet");
                ConsoleLog.Status("The widget is not on screen yet — open the Widgets section and try again.", StatusLevel.Warning);
                return;
            }

            LogOut("getState()", "(CallAsync — waiting for the browser)");

            try
            {
                dynamic state = await ratingWidget.CallAsync("getState");

                string text = state == null
                    ? "(the client returned nothing)"
                    : "value " + state.value + " · savedValue " + state.savedValue + " · saved " + state.saved
                      + " · max " + state.max + " · theme " + state.theme + " · narrow " + state.narrow;

                lblClientState.Text = "getState()  " + text;
                ConsoleLog.Add("← JS→.NET getState → " + text);
                ConsoleLog.Status("Read the widget state straight out of the browser.", StatusLevel.Ok);
            }
            catch (Exception ex)
            {
                ConsoleLog.Add("✗ CallAsync(\"getState\") failed — " + ex.GetType().Name + ": " + ex.Message);
                ConsoleLog.Status("Could not read the client state.", StatusLevel.Error);
            }
        }

        /// <summary>Proves <c>init</c> is safe to run again (the re-initialisation pitfall).</summary>
        private void btnReinit_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnReinit.Name);

            ratingWidget.Call("reinit");
            LogOut("reinit()", "(init runs again; the adapter tears the old library down first)");

            ConsoleLog.Status("The client widget was re-created — same value, no duplicated DOM, console clean.", StatusLevel.Ok);
        }

        /// <summary>Theme switch: the widget follows through <c>rating.css</c>, not through colours in JavaScript.</summary>
        private void btnTheme_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnTheme.Name);

            _currentTheme = _currentTheme == ThemeBootstrap ? ThemeMaterial : ThemeBootstrap;
            Application.LoadTheme(_currentTheme);

            btnTheme.Text = "Theme → " + (_currentTheme == ThemeBootstrap ? ThemeMaterial : ThemeBootstrap);

            // The Wisej theme restyles the native controls; the widget follows because the server
            // tells the adapter which variant of the packaged CSS to use.
            PushOptionsToWidget(_ratings.Current);
            LogOut("Options.theme", ThemeKey(_currentTheme) + " (rating.css variant — no colour crosses the bridge)");

            ConsoleLog.Add("Application.LoadTheme(\"" + _currentTheme + "\") — native controls and the widget restyle together");
            ConsoleLog.Status("Theme: " + _currentTheme + ". The widget followed without a single colour in rating.js.", StatusLevel.Ok);
        }

        /// <summary>Failure path 1: a payload the widget itself would never produce.</summary>
        private void btnSendMalformed_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnSendMalformed.Name);
            ConsoleLog.Add("simulating developer tools: the client will fire ratingChanged with a string");

            // The client function defers with setTimeout(…, 0): this call arrives from the server,
            // and a fireWidgetEvent raised synchronously inside that response would be dropped.
            ratingWidget.Call("sendRawPayload", "seven");
            LogOut("sendRawPayload", "\"seven\"");
        }

        /// <summary>Failure path 2: a number outside the scale.</summary>
        private void btnSendOutOfRange_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnSendOutOfRange.Name);
            ConsoleLog.Add("simulating developer tools: the client will fire ratingChanged with 9");

            ratingWidget.Call("sendRawPayload", 9);
            LogOut("sendRawPayload", "9");
        }

        /// <summary>Failure path 3: the store refuses the write.</summary>
        private void chkSimulateServiceFailure_CheckedChanged(object sender, EventArgs e)
        {
            ConsoleLog.Control(chkSimulateServiceFailure.Name);
            _ratings.SimulateFailure = chkSimulateServiceFailure.Checked;

            ConsoleLog.Add(_ratings.SimulateFailure
                ? "RatingService.SimulateFailure = true — the next Save() throws"
                : "RatingService.SimulateFailure = false — saving works again (recovery)");

            ConsoleLog.Status(_ratings.SimulateFailure
                ? "The ratings service will refuse the next save. Click a star to see the failure path."
                : "The ratings service is healthy again. Click a star to save.",
                _ratings.SimulateFailure ? StatusLevel.Warning : StatusLevel.Ok);
        }

        // ============================================================================================
        // Responsive profile
        // ============================================================================================

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            if (this.IsDisposed) return;

            string profile = Application.ActiveProfile?.Name ?? "Desktop";
            PushOptionsToWidget(_ratings.Current);
            LogOut("Options.profile", profile + (profile == "Phone" ? " → .is-narrow (smaller stars)" : ""));
            ConsoleLog.Add("profile changed → " + profile + " (" + Application.Browser.Size.Width + " px wide)");
        }

        private void WidgetsPage_Disposed(object sender, EventArgs e)
        {
            Application.ResponsiveProfileChanged -= Application_ResponsiveProfileChanged;
        }

        // ============================================================================================
        // Small helpers — the section page stays readable, the handlers stay thin
        // ============================================================================================

        /// <summary>Reflects the server-side model in the card. The browser never writes these labels.</summary>
        private void ShowRating(RatingModel model, string reason)
        {
            lblCustomer.Text = model.CustomerName + " · customer " + model.CustomerId;
            lblSavedState.Text = model.Describe();
            lblSavedState.ForeColor = model.IsSaved
                ? Color.FromArgb(31, 157, 87)
                : Color.FromArgb(90, 107, 125);
            lblWidgetHint.Text = "ratingWidget · Packages: rating.css → rating.js · Options.max = " + model.Max +
                                 " · WiredEvents: ratingChanged · theme " + ThemeKey(_currentTheme) +
                                 " · profile " + (Application.ActiveProfile?.Name ?? "Desktop");

            if (reason == "saved")
                ConsoleLog.Add("• RatingService.Save(" + model.SavedValue + ") → stored for " + model.CustomerId +
                               " at " + (model.SavedAt.HasValue ? model.SavedAt.Value.ToString("HH:mm:ss") : "—"));
        }

        /// <summary>Event log + bridge card, client → server. Format matches the Application Integration samples.</summary>
        private void LogIn(string name, string payload)
        {
            string line = "← JS→.NET " + name + " " + payload;
            ConsoleLog.Add(line);
            lblLastIn.Text = line;
        }

        /// <summary>Event log + bridge card, server → client.</summary>
        private void LogOut(string name, string payload)
        {
            string line = "→ .NET→JS " + name + " " + payload;
            ConsoleLog.Add(line);
            lblLastOut.Text = line;
        }

        /// <summary>Reads the <c>value</c> field of the payload without ever letting a bad shape throw.</summary>
        private static object ReadValueField(object data)
        {
            if (data == null) return null;
            try { return ((dynamic)data).value; }
            catch (Exception) { return null; }
        }

        /// <summary>Renders the payload the way it arrived, so the Event log shows the real shape.</summary>
        private static string PayloadJson(object raw)
        {
            if (raw == null) return "{\"value\":null}";
            if (raw is string s) return "{\"value\":\"" + s + "\"}";
            return "{\"value\":" + Convert.ToString(raw, CultureInfo.InvariantCulture) + "}";
        }

        /// <summary>Maps a Wisej theme name to the CSS variant class rating.css defines.</summary>
        private static string ThemeKey(string themeName)
            => themeName == ThemeMaterial ? "material" : "bootstrap";
    }
}
