namespace NorthwindDashboard.Views
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
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle1 = new Wisej.Web.DataGridViewCellStyle();
			Wisej.Web.DataGridViewCellStyle dataGridViewCellStyle2 = new Wisej.Web.DataGridViewCellStyle();
			this.map = new Wisej.Web.MapView();
			this.detailPanel = new Wisej.Web.Panel();
			this.customersGrid = new Wisej.Web.DataGridView();
			this.colCompany = new Wisej.Web.DataGridViewTextBoxColumn();
			this.colPlace = new Wisej.Web.DataGridViewTextBoxColumn();
			this.countryChip = new Wisej.Web.ChipLabel();
			this.listLabel = new Wisej.Web.Label();
			this.sourceLabel = new Wisej.Web.Label();
			this.customerAvatar = new Wisej.Web.Avatar();
			this.detailPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.customersGrid)).BeginInit();
			this.SuspendLayout();
			//
			// map
			//
			this.map.AccessibleName = "Customer map";
			this.map.Center = new Wisej.Web.GeoPoint(38D, -35D);
			this.map.Cluster = false;
			this.map.Dock = Wisej.Web.DockStyle.Fill;
			this.map.LegendText = "Revenue quintile";
			this.map.Location = new System.Drawing.Point(0, 0);
			this.map.Name = "map";
			this.map.Size = new System.Drawing.Size(846, 752);
			this.map.TabIndex = 0;
			this.map.Zoom = 2;
			this.map.MarkerClick += new System.EventHandler<Wisej.Web.MapMarkerEventArgs>(this.map_MarkerClick);
			//
			// detailPanel
			//
			this.detailPanel.Controls.Add(this.customersGrid);
			this.detailPanel.Controls.Add(this.countryChip);
			this.detailPanel.Controls.Add(this.listLabel);
			this.detailPanel.Controls.Add(this.sourceLabel);
			this.detailPanel.Controls.Add(this.customerAvatar);
			this.detailPanel.Dock = Wisej.Web.DockStyle.Right;
			this.detailPanel.Location = new System.Drawing.Point(846, 0);
			this.detailPanel.Name = "detailPanel";
			this.detailPanel.Size = new System.Drawing.Size(360, 752);
			this.detailPanel.TabIndex = 1;
			//
			// customersGrid
			//
			this.customersGrid.AccessibleName = "Customer list";
			this.customersGrid.AllowUserToResizeColumns = false;
			this.customersGrid.AllowUserToResizeRows = false;
			this.customersGrid.Anchor = ((Wisej.Web.AnchorStyles)((((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom)
            | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.customersGrid.AutoGenerateColumns = false;
			this.customersGrid.ColumnHeadersVisible = false;
			this.customersGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colCompany,
            this.colPlace});
			this.customersGrid.DefaultRowHeight = 32;
			this.customersGrid.Location = new System.Drawing.Point(18, 132);
			this.customersGrid.MultiSelect = false;
			this.customersGrid.Name = "customersGrid";
			this.customersGrid.ReadOnly = true;
			this.customersGrid.RowHeadersVisible = false;
			this.customersGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
			this.customersGrid.ShowColumnVisibilityMenu = false;
			this.customersGrid.Size = new System.Drawing.Size(342, 620);
			this.customersGrid.TabIndex = 4;
			this.customersGrid.CellClick += new Wisej.Web.DataGridViewCellEventHandler(this.customersGrid_CellClick);
			//
			// colCompany
			//
			this.colCompany.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
			this.colCompany.DataPropertyName = "Company";
			dataGridViewCellStyle1.Font = new System.Drawing.Font("@nw-body-strong", 13F, System.Drawing.GraphicsUnit.Pixel);
			this.colCompany.DefaultCellStyle = dataGridViewCellStyle1;
			this.colCompany.HeaderText = "Customer";
			this.colCompany.MinimumWidth = 120;
			this.colCompany.Name = "colCompany";
			//
			// colPlace
			//
			this.colPlace.DataPropertyName = "Place";
			dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromName("@nw-muted");
			this.colPlace.DefaultCellStyle = dataGridViewCellStyle2;
			this.colPlace.HeaderText = "City · Country";
			this.colPlace.Name = "colPlace";
			this.colPlace.Width = 140;
			//
			// countryChip
			//
			this.countryChip.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
			this.countryChip.Closable = true;
			this.countryChip.CloseText = "Show the customers of every country";
			this.countryChip.Location = new System.Drawing.Point(200, 96);
			this.countryChip.Name = "countryChip";
			this.countryChip.Size = new System.Drawing.Size(160, 28);
			this.countryChip.TabIndex = 3;
			this.countryChip.Text = "Country: UK";
			this.countryChip.Tone = Wisej.Web.ChipTone.Primary;
			this.countryChip.Visible = false;
			this.countryChip.CloseClick += new System.EventHandler(this.countryChip_CloseClick);
			//
			// listLabel
			//
			this.listLabel.AppearanceKey = "nw-overline";
			this.listLabel.Location = new System.Drawing.Point(18, 100);
			this.listLabel.Name = "listLabel";
			this.listLabel.Size = new System.Drawing.Size(170, 20);
			this.listLabel.TabIndex = 2;
			this.listLabel.Text = "CUSTOMERS";
			//
			// sourceLabel
			//
			this.sourceLabel.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.sourceLabel.AppearanceKey = "nw-subtitle";
			this.sourceLabel.Location = new System.Drawing.Point(18, 60);
			this.sourceLabel.Name = "sourceLabel";
			this.sourceLabel.Size = new System.Drawing.Size(342, 20);
			this.sourceLabel.TabIndex = 1;
			this.sourceLabel.Text = "Selected through a navigation target";
			this.sourceLabel.Visible = false;
			//
			// customerAvatar
			//
			this.customerAvatar.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.customerAvatar.AvatarSize = 40;
			this.customerAvatar.Label = "Select a customer";
			this.customerAvatar.Location = new System.Drawing.Point(18, 0);
			this.customerAvatar.Name = "customerAvatar";
			this.customerAvatar.Size = new System.Drawing.Size(342, 48);
			this.customerAvatar.SubLabel = "Click a marker or a row, or press Ctrl+K";
			this.customerAvatar.TabIndex = 0;
			this.customerAvatar.Text = "Customer";
			//
			// CustomersView
			//
			this.AppearanceKey = "nw-view";
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = Wisej.Web.AutoScaleMode.Font;
			this.Controls.Add(this.map);
			this.Controls.Add(this.detailPanel);
			this.Name = "CustomersView";
			this.Size = new System.Drawing.Size(1206, 752);
			this.Text = "Customers";
			this.detailPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.customersGrid)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.MapView map;
		private Wisej.Web.Panel detailPanel;
		private Wisej.Web.Label sourceLabel;
		private Wisej.Web.Avatar customerAvatar;
		private Wisej.Web.Label listLabel;
		private Wisej.Web.ChipLabel countryChip;
		private Wisej.Web.DataGridView customersGrid;
		private Wisej.Web.DataGridViewTextBoxColumn colCompany;
		private Wisej.Web.DataGridViewTextBoxColumn colPlace;
	}
}
