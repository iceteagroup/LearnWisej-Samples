namespace WisejPerfLab.Pages
{
    partial class CapacityPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Designer generated code

        private void InitializeComponent()
        {
            this.lblLive = new Wisej.Web.Label();
            this.lblConfig = new Wisej.Web.Label();
            this.lblModel = new Wisej.Web.Label();
            this.lblAnswer = new Wisej.Web.Label();
            this.btnRefreshHealth = new Wisej.Web.Button();
            this.btnRefuseNew = new Wisej.Web.Button();
            this.btnRestoreLimit = new Wisej.Web.Button();
            this.lblScenario = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblLive
            //
            this.lblLive.Font = new System.Drawing.Font("monospace", 11F, System.Drawing.FontStyle.Bold);
            this.lblLive.Location = new System.Drawing.Point(14, 12);
            this.lblLive.Name = "lblLive";
            this.lblLive.Size = new System.Drawing.Size(846, 26);
            this.lblLive.TabIndex = 0;
            this.lblLive.Text = "—";
            //
            // lblConfig
            //
            this.lblConfig.Font = new System.Drawing.Font("monospace", 9F);
            this.lblConfig.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblConfig.Location = new System.Drawing.Point(14, 44);
            this.lblConfig.Name = "lblConfig";
            this.lblConfig.Size = new System.Drawing.Size(846, 22);
            this.lblConfig.TabIndex = 1;
            this.lblConfig.Text = "HealthCheck.json —";
            //
            // lblModel
            //
            this.lblModel.Font = new System.Drawing.Font("monospace", 9F);
            this.lblModel.Location = new System.Drawing.Point(14, 76);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(846, 210);
            this.lblModel.TabIndex = 2;
            //
            // lblAnswer
            //
            this.lblAnswer.Font = new System.Drawing.Font("monospace", 9F);
            this.lblAnswer.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblAnswer.Location = new System.Drawing.Point(14, 296);
            this.lblAnswer.Name = "lblAnswer";
            this.lblAnswer.Size = new System.Drawing.Size(846, 40);
            this.lblAnswer.TabIndex = 3;
            this.lblAnswer.Text = "healthcheck.wx — not asked yet";
            //
            // btnRefreshHealth
            //
            this.btnRefreshHealth.Location = new System.Drawing.Point(14, 348);
            this.btnRefreshHealth.Name = "btnRefreshHealth";
            this.btnRefreshHealth.Size = new System.Drawing.Size(200, 34);
            this.btnRefreshHealth.TabIndex = 4;
            this.btnRefreshHealth.Text = "Ask healthcheck.wx now";
            this.btnRefreshHealth.Click += this.btnRefreshHealth_Click;
            //
            // btnRefuseNew
            //
            this.btnRefuseNew.Location = new System.Drawing.Point(222, 348);
            this.btnRefuseNew.Name = "btnRefuseNew";
            this.btnRefuseNew.Size = new System.Drawing.Size(250, 34);
            this.btnRefuseNew.TabIndex = 5;
            this.btnRefuseNew.Text = "Drive it past the session limit";
            this.btnRefuseNew.Click += this.btnRefuseNew_Click;
            //
            // btnRestoreLimit
            //
            this.btnRestoreLimit.Location = new System.Drawing.Point(480, 348);
            this.btnRestoreLimit.Name = "btnRestoreLimit";
            this.btnRestoreLimit.Size = new System.Drawing.Size(220, 34);
            this.btnRestoreLimit.TabIndex = 6;
            this.btnRestoreLimit.Text = "Restore the configured limit";
            this.btnRestoreLimit.Click += this.btnRestoreLimit_Click;
            //
            // lblScenario
            //
            this.lblScenario.Font = new System.Drawing.Font("monospace", 8F);
            this.lblScenario.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblScenario.Location = new System.Drawing.Point(14, 392);
            this.lblScenario.Name = "lblScenario";
            this.lblScenario.Size = new System.Drawing.Size(846, 32);
            this.lblScenario.TabIndex = 7;
            this.lblScenario.Text = "load balancer health URL: http://localhost:5807/healthcheck.wx — the thresholds come from docs/Capacity.md\r\nafter \"Drive it past the session limit\", open a second browser tab: it is refused while this session keeps working";
            //
            // CapacityPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblScenario);
            this.Controls.Add(this.btnRestoreLimit);
            this.Controls.Add(this.btnRefuseNew);
            this.Controls.Add(this.btnRefreshHealth);
            this.Controls.Add(this.lblAnswer);
            this.Controls.Add(this.lblModel);
            this.Controls.Add(this.lblConfig);
            this.Controls.Add(this.lblLive);
            this.Name = "CapacityPage";
            this.Size = new System.Drawing.Size(876, 440);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblLive;
        private Wisej.Web.Label lblConfig;
        private Wisej.Web.Label lblModel;
        private Wisej.Web.Label lblAnswer;
        private Wisej.Web.Button btnRefreshHealth;
        private Wisej.Web.Button btnRefuseNew;
        private Wisej.Web.Button btnRestoreLimit;
        private Wisej.Web.Label lblScenario;
    }
}
