namespace NorthwindDashboard.Views
{
	partial class OverviewView
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
			this.revenueKpi = new Wisej.Web.KpiPanel();
			this.ordersKpi = new Wisej.Web.KpiPanel();
			this.averageKpi = new Wisej.Web.KpiPanel();
			this.onTimeKpi = new Wisej.Web.KpiPanel();
			this.stagesCard = new Wisej.Web.Panel();
			this.stagesMeter = new Wisej.Web.MeterBar();
			this.notesPanel = new Wisej.Web.Panel();
			this.comparisonLabel = new Wisej.Web.Label();
			this.periodLabel = new Wisej.Web.Label();
			this.layout.SuspendLayout();
			this.stagesCard.SuspendLayout();
			this.notesPanel.SuspendLayout();
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
			this.layout.Controls.Add(this.revenueKpi, 0, 0);
			this.layout.Controls.Add(this.ordersKpi, 3, 0);
			this.layout.Controls.Add(this.averageKpi, 6, 0);
			this.layout.Controls.Add(this.onTimeKpi, 9, 0);
			this.layout.Controls.Add(this.stagesCard, 0, 1);
			this.layout.Controls.Add(this.notesPanel, 0, 2);
			this.layout.Dock = Wisej.Web.DockStyle.Top;
			this.layout.Location = new System.Drawing.Point(0, 0);
			this.layout.Name = "layout";
			this.layout.RowCount = 3;
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 192F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 116F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 52F));
			this.layout.SetColumnSpan(this.revenueKpi, 3);
			this.layout.SetColumnSpan(this.ordersKpi, 3);
			this.layout.SetColumnSpan(this.averageKpi, 3);
			this.layout.SetColumnSpan(this.onTimeKpi, 3);
			this.layout.SetColumnSpan(this.stagesCard, 6);
			this.layout.SetColumnSpan(this.notesPanel, 12);
			this.layout.Size = new System.Drawing.Size(1206, 360);
			this.layout.TabIndex = 0;
			//
			// revenueKpi
			//
			this.revenueKpi.Clickable = true;
			this.revenueKpi.Dock = Wisej.Web.DockStyle.Fill;
			this.revenueKpi.Location = new System.Drawing.Point(0, 0);
			this.revenueKpi.Margin = new Wisej.Web.Padding(0, 0, 12, 12);
			this.revenueKpi.Name = "revenueKpi";
			this.revenueKpi.Size = new System.Drawing.Size(289, 180);
			this.revenueKpi.TabIndex = 0;
			this.revenueKpi.Title = "Revenue";
			this.revenueKpi.TrendFormat = "0.0\'%\';0.0\'%\';0\'%\'";
			this.revenueKpi.Value = "$0.00";
			this.revenueKpi.Visual = Wisej.Web.KpiVisual.Sparkline;
			this.revenueKpi.Click += new System.EventHandler(this.revenueKpi_Click);
			//
			// ordersKpi
			//
			this.ordersKpi.Clickable = true;
			this.ordersKpi.Dock = Wisej.Web.DockStyle.Fill;
			this.ordersKpi.Location = new System.Drawing.Point(301, 0);
			this.ordersKpi.Margin = new Wisej.Web.Padding(0, 0, 12, 12);
			this.ordersKpi.Name = "ordersKpi";
			this.ordersKpi.Size = new System.Drawing.Size(289, 180);
			this.ordersKpi.TabIndex = 1;
			this.ordersKpi.Title = "Orders";
			this.ordersKpi.TrendFormat = "0.0\'%\';0.0\'%\';0\'%\'";
			this.ordersKpi.Value = "0";
			this.ordersKpi.Visual = Wisej.Web.KpiVisual.Sparkline;
			this.ordersKpi.Click += new System.EventHandler(this.ordersKpi_Click);
			//
			// averageKpi
			//
			this.averageKpi.Clickable = true;
			this.averageKpi.Dock = Wisej.Web.DockStyle.Fill;
			this.averageKpi.Location = new System.Drawing.Point(602, 0);
			this.averageKpi.Margin = new Wisej.Web.Padding(0, 0, 12, 12);
			this.averageKpi.Name = "averageKpi";
			this.averageKpi.Size = new System.Drawing.Size(289, 180);
			this.averageKpi.TabIndex = 2;
			this.averageKpi.Title = "Average order";
			this.averageKpi.TrendFormat = "0.0\'%\';0.0\'%\';0\'%\'";
			this.averageKpi.Value = "$0.00";
			this.averageKpi.Visual = Wisej.Web.KpiVisual.Sparkline;
			this.averageKpi.Click += new System.EventHandler(this.revenueKpi_Click);
			//
			// onTimeKpi
			//
			this.onTimeKpi.Clickable = true;
			this.onTimeKpi.Dock = Wisej.Web.DockStyle.Fill;
			this.onTimeKpi.Location = new System.Drawing.Point(903, 0);
			this.onTimeKpi.Margin = new Wisej.Web.Padding(0, 0, 0, 12);
			this.onTimeKpi.Name = "onTimeKpi";
			this.onTimeKpi.Size = new System.Drawing.Size(303, 180);
			this.onTimeKpi.TabIndex = 3;
			this.onTimeKpi.Title = "Shipped on time";
			this.onTimeKpi.Tone = Wisej.Web.KpiTone.Success;
			this.onTimeKpi.TrendFormat = "0.0\' pts\';0.0\' pts\';0\' pts\'";
			this.onTimeKpi.Value = "0%";
			this.onTimeKpi.Visual = Wisej.Web.KpiVisual.Gauge;
			this.onTimeKpi.Click += new System.EventHandler(this.onTimeKpi_Click);
			//
			// stagesCard
			//
			this.stagesCard.AppearanceKey = "nw-card";
			this.stagesCard.Controls.Add(this.stagesMeter);
			this.stagesCard.Dock = Wisej.Web.DockStyle.Fill;
			this.stagesCard.HeaderSize = 46;
			this.stagesCard.Location = new System.Drawing.Point(0, 192);
			this.stagesCard.Margin = new Wisej.Web.Padding(0, 0, 12, 12);
			this.stagesCard.Name = "stagesCard";
			this.stagesCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.stagesCard.ShowHeader = true;
			this.stagesCard.Size = new System.Drawing.Size(590, 104);
			this.stagesCard.TabIndex = 4;
			this.stagesCard.Text = "Order stages";
			//
			// stagesMeter
			//
			this.stagesMeter.BarHeight = 12;
			this.stagesMeter.Dock = Wisej.Web.DockStyle.Top;
			this.stagesMeter.Label = "Open orders";
			this.stagesMeter.Location = new System.Drawing.Point(16, 0);
			this.stagesMeter.Name = "stagesMeter";
			this.stagesMeter.Size = new System.Drawing.Size(558, 46);
			this.stagesMeter.TabIndex = 0;
			//
			// notesPanel
			//
			this.notesPanel.AppearanceKey = "nw-view";
			this.notesPanel.Controls.Add(this.comparisonLabel);
			this.notesPanel.Controls.Add(this.periodLabel);
			this.notesPanel.Dock = Wisej.Web.DockStyle.Fill;
			this.notesPanel.Location = new System.Drawing.Point(0, 308);
			this.notesPanel.Margin = new Wisej.Web.Padding(0);
			this.notesPanel.Name = "notesPanel";
			this.notesPanel.Size = new System.Drawing.Size(1206, 52);
			this.notesPanel.TabIndex = 5;
			//
			// comparisonLabel
			//
			this.comparisonLabel.AppearanceKey = "nw-subtitle";
			this.comparisonLabel.AutoSize = true;
			this.comparisonLabel.Location = new System.Drawing.Point(0, 2);
			this.comparisonLabel.Name = "comparisonLabel";
			this.comparisonLabel.Size = new System.Drawing.Size(280, 18);
			this.comparisonLabel.TabIndex = 0;
			this.comparisonLabel.Text = "Comparison: previous 30 days · Mar 8 – Apr 6, 1998";
			//
			// periodLabel
			//
			this.periodLabel.AppearanceKey = "nw-subtitle";
			this.periodLabel.AutoSize = true;
			this.periodLabel.Location = new System.Drawing.Point(0, 26);
			this.periodLabel.Name = "periodLabel";
			this.periodLabel.Size = new System.Drawing.Size(280, 18);
			this.periodLabel.TabIndex = 1;
			this.periodLabel.Text = "Period label: Last 30 days · 1998-04-07 to 1998-05-06";
			//
			// OverviewView
			//
			this.AppearanceKey = "nw-view";
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = Wisej.Web.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.layout);
			this.Name = "OverviewView";
			this.Size = new System.Drawing.Size(1206, 752);
			this.Text = "Overview";
			this.layout.ResumeLayout(false);
			this.stagesCard.ResumeLayout(false);
			this.notesPanel.ResumeLayout(false);
			this.notesPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.TableLayoutPanel layout;
		private Wisej.Web.KpiPanel revenueKpi;
		private Wisej.Web.KpiPanel ordersKpi;
		private Wisej.Web.KpiPanel averageKpi;
		private Wisej.Web.KpiPanel onTimeKpi;
		private Wisej.Web.Panel stagesCard;
		private Wisej.Web.MeterBar stagesMeter;
		private Wisej.Web.Panel notesPanel;
		private Wisej.Web.Label comparisonLabel;
		private Wisej.Web.Label periodLabel;
	}
}
