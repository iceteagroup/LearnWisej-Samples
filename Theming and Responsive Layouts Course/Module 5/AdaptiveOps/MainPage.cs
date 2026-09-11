using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using AdaptiveOps.Models;
using Wisej.Web;

namespace AdaptiveOps
{
    /// <summary>
    /// Adaptive Operations Console: the docked shell regions stay as in Module 4; the content inside them
    /// is built with the three layout engines. The toolbar is Layout/FilterBar (FlowLayoutPanel: search,
    /// status, Apply with a FlowBreak, then the metric cards), the workspace is Layout/DashboardWorkspace
    /// (FlexLayoutPanel, list and details 2 : 1, written with Wisej.Web.Markup) and the details form is
    /// Shell/TicketEditor (TableLayoutPanel). No code sets Bounds and there is no Resize handler.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly TicketRepository _repository = new TicketRepository();

        private string _selectedId;
        private bool _suppressSelection;

        /// <summary>The grid filter set by ApplyFilters(); null shows every ticket.</summary>
        private Func<Ticket, bool> _filter;

        public MainPage()
        {
            InitializeComponent();

            // The dashboard is built in code (Layout/DashboardWorkspace.cs), so its grid and editor are wired here.
            this.dashboard.Grid.SelectionChanged += this.gridTickets_SelectionChanged;
            this.dashboard.Editor.SaveClick += this.editor_SaveClick;

            Application.BrowserSizeChanged += this.Application_BrowserSizeChanged;
            ReportWidth();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            LoadTickets();
            ReportWidth();
            SetStatus("Ready");
        }

        private void Application_BrowserSizeChanged(object sender, EventArgs e)
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

        #region Filters

        private void filterBar_Apply(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Builds the grid filter from the search text and the status, then refreshes the list. Text written
        /// as "/pattern/" is a regular expression; one that does not parse is reported in the status label and
        /// the grid keeps its last good filter.
        /// </summary>
        private void ApplyFilters()
        {
            string text = (this.filterBar.SearchText ?? string.Empty).Trim();
            string status = this.filterBar.StatusFilter;

            try
            {
                Func<Ticket, bool> textMatch = _ => true;
                if (text.Length >= 2 && text.StartsWith("/", StringComparison.Ordinal) && text.EndsWith("/", StringComparison.Ordinal))
                {
                    var regex = new Regex(text.Substring(1, text.Length - 2), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(250));
                    textMatch = t => regex.IsMatch(Haystack(t));
                }
                else if (text.Length > 0)
                {
                    textMatch = t => Haystack(t).IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;
                }

                TicketStatus? wanted = status == null ? (TicketStatus?)null : Enum.Parse<TicketStatus>(status);
                _filter = t => textMatch(t) && (wanted == null || t.Status == wanted.Value);

                int shown = LoadTickets();
                SetStatus($"Filters applied: {shown} of {_repository.GetAll().Count} tickets");
            }
            catch (ArgumentException ex)
            {
                SetStatus("Filter error: " + ex.Message.Split('\n')[0].Trim());
            }
        }

        private static string Haystack(Ticket t)
        {
            return string.Join(" ", t.Id, t.Title, t.Owner, t.Notes);
        }

        #endregion

        #region Tickets

        /// <summary>
        /// Reloads the metric cards (always all tickets), the grid (through the current filter) and the
        /// editor. Returns the number of rows shown.
        /// </summary>
        private int LoadTickets()
        {
            var today = DateTime.Today;
            var all = _repository.GetAll();
            var shown = _filter == null ? all : all.Where(_filter).ToList();

            this.filterBar.SetMetrics(
                _repository.CountOpen(),
                _repository.CountOverdue(today),
                _repository.CountAssignedToMe(),
                _repository.CountClosedThisWeek(today));

            var grid = this.dashboard.Grid;
            string keep = _selectedId;
            int reselect = -1;

            _suppressSelection = true;
            try
            {
                grid.Rows.Clear();
                foreach (var t in shown)
                {
                    int index = grid.Rows.Add(new object[]
                    {
                        t.Id,
                        t.Title,
                        t.Priority.ToString(),
                        t.Status.ToString(),
                        t.Owner,
                        t.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    });
                    grid.Rows[index].Tag = t.Id;
                    if (t.Id == keep)
                        reselect = index;
                }

                if (reselect < 0 && grid.Rows.Count > 0)
                    reselect = 0;

                grid.ClearSelection();
                if (reselect >= 0)
                    grid.Rows[reselect].Selected = true;
            }
            finally
            {
                _suppressSelection = false;
            }

            if (reselect >= 0)
                ShowTicket((string)grid.Rows[reselect].Tag);
            else
                ClearEditor();

            return shown.Count;
        }

        private void gridTickets_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelection)
                return;

            var grid = this.dashboard.Grid;
            var row = grid.SelectedRows.Count > 0 ? grid.SelectedRows[0] : grid.CurrentRow;

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
            this.dashboard.Editor.ShowTicket(t);
        }

        private void ClearEditor()
        {
            _selectedId = null;
            this.dashboard.Editor.Clear();
        }

        private void editor_SaveClick(object sender, EventArgs e)
        {
            if (_selectedId == null)
            {
                SetStatus("Select a ticket before saving.");
                return;
            }

            try
            {
                var saved = _repository.Save(this.dashboard.Editor.Read(_selectedId));
                LoadTickets();
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
                this.dashboard.ListTitle.Text = button.Text == "Dashboard" ? "Tickets" : button.Text;
        }

        private void SetStatus(string text)
        {
            this.lblStatus.Text = text;
        }
    }
}
