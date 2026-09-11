using System;
using System.Globalization;
using AdaptiveOps.Models;
using Wisej.Web;

namespace AdaptiveOps
{
    /// <summary>
    /// Adaptive Operations Console: the desktop shell of five docked panels (MainPage.Designer.cs),
    /// the base theme selected in Default.json, and a status label that reports the browser width.
    /// </summary>
    public partial class MainPage : Page
    {
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
            LoadTickets();
            ReportWidth();
        }

        private void MainPage_Resize(object sender, EventArgs e)
        {
            ReportWidth();
        }

        /// <summary>
        /// Writes the current browser width into the status bar.
        /// </summary>
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
                AlertBox.Show("The browser width could not be read.", MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTickets();
            SetStatus("Ready");
        }

        /// <summary>
        /// Reloads the metric cards, the grid and the details editor from the repository.
        /// </summary>
        private void LoadTickets()
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

            if (reselect >= 0)
                ShowTicket((string)this.gridTickets.Rows[reselect].Tag);
            else
                ClearEditor();
        }

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
                this.lblWorkspaceTitle.Text = button.Text == "Dashboard" ? "Tickets" : button.Text;
        }

        private void SetStatus(string text)
        {
            this.lblStatus.Text = text;
        }

        private static string N(int value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
