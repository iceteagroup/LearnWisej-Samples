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
	partial class CustomersView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomersView));
			Wisej.Web.ComponentTool componentTool1 = new Wisej.Web.ComponentTool();
			Wisej.Web.ComponentTool componentTool2 = new Wisej.Web.ComponentTool();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle1 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle2 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle3 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle4 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle5 = new Wisej.Web.DataGridViewCellStyle();
			this.layout = new Wisej.Web.TableLayoutPanel();
			this.mapCard = new Wisej.Web.Panel();
			this.map = new Wisej.Web.MapView();
			this.mapNote = new Wisej.Web.Label();
			this.detailCard = new Wisej.Web.Panel();
			this.detailEmpty = new Wisej.Web.EmptyState();
			this.recentOrders = new Wisej.Web.Timeline();
			this.recentLabel = new Wisej.Web.Label();
			this.trendSparkline = new Wisej.Web.Sparkline();
			this.trendLabel = new Wisej.Web.Label();
			this.lastOrderCaption = new Wisej.Web.Label();
			this.lastOrderValue = new Wisej.Web.Label();
			this.ordersCaption = new Wisej.Web.Label();
			this.ordersValue = new Wisej.Web.Label();
			this.revenueCaption = new Wisej.Web.Label();
			this.revenueValue = new Wisej.Web.Label();
			this.placeLabel = new Wisej.Web.Label();
			this.customerAvatar = new Wisej.Web.Avatar();
			this.customersCard = new Wisej.Web.Panel();
			this.customersGrid = new Wisej.Web.DataGridView();
			this.colCompany = new Wisej.Web.DataGridViewAvatarColumn();
			this.colPlace = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colOrders = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colRevenue = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colTrend = new Wisej.Web.DataGridViewSparklineColumn();
			this.colLastOrder = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colChange = new Wisej.Web.DataGridViewChipColumn();
			this.layout.SuspendLayout();
			this.mapCard.SuspendLayout();
			this.detailCard.SuspendLayout();
			this.customersCard.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.customersGrid)).BeginInit();
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
			this.layout.Controls.Add(this.mapCard, 0, 0);
			this.layout.Controls.Add(this.detailCard, 8, 0);
			this.layout.Controls.Add(this.customersCard, 0, 1);
			this.layout.Dock = Wisej.Web.DockStyle.Top;
			this.layout.Location = new System.Drawing.Point(0, 0);
			this.layout.Name = "layout";
			this.layout.Padding = new Wisej.Web.Padding(16);
			this.layout.RowCount = 2;
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 540F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 500F));
			this.layout.SetColumnSpan(this.mapCard, 8);
			this.layout.SetColumnSpan(this.detailCard, 4);
			this.layout.SetColumnSpan(this.customersCard, 12);
			this.layout.Size = new System.Drawing.Size(1208, 1072);
			this.layout.TabIndex = 0;
			//
			// mapCard
			//
			this.mapCard.AppearanceKey = "nw-card";
			this.mapCard.Controls.Add(this.map);
			this.mapCard.Controls.Add(this.mapNote);
			this.mapCard.Dock = Wisej.Web.DockStyle.Fill;
			this.mapCard.HeaderSize = 46;
			this.mapCard.Location = new System.Drawing.Point(24, 24);
			this.mapCard.Margin = new Wisej.Web.Padding(8);
			this.mapCard.Name = "mapCard";
			this.mapCard.Padding = new Wisej.Web.Padding(16, 0, 16, 16);
			this.mapCard.ShowHeader = true;
			this.mapCard.Size = new System.Drawing.Size(768, 524);
			this.mapCard.TabIndex = 0;
			this.mapCard.Text = "Customer map";
			componentTool1.ImageSource = "Assets/Icons/pin.svg";
			componentTool1.Name = "fit";
			componentTool1.ToolTipText = "Show every customer";
			this.mapCard.Tools.AddRange(new Wisej.Web.ComponentTool[] {
            componentTool1});
			this.mapCard.ToolClick += new Wisej.Web.ToolClickEventHandler(this.mapCard_ToolClick);
			//
			// map
			//
			this.map.AccessibleName = "Customer map";
			this.map.Center = new Wisej.Web.GeoPoint(38D, -35D);
			this.map.Dock = Wisej.Web.DockStyle.Fill;
			this.map.LegendText = "Revenue quintile";
			this.map.Location = new System.Drawing.Point(16, 22);
			this.map.Name = "map";
			this.map.Size = new System.Drawing.Size(736, 440);
			this.map.TabIndex = 1;
			this.map.Zoom = 2;
			this.map.MarkerClick += new System.EventHandler<Wisej.Web.MapMarkerEventArgs>(this.map_MarkerClick);
			this.map.RegionClick += new System.EventHandler<Wisej.Web.MapRegionEventArgs>(this.map_RegionClick);
			//
			// mapNote
			//
			this.mapNote.AppearanceKey = "nw-caption";
			this.mapNote.Dock = Wisej.Web.DockStyle.Top;
			this.mapNote.Location = new System.Drawing.Point(16, 0);
			this.mapNote.Name = "mapNote";
			this.mapNote.Size = new System.Drawing.Size(736, 22);
			this.mapNote.TabIndex = 0;
			this.mapNote.Text = "Markers show customers · pulsing markers have open orders · select a country to filter the list";
			//
			// detailCard
			//
			this.detailCard.AppearanceKey = "nw-card";
			this.detailCard.Controls.Add(this.detailEmpty);
			this.detailCard.Controls.Add(this.recentOrders);
			this.detailCard.Controls.Add(this.recentLabel);
			this.detailCard.Controls.Add(this.trendSparkline);
			this.detailCard.Controls.Add(this.trendLabel);
			this.detailCard.Controls.Add(this.lastOrderCaption);
			this.detailCard.Controls.Add(this.lastOrderValue);
			this.detailCard.Controls.Add(this.ordersCaption);
			this.detailCard.Controls.Add(this.ordersValue);
			this.detailCard.Controls.Add(this.revenueCaption);
			this.detailCard.Controls.Add(this.revenueValue);
			this.detailCard.Controls.Add(this.placeLabel);
			this.detailCard.Controls.Add(this.customerAvatar);
			this.detailCard.Dock = Wisej.Web.DockStyle.Fill;
			this.detailCard.HeaderSize = 46;
			this.detailCard.Location = new System.Drawing.Point(808, 24);
			this.detailCard.Margin = new Wisej.Web.Padding(8);
			this.detailCard.Name = "detailCard";
			this.detailCard.ShowHeader = true;
			this.detailCard.Size = new System.Drawing.Size(376, 524);
			this.detailCard.TabIndex = 1;
			this.detailCard.Text = "Customer";
			componentTool2.ImageSource = "Assets/Icons/orders.svg";
			componentTool2.Name = "orders";
			componentTool2.ToolTipText = "Show this customer\'s orders";
			this.detailCard.Tools.AddRange(new Wisej.Web.ComponentTool[] {
            componentTool2});
			this.detailCard.ToolClick += new Wisej.Web.ToolClickEventHandler(this.detailCard_ToolClick);
			//
			// detailEmpty
			//
			this.detailEmpty.Description = "Choose a marker on the map or a row in the customer list.";
			this.detailEmpty.Dock = Wisej.Web.DockStyle.Fill;
			this.detailEmpty.Location = new System.Drawing.Point(0, 0);
			this.detailEmpty.Name = "detailEmpty";
			this.detailEmpty.Size = new System.Drawing.Size(374, 476);
			this.detailEmpty.TabIndex = 12;
			this.detailEmpty.Title = "No customer selected";
			//
			// recentOrders
			//
			this.recentOrders.AccessibleName = "Recent orders of the customer";
			this.recentOrders.Anchor = ((Wisej.Web.AnchorStyles)((((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom)
            | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.recentOrders.HasMoreItems = false;
			this.recentOrders.Location = new System.Drawing.Point(8, 262);
			this.recentOrders.MaxItems = 10;
			this.recentOrders.Name = "recentOrders";
			this.recentOrders.Size = new System.Drawing.Size(358, 204);
			this.recentOrders.TabIndex = 11;
			this.recentOrders.TimeFormat = "MMM d, yyyy";
			this.recentOrders.TimePosition = Wisej.Web.TimelineTimePosition.Inline;
			this.recentOrders.ItemClick += new System.EventHandler<Wisej.Web.TimelineItemEventArgs>(this.recentOrders_ItemClick);
			//
			// recentLabel
			//
			this.recentLabel.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.recentLabel.AppearanceKey = "nw-overline";
			this.recentLabel.Location = new System.Drawing.Point(16, 238);
			this.recentLabel.Name = "recentLabel";
			this.recentLabel.Size = new System.Drawing.Size(342, 20);
			this.recentLabel.TabIndex = 10;
			this.recentLabel.Text = "RECENT ORDERS";
			//
			// trendSparkline
			//
			this.trendSparkline.AccessibleName = "Revenue trend of the customer";
			this.trendSparkline.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.trendSparkline.ChartType = Wisej.Web.SparklineChartType.Area;
			this.trendSparkline.Location = new System.Drawing.Point(16, 168);
			this.trendSparkline.MaxPoints = 60;
			this.trendSparkline.Name = "trendSparkline";
			this.trendSparkline.ShowLastPoint = true;
			this.trendSparkline.Size = new System.Drawing.Size(342, 58);
			this.trendSparkline.TabIndex = 9;
			//
			// trendLabel
			//
			this.trendLabel.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.trendLabel.AppearanceKey = "nw-overline";
			this.trendLabel.Location = new System.Drawing.Point(16, 146);
			this.trendLabel.Name = "trendLabel";
			this.trendLabel.Size = new System.Drawing.Size(342, 20);
			this.trendLabel.TabIndex = 8;
			this.trendLabel.Text = "REVENUE TREND";
			//
			// lastOrderCaption
			//
			this.lastOrderCaption.AppearanceKey = "nw-caption";
			this.lastOrderCaption.Location = new System.Drawing.Point(248, 120);
			this.lastOrderCaption.Name = "lastOrderCaption";
			this.lastOrderCaption.Size = new System.Drawing.Size(110, 18);
			this.lastOrderCaption.TabIndex = 7;
			this.lastOrderCaption.Text = "Last order";
			//
			// lastOrderValue
			//
			this.lastOrderValue.AppearanceKey = "nw-strong";
			this.lastOrderValue.Location = new System.Drawing.Point(248, 96);
			this.lastOrderValue.Name = "lastOrderValue";
			this.lastOrderValue.Size = new System.Drawing.Size(110, 24);
			this.lastOrderValue.TabIndex = 6;
			this.lastOrderValue.Text = "—";
			//
			// ordersCaption
			//
			this.ordersCaption.AppearanceKey = "nw-caption";
			this.ordersCaption.Location = new System.Drawing.Point(144, 120);
			this.ordersCaption.Name = "ordersCaption";
			this.ordersCaption.Size = new System.Drawing.Size(100, 18);
			this.ordersCaption.TabIndex = 5;
			this.ordersCaption.Text = "Orders";
			//
			// ordersValue
			//
			this.ordersValue.AppearanceKey = "nw-strong";
			this.ordersValue.Location = new System.Drawing.Point(144, 96);
			this.ordersValue.Name = "ordersValue";
			this.ordersValue.Size = new System.Drawing.Size(100, 24);
			this.ordersValue.TabIndex = 4;
			this.ordersValue.Text = "0";
			//
			// revenueCaption
			//
			this.revenueCaption.AppearanceKey = "nw-caption";
			this.revenueCaption.Location = new System.Drawing.Point(16, 120);
			this.revenueCaption.Name = "revenueCaption";
			this.revenueCaption.Size = new System.Drawing.Size(124, 18);
			this.revenueCaption.TabIndex = 3;
			this.revenueCaption.Text = "Revenue";
			//
			// revenueValue
			//
			this.revenueValue.AppearanceKey = "nw-strong";
			this.revenueValue.Location = new System.Drawing.Point(16, 96);
			this.revenueValue.Name = "revenueValue";
			this.revenueValue.Size = new System.Drawing.Size(124, 24);
			this.revenueValue.TabIndex = 2;
			this.revenueValue.Text = "$0";
			//
			// placeLabel
			//
			this.placeLabel.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.placeLabel.AppearanceKey = "nw-subtitle";
			this.placeLabel.AutoEllipsis = true;
			this.placeLabel.Location = new System.Drawing.Point(16, 58);
			this.placeLabel.Name = "placeLabel";
			this.placeLabel.Size = new System.Drawing.Size(342, 32);
			this.placeLabel.TabIndex = 1;
			this.placeLabel.Text = "Address";
			//
			// customerAvatar
			//
			this.customerAvatar.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.customerAvatar.AvatarSize = 44;
			this.customerAvatar.Label = "Customer";
			this.customerAvatar.Location = new System.Drawing.Point(16, 4);
			this.customerAvatar.Name = "customerAvatar";
			this.customerAvatar.Shape = Wisej.Web.AvatarShape.Rounded;
			this.customerAvatar.Size = new System.Drawing.Size(342, 50);
			this.customerAvatar.SubLabel = "Contact";
			this.customerAvatar.TabIndex = 0;
			this.customerAvatar.Text = "Customer";
			//
			// customersCard
			//
			this.customersCard.AppearanceKey = "nw-card";
			this.customersCard.Controls.Add(this.customersGrid);
			this.customersCard.Dock = Wisej.Web.DockStyle.Fill;
			this.customersCard.HeaderSize = 46;
			this.customersCard.Location = new System.Drawing.Point(24, 564);
			this.customersCard.Margin = new Wisej.Web.Padding(8);
			this.customersCard.Name = "customersCard";
			this.customersCard.Padding = new Wisej.Web.Padding(4, 0, 4, 4);
			this.customersCard.ShowHeader = true;
			this.customersCard.Size = new System.Drawing.Size(1160, 484);
			this.customersCard.TabIndex = 2;
			this.customersCard.Text = "Customers";
			this.customersCard.ToolClick += new Wisej.Web.ToolClickEventHandler(this.customersCard_ToolClick);
			//
			// customersGrid
			//
			this.customersGrid.AllowUserToResizeRows = false;
			this.customersGrid.AutoGenerateColumns = false;
			this.customersGrid.BorderStyle = Wisej.Web.BorderStyle.None;
			this.customersGrid.CellBorderStyle = Wisej.Web.DataGridViewCellBorderStyle.Horizontal;
			dataGridViewCellStyle1.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
			this.customersGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.customersGrid.ColumnHeadersHeight = 38;
			this.customersGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colCompany,
            this.colPlace,
            this.colOrders,
            this.colRevenue,
            this.colTrend,
            this.colLastOrder,
            this.colChange});
			this.customersGrid.DefaultRowHeight = 52;
			this.customersGrid.Dock = Wisej.Web.DockStyle.Fill;
			this.customersGrid.Location = new System.Drawing.Point(4, 0);
			this.customersGrid.MultiSelect = false;
			this.customersGrid.Name = "customersGrid";
			this.customersGrid.ReadOnly = true;
			this.customersGrid.RowHeadersVisible = false;
			this.customersGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
			this.customersGrid.ShowColumnVisibilityMenu = false;
			this.customersGrid.Size = new System.Drawing.Size(1152, 434);
			this.customersGrid.TabIndex = 0;
			this.customersGrid.CellClick += new Wisej.Web.DataGridViewCellEventHandler(this.customersGrid_CellClick);
			//
			// colCompany
			//
			this.colCompany.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
			this.colCompany.DataPropertyName = "Company";
			this.colCompany.HeaderText = "CUSTOMER";
			this.colCompany.MinimumWidth = 160;
			this.colCompany.Name = "colCompany";
			this.colCompany.Shape = Wisej.Web.AvatarShape.Rounded;
			this.colCompany.Size = 32;
			this.colCompany.SubTextMember = "ContactText";
			//
			// colPlace
			//
			this.colPlace.DataPropertyName = "Place";
			this.colPlace.HeaderText = "LOCATION";
			this.colPlace.Name = "colPlace";
			this.colPlace.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colPlace.ResponsiveProfiles"))));
			this.colPlace.Width = 200;
			//
			// colOrders
			//
			dataGridViewCellStyle2.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colOrders.DefaultCellStyle = dataGridViewCellStyle2;
			this.colOrders.DataPropertyName = "Orders";
			dataGridViewCellStyle3.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colOrders.HeaderStyle = dataGridViewCellStyle3;
			this.colOrders.HeaderText = "ORDERS";
			this.colOrders.Name = "colOrders";
			this.colOrders.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colOrders.ResponsiveProfiles"))));
			this.colOrders.Width = 90;
			//
			// colRevenue
			//
			dataGridViewCellStyle4.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle4.Format = "C0";
			this.colRevenue.DefaultCellStyle = dataGridViewCellStyle4;
			this.colRevenue.DataPropertyName = "Revenue";
			dataGridViewCellStyle5.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colRevenue.HeaderStyle = dataGridViewCellStyle5;
			this.colRevenue.HeaderText = "REVENUE";
			this.colRevenue.Name = "colRevenue";
			this.colRevenue.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colRevenue.ResponsiveProfiles"))));
			this.colRevenue.Width = 120;
			//
			// colTrend
			//
			this.colTrend.ChartType = Wisej.Web.SparklineChartType.Area;
			this.colTrend.DataPropertyName = "Trend";
			this.colTrend.HeaderText = "TREND";
			this.colTrend.Name = "colTrend";
			this.colTrend.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colTrend.ResponsiveProfiles"))));
			this.colTrend.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colTrend.ResponsiveProfiles1"))));
			this.colTrend.Width = 150;
			//
			// colLastOrder
			//
			this.colLastOrder.DataPropertyName = "LastOrderText";
			this.colLastOrder.HeaderText = "LAST ORDER";
			this.colLastOrder.Name = "colLastOrder";
			this.colLastOrder.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colLastOrder.ResponsiveProfiles"))));
			this.colLastOrder.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colLastOrder.ResponsiveProfiles1"))));
			this.colLastOrder.Width = 130;
			//
			// colChange
			//
			this.colChange.DataPropertyName = "ChangeText";
			this.colChange.HeaderText = "VS. PRIOR";
			this.colChange.Name = "colChange";
			this.colChange.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colChange.ResponsiveProfiles"))));
			this.colChange.ToneMember = "ChangeTone";
			this.colChange.Width = 110;
			//
			// CustomersView
			//
			this.AppearanceKey = "nw-view";
			this.AutoScroll = true;
			this.Controls.Add(this.layout);
			this.Name = "CustomersView";
			this.Size = new System.Drawing.Size(1208, 828);
			this.Text = "Customers";
			this.layout.ResumeLayout(false);
			this.mapCard.ResumeLayout(false);
			this.detailCard.ResumeLayout(false);
			this.customersCard.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.customersGrid)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.TableLayoutPanel layout;
		private Wisej.Web.Panel mapCard;
		private Wisej.Web.MapView map;
		private Wisej.Web.Label mapNote;
		private Wisej.Web.Panel detailCard;
		private Wisej.Web.Avatar customerAvatar;
		private Wisej.Web.Label placeLabel;
		private Wisej.Web.Label revenueValue;
		private Wisej.Web.Label revenueCaption;
		private Wisej.Web.Label ordersValue;
		private Wisej.Web.Label ordersCaption;
		private Wisej.Web.Label lastOrderValue;
		private Wisej.Web.Label lastOrderCaption;
		private Wisej.Web.Label trendLabel;
		private Wisej.Web.Sparkline trendSparkline;
		private Wisej.Web.Label recentLabel;
		private Wisej.Web.Timeline recentOrders;
		private Wisej.Web.EmptyState detailEmpty;
		private Wisej.Web.Panel customersCard;
		private Wisej.Web.DataGridView customersGrid;
		private Wisej.Web.DataGridViewAvatarColumn colCompany;
		private Wisej.Web.DataGridViewTextBoxColumn colPlace;
		private Wisej.Web.DataGridViewTextBoxColumn colOrders;
		private Wisej.Web.DataGridViewTextBoxColumn colRevenue;
		private Wisej.Web.DataGridViewSparklineColumn colTrend;
		private Wisej.Web.DataGridViewTextBoxColumn colLastOrder;
		private Wisej.Web.DataGridViewChipColumn colChange;
	}
}
