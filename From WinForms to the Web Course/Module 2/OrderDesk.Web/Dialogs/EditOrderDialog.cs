using System;
using System.Collections.Generic;
using System.Linq;
using Wisej.Web;
using OrderDesk.Domain;

namespace OrderDesk.Dialogs
{
    /// <summary>
    /// The modal edit dialog: customer, owner, status, PO. Returns DialogResult.OK on Save.
    /// Ported from LegacyOrderDesk/EditOrderDialog.cs by the namespace swap alone.
    /// </summary>
    public partial class EditOrderDialog : Form
    {
        public Order Order { get; }

        public EditOrderDialog(Order order, IList<Customer> customers)
        {
            InitializeComponent();
            Order = order.Clone();
            customerComboBox.DataSource = customers.ToList();
            customerComboBox.DisplayMember = "Name";
            customerComboBox.SelectedItem = customers.FirstOrDefault(c => c.Id == Order.CustomerId);
            ownerTextBox.Text = Order.Owner;
            poTextBox.Text = Order.PoNumber;
            statusComboBox.DataSource = Enum.GetValues(typeof(OrderStatus));
            statusComboBox.SelectedItem = Order.Status;
            Text = Order.Id == 0 ? "New Order" : $"Edit Order {Order.Id}";
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (customerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Select a customer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var customer = (Customer)customerComboBox.SelectedItem;
            Order.Customer = customer;
            Order.CustomerId = customer.Id;
            Order.Owner = ownerTextBox.Text.Trim();
            Order.PoNumber = poTextBox.Text.Trim();
            Order.Status = (OrderStatus)statusComboBox.SelectedItem;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
