using System;
using System.Threading.Tasks;
using OperationsConsole.Services;
using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// The <b>Editors</b> section (Module 2 · Editors, Buttons, Validation, and Feedback).
    ///
    /// The page hosts the <see cref="Editors.CustomerEditor"/> UserControl and adds the command row that makes
    /// every path of the lab clickable: a valid sample, a sample that breaks four rules at once, the
    /// "Simulate service failure" switch and a "Save twice" button that proves the busy state swallows the
    /// second submit. It owns the <see cref="CustomerService"/> instance and hands it to the editor, so the
    /// switch and the editor talk to the same back end.
    ///
    /// What the page knows about the editor is exactly its public surface — LoadCustomer, ValidateContent,
    /// SaveAsync, Reset, IsDirty, Customer, Saved, ValidationFailed. It never touches txtEmail, cboStatus or
    /// the ErrorProvider, so the email rule can move into a service without changing one line here.
    /// </summary>
    public partial class EditorsPage : UserControl, ISection
    {
        private readonly CustomerService _service = new CustomerService();

        public EditorsPage()
        {
            InitializeComponent();

            customerEditor.Service = _service;
            customerEditor.Saved += customerEditor_Saved;
            customerEditor.ValidationFailed += customerEditor_ValidationFailed;
            customerEditor.LoadCustomer(_service.CreateBlank());

            ConsoleLog.Add("EditorsPage → CustomerEditor hosted (Dock = Fill) · 6 editors, 1 ErrorProvider, 3 commands");
        }

        /// <inheritdoc/>
        public string Title => "Editors";

        /// <inheritdoc/>
        public void RefreshSection()
        {
            var id = customerEditor.Customer.Id;
            var stored = _service.Get(id);

            if (stored != null)
            {
                customerEditor.LoadCustomer(stored);
                ConsoleLog.Add("EditorsPage.RefreshSection() → reloaded " + stored.Id + " from CustomerService");
                ConsoleLog.Record(stored.Id);
            }
            else
            {
                customerEditor.LoadCustomer(_service.CreateBlank());
                ConsoleLog.Add("EditorsPage.RefreshSection() → nothing saved yet, the editor is back to a blank record");
                ConsoleLog.Record(null);
            }
        }

        // ------------------------------------------------------------------------------------------------
        // Command row
        // ------------------------------------------------------------------------------------------------

        /// <summary>Success path: a customer that passes every editor rule and every business rule.</summary>
        private void btnLoadSample_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnLoadSample.Name);

            var sample = _service.CreateSample();
            customerEditor.LoadCustomer(sample);

            ConsoleLog.Add("btnLoadSample → loaded \"" + sample.Name + "\" (status "
                + CustomerService.TextOf(CustomerService.StatusOptions(), sample.StatusKey)
                + ", stored key " + sample.StatusKey + ")");
            ConsoleLog.Status("A valid sample is loaded — press Validate or Save.", StatusLevel.Ok);
        }

        /// <summary>
        /// Failure paths in one click: no name, a malformed email, a credit limit under the minimum an active
        /// customer needs and a start date in the future. ValidateContent() runs straight away so all four
        /// ErrorProvider marks appear together.
        /// </summary>
        private void btnLoadInvalidSample_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnLoadInvalidSample.Name);

            customerEditor.LoadCustomer(_service.CreateInvalidSample());
            ConsoleLog.Add("btnLoadInvalidSample → loaded a record that breaks four rules, running ValidateContent() so every mark is visible at once");

            var result = customerEditor.ValidateContent();
            ConsoleLog.Status(result.Summary + " — each mark says what to fix.", StatusLevel.Warning);
        }

        /// <summary>
        /// Duplicate-submit path: two SaveAsync() calls 200 ms apart. The first one runs, the second one hits
        /// the busy state and returns false without touching the service.
        /// </summary>
        private async void btnSaveTwice_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnSaveTwice.Name);
            ConsoleLog.Add("btnSaveTwice → SaveAsync() now, SaveAsync() again in 200 ms");

            var firstCall = customerEditor.SaveAsync();     // started, not awaited yet
            await Task.Delay(200);
            var secondResult = await customerEditor.SaveAsync();
            var firstResult = await firstCall;

            ConsoleLog.Add("btnSaveTwice → first call returned " + firstResult
                + ", second call returned " + secondResult
                + " — one record, not two (" + _service.Count + " in memory)");
        }

        /// <summary>Service failure path and its recovery — a CheckBox is a state control, not a command.</summary>
        private void chkSimulateFailure_CheckedChanged(object sender, EventArgs e)
        {
            _service.SimulateFailure = chkSimulateFailure.Checked;

            ConsoleLog.Add(chkSimulateFailure.Checked
                ? "CustomerService.SimulateFailure = true — the next Save throws after the round trip and writes nothing"
                : "CustomerService.SimulateFailure = false — Save works again (recovery)");
            ConsoleLog.Status(chkSimulateFailure.Checked
                ? "The customer service will refuse the next save."
                : "The customer service is available again.",
                chkSimulateFailure.Checked ? StatusLevel.Warning : StatusLevel.Ok);
        }

        // ------------------------------------------------------------------------------------------------
        // What the editor tells the page — two events, no child controls
        // ------------------------------------------------------------------------------------------------

        private void customerEditor_Saved(object sender, EventArgs e)
        {
            var saved = customerEditor.Customer;
            ConsoleLog.Add("EditorsPage ← CustomerEditor.Saved · " + saved.Id + " (" + saved.Name + ")");
        }

        private void customerEditor_ValidationFailed(object sender, EventArgs e)
        {
            var result = customerEditor.LastValidation;
            ConsoleLog.Add("EditorsPage ← CustomerEditor.ValidationFailed · " + (result == null ? "no detail" : result.Summary));
        }
    }
}
