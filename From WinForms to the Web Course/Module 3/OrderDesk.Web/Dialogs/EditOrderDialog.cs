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
    /// The caller awaits ShowDialogAsync and disposes the dialog.
    /// </summary>
    public partial class EditOrderDialog : Form
    {
        /// <summary>A private copy of the order being edited; the caller saves it on OK.</summary>
        public Order Order { get; private set; }

        public EditOrderDialog(Order order, IList<Customer> customers)
        {
            if (customers == null) throw new ArgumentNullException(nameof(customers));
            InitializeComponent();

            customerComboBox.Items.AddRange(customers.Cast<object>().ToArray());
            statusComboBox.Items.AddRange(Enum.GetValues(typeof(OrderStatus)).Cast<object>().ToArray());
            Bind(order);
        }

        /// <summary>
        /// Loads an order into the fields and resets the dialog state (call it again before
        /// re-showing an instance that is reused on purpose).
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
                // A validation message stays modal: Save must not continue.
                MessageBox.Show("Select a customer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Order.Customer = customer;
            Order.CustomerId = customer.Id;
            Order.Owner = ownerTextBox.Text.Trim();
            Order.PoNumber = poTextBox.Text.Trim();
            Order.Status = (OrderStatus)statusComboBox.SelectedItem;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
