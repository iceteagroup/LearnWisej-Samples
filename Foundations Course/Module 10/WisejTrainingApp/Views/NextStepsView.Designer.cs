namespace WisejTrainingApp.Views
{
    partial class NextStepsView
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
            this.lblDescription = new Wisej.Web.Label();
            this.lstNextSteps = new Wisej.Web.ListBox();
            this.SuspendLayout();
            //
            // lblPageTitle
            //
            this.lblPageTitle.AutoSize = true;
            this.lblPageTitle.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(0, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Text = "Next Steps";
            //
            // lblDescription
            //
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(0, 40);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Text = "What a production-ready version would still need.";
            //
            // lstNextSteps
            //
            this.lstNextSteps.Items.AddRange(new object[] {
            "Replace the fake repository with a real database",
            "Add sign-in, roles and real authorization",
            "Write unit tests for TicketService and TicketValidator",
            "Send errors to a central log",
            "Automate the build and the deployment"});
            this.lstNextSteps.Location = new System.Drawing.Point(0, 70);
            this.lstNextSteps.Name = "lstNextSteps";
            this.lstNextSteps.Size = new System.Drawing.Size(560, 140);
            //
            // NextStepsView
            //
            this.Controls.Add(this.lstNextSteps);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblPageTitle);
            this.Name = "NextStepsView";
            this.Size = new System.Drawing.Size(800, 500);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.Label lblDescription;
        private Wisej.Web.ListBox lstNextSteps;
    }
}
