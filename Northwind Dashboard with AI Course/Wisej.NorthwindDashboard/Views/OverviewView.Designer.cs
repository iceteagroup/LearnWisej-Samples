///////////////////////////////////////////////////////////////////////////////
//
// (C) 2026 ICE TEA GROUP LLC - ALL RIGHTS RESERVED
//
//
//
// ALL INFORMATION CONTAINED HEREIN IS, AND REMAINS
// THE PROPERTY OF ICE TEA GROUP LLC AND ITS SUPPLIERS, IF ANY.
// THE INTELLECTUAL PROPERTY AND TECHNICAL CONCEPTS CONTAINED
// HEREIN ARE PROPRIETARY TO ICE TEA GROUP LLC AND ITS SUPPLIERS
// AND MAY BE COVERED BY U.S. AND FOREIGN PATENTS, PATENT IN PROCESS, AND
// ARE PROTECTED BY TRADE SECRET OR COPYRIGHT LAW.
//
// DISSEMINATION OF THIS INFORMATION OR REPRODUCTION OF THIS MATERIAL
// IS STRICTLY FORBIDDEN UNLESS PRIOR WRITTEN PERMISSION IS OBTAINED
// FROM ICE TEA GROUP LLC.
//

///////////////////////////////////////////////////////////////////////////////

