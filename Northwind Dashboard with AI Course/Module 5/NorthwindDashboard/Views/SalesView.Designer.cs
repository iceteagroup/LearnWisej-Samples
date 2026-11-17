namespace NorthwindDashboard.Views
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
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle1 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle2 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle3 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle4 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle5 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle6 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle7 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.ChartSeries chartSeries1 = new Wisej.Web.ChartSeries();
			Wisej.Web.ChartSeries chartSeries2 = new Wisej.Web.ChartSeries();
			Wisej.Web.ChartSeries chartSeries3 = new Wisej.Web.ChartSeries();
			Wisej.Web.ChartSeries chartSeries4 = new Wisej.Web.ChartSeries();
			Wisej.Web.ChartSeries chartSeries5 = new Wisej.Web.ChartSeries();
			this.layout = new Wisej.Web.TableLayoutPanel();
			this.categoryTrendCard = new Wisej.Web.Panel();
			this.categoryTrendChart = new Wisej.Web.ChartView();
			this.categoryTrendNote = new Wisej.Web.Label();
			this.categoryTableCard = new Wisej.Web.Panel();
			this.categoryGrid = new Wisej.Web.DataGridView();
			this.colCategory = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colCategoryRevenue = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colCategoryShare = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colCategoryChange = new Wisej.Web.DataGridViewTextBoxColumn();
			this.bridgeCard = new Wisej.Web.Panel();
			this.bridgeChart = new Wisej.Web.ChartView();
			this.bridgeNote = new Wisej.Web.Label();
			this.revenueCard = new Wisej.Web.Panel();
			this.revenueChart = new Wisej.Web.ChartView();
			this.revenueNote = new Wisej.Web.Label();
			this.countriesCard = new Wisej.Web.Panel();
			this.countriesChart = new Wisej.Web.ChartView();
			this.countriesNote = new Wisej.Web.Label();
			this.mixCard = new Wisej.Web.Panel();
			this.mixChart = new Wisej.Web.ChartView();
			this.mixNote = new Wisej.Web.Label();
			this.layout.SuspendLayout();
			this.categoryTrendCard.SuspendLayout();
			this.categoryTableCard.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.categoryGrid)).BeginInit();
			this.bridgeCard.SuspendLayout();
			this.revenueCard.SuspendLayout();
			this.countriesCard.SuspendLayout();
			this.mixCard.SuspendLayout();
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
			this.layout.Controls.Add(this.revenueCard, 7, 1);
			this.layout.Controls.Add(this.countriesCard, 0, 2);
			this.layout.Controls.Add(this.mixCard, 4, 2);
			this.layout.Dock = Wisej.Web.DockStyle.Top;
			this.layout.Location = new System.Drawing.Point(0, 0);
			this.layout.Name = "layout";
			this.layout.RowCount = 3;
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 416F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 360F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 436F));
			this.layout.SetColumnSpan(this.categoryTrendCard, 8);
			this.layout.SetColumnSpan(this.categoryTableCard, 4);
			this.layout.SetColumnSpan(this.bridgeCard, 7);
			this.layout.SetColumnSpan(this.revenueCard, 5);
			this.layout.SetColumnSpan(this.countriesCard, 4);
			this.layout.SetColumnSpan(this.mixCard, 8);
			this.layout.Size = new System.Drawing.Size(1206, 1212);
			this.layout.TabIndex = 0;
			//
			// categoryTrendCard
			//
			this.categoryTrendCard.AppearanceKey = "nw-card";
			this.categoryTrendCard.Controls.Add(this.categoryTrendChart);
			this.categoryTrendCard.Controls.Add(this.categoryTrendNote);
			this.categoryTrendCard.Dock = Wisej.Web.DockStyle.Fill;
			this.categoryTrendCard.HeaderSize = 46;
			this.categoryTrendCard.Location = new System.Drawing.Point(0, 0);
			this.categoryTrendCard.Margin = new Wisej.Web.Padding(0, 0, 12, 12);
			this.categoryTrendCard.Name = "categoryTrendCard";
			this.categoryTrendCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.categoryTrendCard.ShowHeader = true;
			this.categoryTrendCard.Size = new System.Drawing.Size(792, 404);
			this.categoryTrendCard.TabIndex = 0;
			this.categoryTrendCard.Text = "Revenue by category";
			//
			// categoryTrendChart
			//
			this.categoryTrendChart.AccessibleName = "Revenue by category over time";
			this.categoryTrendChart.Dock = Wisej.Web.DockStyle.Fill;
			this.categoryTrendChart.EmptyState = "No orders in this period";
			this.categoryTrendChart.Location = new System.Drawing.Point(16, 22);
			this.categoryTrendChart.Name = "categoryTrendChart";
			this.categoryTrendChart.Size = new System.Drawing.Size(760, 324);
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
			this.categoryTrendNote.Size = new System.Drawing.Size(760, 22);
			this.categoryTrendNote.TabIndex = 0;
			this.categoryTrendNote.Text = "Stacked net revenue per day · select a legend entry to hide a category";
			//
			// categoryTableCard
			//
			this.categoryTableCard.AppearanceKey = "nw-card";
			this.categoryTableCard.Controls.Add(this.categoryGrid);
			this.categoryTableCard.Dock = Wisej.Web.DockStyle.Fill;
			this.categoryTableCard.HeaderSize = 46;
			this.categoryTableCard.Location = new System.Drawing.Point(804, 0);
			this.categoryTableCard.Margin = new Wisej.Web.Padding(0, 0, 0, 12);
			this.categoryTableCard.Name = "categoryTableCard";
			this.categoryTableCard.Padding = new Wisej.Web.Padding(4, 0, 4, 8);
			this.categoryTableCard.ShowHeader = true;
			this.categoryTableCard.Size = new System.Drawing.Size(402, 404);
			this.categoryTableCard.TabIndex = 1;
			this.categoryTableCard.Text = "Table of the same rows";
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
            this.colCategoryShare,
            this.colCategoryChange});
			this.categoryGrid.DefaultRowHeight = 38;
			this.categoryGrid.Dock = Wisej.Web.DockStyle.Fill;
			this.categoryGrid.Location = new System.Drawing.Point(4, 0);
			this.categoryGrid.MultiSelect = false;
			this.categoryGrid.Name = "categoryGrid";
			this.categoryGrid.ReadOnly = true;
			this.categoryGrid.RowHeadersVisible = false;
			this.categoryGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
			this.categoryGrid.ShowColumnVisibilityMenu = false;
			this.categoryGrid.Size = new System.Drawing.Size(394, 350);
			this.categoryGrid.TabIndex = 0;
			this.categoryGrid.CellDoubleClick += new Wisej.Web.DataGridViewCellEventHandler(this.categoryGrid_CellDoubleClick);
			//
			// colCategory
			//
			this.colCategory.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
			this.colCategory.DataPropertyName = "Name";
			this.colCategory.HeaderText = "CATEGORY";
			this.colCategory.Name = "colCategory";
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
			this.colCategoryRevenue.Width = 92;
			//
			// colCategoryShare
			//
			dataGridViewCellStyle4.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colCategoryShare.DefaultCellStyle = dataGridViewCellStyle4;
			this.colCategoryShare.DataPropertyName = "ShareText";
			dataGridViewCellStyle5.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colCategoryShare.HeaderStyle = dataGridViewCellStyle5;
			this.colCategoryShare.HeaderText = "SHARE";
			this.colCategoryShare.Name = "colCategoryShare";
			this.colCategoryShare.Width = 70;
			//
			// colCategoryChange
			//
			dataGridViewCellStyle6.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colCategoryChange.DefaultCellStyle = dataGridViewCellStyle6;
			this.colCategoryChange.DataPropertyName = "ChangeText";
			dataGridViewCellStyle7.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colCategoryChange.HeaderStyle = dataGridViewCellStyle7;
			this.colCategoryChange.HeaderText = "VS. PRIOR";
			this.colCategoryChange.Name = "colCategoryChange";
			this.colCategoryChange.Width = 86;
			//
			// bridgeCard
			//
			this.bridgeCard.AppearanceKey = "nw-card";
			this.bridgeCard.Controls.Add(this.bridgeChart);
			this.bridgeCard.Controls.Add(this.bridgeNote);
			this.bridgeCard.Dock = Wisej.Web.DockStyle.Fill;
			this.bridgeCard.HeaderSize = 46;
			this.bridgeCard.Location = new System.Drawing.Point(0, 416);
			this.bridgeCard.Margin = new Wisej.Web.Padding(0, 0, 12, 12);
			this.bridgeCard.Name = "bridgeCard";
			this.bridgeCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.bridgeCard.ShowHeader = true;
			this.bridgeCard.Size = new System.Drawing.Size(691, 348);
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
			this.bridgeChart.Size = new System.Drawing.Size(659, 268);
			this.bridgeChart.TabIndex = 1;
			this.bridgeChart.YAxis.Format = "N0";
			this.bridgeChart.PointClick += new System.EventHandler<Wisej.Web.SeriesPointEventArgs>(this.bridgeChart_PointClick);
			//
			// bridgeNote
			//
			this.bridgeNote.AppearanceKey = "nw-caption";
			this.bridgeNote.Dock = Wisej.Web.DockStyle.Top;
			this.bridgeNote.Location = new System.Drawing.Point(16, 0);
			this.bridgeNote.Name = "bridgeNote";
			this.bridgeNote.Size = new System.Drawing.Size(659, 22);
			this.bridgeNote.TabIndex = 0;
			this.bridgeNote.Text = "How each category moved revenue from the comparison period to this period";
			//
			// revenueCard
			//
			this.revenueCard.AppearanceKey = "nw-card";
			this.revenueCard.Controls.Add(this.revenueChart);
			this.revenueCard.Controls.Add(this.revenueNote);
			this.revenueCard.Dock = Wisej.Web.DockStyle.Fill;
			this.revenueCard.HeaderSize = 46;
			this.revenueCard.Location = new System.Drawing.Point(703, 416);
			this.revenueCard.Margin = new Wisej.Web.Padding(0, 0, 0, 12);
			this.revenueCard.Name = "revenueCard";
			this.revenueCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.revenueCard.ShowHeader = true;
			this.revenueCard.Size = new System.Drawing.Size(503, 348);
			this.revenueCard.TabIndex = 3;
			this.revenueCard.Text = "Revenue trend";
			//
			// revenueChart
			//
			this.revenueChart.AccessibleName = "Revenue trend";
			this.revenueChart.Dock = Wisej.Web.DockStyle.Fill;
			this.revenueChart.EmptyState = "No orders in this period";
			this.revenueChart.Location = new System.Drawing.Point(16, 22);
			this.revenueChart.Name = "revenueChart";
			chartSeries2.CategoryMember = "Label";
			chartSeries2.ChartType = Wisej.Web.ChartType.Area;
			chartSeries2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(216)))));
			chartSeries2.FillOpacity = 0.12D;
			chartSeries2.LabelFormat = "{0:C0}";
			chartSeries2.Name = "This period";
			chartSeries2.Smooth = true;
			chartSeries2.ValueMember = "Revenue";
			chartSeries3.CategoryMember = "Label";
			chartSeries3.Color = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(173)))), ((int)(((byte)(191)))));
			chartSeries3.LabelFormat = "{0:C0}";
			chartSeries3.LineStyle = Wisej.Web.ChartLineStyle.Dashed;
			chartSeries3.Markers = Wisej.Web.ChartMarkers.None;
			chartSeries3.Name = "Comparison period";
			chartSeries3.Smooth = true;
			chartSeries3.ValueMember = "PreviousRevenue";
			this.revenueChart.Series.Add(chartSeries2);
			this.revenueChart.Series.Add(chartSeries3);
			this.revenueChart.Size = new System.Drawing.Size(471, 268);
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
			this.revenueNote.Size = new System.Drawing.Size(471, 22);
			this.revenueNote.TabIndex = 0;
			this.revenueNote.Text = "Net revenue per day";
			//
			// countriesCard
			//
			this.countriesCard.AppearanceKey = "nw-card";
			this.countriesCard.Controls.Add(this.countriesChart);
			this.countriesCard.Controls.Add(this.countriesNote);
			this.countriesCard.Dock = Wisej.Web.DockStyle.Fill;
			this.countriesCard.HeaderSize = 46;
			this.countriesCard.Location = new System.Drawing.Point(0, 776);
			this.countriesCard.Margin = new Wisej.Web.Padding(0, 0, 12, 12);
			this.countriesCard.Name = "countriesCard";
			this.countriesCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.countriesCard.ShowHeader = true;
			this.countriesCard.Size = new System.Drawing.Size(390, 424);
			this.countriesCard.TabIndex = 4;
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
			chartSeries4.CategoryMember = "Country";
			chartSeries4.ChartType = Wisej.Web.ChartType.Bar;
			chartSeries4.Color = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(216)))));
			chartSeries4.LabelFormat = "{0:C0}";
			chartSeries4.Name = "Revenue";
			chartSeries4.ValueMember = "Revenue";
			this.countriesChart.Series.Add(chartSeries4);
			this.countriesChart.Size = new System.Drawing.Size(358, 344);
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
			this.countriesNote.Size = new System.Drawing.Size(358, 22);
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
			this.mixCard.Location = new System.Drawing.Point(402, 776);
			this.mixCard.Margin = new Wisej.Web.Padding(0, 0, 0, 12);
			this.mixCard.Name = "mixCard";
			this.mixCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.mixCard.ShowHeader = true;
			this.mixCard.Size = new System.Drawing.Size(804, 424);
			this.mixCard.TabIndex = 5;
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
			chartSeries5.CategoryMember = "Name";
			chartSeries5.ChartType = Wisej.Web.ChartType.Treemap;
			chartSeries5.Color = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(216)))));
			chartSeries5.ColorMember = "Change";
			chartSeries5.DownTone = Wisej.Web.ChartTone.Neutral;
			chartSeries5.LabelFormat = "{0:C0}";
			chartSeries5.Name = "Revenue";
			chartSeries5.UpTone = Wisej.Web.ChartTone.Primary;
			chartSeries5.ValueMember = "Revenue";
			this.mixChart.Series.Add(chartSeries5);
			this.mixChart.Size = new System.Drawing.Size(772, 344);
			this.mixChart.TabIndex = 1;
			this.mixChart.PointClick += new System.EventHandler<Wisej.Web.SeriesPointEventArgs>(this.mixChart_PointClick);
			//
			// mixNote
			//
			this.mixNote.AppearanceKey = "nw-caption";
			this.mixNote.Dock = Wisej.Web.DockStyle.Top;
			this.mixNote.Location = new System.Drawing.Point(16, 0);
			this.mixNote.Name = "mixNote";
			this.mixNote.Size = new System.Drawing.Size(772, 22);
			this.mixNote.TabIndex = 0;
			this.mixNote.Text = "Top 20 products sized by revenue";
			//
			// SalesView
			//
			this.AppearanceKey = "nw-view";
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = Wisej.Web.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.layout);
			this.Name = "SalesView";
			this.Size = new System.Drawing.Size(1206, 752);
			this.Text = "Sales";
			this.layout.ResumeLayout(false);
			this.categoryTrendCard.ResumeLayout(false);
			this.categoryTableCard.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.categoryGrid)).EndInit();
			this.bridgeCard.ResumeLayout(false);
			this.revenueCard.ResumeLayout(false);
			this.countriesCard.ResumeLayout(false);
			this.mixCard.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.TableLayoutPanel layout;
		private Wisej.Web.Panel categoryTrendCard;
		private Wisej.Web.ChartView categoryTrendChart;
		private Wisej.Web.Label categoryTrendNote;
		private Wisej.Web.Panel categoryTableCard;
		private Wisej.Web.DataGridView categoryGrid;
		private Wisej.Web.DataGridViewTextBoxColumn colCategory;
		private Wisej.Web.DataGridViewTextBoxColumn colCategoryRevenue;
		private Wisej.Web.DataGridViewTextBoxColumn colCategoryShare;
		private Wisej.Web.DataGridViewTextBoxColumn colCategoryChange;
		private Wisej.Web.Panel bridgeCard;
		private Wisej.Web.ChartView bridgeChart;
		private Wisej.Web.Label bridgeNote;
		private Wisej.Web.Panel revenueCard;
		private Wisej.Web.ChartView revenueChart;
		private Wisej.Web.Label revenueNote;
		private Wisej.Web.Panel countriesCard;
		private Wisej.Web.ChartView countriesChart;
		private Wisej.Web.Label countriesNote;
		private Wisej.Web.Panel mixCard;
		private Wisej.Web.ChartView mixChart;
		private Wisej.Web.Label mixNote;
	}
}
