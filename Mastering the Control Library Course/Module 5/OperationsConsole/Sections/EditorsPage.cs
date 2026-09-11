using System;
using OperationsConsole.Services;
using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// The <b>Editors</b> section: hosts the <see cref="Editors.CustomerEditor"/> UserControl and owns the
    /// <see cref="CustomerService"/> it saves through. The page uses only the editor's public surface.
    /// </summary>
    public partial class EditorsPage : UserControl, ISection
    {
        private readonly CustomerService _service = new CustomerService();

        public EditorsPage()
        {
            InitializeComponent();

            customerEditor.Service = _service;
            customerEditor.LoadCustomer(_service.CreateBlank());
        }

        /// <inheritdoc/>
        public string Title => "Editors";

        /// <inheritdoc/>
        public void RefreshSection()
        {
            var stored = _service.Get(customerEditor.Customer.Id);
            customerEditor.LoadCustomer(stored ?? _service.CreateBlank());
            ShellStatus.Record(stored?.Id);
        }

        private void chkSimulateFailure_CheckedChanged(object sender, EventArgs e)
        {
            _service.SimulateFailure = chkSimulateFailure.Checked;
        }
    }
}
