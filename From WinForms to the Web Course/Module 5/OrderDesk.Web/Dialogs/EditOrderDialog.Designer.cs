namespace OrderDesk.Dialogs
{
    partial class EditOrderDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.errorProvider = new Wisej.Web.ErrorProvider(this.components);
            this.labelCustomer = new Wisej.Web.Label();
            this.comboCustomer = new Wisej.Web.ComboBox();
            this.labelPo = new Wisej.Web.Label();
            this.textPo = new Wisej.Web.TextBox();
            this.labelOwner = new Wisej.Web.Label();
            this.textOwner = new Wisej.Web.TextBox();
            this.labelStatus = new Wisej.Web.Label();
            this.comboStatus = new Wisej.Web.ComboBox();
            this.labelLine = new Wisej.Web.Label();
            this.labelSku = new Wisej.Web.Label();
            this.textSku = new Wisej.Web.TextBox();
            this.labelQuantity = new Wisej.Web.Label();
            this.numQuantity = new Wisej.Web.NumericUpDown();
            this.labelUnitPrice = new Wisej.Web.Label();
            this.numUnitPrice = new Wisej.Web.NumericUpDown();
            this.labelTotal = new Wisej.Web.Label();
            this.labelTotalValue = new Wisej.Web.Label();
            this.labelHint = new Wisej.Web.Label();
            this.buttonCancel = new Wisej.Web.Button();
            this.buttonSave = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // header fields  (the same four LegacyOrderDesk.EditOrderDialog had)
            //
            this.labelCustomer.AutoSize = false;
            this.labelCustomer.Location = new System.Drawing.Point(20, 20);
            this.labelCustomer.Name = "labelCustomer";
            this.labelCustomer.Size = new System.Drawing.Size(90, 26);
            this.labelCustomer.Text = "Customer";
            this.labelCustomer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.comboCustomer.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboCustomer.Location = new System.Drawing.Point(116, 20);
            this.comboCustomer.Name = "comboCustomer";
            this.comboCustomer.Size = new System.Drawing.Size(280, 26);
            this.comboCustomer.Watermark = "Select a customer";
            this.labelPo.AutoSize = false;
            this.labelPo.Location = new System.Drawing.Point(20, 56);
            this.labelPo.Name = "labelPo";
            this.labelPo.Size = new System.Drawing.Size(90, 26);
            this.labelPo.Text = "PO #";
            this.labelPo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.textPo.Location = new System.Drawing.Point(116, 56);
            this.textPo.Name = "textPo";
            this.textPo.Size = new System.Drawing.Size(280, 26);
            this.textPo.Watermark = "e.g. NW-88231 (20 characters max)";
            this.labelOwner.AutoSize = false;
            this.labelOwner.Location = new System.Drawing.Point(20, 92);
            this.labelOwner.Name = "labelOwner";
            this.labelOwner.Size = new System.Drawing.Size(90, 26);
            this.labelOwner.Text = "Owner";
            this.labelOwner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.textOwner.Location = new System.Drawing.Point(116, 92);
            this.textOwner.Name = "textOwner";
            this.textOwner.Size = new System.Drawing.Size(280, 26);
            this.textOwner.Watermark = "optional";
            this.labelStatus.AutoSize = false;
            this.labelStatus.Location = new System.Drawing.Point(20, 128);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(90, 26);
            this.labelStatus.Text = "Status";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.comboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboStatus.Location = new System.Drawing.Point(116, 128);
            this.comboStatus.Name = "comboStatus";
            this.comboStatus.Size = new System.Drawing.Size(280, 26);
            //
            // the order line  (new in Module 5 so the quantity / price / total rules have something to check)
            //
            this.labelLine.AutoSize = false;
            this.labelLine.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelLine.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLine.Location = new System.Drawing.Point(20, 170);
            this.labelLine.Name = "labelLine";
            this.labelLine.Size = new System.Drawing.Size(376, 22);
            this.labelLine.Text = "Order line";
            this.labelSku.AutoSize = false;
            this.labelSku.Location = new System.Drawing.Point(20, 196);
            this.labelSku.Name = "labelSku";
            this.labelSku.Size = new System.Drawing.Size(90, 26);
            this.labelSku.Text = "SKU";
            this.labelSku.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.textSku.Location = new System.Drawing.Point(116, 196);
            this.textSku.Name = "textSku";
            this.textSku.Size = new System.Drawing.Size(280, 26);
            this.textSku.Watermark = "clear it to see \"Add at least one order line.\"";
            this.labelQuantity.AutoSize = false;
            this.labelQuantity.Location = new System.Drawing.Point(20, 232);
            this.labelQuantity.Name = "labelQuantity";
            this.labelQuantity.Size = new System.Drawing.Size(90, 26);
            this.labelQuantity.Text = "Quantity";
            this.labelQuantity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.numQuantity.Location = new System.Drawing.Point(116, 232);
            this.numQuantity.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numQuantity.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.numQuantity.Name = "numQuantity";
            this.numQuantity.Size = new System.Drawing.Size(110, 26);
            this.numQuantity.ValueChanged += new System.EventHandler(this.line_ValueChanged);
            this.labelUnitPrice.AutoSize = false;
            this.labelUnitPrice.Location = new System.Drawing.Point(236, 232);
            this.labelUnitPrice.Name = "labelUnitPrice";
            this.labelUnitPrice.Size = new System.Drawing.Size(60, 26);
            this.labelUnitPrice.Text = "Price";
            this.labelUnitPrice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.numUnitPrice.DecimalPlaces = 2;
            this.numUnitPrice.Location = new System.Drawing.Point(302, 232);
            this.numUnitPrice.Minimum = new decimal(new int[] { 1000000, 0, 0, -2147483648 });   // -1,000,000: negatives are allowed so the rule can fire
            this.numUnitPrice.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            this.numUnitPrice.Name = "numUnitPrice";
            this.numUnitPrice.Size = new System.Drawing.Size(94, 26);
            this.numUnitPrice.ValueChanged += new System.EventHandler(this.line_ValueChanged);
            this.labelTotal.AutoSize = false;
            this.labelTotal.Location = new System.Drawing.Point(20, 268);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(90, 26);
            this.labelTotal.Text = "Total";
            this.labelTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelTotalValue.AutoSize = false;
            this.labelTotalValue.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelTotalValue.Location = new System.Drawing.Point(116, 268);
            this.labelTotalValue.Name = "labelTotalValue";
            this.labelTotalValue.Size = new System.Drawing.Size(280, 26);
            this.labelTotalValue.Text = "0.00";
            this.labelTotalValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // labelHint  (where the rule runs)
            //
            this.labelHint.AutoSize = false;
            this.labelHint.Font = new System.Drawing.Font("default", 9F);
            this.labelHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelHint.Location = new System.Drawing.Point(20, 304);
            this.labelHint.Name = "labelHint";
            this.labelHint.Size = new System.Drawing.Size(376, 22);
            this.labelHint.Text = "Save runs OrderValidator on the server; field messages appear next to the control.";
            //
            // buttons
            //
            this.buttonCancel.Location = new System.Drawing.Point(230, 336);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(80, 30);
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            this.buttonSave.Location = new System.Drawing.Point(316, 336);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(80, 30);
            this.buttonSave.Text = "Save";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            //
            // EditOrderDialog
            //
            this.AcceptButton = this.buttonSave;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.labelCustomer);
            this.Controls.Add(this.comboCustomer);
            this.Controls.Add(this.labelPo);
            this.Controls.Add(this.textPo);
            this.Controls.Add(this.labelOwner);
            this.Controls.Add(this.textOwner);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.comboStatus);
            this.Controls.Add(this.labelLine);
            this.Controls.Add(this.labelSku);
            this.Controls.Add(this.textSku);
            this.Controls.Add(this.labelQuantity);
            this.Controls.Add(this.numQuantity);
            this.Controls.Add(this.labelUnitPrice);
            this.Controls.Add(this.numUnitPrice);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.labelTotalValue);
            this.Controls.Add(this.labelHint);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.FormBorderStyle = Wisej.Web.FormBorderStyle.Fixed;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditOrderDialog";
            this.Size = new System.Drawing.Size(420, 420);
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Edit Order";
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.ErrorProvider errorProvider;
        private Wisej.Web.Label labelCustomer;
        private Wisej.Web.ComboBox comboCustomer;
        private Wisej.Web.Label labelPo;
        private Wisej.Web.TextBox textPo;
        private Wisej.Web.Label labelOwner;
        private Wisej.Web.TextBox textOwner;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.ComboBox comboStatus;
        private Wisej.Web.Label labelLine;
        private Wisej.Web.Label labelSku;
        private Wisej.Web.TextBox textSku;
        private Wisej.Web.Label labelQuantity;
        private Wisej.Web.NumericUpDown numQuantity;
        private Wisej.Web.Label labelUnitPrice;
        private Wisej.Web.NumericUpDown numUnitPrice;
        private Wisej.Web.Label labelTotal;
        private Wisej.Web.Label labelTotalValue;
        private Wisej.Web.Label labelHint;
        private Wisej.Web.Button buttonCancel;
        private Wisej.Web.Button buttonSave;
    }
}
