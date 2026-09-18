using System;
using System.ComponentModel;
using System.Linq;
using Wisej.Web;
using ValidationClinic.Models;
using ValidationClinic.Services;

namespace ValidationClinic
{
    public partial class MainPage : Page
    {
        private readonly ContactRepository repository = new();

        private BindingList<ContactEditModel> workingRows;

        public MainPage()
        {
            InitializeComponent();

            Reload();
            Trace("session", "Repository belongs to this page. A new session starts with its own seed data.");
        }

        private void Reload()
        {
            workingRows = new BindingList<ContactEditModel>(repository.Snapshot());
            gridSource.DataSource = workingRows;
            lblStatus.Text = $"Saved contacts: {repository.Snapshot().Count}. Repository writes: {repository.WriteCount}.";

        }
        internal void Trace(string layer, string message)
        {
            listTrace.Items.Add($"{DateTime.Now:HH:mm:ss} | {layer}: {message}");
            while (listTrace.Items.Count > 200) listTrace.Items.RemoveAt(0);
            listTrace.SelectedIndex = listTrace.Items.Count - 1;
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            gridContacts.CancelEdit();
            Reload();
            Trace("discard", "Working grid reset from persisted snapshot.");
        }

        private void btnFailure_Click(object sender, EventArgs e)
        {
            repository.FailNextSave = true;
            Trace("demo", "Next valid write will fail; invalid attempts do not consume it.");
        }

        private void btnIntake_Click(object sender, EventArgs e)
        {
            var form = new CustomerIntakeForm(repository, Trace);
            form.Saved += (_, _) => Reload();
            form.ShowDialog();
        }

        private void btnBindingDemo_Click(object sender, EventArgs e)
        {
            new DataBoundErrorsForm().ShowDialog();
        }
    }
}
