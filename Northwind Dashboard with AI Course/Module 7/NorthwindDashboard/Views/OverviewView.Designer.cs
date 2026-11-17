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
			Wisej.Web.ChartSeries chartSeries1 = new Wisej.Web.ChartSeries();
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
			this.categoryCard = new Wisej.Web.Panel();
			this.categoryChart = new Wisej.Web.ChartView();
			this.categoryNote = new Wisej.Web.Label();
			this.activityCard = new Wisej.Web.Panel();
			this.activityTimeline = new Wisej.Web.Timeline();
			this.heatmapCard = new Wisej.Web.Panel();
			this.ordersHeatmap = new Wisej.Web.HeatmapCalendar();
			this.heatmapNote = new Wisej.Web.Label();
			this.layout.SuspendLayout();
			this.stagesCard.SuspendLayout();
			this.notesPanel.SuspendLayout();
			this.categoryCard.SuspendLayout();
			this.activityCard.SuspendLayout();
			this.heatmapCard.SuspendLayout();
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
			this.layout.Controls.Add(this.categoryCard, 4, 1);
			this.layout.Controls.Add(this.activityCard, 8, 1);
			this.layout.Controls.Add(this.notesPanel, 0, 2);
			this.layout.Controls.Add(this.heatmapCard, 0, 3);
			this.layout.Dock = Wisej.Web.DockStyle.Top;
			this.layout.Location = new System.Drawing.Point(0, 0);
			this.layout.Name = "layout";
			this.layout.RowCount = 4;
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 192F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 116F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 162F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 262F));
			this.layout.SetColumnSpan(this.revenueKpi, 3);
			this.layout.SetColumnSpan(this.ordersKpi, 3);
			this.layout.SetColumnSpan(this.averageKpi, 3);
			this.layout.SetColumnSpan(this.onTimeKpi, 3);
			this.layout.SetColumnSpan(this.stagesCard, 4);
			this.layout.SetColumnSpan(this.categoryCard, 4);
			this.layout.SetRowSpan(this.categoryCard, 2);
			this.layout.SetColumnSpan(this.activityCard, 4);
			this.layout.SetRowSpan(this.activityCard, 2);
			this.layout.SetColumnSpan(this.notesPanel, 4);
			this.layout.SetColumnSpan(this.heatmapCard, 12);
			this.layout.Size = new System.Drawing.Size(1206, 732);
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
			this.stagesCard.Size = new System.Drawing.Size(390, 104);
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
			this.stagesMeter.Size = new System.Drawing.Size(358, 46);
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
			this.notesPanel.Size = new System.Drawing.Size(402, 162);
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
			// categoryCard
			//
			this.categoryCard.AppearanceKey = "nw-card";
			this.categoryCard.Controls.Add(this.categoryChart);
			this.categoryCard.Controls.Add(this.categoryNote);
			this.categoryCard.Dock = Wisej.Web.DockStyle.Fill;
			this.categoryCard.HeaderSize = 46;
			this.categoryCard.Location = new System.Drawing.Point(402, 192);
			this.categoryCard.Margin = new Wisej.Web.Padding(0, 0, 12, 12);
			this.categoryCard.Name = "categoryCard";
			this.categoryCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.categoryCard.ShowHeader = true;
			this.categoryCard.Size = new System.Drawing.Size(390, 266);
			this.categoryCard.TabIndex = 6;
			this.categoryCard.Text = "Revenue by category";
			//
			// categoryChart
			//
			this.categoryChart.AccessibleName = "Revenue by category";
			this.categoryChart.Dock = Wisej.Web.DockStyle.Fill;
			this.categoryChart.EmptyState = "No orders in this period";
			this.categoryChart.Legend = Wisej.Web.ChartLegend.Hidden;
			this.categoryChart.Location = new System.Drawing.Point(16, 22);
			this.categoryChart.Name = "categoryChart";
			chartSeries1.CategoryMember = "Name";
			chartSeries1.ChartType = Wisej.Web.ChartType.Bar;
			chartSeries1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(216)))));
			chartSeries1.LabelFormat = "{0:C0}";
			chartSeries1.Name = "Revenue";
			chartSeries1.ValueMember = "Revenue";
			this.categoryChart.Series.Add(chartSeries1);
			this.categoryChart.Size = new System.Drawing.Size(358, 186);
			this.categoryChart.TabIndex = 1;
			this.categoryChart.YAxis.Format = "N0";
			this.categoryChart.YAxis.Min = 0D;
			this.categoryChart.PointClick += new System.EventHandler<Wisej.Web.SeriesPointEventArgs>(this.categoryChart_PointClick);
			//
			// categoryNote
			//
			this.categoryNote.AppearanceKey = "nw-caption";
			this.categoryNote.Dock = Wisej.Web.DockStyle.Top;
			this.categoryNote.Location = new System.Drawing.Point(16, 0);
			this.categoryNote.Name = "categoryNote";
			this.categoryNote.Size = new System.Drawing.Size(358, 22);
			this.categoryNote.TabIndex = 0;
			this.categoryNote.Text = "Select a bar to see the category's products";
			//
			// activityCard
			//
			this.activityCard.AppearanceKey = "nw-card";
			this.activityCard.Controls.Add(this.activityTimeline);
			this.activityCard.Dock = Wisej.Web.DockStyle.Fill;
			this.activityCard.HeaderSize = 46;
			this.activityCard.Location = new System.Drawing.Point(804, 192);
			this.activityCard.Margin = new Wisej.Web.Padding(0, 0, 0, 12);
			this.activityCard.Name = "activityCard";
			this.activityCard.Padding = new Wisej.Web.Padding(8, 0, 8, 8);
			this.activityCard.ShowHeader = true;
			this.activityCard.Size = new System.Drawing.Size(402, 266);
			this.activityCard.TabIndex = 7;
			this.activityCard.Text = "Activity";
			//
			// activityTimeline
			//
			this.activityTimeline.AccessibleName = "Recent orders and activity";
			this.activityTimeline.Dock = Wisej.Web.DockStyle.Fill;
			this.activityTimeline.Grouping = Wisej.Web.TimelineGrouping.ByDay;
			this.activityTimeline.LoadMoreText = "Older orders";
			this.activityTimeline.Location = new System.Drawing.Point(8, 0);
			this.activityTimeline.MaxItems = 200;
			this.activityTimeline.Name = "activityTimeline";
			this.activityTimeline.Size = new System.Drawing.Size(386, 212);
			this.activityTimeline.TabIndex = 0;
			this.activityTimeline.TimePosition = Wisej.Web.TimelineTimePosition.Hidden;
			this.activityTimeline.ItemClick += new System.EventHandler<Wisej.Web.TimelineItemEventArgs>(this.activityTimeline_ItemClick);
			this.activityTimeline.LoadMore += new System.EventHandler(this.activityTimeline_LoadMore);
			//
			// heatmapCard
			//
			this.heatmapCard.AppearanceKey = "nw-card";
			this.heatmapCard.Controls.Add(this.ordersHeatmap);
			this.heatmapCard.Controls.Add(this.heatmapNote);
			this.heatmapCard.Dock = Wisej.Web.DockStyle.Fill;
			this.heatmapCard.HeaderSize = 46;
			this.heatmapCard.Location = new System.Drawing.Point(0, 470);
			this.heatmapCard.Margin = new Wisej.Web.Padding(0, 0, 0, 12);
			this.heatmapCard.Name = "heatmapCard";
			this.heatmapCard.Padding = new Wisej.Web.Padding(16, 0, 16, 8);
			this.heatmapCard.ShowHeader = true;
			this.heatmapCard.Size = new System.Drawing.Size(1206, 250);
			this.heatmapCard.TabIndex = 8;
			this.heatmapCard.Text = "Order activity";
			//
			// ordersHeatmap
			//
			this.ordersHeatmap.AccessibleName = "Orders per day";
			this.ordersHeatmap.CellSize = 13;
			this.ordersHeatmap.Dock = Wisej.Web.DockStyle.Fill;
			this.ordersHeatmap.Location = new System.Drawing.Point(16, 22);
			this.ordersHeatmap.Name = "ordersHeatmap";
			this.ordersHeatmap.Size = new System.Drawing.Size(1174, 174);
			this.ordersHeatmap.TabIndex = 1;
			this.ordersHeatmap.Tone = Wisej.Web.MeterTone.Primary;
			this.ordersHeatmap.ToolTipFormat = "{0:ddd, MMM d, yyyy}: {1:N0} orders";
			this.ordersHeatmap.CellClick += new System.EventHandler<Wisej.Web.HeatmapCellEventArgs>(this.ordersHeatmap_CellClick);
			//
			// heatmapNote
			//
			this.heatmapNote.AppearanceKey = "nw-caption";
			this.heatmapNote.Dock = Wisej.Web.DockStyle.Top;
			this.heatmapNote.Location = new System.Drawing.Point(16, 0);
			this.heatmapNote.Name = "heatmapNote";
			this.heatmapNote.Size = new System.Drawing.Size(1174, 22);
			this.heatmapNote.TabIndex = 0;
			this.heatmapNote.Text = "Orders per day over the last 12 months · select a day to list its orders";
			//
			// OverviewView
			//			this.AppearanceKey = "nw-view";
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
			this.categoryCard.ResumeLayout(false);
			this.activityCard.ResumeLayout(false);
			this.heatmapCard.ResumeLayout(false);
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
		private Wisej.Web.Panel categoryCard;
		private Wisej.Web.ChartView categoryChart;
		private Wisej.Web.Label categoryNote;
		private Wisej.Web.Panel activityCard;
		private Wisej.Web.Timeline activityTimeline;
		private Wisej.Web.Panel heatmapCard;
		private Wisej.Web.HeatmapCalendar ordersHeatmap;
		private Wisej.Web.Label heatmapNote;
	}
}
