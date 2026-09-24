using System;
using Wisej.Web;

namespace GlobalDesk
{
    /// <summary>
    /// The customer editor, with the half of its text the designer can never reach.
    ///
    /// Module 2 put the designed captions - the three row labels and the two buttons - into
    /// <c>CustomerEditor.resx</c> and its German companion. They arrive when the control is
    /// constructed.
    ///
    /// Module 3 is about everything else on this screen: the last-order sentence, the validation
    /// message and the save confirmation. None of them is a designed property of a control. They
    /// are produced while the application runs, they come from the shared
    /// <c>Resources/Strings.resx</c> through <see cref="Texts"/>, and because a shared resource is
    /// read on every lookup they follow a culture change without a rebuild.
    /// </summary>
    public partial class CustomerEditor : UserControl
    {
        // Customer data. Real types, no formatting: the same values in every language.
        private const string CustomerName = "Anna Weber";
        private static readonly DateTime LastOrderDate = new DateTime(2026, 9, 14);

        public CustomerEditor()
        {
            InitializeComponent();

            this.txtName.Text = CustomerName;
            this.txtEmail.Text = "a.weber@globaldesk.io";
            this.txtNotes.Text = "Priority account";

            ShowLastOrder();
        }

        /// <summary>The culture this instance was constructed under - see Module 2.</summary>
        public string BuiltForCulture { get; } = Application.CurrentCulture.Name;

        /// <summary>
        /// One resource string, two placeholders, one <c>string.Format</c>.
        ///
        /// <c>"{0} last ordered on {1}."</c> becomes <c>"{0} hat zuletzt am {1} bestellt."</c> -
        /// the date moves into the middle of the sentence and the verb goes to the end. No amount
        /// of concatenating <c>Texts.Get("LastOrderPrefix") + date</c> can produce that, which is
        /// why a sentence is one resource and never three fragments.
        ///
        /// The culture is passed to <c>string.Format</c> as well as to <c>ToString</c>. Without
        /// it, <c>string.Format</c> uses the thread's culture, which in a server application is
        /// whatever that thread last happened to be doing.
        /// </summary>
        private void ShowLastOrder()
        {
            var culture = Application.CurrentCulture;

            this.lblLastOrder.Text = string.Format(culture,
                Texts.Get("Customer.LastOrder"),
                CustomerName,
                LastOrderDate.ToString("d", culture));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // The domain decides what happened. It returns a value, not a sentence: a service
            // that returned "Customer saved." would have chosen a language for every caller.
            var result = CustomerSaveService.Save(this.txtName.Text, this.txtEmail.Text);
            var message = Texts.Get(CustomerSaveService.ResourceKeyFor(result));

            if (result == SaveResult.Saved)
            {
                ShowValidation(null);
                ShowMessage(message);

                // A Wisej.NET dialog, so the buttons on it are the framework's own text rather
                // than ours - see docs/TextKinds.md.
                MessageBox.Show(message, Texts.Get("App.ProductName"),
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                return;
            }

            ShowMessage(null);
            ShowValidation(message);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.txtName.Text = CustomerName;
            this.txtEmail.Text = "a.weber@globaldesk.io";
            this.txtNotes.Text = "Priority account";
            ShowValidation(null);
            ShowMessage(null);
        }

        /// <summary>The message beside the field that failed, and the field's own red border.</summary>
        private void ShowValidation(string message)
        {
            this.lblValidation.Text = message ?? string.Empty;
            this.txtName.CssStyle = message == null
                ? "border:1.5px solid #c9d4e0;border-radius:6px"
                : "border:1.5px solid #d93b3b;border-radius:6px;background:#fff6f6";
        }

        /// <summary>The save result, as a pill. Green when it worked, amber when the lookup came
        /// back bracketed - a bracketed value is a content defect, not a success.</summary>
        private void ShowMessage(string message)
        {
            this.lblMessage.Visible = message != null;
            if (message == null)
                return;

            var missing = message.StartsWith("[", StringComparison.Ordinal);

            this.lblMessage.Text = message;
            this.lblMessage.Font = missing ? Desk.Mono(13.5F, System.Drawing.FontStyle.Bold)
                                           : Desk.Px(13.5F, System.Drawing.FontStyle.Bold);
            this.lblMessage.BackColor = missing ? Desk.WarnBack
                                                : System.Drawing.Color.FromArgb(0xE8, 0xF7, 0xEE);
            this.lblMessage.ForeColor = missing ? Desk.WarnInk
                                                : System.Drawing.Color.FromArgb(0x1F, 0x7A, 0x4D);
            this.lblMessage.CssStyle = missing
                ? "border:1px solid #e8c48a;border-radius:6px"
                : "border:1px solid #9fd9b8;border-radius:6px";
        }
    }
}
