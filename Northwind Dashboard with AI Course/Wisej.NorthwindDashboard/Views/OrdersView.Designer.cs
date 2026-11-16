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
	partial class OrdersView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrdersView));
			Wisej.Web.Segment segment1 = new Wisej.Web.Segment();
			Wisej.Web.Segment segment2 = new Wisej.Web.Segment();
			Wisej.Web.Segment segment3 = new Wisej.Web.Segment();
			Wisej.Web.Segment segment4 = new Wisej.Web.Segment();
			Wisej.Web.Segment segment5 = new Wisej.Web.Segment();
			Wisej.Web.ComponentTool componentTool1 = new Wisej.Web.ComponentTool();
			Wisej.Web.ComponentTool componentTool2 = new Wisej.Web.ComponentTool();
			Wisej.Web.ComponentTool componentTool3 = new Wisej.Web.ComponentTool();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle1 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle2 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle3 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle4 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle5 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.CardAction cardAction1 = new Wisej.Web.CardAction();
			Wisej.Web.CardAction cardAction2 = new Wisej.Web.CardAction();
			this.layout = new Wisej.Web.TableLayoutPanel();
			this.toolbar = new Wisej.Web.Panel();
			this.advancedButton = new Wisej.Web.Button();
			this.searchBox = new Wisej.Web.TextBox();
			this.statusFilter = new Wisej.Web.ChipGroup();
			this.queryCard = new Wisej.Web.Panel();
			this.queryBuilder = new Wisej.Web.QueryBuilder();
			this.gridCard = new Wisej.Web.Panel();
			this.ordersGrid = new Wisej.Web.DataGridView();
			this.colOrder = new Wisej.Web.DataGridViewSubtitleColumn();
			this.colCustomer = new Wisej.Web.DataGridViewAvatarColumn();
			this.colSalesRep = new Wisej.Web.DataGridViewAvatarColumn();
			this.colStatus = new Wisej.Web.DataGridViewChipColumn();
			this.colDelivery = new Wisej.Web.DataGridViewSubtitleColumn();
			this.colItems = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colAmount = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colActions = new Wisej.Web.DataGridViewActionsColumn();
			this.summaryLabel = new Wisej.Web.Label();
			this.layout.SuspendLayout();
			this.toolbar.SuspendLayout();
			this.queryCard.SuspendLayout();
			this.gridCard.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ordersGrid)).BeginInit();
			this.SuspendLayout();
			//
			// layout
			//
			this.layout.ColumnCount = 1;
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 100F));
			this.layout.Controls.Add(this.toolbar, 0, 0);
			this.layout.Controls.Add(this.queryCard, 0, 1);
			this.layout.Controls.Add(this.gridCard, 0, 2);
			this.layout.Dock = Wisej.Web.DockStyle.Fill;
			this.layout.Location = new System.Drawing.Point(0, 0);
			this.layout.Name = "layout";
			this.layout.Padding = new Wisej.Web.Padding(16, 8, 16, 16);
			this.layout.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("layout.ResponsiveProfiles"))));
			this.layout.RowCount = 3;
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 60F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 0F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
			this.layout.Size = new System.Drawing.Size(1208, 828);
			this.layout.TabIndex = 0;
			//
			// toolbar
			//
			this.toolbar.AppearanceKey = "nw-view";
			this.toolbar.Controls.Add(this.searchBox);
			this.toolbar.Controls.Add(this.advancedButton);
			this.toolbar.Controls.Add(this.statusFilter);
			this.toolbar.Dock = Wisej.Web.DockStyle.Fill;
			this.toolbar.Location = new System.Drawing.Point(24, 16);
			this.toolbar.Margin = new Wisej.Web.Padding(8);
			this.toolbar.Name = "toolbar";
			this.toolbar.Size = new System.Drawing.Size(1160, 44);
			this.toolbar.TabIndex = 0;
			//
			// advancedButton
			//
			this.advancedButton.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
			this.advancedButton.AppearanceKey = "nw-button";
			this.advancedButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.advancedButton.ImageSource = "Assets/Icons/filter.svg";
			this.advancedButton.Location = new System.Drawing.Point(990, 4);
			this.advancedButton.Name = "advancedButton";
			this.advancedButton.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("advancedButton.ResponsiveProfiles"))));
			this.advancedButton.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("advancedButton.ResponsiveProfiles1"))));
			this.advancedButton.Size = new System.Drawing.Size(170, 36);
			this.advancedButton.TabIndex = 2;
			this.advancedButton.Text = "Advanced filter";
			this.advancedButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.advancedButton.Click += new System.EventHandler(this.advancedButton_Click);
			//
			// searchBox
			//
			this.searchBox.AccessibleName = "Search orders";
			this.searchBox.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
			this.searchBox.Location = new System.Drawing.Point(706, 4);
			this.searchBox.Name = "searchBox";
			this.searchBox.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("searchBox.ResponsiveProfiles"))));
			this.searchBox.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("searchBox.ResponsiveProfiles1"))));
			this.searchBox.Size = new System.Drawing.Size(272, 36);
			this.searchBox.TabIndex = 1;
			componentTool1.ImageSource = "Assets/Icons/search.svg";
			componentTool1.Name = "search";
			componentTool1.Position = Wisej.Web.LeftRightAlignment.Left;
			this.searchBox.Tools.AddRange(new Wisej.Web.ComponentTool[] {
            componentTool1});
			this.searchBox.Watermark = "Order, customer, rep, product…";
			this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
			//
			// statusFilter
			//
			this.statusFilter.AccessibleName = "Order status";
			segment1.Text = "All";
			segment1.Value = "All";
			segment2.Text = "Open";
			segment2.Value = "Open";
			segment3.Text = "Overdue";
			segment3.Value = "Overdue";
			segment4.Text = "Shipped";
			segment4.Value = "Shipped";
			segment5.Text = "Shipped late";
			segment5.Value = "ShippedLate";
			this.statusFilter.Items.Add(segment1);
			this.statusFilter.Items.Add(segment2);
			this.statusFilter.Items.Add(segment3);
			this.statusFilter.Items.Add(segment4);
			this.statusFilter.Items.Add(segment5);
			this.statusFilter.Location = new System.Drawing.Point(0, 4);
			this.statusFilter.Name = "statusFilter";
			this.statusFilter.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("statusFilter.ResponsiveProfiles"))));
			this.statusFilter.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("statusFilter.ResponsiveProfiles1"))));
			this.statusFilter.ShowCounts = true;
			this.statusFilter.Size = new System.Drawing.Size(640, 38);
			this.statusFilter.TabIndex = 0;
			this.statusFilter.SelectionChanged += new System.EventHandler(this.statusFilter_SelectionChanged);
			//
			// queryCard
			//
			this.queryCard.AppearanceKey = "nw-card";
			this.queryCard.Controls.Add(this.queryBuilder);
			this.queryCard.Dock = Wisej.Web.DockStyle.Fill;
			this.queryCard.HeaderSize = 46;
			this.queryCard.Location = new System.Drawing.Point(24, 76);
			this.queryCard.Margin = new Wisej.Web.Padding(8);
			this.queryCard.Name = "queryCard";
			this.queryCard.Padding = new Wisej.Web.Padding(16, 0, 16, 12);
			this.queryCard.ShowHeader = true;
			this.queryCard.Size = new System.Drawing.Size(1160, 0);
			this.queryCard.TabIndex = 1;
			this.queryCard.Text = "Advanced filter";
			componentTool2.ImageSource = "Assets/Icons/refresh.svg";
			componentTool2.Name = "reset";
			componentTool2.ToolTipText = "Remove all rules";
			componentTool3.ImageSource = "Assets/Icons/close.svg";
			componentTool3.Name = "close";
			componentTool3.ToolTipText = "Hide the filter (rules stay applied)";
			this.queryCard.Tools.AddRange(new Wisej.Web.ComponentTool[] {
            componentTool2,
            componentTool3});
			this.queryCard.Visible = false;
			this.queryCard.ToolClick += new Wisej.Web.ToolClickEventHandler(this.queryCard_ToolClick);
			//
			// queryBuilder
			//
			this.queryBuilder.AccessibleName = "Advanced order filter";
			this.queryBuilder.Dock = Wisej.Web.DockStyle.Fill;
			this.queryBuilder.Location = new System.Drawing.Point(16, 0);
			this.queryBuilder.MaxDepth = 2;
			this.queryBuilder.Name = "queryBuilder";
			this.queryBuilder.Size = new System.Drawing.Size(1128, 0);
			this.queryBuilder.TabIndex = 0;
			this.queryBuilder.QueryChanged += new System.EventHandler(this.queryBuilder_QueryChanged);
			//
			// gridCard
			//
			this.gridCard.AppearanceKey = "nw-card";
			this.gridCard.Controls.Add(this.ordersGrid);
			this.gridCard.Controls.Add(this.summaryLabel);
			this.gridCard.Dock = Wisej.Web.DockStyle.Fill;
			this.gridCard.HeaderSize = 46;
			this.gridCard.Location = new System.Drawing.Point(24, 84);
			this.gridCard.Margin = new Wisej.Web.Padding(8);
			this.gridCard.Name = "gridCard";
			this.gridCard.Padding = new Wisej.Web.Padding(4, 0, 4, 4);
			this.gridCard.ShowHeader = true;
			this.gridCard.Size = new System.Drawing.Size(1160, 720);
			this.gridCard.TabIndex = 2;
			this.gridCard.Text = "Orders";
			this.gridCard.ToolClick += new Wisej.Web.ToolClickEventHandler(this.gridCard_ToolClick);
			//
			// ordersGrid
			//
			this.ordersGrid.AllowUserToResizeRows = false;
			this.ordersGrid.AutoGenerateColumns = false;
			this.ordersGrid.BorderStyle = Wisej.Web.BorderStyle.None;
			this.ordersGrid.CellBorderStyle = Wisej.Web.DataGridViewCellBorderStyle.Horizontal;
			dataGridViewCellStyle1.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
			this.ordersGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.ordersGrid.ColumnHeadersHeight = 38;
			this.ordersGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colOrder,
            this.colCustomer,
            this.colSalesRep,
            this.colStatus,
            this.colDelivery,
            this.colItems,
            this.colAmount,
            this.colActions});
			this.ordersGrid.DefaultRowHeight = 54;
			this.ordersGrid.Dock = Wisej.Web.DockStyle.Fill;
			this.ordersGrid.Location = new System.Drawing.Point(4, 0);
			this.ordersGrid.MultiSelect = false;
			this.ordersGrid.Name = "ordersGrid";
			this.ordersGrid.ReadOnly = true;
			this.ordersGrid.RowHeadersVisible = false;
			this.ordersGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
			this.ordersGrid.ShowColumnVisibilityMenu = false;
			this.ordersGrid.Size = new System.Drawing.Size(1152, 638);
			this.ordersGrid.TabIndex = 0;
			this.ordersGrid.CellDoubleClick += new Wisej.Web.DataGridViewCellEventHandler(this.ordersGrid_CellDoubleClick);
			//
			// colOrder
			//
			this.colOrder.DataPropertyName = "Number";
			this.colOrder.HeaderText = "ORDER";
			this.colOrder.Name = "colOrder";
			this.colOrder.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colOrder.ResponsiveProfiles"))));
			this.colOrder.SubtitleMember = "DateText";
			this.colOrder.Width = 120;
			//
			// colCustomer
			//
			this.colCustomer.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
			this.colCustomer.DataPropertyName = "Customer";
			this.colCustomer.HeaderText = "CUSTOMER";
			this.colCustomer.MinimumWidth = 150;
			this.colCustomer.Name = "colCustomer";
			this.colCustomer.Size = 32;
			this.colCustomer.SubTextMember = "CustomerPlace";
			this.colCustomer.AvatarClick += new System.EventHandler<Wisej.Web.DataGridViewVisualCellEventArgs>(this.colCustomer_AvatarClick);
			//
			// colSalesRep
			//
			this.colSalesRep.DataPropertyName = "SalesRep";
			this.colSalesRep.HeaderText = "SALES REP";
			this.colSalesRep.Name = "colSalesRep";
			this.colSalesRep.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colSalesRep.ResponsiveProfiles"))));
			this.colSalesRep.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colSalesRep.ResponsiveProfiles1"))));
			this.colSalesRep.Size = 28;
			this.colSalesRep.SubTextMember = "SalesRepTitle";
			this.colSalesRep.Width = 200;
			this.colSalesRep.AvatarClick += new System.EventHandler<Wisej.Web.DataGridViewVisualCellEventArgs>(this.colSalesRep_AvatarClick);
			//
			// colStatus
			//
			this.colStatus.DataPropertyName = "StatusText";
			this.colStatus.HeaderText = "STATUS";
			this.colStatus.Name = "colStatus";
			this.colStatus.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colStatus.ResponsiveProfiles"))));
			this.colStatus.ShowDot = true;
			this.colStatus.ToneMember = "StatusTone";
			this.colStatus.Width = 130;
			//
			// colDelivery
			//
			this.colDelivery.DataPropertyName = "DeliveryText";
			this.colDelivery.HeaderText = "DELIVERY";
			this.colDelivery.Name = "colDelivery";
			this.colDelivery.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colDelivery.ResponsiveProfiles"))));
			this.colDelivery.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colDelivery.ResponsiveProfiles1"))));
			this.colDelivery.SubtitleMember = "Shipper";
			this.colDelivery.Width = 180;
			//
			// colItems
			//
			dataGridViewCellStyle2.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colItems.DefaultCellStyle = dataGridViewCellStyle2;
			this.colItems.DataPropertyName = "Items";
			dataGridViewCellStyle3.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colItems.HeaderStyle = dataGridViewCellStyle3;
			this.colItems.HeaderText = "LINES";
			this.colItems.Name = "colItems";
			this.colItems.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colItems.ResponsiveProfiles"))));
			this.colItems.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colItems.ResponsiveProfiles1"))));
			this.colItems.Width = 70;
			//
			// colAmount
			//
			dataGridViewCellStyle4.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle4.Format = "C2";
			this.colAmount.DefaultCellStyle = dataGridViewCellStyle4;
			this.colAmount.DataPropertyName = "Amount";
			dataGridViewCellStyle5.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colAmount.HeaderStyle = dataGridViewCellStyle5;
			this.colAmount.HeaderText = "AMOUNT";
			this.colAmount.Name = "colAmount";
			this.colAmount.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colAmount.ResponsiveProfiles"))));
			this.colAmount.Width = 120;
			//
			// colActions
			//
			cardAction1.Icon = "Assets/Icons/eye.svg";
			cardAction1.Name = "view";
			cardAction1.Text = "View order";
			cardAction2.Icon = "Assets/Icons/ship.svg";
			cardAction2.Name = "ship";
			cardAction2.Text = "Mark as shipped";
			this.colActions.Actions.Add(cardAction1);
			this.colActions.Actions.Add(cardAction2);
			this.colActions.HeaderText = "";
			this.colActions.IconOnly = true;
			this.colActions.Name = "colActions";
			this.colActions.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colActions.ResponsiveProfiles"))));
			this.colActions.Width = 96;
			this.colActions.ActionClick += new System.EventHandler<Wisej.Web.DataGridViewVisualCellEventArgs>(this.colActions_ActionClick);
			//
			// summaryLabel
			//
			this.summaryLabel.AppearanceKey = "nw-caption";
			this.summaryLabel.Dock = Wisej.Web.DockStyle.Bottom;
			this.summaryLabel.Location = new System.Drawing.Point(4, 638);
			this.summaryLabel.Name = "summaryLabel";
			this.summaryLabel.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
			this.summaryLabel.Size = new System.Drawing.Size(1152, 32);
			this.summaryLabel.TabIndex = 1;
			this.summaryLabel.Text = "Showing 0 orders";
			this.summaryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			// OrdersView
			//
			this.AppearanceKey = "nw-view";
			this.Controls.Add(this.layout);
			this.Name = "OrdersView";
			this.Size = new System.Drawing.Size(1208, 828);
			this.Text = "Orders";
			this.layout.ResumeLayout(false);
			this.toolbar.ResumeLayout(false);
			this.queryCard.ResumeLayout(false);
			this.gridCard.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ordersGrid)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.TableLayoutPanel layout;
		private Wisej.Web.Panel toolbar;
		private Wisej.Web.ChipGroup statusFilter;
		private Wisej.Web.TextBox searchBox;
		private Wisej.Web.Button advancedButton;
		private Wisej.Web.Panel queryCard;
		private Wisej.Web.QueryBuilder queryBuilder;
		private Wisej.Web.Panel gridCard;
		private Wisej.Web.DataGridView ordersGrid;
		private Wisej.Web.DataGridViewSubtitleColumn colOrder;
		private Wisej.Web.DataGridViewAvatarColumn colCustomer;
		private Wisej.Web.DataGridViewAvatarColumn colSalesRep;
		private Wisej.Web.DataGridViewChipColumn colStatus;
		private Wisej.Web.DataGridViewSubtitleColumn colDelivery;
		private Wisej.Web.DataGridViewTextBoxColumn colItems;
		private Wisej.Web.DataGridViewTextBoxColumn colAmount;
		private Wisej.Web.DataGridViewActionsColumn colActions;
		private Wisej.Web.Label summaryLabel;
	}
}
