namespace OperationsConsole.Sections
{
    partial class DataGridViewPage
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
            this.pnlCard = new Wisej.Web.Panel();
            this.lblTitle = new Wisej.Web.Label();
            this.lblModule = new Wisej.Web.Label();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlCard
            //
            this.pnlCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlCard.BackColor = System.Drawing.Color.White;
            this.pnlCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblModule);
            this.pnlCard.Location = new System.Drawing.Point(24, 24);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(692, 90);
            //
            // lblTitle
            //
            this.lblTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(652, 34);
            this.lblTitle.Text = "DataGridView";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblModule
            //
            this.lblModule.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblModule.AutoSize = false;
            this.lblModule.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblModule.Location = new System.Drawing.Point(20, 50);
            this.lblModule.Name = "lblModule";
            this.lblModule.Size = new System.Drawing.Size(652, 24);
            this.lblModule.Text = "Module 5 replaces this page";
            this.lblModule.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // DataGridViewPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlCard);
            this.Name = "DataGridViewPage";
            this.Size = new System.Drawing.Size(740, 704);
            this.pnlCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlCard;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblModule;
    }
}
