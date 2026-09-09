namespace WisejTrainingApp.Views
{
    partial class NextStepsView
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
            this.components = new System.ComponentModel.Container();
            this.pnlNextSteps = new Wisej.Web.Panel();
            this.labelNextStepsCard = new Wisej.Web.Label();
            this.lstNextSteps = new Wisej.Web.ListBox();
            this.lblStepTitle = new Wisej.Web.Label();
            this.lblStepToday = new Wisej.Web.Label();
            this.lblStepProduction = new Wisej.Web.Label();
            this.lblStepTouches = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlNextSteps.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlNextSteps
            //
            this.pnlNextSteps.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlNextSteps.BackColor = System.Drawing.Color.White;
            this.pnlNextSteps.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlNextSteps.Controls.Add(this.labelNextStepsCard);
            this.pnlNextSteps.Controls.Add(this.lstNextSteps);
            this.pnlNextSteps.Controls.Add(this.lblStepTitle);
            this.pnlNextSteps.Controls.Add(this.lblStepToday);
            this.pnlNextSteps.Controls.Add(this.lblStepProduction);
            this.pnlNextSteps.Controls.Add(this.lblStepTouches);
            this.pnlNextSteps.Controls.Add(this.lblStatus);
            this.pnlNextSteps.Location = new System.Drawing.Point(0, 0);
            this.pnlNextSteps.Name = "pnlNextSteps";
            this.pnlNextSteps.Size = new System.Drawing.Size(1032, 532);
            //
            // labelNextStepsCard
            //
            this.labelNextStepsCard.AutoSize = false;
            this.labelNextStepsCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelNextStepsCard.Location = new System.Drawing.Point(24, 14);
            this.labelNextStepsCard.Name = "labelNextStepsCard";
            this.labelNextStepsCard.Size = new System.Drawing.Size(984, 28);
            this.labelNextStepsCard.Text = "Next steps  ·  what a production-ready version still needs (lesson s47 §3)";
            //
            // lstNextSteps
            //
            this.lstNextSteps.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.lstNextSteps.Font = new System.Drawing.Font("default", 10F);
            this.lstNextSteps.Location = new System.Drawing.Point(24, 56);
            this.lstNextSteps.Name = "lstNextSteps";
            this.lstNextSteps.Size = new System.Drawing.Size(300, 410);
            this.lstNextSteps.SelectedIndexChanged += new System.EventHandler(this.lstNextSteps_SelectedIndexChanged);
            //
            // lblStepTitle
            //
            this.lblStepTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStepTitle.AutoSize = false;
            this.lblStepTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblStepTitle.Location = new System.Drawing.Point(348, 56);
            this.lblStepTitle.Name = "lblStepTitle";
            this.lblStepTitle.Size = new System.Drawing.Size(660, 32);
            this.lblStepTitle.Text = "Select a step";
            //
            // lblStepToday
            //
            this.lblStepToday.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStepToday.AutoSize = false;
            this.lblStepToday.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblStepToday.ForeColor = System.Drawing.Color.FromArgb(40, 52, 70);
            this.lblStepToday.Location = new System.Drawing.Point(348, 100);
            this.lblStepToday.Name = "lblStepToday";
            this.lblStepToday.Padding = new Wisej.Web.Padding(12, 8, 12, 8);
            this.lblStepToday.Size = new System.Drawing.Size(660, 96);
            this.lblStepToday.Text = "";
            this.lblStepToday.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // lblStepProduction
            //
            this.lblStepProduction.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStepProduction.AutoSize = false;
            this.lblStepProduction.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblStepProduction.ForeColor = System.Drawing.Color.FromArgb(40, 52, 70);
            this.lblStepProduction.Location = new System.Drawing.Point(348, 208);
            this.lblStepProduction.Name = "lblStepProduction";
            this.lblStepProduction.Padding = new Wisej.Web.Padding(12, 8, 12, 8);
            this.lblStepProduction.Size = new System.Drawing.Size(660, 130);
            this.lblStepProduction.Text = "";
            this.lblStepProduction.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // lblStepTouches
            //
            this.lblStepTouches.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStepTouches.AutoSize = false;
            this.lblStepTouches.Font = new System.Drawing.Font("monospace", 9F);
            this.lblStepTouches.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblStepTouches.Location = new System.Drawing.Point(348, 350);
            this.lblStepTouches.Name = "lblStepTouches";
            this.lblStepTouches.Size = new System.Drawing.Size(660, 90);
            this.lblStepTouches.Text = "";
            this.lblStepTouches.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(24, 476);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(984, 26);
            this.lblStatus.Text = "● select a step — full list in docs/NextSteps.md";
            //
            // NextStepsView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlNextSteps);
            this.Name = "NextStepsView";
            this.Size = new System.Drawing.Size(1032, 532);
            this.pnlNextSteps.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlNextSteps;
        private Wisej.Web.Label labelNextStepsCard;
        private Wisej.Web.ListBox lstNextSteps;
        private Wisej.Web.Label lblStepTitle;
        private Wisej.Web.Label lblStepToday;
        private Wisej.Web.Label lblStepProduction;
        private Wisej.Web.Label lblStepTouches;
        private Wisej.Web.Label lblStatus;
    }
}
