namespace IntegrationLab
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
            this.panelGauge = new Wisej.Web.Panel();
            this.labelTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.gaugeKnob = new Wisej.Web.Widget();
            this.labelAlarm = new Wisej.Web.Label();
            this.panelGauge.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGauge
            //
            this.panelGauge.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelGauge.BackColor = System.Drawing.Color.White;
            this.panelGauge.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGauge.Controls.Add(this.labelTitle);
            this.panelGauge.Controls.Add(this.labelStatus);
            this.panelGauge.Controls.Add(this.gaugeKnob);
            this.panelGauge.Controls.Add(this.labelAlarm);
            this.panelGauge.Location = new System.Drawing.Point(20, 20);
            this.panelGauge.Name = "panelGauge";
            this.panelGauge.Size = new System.Drawing.Size(380, 386);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 18);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(180, 30);
            this.labelTitle.Text = "Pressure knob";
            //
            // labelStatus
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(206, 20);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(150, 26);
            this.labelStatus.Text = "● 40 psi";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gaugeKnob
            //
            this.gaugeKnob.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gaugeKnob.Location = new System.Drawing.Point(24, 58);
            this.gaugeKnob.Name = "gaugeKnob";
            this.gaugeKnob.Packages.AddRange(new Wisej.Web.Widget.Package[] {
                new Wisej.Web.Widget.Package { Name = "jquery-lite", Source = "wwwroot/jquery-lite.js" },
                new Wisej.Web.Widget.Package { Name = "vendor-knob-css", Source = "wwwroot/vendor-knob.css" },
                new Wisej.Web.Widget.Package { Name = "vendor-knob", Source = "wwwroot/vendor-knob.js" }});
            this.gaugeKnob.Size = new System.Drawing.Size(332, 260);
            this.gaugeKnob.WiredEvents = new string[] { "valueChanged", "error" };
            this.gaugeKnob.WidgetEvent += new Wisej.Web.WidgetEventHandler(this.gaugeKnob_WidgetEvent);
            //
            // labelAlarm
            //
            this.labelAlarm.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelAlarm.AutoSize = false;
            this.labelAlarm.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelAlarm.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelAlarm.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelAlarm.Location = new System.Drawing.Point(24, 326);
            this.labelAlarm.Name = "labelAlarm";
            this.labelAlarm.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelAlarm.Size = new System.Drawing.Size(332, 44);
            this.labelAlarm.Text = "";
            this.labelAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelAlarm.Visible = false;
            //
            // Window1
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(420, 426);
            this.Controls.Add(this.panelGauge);
            this.Name = "Window1";
            this.Text = "IntegrationLab — Knob Demo";
            this.panelGauge.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelGauge;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.Widget gaugeKnob;
        private Wisej.Web.Label labelAlarm;
    }
}
