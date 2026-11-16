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

namespace Wisej.NorthwindDashboard.Dialogs
{
	partial class OrderDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderDialog));
			Wisej.Web.Step step1 = new Wisej.Web.Step();
			Wisej.Web.Step step2 = new Wisej.Web.Step();
			Wisej.Web.Step step3 = new Wisej.Web.Step();
			Wisej.Web.Step step4 = new Wisej.Web.Step();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle1 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle2 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle3 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle4 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle5 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle6 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle7 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle8 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle9 = new Wisej.Web.DataGridViewCellStyle();
			this.headerPanel = new Wisej.Web.Panel();
			this.statusChip = new Wisej.Web.ChipLabel();
			this.subtitleLabel = new Wisej.Web.Label();
			this.titleLabel = new Wisej.Web.Label();
			this.stepper = new Wisej.Web.Stepper();
			this.partiesPanel = new Wisej.Web.Panel();
			this.shipToLabel = new Wisej.Web.Label();
			this.salesRepAvatar = new Wisej.Web.Avatar();
			this.customerAvatar = new Wisej.Web.Avatar();
			this.linesGrid = new Wisej.Web.DataGridView();
			this.colProduct = new Wisej.Web.DataGridViewSubtitleColumn();
			this.colQuantity = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colUnitPrice = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colDiscount = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colTotal = new Wisej.Web.DataGridViewTextBoxColumn();
			this.footerPanel = new Wisej.Web.Panel();
			this.totalsLabel = new Wisej.Web.Label();
			this.shipButton = new Wisej.Web.Button();
			this.closeButton = new Wisej.Web.Button();
			this.headerPanel.SuspendLayout();
			this.partiesPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.linesGrid)).BeginInit();
			this.footerPanel.SuspendLayout();
			this.SuspendLayout();
			//
			// headerPanel
			//
			this.headerPanel.AppearanceKey = "nw-view";
			this.headerPanel.Controls.Add(this.statusChip);
			this.headerPanel.Controls.Add(this.subtitleLabel);
			this.headerPanel.Controls.Add(this.titleLabel);
			this.headerPanel.Dock = Wisej.Web.DockStyle.Top;
			this.headerPanel.Location = new System.Drawing.Point(24, 16);
			this.headerPanel.Name = "headerPanel";
			this.headerPanel.Size = new System.Drawing.Size(772, 64);
			this.headerPanel.TabIndex = 0;
			//
			// statusChip
			//
			this.statusChip.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
			this.statusChip.Location = new System.Drawing.Point(652, 6);
			this.statusChip.Name = "statusChip";
			this.statusChip.ShowDot = true;
			this.statusChip.Size = new System.Drawing.Size(120, 30);
			this.statusChip.TabIndex = 2;
			this.statusChip.Text = "Open";
			this.statusChip.Tone = Wisej.Web.ChipTone.Primary;
			//
			// subtitleLabel
			//
			this.subtitleLabel.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.subtitleLabel.AppearanceKey = "nw-subtitle";
			this.subtitleLabel.AutoEllipsis = true;
			this.subtitleLabel.Location = new System.Drawing.Point(0, 38);
			this.subtitleLabel.Name = "subtitleLabel";
			this.subtitleLabel.Size = new System.Drawing.Size(620, 20);
			this.subtitleLabel.TabIndex = 1;
			this.subtitleLabel.Text = "Placed";
			//
			// titleLabel
			//
			this.titleLabel.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.titleLabel.AppearanceKey = "nw-page-title";
			this.titleLabel.Location = new System.Drawing.Point(0, 4);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(620, 32);
			this.titleLabel.TabIndex = 0;
			this.titleLabel.Text = "Order";
			//
			// stepper
			//
			this.stepper.AccessibleName = "Order progress";
			this.stepper.Dock = Wisej.Web.DockStyle.Top;
			this.stepper.Location = new System.Drawing.Point(24, 80);
			this.stepper.Name = "stepper";
			this.stepper.Size = new System.Drawing.Size(772, 84);
			step1.Text = "Ordered";
			step2.Text = "Picked and packed";
			step3.Text = "Shipped";
			step4.Text = "Delivery due";
			this.stepper.Steps.Add(step1);
			this.stepper.Steps.Add(step2);
			this.stepper.Steps.Add(step3);
			this.stepper.Steps.Add(step4);
			this.stepper.TabIndex = 1;
			//
			// partiesPanel
			//
			this.partiesPanel.AppearanceKey = "nw-view";
			this.partiesPanel.Controls.Add(this.shipToLabel);
			this.partiesPanel.Controls.Add(this.salesRepAvatar);
			this.partiesPanel.Controls.Add(this.customerAvatar);
			this.partiesPanel.Dock = Wisej.Web.DockStyle.Top;
			this.partiesPanel.Location = new System.Drawing.Point(24, 164);
			this.partiesPanel.Name = "partiesPanel";
			this.partiesPanel.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("partiesPanel.ResponsiveProfiles"))));
			this.partiesPanel.Size = new System.Drawing.Size(772, 104);
			this.partiesPanel.TabIndex = 2;
			//
			// shipToLabel
			//
			this.shipToLabel.AppearanceKey = "nw-subtitle";
			this.shipToLabel.Location = new System.Drawing.Point(0, 58);
			this.shipToLabel.Name = "shipToLabel";
			this.shipToLabel.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("shipToLabel.ResponsiveProfiles"))));
			this.shipToLabel.Size = new System.Drawing.Size(420, 40);
			this.shipToLabel.TabIndex = 1;
			this.shipToLabel.Text = "Ship to";
			//
			// salesRepAvatar
			//
			this.salesRepAvatar.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
			this.salesRepAvatar.Label = "Sales rep";
			this.salesRepAvatar.Location = new System.Drawing.Point(472, 6);
			this.salesRepAvatar.Name = "salesRepAvatar";
			this.salesRepAvatar.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("salesRepAvatar.ResponsiveProfiles"))));
			this.salesRepAvatar.Size = new System.Drawing.Size(300, 48);
			this.salesRepAvatar.SubLabel = "Title";
			this.salesRepAvatar.TabIndex = 2;
			this.salesRepAvatar.Text = "Sales rep";
			this.salesRepAvatar.Click += new System.EventHandler(this.salesRepAvatar_Click);
			//
			// customerAvatar
			//
			this.customerAvatar.Label = "Customer";
			this.customerAvatar.Location = new System.Drawing.Point(0, 6);
			this.customerAvatar.Name = "customerAvatar";
			this.customerAvatar.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("customerAvatar.ResponsiveProfiles"))));
			this.customerAvatar.Shape = Wisej.Web.AvatarShape.Rounded;
			this.customerAvatar.Size = new System.Drawing.Size(420, 48);
			this.customerAvatar.SubLabel = "Contact";
			this.customerAvatar.TabIndex = 0;
			this.customerAvatar.Text = "Customer";
			this.customerAvatar.Click += new System.EventHandler(this.customerAvatar_Click);
			//
			// linesGrid
			//
			this.linesGrid.AllowUserToResizeRows = false;
			this.linesGrid.AutoGenerateColumns = false;
			this.linesGrid.CellBorderStyle = Wisej.Web.DataGridViewCellBorderStyle.Horizontal;
			dataGridViewCellStyle1.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
			this.linesGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.linesGrid.ColumnHeadersHeight = 36;
			this.linesGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colProduct,
            this.colQuantity,
            this.colUnitPrice,
            this.colDiscount,
            this.colTotal});
			this.linesGrid.DefaultRowHeight = 46;
			this.linesGrid.Dock = Wisej.Web.DockStyle.Fill;
			this.linesGrid.Location = new System.Drawing.Point(24, 268);
			this.linesGrid.Name = "linesGrid";
			this.linesGrid.ReadOnly = true;
			this.linesGrid.RowHeadersVisible = false;
			this.linesGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
			this.linesGrid.ShowColumnVisibilityMenu = false;
			this.linesGrid.Size = new System.Drawing.Size(772, 246);
			this.linesGrid.TabIndex = 3;
			//
			// colProduct
			//
			this.colProduct.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
			this.colProduct.DataPropertyName = "Product";
			this.colProduct.HeaderText = "PRODUCT";
			this.colProduct.Name = "colProduct";
			this.colProduct.SubtitleMember = "Category";
			//
			// colQuantity
			//
			dataGridViewCellStyle2.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colQuantity.DefaultCellStyle = dataGridViewCellStyle2;
			this.colQuantity.DataPropertyName = "Quantity";
			dataGridViewCellStyle3.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colQuantity.HeaderStyle = dataGridViewCellStyle3;
			this.colQuantity.HeaderText = "QTY";
			this.colQuantity.Name = "colQuantity";
			this.colQuantity.Width = 70;
			//
			// colUnitPrice
			//
			dataGridViewCellStyle4.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle4.Format = "C2";
			this.colUnitPrice.DefaultCellStyle = dataGridViewCellStyle4;
			this.colUnitPrice.DataPropertyName = "UnitPrice";
			dataGridViewCellStyle5.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colUnitPrice.HeaderStyle = dataGridViewCellStyle5;
			this.colUnitPrice.HeaderText = "UNIT PRICE";
			this.colUnitPrice.Name = "colUnitPrice";
			this.colUnitPrice.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colUnitPrice.ResponsiveProfiles"))));
			this.colUnitPrice.Width = 110;
			//
			// colDiscount
			//
			dataGridViewCellStyle6.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle6.Format = "0%";
			this.colDiscount.DefaultCellStyle = dataGridViewCellStyle6;
			this.colDiscount.DataPropertyName = "Discount";
			dataGridViewCellStyle7.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colDiscount.HeaderStyle = dataGridViewCellStyle7;
			this.colDiscount.HeaderText = "DISCOUNT";
			this.colDiscount.Name = "colDiscount";
			this.colDiscount.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("colDiscount.ResponsiveProfiles"))));
			this.colDiscount.Width = 96;
			//
			// colTotal
			//
			dataGridViewCellStyle8.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle8.Format = "C2";
			this.colTotal.DefaultCellStyle = dataGridViewCellStyle8;
			this.colTotal.DataPropertyName = "Total";
			dataGridViewCellStyle9.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
			this.colTotal.HeaderStyle = dataGridViewCellStyle9;
			this.colTotal.HeaderText = "TOTAL";
			this.colTotal.Name = "colTotal";
			this.colTotal.Width = 110;
			//
			// footerPanel
			//
			this.footerPanel.AppearanceKey = "nw-view";
			this.footerPanel.Controls.Add(this.totalsLabel);
			this.footerPanel.Controls.Add(this.shipButton);
			this.footerPanel.Controls.Add(this.closeButton);
			this.footerPanel.Dock = Wisej.Web.DockStyle.Bottom;
			this.footerPanel.Location = new System.Drawing.Point(24, 514);
			this.footerPanel.Name = "footerPanel";
			this.footerPanel.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("footerPanel.ResponsiveProfiles"))));
			this.footerPanel.Size = new System.Drawing.Size(772, 64);
			this.footerPanel.TabIndex = 4;
			//
			// totalsLabel
			//
			this.totalsLabel.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.totalsLabel.AppearanceKey = "nw-strong";
			this.totalsLabel.Location = new System.Drawing.Point(0, 20);
			this.totalsLabel.Name = "totalsLabel";
			this.totalsLabel.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("totalsLabel.ResponsiveProfiles"))));
			this.totalsLabel.Size = new System.Drawing.Size(460, 24);
			this.totalsLabel.TabIndex = 0;
			this.totalsLabel.Text = "Total";
			//
			// shipButton
			//
			this.shipButton.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
			this.shipButton.AppearanceKey = "nw-button-primary";
			this.shipButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.shipButton.ImageSource = "Assets/Icons/ship.svg";
			this.shipButton.Location = new System.Drawing.Point(488, 16);
			this.shipButton.Name = "shipButton";
			this.shipButton.Size = new System.Drawing.Size(170, 36);
			this.shipButton.TabIndex = 1;
			this.shipButton.Text = "Mark as shipped";
			this.shipButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.shipButton.Click += new System.EventHandler(this.shipButton_Click);
			//
			// closeButton
			//
			this.closeButton.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
			this.closeButton.AppearanceKey = "nw-button";
			this.closeButton.DialogResult = Wisej.Web.DialogResult.Cancel;
			this.closeButton.Location = new System.Drawing.Point(672, 16);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(100, 36);
			this.closeButton.TabIndex = 2;
			this.closeButton.Text = "Close";
			//
			// OrderDialog
			//
			this.AutoCloseModalDialog = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = Wisej.Web.AutoScaleMode.Font;
			this.CancelButton = this.closeButton;
			this.ClientSize = new System.Drawing.Size(820, 594);
			this.Controls.Add(this.linesGrid);
			this.Controls.Add(this.partiesPanel);
			this.Controls.Add(this.stepper);
			this.Controls.Add(this.headerPanel);
			this.Controls.Add(this.footerPanel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OrderDialog";
			this.Padding = new Wisej.Web.Padding(24, 16, 24, 16);
			this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
			this.Text = "Order details";
			this.headerPanel.ResumeLayout(false);
			this.partiesPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.linesGrid)).EndInit();
			this.footerPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.Panel headerPanel;
		private Wisej.Web.Label titleLabel;
		private Wisej.Web.Label subtitleLabel;
		private Wisej.Web.ChipLabel statusChip;
		private Wisej.Web.Stepper stepper;
		private Wisej.Web.Panel partiesPanel;
		private Wisej.Web.Avatar customerAvatar;
		private Wisej.Web.Avatar salesRepAvatar;
		private Wisej.Web.Label shipToLabel;
		private Wisej.Web.DataGridView linesGrid;
		private Wisej.Web.DataGridViewSubtitleColumn colProduct;
		private Wisej.Web.DataGridViewTextBoxColumn colQuantity;
		private Wisej.Web.DataGridViewTextBoxColumn colUnitPrice;
		private Wisej.Web.DataGridViewTextBoxColumn colDiscount;
		private Wisej.Web.DataGridViewTextBoxColumn colTotal;
		private Wisej.Web.Panel footerPanel;
		private Wisej.Web.Label totalsLabel;
		private Wisej.Web.Button shipButton;
		private Wisej.Web.Button closeButton;
	}
}
