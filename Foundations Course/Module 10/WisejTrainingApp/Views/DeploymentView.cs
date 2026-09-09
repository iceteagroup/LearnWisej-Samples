using System;
using Wisej.Web;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Deployment: the required release checklist from lesson s42 §4 (Module 9) applied to this project.
    /// lblPackageStatus says "not ready" until every required item is checked — the same gate the Module 9
    /// release-review screen had. The notes card says what each item means for this app.
    /// </summary>
    public partial class DeploymentView : HelpdeskView
    {
        private static readonly string[] RequiredChecks =
        {
            "Web.config reviewed and debug mode set correctly",
            "Wisej.NET license key handled securely",
            "Default/startup window, theme and URL settings checked",
            "Themes and static resources included",
            "Logging destination configured",
            "Authentication and authorization plan reviewed",
            "Sensitive files are not publicly downloadable",
            "Release build tested locally before deployment",
            "Deployment target requirements checked",
        };

        public DeploymentView(IHelpdeskShell shell)
            : base(shell)
        {
            InitializeComponent();

            foreach (string check in RequiredChecks)
                chkRequired.Items.Add(check);

            lblDeploymentNotes.Text = string.Join("\n", new[]
            {
                "Web.config      debug is in Default.json (\"debug\": true) — set false for release",
                "License key     Web.config Wisej.LicenseKey is empty here; supply it from the host's",
                "                app settings / environment, never commit it",
                "Startup         Default.json: startup = WisejTrainingApp.Program.Main → Window1",
                "Theme           Default.json theme = Bootstrap-4; cboTheme loads the others live",
                "Static files    Default.html only — no Widgets/ folder in this module",
                "Logging         today: lstActivity + lstJobLog (per session). Production: a file or",
                "                App Insights sink — see Next Steps",
                "Auth plan       lblUser is a placeholder; roles gate commands server-side (s42 §1)",
                "Sensitive       Startup.cs never serves *.json; Web.config is CopyToOutput=Never",
                "Release build   dotnet publish -c Release -f net10.0; run it once before shipping",
                "Target          Kestrel: --urls / ports / reverse proxy / HTTPS   IIS: hosting bundle,",
                "                app pool, Web.config   Cloud: app settings, secrets, slot",
            });

            UpdatePackageStatus();
        }

        private void chkRequired_AfterItemCheck(object sender, ItemCheckEventArgs e)
        {
            UpdatePackageStatus();
            Shell.AddActivity($"Deployment check \"{RequiredChecks[e.Index]}\" → {(e.NewValue == CheckState.Checked ? "done" : "undone")}");
        }

        /// <summary>The gate: ready only when every required item is checked.</summary>
        private void UpdatePackageStatus()
        {
            int done = chkRequired.CheckedItems.Count;
            int total = chkRequired.Items.Count;
            bool ready = done == total;

            lblPackageStatus.Text = ready
                ? "Package status: READY — all required checks complete."
                : $"Package status: NOT READY — {total - done} required check(s) still open.";
            lblPackageStatus.ForeColor = ready ? OkColor : ErrorColor;

            ShowStatus(lblStatus, ready ? "release package ready" : $"{done} of {total} required checks done", ready ? StatusKind.Ok : StatusKind.Warn);
        }

        /// <summary>Recovery: complete the list so the gate opens.</summary>
        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkRequired.Items.Count; i++)
                chkRequired.SetItemChecked(i, true);

            UpdatePackageStatus();
            Shell.AddActivity("btnCheckAll_Click → all required checks marked done → package READY");
        }

        private void btnResetChecklist_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkRequired.Items.Count; i++)
                chkRequired.SetItemChecked(i, false);

            UpdatePackageStatus();
            Shell.AddActivity("btnResetChecklist_Click → checklist cleared → package NOT READY");
        }
    }
}
