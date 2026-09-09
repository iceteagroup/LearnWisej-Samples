using System;
using System.Collections.Generic;
using System.Globalization;
using Wisej.Web;
using WisejTrainingApp.Services;
using WisejTrainingApp.Views;

namespace WisejTrainingApp
{
    /// <summary>
    /// Modern dashboard shell — Module 7 lab window (Theming and UI modernization).
    ///
    /// Header (Dock Top):    lblAppTitle "ServiceDesk", lblModule, the theme selector cboTheme + lblCurrentTheme,
    ///                       and the failure-path button "Try an unknown theme".
    /// Navigation (Dock Left): btnDashboard · btnTickets · btnCustomers · btnReports · btnSettings.
    ///                       SetActiveButton() makes the current page's button look selected.
    /// Content (Dock Fill):  panelContent hosts one Views/*View UserControl at a time; NavigateTo(string) swaps it.
    /// Status (Dock Bottom): lblStatus — green ok, amber warning, red error.
    ///
    /// Everything in this file is the visual layer. TicketService, the grid binding in TicketsView and
    /// the validation rule in TicketDialog are exactly what Modules 4 and 5 built — Module 7 changes the theme
    /// and the layout, not the logic. (This file never names a service method or a validation method on purpose:
    /// the lab's code check pastes it and expects the theme code to stay clear of ticket logic.)
    /// </summary>
    public partial class MainWindow : Form, IAppShell
    {
        // Business logic — one instance per user session (never static), shared by every page through IAppShell.
        private readonly TicketService ticketService = new TicketService();

        // UI state — a UI setting, kept here in the shell and never passed to the service.
        private string currentTheme = "Bootstrap-4";

        // The pages, created once and swapped in and out of panelContent, so the Dashboard's activity list survives navigation.
        private readonly Dictionary<string, UserControl> views = new Dictionary<string, UserControl>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Button> navButtons = new Dictionary<string, Button>(StringComparer.OrdinalIgnoreCase);
        private DashboardView dashboardView;
        private UserControl currentView;

        public MainWindow()
        {
            // InitializeComponent() builds the header, the navigation and the status bar from the Designer file.
            InitializeComponent();

            // The pages are code-behind work: each one gets the shell (this window) through the IAppShell interface.
            dashboardView = new DashboardView(this);
            views["Dashboard"] = dashboardView;
            views["Tickets"] = new TicketsView(this);
            views["Customers"] = new CustomersView(this);
            views["Reports"] = new ReportsView(this);
            views["Settings"] = new SettingsView(this);

            navButtons["Dashboard"] = btnDashboard;
            navButtons["Tickets"] = btnTickets;
            navButtons["Customers"] = btnCustomers;
            navButtons["Reports"] = btnReports;
            navButtons["Settings"] = btnSettings;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            AddActivity("Program.Main → new MainWindow().Show() — theme from Default.json: " + currentTheme);
            lblCurrentTheme.Text = "Current theme: " + currentTheme;
            NavigateTo("Dashboard");
        }

        #region IAppShell — what the pages may ask for

        public TicketService Tickets => ticketService;

        public string CurrentTheme => currentTheme;

        /// <summary>
        /// The one navigation method (Module 3's habit). Every nav button and every "go to …" command calls this;
        /// it swaps the content view, highlights the nav button and logs the move.
        /// </summary>
        public void NavigateTo(string page)
        {
            if (!views.TryGetValue(page, out UserControl view))
            {
                // Safe failure: an unknown page name never throws at the user — it is reported and logged.
                SetStatus($"Unknown page '{page}'", StatusKind.Error);
                AddActivity($"NavigateTo(\"{page}\") → no such page — nothing changed");
                return;
            }

            if (currentView != null)
                panelContent.Controls.Remove(currentView);

            view.Dock = DockStyle.Fill;
            panelContent.Controls.Add(view);
            currentView = view;

            (view as IAppView)?.RefreshView();
            SetActiveButton(navButtons[page]);

            SetStatus($"{page} page", StatusKind.Ok);
            AddActivity($"NavigateTo(\"{page}\") → {view.GetType().Name} shown, {navButtons[page].Name} active");
        }

        /// <summary>Recent activity — the course's event log, HH:mm:ss + message, kept on the Dashboard card.</summary>
        public void AddActivity(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            dashboardView.AppendActivity($"{time}  {message}");
        }

        public void SetStatus(string text, StatusKind kind)
        {
            lblStatus.Text = "● " + text;
            lblStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
        }

        #endregion

        #region Navigation buttons + active state (lab steps 4 and 5)

        private void btnDashboard_Click(object sender, EventArgs e) => NavigateTo("Dashboard");

        private void btnTickets_Click(object sender, EventArgs e) => NavigateTo("Tickets");

