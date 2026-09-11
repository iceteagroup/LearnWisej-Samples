namespace IntegrationLab
{
    partial class DemoPage
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
            this.simpleGauge1 = new IntegrationLab.Controls.SimpleGauge();
            this.labelAlarm = new Wisej.Web.Label();
            this.panelCodeBehind = new Wisej.Web.Panel();
            this.labelCodeBehindTitle = new Wisej.Web.Label();
            this.listCodeBehind = new Wisej.Web.ListBox();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonValue72 = new Wisej.Web.Button();
            this.buttonValue90 = new Wisej.Web.Button();
            this.buttonMax120 = new Wisej.Web.Button();
            this.buttonValue78 = new Wisej.Web.Button();
            this.panelGauge.SuspendLayout();
            this.panelCodeBehind.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGauge
            //
            this.panelGauge.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelGauge.BackColor = System.Drawing.Color.White;
            this.panelGauge.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGauge.Controls.Add(this.labelTitle);
            this.panelGauge.Controls.Add(this.simpleGauge1);
            this.panelGauge.Controls.Add(this.labelAlarm);
            this.panelGauge.Location = new System.Drawing.Point(30, 30);
            this.panelGauge.Name = "panelGauge";
            this.panelGauge.Size = new System.Drawing.Size(560, 400);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 18);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(320, 30);
            this.labelTitle.Text = "Boiler 3";
            //
            // simpleGauge1
            //
            this.simpleGauge1.Caption = "Boiler 3";
            this.simpleGauge1.Location = new System.Drawing.Point(40, 56);
            this.simpleGauge1.Name = "simpleGauge1";
            this.simpleGauge1.Size = new System.Drawing.Size(480, 270);
            this.simpleGauge1.Value = 72D;
            this.simpleGauge1.ValueChanged += new System.EventHandler<IntegrationLab.Controls.GaugeValueChangedEventArgs>(this.simpleGauge1_ValueChanged);
            this.simpleGauge1.ThresholdExceeded += new System.EventHandler<IntegrationLab.Controls.GaugeEventArgs>(this.simpleGauge1_ThresholdExceeded);
            this.simpleGauge1.WidgetError += new System.EventHandler<IntegrationLab.Controls.GaugeErrorEventArgs>(this.simpleGauge1_WidgetError);
            //
            // labelAlarm
            //
            this.labelAlarm.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelAlarm.AutoSize = false;
            this.labelAlarm.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelAlarm.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelAlarm.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelAlarm.Location = new System.Drawing.Point(24, 338);
            this.labelAlarm.Name = "labelAlarm";
            this.labelAlarm.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelAlarm.Size = new System.Drawing.Size(512, 44);
            this.labelAlarm.Text = "";
            this.labelAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelAlarm.Visible = false;
            //
            // panelCodeBehind
            //
            this.panelCodeBehind.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelCodeBehind.BackColor = System.Drawing.Color.White;
            this.panelCodeBehind.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelCodeBehind.Controls.Add(this.labelCodeBehindTitle);
            this.panelCodeBehind.Controls.Add(this.listCodeBehind);
            this.panelCodeBehind.Location = new System.Drawing.Point(618, 30);
            this.panelCodeBehind.Name = "panelCodeBehind";
            this.panelCodeBehind.Size = new System.Drawing.Size(700, 400);
            //
            // labelCodeBehindTitle
            //
            this.labelCodeBehindTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelCodeBehindTitle.AutoSize = false;
            this.labelCodeBehindTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelCodeBehindTitle.Location = new System.Drawing.Point(20, 14);
            this.labelCodeBehindTitle.Name = "labelCodeBehindTitle";
            this.labelCodeBehindTitle.Size = new System.Drawing.Size(660, 30);
            this.labelCodeBehindTitle.Text = "DemoPage.cs  ·  code-behind";
            //
            // listCodeBehind
            //
            this.listCodeBehind.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listCodeBehind.Font = new System.Drawing.Font("monospace", 10F);
            this.listCodeBehind.Location = new System.Drawing.Point(20, 52);
            this.listCodeBehind.Name = "listCodeBehind";
            this.listCodeBehind.Size = new System.Drawing.Size(660, 332);
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonValue72);
            this.panelActions.Controls.Add(this.buttonValue90);
            this.panelActions.Controls.Add(this.buttonMax120);
            this.panelActions.Controls.Add(this.buttonValue78);
            this.panelActions.Location = new System.Drawing.Point(30, 446);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // buttonValue72
            //
            this.buttonValue72.Location = new System.Drawing.Point(0, 4);
            this.buttonValue72.Name = "buttonValue72";
            this.buttonValue72.Size = new System.Drawing.Size(104, 36);
            this.buttonValue72.Text = "Value = 72";
            this.buttonValue72.Click += new System.EventHandler(this.buttonValue72_Click);
            //
            // buttonValue90
            //
            this.buttonValue90.Location = new System.Drawing.Point(112, 4);
            this.buttonValue90.Name = "buttonValue90";
            this.buttonValue90.Size = new System.Drawing.Size(104, 36);
            this.buttonValue90.Text = "Value = 90";
            this.buttonValue90.Click += new System.EventHandler(this.buttonValue90_Click);
            //
            // buttonMax120
            //
            this.buttonMax120.Location = new System.Drawing.Point(224, 4);
            this.buttonMax120.Name = "buttonMax120";
            this.buttonMax120.Size = new System.Drawing.Size(206, 36);
            this.buttonMax120.Text = "Maximum = 120; Value = 45";
            this.buttonMax120.Click += new System.EventHandler(this.buttonMax120_Click);
            //
            // buttonValue78
            //
            this.buttonValue78.Location = new System.Drawing.Point(438, 4);
            this.buttonValue78.Name = "buttonValue78";
            this.buttonValue78.Size = new System.Drawing.Size(104, 36);
            this.buttonValue78.Text = "Value = 78";
            this.buttonValue78.Click += new System.EventHandler(this.buttonValue78_Click);
            //
            // DemoPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelGauge);
            this.Controls.Add(this.panelCodeBehind);
            this.Controls.Add(this.panelActions);
            this.Name = "DemoPage";
            this.Size = new System.Drawing.Size(1348, 510);
            this.Text = "IntegrationLab — Demo";
            this.panelGauge.ResumeLayout(false);
            this.panelCodeBehind.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelGauge;
        private Wisej.Web.Label labelTitle;
        private IntegrationLab.Controls.SimpleGauge simpleGauge1;
        private Wisej.Web.Label labelAlarm;
        private Wisej.Web.Panel panelCodeBehind;
        private Wisej.Web.Label labelCodeBehindTitle;
        private Wisej.Web.ListBox listCodeBehind;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonValue72;
        private Wisej.Web.Button buttonValue90;
        private Wisej.Web.Button buttonMax120;
        private Wisej.Web.Button buttonValue78;
    }
}
