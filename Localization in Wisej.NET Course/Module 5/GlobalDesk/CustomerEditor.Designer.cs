using System.Drawing;
using Wisej.Web;

namespace GlobalDesk
{
    partial class CustomerEditor
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        //
        // Still Localizable. What changed in Module 5 is what the German file has to carry: the
        // row flows and the buttons are AutoSize with a MinimumSize, so a longer caption changes
        // nothing but itself. Compare CustomerEditor.de.resx with the one in Module 4 - the Size
        // and Location overrides are gone.
        //
        // Nothing here sets RightToLeft either. The control's RightToLeftLayout is set in the
        // constructor and every child inherits.
        //
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(CustomerEditor));

            this.pnlCodeRow = new Wisej.Web.FlowLayoutPanel();
            this.lblCustomerCode = new Wisej.Web.Label();
            this.txtCustomerCode = new Wisej.Web.TextBox();
            this.pnlGridHost = new Wisej.Web.Panel();
            this.gridContacts = new Wisej.Web.DataGridView();
            this.colContact = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCompany = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCity = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlButtons = new Wisej.Web.FlowLayoutPanel();
            this.btnSave = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // the customer code row
            //
            resources.ApplyResources(this.lblCustomerCode, "lblCustomerCode");
            this.lblCustomerCode.Name = "lblCustomerCode";
            this.lblCustomerCode.AutoSize = true;
            this.lblCustomerCode.MinimumSize = new Size(148, 34);
            this.lblCustomerCode.Font = Desk.Px(13, FontStyle.Bold);
            this.lblCustomerCode.ForeColor = Desk.FieldInk;
            this.lblCustomerCode.TextAlign = ContentAlignment.MiddleLeft;

            this.txtCustomerCode.Name = "txtCustomerCode";
            this.txtCustomerCode.Size = new Size(190, 34);
            this.txtCustomerCode.Font = Desk.Mono(14);
            this.txtCustomerCode.ForeColor = Desk.Body;
            this.txtCustomerCode.CssStyle = "border:1.5px solid #c9d4e0;border-radius:6px";
            //
            // A customer code is an identifier: typed, read aloud, quoted in an email and compared
            // character by character. Latin letters and digits are NEUTRAL characters in the
            // bidirectional algorithm, so inside a right-to-left paragraph the run may be
            // reordered. Pin it, and it reads the same for every user in every language.
            //
            // Names and addresses are the opposite case - they are prose and must mirror.
            this.txtCustomerCode.RightToLeft = Wisej.Web.RightToLeft.No;

            this.pnlCodeRow.Name = "pnlCodeRow";
            this.pnlCodeRow.Dock = Wisej.Web.DockStyle.Top;
            this.pnlCodeRow.Size = new Size(1150, 44);
            this.pnlCodeRow.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.pnlCodeRow.WrapContents = true;
            this.pnlCodeRow.Controls.Add(this.lblCustomerCode);
            this.pnlCodeRow.Controls.Add(this.txtCustomerCode);
            //
            // the contacts grid
            //
            this.colContact.Name = "colContact";
            this.colContact.Width = 170;

            this.colCompany.Name = "colCompany";
            this.colCompany.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;

            this.colCity.Name = "colCity";
            this.colCity.Width = 120;

            this.gridContacts.Name = "gridContacts";
            this.gridContacts.Dock = Wisej.Web.DockStyle.Fill;
            this.gridContacts.AllowUserToAddRows = false;
            this.gridContacts.AllowUserToDeleteRows = false;
            this.gridContacts.AllowUserToResizeRows = false;
            this.gridContacts.ReadOnly = true;
            this.gridContacts.RowHeadersVisible = false;
            this.gridContacts.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridContacts.ColumnHeadersHeight = 28;
            this.gridContacts.RowTemplate.Height = 32;
            this.gridContacts.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.gridContacts.Font = Desk.Px(12.5F);
            this.gridContacts.ColumnHeadersDefaultCellStyle.BackColor = Desk.CardHead;
            this.gridContacts.ColumnHeadersDefaultCellStyle.ForeColor = Desk.Muted;
            this.gridContacts.ColumnHeadersDefaultCellStyle.Font = Desk.Px(11, FontStyle.Bold);
            // Header alignment is left because the neutral culture reads left to right. Under an
            // RTL culture the grid mirrors this on its own - see docs/RtlTests.md for what it
            // does and does not decide for you.
            this.gridContacts.ColumnHeadersDefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            this.gridContacts.Columns.Add(this.colContact);
            this.gridContacts.Columns.Add(this.colCompany);
            this.gridContacts.Columns.Add(this.colCity);

            // Tall enough for the header and the three rows, and no taller: the grid is a list of
            // contacts, not the whole screen.
            this.pnlGridHost.Name = "pnlGridHost";
            this.pnlGridHost.Dock = Wisej.Web.DockStyle.Top;
            this.pnlGridHost.Size = new Size(1150, 152);
            this.pnlGridHost.BackColor = Color.White;
            this.pnlGridHost.Padding = new Wisej.Web.Padding(0, 12, 0, 12);
            this.pnlGridHost.Controls.Add(this.gridContacts);
            //
            // the button row
            //
            // AutoSize with a MinimumSize, in a FlowLayoutPanel that wraps. A caption half as long
            // again pushes the next button along, or onto a second row in a narrow window - it
            // never clips and never needs a per-language Size.
            SetActionButton(this.btnSave, "btnSave", primary: true);
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Click += this.btnSave_Click;

            SetActionButton(this.btnCancel, "btnCancel", primary: false);
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Click += this.btnCancel_Click;

            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Dock = Wisej.Web.DockStyle.Top;
            this.pnlButtons.Size = new Size(1150, 48);
            this.pnlButtons.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.pnlButtons.WrapContents = true;
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnCancel);
            //
            // CustomerEditor
            //
            resources.ApplyResources(this, "$this");
            this.Name = "CustomerEditor";
            this.BackColor = Color.White;
            // Docked Top, so the control added last is the one nearest the top: the code row,
            // then the contacts, then the actions.
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlGridHost);
            this.Controls.Add(this.pnlCodeRow);
            this.ResumeLayout(false);
        }

        private static void SetActionButton(Wisej.Web.Button button, string name, bool primary)
        {
            button.Name = name;
            button.AutoSize = true;
            button.MinimumSize = new Size(96, 34);
            button.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            button.Padding = new Wisej.Web.Padding(18, 0, 18, 0);
            button.Font = Desk.Px(13, FontStyle.Bold);
            button.BackColor = primary ? Desk.Accent : Color.White;
            button.ForeColor = primary ? Color.White : Desk.FieldInk;
            button.CssStyle = primary
                ? "border:1px solid #1565d8;border-radius:7px"
                : "border:1px solid #cdd9e6;border-radius:7px";
        }

        #endregion

        private Wisej.Web.FlowLayoutPanel pnlCodeRow;
        private Wisej.Web.Label lblCustomerCode;
        private Wisej.Web.TextBox txtCustomerCode;
        private Wisej.Web.Panel pnlGridHost;
        private Wisej.Web.DataGridView gridContacts;
        private Wisej.Web.DataGridViewTextBoxColumn colContact;
        private Wisej.Web.DataGridViewTextBoxColumn colCompany;
        private Wisej.Web.DataGridViewTextBoxColumn colCity;
        private Wisej.Web.FlowLayoutPanel pnlButtons;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnCancel;
    }
}
