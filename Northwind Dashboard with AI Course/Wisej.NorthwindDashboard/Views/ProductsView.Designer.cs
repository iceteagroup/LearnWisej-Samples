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
	partial class ProductsView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductsView));
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle1 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle2 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle3 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.CardAction cardAction1 = new Wisej.Web.CardAction();
			Wisej.Web.CardAction cardAction2 = new Wisej.Web.CardAction();
			this.layout = new Wisej.Web.TableLayoutPanel();
			this.inventoryCard = new Wisej.Web.Panel();
			this.discontinuedChip = new Wisej.Web.ChipLabel();
			this.outOfStockChip = new Wisej.Web.ChipLabel();
			this.reorderChip = new Wisej.Web.ChipLabel();
			this.incomingChip = new Wisej.Web.ChipLabel();
			this.inStockChip = new Wisej.Web.ChipLabel();
			this.inventoryMeter = new Wisej.Web.MeterBar();
			this.attentionKpi = new Wisej.Web.KpiPanel();
			this.catalogCard = new Wisej.Web.Panel();
			this.productsGrid = new Wisej.Web.DataGridView();
			this.colProduct = new Wisej.Web.DataGridViewSubtitleColumn();
			this.colPrice = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colUnitsTrend = new Wisej.Web.DataGridViewSparklineColumn();
			this.colStock = new Wisej.Web.DataGridViewMeterColumn();
			this.colStatus = new Wisej.Web.DataGridViewChipColumn();
			this.colActions = new Wisej.Web.DataGridViewActionsColumn();
			this.categoryFilter = new Wisej.Web.ChipGroup();
			this.reorderCard = new Wisej.Web.Panel();
			this.reorderList = new Wisej.Web.ActionCardList();
			this.layout.SuspendLayout();
			this.inventoryCard.SuspendLayout();
			this.catalogCard.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.productsGrid)).BeginInit();
			this.reorderCard.SuspendLayout();
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
			this.layout.Controls.Add(this.inventoryCard, 0, 0);
			this.layout.Controls.Add(this.attentionKpi, 8, 0);
			this.layout.Controls.Add(this.catalogCard, 0, 1);
			this.layout.Controls.Add(this.reorderCard, 8, 1);
			this.layout.Dock = Wisej.Web.DockStyle.Fill;
			this.layout.Location = new System.Drawing.Point(0, 0);
			this.layout.Name = "layout";
			this.layout.Padding = new Wisej.Web.Padding(16);
			this.layout.RowCount = 2;
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 156F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
			this.layout.SetColumnSpan(this.inventoryCard, 8);
			this.layout.SetColumnSpan(this.attentionKpi, 4);
			this.layout.SetColumnSpan(this.catalogCard, 8);
			this.layout.SetColumnSpan(this.reorderCard, 4);
			this.layout.Size = new System.Drawing.Size(1208, 828);
			this.layout.TabIndex = 0;
			//
			// inventoryCard
			//
			this.inventoryCard.AppearanceKey = "nw-card";
			this.inventoryCard.Controls.Add(this.discontinuedChip);
			this.inventoryCard.Controls.Add(this.outOfStockChip);
			this.inventoryCard.Controls.Add(this.reorderChip);
			this.inventoryCard.Controls.Add(this.incomingChip);
			this.inventoryCard.Controls.Add(this.inStockChip);
			this.inventoryCard.Controls.Add(this.inventoryMeter);
			this.inventoryCard.Dock = Wisej.Web.DockStyle.Fill;
			this.inventoryCard.HeaderSize = 46;
			this.inventoryCard.Location = new System.Drawing.Point(24, 24);
			this.inventoryCard.Margin = new Wisej.Web.Padding(8);
			this.inventoryCard.Name = "inventoryCard";
			this.inventoryCard.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("inventoryCard.ResponsiveProfiles"))));
			this.inventoryCard.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("inventoryCard.ResponsiveProfiles1"))));
			this.inventoryCard.ShowHeader = true;
			this.inventoryCard.Size = new System.Drawing.Size(768, 140);
			this.inventoryCard.TabIndex = 0;
			this.inventoryCard.Text = "Inventory health";
			//
			// discontinuedChip
			//
			this.discontinuedChip.Location = new System.Drawing.Point(600, 56);
			this.discontinuedChip.Name = "discontinuedChip";
			this.discontinuedChip.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("discontinuedChip.ResponsiveProfiles"))));
			this.discontinuedChip.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("discontinuedChip.ResponsiveProfiles1"))));
			this.discontinuedChip.Selectable = true;
			this.discontinuedChip.ShowDot = true;
			this.discontinuedChip.Size = new System.Drawing.Size(138, 28);
			this.discontinuedChip.TabIndex = 5;
			this.discontinuedChip.Tag = "Discontinued";
			this.discontinuedChip.Text = "Discontinued";
			this.discontinuedChip.SelectedChanged += new System.EventHandler(this.statusChip_SelectedChanged);
			//
			// outOfStockChip
			//
			this.outOfStockChip.Location = new System.Drawing.Point(454, 56);
			this.outOfStockChip.Name = "outOfStockChip";
			this.outOfStockChip.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("outOfStockChip.ResponsiveProfiles"))));
			this.outOfStockChip.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("outOfStockChip.ResponsiveProfiles1"))));
			this.outOfStockChip.Selectable = true;
			this.outOfStockChip.ShowDot = true;
			this.outOfStockChip.Size = new System.Drawing.Size(138, 28);
			this.outOfStockChip.TabIndex = 4;
			this.outOfStockChip.Tag = "OutOfStock";
			this.outOfStockChip.Text = "Out of stock";
			this.outOfStockChip.Tone = Wisej.Web.ChipTone.Danger;
			this.outOfStockChip.SelectedChanged += new System.EventHandler(this.statusChip_SelectedChanged);
			//
			// reorderChip
			//
			this.reorderChip.Location = new System.Drawing.Point(308, 56);
			this.reorderChip.Name = "reorderChip";
			this.reorderChip.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("reorderChip.ResponsiveProfiles"))));
			this.reorderChip.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("reorderChip.ResponsiveProfiles1"))));
			this.reorderChip.Selectable = true;
			this.reorderChip.ShowDot = true;
			this.reorderChip.Size = new System.Drawing.Size(138, 28);
			this.reorderChip.TabIndex = 3;
			this.reorderChip.Tag = "Reorder";
			this.reorderChip.Text = "Reorder";
			this.reorderChip.Tone = Wisej.Web.ChipTone.Warning;
			this.reorderChip.SelectedChanged += new System.EventHandler(this.statusChip_SelectedChanged);
			//
			// incomingChip
			//
			this.incomingChip.Location = new System.Drawing.Point(162, 56);
			this.incomingChip.Name = "incomingChip";
			this.incomingChip.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("incomingChip.ResponsiveProfiles"))));
			this.incomingChip.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("incomingChip.ResponsiveProfiles1"))));
			this.incomingChip.Selectable = true;
			this.incomingChip.ShowDot = true;
			this.incomingChip.Size = new System.Drawing.Size(138, 28);
			this.incomingChip.TabIndex = 2;
			this.incomingChip.Tag = "Incoming";
			this.incomingChip.Text = "Incoming";
			this.incomingChip.Tone = Wisej.Web.ChipTone.Primary;
			this.incomingChip.SelectedChanged += new System.EventHandler(this.statusChip_SelectedChanged);
			//
			// inStockChip
			//
			this.inStockChip.Location = new System.Drawing.Point(16, 56);
			this.inStockChip.Name = "inStockChip";
			this.inStockChip.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("inStockChip.ResponsiveProfiles"))));
			this.inStockChip.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("inStockChip.ResponsiveProfiles1"))));
			this.inStockChip.Selectable = true;
			this.inStockChip.ShowDot = true;
			this.inStockChip.Size = new System.Drawing.Size(138, 28);
			this.inStockChip.TabIndex = 1;
			this.inStockChip.Tag = "InStock";
			this.inStockChip.Text = "In stock";
			this.inStockChip.Tone = Wisej.Web.ChipTone.Success;
			this.inStockChip.SelectedChanged += new System.EventHandler(this.statusChip_SelectedChanged);
			//
			// inventoryMeter
			//
			this.inventoryMeter.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.inventoryMeter.BarHeight = 12;
			this.inventoryMeter.Label = "Products by stock position";
			this.inventoryMeter.Location = new System.Drawing.Point(16, 2);
			this.inventoryMeter.Name = "inventoryMeter";
			this.inventoryMeter.Size = new System.Drawing.Size(736, 46);
			this.inventoryMeter.TabIndex = 0;
			//
			// attentionKpi
			//
			this.attentionKpi.AccentPosition = Wisej.Web.KpiAccentPosition.Left;
			this.attentionKpi.Dock = Wisej.Web.DockStyle.Fill;
			this.attentionKpi.Icon = "Assets/Icons/alert.svg";
			this.attentionKpi.Location = new System.Drawing.Point(808, 24);
			this.attentionKpi.Margin = new Wisej.Web.Padding(8);
			this.attentionKpi.Name = "attentionKpi";
			this.attentionKpi.Size = new System.Drawing.Size(376, 140);
			this.attentionKpi.TabIndex = 1;
			this.attentionKpi.Title = "NEEDS ATTENTION";
			this.attentionKpi.Tone = Wisej.Web.KpiTone.Warning;
			this.attentionKpi.Value = "0 products";
			//
			// catalogCard
			//
			this.catalogCard.AppearanceKey = "nw-card";
			this.catalogCard.Controls.Add(this.productsGrid);
			this.catalogCard.Controls.Add(this.categoryFilter);
			this.catalogCard.Dock = Wisej.Web.DockStyle.Fill;
			this.catalogCard.HeaderSize = 46;
			this.catalogCard.Location = new System.Drawing.Point(24, 180);
			this.catalogCard.Margin = new Wisej.Web.Padding(8);
			this.catalogCard.Name = "catalogCard";
			this.catalogCard.Padding = new Wisej.Web.Padding(4, 0, 4, 4);
			this.catalogCard.ShowHeader = true;
			this.catalogCard.Size = new System.Drawing.Size(768, 624);
			this.catalogCard.TabIndex = 2;
			this.catalogCard.Text = "Product catalog";
			//
			// productsGrid
			//
			this.productsGrid.AllowUserToResizeRows = false;
			this.productsGrid.AutoGenerateColumns = false;
			this.productsGrid.BorderStyle = Wisej.Web.BorderStyle.None;
			this.productsGrid.CellBorderStyle = Wisej.Web.DataGridViewCellBorderStyle.Horizontal;
			dataGridViewCellStyle1.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
			this.productsGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.productsGrid.ColumnHeadersHeight = 38;
			this.productsGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colProduct,
            this.colPrice,
            this.colUnitsTrend,
            this.colStock,
            this.colStatus,
            this.colActions});
			this.productsGrid.DefaultRowHeight = 52;
			this.productsGrid.Dock = Wisej.Web.DockStyle.Fill;
			this.productsGrid.Location = new System.Drawing.Point(4, 76);
			this.productsGrid.MultiSelect = false;
			this.productsGrid.Name = "productsGrid";
			this.productsGrid.ReadOnly = true;
			this.productsGrid.RowHeadersVisible = false;
			this.productsGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
			this.productsGrid.ShowColumnVisibilityMenu = false;
			this.productsGrid.Size = new System.Drawing.Size(760, 498);
			this.productsGrid.TabIndex = 1;
			//
			// colProduct
			//
			this.colProduct.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
			this.colProduct.DataPropertyName = "Name";
			this.colProduct.HeaderText = "PRODUCT";
			this.colProduct.MinimumWidth = 150;
			this.colProduct.Name = "colProduct";
			this.colProduct.SubtitleMember = "DetailText";
			//
			// colPrice
			//
			dataGridViewCellStyle2.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle2.Format = "C2";
			this.colPrice.DefaultCellStyle = dataGridViewCellStyle2;
			this.colPrice.DataPropertyName = "UnitPrice";
			dataGridViewCellStyle3.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colPrice.HeaderStyle = dataGridViewCellStyle3;
			this.colPrice.HeaderText = "PRICE";
			this.colPrice.Name = "colPrice";
			this.colPrice.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colPrice.ResponsiveProfiles"))));
			this.colPrice.Width = 76;
			//
			// colUnitsTrend
			//
			this.colUnitsTrend.DataPropertyName = "UnitsTrend";
			this.colUnitsTrend.HeaderText = "UNITS SOLD";
			this.colUnitsTrend.Name = "colUnitsTrend";
			this.colUnitsTrend.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colUnitsTrend.ResponsiveProfiles"))));
			this.colUnitsTrend.Width = 96;
			//
			// colStock
			//
			this.colStock.DataPropertyName = "StockCoverage";
			this.colStock.HeaderText = "STOCK";
			this.colStock.Maximum = 4D;
			this.colStock.Name = "colStock";
			this.colStock.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colStock.ResponsiveProfiles"))));
			this.colStock.TextMember = "StockText";
			this.colStock.Width = 130;
			//
			// colStatus
			//
			this.colStatus.DataPropertyName = "StatusText";
			this.colStatus.HeaderText = "STATUS";
			this.colStatus.Name = "colStatus";
			this.colStatus.ShowDot = true;
			this.colStatus.ToneMember = "StatusTone";
			this.colStatus.Width = 124;
			//
			// colActions
			//
			cardAction1.Icon = "Assets/Icons/reorder.svg";
			cardAction1.Name = "reorder";
			cardAction1.Text = "Create purchase order";
			cardAction2.Icon = "Assets/Icons/orders.svg";
			cardAction2.Name = "orders";
			cardAction2.Text = "Show orders";
			this.colActions.Actions.Add(cardAction1);
			this.colActions.Actions.Add(cardAction2);
			this.colActions.HeaderText = "";
			this.colActions.IconOnly = true;
			this.colActions.Name = "colActions";
			this.colActions.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colActions.ResponsiveProfiles"))));
			this.colActions.Width = 80;
			this.colActions.ActionClick += new System.EventHandler<Wisej.Web.DataGridViewVisualCellEventArgs>(this.colActions_ActionClick);
			//
			// categoryFilter
			//
			this.categoryFilter.AccessibleName = "Categories";
			this.categoryFilter.AllowDeselect = true;
			this.categoryFilter.Dock = Wisej.Web.DockStyle.Top;
			this.categoryFilter.Location = new System.Drawing.Point(4, 0);
			this.categoryFilter.Name = "categoryFilter";
			this.categoryFilter.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
			this.categoryFilter.SegmentSize = Wisej.Web.SegmentSize.Small;
			this.categoryFilter.SelectionMode = Wisej.Web.SegmentSelectionMode.Multiple;
			this.categoryFilter.ShowCounts = true;
			this.categoryFilter.Size = new System.Drawing.Size(760, 76);
			this.categoryFilter.TabIndex = 0;
			this.categoryFilter.SelectionChanged += new System.EventHandler(this.categoryFilter_SelectionChanged);
			//
			// reorderCard
			//
			this.reorderCard.AppearanceKey = "nw-card";
			this.reorderCard.Controls.Add(this.reorderList);
			this.reorderCard.Dock = Wisej.Web.DockStyle.Fill;
			this.reorderCard.HeaderSize = 46;
			this.reorderCard.Location = new System.Drawing.Point(808, 180);
			this.reorderCard.Margin = new Wisej.Web.Padding(8);
			this.reorderCard.Name = "reorderCard";
			this.reorderCard.Padding = new Wisej.Web.Padding(12, 0, 12, 12);
			this.reorderCard.ShowHeader = true;
			this.reorderCard.Size = new System.Drawing.Size(376, 624);
			this.reorderCard.TabIndex = 3;
			this.reorderCard.Text = "Reorder suggestions";
			//
			// reorderList
			//
			this.reorderList.AccessibleName = "Reorder suggestions";
			this.reorderList.Dock = Wisej.Web.DockStyle.Fill;
			this.reorderList.ItemHeight = 132;
			this.reorderList.ItemSpacing = 10;
			this.reorderList.Location = new System.Drawing.Point(12, 0);
			this.reorderList.Name = "reorderList";
			this.reorderList.Size = new System.Drawing.Size(352, 566);
			this.reorderList.TabIndex = 0;
			this.reorderList.WindowSize = 12;
			this.reorderList.CardCreated += new System.EventHandler<Wisej.Web.ActionCardListItemEventArgs>(this.reorderList_CardCreated);
			//
			// ProductsView
			//
			this.AppearanceKey = "nw-view";
			this.Controls.Add(this.layout);
			this.Name = "ProductsView";
			this.Size = new System.Drawing.Size(1208, 828);
			this.Text = "Products";
			this.layout.ResumeLayout(false);
			this.inventoryCard.ResumeLayout(false);
			this.catalogCard.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.productsGrid)).EndInit();
			this.reorderCard.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.TableLayoutPanel layout;
		private Wisej.Web.Panel inventoryCard;
		private Wisej.Web.MeterBar inventoryMeter;
		private Wisej.Web.ChipLabel inStockChip;
		private Wisej.Web.ChipLabel incomingChip;
		private Wisej.Web.ChipLabel reorderChip;
		private Wisej.Web.ChipLabel outOfStockChip;
		private Wisej.Web.ChipLabel discontinuedChip;
		private Wisej.Web.KpiPanel attentionKpi;
		private Wisej.Web.Panel catalogCard;
		private Wisej.Web.ChipGroup categoryFilter;
		private Wisej.Web.DataGridView productsGrid;
		private Wisej.Web.DataGridViewSubtitleColumn colProduct;
		private Wisej.Web.DataGridViewTextBoxColumn colPrice;
		private Wisej.Web.DataGridViewSparklineColumn colUnitsTrend;
		private Wisej.Web.DataGridViewMeterColumn colStock;
		private Wisej.Web.DataGridViewChipColumn colStatus;
		private Wisej.Web.DataGridViewActionsColumn colActions;
		private Wisej.Web.Panel reorderCard;
		private Wisej.Web.ActionCardList reorderList;
	}
}
