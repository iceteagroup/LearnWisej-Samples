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
            this.lblBody = new Wisej.Web.Label();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlCard  (white card on the grey content area)
            //
            this.pnlCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlCard.BackColor = System.Drawing.Color.White;
            this.pnlCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblModule);
            this.pnlCard.Controls.Add(this.lblBody);
            this.pnlCard.Location = new System.Drawing.Point(24, 24);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(692, 170);
            //
            // lblTitle  (the title label the lab asks for on every placeholder page)
            //
            this.lblTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(652, 34);
            this.lblTitle.Text = "DataGridView";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblModule
            //
            this.lblModule.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblModule.AutoSize = false;
            this.lblModule.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblModule.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblModule.Location = new System.Drawing.Point(20, 52);
            this.lblModule.Name = "lblModule";
            this.lblModule.Size = new System.Drawing.Size(652, 22);
            this.lblModule.Text = "PLACEHOLDER · BUILT IN MODULE 5 — DATAGRIDVIEW MASTERY";
            this.lblModule.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblBody
            //
            this.lblBody.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblBody.AutoSize = false;
            this.lblBody.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblBody.Location = new System.Drawing.Point(20, 84);
            this.lblBody.Name = "lblBody";
            this.lblBody.Size = new System.Drawing.Size(652, 70);
            this.lblBody.Text = "This page is a UserControl swapped into contentPanel by MainPage.Navigate(). Module 5 replaces its body with the Orders grid: BindingSource + explicit columns, HTML status badge, custom editor, virtual mode with a cache, filter and status strips.";
            this.lblBody.TextAlign = System.Drawing.ContentAlignment.TopLeft;
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
        private Wisej.Web.Label lblBody;
    }
}
