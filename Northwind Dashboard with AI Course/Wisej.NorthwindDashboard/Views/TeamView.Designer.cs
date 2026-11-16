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
	partial class TeamView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TeamView));
			Wisej.Web.ChartSeries chartSeries1 = new Wisej.Web.ChartSeries();
			Wisej.Web.ChartSeries chartSeries2 = new Wisej.Web.ChartSeries();
			Wisej.Web.ComponentTool componentTool1 = new Wisej.Web.ComponentTool();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle1 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle2 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle3 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle4 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle5 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle6 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle7 = new Wisej.Web.DataGridViewCellStyle();
			this.layout = new Wisej.Web.TableLayoutPanel();
			this.orgCard = new Wisej.Web.Panel();
			this.orgChart = new Wisej.Web.OrgChart();
			this.orgNote = new Wisej.Web.Label();
			this.profileCard = new Wisej.Web.Panel();
			this.customersGroup = new Wisej.Web.AvatarGroup();
			this.customersLabel = new Wisej.Web.Label();
			this.radarChart = new Wisej.Web.ChartView();
			this.territoriesLabel = new Wisej.Web.Label();
			this.onTimeCaption = new Wisej.Web.Label();
			this.onTimeValue = new Wisej.Web.Label();
			this.averageCaption = new Wisej.Web.Label();
			this.averageValue = new Wisej.Web.Label();
			this.ordersCaption = new Wisej.Web.Label();
			this.ordersValue = new Wisej.Web.Label();
			this.revenueCaption = new Wisej.Web.Label();
			this.revenueValue = new Wisej.Web.Label();
			this.profileAvatar = new Wisej.Web.Avatar();
			this.leaderboardCard = new Wisej.Web.Panel();
			this.leaderboardGrid = new Wisej.Web.DataGridView();
			this.colEmployee = new Wisej.Web.DataGridViewAvatarColumn();
			this.colRevenue = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colOrders = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colAverage = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colShare = new Wisej.Web.DataGridViewMeterColumn();
			this.colTrend = new Wisej.Web.DataGridViewSparklineColumn();
			this.colChange = new Wisej.Web.DataGridViewChipColumn();
			this.layout.SuspendLayout();
			this.orgCard.SuspendLayout();
			this.profileCard.SuspendLayout();
			this.leaderboardCard.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.leaderboardGrid)).BeginInit();
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
			this.layout.Controls.Add(this.orgCard, 0, 0);
			this.layout.Controls.Add(this.profileCard, 0, 1);
			this.layout.Controls.Add(this.leaderboardCard, 4, 1);
			this.layout.Dock = Wisej.Web.DockStyle.Top;
			this.layout.Location = new System.Drawing.Point(0, 0);
			this.layout.Name = "layout";
			this.layout.Padding = new Wisej.Web.Padding(16);
			this.layout.RowCount = 2;
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 400F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 560F));
			this.layout.SetColumnSpan(this.orgCard, 12);
			this.layout.SetColumnSpan(this.profileCard, 4);
			this.layout.SetColumnSpan(this.leaderboardCard, 8);
			this.layout.Size = new System.Drawing.Size(1208, 992);
			this.layout.TabIndex = 0;
			//
			// orgCard
			//
			this.orgCard.AppearanceKey = "nw-card";
			this.orgCard.Controls.Add(this.orgChart);
			this.orgCard.Controls.Add(this.orgNote);
			this.orgCard.Dock = Wisej.Web.DockStyle.Fill;
			this.orgCard.HeaderSize = 46;
			this.orgCard.Location = new System.Drawing.Point(24, 24);
			this.orgCard.Margin = new Wisej.Web.Padding(8);
			this.orgCard.Name = "orgCard";
			this.orgCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.orgCard.ShowHeader = true;
			this.orgCard.Size = new System.Drawing.Size(1160, 384);
			this.orgCard.TabIndex = 0;
			this.orgCard.Text = "Sales organization";
			//
			// orgChart
			//
			this.orgChart.AccessibleName = "Sales organization";
			this.orgChart.CollapseBelow = 1;
			this.orgChart.Dock = Wisej.Web.DockStyle.Fill;
			this.orgChart.Location = new System.Drawing.Point(16, 22);
			this.orgChart.Name = "orgChart";
			this.orgChart.Size = new System.Drawing.Size(1128, 304);
			this.orgChart.TabIndex = 1;
			this.orgChart.NodeClick += new System.EventHandler<Wisej.Web.OrgNodeEventArgs>(this.orgChart_NodeClick);
			//
			// orgNote
			//
			this.orgNote.AppearanceKey = "nw-caption";
			this.orgNote.Dock = Wisej.Web.DockStyle.Top;
			this.orgNote.Location = new System.Drawing.Point(16, 0);
			this.orgNote.Name = "orgNote";
			this.orgNote.Size = new System.Drawing.Size(1128, 22);
			this.orgNote.TabIndex = 0;
			this.orgNote.Text = "Select a person to see their performance · green: revenue above the team average, amber: below";
			//
			// profileCard
			//
			this.profileCard.AppearanceKey = "nw-card";
			this.profileCard.Controls.Add(this.customersGroup);
			this.profileCard.Controls.Add(this.customersLabel);
			this.profileCard.Controls.Add(this.radarChart);
			this.profileCard.Controls.Add(this.territoriesLabel);
			this.profileCard.Controls.Add(this.onTimeCaption);
			this.profileCard.Controls.Add(this.onTimeValue);
			this.profileCard.Controls.Add(this.averageCaption);
			this.profileCard.Controls.Add(this.averageValue);
			this.profileCard.Controls.Add(this.ordersCaption);
			this.profileCard.Controls.Add(this.ordersValue);
			this.profileCard.Controls.Add(this.revenueCaption);
			this.profileCard.Controls.Add(this.revenueValue);
			this.profileCard.Controls.Add(this.profileAvatar);
			this.profileCard.Dock = Wisej.Web.DockStyle.Fill;
			this.profileCard.HeaderSize = 46;
			this.profileCard.Location = new System.Drawing.Point(24, 424);
			this.profileCard.Margin = new Wisej.Web.Padding(8);
			this.profileCard.Name = "profileCard";
			this.profileCard.ShowHeader = true;
			this.profileCard.Size = new System.Drawing.Size(376, 544);
			this.profileCard.TabIndex = 1;
			this.profileCard.Text = "Profile";
			componentTool1.ImageSource = "Assets/Icons/orders.svg";
			componentTool1.Name = "orders";
			componentTool1.ToolTipText = "Show the orders booked by this person";
			this.profileCard.Tools.AddRange(new Wisej.Web.ComponentTool[] {
            componentTool1});
			this.profileCard.ToolClick += new Wisej.Web.ToolClickEventHandler(this.profileCard_ToolClick);
			//
			// customersGroup
			//
			this.customersGroup.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.customersGroup.AvatarSize = 36;
			this.customersGroup.Location = new System.Drawing.Point(16, 446);
			this.customersGroup.Name = "customersGroup";
			this.customersGroup.Size = new System.Drawing.Size(342, 44);
			this.customersGroup.TabIndex = 12;
			this.customersGroup.ItemClick += new System.EventHandler<Wisej.Web.AvatarItemEventArgs>(this.customersGroup_ItemClick);
			this.customersGroup.OverflowClick += new System.EventHandler(this.customersGroup_OverflowClick);
			//
			// customersLabel
			//
			this.customersLabel.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.customersLabel.AppearanceKey = "nw-overline";
			this.customersLabel.Location = new System.Drawing.Point(16, 422);
			this.customersLabel.Name = "customersLabel";
			this.customersLabel.Size = new System.Drawing.Size(342, 20);
			this.customersLabel.TabIndex = 11;
			this.customersLabel.Text = "TOP CUSTOMERS";
			//
			// radarChart
			//
			this.radarChart.AccessibleName = "Performance compared with the team average";
			this.radarChart.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.radarChart.Location = new System.Drawing.Point(8, 176);
			this.radarChart.Name = "radarChart";
			chartSeries1.CategoryMember = "Axis";
			chartSeries1.ChartType = Wisej.Web.ChartType.Radar;
			chartSeries1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
			chartSeries1.LabelFormat = "{0:N0}";
			chartSeries1.Name = "Selected";
			chartSeries1.ValueMember = "Person";
			chartSeries2.CategoryMember = "Axis";
			chartSeries2.ChartType = Wisej.Web.ChartType.Radar;
			chartSeries2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(173)))), ((int)(((byte)(191)))));
			chartSeries2.Fill = false;
			chartSeries2.LabelFormat = "{0:N0}";
			chartSeries2.LineStyle = Wisej.Web.ChartLineStyle.Dashed;
			chartSeries2.Name = "Team average";
			chartSeries2.ValueMember = "Team";
			this.radarChart.Series.Add(chartSeries1);
			this.radarChart.Series.Add(chartSeries2);
			this.radarChart.Size = new System.Drawing.Size(358, 238);
			this.radarChart.TabIndex = 10;
			this.radarChart.YAxis.Max = 100D;
			this.radarChart.YAxis.Min = 0D;
			//
			// territoriesLabel
			//
			this.territoriesLabel.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.territoriesLabel.AppearanceKey = "nw-caption";
			this.territoriesLabel.AutoEllipsis = true;
			this.territoriesLabel.Location = new System.Drawing.Point(16, 140);
			this.territoriesLabel.Name = "territoriesLabel";
			this.territoriesLabel.Size = new System.Drawing.Size(342, 32);
			this.territoriesLabel.TabIndex = 9;
			this.territoriesLabel.Text = "Territories";
			//
			// onTimeCaption
			//
			this.onTimeCaption.AppearanceKey = "nw-caption";
			this.onTimeCaption.Location = new System.Drawing.Point(274, 112);
			this.onTimeCaption.Name = "onTimeCaption";
			this.onTimeCaption.Size = new System.Drawing.Size(84, 18);
			this.onTimeCaption.TabIndex = 8;
			this.onTimeCaption.Text = "On time";
			//
			// onTimeValue
			//
			this.onTimeValue.AppearanceKey = "nw-strong";
			this.onTimeValue.Location = new System.Drawing.Point(274, 88);
			this.onTimeValue.Name = "onTimeValue";
			this.onTimeValue.Size = new System.Drawing.Size(84, 24);
			this.onTimeValue.TabIndex = 7;
			this.onTimeValue.Text = "0%";
			//
			// averageCaption
			//
			this.averageCaption.AppearanceKey = "nw-caption";
			this.averageCaption.Location = new System.Drawing.Point(186, 112);
			this.averageCaption.Name = "averageCaption";
			this.averageCaption.Size = new System.Drawing.Size(84, 18);
			this.averageCaption.TabIndex = 6;
			this.averageCaption.Text = "Avg. order";
			//
			// averageValue
			//
			this.averageValue.AppearanceKey = "nw-strong";
			this.averageValue.Location = new System.Drawing.Point(186, 88);
			this.averageValue.Name = "averageValue";
			this.averageValue.Size = new System.Drawing.Size(84, 24);
			this.averageValue.TabIndex = 5;
			this.averageValue.Text = "$0";
			//
			// ordersCaption
			//
			this.ordersCaption.AppearanceKey = "nw-caption";
			this.ordersCaption.Location = new System.Drawing.Point(116, 112);
			this.ordersCaption.Name = "ordersCaption";
			this.ordersCaption.Size = new System.Drawing.Size(66, 18);
			this.ordersCaption.TabIndex = 4;
			this.ordersCaption.Text = "Orders";
			//
			// ordersValue
			//
			this.ordersValue.AppearanceKey = "nw-strong";
			this.ordersValue.Location = new System.Drawing.Point(116, 88);
			this.ordersValue.Name = "ordersValue";
			this.ordersValue.Size = new System.Drawing.Size(66, 24);
			this.ordersValue.TabIndex = 3;
			this.ordersValue.Text = "0";
			//
			// revenueCaption
			//
			this.revenueCaption.AppearanceKey = "nw-caption";
			this.revenueCaption.Location = new System.Drawing.Point(16, 112);
			this.revenueCaption.Name = "revenueCaption";
			this.revenueCaption.Size = new System.Drawing.Size(96, 18);
			this.revenueCaption.TabIndex = 2;
			this.revenueCaption.Text = "Revenue";
			//
			// revenueValue
			//
			this.revenueValue.AppearanceKey = "nw-strong";
			this.revenueValue.Location = new System.Drawing.Point(16, 88);
			this.revenueValue.Name = "revenueValue";
			this.revenueValue.Size = new System.Drawing.Size(96, 24);
			this.revenueValue.TabIndex = 1;
			this.revenueValue.Text = "$0";
			//
			// profileAvatar
			//
			this.profileAvatar.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.profileAvatar.AvatarSize = 56;
			this.profileAvatar.Label = "Employee";
			this.profileAvatar.Location = new System.Drawing.Point(16, 8);
			this.profileAvatar.Name = "profileAvatar";
			this.profileAvatar.Size = new System.Drawing.Size(342, 64);
			this.profileAvatar.SubLabel = "Title";
			this.profileAvatar.TabIndex = 0;
			this.profileAvatar.Text = "Employee";
			//
			// leaderboardCard
			//
			this.leaderboardCard.AppearanceKey = "nw-card";
			this.leaderboardCard.Controls.Add(this.leaderboardGrid);
			this.leaderboardCard.Dock = Wisej.Web.DockStyle.Fill;
			this.leaderboardCard.HeaderSize = 46;
			this.leaderboardCard.Location = new System.Drawing.Point(416, 424);
			this.leaderboardCard.Margin = new Wisej.Web.Padding(8);
			this.leaderboardCard.Name = "leaderboardCard";
			this.leaderboardCard.Padding = new Wisej.Web.Padding(4, 0, 4, 4);
			this.leaderboardCard.ShowHeader = true;
			this.leaderboardCard.Size = new System.Drawing.Size(768, 544);
			this.leaderboardCard.TabIndex = 2;
			this.leaderboardCard.Text = "Leaderboard";
			//
			// leaderboardGrid
			//
			this.leaderboardGrid.AllowUserToResizeRows = false;
			this.leaderboardGrid.AutoGenerateColumns = false;
			this.leaderboardGrid.BorderStyle = Wisej.Web.BorderStyle.None;
			this.leaderboardGrid.CellBorderStyle = Wisej.Web.DataGridViewCellBorderStyle.Horizontal;
			dataGridViewCellStyle1.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
			this.leaderboardGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.leaderboardGrid.ColumnHeadersHeight = 38;
			this.leaderboardGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colEmployee,
            this.colRevenue,
            this.colOrders,
            this.colAverage,
            this.colShare,
            this.colTrend,
            this.colChange});
			this.leaderboardGrid.DefaultRowHeight = 52;
			this.leaderboardGrid.Dock = Wisej.Web.DockStyle.Fill;
			this.leaderboardGrid.Location = new System.Drawing.Point(4, 0);
			this.leaderboardGrid.MultiSelect = false;
			this.leaderboardGrid.Name = "leaderboardGrid";
			this.leaderboardGrid.ReadOnly = true;
			this.leaderboardGrid.RowHeadersVisible = false;
			this.leaderboardGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
			this.leaderboardGrid.ShowColumnVisibilityMenu = false;
			this.leaderboardGrid.Size = new System.Drawing.Size(760, 494);
			this.leaderboardGrid.TabIndex = 0;
			this.leaderboardGrid.CellClick += new Wisej.Web.DataGridViewCellEventHandler(this.leaderboardGrid_CellClick);
			//
			// colEmployee
			//
			this.colEmployee.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
			this.colEmployee.DataPropertyName = "Name";
			this.colEmployee.HeaderText = "EMPLOYEE";
			this.colEmployee.MinimumWidth = 150;
			this.colEmployee.Name = "colEmployee";
			this.colEmployee.Size = 32;
			this.colEmployee.SubTextMember = "TitleText";
			//
			// colRevenue
			//
			dataGridViewCellStyle2.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle2.Format = "C0";
			this.colRevenue.DefaultCellStyle = dataGridViewCellStyle2;
			this.colRevenue.DataPropertyName = "Revenue";
			dataGridViewCellStyle3.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colRevenue.HeaderStyle = dataGridViewCellStyle3;
			this.colRevenue.HeaderText = "REVENUE";
			this.colRevenue.Name = "colRevenue";
			this.colRevenue.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colRevenue.ResponsiveProfiles"))));
			this.colRevenue.Width = 96;
			//
			// colOrders
			//
			dataGridViewCellStyle4.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colOrders.DefaultCellStyle = dataGridViewCellStyle4;
			this.colOrders.DataPropertyName = "Orders";
			dataGridViewCellStyle5.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colOrders.HeaderStyle = dataGridViewCellStyle5;
			this.colOrders.HeaderText = "ORDERS";
			this.colOrders.Name = "colOrders";
			this.colOrders.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colOrders.ResponsiveProfiles"))));
			this.colOrders.Width = 70;
			//
			// colAverage
			//
			dataGridViewCellStyle6.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle6.Format = "C0";
			this.colAverage.DefaultCellStyle = dataGridViewCellStyle6;
			this.colAverage.DataPropertyName = "AverageOrder";
			dataGridViewCellStyle7.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colAverage.HeaderStyle = dataGridViewCellStyle7;
			this.colAverage.HeaderText = "AVG. ORDER";
			this.colAverage.Name = "colAverage";
			this.colAverage.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colAverage.ResponsiveProfiles"))));
			this.colAverage.Width = 90;
			//
			// colShare
			//
			this.colShare.DataPropertyName = "Share";
			this.colShare.HeaderText = "SHARE";
			this.colShare.Name = "colShare";
			this.colShare.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colShare.ResponsiveProfiles"))));
			this.colShare.TextMember = "ShareText";
			this.colShare.Width = 100;
			//
			// colTrend
			//
			this.colTrend.DataPropertyName = "Trend";
			this.colTrend.HeaderText = "TREND";
			this.colTrend.Name = "colTrend";
			this.colTrend.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colTrend.ResponsiveProfiles"))));
			this.colTrend.Width = 96;
			//
			// colChange
			//
			this.colChange.DataPropertyName = "ChangeText";
			this.colChange.HeaderText = "VS. PRIOR";
			this.colChange.Name = "colChange";
			this.colChange.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colChange.ResponsiveProfiles"))));
			this.colChange.ToneMember = "ChangeTone";
			this.colChange.Width = 96;
			//
			// TeamView
			//
			this.AppearanceKey = "nw-view";
			this.AutoScroll = true;
			this.Controls.Add(this.layout);
			this.Name = "TeamView";
			this.Size = new System.Drawing.Size(1208, 828);
			this.Text = "Team";
			this.layout.ResumeLayout(false);
			this.orgCard.ResumeLayout(false);
			this.profileCard.ResumeLayout(false);
			this.leaderboardCard.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.leaderboardGrid)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.TableLayoutPanel layout;
		private Wisej.Web.Panel orgCard;
		private Wisej.Web.OrgChart orgChart;
		private Wisej.Web.Label orgNote;
		private Wisej.Web.Panel profileCard;
		private Wisej.Web.Avatar profileAvatar;
		private Wisej.Web.Label revenueValue;
		private Wisej.Web.Label revenueCaption;
		private Wisej.Web.Label ordersValue;
		private Wisej.Web.Label ordersCaption;
		private Wisej.Web.Label averageValue;
		private Wisej.Web.Label averageCaption;
		private Wisej.Web.Label onTimeValue;
		private Wisej.Web.Label onTimeCaption;
		private Wisej.Web.Label territoriesLabel;
		private Wisej.Web.ChartView radarChart;
		private Wisej.Web.Label customersLabel;
		private Wisej.Web.AvatarGroup customersGroup;
		private Wisej.Web.Panel leaderboardCard;
		private Wisej.Web.DataGridView leaderboardGrid;
		private Wisej.Web.DataGridViewAvatarColumn colEmployee;
		private Wisej.Web.DataGridViewTextBoxColumn colRevenue;
		private Wisej.Web.DataGridViewTextBoxColumn colOrders;
		private Wisej.Web.DataGridViewTextBoxColumn colAverage;
		private Wisej.Web.DataGridViewMeterColumn colShare;
		private Wisej.Web.DataGridViewSparklineColumn colTrend;
		private Wisej.Web.DataGridViewChipColumn colChange;
	}
}
