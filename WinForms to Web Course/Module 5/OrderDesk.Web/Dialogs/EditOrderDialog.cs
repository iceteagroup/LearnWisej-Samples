using System;
using System.Linq;
using OrderDesk.Domain;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk.Dialogs
{
    /// <summary>
    /// The WinForms EditOrderDialog ported to <see cref="Wisej.Web.Form"/> (Module 3) and given
    /// field-level validation (Module 5): <c>OrderValidator.Validate</c> — the reusable server-side rule with
    /// no UI dependency — runs on Save and its messages land on the offending controls through an
    /// <see cref="ErrorProvider"/> instead of a MessageBox. The dialog never saves; it returns
    /// <c>DialogResult.OK</c> with the edited copy in <see cref="Order"/> and the caller calls
    /// <c>OrderService.Save</c> (which validates again — the service is the boundary, the dialog is a courtesy).
    /// </summary>
    public sealed class EditOrderDialog : Form
    {
        private const string NoOwner = "(none)";

        private readonly System.ComponentModel.IContainer components;
        private readonly ErrorProvider errorProvider;
        private readonly ComboBox customerCombo;
        private readonly ComboBox ownerCombo;
        private readonly ComboBox statusCombo;
        private readonly NumericUpDown quantityBox;
        private readonly Label quantityCaption;
        private readonly Label totalLabel;
        private readonly Label errorSummary;
        private readonly Button saveButton;
        private readonly Button cancelButton;
        private readonly OrderService _service;

        /// <summary>The edited copy (the original is untouched until the caller saves).</summary>
        public Order Order { get; }

        /// <summary>Trace sink: (name, payload) for every validation pass.</summary>
        public event Action<string, string> Trace;

        /// <summary>Raised with the result of every OrderValidator pass (errors or "valid").</summary>
        public event Action<ValidationResult> ValidationChecked;

        public EditOrderDialog(Order order, OrderService service)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            Order = order.Clone();
            if (Order.Lines.Count == 0)
                Order.Lines.Add(new OrderLine { Sku = "LAMP-01", Description = "Desk lamp", Quantity = 1, UnitPrice = 48m });

            components = new System.ComponentModel.Container();
            errorProvider = new ErrorProvider(components) { ContainerControl = this };

            this.Text = "Edit Order " + order.Id + "  ·  " + order.CustomerName;
            this.Size = new System.Drawing.Size(460, 420);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Palette.CardBackground;

            customerCombo = AddField("Customer", 18, _service.Customers.Select(c => c.Name).ToArray(), order.CustomerName);
            ownerCombo = AddField("Owner", 86, new[] { NoOwner }.Concat(_service.Owners).ToArray(), order.Owner ?? NoOwner);
            statusCombo = AddField("Status", 154, Enum.GetNames(typeof(OrderStatus)), order.Status.ToString());

            var first = Order.Lines[0];
            quantityCaption = new Label
            {
                AutoSize = false,
                Location = new System.Drawing.Point(24, 222),
                Size = new System.Drawing.Size(392, 18),
                Text = "Quantity — first line " + first.Description + " @ " + first.UnitPrice.ToString("C2"),
                ForeColor = Palette.MutedText,
                Font = new System.Drawing.Font("default", 9F),
            };
            quantityBox = new NumericUpDown
            {
                Location = new System.Drawing.Point(24, 242),
                Size = new System.Drawing.Size(150, 30),
                Minimum = 0,
                Maximum = 1_000_000,
                Value = first.Quantity,
                ToolTipText = "Push it up to exceed the customer's credit limit; 0 breaks the line rule",
            };
            quantityBox.ValueChanged += (s, e) => Recalculate();
            totalLabel = new Label
            {
                AutoSize = false,
                Location = new System.Drawing.Point(190, 242),
                Size = new System.Drawing.Size(226, 30),
                Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                ForeColor = Palette.Ink,
            };
            errorSummary = new Label
            {
                AutoSize = false,
                Location = new System.Drawing.Point(24, 286),
                Size = new System.Drawing.Size(392, 40),
                ForeColor = Palette.Bad,
                Font = new System.Drawing.Font("default", 9F),
                Visible = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            };
            cancelButton = new Button { Text = "Cancel", Location = new System.Drawing.Point(226, 334), Size = new System.Drawing.Size(90, 32) };
            cancelButton.Click += (s, e) => { Trace?.Invoke("EditOrderDialog", "Cancel → DialogResult.Cancel"); this.DialogResult = DialogResult.Cancel; this.Close(); };
            saveButton = new Button { Text = "Save", Location = new System.Drawing.Point(326, 334), Size = new System.Drawing.Size(90, 32) };
            saveButton.Click += Save_Click;

            this.Controls.Add(quantityCaption);
            this.Controls.Add(quantityBox);
            this.Controls.Add(totalLabel);
            this.Controls.Add(errorSummary);
            this.Controls.Add(cancelButton);
            this.Controls.Add(saveButton);
            this.AcceptButton = saveButton;
            this.CancelButton = cancelButton;

            customerCombo.SelectedIndexChanged += (s, e) => Recalculate();
            Recalculate();
        }

        private ComboBox AddField(string caption, int top, string[] items, string value)
        {
            this.Controls.Add(new Label
            {
                AutoSize = false,
                Text = caption,
                Location = new System.Drawing.Point(24, top),
                Size = new System.Drawing.Size(392, 18),
                ForeColor = Palette.MutedText,
                Font = new System.Drawing.Font("default", 9F),
            });
            var combo = new ComboBox
            {
                Location = new System.Drawing.Point(24, top + 20),
                Size = new System.Drawing.Size(392, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
            };
            combo.Items.AddRange(items);
            combo.SelectedItem = value;
            this.Controls.Add(combo);
            return combo;
        }

        /// <summary>Pushes the form values into the order copy (no validation here).</summary>
        private void ApplyToOrder()
        {
            var customerName = customerCombo.SelectedItem as string;
            Order.Customer = _service.Customers.FirstOrDefault(c => c.Name == customerName);
            var owner = ownerCombo.SelectedItem as string;
            Order.Owner = owner == NoOwner ? null : owner;
            var status = statusCombo.SelectedItem as string;
            if (status != null) Order.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), status);
            Order.Lines[0].Quantity = (int)quantityBox.Value;
        }

        private void Recalculate()
        {
            ApplyToOrder();
            var limit = Order.Customer?.CreditLimit ?? 0m;
            totalLabel.Text = "Total " + Order.Total.ToString("C2");
            totalLabel.ForeColor = Order.Customer != null && Order.Total > limit ? Palette.Bad : Palette.Ink;
            totalLabel.ToolTipText = Order.Customer == null ? "" : "Credit limit " + limit.ToString("C2") + (Order.Customer.Tier == "Gold" ? " · Gold: 5% off from $15,000" : "");
        }

        private void Save_Click(object sender, EventArgs e)
        {
            ApplyToOrder();
            errorProvider.Clear();
            errorSummary.Visible = false;

            // ✓ the reusable rule — no UI dependency; the same class runs inside OrderService.Save
            var result = new OrderValidator().Validate(Order);
            ValidationChecked?.Invoke(result);

            if (result.HasErrors)
            {
                // field-level messages, not a MessageBox
                foreach (var error in result.Errors)
                    errorProvider.SetError(ControlFor(error.Key), error.Value);
                errorSummary.Text = result.Errors.Count + (result.Errors.Count == 1 ? " field needs attention: " : " fields need attention: ") + string.Join(", ", result.Errors.Keys);
                errorSummary.Visible = true;
                Trace?.Invoke("OrderValidator.Validate", result.Errors.Count + " error(s) → ErrorProvider.SetError on " + string.Join(", ", result.Errors.Keys) + " (dialog stays open)");
                return;
            }

            Trace?.Invoke("OrderValidator.Validate", "valid → DialogResult.OK");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>Maps the validator's field names onto the controls that show the message.</summary>
        private Control ControlFor(string field)
        {
            switch (field)
            {
                case "Customer": return customerCombo;
                case "Owner": return ownerCombo;
                case "Lines": return quantityBox;
                case "Total": return quantityBox;
                default: return statusCombo;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }
    }
}
