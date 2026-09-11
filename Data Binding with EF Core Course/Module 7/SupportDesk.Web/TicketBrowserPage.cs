using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupportDesk.Services;
using Wisej.Services;
using Wisej.Web;

namespace SupportDesk.Web
{
    public partial class TicketBrowserPage : Page
    {
        private const int PageSize = 50;

        // The "All" lookup rows carry these keys; they become a null criterion.
        private const int AllCustomersId = 0;
        private const string AllStatusesKey = "";

        // Resolved through Microsoft DI: Startup.cs registered app.Services with Wisej.NET.
        [Inject]
        private TicketQueryService TicketQueries { get; set; }

        // The loading guard: one database operation per page at a time.
        private bool _loading;

        private int _pageIndex;
        private int _totalCount;

        public TicketBrowserPage()
        {
            InitializeComponent();
        }

        private async void TicketBrowserPage_Load(object sender, EventArgs e)
        {
            // There is no sign-in in this sample: cboRole stands in for the user's role claim.
            var roleRows = new List<NamedValue> { new NamedValue(nameof(UserRole.Agent), "Agent"), new NamedValue(nameof(UserRole.Supervisor), "Supervisor") };
            cboRole.DisplayMember = nameof(NamedValue.Name);
            cboRole.ValueMember = nameof(NamedValue.Value);
            cboRole.DataSource = roleRows;
            cboRole.SelectedIndex = 0;

            // The lookups come first: the criteria read SelectedValue from them.
            await LoadLookupsAsync();
            await LoadTicketsAsync();
            Application.Update(this);
        }

        #region Search and paging

        private async void searchButton_Click(object sender, EventArgs e)
        {
            _pageIndex = 0;
            await LoadTicketsAsync();
        }

        private async void nextPageButton_Click(object sender, EventArgs e)
        {
            _pageIndex++;
            await LoadTicketsAsync();
        }

        private async void prevPageButton_Click(object sender, EventArgs e)
        {
            _pageIndex = Math.Max(0, _pageIndex - 1);
            await LoadTicketsAsync();
        }

        private async Task LoadTicketsAsync()
        {
            if (_loading)
                return;

            try
            {
                _loading = true;
                SetBusy(true);
                statusLabel.Text = "Loading tickets...";

                var result = await TicketQueries.SearchTicketsAsync(ReadCriteria());

                _totalCount = result.TotalCount;
                ticketBindingSource.DataSource = result.Items.ToList();
                ticketBindingSource.ResetBindings(false);

                if (result.TotalCount == 0)
                {
                    _pageIndex = 0;
                    statusLabel.Text = "No tickets match these filters";
                }
                else
                {
                    statusLabel.Text = $"Showing {result.Items.Count} of {result.TotalCount} tickets · page {_pageIndex + 1} of {result.PageCount(PageSize)} · page size {PageSize}";
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[SupportDesk] Ticket search failed: " + ex);
                AlertBox.Show("Tickets could not be loaded. Your filters are unchanged. Please try again in a moment.",
                    MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                statusLabel.Text = "Tickets could not be loaded";
            }
            finally
            {
                _loading = false;
                SetBusy(false);
                Application.Update(this);
            }
        }

        private TicketSearchCriteria ReadCriteria()
        {
            var status = statusComboBox.SelectedValue is string s && s != AllStatusesKey ? s : null;
            var customerId = customerComboBox.SelectedValue is int id && id != AllCustomersId ? id : (int?)null;

            return new TicketSearchCriteria
            {
                Text = searchTextBox.Text,
                Status = status,
                CustomerId = customerId,
                PageIndex = _pageIndex,
                PageSize = PageSize
            };
        }

        private async Task LoadLookupsAsync()
        {
            try
            {
                var statuses = await TicketQueries.GetStatusesAsync();
                var customers = await TicketQueries.GetCustomersAsync();

                var statusRows = new List<LookupText> { new LookupText(AllStatusesKey, "All statuses") };
                statusRows.AddRange(statuses.Select(status => new LookupText(status, status)));
                statusComboBox.DisplayMember = nameof(LookupText.Name);
                statusComboBox.ValueMember = nameof(LookupText.Key);
                statusComboBox.DataSource = statusRows;
                statusComboBox.SelectedIndex = 0;

                var customerRows = new List<LookupItem> { new LookupItem(AllCustomersId, "All customers") };
                customerRows.AddRange(customers);
                customerComboBox.DisplayMember = nameof(LookupItem.Name);
                customerComboBox.ValueMember = nameof(LookupItem.Id);
                customerComboBox.DataSource = customerRows;
                customerComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[SupportDesk] Lookups not loaded: " + ex);
                AlertBox.Show("The filter lists could not be loaded. Search still works without them.",
                    MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
        }

        /// <summary>A status lookup row: a string key instead of an integer one.</summary>
        private sealed class LookupText
        {
            public LookupText(string key, string name)
            {
                Key = key;
                Name = name;
            }

            public string Key { get; }

            public string Name { get; }
        }

        #endregion

        #region Add and Edit

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            await OpenEditorAsync(null);
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            if (ticketsDataGridView.CurrentRow?.DataBoundItem is TicketListItem selected)
                await OpenEditorAsync(selected.Id);
        }

        /// <summary>Opens the modal editor and re-runs the search only when it closes with OK.</summary>
        private async Task OpenEditorAsync(int? ticketId)
        {
            if (_loading)
                return;

            DialogResult result;
            using (var editor = new TicketEditorForm(ticketId, ReadRole()))
            {
                result = await editor.ShowDialogAsync();
            }

            if (result == DialogResult.OK)
            {
                _pageIndex = 0;
                await LoadTicketsAsync();
            }
        }

        private UserRole ReadRole()
            => cboRole.SelectedValue is string s && Enum.TryParse<UserRole>(s, out var role) ? role : UserRole.Agent;

        #endregion

        private void SetBusy(bool busy)
        {
            searchButton.Enabled = !busy;
            prevPageButton.Enabled = !busy;
            nextPageButton.Enabled = !busy;
            btnAdd.Enabled = !busy;
            btnEdit.Enabled = !busy;

            // Previous on page 1 and Next on the last page stay off even when nothing is running.
            if (!busy)
                UpdatePagingButtons();
        }

        private void UpdatePagingButtons()
        {
            var pageCount = _totalCount <= 0 ? 1 : (_totalCount + PageSize - 1) / PageSize;
            prevPageButton.Enabled = _pageIndex > 0;
            nextPageButton.Enabled = _pageIndex + 1 < pageCount;
        }
    }
}