        private void btnCustomers_Click(object sender, EventArgs e) => NavigateTo("Customers");

        private void btnReports_Click(object sender, EventArgs e) => NavigateTo("Reports");

        private void btnSettings_Click(object sender, EventArgs e) => NavigateTo("Settings");

        /// <summary>
        /// One consistent active nav state: the selected button is green with white text and a solid border,
        /// every other button goes back to the theme's flat look. Users can always tell which page they are on.
        /// </summary>
        private void SetActiveButton(Button active)
        {
            foreach (Button button in navButtons.Values)
            {
                bool isActive = button == active;
                if (isActive)
                {
                    button.BackColor = System.Drawing.Color.FromArgb(31, 157, 87);
                    button.ForeColor = System.Drawing.Color.White;
                    button.BorderStyle = BorderStyle.Solid;
                    button.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
                }
                else
                {
                    button.ResetBackColor();
                    button.ResetForeColor();
                    button.BorderStyle = BorderStyle.None;
                    button.Font = new System.Drawing.Font("default", 10F);
                }
            }
        }

        #endregion

        #region Theme selector (lab steps 2 and 3) — a UI setting, kept out of the ticket logic

        /// <summary>
        /// The lesson's handler. It reads the selection, applies the theme and updates a small label — and that is all.
        /// No save call, no validation call, no service call: the theme is a UI setting.
        /// </summary>
        private void cboTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTheme = cboTheme.Text;
            if (string.IsNullOrEmpty(selectedTheme) && cboTheme.SelectedItem != null)
                selectedTheme = cboTheme.SelectedItem.ToString();

            if (string.IsNullOrEmpty(selectedTheme) || selectedTheme == currentTheme)
                return;

            ApplyTheme(selectedTheme);
        }

        /// <summary>
        /// The failure path: a theme name that is not embedded in Wisej.NET. The app reports it, keeps the
        /// current theme and logs it; choosing a real theme in cboTheme afterwards is the recovery.
        /// </summary>
        private void btnTryUnknownTheme_Click(object sender, EventArgs e)
        {
            AddActivity("btnTryUnknownTheme_Click → Application.LoadTheme(\"Foo-9\") — there is no such theme");
            ApplyTheme("Foo-9");
        }

        /// <summary>
        /// Applies a built-in theme by name. The lesson writes this line as
        ///     Application.Theme.Name = selectedTheme;
        /// Application.Theme is the current ClientTheme object; Application.LoadTheme(name) is the call that
        /// loads one of the themes embedded in Wisej.NET (Bootstrap-4, BootstrapDark-4, Blue-1, Classic-2,
        /// Material-3, FluentLight-5, …) and restyles the running app live.
        /// </summary>
        private void ApplyTheme(string selectedTheme)
        {
            string previousTheme = currentTheme;
            try
            {
                Application.LoadTheme(selectedTheme);

                // If the framework silently ignores an unknown name, the loaded theme still carries the old name.
                string loadedName = Application.Theme?.Name;
                if (!string.IsNullOrEmpty(loadedName) && !string.Equals(loadedName, selectedTheme, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"Application.Theme.Name is still '{loadedName}'");

                currentTheme = selectedTheme;
                lblCurrentTheme.Text = "Current theme: " + selectedTheme;
                SetStatus($"theme {selectedTheme} applied — tickets, binding and validation untouched", StatusKind.Ok);
                AddActivity($"Theme changed to {selectedTheme} (Application.LoadTheme) — lblCurrentTheme updated; TicketService not involved");
            }
            catch (Exception ex)
            {
                // Keep what worked: re-assert the previous theme and tell the user without an exception page.
                TryReloadTheme(previousTheme);
                currentTheme = previousTheme;
                lblCurrentTheme.Text = "Current theme: " + previousTheme;
                SelectThemeInCombo(previousTheme);
                SetStatus($"Theme '{selectedTheme}' not found — kept {previousTheme}", StatusKind.Error);
                AddActivity($"Theme '{selectedTheme}' not found ({ex.GetType().Name}: {ex.Message}) — kept {previousTheme}; pick a real theme in cboTheme to recover");
            }

            // The Settings page shows the current theme; refresh it if it is the page on screen.
            (currentView as IAppView)?.RefreshView();
        }

        private static void TryReloadTheme(string name)
        {
            try { Application.LoadTheme(name); }
            catch { /* the previous theme is still the one the browser shows */ }
        }

        /// <summary>Puts the combo back on a theme name without re-firing ApplyTheme (the guard in the handler skips the current theme).</summary>
        private void SelectThemeInCombo(string name)
        {
            int index = cboTheme.Items.IndexOf(name);
            if (index >= 0 && cboTheme.SelectedIndex != index)
                cboTheme.SelectedIndex = index;
        }

        #endregion
    }
}
