using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OrderDesk.Domain;

namespace LegacyOrderDesk
{
    /// <summary>
    /// The modal edit dialog (Customer / Owner / Status). Returns DialogResult.OK with the edited
    /// copy in <see cref="Order"/>. In WinForms the process exit cleaned up forgotten dialogs; in a
    /// server-hosted app the caller must Dispose it (Module 3).
    /// </summary>
    public class EditOrderDialog : Form
    {
        private readonly ComboBox _customer;
        private readonly ComboBox _owner;
        private readonly ComboBox _status;
        private readonly OrderService _service;

        public Order Order { get; }

        public EditOrderDialog(Order order, OrderService service)
        {
            _service = service;
            Order = order.Clone();

            Text = "Edit Order " + order.Id;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false; MinimizeBox = false;
            ClientSize = new Size(380, 260);

            _customer = AddField("Customer", 20, service.Customers.Select(c => c.Name).ToArray(), order.CustomerName);
            _owner = AddField("Owner", 90, service.Owners.ToArray(), order.Owner);
            _status = AddField("Status", 160, Enum.GetNames(typeof(OrderStatus)), order.Status.ToString());

            var cancel = new Button { Text = "Cancel", Location = new Point(170, 218), Size = new Size(90, 30), DialogResult = DialogResult.Cancel };
            var save = new Button { Text = "Save", Location = new Point(270, 218), Size = new Size(90, 30) };
            save.Click += Save_Click;
            Controls.Add(cancel); Controls.Add(save);
            AcceptButton = save; CancelButton = cancel;
        }

        private ComboBox AddField(string label, int top, string[] items, string value)
        {
            Controls.Add(new Label { Text = label, Location = new Point(20, top), AutoSize = true });
            var combo = new ComboBox { Location = new Point(20, top + 20), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };
            combo.Items.AddRange(items);
            combo.SelectedItem = value;
            Controls.Add(combo);
            return combo;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Order.Customer = _service.Customers.First(c => c.Name == (string)_customer.SelectedItem);
            Order.Owner = (string)_owner.SelectedItem;
            Order.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), (string)_status.SelectedItem);

            var result = new OrderValidator().Validate(Order);
            if (result.HasErrors)
            {
                // ✕ a blocking MessageBox for a validation failure is acceptable; for "Saved." it is not
                MessageBox.Show(string.Join(Environment.NewLine, result.Errors.Values), "Cannot save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
