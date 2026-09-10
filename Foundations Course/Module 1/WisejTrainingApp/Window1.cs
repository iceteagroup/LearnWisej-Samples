using System;
using System.Globalization;
using System.IO;
using Wisej.Web;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp
{
    /// <summary>
    /// First app — Module 1 (How Wisej.NET Works: "What Wisej.NET Is" + "Getting Started on Wisej.NET").
    ///
    /// Top left:    the lesson's worked example — lblTitle, txtName, btnSayHello, lblStatus — and the
    ///              handler that reads like a short story: get the input, validate, update the UI.
    /// Bottom left: "Solution structure" — Program.cs, the code-behind, the .Designer.cs file, the config
    ///              files and the Models/ + Services/ folders, checked against the disk.
    /// Top right:   the event log — every server-side decision, with a timestamp.
    /// Bottom right: the lesson's second example — btnSaveTicket_Click stays four lines because
    ///              Models/Ticket.cs holds the shape and Services/TicketService.cs holds the rule.
    /// Bottom bar:  a failure path for each example, the recovery, and Clear log.
    /// </summary>
    public partial class Window1 : Form
    {
        /// <summary>
        /// "Separate business logic: a TicketService owns the rules." The screen holds a reference and
        /// calls it — it never stores or numbers a ticket itself.
        /// </summary>
        private readonly TicketService ticketService = new TicketService();

        public Window1()
        {
            // InitializeComponent() builds every control from Window1.Designer.cs — the Designer owns that file.
            InitializeComponent();
        }

        private void Window1_Load(object sender, EventArgs e)
        {
            // The beginner lifecycle: Program.Main → new Window1() → InitializeComponent() → Load → user events.
            AddLog("Program.Main → new Window1().Show()");
            AddLog("InitializeComponent() built lblTitle, txtName, btnSayHello, lblStatus from Window1.Designer.cs");
            AddLog("Window1_Load → ready; waiting for btnSayHello.Click");
            InspectFiles();
        }

        #region The lesson's first example: a readable event handler

        /// <summary>
        /// The handler the lesson prints in full. It reads like a short story: get the input, validate,
        /// update the UI. Everything in here runs on the server; Wisej.NET refreshes the browser when it returns.
        /// </summary>
        private void btnSayHello_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                // Failure path: the guard answers before anything else happens.
                lblStatus.Text = "Please enter a name.";
                SetRunState("validation: a name is required", StatusKind.Warn);
                AddLog("btnSayHello_Click → txtName is blank → IsNullOrWhiteSpace guard → lblStatus = \"Please enter a name.\"");
                txtName.Focus();
                return;
            }

            // Success path.
            lblStatus.Text = $"Hello, {name}!";
            SetRunState($"greeted {name}", StatusKind.Ok);
            AddLog($"btnSayHello_Click → txtName.Text = \"{name}\" → lblStatus.Text = \"Hello, {name}!\"");
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            // Enter in the textbox behaves like the button — same handler, no duplicated logic.
            if (e.KeyCode == Keys.Enter)
            {
                AddLog("txtName.KeyDown(Enter) → btnSayHello_Click");
                btnSayHello_Click(sender, EventArgs.Empty);
            }
        }

        #endregion

        #region The lesson's second example: the click calls a service

        /// <summary>
        /// "Call ValidateInput() and ticketService.Save(ticket)" — the whole handler, four lines long.
        /// It does not know what a valid ticket is, how ids are assigned, or where tickets are stored.
        /// </summary>
        private void btnSaveTicket_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            Ticket ticket = ReadTicketFromScreen();
            ticketService.Save(ticket);
            lblStatus.Text = "Ticket saved.";

            SetRunState($"saved ticket #{ticket.Id} — {ticketService.Count} in the service", StatusKind.Ok);
            AddLog($"btnSaveTicket_Click → ReadTicketFromScreen() → ticketService.Save(ticket) → #{ticket.Id} \"{ticket.Title}\"");
            RefreshTicketList();

            txtTicketTitle.Text = "";
            txtTicketCustomer.Text = "";
            txtTicketTitle.Focus();
        }

        /// <summary>The guard the habits table names — one place, so the handler stays readable.</summary>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtTicketTitle.Text))
            {
                lblStatus.Text = "A ticket needs a title.";
                SetRunState("validation: ticket title is required", StatusKind.Warn);
                AddLog("btnSaveTicket_Click → ValidateInput() = false → returned before the service was called");
                txtTicketTitle.Focus();
                return false;
            }

            return true;
        }

        /// <summary>Turns what is on the screen into a Ticket. UI in, model out — no rules here.</summary>
        private Ticket ReadTicketFromScreen()
        {
            return new Ticket
            {
                Title = txtTicketTitle.Text.Trim(),
                Customer = txtTicketCustomer.Text.Trim(),
                Status = "Open",
            };
        }

        private void RefreshTicketList()
        {
            lstTickets.Items.Clear();
            foreach (Ticket ticket in ticketService.GetTickets())
            {
                string customer = string.IsNullOrWhiteSpace(ticket.Customer) ? "—" : ticket.Customer;
                lstTickets.Items.Add($"#{ticket.Id,-3} {ticket.Title,-42} {customer,-20} {ticket.Status}");
            }

            lstTickets.SelectedIndex = lstTickets.Items.Count - 1;
        }

        #endregion

        #region Bottom bar: failure paths, recovery, clear

        private void btnTryBlank_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
            AddLog("btnTryBlank_Click → txtName cleared → calling btnSayHello_Click");
            btnSayHello_Click(sender, e);
        }

        private void btnFillSample_Click(object sender, EventArgs e)
        {
            txtName.Text = "  Ada  ";     // extra spaces on purpose: Trim() removes them
            AddLog("btnFillSample_Click → txtName = \"  Ada  \" (spaces on purpose) → calling btnSayHello_Click");
            btnSayHello_Click(sender, e);
        }

        private void btnTryEmptyTicket_Click(object sender, EventArgs e)
        {
            txtTicketTitle.Text = "";
            txtTicketCustomer.Text = "";
            AddLog("btnTryEmptyTicket_Click → ticket fields cleared → calling btnSaveTicket_Click");
            btnSaveTicket_Click(sender, e);
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            lstEventLog.Items.Clear();
        }

        #endregion

        #region "Inspect the solution structure"

        private void btnInspectFiles_Click(object sender, EventArgs e)
        {
            InspectFiles();
        }

        /// <summary>
        /// The files and folders the lesson names in "Inspect the solution structure", checked against the
        /// project folder the app runs from. A ✓ means it exists on disk right now.
        /// </summary>
        private void InspectFiles()
        {
            string root = Application.StartupPath;
            lblFilesPath.Text = root;
            lstFiles.Items.Clear();

            var files = new (string File, string Purpose)[]
            {
                ("Program.cs",                      "startup: Main() shows Window1 (Default.json → \"startup\")"),
                ("Window1.cs",                      "code-behind: btnSayHello_Click and helpers live here"),
                ("Window1.Designer.cs",             "Designer-generated layout — don't edit by hand"),
                ("Models/Ticket.cs",                "Models/: simple data classes — the shape of a record"),
                ("Models/Customer.cs",              "Models/: the second data class the lesson names"),
                ("Services/TicketService.cs",       "Services/: business logic, away from the UI"),
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

        private void SetRunState(string text, StatusKind kind)
        {
            lblRunState.Text = "● " + text;
            lblRunState.ForeColor = kind switch
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