namespace Wisej.NorthwindDashboard.Views
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OverviewView));
			Wisej.Web.ComponentTool componentTool1 = new Wisej.Web.ComponentTool();
			Wisej.Web.ChartSeries chartSeries1 = new Wisej.Web.ChartSeries();
			Wisej.Web.ChartSeries chartSeries2 = new Wisej.Web.ChartSeries();
			Wisej.Web.ChartSeries chartSeries3 = new Wisej.Web.ChartSeries();
			Wisej.Web.ComponentTool componentTool2 = new Wisej.Web.ComponentTool();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle1 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle2 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle3 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle4 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle5 = new Wisej.Web.DataGridViewCellStyle();
			this.layout = new Wisej.Web.TableLayoutPanel();
			this.revenueKpi = new Wisej.Web.KpiPanel();
			this.ordersKpi = new Wisej.Web.KpiPanel();
			this.averageKpi = new Wisej.Web.KpiPanel();
			this.onTimeKpi = new Wisej.Web.KpiPanel();
			this.revenueCard = new Wisej.Web.Panel();
			this.revenueChart = new Wisej.Web.ChartView();
			this.revenueNote = new Wisej.Web.Label();
			this.categoryCard = new Wisej.Web.Panel();
			this.categoryChart = new Wisej.Web.ChartView();
			this.categoryNote = new Wisej.Web.Label();
			this.productsCard = new Wisej.Web.Panel();
			this.productsGrid = new Wisej.Web.DataGridView();
			this.colProduct = new Wisej.Web.DataGridViewSubtitleColumn();
			this.colUnitsTrend = new Wisej.Web.DataGridViewSparklineColumn();
			this.colUnits = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colRevenue = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colShare = new Wisej.Web.DataGridViewMeterColumn();
			this.colChange = new Wisej.Web.DataGridViewChipColumn();
			this.activityCard = new Wisej.Web.Panel();
			this.activityTimeline = new Wisej.Web.Timeline();
			this.heatmapCard = new Wisej.Web.Panel();
			this.ordersHeatmap = new Wisej.Web.HeatmapCalendar();
			this.heatmapNote = new Wisej.Web.Label();
			this.openOrdersCard = new Wisej.Web.Panel();
			this.fulfillmentButton = new Wisej.Web.Button();
			this.dueMeter = new Wisej.Web.MeterBar();
			this.stagesMeter = new Wisej.Web.MeterBar();
			this.openOrdersCaption = new Wisej.Web.Label();
			this.openOrdersValue = new Wisej.Web.Label();
			this.layout.SuspendLayout();
			this.revenueCard.SuspendLayout();
			this.categoryCard.SuspendLayout();
			this.productsCard.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.productsGrid)).BeginInit();
			this.activityCard.SuspendLayout();
			this.heatmapCard.SuspendLayout();
			this.openOrdersCard.SuspendLayout();
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
			this.layout.Controls.Add(this.revenueCard, 0, 1);
			this.layout.Controls.Add(this.categoryCard, 8, 1);
			this.layout.Controls.Add(this.productsCard, 0, 2);
			this.layout.Controls.Add(this.activityCard, 8, 2);
			this.layout.Controls.Add(this.heatmapCard, 0, 3);
			this.layout.Controls.Add(this.openOrdersCard, 9, 3);
			this.layout.Dock = Wisej.Web.DockStyle.Top;
			this.layout.Location = new System.Drawing.Point(0, 0);
			this.layout.Name = "layout";
			this.layout.Padding = new Wisej.Web.Padding(16);
			this.layout.RowCount = 4;
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 196F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 360F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 500F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 262F));
			this.layout.SetColumnSpan(this.revenueKpi, 3);
			this.layout.SetColumnSpan(this.ordersKpi, 3);
			this.layout.SetColumnSpan(this.averageKpi, 3);
			this.layout.SetColumnSpan(this.onTimeKpi, 3);
			this.layout.SetColumnSpan(this.revenueCard, 8);
			this.layout.SetColumnSpan(this.categoryCard, 4);
			this.layout.SetColumnSpan(this.productsCard, 8);
			this.layout.SetColumnSpan(this.activityCard, 4);
			this.layout.SetColumnSpan(this.heatmapCard, 9);
			this.layout.SetColumnSpan(this.openOrdersCard, 3);
			this.layout.Size = new System.Drawing.Size(1208, 1350);
			this.layout.TabIndex = 0;
			//
			// revenueKpi
			//
			this.revenueKpi.Clickable = true;
			this.revenueKpi.Dock = Wisej.Web.DockStyle.Fill;
			this.revenueKpi.Icon = "Assets/Icons/revenue.svg";
			this.revenueKpi.Location = new System.Drawing.Point(24, 24);
			this.revenueKpi.Margin = new Wisej.Web.Padding(8);
			this.revenueKpi.Name = "revenueKpi";
			this.revenueKpi.Size = new System.Drawing.Size(278, 180);
			this.revenueKpi.TabIndex = 0;
			this.revenueKpi.Title = "REVENUE";
			this.revenueKpi.TrendFormat = "0.0\'%\';0.0\'%\';0\'%\'";
			this.revenueKpi.Value = "$0";
			this.revenueKpi.Visual = Wisej.Web.KpiVisual.Sparkline;
			this.revenueKpi.Click += new System.EventHandler(this.revenueKpi_Click);
			this.revenueKpi.IconClick += new System.EventHandler(this.revenueKpi_Click);
			//
			// ordersKpi
			//
			this.ordersKpi.Clickable = true;
			this.ordersKpi.Dock = Wisej.Web.DockStyle.Fill;
			this.ordersKpi.Icon = "Assets/Icons/cart.svg";
			this.ordersKpi.Location = new System.Drawing.Point(318, 24);
			this.ordersKpi.Margin = new Wisej.Web.Padding(8);
			this.ordersKpi.Name = "ordersKpi";
			this.ordersKpi.Size = new System.Drawing.Size(278, 180);
			this.ordersKpi.TabIndex = 1;
			this.ordersKpi.Title = "ORDERS";
			this.ordersKpi.TrendFormat = "0.0\'%\';0.0\'%\';0\'%\'";
			this.ordersKpi.Value = "0";
			this.ordersKpi.Visual = Wisej.Web.KpiVisual.Sparkline;
			this.ordersKpi.Click += new System.EventHandler(this.ordersKpi_Click);
			this.ordersKpi.IconClick += new System.EventHandler(this.ordersKpi_Click);
			//
			// averageKpi
			//
			this.averageKpi.Clickable = true;
			this.averageKpi.Dock = Wisej.Web.DockStyle.Fill;
			this.averageKpi.Icon = "Assets/Icons/tag.svg";
			this.averageKpi.Location = new System.Drawing.Point(612, 24);
			this.averageKpi.Margin = new Wisej.Web.Padding(8);
			this.averageKpi.Name = "averageKpi";
			this.averageKpi.Size = new System.Drawing.Size(278, 180);
			this.averageKpi.TabIndex = 2;
			this.averageKpi.Title = "AVERAGE ORDER";
			this.averageKpi.TrendFormat = "0.0\'%\';0.0\'%\';0\'%\'";
			this.averageKpi.Value = "$0";
			this.averageKpi.Visual = Wisej.Web.KpiVisual.Sparkline;
			this.averageKpi.Click += new System.EventHandler(this.revenueKpi_Click);
			this.averageKpi.IconClick += new System.EventHandler(this.revenueKpi_Click);
			//
			// onTimeKpi
			//
			this.onTimeKpi.Clickable = true;
			this.onTimeKpi.Dock = Wisej.Web.DockStyle.Fill;
			this.onTimeKpi.Icon = "Assets/Icons/clock.svg";
			this.onTimeKpi.Location = new System.Drawing.Point(906, 24);
			this.onTimeKpi.Margin = new Wisej.Web.Padding(8);
			this.onTimeKpi.Name = "onTimeKpi";
			this.onTimeKpi.Size = new System.Drawing.Size(278, 180);
			this.onTimeKpi.TabIndex = 3;
			this.onTimeKpi.Title = "SHIPPED ON TIME";
			this.onTimeKpi.Tone = Wisej.Web.KpiTone.Success;
			this.onTimeKpi.TrendFormat = "0.0\' pts\';0.0\' pts\';0\' pts\'";
			this.onTimeKpi.Value = "0%";
			this.onTimeKpi.Visual = Wisej.Web.KpiVisual.Gauge;
			this.onTimeKpi.Click += new System.EventHandler(this.onTimeKpi_Click);
			this.onTimeKpi.IconClick += new System.EventHandler(this.onTimeKpi_Click);
			//
			// revenueCard
			//
			this.revenueCard.AppearanceKey = "nw-card";
			this.revenueCard.Controls.Add(this.revenueChart);
			this.revenueCard.Controls.Add(this.revenueNote);
			this.revenueCard.Dock = Wisej.Web.DockStyle.Fill;
			this.revenueCard.HeaderSize = 46;
			this.revenueCard.Location = new System.Drawing.Point(24, 220);
			this.revenueCard.Margin = new Wisej.Web.Padding(8);
			this.revenueCard.Name = "revenueCard";
			this.revenueCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.revenueCard.ShowHeader = true;
			this.revenueCard.Size = new System.Drawing.Size(768, 344);
			this.revenueCard.TabIndex = 4;
			this.revenueCard.Text = "Revenue";
			componentTool1.ImageSource = "Assets/Icons/download.svg";
			componentTool1.Name = "export";
			componentTool1.ToolTipText = "Download the chart as PNG";
			this.revenueCard.Tools.AddRange(new Wisej.Web.ComponentTool[] {
            componentTool1});
			this.revenueCard.ToolClick += new Wisej.Web.ToolClickEventHandler(this.revenueCard_ToolClick);
			//
			// revenueChart
			//
			this.revenueChart.AccessibleName = "Revenue trend";
			this.revenueChart.Dock = Wisej.Web.DockStyle.Fill;
			this.revenueChart.EmptyState = "No orders in this period";
			this.revenueChart.Location = new System.Drawing.Point(16, 22);
			this.revenueChart.Name = "revenueChart";
			chartSeries1.CategoryMember = "Label";
			chartSeries1.ChartType = Wisej.Web.ChartType.Area;
			chartSeries1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
			chartSeries1.FillOpacity = 0.12D;
			chartSeries1.LabelFormat = "{0:C0}";
			chartSeries1.Name = "This period";
			chartSeries1.Smooth = true;
			chartSeries1.ValueMember = "Revenue";
			chartSeries2.CategoryMember = "Label";
			chartSeries2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(173)))), ((int)(((byte)(191)))));
			chartSeries2.LabelFormat = "{0:C0}";
			chartSeries2.LineStyle = Wisej.Web.ChartLineStyle.Dashed;
			chartSeries2.Markers = Wisej.Web.ChartMarkers.None;
			chartSeries2.Name = "Comparison period";
			chartSeries2.Smooth = true;
			chartSeries2.ValueMember = "PreviousRevenue";
			this.revenueChart.Series.Add(chartSeries1);
			this.revenueChart.Series.Add(chartSeries2);
			this.revenueChart.Size = new System.Drawing.Size(736, 264);
			this.revenueChart.TabIndex = 1;
			this.revenueChart.YAxis.Format = "N0";
			this.revenueChart.YAxis.Min = 0D;
			//
			// revenueNote
			//
			this.revenueNote.AppearanceKey = "nw-caption";
			this.revenueNote.Dock = Wisej.Web.DockStyle.Top;
			this.revenueNote.Location = new System.Drawing.Point(16, 0);
			this.revenueNote.Name = "revenueNote";
			this.revenueNote.Size = new System.Drawing.Size(736, 22);
			this.revenueNote.TabIndex = 0;
			this.revenueNote.Text = "Net merchandise value by week";
			//
			// categoryCard
			//
			this.categoryCard.AppearanceKey = "nw-card";
			this.categoryCard.Controls.Add(this.categoryChart);
			this.categoryCard.Controls.Add(this.categoryNote);
			this.categoryCard.Dock = Wisej.Web.DockStyle.Fill;
			this.categoryCard.HeaderSize = 46;
			this.categoryCard.Location = new System.Drawing.Point(808, 220);
			this.categoryCard.Margin = new Wisej.Web.Padding(8);
			this.categoryCard.Name = "categoryCard";
			this.categoryCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.categoryCard.ShowHeader = true;
			this.categoryCard.Size = new System.Drawing.Size(376, 344);
			this.categoryCard.TabIndex = 5;
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
			chartSeries3.CategoryMember = "Name";
			chartSeries3.ChartType = Wisej.Web.ChartType.Bar;
			chartSeries3.Color = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
			chartSeries3.LabelFormat = "{0:C0}";
			chartSeries3.Name = "Revenue";
			chartSeries3.ValueMember = "Revenue";
			this.categoryChart.Series.Add(chartSeries3);
			this.categoryChart.Size = new System.Drawing.Size(344, 264);
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
			this.categoryNote.Size = new System.Drawing.Size(344, 22);
			this.categoryNote.TabIndex = 0;
			this.categoryNote.Text = "Select a bar to see the category\'s products";
			//
			// productsCard
			//
			this.productsCard.AppearanceKey = "nw-card";
			this.productsCard.Controls.Add(this.productsGrid);
			this.productsCard.Dock = Wisej.Web.DockStyle.Fill;
			this.productsCard.HeaderSize = 46;
			this.productsCard.Location = new System.Drawing.Point(24, 580);
			this.productsCard.Margin = new Wisej.Web.Padding(8);
			this.productsCard.Name = "productsCard";
			this.productsCard.Padding = new Wisej.Web.Padding(4, 0, 4, 8);
			this.productsCard.ShowHeader = true;
			this.productsCard.Size = new System.Drawing.Size(768, 484);
			this.productsCard.TabIndex = 6;
			this.productsCard.Text = "Top products";
			componentTool2.ImageSource = "Assets/Icons/arrow-right.svg";
			componentTool2.Name = "products";
			componentTool2.ToolTipText = "Open the product catalog";
			this.productsCard.Tools.AddRange(new Wisej.Web.ComponentTool[] {
            componentTool2});
			this.productsCard.ToolClick += new Wisej.Web.ToolClickEventHandler(this.productsCard_ToolClick);
			//
			// productsGrid
			//
			this.productsGrid.AllowUserToResizeRows = false;
			this.productsGrid.AutoGenerateColumns = false;
			this.productsGrid.BorderStyle = Wisej.Web.BorderStyle.None;
			this.productsGrid.CellBorderStyle = Wisej.Web.DataGridViewCellBorderStyle.Horizontal;
			dataGridViewCellStyle5.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
			this.productsGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
			this.productsGrid.ColumnHeadersHeight = 36;
			this.productsGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colProduct,
            this.colUnitsTrend,
            this.colUnits,
            this.colRevenue,
            this.colShare,
            this.colChange});
			this.productsGrid.DefaultRowHeight = 48;
			this.productsGrid.Dock = Wisej.Web.DockStyle.Fill;
			this.productsGrid.Location = new System.Drawing.Point(4, 0);
			this.productsGrid.MultiSelect = false;
			this.productsGrid.Name = "productsGrid";
			this.productsGrid.ReadOnly = true;
			this.productsGrid.RowHeadersVisible = false;
			this.productsGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
			this.productsGrid.ShowColumnVisibilityMenu = false;
			this.productsGrid.Size = new System.Drawing.Size(760, 430);
			this.productsGrid.TabIndex = 0;
			this.productsGrid.CellDoubleClick += new Wisej.Web.DataGridViewCellEventHandler(this.productsGrid_CellDoubleClick);
			//
			// colProduct
			//
			this.colProduct.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
			this.colProduct.DataPropertyName = "Name";
			this.colProduct.HeaderText = "PRODUCT";
			this.colProduct.MinimumWidth = 130;
			this.colProduct.Name = "colProduct";
			this.colProduct.SubtitleMember = "CategoryName";
			//
			// colUnitsTrend
			//
			this.colUnitsTrend.ChartType = Wisej.Web.SparklineChartType.Bar;
			this.colUnitsTrend.DataPropertyName = "UnitsTrend";
			this.colUnitsTrend.HeaderText = "UNITS SOLD";
			this.colUnitsTrend.Name = "colUnitsTrend";
			this.colUnitsTrend.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colUnitsTrend.ResponsiveProfiles"))));
			this.colUnitsTrend.Width = 130;

			//
			// colUnits
			//
			dataGridViewCellStyle1.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle1.Format = "N0";
			this.colUnits.DefaultCellStyle = dataGridViewCellStyle1;
			this.colUnits.DataPropertyName = "Units";
			dataGridViewCellStyle2.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colUnits.HeaderStyle = dataGridViewCellStyle2;
			this.colUnits.HeaderText = "UNITS";
			this.colUnits.Name = "colUnits";
			this.colUnits.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colUnits.ResponsiveProfiles"))));
			this.colUnits.Width = 72;
			//
			// colRevenue
			//
			dataGridViewCellStyle3.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle3.Format = "C0";
			this.colRevenue.DefaultCellStyle = dataGridViewCellStyle3;
			this.colRevenue.DataPropertyName = "Revenue";
			dataGridViewCellStyle4.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colRevenue.HeaderStyle = dataGridViewCellStyle4;
			this.colRevenue.HeaderText = "REVENUE";
			this.colRevenue.Name = "colRevenue";
			this.colRevenue.Width = 96;
			//
			// colShare
			//
			this.colShare.DataPropertyName = "Share";
			this.colShare.HeaderText = "SHARE";
			this.colShare.Name = "colShare";
			this.colShare.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colShare.ResponsiveProfiles"))));
			this.colShare.TextMember = "ShareText";
			this.colShare.Width = 132;
			//
			// colChange
			//
			this.colChange.DataPropertyName = "ChangeText";
			this.colChange.HeaderText = "VS. PRIOR";
			this.colChange.Name = "colChange";
			this.colChange.ToneMember = "ChangeTone";
			this.colChange.Width = 100;
			//
			// activityCard
			//
			this.activityCard.AppearanceKey = "nw-card";
			this.activityCard.Controls.Add(this.activityTimeline);
			this.activityCard.Dock = Wisej.Web.DockStyle.Fill;
			this.activityCard.HeaderSize = 46;
			this.activityCard.Location = new System.Drawing.Point(808, 580);
			this.activityCard.Margin = new Wisej.Web.Padding(8);
			this.activityCard.Name = "activityCard";
			this.activityCard.Padding = new Wisej.Web.Padding(8, 0, 8, 8);
			this.activityCard.ShowHeader = true;
			this.activityCard.Size = new System.Drawing.Size(376, 484);
			this.activityCard.TabIndex = 7;
			this.activityCard.Text = "Recent orders";
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
			this.activityTimeline.Size = new System.Drawing.Size(360, 430);
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
			this.heatmapCard.Location = new System.Drawing.Point(24, 1080);
			this.heatmapCard.Margin = new Wisej.Web.Padding(8);
			this.heatmapCard.Name = "heatmapCard";
			this.heatmapCard.Padding = new Wisej.Web.Padding(16, 0, 16, 8);
			this.heatmapCard.ShowHeader = true;
			this.heatmapCard.Size = new System.Drawing.Size(866, 246);
			this.heatmapCard.TabIndex = 8;
			this.heatmapCard.Text = "Order activity";
			//
			// ordersHeatmap
			//
			this.ordersHeatmap.AccessibleName = "Orders per day";
			this.ordersHeatmap.CellSize = 11;
			this.ordersHeatmap.Dock = Wisej.Web.DockStyle.Fill;
			this.ordersHeatmap.Location = new System.Drawing.Point(16, 22);
			this.ordersHeatmap.Name = "ordersHeatmap";
			this.ordersHeatmap.Size = new System.Drawing.Size(834, 170);
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
			this.heatmapNote.Size = new System.Drawing.Size(834, 22);
			this.heatmapNote.TabIndex = 0;
			this.heatmapNote.Text = "Orders per day over the last 12 months · select a day to list its orders";
			//
			// openOrdersCard
			//
			this.openOrdersCard.AppearanceKey = "nw-card";
			this.openOrdersCard.Controls.Add(this.fulfillmentButton);
			this.openOrdersCard.Controls.Add(this.dueMeter);
			this.openOrdersCard.Controls.Add(this.stagesMeter);
			this.openOrdersCard.Controls.Add(this.openOrdersCaption);
			this.openOrdersCard.Controls.Add(this.openOrdersValue);
			this.openOrdersCard.Dock = Wisej.Web.DockStyle.Fill;
			this.openOrdersCard.HeaderSize = 46;
			this.openOrdersCard.Location = new System.Drawing.Point(906, 1080);
			this.openOrdersCard.Margin = new Wisej.Web.Padding(8);
			this.openOrdersCard.Name = "openOrdersCard";
			this.openOrdersCard.ShowHeader = true;
			this.openOrdersCard.Size = new System.Drawing.Size(278, 246);
			this.openOrdersCard.TabIndex = 9;
			this.openOrdersCard.Text = "Open orders";
			//
			// fulfillmentButton
			//
			this.fulfillmentButton.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.fulfillmentButton.AppearanceKey = "nw-button";
			this.fulfillmentButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.fulfillmentButton.ImageSource = "Assets/Icons/fulfillment.svg";
			this.fulfillmentButton.Location = new System.Drawing.Point(16, 158);
			this.fulfillmentButton.Name = "fulfillmentButton";
			this.fulfillmentButton.Size = new System.Drawing.Size(244, 34);
			this.fulfillmentButton.TabIndex = 4;
			this.fulfillmentButton.Text = "Open fulfillment board";
			this.fulfillmentButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.fulfillmentButton.TextImageRelation = Wisej.Web.TextImageRelation.ImageBeforeText;
			this.fulfillmentButton.Click += new System.EventHandler(this.fulfillmentButton_Click);
			//
			// dueMeter
			//
			this.dueMeter.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.dueMeter.BarHeight = 8;
			this.dueMeter.Format = "{0:N0} of {1:N0}";
			this.dueMeter.Label = "Due within 7 days";
			this.dueMeter.Location = new System.Drawing.Point(16, 108);
			this.dueMeter.Name = "dueMeter";
			this.dueMeter.Size = new System.Drawing.Size(244, 42);
			this.dueMeter.TabIndex = 3;
			//
			// stagesMeter
			//
			this.stagesMeter.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.stagesMeter.BarHeight = 8;
			this.stagesMeter.Label = "Steps";
			this.stagesMeter.Location = new System.Drawing.Point(16, 60);
			this.stagesMeter.Name = "stagesMeter";
			this.stagesMeter.Size = new System.Drawing.Size(244, 42);
			this.stagesMeter.TabIndex = 2;
			//
			// openOrdersCaption
			//
			this.openOrdersCaption.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.openOrdersCaption.AppearanceKey = "nw-subtitle";
			this.openOrdersCaption.AutoEllipsis = true;
			this.openOrdersCaption.Location = new System.Drawing.Point(16, 34);
			this.openOrdersCaption.Name = "openOrdersCaption";
			this.openOrdersCaption.Size = new System.Drawing.Size(244, 18);
			this.openOrdersCaption.TabIndex = 1;
			this.openOrdersCaption.Text = "awaiting shipment";
			//
			// openOrdersValue
			//
			this.openOrdersValue.AppearanceKey = "nw-metric";
			this.openOrdersValue.Location = new System.Drawing.Point(16, 2);
			this.openOrdersValue.Name = "openOrdersValue";
			this.openOrdersValue.Size = new System.Drawing.Size(200, 32);
			this.openOrdersValue.TabIndex = 0;
			this.openOrdersValue.Text = "0 orders";
			//
			// OverviewView
			//
			this.AppearanceKey = "nw-view";
			this.AutoScroll = true;
			this.Controls.Add(this.layout);
			this.Name = "OverviewView";
			this.Size = new System.Drawing.Size(1208, 828);
			this.Text = "Overview";
			this.layout.ResumeLayout(false);
			this.revenueCard.ResumeLayout(false);
			this.categoryCard.ResumeLayout(false);
			this.productsCard.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.productsGrid)).EndInit();
			this.activityCard.ResumeLayout(false);
			this.heatmapCard.ResumeLayout(false);
			this.openOrdersCard.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.TableLayoutPanel layout;
		private Wisej.Web.KpiPanel revenueKpi;
		private Wisej.Web.KpiPanel ordersKpi;
		private Wisej.Web.KpiPanel averageKpi;
		private Wisej.Web.KpiPanel onTimeKpi;
		private Wisej.Web.Panel revenueCard;
		private Wisej.Web.ChartView revenueChart;
		private Wisej.Web.Label revenueNote;
		private Wisej.Web.Panel categoryCard;
		private Wisej.Web.ChartView categoryChart;
		private Wisej.Web.Label categoryNote;
		private Wisej.Web.Panel productsCard;
		private Wisej.Web.DataGridView productsGrid;
		private Wisej.Web.DataGridViewSubtitleColumn colProduct;
		private Wisej.Web.DataGridViewSparklineColumn colUnitsTrend;
		private Wisej.Web.DataGridViewTextBoxColumn colUnits;
		private Wisej.Web.DataGridViewTextBoxColumn colRevenue;
		private Wisej.Web.DataGridViewMeterColumn colShare;
		private Wisej.Web.DataGridViewChipColumn colChange;
		private Wisej.Web.Panel activityCard;
		private Wisej.Web.Timeline activityTimeline;
		private Wisej.Web.Panel heatmapCard;
		private Wisej.Web.HeatmapCalendar ordersHeatmap;
		private Wisej.Web.Label heatmapNote;
		private Wisej.Web.Panel openOrdersCard;
		private Wisej.Web.Label openOrdersValue;
		private Wisej.Web.Label openOrdersCaption;
		private Wisej.Web.MeterBar stagesMeter;
		private Wisej.Web.MeterBar dueMeter;
		private Wisej.Web.Button fulfillmentButton;
	}
}
