using System;
using System.Collections.Generic;
using System.Globalization;
using Wisej.Web;
using WisejTrainingApp.Services;
using WisejTrainingApp.Views;

namespace WisejTrainingApp
{
    /// <summary>
    /// Mini Helpdesk — Module 10 capstone shell (Program.cs → new Window1().Show()).
    ///
    /// The shell does three things and nothing else: it draws the frame (header, left navigation, content
    /// area, status bar), it swaps screens through NavigateTo(), and it keeps the activity log the Dashboard
    /// shows. There is NO business logic in this file — tickets live in Services/TicketService (through
    /// Services/TicketRepository), rules in Services/TicketValidator, customers in Services/CustomerService,
    /// and each screen's behaviour in its own Views/*View.cs.
    ///
    /// Spacing (the same numbers on every screen, so the app feels like one product):
    ///   header 64 px · left nav 220 px · status bar 36 px · content padding 24 px
    ///   gap between cards 16 px · card padding 24 px · gap between related controls 8 px
    ///   nav buttons 188×44, 6 px apart · command buttons 36 px high, grouped left→right: primary, secondary, danger last
    ///   card titles "default" 12 pt bold · body 10 pt · logs and code "monospace" 9 pt
    /// </summary>
    public partial class Window1 : Form, IHelpdeskShell
    {
        // Per-session state: one service instance per Window1, never static (each browser tab is its own user).
        private readonly TicketService ticketService;
        private readonly CustomerService customerService;

        private readonly Dictionary<string, HelpdeskView> screens = new Dictionary<string, HelpdeskView>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Button> navButtons = new Dictionary<string, Button>(StringComparer.OrdinalIgnoreCase);

        private DashboardView dashboardView;
        private HelpdeskView currentScreen;

        public Window1()
        {
            InitializeComponent();

            ticketService = new TicketService();      // wraps TicketRepository + TicketValidator
            customerService = new CustomerService();

            BuildScreens();
        }

        /// <summary>
        /// Creates the eight screens once and maps each nav button to its name. Screens receive only the
        /// services they need plus this shell (as IHelpdeskShell) — they never reach into Window1's controls.
        /// </summary>
        private void BuildScreens()
        {
            dashboardView = new DashboardView(this, ticketService);

            Register("Dashboard", btnDashboard, dashboardView);
            Register("Tickets", btnTickets, new TicketsView(this, ticketService));
            Register("Customers", btnCustomers, new CustomersView(this, customerService, ticketService));
            Register("Jobs", btnJobs, new JobsView(this, ticketService));
            Register("Architecture", btnArchitecture, new ArchitectureView(this));
            Register("Code Review", btnCodeReview, new CodeReviewView(this));
            Register("Deployment", btnDeployment, new DeploymentView(this));
            Register("Next Steps", btnNextSteps, new NextStepsView(this));
        }

        private void Register(string name, Button button, HelpdeskView view)
        {
            screens[name] = view;
            navButtons[name] = button;
            button.Tag = name;
        }

        private void Window1_Load(object sender, EventArgs e)
        {
            AddActivity("Program.Main → new Window1().Show() — shell built from Window1.Designer.cs");
            AddActivity("Services created for this session: TicketService (TicketRepository + TicketValidator), CustomerService");
            NavigateTo("Dashboard");
        }

        #region Navigation

        /// <summary>
        /// Every nav button uses this one handler; the screen name is in the button's Tag. Adding a screen is
        /// one Register() line, not a new handler.
        /// </summary>
        private void NavButton_Click(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is string screen)
                NavigateTo(screen);
        }

        /// <summary>The single place a screen is shown: swap the content panel, highlight the button, log it.</summary>
        public void NavigateTo(string screen)
        {
            if (!screens.TryGetValue(screen, out HelpdeskView view))
            {
                SetStatus($"Unknown screen \"{screen}\".", StatusKind.Error);
                AddActivity($"NavigateTo(\"{screen}\") → no such screen");
                return;
            }

            if (currentScreen != null)
                pnlContent.Controls.Remove(currentScreen);

            view.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(view);
            currentScreen = view;

            SetActiveButton(navButtons[screen]);
            view.ActivateScreen();

            AddActivity($"NavigateTo(\"{screen}\") → {view.GetType().Name} shown in pnlContent");
            SetStatus($"{screen} — ready", StatusKind.Ok);
        }

        /// <summary>Highlights the active nav button; the others fall back to the quiet card colour.</summary>
        private void SetActiveButton(Button active)
        {
            foreach (Button button in navButtons.Values)
            {
                bool isActive = ReferenceEquals(button, active);
                button.BackColor = isActive ? System.Drawing.Color.FromArgb(31, 111, 235) : System.Drawing.Color.FromArgb(247, 249, 252);
                button.ForeColor = isActive ? System.Drawing.Color.White : System.Drawing.Color.FromArgb(40, 52, 70);
                button.Font = new System.Drawing.Font("default", 10F, isActive ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular);
            }
        }

        #endregion

        #region Theme (Module 8)

        private void cboTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = cboTheme.SelectedItem as string;
            if (string.IsNullOrEmpty(name))
                return;

            try
            {
                Application.LoadTheme(name);
                lblCurrentTheme.Text = "Current: " + name;
                AddActivity($"cboTheme → Application.LoadTheme(\"{name}\") — every screen restyled live");
                SetStatus($"Theme {name} applied", StatusKind.Ok);
            }
            catch (Exception ex)
            {
                // Safe message for the user; the detail goes to the activity log, not the status bar.
                SetStatus("The theme could not be applied. The current theme is unchanged.", StatusKind.Error);
                AddActivity($"LoadTheme(\"{name}\") failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

        #endregion

        #region IHelpdeskShell — what the screens may ask the shell to do

        /// <summary>One place for logging (with a timestamp) so every handler in every screen stays short.</summary>
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

        /// <summary>Create / edit / delete happened: the Dashboard cards are recomputed from TicketService.GetSummary().</summary>
        public void TicketsChanged()
        {
            dashboardView.RefreshSummary();
        }

        #endregion
    }
}
