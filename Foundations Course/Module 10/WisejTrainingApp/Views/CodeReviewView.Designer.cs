namespace WisejTrainingApp.Views
{
    partial class CodeReviewView
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
            this.lblPageTitle = new Wisej.Web.Label();
            this.chkReview = new Wisej.Web.CheckedListBox();
            this.SuspendLayout();
            //
            // lblPageTitle
            //
            this.lblPageTitle.AutoSize = true;
            this.lblPageTitle.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(0, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Text = "Code Review";
            //
            // chkReview
            //
            this.chkReview.CheckOnClick = true;
            this.chkReview.Items.AddRange(new object[] {
            "Naming: do classes, controls and methods clearly describe their job?",
            "Separation of concerns: is ticket logic in a service/validator instead of scattered across UI events?",
            "Navigation: can the reviewer follow how each page opens?",
            "Usability: are buttons placed where users expect them, with clear labels and feedback?",
            "Deployment: are configuration, logging, secrets, themes and release notes checked before review?"});
            this.chkReview.Location = new System.Drawing.Point(0, 50);
            this.chkReview.Name = "chkReview";
            this.chkReview.Size = new System.Drawing.Size(760, 160);
            //
            // CodeReviewView
            //
            this.Controls.Add(this.chkReview);
            this.Controls.Add(this.lblPageTitle);
            this.Name = "CodeReviewView";
            this.Size = new System.Drawing.Size(800, 500);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.CheckedListBox chkReview;
    }
}
