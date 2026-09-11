using System;
using System.Globalization;
using AdaptiveOps.Models;
using Wisej.Core;
using Wisej.Web;

namespace AdaptiveOps
{
    /// <summary>
    /// Adaptive Operations Console: the theme (Themes/AdaptiveOps.theme) owns the controls' identity,
    /// Styles/AdaptiveOps.css owns the app's own classes (metric-card, metric-title, compact-badge),
    /// barFill carries the only CssStyle, the Overdue card gets a themed "stale" state from data,
    /// and btnTheme switches this session between light and dark.
    /// </summary>
    public partial class MainPage : Page
    {
        private const string LightThemeName = "AdaptiveOps";
        private const string DarkThemeName = "AdaptiveOps-Dark";
        private const string DarkThemeKey = "DarkTheme";
        private const string LightThemeKey = "LightTheme";
        private const string StaleState = "stale";

        /// <summary>Colour tokens the dark copy overrides; every appearance refers to them by name.</summary>
        private static readonly (string Token, string Value)[] DarkTokens =
        {
            ("surface", "#1E2430"), ("surfaceAlt", "#141A24"), ("window", "#141A24"),
            ("windowText", "#E6EAF0"), ("textMain", "#E6EAF0"), ("textMuted", "#9AA5B8"), ("controlText", "#C9D1DC"),
            ("buttonFace", "#2A3342"), ("buttonText", "#E6EAF0"), ("buttonHighlight", "#3A4556"), ("windowFrame", "#3A4556"),
            ("warningBg", "#4A3A1A"), ("dangerBg", "#4A1F1B"), ("gray-200", "#2A3342"), ("light", "#1E2430"), ("toolbar", "#1E2430"),
            ("table-row-background", "#1E2430"), ("table-row-background-even", "#1E2430"), ("table-row-background-odd", "#232B38"),
            ("table-row-background-selected", "#2F3B4E"), ("table-row-background-focused", "#2F3B4E"),
            ("table-row-background-focused-selected", "#3B5C9E"), ("table-row", "#C9D1DC"), ("table-row-selected", "#FFFFFF"),
            ("table-row-line", "#3A4556"), ("table-column-line", "#3A4556"), ("text-placeholder", "#6B7688"),
        };

        private readonly TicketRepository _repository = new TicketRepository();

        private string _selectedId;
        private bool _suppressSelection;

        public MainPage()
        {
            InitializeComponent();

            this.cboPriority.Items.AddRange(Enum.GetNames(typeof(TicketPriority)));
            this.cboStatus.Items.AddRange(Enum.GetNames(typeof(TicketStatus)));

            ReportWidth();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            // A session that chose dark before a reload gets it back.
            if (IsDarkChoice())
                SetSessionTheme(true);

            ShowThemeState();
            RefreshTickets();
            ReportWidth();
        }

        private void MainPage_Resize(object sender, EventArgs e)
        {
            ReportWidth();
        }

        private void ReportWidth()
        {
            try
            {
                int width = Application.Browser.Size.Width;
                if (width <= 0)
                    throw new InvalidOperationException("The browser has not reported its size yet.");

                this.widthLabel.Text = $"Width: {width} px";
            }
            catch (Exception)
            {
                this.widthLabel.Text = "Width: unavailable";
            }
        }

        #region Session-only light/dark theme

