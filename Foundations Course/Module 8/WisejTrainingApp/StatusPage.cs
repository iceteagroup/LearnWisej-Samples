using System;
using System.Globalization;
using System.IO;
using Wisej.Web;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp
{
    /// <summary>
    /// Status page — Module 8 lab (Integrating a JavaScript widget).
    ///
    /// Left, top:    "Status widget" — widStatus, a Wisej.Web.Widget whose visual is Widgets/statusGauge.js
    ///               (InitScript) styled by Widgets/statusGauge.css (Packages) and fed by Options.
    /// Left, below:  "Server-side data" — native labels with what C# holds right now, the exact
    ///               Options object last sent, and the list of what is never sent to the browser.
    /// Right:        the event log — every server-side decision, with a timestamp.
    /// Bottom bar:   Refresh (success), Set Healthy / Warning / Critical (the C# rule recolours the
    ///               widget), Simulate service error (failure + recovery), theme switch (lab step 10).
    ///
    /// C# owns the data and the rules (StatusService); JavaScript owns the drawing (statusGauge.js).
    /// The bridge between them is three members of one control: Packages, InitScript, Options.
    /// </summary>
    public partial class StatusPage : Page
    {
        // One service per page = per user session. An instance field, never static.
        private readonly StatusService statusService = new StatusService();

        private bool _usingMaterial;

        public StatusPage()
        {
            // InitializeComponent() builds every control from StatusPage.Designer.cs — the Designer owns that file.
            InitializeComponent();
        }

        #region Lab steps 3–5: connect the JavaScript file to C#

        private void StatusPage_Load(object sender, EventArgs e)
        {
            // 1. Stylesheet (and any third-party scripts) load in list order, once per session.
            widStatus.Packages.Add(new Widget.Package
            {
                Name = "statusGauge-css",
                Source = "Widgets/statusGauge.css"
            });

            // 2. The JavaScript file IS the widget: load it as the InitScript.
            //    (You can also paste the file into the InitScript property in the designer.)
            widStatus.InitScript = File.ReadAllText(Application.MapPath("Widgets/statusGauge.js"));

            // 3. First options — safe display values only, prepared by a C# service.
            var data = statusService.GetStatus();
            widStatus.Options = new { percent = data.Percent, label = data.Label, status = data.Status };

            // 4. Listen only for the event we wired in the JS file.
            widStatus.WidgetEvent += widStatus_WidgetEvent;

            // The native card shows the same values, so what was sent can be verified by eye.
            ShowServerData(data);
            lblNeverSent.Text = string.Join("\n", statusService.DescribeServerOnly());

            AddLog("StatusPage_Load → Packages += statusGauge-css (Widgets/statusGauge.css)");
            AddLog($"StatusPage_Load → InitScript = File.ReadAllText(Application.MapPath(\"Widgets/statusGauge.js\")) → {widStatus.InitScript.Length} chars");
            AddLog($"StatusPage_Load → Options = {OptionsText(data)} → WidgetEvent += widStatus_WidgetEvent");
            AddLog("browser: loads the CSS → runs the InitScript (defines init/update/pulse/render) → init(options) → the gauge appears");
        }

        #endregion

        #region Lab step 7: the refresh handler (success path + failure path)

        /// <summary>
        /// The handler the lab asks for. C# updates its own data, C# applies the rule, and one helper
        /// sends the resulting display values to the widget and to the native card.
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                statusService.Refresh();                 // C# updates its data (a new load sample)
                var data = statusService.GetStatus();    // C# owns the data and the rules
                UpdateWidget(data);
                AddLog("Widget refreshed with server data");
            }
            catch (Exception ex)
            {
                // Failure path: the user gets a safe message, the detail goes to the log, and the
                // widget keeps whatever it drew last — nothing is half-updated.
                SetStatus("Could not refresh status. Please try again.", StatusKind.Error);
                AddLog("btnRefresh_Click → GetStatus() threw → widget keeps its last values → user sees the safe message");
                AddLog($"   server-side detail (log only, never sent to the widget): {ex.GetType().Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// The one place that talks to the widget after load (the walkthrough calls it UpdateWidget).
        /// Options first, then Update(), then Call(): Wisej.NET sends them to the browser as one batch,
        /// applied in order.
        /// </summary>
        private void UpdateWidget(StatusInfo data)
        {
            widStatus.Options = new { percent = data.Percent, label = data.Label, status = data.Status };
            widStatus.Update();          // runs this.update(options, old) in the browser
            widStatus.Call("pulse");     // runs this.pulse() in the browser

            ShowServerData(data);        // the native card shows the same values → compare by eye
        }

        #endregion

        #region The C# rule recolours the widget (Set Healthy / Warning / Critical)

        private void btnSetHealthy_Click(object sender, EventArgs e) => ApplyRule(statusService.SetHealthy());

        private void btnSetWarning_Click(object sender, EventArgs e) => ApplyRule(statusService.SetWarning());

        private void btnSetCritical_Click(object sender, EventArgs e) => ApplyRule(statusService.SetCritical());

        private void ApplyRule(StatusInfo data)
        {
            UpdateWidget(data);
            AddLog($"Status set to {data.Status} by C# rule (load {data.SystemLoad} %)");
        }

        #endregion

        #region Lab step 8: the one widget event that supports a workflow

        private void widStatus_WidgetEvent(object sender, WidgetEventArgs e)
        {
            if (e.Type == "gaugeClick")
            {
                // e.Data is the small object the JS sent: { percent: 42 }.
                // Convert.ToInt32 rather than a bare cast: the JSON number may arrive as int, long or double.
                int percent = Convert.ToInt32(e.Data.percent);
                AddLog($"Gauge clicked at {percent}%");
                SetStatus($"gaugeClick received from the browser — {percent}% (matches lblLoad? {lblLoad.Text})", StatusKind.Ok);
            }
            else
            {
                // Anything else is noise for this screen: log it, do not build logic on it.
                AddLog($"widStatus_WidgetEvent → \"{e.Type}\" ignored (only gaugeClick supports a workflow here)");
            }
        }

        #endregion

        #region Bottom bar: simulated failure, theme re-test, clear

        private void btnSimulateError_Click(object sender, EventArgs e)
        {
            statusService.FailNextCall = true;
            AddLog("btnSimulateError_Click → StatusService.FailNextCall = true → calling btnRefresh_Click");
            btnRefresh_Click(sender, e);
        }

        private void btnToggleTheme_Click(object sender, EventArgs e)
        {
            _usingMaterial = !_usingMaterial;
            string theme = _usingMaterial ? "Material-3" : "Bootstrap-4";

            Application.LoadTheme(theme);

            AddLog($"btnToggleTheme_Click → Application.LoadTheme(\"{theme}\") → native controls restyle; the widget keeps its own lw-gauge-* CSS");
            AddLog("   lab step 10: click Refresh Server Data again — Update() and Call(\"pulse\") must still work after the theme change");
            SetStatus($"theme → {theme}; now re-test the widget with Refresh Server Data", StatusKind.Warn);
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            lstEventLog.Items.Clear();
        }

        #endregion

        #region Helpers (small, reusable — the habit the course teaches)

        /// <summary>Lab step 6: the native "Server-side data" card — what C# holds right now.</summary>
        private void ShowServerData(StatusInfo data)
        {
            lblOpen.Text = data.Open.ToString(CultureInfo.InvariantCulture);
            lblClosed.Text = data.Closed.ToString(CultureInfo.InvariantCulture);
            lblLoad.Text = data.SystemLoad.ToString(CultureInfo.InvariantCulture) + " %";
            lblStatusValue.Text = data.Status;
            lblStatusValue.ForeColor = ColorFor(data.Status);
            lblSent.Text = OptionsText(data);

            string rule = data.Status switch
            {
                "error" => $"above {StatusService.ErrorAbove}",
                "warn" => $"{StatusService.WarnFrom}–{StatusService.ErrorAbove}",
                _ => $"below {StatusService.WarnFrom}",
            };
            SetStatus($"{data.Status} — load {data.SystemLoad} % ({rule}) — widget and card show the same values", KindFor(data.Status));
        }

        /// <summary>The exact anonymous object handed to widStatus.Options, as text.</summary>
        private static string OptionsText(StatusInfo data)
        {
            return $"{{ percent = {data.Percent}, label = \"{data.Label}\", status = \"{data.Status}\" }}";
        }

        private enum StatusKind { Ok, Warn, Error }

        private static StatusKind KindFor(string status) => status switch
        {
            "error" => StatusKind.Error,
            "warn" => StatusKind.Warn,
            _ => StatusKind.Ok,
        };

        private static System.Drawing.Color ColorFor(string status) => status switch
        {
            "error" => System.Drawing.Color.FromArgb(224, 86, 59),
            "warn" => System.Drawing.Color.FromArgb(232, 161, 60),
            _ => System.Drawing.Color.FromArgb(31, 157, 87),
        };

        private void SetStatus(string text, StatusKind kind)
        {
            lblStatus.Text = "● " + text;
            lblStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
        }

        /// <summary>One place for logging, so every handler stays short.</summary>
        private void AddLog(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            lstEventLog.Items.Add($"{time}  {message}");
            lstEventLog.SelectedIndex = lstEventLog.Items.Count - 1;
        }

        #endregion
    }
}
