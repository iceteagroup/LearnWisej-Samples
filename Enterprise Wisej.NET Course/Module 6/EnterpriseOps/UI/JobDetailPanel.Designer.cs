namespace EnterpriseOps.UI
{
    partial class JobDetailPanel
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
            this.labelDetailTitle = new Wisej.Web.Label();
            this.labelResult = new Wisej.Web.Label();
            this.labelHistoryTitle = new Wisej.Web.Label();
            this.listHistory = new Wisej.Web.ListBox();
            this.labelErrorsTitle = new Wisej.Web.Label();
            this.listRowErrors = new Wisej.Web.ListBox();
            this.labelPolicy = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // labelDetailTitle
            //
            this.labelDetailTitle.AutoSize = false;
            this.labelDetailTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelDetailTitle.Location = new System.Drawing.Point(16, 10);
            this.labelDetailTitle.Name = "labelDetailTitle";
            this.labelDetailTitle.Size = new System.Drawing.Size(520, 24);
            this.labelDetailTitle.Text = "Job detail";
            //
            // labelResult
            //
            this.labelResult.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelResult.AutoSize = false;
            this.labelResult.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelResult.ForeColor = System.Drawing.Color.FromArgb(70, 88, 106);
            this.labelResult.Location = new System.Drawing.Point(16, 36);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(852, 22);
            this.labelResult.Text = "Select a job in the queue to see its history and its per-row result.";
            //
            // labelHistoryTitle
            //
            this.labelHistoryTitle.AutoSize = false;
            this.labelHistoryTitle.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelHistoryTitle.ForeColor = System.Drawing.Color.FromArgb(110, 126, 142);
            this.labelHistoryTitle.Location = new System.Drawing.Point(16, 64);
            this.labelHistoryTitle.Name = "labelHistoryTitle";
            this.labelHistoryTitle.Size = new System.Drawing.Size(510, 18);
            this.labelHistoryTitle.Text = "STATUS HISTORY — every transition the store recorded";
            //
            // listHistory
            //
            this.listHistory.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.listHistory.Font = new System.Drawing.Font("monospace", 9F);
            this.listHistory.Location = new System.Drawing.Point(16, 86);
            this.listHistory.Name = "listHistory";
            this.listHistory.Size = new System.Drawing.Size(510, 172);
            //
            // labelErrorsTitle
            //
            this.labelErrorsTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelErrorsTitle.AutoSize = false;
            this.labelErrorsTitle.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelErrorsTitle.ForeColor = System.Drawing.Color.FromArgb(110, 126, 142);
            this.labelErrorsTitle.Location = new System.Drawing.Point(538, 64);
            this.labelErrorsTitle.Name = "labelErrorsTitle";
            this.labelErrorsTitle.Size = new System.Drawing.Size(330, 18);
            this.labelErrorsTitle.Text = "ROWS THAT FAILED — line, ref, transient or terminal";
            //
            // listRowErrors
            //
            this.listRowErrors.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.listRowErrors.Font = new System.Drawing.Font("monospace", 9F);
            this.listRowErrors.Location = new System.Drawing.Point(538, 86);
            this.listRowErrors.Name = "listRowErrors";
            this.listRowErrors.Size = new System.Drawing.Size(330, 148);
            //
            // labelPolicy
            //
            this.labelPolicy.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.labelPolicy.AutoSize = false;
            this.labelPolicy.Font = new System.Drawing.Font("monospace", 8F);
            this.labelPolicy.ForeColor = System.Drawing.Color.FromArgb(110, 126, 142);
            this.labelPolicy.Location = new System.Drawing.Point(538, 238);
            this.labelPolicy.Name = "labelPolicy";
            this.labelPolicy.Size = new System.Drawing.Size(330, 20);
            this.labelPolicy.Text = "retry policy: —";
            //
            // JobDetailPanel
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.labelDetailTitle);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.labelHistoryTitle);
            this.Controls.Add(this.listHistory);
            this.Controls.Add(this.labelErrorsTitle);
            this.Controls.Add(this.listRowErrors);
            this.Controls.Add(this.labelPolicy);
            this.Name = "JobDetailPanel";
            this.Size = new System.Drawing.Size(884, 274);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label labelDetailTitle;
        private Wisej.Web.Label labelResult;
        private Wisej.Web.Label labelHistoryTitle;
        private Wisej.Web.ListBox listHistory;
        private Wisej.Web.Label labelErrorsTitle;
        private Wisej.Web.ListBox listRowErrors;
        private Wisej.Web.Label labelPolicy;
    }
}
