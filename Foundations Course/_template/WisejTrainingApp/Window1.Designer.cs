namespace WisejTrainingApp
{
    partial class Window1
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
            this.lblTitle = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(24, 24);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(260, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "WisejTrainingApp — new module";
            //
            // Window1
            //
            this.ClientSize = new System.Drawing.Size(900, 560);
            this.Controls.Add(this.lblTitle);
            this.Name = "Window1";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "WisejTrainingApp — new module";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblTitle;
    }
}
