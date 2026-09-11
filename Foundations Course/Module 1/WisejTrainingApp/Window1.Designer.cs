namespace WisejTrainingApp
{
    partial class Window1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.txtName = new Wisej.Web.TextBox();
            this.btnSayHello = new Wisej.Web.Button();
            this.lblStatus = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(22, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(172, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "What's your name?";
            //
            // txtName
            //
            this.txtName.Location = new System.Drawing.Point(22, 68);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(196, 24);
            this.txtName.TabIndex = 1;
            //
            // btnSayHello
            //
            this.btnSayHello.Location = new System.Drawing.Point(22, 104);
            this.btnSayHello.Name = "btnSayHello";
            this.btnSayHello.Size = new System.Drawing.Size(87, 24);
            this.btnSayHello.TabIndex = 2;
            this.btnSayHello.Text = "Say Hello";
            this.btnSayHello.Click += new System.EventHandler(this.btnSayHello_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(22, 146);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 20);
            this.lblStatus.TabIndex = 3;
            //
            // Window1
            //
            this.ClientSize = new System.Drawing.Size(336, 236);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnSayHello);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblTitle);
            this.Name = "Window1";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "Window1";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblTitle;
        private Wisej.Web.TextBox txtName;
        private Wisej.Web.Button btnSayHello;
        private Wisej.Web.Label lblStatus;
    }
}
