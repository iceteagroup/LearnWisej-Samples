namespace OperationsConsole.Sections
{
    partial class EditorsPage
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

        private void InitializeComponent()
        {
            this.customerEditor = new OperationsConsole.Editors.CustomerEditor();
            this.pnlOptions = new Wisej.Web.Panel();
            this.chkSimulateFailure = new Wisej.Web.CheckBox();
            this.pnlOptions.SuspendLayout();
            this.SuspendLayout();
            //
            // customerEditor
            //
            this.customerEditor.Dock = Wisej.Web.DockStyle.Fill;
            this.customerEditor.Name = "customerEditor";
            this.customerEditor.Size = new System.Drawing.Size(740, 660);
            this.customerEditor.TabIndex = 1;
            //
            // pnlOptions
            //
            this.pnlOptions.BackColor = System.Drawing.Color.White;
            this.pnlOptions.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlOptions.Controls.Add(this.chkSimulateFailure);
            this.pnlOptions.Dock = Wisej.Web.DockStyle.Top;
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(740, 44);
            //
            // chkSimulateFailure
            //
            this.chkSimulateFailure.AccessibleName = "Make the customer service fail on the next save";
            this.chkSimulateFailure.Location = new System.Drawing.Point(24, 10);
            this.chkSimulateFailure.Name = "chkSimulateFailure";
            this.chkSimulateFailure.Size = new System.Drawing.Size(220, 24);
            this.chkSimulateFailure.TabIndex = 10;
            this.chkSimulateFailure.Text = "Simulate service failure";
            this.chkSimulateFailure.CheckedChanged += new System.EventHandler(this.chkSimulateFailure_CheckedChanged);
            //
            // EditorsPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.customerEditor);
            this.Controls.Add(this.pnlOptions);
            this.Name = "EditorsPage";
            this.Size = new System.Drawing.Size(740, 704);
            this.pnlOptions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private OperationsConsole.Editors.CustomerEditor customerEditor;
        private Wisej.Web.Panel pnlOptions;
        private Wisej.Web.CheckBox chkSimulateFailure;
    }
}
