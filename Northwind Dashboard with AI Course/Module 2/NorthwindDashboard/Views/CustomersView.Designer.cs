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
			this.map = new Wisej.Web.MapView();
			this.detailPanel = new Wisej.Web.Panel();
			this.sourceLabel = new Wisej.Web.Label();
			this.customerAvatar = new Wisej.Web.Avatar();
			this.detailPanel.SuspendLayout();
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
			this.map.Size = new System.Drawing.Size(906, 752);
			this.map.TabIndex = 0;
			this.map.Zoom = 2;
			this.map.MarkerClick += new System.EventHandler<Wisej.Web.MapMarkerEventArgs>(this.map_MarkerClick);
			//
			// detailPanel
			//
			this.detailPanel.Controls.Add(this.sourceLabel);
			this.detailPanel.Controls.Add(this.customerAvatar);
			this.detailPanel.Dock = Wisej.Web.DockStyle.Right;
			this.detailPanel.Location = new System.Drawing.Point(906, 0);
			this.detailPanel.Name = "detailPanel";
			this.detailPanel.Size = new System.Drawing.Size(300, 752);
			this.detailPanel.TabIndex = 1;
			//
			// sourceLabel
			//
			this.sourceLabel.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.sourceLabel.AppearanceKey = "nw-subtitle";
			this.sourceLabel.Location = new System.Drawing.Point(18, 60);
			this.sourceLabel.Name = "sourceLabel";
			this.sourceLabel.Size = new System.Drawing.Size(282, 20);
			this.sourceLabel.TabIndex = 1;
			this.sourceLabel.Text = "Selected through CommandExecuted";
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
			this.customerAvatar.Size = new System.Drawing.Size(282, 48);
			this.customerAvatar.SubLabel = "Click a marker, or press Ctrl+K";
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
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.MapView map;
		private Wisej.Web.Panel detailPanel;
		private Wisej.Web.Label sourceLabel;
		private Wisej.Web.Avatar customerAvatar;
	}
}
