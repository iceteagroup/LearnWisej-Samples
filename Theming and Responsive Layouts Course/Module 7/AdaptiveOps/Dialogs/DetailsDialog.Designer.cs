namespace AdaptiveOps.Dialogs
{
    partial class DetailsDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.editor = new AdaptiveOps.Shell.DetailsEditor();
            this.footerPanel = new Wisej.Web.Panel();
            this.btnCancel = new Wisej.Web.Button();
            this.footerPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // editor  (the shared DetailsEditor, compact, Dock = Fill)
            //
            this.editor.Dock = Wisej.Web.DockStyle.Fill;
            this.editor.MinimumSize = new System.Drawing.Size(280, 360);
            this.editor.Name = "editor";
            this.editor.TabIndex = 1;
            //
            // footerPanel / btnCancel
            //
            this.footerPanel.Controls.Add(this.btnCancel);
            this.footerPanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Padding = new Wisej.Web.Padding(8);
            this.footerPanel.Size = new System.Drawing.Size(380, 52);
            this.footerPanel.TabIndex = 2;
            this.footerPanel.TabStop = false;
            this.btnCancel.AccessibleName = "Close without saving";
            this.btnCancel.Dock = Wisej.Web.DockStyle.Right;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 36);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Close";
            this.btnCancel.ToolTipText = "Close the editor without saving";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // DetailsDialog
            //
            this.AccessibleName = "Ticket details dialog";
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.editor);
            this.Controls.Add(this.footerPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 420);
            this.Name = "DetailsDialog";
            this.Size = new System.Drawing.Size(380, 600);
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "Ticket details";
            this.footerPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private AdaptiveOps.Shell.DetailsEditor editor;
        private Wisej.Web.Panel footerPanel;
        private Wisej.Web.Button btnCancel;
    }
}
