using System;
using Wisej.Web;
using ValidationClinic.Models;

namespace ValidationClinic
{
    // A separate comparison surface keeps automatic IDataErrorInfo icons independent of manual rules.
    public partial class DataBoundErrorsForm : Form
    {

        public DataBoundErrorsForm()
        {
            InitializeComponent();
            txtName.DataBindings.Add("Text", contactBindingSource, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
            txtEmail.DataBindings.Add("Text", contactBindingSource, "Email", true, DataSourceUpdateMode.OnPropertyChanged);
            SwitchModel();
        }

        private void SwitchModel()
        {
            // New BindingSource demonstrates an actual provider-source replacement at runtime.
            var previous = contactBindingSource;
            contactBindingSource = new BindingSource(components) { DataSource = new ContactErrorModel() };
            txtName.DataBindings.Clear(); txtEmail.DataBindings.Clear();
            txtName.DataBindings.Add("Text", contactBindingSource, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
            txtEmail.DataBindings.Add("Text", contactBindingSource, "Email", true, DataSourceUpdateMode.OnPropertyChanged);
            errorProvider.BindToDataAndErrors(contactBindingSource, string.Empty);
            previous.Dispose();
            lblSummary.Text = "Blank Name and Email are model errors. Type values, then Check model. This comparison never saves.";
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            SwitchModel();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            contactBindingSource.EndEdit();
            foreach (Binding binding in txtName.DataBindings) binding.WriteValue();
            foreach (Binding binding in txtEmail.DataBindings) binding.WriteValue();
            errorProvider.UpdateBinding();
            var current = (ContactErrorModel)contactBindingSource.Current;
            lblSummary.Text = string.Join(Environment.NewLine, new[] { current["Name"], current["Email"] });
        }
    }
}
