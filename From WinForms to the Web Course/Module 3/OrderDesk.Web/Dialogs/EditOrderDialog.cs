using System;
using System.Collections.Generic;
using System.Linq;
using OrderDesk.Domain;
using Wisej.Web;

namespace OrderDesk.Dialogs
{
    /// <summary>
    /// The modal edit dialog, ported from LegacyOrderDesk/EditOrderDialog.cs. Same fields, same
    /// DialogResult contract (OK on Save, Cancel otherwise), same AcceptButton/CancelButton.
    ///
    /// What changed for the web:
    ///  - ✓ ShowDialog does not block: the caller passes a callback (or awaits ShowDialogAsync) and
    ///    reads the result there — see Screens/OrdersScreen.EditOrder.
    ///  - ✓ Closing does not dispose: the caller owns the lifetime and disposes a transient dialog
    ///    in that callback. Dispose(bool) lives here (not in the .Designer.cs) so the DialogTracker
    ///    bookkeeping stays in hand-written code and the designer file stays designer-owned.
    ///  - ✓ Bind(order) is public so a caller that reuses one instance on purpose can reset it.
    /// </summary>
    public partial class EditOrderDialog : Form
    {
        private readonly string _sessionId;   // captured now: Dispose may run outside a request
        private bool _released;

        /// <summary>A private copy of the order being edited; the caller saves it on OK.</summary>
        public Order Order { get; private set; }

        public EditOrderDialog(Order order, IList<Customer> customers)
        {
            if (customers == null) throw new ArgumentNullException(nameof(customers));
            InitializeComponent();

            customerComboBox.Items.AddRange(customers.Cast<object>().ToArray());
            statusComboBox.Items.AddRange(Enum.GetValues(typeof(OrderStatus)).Cast<object>().ToArray());
            Bind(order);

            _sessionId = Application.SessionId;
            DialogTracker.Opened(_sessionId);
        }

        /// <summary>
        /// Loads an order into the fields and resets the dialog state. Called by the constructor and
        /// again by callers that reuse the instance ("reuse on purpose: make it visible + reset state").
        /// </summary>
        public void Bind(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            Order = order.Clone();

            customerComboBox.SelectedIndex = IndexOf(customerComboBox, item => (item as Customer)?.Id == Order.CustomerId);
            ownerTextBox.Text = Order.Owner ?? "";
            poTextBox.Text = Order.PoNumber ?? "";
            statusComboBox.SelectedIndex = IndexOf(statusComboBox, item => Equals(item, Order.Status));

            DialogResult = DialogResult.None;
            Text = Order.Id == 0 ? "New Order" : $"Edit Order {Order.Id}";
        }

        private static int IndexOf(ComboBox comboBox, Func<object, bool> match)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
                if (match(comboBox.Items[i])) return i;
            return -1;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!(customerComboBox.SelectedItem is Customer customer))
            {
                // ✓ kept modal on purpose: the user must fix the input before Save can continue (docs/NotificationsReview.md).
                MessageBox.Show("Select a customer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Order.Customer = customer;
            Order.CustomerId = customer.Id;
            Order.Owner = ownerTextBox.Text.Trim();
            Order.PoNumber = poTextBox.Text.Trim();
            Order.Status = (OrderStatus)statusComboBox.SelectedItem;

            // ✓ the same contract as WinForms: set the result, close; the caller's callback receives it.
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Deterministic release. Nothing calls this automatically when the dialog closes — the
        /// caller does (Screens/OrdersScreen.EditOrder), which is the Module 3 rule.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_released)
            {
                _released = true;
                DialogTracker.Released(_sessionId);
            }
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }
    }
}
