using System;
using System.Globalization;
using System.IO;
using Wisej.Web;

namespace WisejTrainingApp
{
    /// <summary>
    /// First app — Module 1 lab window (Getting started on Wisej.NET).
    ///
    /// Left card:   the lab's four controls — lblPrompt, txtName, btnGreet, lblResult — and the one
    ///              event handler the lab asks for (btnGreet_Click): read the input, validate, update the label.
    /// Below it:    "Inspect files" — lab step 7: what Program.cs, the code-behind, the .Designer.cs file
    ///              and the config files are for, checked against the disk.
    /// Right card:  the event log — every server-side decision, with a timestamp.
    /// Bottom bar:  the failure path (blank name → validation message), the recovery (a valid name
    ///              through the same handler) and Clear log.
    /// </summary>
    public partial class Window1 : Form
    {
        public Window1()
        {
            // InitializeComponent() builds every control from Window1.Designer.cs — the Designer owns that file.
            InitializeComponent();
        }

        private void Window1_Load(object sender, EventArgs e)
        {
            // The beginner lifecycle: Program.Main → new Window1() → InitializeComponent() → Load → user events.
            AddLog("Program.Main → new Window1().Show()");
            AddLog("InitializeComponent() built lblPrompt, txtName, btnGreet, lblResult from Window1.Designer.cs");
            AddLog("Window1_Load → ready; waiting for btnGreet.Click");
            InspectFiles();
        }

        #region The lab's event handler

        /// <summary>
        /// The one handler the lab asks for. It reads like a short story: get the input, validate, update the UI.
        /// Everything in here runs on the server; Wisej.NET refreshes the browser when it returns.
        /// </summary>
        private void btnGreet_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                // Failure path: the guard answers before anything else happens.
                lblResult.Text = "Please enter a name.";
                SetStatus("validation: a name is required", StatusKind.Warn);
                AddLog("btnGreet_Click → txtName is blank → IsNullOrWhiteSpace guard → lblResult = \"Please enter a name.\"");
                txtName.Focus();
                return;
            }

            // Success path.
            lblResult.Text = $"Hello, {name}!";
            SetStatus($"greeted {name}", StatusKind.Ok);
            AddLog($"btnGreet_Click → txtName.Text = \"{name}\" → lblResult.Text = \"Hello, {name}!\"");
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            // Enter in the textbox behaves like the button — same handler, no duplicated logic.
            if (e.KeyCode == Keys.Enter)
            {
                AddLog("txtName.KeyDown(Enter) → btnGreet_Click");
                btnGreet_Click(sender, EventArgs.Empty);
            }
        }

        #endregion

        #region Bottom bar: failure path, recovery, clear

        private void btnTryBlank_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
            AddLog("btnTryBlank_Click → txtName cleared → calling btnGreet_Click");
            btnGreet_Click(sender, e);
        }

        private void btnFillSample_Click(object sender, EventArgs e)
        {
            txtName.Text = "  Ada  ";     // extra spaces on purpose: Trim() removes them
            AddLog("btnFillSample_Click → txtName = \"  Ada  \" (spaces on purpose) → calling btnGreet_Click");
            btnGreet_Click(sender, e);
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            lstEventLog.Items.Clear();
        }

        #endregion

        #region Lab step 7: inspect the project files

        private void btnInspectFiles_Click(object sender, EventArgs e)
        {
            InspectFiles();
        }

        /// <summary>
        /// The files the lesson names in "Inspect the solution structure", checked against the project
        /// folder the app runs from. A ✓ means the file exists on disk right now.
        /// </summary>
        private void InspectFiles()
        {
            string root = Application.StartupPath;
            lblFilesPath.Text = root;
            lstFiles.Items.Clear();

            var files = new (string File, string Purpose)[]
            {
                ("Program.cs",                      "startup: Main() shows Window1 (Default.json → \"startup\")"),
                ("Window1.cs",                      "code-behind: btnGreet_Click and helpers live here"),
                ("Window1.Designer.cs",             "Designer-generated layout — don't edit by hand"),
                ("Startup.cs",                      "Kestrel host: app.UseWisej() + static files"),
                ("Default.json",                    "Wisej.NET config: startup class, theme, debug"),
                ("Default.html",                    "the page that loads wisej.wx (the client)"),
                ("Web.config",                      "license key + default theme (IIS-style settings)"),
                ("WisejTrainingApp.csproj",         "the project: Wisej-4 package, net10.0-windows;net10.0"),
                ("Properties/launchSettings.json",  "F5 profile: http://localhost:5081"),
            };

            int found = 0;
            foreach (var (file, purpose) in files)
            {
                bool exists = File.Exists(Path.Combine(root, file.Replace('/', Path.DirectorySeparatorChar)));
                if (exists) found++;
                lstFiles.Items.Add($"{(exists ? "✓" : "✗")} {file,-34} {purpose}");
            }

            AddLog($"InspectFiles → {found}/{files.Length} files found under Application.StartupPath");
        }

        #endregion

        #region Helpers (small, reusable — the habit the course teaches)

        private enum StatusKind { Ok, Warn, Error }

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