        private void btnTheme_Click(object sender, EventArgs e)
        {
            bool dark = !IsDarkChoice();
            try
            {
                SetSessionTheme(dark);
                StoreSessionValue(DarkThemeKey, dark);
                ShowThemeState();
            }
            catch (Exception ex)
            {
                // A missing or invalid theme: say so and stay on the current theme.
                SetStatus("Theme not applied: " + ex.Message);
                AlertBox.Show("Theme not applied: " + ex.Message, MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
        }

        /// <summary>
        /// Assigns Application.Theme for this session only. Dark is a copy of the shared theme with
        /// dark colour tokens; the shared theme object that every session uses is never modified.
        /// </summary>
        private void SetSessionTheme(bool dark)
        {
            ClientTheme current = Application.Theme ?? throw new InvalidOperationException("No theme is loaded.");

            if (dark)
            {
                if (string.Equals(current.Name, DarkThemeName, StringComparison.OrdinalIgnoreCase))
                    return;

                var copy = new ClientTheme(DarkThemeName, current);
                var colors = copy.Colors as DynamicObject
                    ?? throw new InvalidOperationException("The theme has no colors.");

                foreach (var (token, value) in DarkTokens)
                    colors[token] = value;

                StoreSessionValue(LightThemeKey, current);
                Application.Theme = copy;
            }
            else
            {
                if (ReadSessionValue(LightThemeKey) is ClientTheme light)
                    Application.Theme = light;
            }
        }

        private void ShowThemeState()
        {
            bool dark = string.Equals(Application.Theme?.Name, DarkThemeName, StringComparison.OrdinalIgnoreCase);
            SetStatus($"Theme: {(dark ? DarkThemeName : LightThemeName)} (this session)");
            this.btnTheme.Text = dark ? "Light theme" : "Dark theme";
        }

        private static bool IsDarkChoice() => ReadSessionValue(DarkThemeKey) is bool dark && dark;

        private static object ReadSessionValue(string key)
        {
            var session = Application.Session as DynamicObject;
            return session != null && session.Contains(key) ? session[key] : null;
        }

        private static void StoreSessionValue(string key, object value)
        {
            if (Application.Session is DynamicObject session)
                session[key] = value;
        }

        #endregion

        #region Tickets, stale state and the SLA bar

        private void RefreshTickets()
        {
            var today = DateTime.Today;
            var tickets = _repository.GetAll();

            this.lblOpenValue.Text = N(_repository.CountOpen());
            this.lblOverdueValue.Text = N(_repository.CountOverdue(today));
            this.lblMineValue.Text = N(_repository.CountAssignedToMe());
            this.lblClosedValue.Text = N(_repository.CountClosedThisWeek(today));

            string keep = _selectedId;
            int reselect = -1;

            _suppressSelection = true;
            try
            {
                this.gridTickets.Rows.Clear();
                foreach (var t in tickets)
                {
                    int index = this.gridTickets.Rows.Add(new object[]
                    {
                        t.Id,
                        t.Title,
                        t.Priority.ToString(),
                        t.Status.ToString(),
                        t.Owner,
                        t.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    });
                    this.gridTickets.Rows[index].Tag = t.Id;
                    if (t.Id == keep)
                        reselect = index;
                }

                if (reselect < 0 && this.gridTickets.Rows.Count > 0)
                    reselect = 0;

                this.gridTickets.ClearSelection();
                if (reselect >= 0)
                    this.gridTickets.Rows[reselect].Selected = true;
            }
            finally
            {
                _suppressSelection = false;
            }

            // The threshold is data (TicketRepository.StaleAfterDays); the look is the theme's "stale" state.
            if (_repository.CountStaleOverdue(today) > 0)
            {
                this.cardOverdue.AddState(StaleState);
                this.lblOverdueValue.AddState(StaleState);
            }
            else
            {
                this.cardOverdue.RemoveState(StaleState);
                this.lblOverdueValue.RemoveState(StaleState);
            }

            UpdateSla();

            if (reselect >= 0)
                ShowTicket((string)this.gridTickets.Rows[reselect].Tag);
            else
                ClearEditor();
        }

        /// <summary>
        /// The only CssStyle in the project: the share of open tickets on time is computed from data,
        /// so it cannot be a class. clip-path, not width: the layout engine rewrites an inline width.
        /// </summary>
        private void UpdateSla()
        {
            int slaPercent = Math.Max(0, Math.Min(100, _repository.SlaOnTimePercent(DateTime.Today)));
            this.barFill.CssStyle = $"clip-path:inset(0 {100 - slaPercent}% 0 0)";
            this.lblSlaValue.Text = $"{slaPercent} %";
        }

        private void UpdatePriorityBadge(TicketPriority? priority)
        {
            this.lblPriorityBadge.Text = priority?.ToString() ?? string.Empty;
            this.lblPriorityBadge.CssClass = priority == null
                ? "compact-badge"
                : "compact-badge priority-" + priority.ToString().ToLowerInvariant();
        }

        #endregion

        #region Details editor

        private void gridTickets_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelection)
                return;

            var row = this.gridTickets.SelectedRows.Count > 0
                ? this.gridTickets.SelectedRows[0]
                : this.gridTickets.CurrentRow;

            if (row == null || row.Tag == null)
            {
                ClearEditor();
                return;
            }

            ShowTicket((string)row.Tag);
        }

        private void ShowTicket(string id)
        {
            var t = _repository.Get(id);
            if (t == null)
            {
                ClearEditor();
                return;
            }

            _selectedId = t.Id;
            this.lblDetailsSubtitle.Text = $"{t.Id} · {t.Status} · due {t.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
            this.txtTitle.Text = t.Title;
            this.cboPriority.SelectedIndex = (int)t.Priority;
            this.cboStatus.SelectedIndex = (int)t.Status;
            this.txtOwner.Text = t.Owner;
            this.dtpDue.Value = t.DueDate;
            this.txtNotes.Text = t.Notes;
            UpdatePriorityBadge(t.Priority);
        }

        private void ClearEditor()
        {
            _selectedId = null;
            this.lblDetailsSubtitle.Text = "Select a ticket in the grid";
            this.txtTitle.Text = string.Empty;
            this.cboPriority.SelectedIndex = -1;
            this.cboStatus.SelectedIndex = -1;
            this.txtOwner.Text = string.Empty;
            this.dtpDue.Value = DateTime.Today;
            this.txtNotes.Text = string.Empty;
            UpdatePriorityBadge(null);
        }

        private Ticket ReadEditor()
        {
            return new Ticket
            {
                Id = _selectedId,
                Title = this.txtTitle.Text,
                Priority = (TicketPriority)Math.Max(0, this.cboPriority.SelectedIndex),
                Status = (TicketStatus)Math.Max(0, this.cboStatus.SelectedIndex),
                Owner = this.txtOwner.Text,
                DueDate = this.dtpDue.Value,
                Notes = this.txtNotes.Text
            };
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_selectedId == null)
            {
                SetStatus("Select a ticket before saving.");
                return;
            }

            try
            {
                var saved = _repository.Save(ReadEditor());
                RefreshTickets();
                SetStatus($"Saved {saved.Id}");
                AlertBox.Show($"{saved.Id} saved.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
            catch (TicketValidationException ex)
            {
                SetStatus("Not saved: " + ex.Message);
                AlertBox.Show(ex.Message, MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
        }

        #endregion

        private void btnNav_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
                this.lblWorkspaceTitle.Text = button.Text == "Dashboard" ? "Tickets" : button.Text;
        }

        private void SetStatus(string text)
        {
            this.lblStatus.Text = text;
        }

        private static string N(int value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
