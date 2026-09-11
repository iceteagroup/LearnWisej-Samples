using System;
using System.IO;
using Wisej.Web;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp
{
    public partial class StatusPage : Page
    {
        // C# owns the data and the rules. One service per page = per user session.
        private readonly StatusService statusService = new StatusService();

        public StatusPage()
        {
            InitializeComponent();
        }

        // C# — StatusPage.cs (both files live under the web project's /Widgets folder)
        private void StatusPage_Load(object sender, EventArgs e)
        {
            // 1. Stylesheet (and any third-party scripts) load in list order, once per session.
            widStatus.Packages.Add(new Widget.Package {
                Name   = "statusGauge-css",
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

            ShowServerData(data);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            statusService.Refresh();                    // C# updates its data
            var data = statusService.GetStatus();      // C# owns the data and the rules
            UpdateWidget(data);
            AddLog("Widget refreshed with server data");
        }

        private void btnSetHealthy_Click(object sender, EventArgs e)
        {
            UpdateWidget(statusService.SetHealthy());
            AddLog("Status set to Healthy.");
        }

        private void btnSetWarning_Click(object sender, EventArgs e)
        {
            UpdateWidget(statusService.SetWarning());
            AddLog("Status set to Warning.");
        }

        private void btnSetCritical_Click(object sender, EventArgs e)
        {
            UpdateWidget(statusService.SetCritical());
            AddLog("Status set to Critical.");
        }

        private void widStatus_WidgetEvent(object sender, WidgetEventArgs e)
        {
            if (e.Type == "gaugeClick")
            {
                // e.Data is the small object the JS sent: { percent: 42 }
                int percent = Convert.ToInt32(e.Data.percent);
                AddLog($"Gauge clicked at {percent}%");
            }
        }

        // The native labels and the widget always show the same values, from one C# call.
        private void UpdateWidget(StatusInfo data)
        {
            widStatus.Options = new { percent = data.Percent, label = data.Label, status = data.Status };
            widStatus.Update();                         // runs this.update(options, old) in the browser
            widStatus.Call("pulse");                    // runs this.pulse() in the browser
            ShowServerData(data);
        }

        private void ShowServerData(StatusInfo data)
        {
            lblStatusValue.Text = data.Status;
            lblOpen.Text = data.Open.ToString();
            lblClosed.Text = data.Closed.ToString();
            lblLoad.Text = data.SystemLoad + " %";
        }

        private void AddLog(string message)
        {
            lstEventLog.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " - " + message);
        }
    }
}
