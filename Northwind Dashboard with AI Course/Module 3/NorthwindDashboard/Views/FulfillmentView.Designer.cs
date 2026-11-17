namespace NorthwindDashboard.Views
{
	partial class FulfillmentView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Wisej Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.layout = new Wisej.Web.TableLayoutPanel();
			this.shipper1Card = new Wisej.Web.Panel();
			this.shipper1Stats = new Wisej.Web.Label();
			this.shipper1Name = new Wisej.Web.Label();
			this.shipper1Gauge = new Wisej.Web.RadialGauge();
			this.shipper2Card = new Wisej.Web.Panel();
			this.shipper2Stats = new Wisej.Web.Label();
			this.shipper2Name = new Wisej.Web.Label();
			this.shipper2Gauge = new Wisej.Web.RadialGauge();
			this.shipper3Card = new Wisej.Web.Panel();
			this.shipper3Stats = new Wisej.Web.Label();
			this.shipper3Name = new Wisej.Web.Label();
			this.shipper3Gauge = new Wisej.Web.RadialGauge();
			this.notePanel = new Wisej.Web.Panel();
			this.periodLabel = new Wisej.Web.Label();
			this.thresholdsLabel = new Wisej.Web.Label();
			this.layout.SuspendLayout();
			this.shipper1Card.SuspendLayout();
			this.shipper2Card.SuspendLayout();
			this.shipper3Card.SuspendLayout();
			this.notePanel.SuspendLayout();
			this.SuspendLayout();
			//
			// layout
			//
			this.layout.ColumnCount = 12;
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.Controls.Add(this.shipper1Card, 0, 0);
			this.layout.Controls.Add(this.shipper2Card, 3, 0);
			this.layout.Controls.Add(this.shipper3Card, 6, 0);
			this.layout.Controls.Add(this.notePanel, 9, 0);
			this.layout.Dock = Wisej.Web.DockStyle.Fill;
			this.layout.Location = new System.Drawing.Point(0, 0);
			this.layout.Name = "layout";
			this.layout.Padding = new Wisej.Web.Padding(0, 0, 0, 8);
			this.layout.RowCount = 2;
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 150F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
			this.layout.SetColumnSpan(this.shipper1Card, 3);
			this.layout.SetColumnSpan(this.shipper2Card, 3);
			this.layout.SetColumnSpan(this.shipper3Card, 3);
			this.layout.SetColumnSpan(this.notePanel, 3);
			this.layout.Size = new System.Drawing.Size(1200, 758);
			this.layout.TabIndex = 0;
			//
			// shipper1Card
			//
			this.shipper1Card.AppearanceKey = "nw-card";
			this.shipper1Card.Controls.Add(this.shipper1Stats);
			this.shipper1Card.Controls.Add(this.shipper1Name);
			this.shipper1Card.Controls.Add(this.shipper1Gauge);
			this.shipper1Card.Dock = Wisej.Web.DockStyle.Fill;
			this.shipper1Card.Location = new System.Drawing.Point(0, 8);
			this.shipper1Card.Margin = new Wisej.Web.Padding(0, 8, 16, 8);
			this.shipper1Card.Name = "shipper1Card";
			this.shipper1Card.Size = new System.Drawing.Size(284, 134);
			this.shipper1Card.TabIndex = 0;
			//
			// shipper1Stats
			//
			this.shipper1Stats.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper1Stats.AppearanceKey = "nw-subtitle";
			this.shipper1Stats.Location = new System.Drawing.Point(132, 44);
			this.shipper1Stats.Name = "shipper1Stats";
			this.shipper1Stats.Size = new System.Drawing.Size(142, 72);
			this.shipper1Stats.TabIndex = 2;
			this.shipper1Stats.Text = "Shipped";
			//
			// shipper1Name
			//
			this.shipper1Name.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper1Name.AppearanceKey = "nw-strong";
			this.shipper1Name.AutoEllipsis = true;
			this.shipper1Name.Location = new System.Drawing.Point(132, 18);
			this.shipper1Name.Name = "shipper1Name";
			this.shipper1Name.Size = new System.Drawing.Size(142, 22);
			this.shipper1Name.TabIndex = 1;
			this.shipper1Name.Text = "Shipper";
			//
			// shipper1Gauge
			//
			this.shipper1Gauge.Label = "ON TIME";
			this.shipper1Gauge.Location = new System.Drawing.Point(16, 16);
			this.shipper1Gauge.Name = "shipper1Gauge";
			this.shipper1Gauge.Size = new System.Drawing.Size(102, 102);
			this.shipper1Gauge.TabIndex = 0;
			this.shipper1Gauge.Thickness = 10;
			this.shipper1Gauge.ValueFormat = "{0:N1}%";
			//
			// shipper2Card
			//
			this.shipper2Card.AppearanceKey = "nw-card";
			this.shipper2Card.Controls.Add(this.shipper2Stats);
			this.shipper2Card.Controls.Add(this.shipper2Name);
			this.shipper2Card.Controls.Add(this.shipper2Gauge);
			this.shipper2Card.Dock = Wisej.Web.DockStyle.Fill;
			this.shipper2Card.Location = new System.Drawing.Point(300, 8);
			this.shipper2Card.Margin = new Wisej.Web.Padding(0, 8, 16, 8);
			this.shipper2Card.Name = "shipper2Card";
			this.shipper2Card.Size = new System.Drawing.Size(284, 134);
			this.shipper2Card.TabIndex = 1;
			//
			// shipper2Stats
			//
			this.shipper2Stats.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper2Stats.AppearanceKey = "nw-subtitle";
			this.shipper2Stats.Location = new System.Drawing.Point(132, 44);
			this.shipper2Stats.Name = "shipper2Stats";
			this.shipper2Stats.Size = new System.Drawing.Size(142, 72);
			this.shipper2Stats.TabIndex = 2;
			this.shipper2Stats.Text = "Shipped";
			//
			// shipper2Name
			//
			this.shipper2Name.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper2Name.AppearanceKey = "nw-strong";
			this.shipper2Name.AutoEllipsis = true;
			this.shipper2Name.Location = new System.Drawing.Point(132, 18);
			this.shipper2Name.Name = "shipper2Name";
			this.shipper2Name.Size = new System.Drawing.Size(142, 22);
			this.shipper2Name.TabIndex = 1;
			this.shipper2Name.Text = "Shipper";
			//
			// shipper2Gauge
			//
			this.shipper2Gauge.Label = "ON TIME";
			this.shipper2Gauge.Location = new System.Drawing.Point(16, 16);
			this.shipper2Gauge.Name = "shipper2Gauge";
			this.shipper2Gauge.Size = new System.Drawing.Size(102, 102);
			this.shipper2Gauge.TabIndex = 0;
			this.shipper2Gauge.Thickness = 10;
			this.shipper2Gauge.ValueFormat = "{0:N1}%";
			//
			// shipper3Card
			//
			this.shipper3Card.AppearanceKey = "nw-card";
			this.shipper3Card.Controls.Add(this.shipper3Stats);
			this.shipper3Card.Controls.Add(this.shipper3Name);
			this.shipper3Card.Controls.Add(this.shipper3Gauge);
			this.shipper3Card.Dock = Wisej.Web.DockStyle.Fill;
			this.shipper3Card.Location = new System.Drawing.Point(600, 8);
			this.shipper3Card.Margin = new Wisej.Web.Padding(0, 8, 16, 8);
			this.shipper3Card.Name = "shipper3Card";
			this.shipper3Card.Size = new System.Drawing.Size(284, 134);
			this.shipper3Card.TabIndex = 2;
			//
			// shipper3Stats
			//
			this.shipper3Stats.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper3Stats.AppearanceKey = "nw-subtitle";
			this.shipper3Stats.Location = new System.Drawing.Point(132, 44);
			this.shipper3Stats.Name = "shipper3Stats";
			this.shipper3Stats.Size = new System.Drawing.Size(142, 72);
			this.shipper3Stats.TabIndex = 2;
			this.shipper3Stats.Text = "Shipped";
			//
			// shipper3Name
			//
			this.shipper3Name.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper3Name.AppearanceKey = "nw-strong";
			this.shipper3Name.AutoEllipsis = true;
			this.shipper3Name.Location = new System.Drawing.Point(132, 18);
			this.shipper3Name.Name = "shipper3Name";
			this.shipper3Name.Size = new System.Drawing.Size(142, 22);
			this.shipper3Name.TabIndex = 1;
			this.shipper3Name.Text = "Shipper";
			//
			// shipper3Gauge
			//
			this.shipper3Gauge.Label = "ON TIME";
			this.shipper3Gauge.Location = new System.Drawing.Point(16, 16);
			this.shipper3Gauge.Name = "shipper3Gauge";
			this.shipper3Gauge.Size = new System.Drawing.Size(102, 102);
			this.shipper3Gauge.TabIndex = 0;
			this.shipper3Gauge.Thickness = 10;
			this.shipper3Gauge.ValueFormat = "{0:N1}%";
			//
			// notePanel
			//
			this.notePanel.AppearanceKey = "nw-view";
			this.notePanel.Controls.Add(this.periodLabel);
			this.notePanel.Controls.Add(this.thresholdsLabel);
			this.notePanel.Dock = Wisej.Web.DockStyle.Fill;
			this.notePanel.Location = new System.Drawing.Point(900, 8);
			this.notePanel.Margin = new Wisej.Web.Padding(0, 8, 0, 8);
			this.notePanel.Name = "notePanel";
			this.notePanel.Size = new System.Drawing.Size(284, 134);
			this.notePanel.TabIndex = 3;
			//
			// periodLabel
			//
			this.periodLabel.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.periodLabel.AppearanceKey = "nw-caption";
			this.periodLabel.AutoEllipsis = true;
			this.periodLabel.Location = new System.Drawing.Point(8, 86);
			this.periodLabel.Name = "periodLabel";
			this.periodLabel.Size = new System.Drawing.Size(268, 18);
			this.periodLabel.TabIndex = 1;
			this.periodLabel.Text = "Orders placed";
			//
			// thresholdsLabel
			//
			this.thresholdsLabel.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.thresholdsLabel.AppearanceKey = "nw-subtitle";
			this.thresholdsLabel.Location = new System.Drawing.Point(8, 44);
			this.thresholdsLabel.Name = "thresholdsLabel";
			this.thresholdsLabel.Size = new System.Drawing.Size(268, 40);
			this.thresholdsLabel.TabIndex = 0;
			this.thresholdsLabel.Text = "On-time thresholds";
			//
			// FulfillmentView
			//
			this.AppearanceKey = "nw-view";
			this.Controls.Add(this.layout);
			this.Name = "FulfillmentView";
			this.Size = new System.Drawing.Size(1200, 758);
			this.Text = "Fulfillment";
			this.layout.ResumeLayout(false);
			this.shipper1Card.ResumeLayout(false);
			this.shipper2Card.ResumeLayout(false);
			this.shipper3Card.ResumeLayout(false);
			this.notePanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.TableLayoutPanel layout;
		private Wisej.Web.Panel shipper1Card;
		private Wisej.Web.RadialGauge shipper1Gauge;
		private Wisej.Web.Label shipper1Name;
		private Wisej.Web.Label shipper1Stats;
		private Wisej.Web.Panel shipper2Card;
		private Wisej.Web.RadialGauge shipper2Gauge;
		private Wisej.Web.Label shipper2Name;
		private Wisej.Web.Label shipper2Stats;
		private Wisej.Web.Panel shipper3Card;
		private Wisej.Web.RadialGauge shipper3Gauge;
		private Wisej.Web.Label shipper3Name;
		private Wisej.Web.Label shipper3Stats;
		private Wisej.Web.Panel notePanel;
		private Wisej.Web.Label thresholdsLabel;
		private Wisej.Web.Label periodLabel;
	}
}
