using System;
using System.Collections.Generic;
using System.Linq;
using OrderDesk.Domain;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk.Dialogs
{
    /// <summary>
    /// The ported edit dialog. Same fields as LegacyOrderDesk.EditOrderDialog plus one editable order
    /// line, but the rule is no longer inside the form: Save collects the order, asks
    /// <see cref="OrderValidator"/> on the server and shows the answer field by field through an
    /// ErrorProvider (general messages as a Toast). The dialog only closes with DialogResult.OK when
    /// the order is valid. The caller disposes the dialog in its ShowDialog callback.
    /// </summary>
    public partial class EditOrderDialog : Form
    {
        private readonly IList<Customer> _customers;

        /// <summary>The order being edited (a clone of the original; the caller saves it).</summary>
        public Order Order { get; }

        public EditOrderDialog(Order order, IList<Customer> customers)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            InitializeComponent();

            _customers = customers ?? throw new ArgumentNullException(nameof(customers));
            Order = order.Clone();

            comboCustomer.Items.AddRange(_customers.Cast<object>().ToArray());
            comboCustomer.SelectedItem = _customers.FirstOrDefault(c => c.Id == Order.CustomerId);
            textPo.Text = Order.PoNumber ?? "";
            textOwner.Text = Order.Owner ?? "";
            comboStatus.Items.AddRange(Enum.GetNames(typeof(OrderStatus)).Cast<object>().ToArray());
            comboStatus.SelectedItem = Order.Status.ToString();

            var line = Order.Lines.FirstOrDefault();
            textSku.Text = line?.Sku ?? "";
            numQuantity.Value = line?.Quantity ?? 1;
            numUnitPrice.Value = line?.UnitPrice ?? 0m;
            RefreshTotal();

            Text = Order.Id == 0 ? "New Order" : $"Edit Order {Order.Id}";
        }

        /// <summary>The line total as the user types — display only, the rule still runs on Save.</summary>
        private void line_ValueChanged(object sender, EventArgs e) => RefreshTotal();

        private void RefreshTotal()
        {
            labelTotalValue.Text = (numQuantity.Value * numUnitPrice.Value).ToString("N2");
        }

        /// <summary>Copies the form fields into <see cref="Order"/>. No rule here — that is the validator's job.</summary>
        private void CollectOrder()
        {
            var customer = comboCustomer.SelectedItem as Customer;
            Order.Customer = customer;
            Order.CustomerId = customer?.Id ?? 0;
            Order.PoNumber = textPo.Text.Trim();
            Order.Owner = textOwner.Text.Trim();
            Order.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), (string)comboStatus.SelectedItem ?? "Open");

            Order.Lines.Clear();
            string sku = textSku.Text.Trim();
            if (sku.Length > 0)
            {
                // A line without a SKU is not a line.
                Order.Lines.Add(new OrderLine
                {
                    Sku = sku, Description = sku, Quantity = (int)numQuantity.Value, UnitPrice = numUnitPrice.Value
                });
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            CollectOrder();

            var result = OrderValidator.Validate(Order);

            errorProvider.Clear();
            if (result.HasErrors)
            {
                ShowFieldErrors(result);
                foreach (var message in result.General)
                    Ui.Toast(message, MessageBoxIcon.Warning);
                return;                                  // Save stays blocked; the dialog stays open
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>Field name → control. Anything the map doesn't know becomes a general message.</summary>
        private void ShowFieldErrors(ValidationResult result)
        {
            foreach (var error in result.Errors)
            {
                var control = ControlFor(error.Key);
                if (control != null)
                    errorProvider.SetError(control, error.Value);
                else
                    Ui.Toast(error.Value, MessageBoxIcon.Warning);
            }
        }

        private Control ControlFor(string field)
        {
            switch (field)
            {
                case OrderValidator.CustomerField: return comboCustomer;
                case OrderValidator.PoNumberField: return textPo;
                case OrderValidator.QuantityField: return numQuantity;
                case OrderValidator.UnitPriceField: return numUnitPrice;
                case OrderValidator.TotalField: return labelTotalValue;
                default: return null;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
