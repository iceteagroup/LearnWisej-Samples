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
	partial class SalesView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SalesView));
			Wisej.Web.ComponentTool componentTool1 = new Wisej.Web.ComponentTool();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle1 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle2 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle3 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.ChartSeries chartSeries1 = new Wisej.Web.ChartSeries();
			Wisej.Web.ChartSeries chartSeries2 = new Wisej.Web.ChartSeries();
			Wisej.Web.ChartSeries chartSeries3 = new Wisej.Web.ChartSeries();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle4 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle5 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle6 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle7 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle8 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle9 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle10 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle11 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle12 = new Wisej.Web.DataGridViewCellStyle();
			this.layout = new Wisej.Web.TableLayoutPanel();
			this.categoryTrendCard = new Wisej.Web.Panel();
			this.categoryTrendChart = new Wisej.Web.ChartView();
			this.categoryTrendNote = new Wisej.Web.Label();
			this.categoryTableCard = new Wisej.Web.Panel();
			this.categoryGrid = new Wisej.Web.DataGridView();
			this.colCategory = new Wisej.Web.DataGridViewSubtitleColumn();
			this.colCategoryRevenue = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colCategoryChange = new Wisej.Web.DataGridViewChipColumn();
			this.bridgeCard = new Wisej.Web.Panel();
			this.bridgeChart = new Wisej.Web.ChartView();
			this.bridgeNote = new Wisej.Web.Label();
			this.countriesCard = new Wisej.Web.Panel();
			this.countriesChart = new Wisej.Web.ChartView();
			this.countriesNote = new Wisej.Web.Label();
			this.mixCard = new Wisej.Web.Panel();
			this.mixChart = new Wisej.Web.ChartView();
			this.mixNote = new Wisej.Web.Label();
			this.periodCard = new Wisej.Web.Panel();
			this.periodGrid = new Wisej.Web.DataGridView();
			this.colPeriod = new Wisej.Web.DataGridViewSubtitleColumn();
			this.colPeriodOrders = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colPeriodRevenue = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colPeriodAverage = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colPeriodPrevious = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colPeriodChange = new Wisej.Web.DataGridViewChipColumn();
			this.layout.SuspendLayout();
			this.categoryTrendCard.SuspendLayout();
			this.categoryTableCard.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.categoryGrid)).BeginInit();
			this.bridgeCard.SuspendLayout();
			this.countriesCard.SuspendLayout();
			this.mixCard.SuspendLayout();
			this.periodCard.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.periodGrid)).BeginInit();
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
			this.layout.Controls.Add(this.categoryTrendCard, 0, 0);
			this.layout.Controls.Add(this.categoryTableCard, 8, 0);
			this.layout.Controls.Add(this.bridgeCard, 0, 1);
			this.layout.Controls.Add(this.countriesCard, 0, 2);
			this.layout.Controls.Add(this.mixCard, 4, 2);
			this.layout.Controls.Add(this.periodCard, 0, 3);
			this.layout.Dock = Wisej.Web.DockStyle.Top;
			this.layout.Location = new System.Drawing.Point(0, 0);
			this.layout.Name = "layout";
			this.layout.Padding = new Wisej.Web.Padding(16);
			this.layout.RowCount = 4;
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 440F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 360F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 440F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 470F));
			this.layout.SetColumnSpan(this.categoryTrendCard, 8);
			this.layout.SetColumnSpan(this.categoryTableCard, 4);
			this.layout.SetColumnSpan(this.bridgeCard, 12);
			this.layout.SetColumnSpan(this.countriesCard, 4);
			this.layout.SetColumnSpan(this.mixCard, 8);
			this.layout.SetColumnSpan(this.periodCard, 12);
			this.layout.Size = new System.Drawing.Size(1208, 1742);
			this.layout.TabIndex = 0;
			//
			// categoryTrendCard
			//
			this.categoryTrendCard.AppearanceKey = "nw-card";
			this.categoryTrendCard.Controls.Add(this.categoryTrendChart);
			this.categoryTrendCard.Controls.Add(this.categoryTrendNote);
			this.categoryTrendCard.Dock = Wisej.Web.DockStyle.Fill;
			this.categoryTrendCard.HeaderSize = 46;
			this.categoryTrendCard.Location = new System.Drawing.Point(24, 24);
			this.categoryTrendCard.Margin = new Wisej.Web.Padding(8);
			this.categoryTrendCard.Name = "categoryTrendCard";
			this.categoryTrendCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.categoryTrendCard.ShowHeader = true;
			this.categoryTrendCard.Size = new System.Drawing.Size(768, 404);
			this.categoryTrendCard.TabIndex = 0;
			this.categoryTrendCard.Text = "Revenue by category";
			componentTool1.ImageSource = "Assets/Icons/download.svg";
			componentTool1.Name = "export";
			componentTool1.ToolTipText = "Download the chart as PNG";
			this.categoryTrendCard.Tools.AddRange(new Wisej.Web.ComponentTool[] {
            componentTool1});
			this.categoryTrendCard.ToolClick += new Wisej.Web.ToolClickEventHandler(this.categoryTrendCard_ToolClick);
			//
			// categoryTrendChart
			//
			this.categoryTrendChart.AccessibleName = "Revenue by category over time";
			this.categoryTrendChart.Dock = Wisej.Web.DockStyle.Fill;
			this.categoryTrendChart.EmptyState = "No orders in this period";
			this.categoryTrendChart.Location = new System.Drawing.Point(16, 22);
			this.categoryTrendChart.Name = "categoryTrendChart";
			this.categoryTrendChart.Size = new System.Drawing.Size(736, 324);
			this.categoryTrendChart.TabIndex = 1;
			this.categoryTrendChart.YAxis.Format = "N0";
			this.categoryTrendChart.YAxis.Min = 0D;
			this.categoryTrendChart.LegendItemClick += new System.EventHandler<Wisej.Web.SeriesPointEventArgs>(this.categoryTrendChart_LegendItemClick);
			//
			// categoryTrendNote
			//
			this.categoryTrendNote.AppearanceKey = "nw-caption";
			this.categoryTrendNote.Dock = Wisej.Web.DockStyle.Top;
			this.categoryTrendNote.Location = new System.Drawing.Point(16, 0);
			this.categoryTrendNote.Name = "categoryTrendNote";
			this.categoryTrendNote.Size = new System.Drawing.Size(736, 22);
			this.categoryTrendNote.TabIndex = 0;
			this.categoryTrendNote.Text = "Stacked net revenue per week · select a legend entry to hide a category";
			//
			// categoryTableCard
			//
			this.categoryTableCard.AppearanceKey = "nw-card";
			this.categoryTableCard.Controls.Add(this.categoryGrid);
			this.categoryTableCard.Dock = Wisej.Web.DockStyle.Fill;
			this.categoryTableCard.HeaderSize = 46;
			this.categoryTableCard.Location = new System.Drawing.Point(808, 24);
			this.categoryTableCard.Margin = new Wisej.Web.Padding(8);
			this.categoryTableCard.Name = "categoryTableCard";
			this.categoryTableCard.Padding = new Wisej.Web.Padding(4, 0, 4, 8);
			this.categoryTableCard.ShowHeader = true;
			this.categoryTableCard.Size = new System.Drawing.Size(376, 404);
			this.categoryTableCard.TabIndex = 1;
			this.categoryTableCard.Text = "Category performance";
			//
			// categoryGrid
			//
			this.categoryGrid.AllowUserToResizeRows = false;
			this.categoryGrid.AutoGenerateColumns = false;
			this.categoryGrid.BorderStyle = Wisej.Web.BorderStyle.None;
			this.categoryGrid.CellBorderStyle = Wisej.Web.DataGridViewCellBorderStyle.Horizontal;
			dataGridViewCellStyle1.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
			this.categoryGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.categoryGrid.ColumnHeadersHeight = 36;
			this.categoryGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colCategory,
            this.colCategoryRevenue,
            this.colCategoryChange});
			this.categoryGrid.DefaultRowHeight = 41;
			this.categoryGrid.Dock = Wisej.Web.DockStyle.Fill;
			this.categoryGrid.Location = new System.Drawing.Point(4, 0);
			this.categoryGrid.MultiSelect = false;
			this.categoryGrid.Name = "categoryGrid";
			this.categoryGrid.ReadOnly = true;
			this.categoryGrid.RowHeadersVisible = false;
			this.categoryGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
			this.categoryGrid.ShowColumnVisibilityMenu = false;
			this.categoryGrid.Size = new System.Drawing.Size(368, 350);
			this.categoryGrid.TabIndex = 0;
			this.categoryGrid.CellDoubleClick += new Wisej.Web.DataGridViewCellEventHandler(this.categoryGrid_CellDoubleClick);
			//
			// colCategory
			//
			this.colCategory.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
			this.colCategory.DataPropertyName = "Name";
			this.colCategory.HeaderText = "CATEGORY";
			this.colCategory.Name = "colCategory";
			this.colCategory.SubtitleMember = "ShareText";
			//
			// colCategoryRevenue
			//
			dataGridViewCellStyle2.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle2.Format = "C0";
			this.colCategoryRevenue.DefaultCellStyle = dataGridViewCellStyle2;
			this.colCategoryRevenue.DataPropertyName = "Revenue";
			dataGridViewCellStyle3.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colCategoryRevenue.HeaderStyle = dataGridViewCellStyle3;
			this.colCategoryRevenue.HeaderText = "REVENUE";
			this.colCategoryRevenue.Name = "colCategoryRevenue";
			this.colCategoryRevenue.Width = 96;
			//
			// colCategoryChange
			//
			this.colCategoryChange.DataPropertyName = "ChangeText";
			this.colCategoryChange.HeaderText = "VS. PRIOR";
			this.colCategoryChange.Name = "colCategoryChange";
			this.colCategoryChange.ToneMember = "ChangeTone";
			this.colCategoryChange.Width = 96;
			//
			// bridgeCard
			//
			this.bridgeCard.AppearanceKey = "nw-card";
			this.bridgeCard.Controls.Add(this.bridgeChart);
			this.bridgeCard.Controls.Add(this.bridgeNote);
			this.bridgeCard.Dock = Wisej.Web.DockStyle.Fill;
			this.bridgeCard.HeaderSize = 46;
			this.bridgeCard.Location = new System.Drawing.Point(24, 444);
			this.bridgeCard.Margin = new Wisej.Web.Padding(8);
			this.bridgeCard.Name = "bridgeCard";
			this.bridgeCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.bridgeCard.ShowHeader = true;
			this.bridgeCard.Size = new System.Drawing.Size(1160, 344);
			this.bridgeCard.TabIndex = 2;
			this.bridgeCard.Text = "Revenue bridge";
			//
			// bridgeChart
			//
			this.bridgeChart.AccessibleName = "Revenue change by category";
			this.bridgeChart.Dock = Wisej.Web.DockStyle.Fill;
			this.bridgeChart.EmptyState = "Choose 30D, 90D or YTD to compare with an earlier period";
			this.bridgeChart.Legend = Wisej.Web.ChartLegend.Hidden;
			this.bridgeChart.Location = new System.Drawing.Point(16, 22);
			this.bridgeChart.Name = "bridgeChart";
			chartSeries1.ChartType = Wisej.Web.ChartType.Waterfall;
			chartSeries1.LabelFormat = "{0:C0}";
			chartSeries1.Name = "Revenue change";
			this.bridgeChart.Series.Add(chartSeries1);
			this.bridgeChart.Size = new System.Drawing.Size(1128, 264);
			this.bridgeChart.TabIndex = 1;
			this.bridgeChart.YAxis.Format = "N0";
			//
			// bridgeNote
			//
			this.bridgeNote.AppearanceKey = "nw-caption";
			this.bridgeNote.Dock = Wisej.Web.DockStyle.Top;
			this.bridgeNote.Location = new System.Drawing.Point(16, 0);
			this.bridgeNote.Name = "bridgeNote";
			this.bridgeNote.Size = new System.Drawing.Size(1128, 22);
			this.bridgeNote.TabIndex = 0;
			this.bridgeNote.Text = "How each category moved revenue from the comparison period to this period";
			//
			// countriesCard
			//
			this.countriesCard.AppearanceKey = "nw-card";
			this.countriesCard.Controls.Add(this.countriesChart);
			this.countriesCard.Controls.Add(this.countriesNote);
			this.countriesCard.Dock = Wisej.Web.DockStyle.Fill;
			this.countriesCard.HeaderSize = 46;
			this.countriesCard.Location = new System.Drawing.Point(24, 804);
			this.countriesCard.Margin = new Wisej.Web.Padding(8);
			this.countriesCard.Name = "countriesCard";
			this.countriesCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.countriesCard.ShowHeader = true;
			this.countriesCard.Size = new System.Drawing.Size(376, 424);
			this.countriesCard.TabIndex = 3;
			this.countriesCard.Text = "Top countries";
			//
			// countriesChart
			//
			this.countriesChart.AccessibleName = "Revenue by country";
			this.countriesChart.Dock = Wisej.Web.DockStyle.Fill;
			this.countriesChart.EmptyState = "No orders in this period";
			this.countriesChart.Legend = Wisej.Web.ChartLegend.Hidden;
			this.countriesChart.Location = new System.Drawing.Point(16, 22);
			this.countriesChart.Name = "countriesChart";
			chartSeries2.CategoryMember = "Country";
			chartSeries2.ChartType = Wisej.Web.ChartType.Bar;
			chartSeries2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
			chartSeries2.LabelFormat = "{0:C0}";
			chartSeries2.Name = "Revenue";
			chartSeries2.ValueMember = "Revenue";
			this.countriesChart.Series.Add(chartSeries2);
			this.countriesChart.Size = new System.Drawing.Size(344, 344);
			this.countriesChart.TabIndex = 1;
			this.countriesChart.YAxis.Format = "N0";
			this.countriesChart.YAxis.Min = 0D;
			this.countriesChart.PointClick += new System.EventHandler<Wisej.Web.SeriesPointEventArgs>(this.countriesChart_PointClick);
			//
			// countriesNote
			//
			this.countriesNote.AppearanceKey = "nw-caption";
			this.countriesNote.Dock = Wisej.Web.DockStyle.Top;
			this.countriesNote.Location = new System.Drawing.Point(16, 0);
			this.countriesNote.Name = "countriesNote";
			this.countriesNote.Size = new System.Drawing.Size(344, 22);
			this.countriesNote.TabIndex = 0;
			this.countriesNote.Text = "Ten largest markets · select a bar to see its customers";
			//
			// mixCard
			//
			this.mixCard.AppearanceKey = "nw-card";
			this.mixCard.Controls.Add(this.mixChart);
			this.mixCard.Controls.Add(this.mixNote);
			this.mixCard.Dock = Wisej.Web.DockStyle.Fill;
			this.mixCard.HeaderSize = 46;
			this.mixCard.Location = new System.Drawing.Point(416, 804);
			this.mixCard.Margin = new Wisej.Web.Padding(8);
			this.mixCard.Name = "mixCard";
			this.mixCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.mixCard.ShowHeader = true;
			this.mixCard.Size = new System.Drawing.Size(768, 424);
			this.mixCard.TabIndex = 4;
			this.mixCard.Text = "Product mix";
			//
			// mixChart
			//
			this.mixChart.AccessibleName = "Revenue by product";
			this.mixChart.Dock = Wisej.Web.DockStyle.Fill;
			this.mixChart.EmptyState = "No orders in this period";
			this.mixChart.Legend = Wisej.Web.ChartLegend.Hidden;
			this.mixChart.Location = new System.Drawing.Point(16, 22);
			this.mixChart.Name = "mixChart";
			chartSeries3.CategoryMember = "Name";
			chartSeries3.ChartType = Wisej.Web.ChartType.Treemap;
			chartSeries3.Color = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
			chartSeries3.ColorMember = "Change";
			chartSeries3.DownTone = Wisej.Web.ChartTone.Neutral;
			chartSeries3.LabelFormat = "{0:C0}";
			chartSeries3.Name = "Revenue";
			chartSeries3.UpTone = Wisej.Web.ChartTone.Primary;
			chartSeries3.ValueMember = "Revenue";
			this.mixChart.Series.Add(chartSeries3);
			this.mixChart.Size = new System.Drawing.Size(736, 344);
			this.mixChart.TabIndex = 1;
			this.mixChart.PointClick += new System.EventHandler<Wisej.Web.SeriesPointEventArgs>(this.mixChart_PointClick);
			//
			// mixNote
			//
			this.mixNote.AppearanceKey = "nw-caption";
			this.mixNote.Dock = Wisej.Web.DockStyle.Top;
			this.mixNote.Location = new System.Drawing.Point(16, 0);
			this.mixNote.Name = "mixNote";
			this.mixNote.Size = new System.Drawing.Size(736, 22);
			this.mixNote.TabIndex = 0;
			this.mixNote.Text = "Top 20 products sized by revenue";
			//
			// periodCard
			//
			this.periodCard.AppearanceKey = "nw-card";
			this.periodCard.Controls.Add(this.periodGrid);
			this.periodCard.Dock = Wisej.Web.DockStyle.Fill;
			this.periodCard.HeaderSize = 46;
			this.periodCard.Location = new System.Drawing.Point(24, 1244);
			this.periodCard.Margin = new Wisej.Web.Padding(8);
			this.periodCard.Name = "periodCard";
			this.periodCard.Padding = new Wisej.Web.Padding(4, 0, 4, 8);
			this.periodCard.ShowHeader = true;
			this.periodCard.Size = new System.Drawing.Size(1160, 454);
			this.periodCard.TabIndex = 5;
			this.periodCard.Text = "Revenue by period";
			//
			// periodGrid
			//
			this.periodGrid.AllowUserToResizeRows = false;
			this.periodGrid.AutoGenerateColumns = false;
			this.periodGrid.BorderStyle = Wisej.Web.BorderStyle.None;
			this.periodGrid.CellBorderStyle = Wisej.Web.DataGridViewCellBorderStyle.Horizontal;
			dataGridViewCellStyle4.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
			this.periodGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
			this.periodGrid.ColumnHeadersHeight = 36;
			this.periodGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colPeriod,
            this.colPeriodOrders,
            this.colPeriodRevenue,
            this.colPeriodAverage,
            this.colPeriodPrevious,
            this.colPeriodChange});
			this.periodGrid.DefaultRowHeight = 44;
			this.periodGrid.Dock = Wisej.Web.DockStyle.Fill;
			this.periodGrid.Location = new System.Drawing.Point(4, 0);
			this.periodGrid.MultiSelect = false;
			this.periodGrid.Name = "periodGrid";
			this.periodGrid.ReadOnly = true;
			this.periodGrid.RowHeadersVisible = false;
			this.periodGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
			this.periodGrid.ShowColumnVisibilityMenu = false;
			this.periodGrid.Size = new System.Drawing.Size(1152, 400);
			this.periodGrid.TabIndex = 0;
			//
			// colPeriod
			//
			this.colPeriod.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
			this.colPeriod.DataPropertyName = "Label";
			this.colPeriod.HeaderText = "PERIOD";
			this.colPeriod.MinimumWidth = 130;
			this.colPeriod.Name = "colPeriod";
			this.colPeriod.SubtitleMember = "PeriodText";
			//
			// colPeriodOrders
			//
			dataGridViewCellStyle5.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle5.Format = "N0";
			this.colPeriodOrders.DefaultCellStyle = dataGridViewCellStyle5;
			this.colPeriodOrders.DataPropertyName = "Orders";
			dataGridViewCellStyle6.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colPeriodOrders.HeaderStyle = dataGridViewCellStyle6;
			this.colPeriodOrders.HeaderText = "ORDERS";
			this.colPeriodOrders.Name = "colPeriodOrders";
			this.colPeriodOrders.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colPeriodOrders.ResponsiveProfiles"))));
			this.colPeriodOrders.Width = 110;
			//
			// colPeriodRevenue
			//
			dataGridViewCellStyle7.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle7.Format = "C0";
			this.colPeriodRevenue.DefaultCellStyle = dataGridViewCellStyle7;
			this.colPeriodRevenue.DataPropertyName = "Revenue";
			dataGridViewCellStyle8.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colPeriodRevenue.HeaderStyle = dataGridViewCellStyle8;
			this.colPeriodRevenue.HeaderText = "REVENUE";
			this.colPeriodRevenue.Name = "colPeriodRevenue";
			this.colPeriodRevenue.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colPeriodRevenue.ResponsiveProfiles"))));
			this.colPeriodRevenue.Width = 150;
			//
			// colPeriodAverage
			//
			dataGridViewCellStyle9.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle9.Format = "C0";
			this.colPeriodAverage.DefaultCellStyle = dataGridViewCellStyle9;
			this.colPeriodAverage.DataPropertyName = "AverageOrder";
			dataGridViewCellStyle10.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colPeriodAverage.HeaderStyle = dataGridViewCellStyle10;
			this.colPeriodAverage.HeaderText = "AVERAGE ORDER";
			this.colPeriodAverage.Name = "colPeriodAverage";
			this.colPeriodAverage.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colPeriodAverage.ResponsiveProfiles"))));
			this.colPeriodAverage.Width = 150;
			//
			// colPeriodPrevious
			//
			dataGridViewCellStyle11.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle11.Format = "C0";
			this.colPeriodPrevious.DefaultCellStyle = dataGridViewCellStyle11;
			this.colPeriodPrevious.DataPropertyName = "PreviousRevenue";
			dataGridViewCellStyle12.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colPeriodPrevious.HeaderStyle = dataGridViewCellStyle12;
			this.colPeriodPrevious.HeaderText = "COMPARISON";
			this.colPeriodPrevious.Name = "colPeriodPrevious";
			this.colPeriodPrevious.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colPeriodPrevious.ResponsiveProfiles"))));
			this.colPeriodPrevious.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colPeriodPrevious.ResponsiveProfiles1"))));
			this.colPeriodPrevious.Width = 150;
			//
			// colPeriodChange
			//
			this.colPeriodChange.DataPropertyName = "ChangeText";
			this.colPeriodChange.HeaderText = "CHANGE";
			this.colPeriodChange.Name = "colPeriodChange";
			this.colPeriodChange.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colPeriodChange.ResponsiveProfiles"))));
			this.colPeriodChange.ToneMember = "ChangeTone";
			this.colPeriodChange.Width = 120;
			//
			// SalesView
			//
			this.AppearanceKey = "nw-view";
			this.AutoScroll = true;
			this.Controls.Add(this.layout);
			this.Name = "SalesView";
			this.Size = new System.Drawing.Size(1208, 828);
			this.Text = "Sales";
			this.layout.ResumeLayout(false);
			this.categoryTrendCard.ResumeLayout(false);
			this.categoryTableCard.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.categoryGrid)).EndInit();
			this.bridgeCard.ResumeLayout(false);
			this.countriesCard.ResumeLayout(false);
			this.mixCard.ResumeLayout(false);
			this.periodCard.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.periodGrid)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.TableLayoutPanel layout;
		private Wisej.Web.Panel categoryTrendCard;
		private Wisej.Web.ChartView categoryTrendChart;
		private Wisej.Web.Label categoryTrendNote;
		private Wisej.Web.Panel categoryTableCard;
		private Wisej.Web.DataGridView categoryGrid;
		private Wisej.Web.DataGridViewSubtitleColumn colCategory;
		private Wisej.Web.DataGridViewTextBoxColumn colCategoryRevenue;
		private Wisej.Web.DataGridViewChipColumn colCategoryChange;
		private Wisej.Web.Panel bridgeCard;
		private Wisej.Web.ChartView bridgeChart;
		private Wisej.Web.Label bridgeNote;
		private Wisej.Web.Panel countriesCard;
		private Wisej.Web.ChartView countriesChart;
		private Wisej.Web.Label countriesNote;
		private Wisej.Web.Panel mixCard;
		private Wisej.Web.ChartView mixChart;
		private Wisej.Web.Label mixNote;
		private Wisej.Web.Panel periodCard;
		private Wisej.Web.DataGridView periodGrid;
		private Wisej.Web.DataGridViewSubtitleColumn colPeriod;
		private Wisej.Web.DataGridViewTextBoxColumn colPeriodOrders;
		private Wisej.Web.DataGridViewTextBoxColumn colPeriodRevenue;
		private Wisej.Web.DataGridViewTextBoxColumn colPeriodAverage;
		private Wisej.Web.DataGridViewTextBoxColumn colPeriodPrevious;
		private Wisej.Web.DataGridViewChipColumn colPeriodChange;
	}
}
