using System;
using AdaptiveOps.Models;
using AdaptiveOps.Shell;
using Wisej.Web;

namespace AdaptiveOps
{
    /// <summary>
    /// Adaptive Operations Console shell: five docked regions (MainPage.Designer.cs) composed by Dock
    /// and child order only, with Padding for the gaps and MinimumSize / MaximumSize guard rails.
    /// Four regions are UserControls with their own local layout; the page only uses their public
    /// members (Ticket, Saved, SelectedSection, SectionChanged). No code sets Bounds.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly TicketRepository _repository = new TicketRepository();

        private string _selectedId;

        public MainPage()
        {
            InitializeComponent();

            // Only reports the width; the layout itself is Dock, Padding and MinimumSize.
            Application.BrowserSizeChanged += this.Application_BrowserSizeChanged;
            ReportWidth();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            LoadTickets();
            ReportWidth();
            this.statusBar.ShowStatus("Ready");
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

                this.statusBar.WidthText = $"Width: {width} px";
            }
            catch (Exception)
            {
                this.statusBar.WidthText = "Width: unavailable";
            }
        }

        private void LoadTickets()
        {
            var today = DateTime.Today;

            this.workspace.SetMetrics(
                _repository.CountOpen(),
                _repository.CountOverdue(today),
                _repository.CountAssignedToMe(),
                _repository.CountClosedThisWeek(today));

            string selected = this.workspace.ShowTickets(_repository.GetAll(), _selectedId);
            ShowTicket(selected);
        }

        private void workspace_SelectionChanged(object sender, EventArgs e)
        {
            ShowTicket(this.workspace.SelectedTicketId);
        }

        private void ShowTicket(string id)
        {
            var t = id == null ? null : _repository.Get(id);
            _selectedId = t?.Id;
            this.detailsEditor.Ticket = t;
        }

        /// <summary>
        /// Saves the ticket the editor raised. A validation error or any other exception becomes a
        /// message in the status region, never a broken layout.
        /// </summary>
        private void detailsEditor_Saved(object sender, TicketSaveEventArgs e)
        {
            try
            {
                var saved = _repository.Save(e.Ticket);
                LoadTickets();
                this.statusBar.ShowStatus($"Saved {saved.Id}");
                AlertBox.Show($"{saved.Id} saved.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
            catch (TicketValidationException ex)
            {
                this.statusBar.ShowStatus("Not saved: " + ex.Message);
                AlertBox.Show(ex.Message, MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
            catch (Exception ex)
            {
                this.statusBar.ShowStatus("Save failed: " + ex.Message);
                AlertBox.Show("The ticket could not be saved: " + ex.Message, MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
        }

        private void navigationRail_SectionChanged(object sender, EventArgs e)
        {
            string section = this.navigationRail.SelectedSection;
            this.workspace.Title = section == "Dashboard" ? "Tickets" : section;
        }
    }
}
