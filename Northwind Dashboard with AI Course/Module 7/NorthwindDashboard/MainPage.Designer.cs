namespace NorthwindDashboard
{
	partial class MainPage
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainPage));
			Wisej.Web.Segment segment1 = new Wisej.Web.Segment();
			Wisej.Web.Segment segment2 = new Wisej.Web.Segment();
			Wisej.Web.Segment segment3 = new Wisej.Web.Segment();
			Wisej.Web.Segment segment4 = new Wisej.Web.Segment();
			Wisej.Web.Segment segment5 = new Wisej.Web.Segment();
			this.appBar = new Wisej.Web.Panel();
			this.versionLabel = new Wisej.Web.Label();
			this.brandLabel = new Wisej.Web.Label();
			this.menuButton = new Wisej.Web.Button();
			this.toolsPanel = new Wisej.Web.FlowLayoutPanel();
			this.userAvatar = new Wisej.Web.Avatar();
			this.searchButton = new Wisej.Web.Button();
			this.asOfLabel = new Wisej.Web.Label();
			this.statusBar = new Wisej.Web.Panel();
			this.statusLabel = new Wisej.Web.Label();
			this.sidebar = new Wisej.Web.Panel();
			this.teamButton = new Wisej.Web.Button();
			this.customersButton = new Wisej.Web.Button();
			this.productsButton = new Wisej.Web.Button();
			this.fulfillmentButton = new Wisej.Web.Button();
			this.ordersButton = new Wisej.Web.Button();
			this.salesButton = new Wisej.Web.Button();
			this.overviewButton = new Wisej.Web.Button();
			this.contentPanel = new Wisej.Web.Panel();
			this.viewHost = new Wisej.Web.Panel();
			this.headerPanel = new Wisej.Web.Panel();
			this.periodSelector = new Wisej.Web.SegmentedButton();
			this.headingLabel = new Wisej.Web.Label();
			this.commandPalette = new Wisej.Web.CommandPalette(this.components);
			this.appBar.SuspendLayout();
			this.toolsPanel.SuspendLayout();
			this.statusBar.SuspendLayout();
			this.sidebar.SuspendLayout();
			this.contentPanel.SuspendLayout();
			this.headerPanel.SuspendLayout();
			this.SuspendLayout();
			//
			// appBar
			//
			this.appBar.AppearanceKey = "nw-appbar";
			this.appBar.Controls.Add(this.versionLabel);
			this.appBar.Controls.Add(this.brandLabel);
			this.appBar.Controls.Add(this.menuButton);
			this.appBar.Controls.Add(this.toolsPanel);
			this.appBar.Dock = Wisej.Web.DockStyle.Top;
			this.appBar.Location = new System.Drawing.Point(0, 0);
			this.appBar.Name = "appBar";
			this.appBar.Size = new System.Drawing.Size(1400, 46);
			this.appBar.TabIndex = 0;
			//
			// versionLabel
			//
			this.versionLabel.AppearanceKey = "nw-appbar-text";
			this.versionLabel.Location = new System.Drawing.Point(214, 2);
			this.versionLabel.Name = "versionLabel";
			this.versionLabel.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("versionLabel.ResponsiveProfiles"))));
			this.versionLabel.Size = new System.Drawing.Size(120, 44);
			this.versionLabel.TabIndex = 1;
			this.versionLabel.Text = "Wisej.NET 4.2";
			this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			// brandLabel
			//
			this.brandLabel.AppearanceKey = "nw-appbar-title";
			this.brandLabel.Location = new System.Drawing.Point(18, 0);
			this.brandLabel.Name = "brandLabel";
			this.brandLabel.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("brandLabel.ResponsiveProfiles"))));
			this.brandLabel.Size = new System.Drawing.Size(196, 46);
			this.brandLabel.TabIndex = 0;
			this.brandLabel.Text = "Northwind Dashboard";
			this.brandLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			// menuButton
			//
			this.menuButton.AccessibleName = "Menu";
			this.menuButton.AppearanceKey = "nw-appbar-menu";
			this.menuButton.Display = Wisej.Web.Display.Icon;
			this.menuButton.Focusable = false;
			this.menuButton.ImageSource = "Assets/Icons/menu.svg";
			this.menuButton.Location = new System.Drawing.Point(8, 7);
			this.menuButton.Name = "menuButton";
			this.menuButton.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("menuButton.ResponsiveProfiles"))));
			this.menuButton.Size = new System.Drawing.Size(36, 32);
			this.menuButton.TabIndex = 3;
			this.menuButton.ToolTipText = "Menu";
			this.menuButton.Visible = false;
			this.menuButton.Click += new System.EventHandler(this.menuButton_Click);
			//
			// toolsPanel
			//
			this.toolsPanel.Controls.Add(this.userAvatar);
			this.toolsPanel.Controls.Add(this.searchButton);
			this.toolsPanel.Controls.Add(this.asOfLabel);
			this.toolsPanel.Dock = Wisej.Web.DockStyle.Right;
			this.toolsPanel.FlowDirection = Wisej.Web.FlowDirection.RightToLeft;
			this.toolsPanel.Location = new System.Drawing.Point(962, 0);
			this.toolsPanel.Name = "toolsPanel";
			this.toolsPanel.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("toolsPanel.ResponsiveProfiles"))));
			this.toolsPanel.Padding = new Wisej.Web.Padding(0, 0, 18, 0);
			this.toolsPanel.Size = new System.Drawing.Size(438, 46);
			this.toolsPanel.TabIndex = 2;
			this.toolsPanel.WrapContents = false;
			//
			// userAvatar
			//
			this.userAvatar.AccessibleName = "Andrew Fuller, Vice President, Sales";
			this.userAvatar.AvatarSize = 30;
			this.userAvatar.Location = new System.Drawing.Point(390, 8);
			this.userAvatar.Margin = new Wisej.Web.Padding(12, 8, 0, 0);
			this.userAvatar.Name = "userAvatar";
			this.userAvatar.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("userAvatar.ResponsiveProfiles"))));
			this.userAvatar.Size = new System.Drawing.Size(30, 30);
			this.userAvatar.TabIndex = 2;
			this.userAvatar.Text = "Andrew Fuller";
			this.userAvatar.ToolTipText = "Andrew Fuller · Vice President, Sales";
			this.userAvatar.Click += new System.EventHandler(this.userAvatar_Click);
			//
			// searchButton
			//
			this.searchButton.AppearanceKey = "nw-appbar-button";
			this.searchButton.Focusable = false;
			this.searchButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.searchButton.ImageSource = "Assets/Icons/search.svg";
			this.searchButton.Location = new System.Drawing.Point(226, 9);
			this.searchButton.Margin = new Wisej.Web.Padding(14, 9, 0, 0);
			this.searchButton.Name = "searchButton";
			this.searchButton.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("searchButton.ResponsiveProfiles"))));
			this.searchButton.Size = new System.Drawing.Size(152, 28);
			this.searchButton.TabIndex = 1;
			this.searchButton.Text = "Search…      Ctrl+K";
			this.searchButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.searchButton.TextImageRelation = Wisej.Web.TextImageRelation.ImageBeforeText;
			this.searchButton.ToolTipText = "Search pages, periods, customers, people, products and orders";
			this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
			//
			// asOfLabel
			//
			this.asOfLabel.AppearanceKey = "nw-appbar-text";
			this.asOfLabel.Location = new System.Drawing.Point(72, 0);
			this.asOfLabel.Margin = new Wisej.Web.Padding(0);
			this.asOfLabel.Name = "asOfLabel";
			this.asOfLabel.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("asOfLabel.ResponsiveProfiles"))));
			this.asOfLabel.Size = new System.Drawing.Size(140, 46);
			this.asOfLabel.TabIndex = 0;
			this.asOfLabel.Text = "As of 1998-05-06";
			this.asOfLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			//
			// statusBar
			//
			this.statusBar.AppearanceKey = "nw-status";
			this.statusBar.Controls.Add(this.statusLabel);
			this.statusBar.Dock = Wisej.Web.DockStyle.Bottom;
			this.statusBar.Location = new System.Drawing.Point(0, 860);
			this.statusBar.Name = "statusBar";
			this.statusBar.Padding = new Wisej.Web.Padding(18, 0, 18, 0);
			this.statusBar.Size = new System.Drawing.Size(1400, 40);
			this.statusBar.TabIndex = 3;
			//
			// statusLabel
			//
			this.statusLabel.AppearanceKey = "nw-status-text";
			this.statusLabel.AutoEllipsis = true;
			this.statusLabel.Dock = Wisej.Web.DockStyle.Fill;
			this.statusLabel.Location = new System.Drawing.Point(18, 0);
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(1364, 39);
			this.statusLabel.TabIndex = 0;
			this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			// sidebar
			//
			this.sidebar.AppearanceKey = "nw-sidebar";
			this.sidebar.Controls.Add(this.teamButton);
			this.sidebar.Controls.Add(this.customersButton);
			this.sidebar.Controls.Add(this.productsButton);
			this.sidebar.Controls.Add(this.fulfillmentButton);
			this.sidebar.Controls.Add(this.ordersButton);
			this.sidebar.Controls.Add(this.salesButton);
			this.sidebar.Controls.Add(this.overviewButton);
			this.sidebar.Dock = Wisej.Web.DockStyle.Left;
			this.sidebar.Location = new System.Drawing.Point(0, 46);
			this.sidebar.Name = "sidebar";
			this.sidebar.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("sidebar.ResponsiveProfiles"))));
			this.sidebar.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("sidebar.ResponsiveProfiles1"))));
			this.sidebar.Size = new System.Drawing.Size(150, 814);
			this.sidebar.TabIndex = 1;
			//
			// teamButton
			//
			this.teamButton.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.teamButton.AppearanceKey = "nw-nav";
			this.teamButton.Display = Wisej.Web.Display.Label;
			this.teamButton.ImageSource = "Assets/Icons/team.svg";
			this.teamButton.Location = new System.Drawing.Point(10, 248);
			this.teamButton.Name = "teamButton";
			this.teamButton.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("teamButton.ResponsiveProfiles"))));
			this.teamButton.Size = new System.Drawing.Size(129, 36);
			this.teamButton.TabIndex = 6;
			this.teamButton.Text = "Team";
			this.teamButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.teamButton.Click += new System.EventHandler(this.navButton_Click);
			//
			// customersButton
			//
			this.customersButton.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.customersButton.AppearanceKey = "nw-nav";
			this.customersButton.Display = Wisej.Web.Display.Label;
			this.customersButton.ImageSource = "Assets/Icons/customers.svg";
			this.customersButton.Location = new System.Drawing.Point(10, 209);
			this.customersButton.Name = "customersButton";
			this.customersButton.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("customersButton.ResponsiveProfiles"))));
			this.customersButton.Size = new System.Drawing.Size(129, 36);
			this.customersButton.TabIndex = 5;
			this.customersButton.Text = "Customers";
			this.customersButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.customersButton.Click += new System.EventHandler(this.navButton_Click);
			//
			// productsButton
			//
			this.productsButton.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.productsButton.AppearanceKey = "nw-nav";
			this.productsButton.Display = Wisej.Web.Display.Label;
			this.productsButton.ImageSource = "Assets/Icons/products.svg";
			this.productsButton.Location = new System.Drawing.Point(10, 170);
			this.productsButton.Name = "productsButton";
			this.productsButton.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("productsButton.ResponsiveProfiles"))));
			this.productsButton.Size = new System.Drawing.Size(129, 36);
			this.productsButton.TabIndex = 4;
			this.productsButton.Text = "Products";
			this.productsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.productsButton.Click += new System.EventHandler(this.navButton_Click);
			//
			// fulfillmentButton
			//
			this.fulfillmentButton.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.fulfillmentButton.AppearanceKey = "nw-nav";
			this.fulfillmentButton.Display = Wisej.Web.Display.Label;
			this.fulfillmentButton.ImageSource = "Assets/Icons/fulfillment.svg";
			this.fulfillmentButton.Location = new System.Drawing.Point(10, 131);
			this.fulfillmentButton.Name = "fulfillmentButton";
			this.fulfillmentButton.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("fulfillmentButton.ResponsiveProfiles"))));
			this.fulfillmentButton.Size = new System.Drawing.Size(129, 36);
			this.fulfillmentButton.TabIndex = 3;
			this.fulfillmentButton.Text = "Fulfillment";
			this.fulfillmentButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.fulfillmentButton.Click += new System.EventHandler(this.navButton_Click);
			//
			// ordersButton
			//
			this.ordersButton.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.ordersButton.AppearanceKey = "nw-nav";
			this.ordersButton.Display = Wisej.Web.Display.Label;
			this.ordersButton.ImageSource = "Assets/Icons/orders.svg";
			this.ordersButton.Location = new System.Drawing.Point(10, 92);
			this.ordersButton.Name = "ordersButton";
			this.ordersButton.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("ordersButton.ResponsiveProfiles"))));
			this.ordersButton.Size = new System.Drawing.Size(129, 36);
			this.ordersButton.TabIndex = 2;
			this.ordersButton.Text = "Orders";
			this.ordersButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.ordersButton.Click += new System.EventHandler(this.navButton_Click);
			//
			// salesButton
			//
			this.salesButton.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.salesButton.AppearanceKey = "nw-nav";
			this.salesButton.Display = Wisej.Web.Display.Label;
			this.salesButton.ImageSource = "Assets/Icons/sales.svg";
			this.salesButton.Location = new System.Drawing.Point(10, 53);
			this.salesButton.Name = "salesButton";
			this.salesButton.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("salesButton.ResponsiveProfiles"))));
			this.salesButton.Size = new System.Drawing.Size(129, 36);
			this.salesButton.TabIndex = 1;
			this.salesButton.Text = "Sales";
			this.salesButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.salesButton.Click += new System.EventHandler(this.navButton_Click);
			//
			// overviewButton
			//
			this.overviewButton.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.overviewButton.AppearanceKey = "nw-nav-active";
			this.overviewButton.Display = Wisej.Web.Display.Label;
			this.overviewButton.ImageSource = "Assets/Icons/overview.svg";
			this.overviewButton.Location = new System.Drawing.Point(10, 14);
			this.overviewButton.Name = "overviewButton";
			this.overviewButton.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("overviewButton.ResponsiveProfiles"))));
			this.overviewButton.Size = new System.Drawing.Size(129, 36);
			this.overviewButton.TabIndex = 0;
			this.overviewButton.Text = "Overview";
			this.overviewButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.overviewButton.Click += new System.EventHandler(this.navButton_Click);
			//
			// contentPanel
			//
			this.contentPanel.AppearanceKey = "nw-content";
			this.contentPanel.Controls.Add(this.viewHost);
			this.contentPanel.Controls.Add(this.headerPanel);
			this.contentPanel.Dock = Wisej.Web.DockStyle.Fill;
			this.contentPanel.Location = new System.Drawing.Point(150, 46);
			this.contentPanel.Name = "contentPanel";
			this.contentPanel.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("contentPanel.ResponsiveProfiles"))));
			this.contentPanel.Padding = new Wisej.Web.Padding(22, 16, 22, 16);
			this.contentPanel.Size = new System.Drawing.Size(1250, 814);
			this.contentPanel.TabIndex = 2;
			//
			// viewHost
			//
			this.viewHost.AppearanceKey = "nw-view";
			this.viewHost.Dock = Wisej.Web.DockStyle.Fill;
			this.viewHost.Location = new System.Drawing.Point(22, 64);
			this.viewHost.Name = "viewHost";
			this.viewHost.Size = new System.Drawing.Size(1206, 734);
			this.viewHost.TabIndex = 1;
			//
			// headerPanel
			//
			this.headerPanel.AppearanceKey = "nw-view";
			this.headerPanel.Controls.Add(this.periodSelector);
			this.headerPanel.Controls.Add(this.headingLabel);
			this.headerPanel.Dock = Wisej.Web.DockStyle.Top;
			this.headerPanel.Location = new System.Drawing.Point(22, 16);
			this.headerPanel.Name = "headerPanel";
			this.headerPanel.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("headerPanel.ResponsiveProfiles"))));
			this.headerPanel.Size = new System.Drawing.Size(1206, 48);
			this.headerPanel.TabIndex = 0;
			//
			// periodSelector
			//
			this.periodSelector.AccessibleName = "Reporting period";
			this.periodSelector.Appearance = Wisej.Web.SegmentAppearance.Outlined;
			this.periodSelector.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
			segment1.Text = "30D";
			segment1.ToolTipText = "Last 30 days";
			segment1.Value = "Last30Days";
			segment2.Text = "90D";
			segment2.ToolTipText = "Last 90 days";
			segment2.Value = "Last90Days";
			segment3.Text = "YTD";
			segment3.ToolTipText = "Year to date";
			segment3.Value = "YearToDate";
			segment4.Text = "12M";
			segment4.ToolTipText = "Last 12 months";
			segment4.Value = "Last12Months";
			segment5.Text = "All";
			segment5.ToolTipText = "Complete order history";
			segment5.Value = "AllTime";
			this.periodSelector.Items.Add(segment1);
			this.periodSelector.Items.Add(segment2);
			this.periodSelector.Items.Add(segment3);
			this.periodSelector.Items.Add(segment4);
			this.periodSelector.Items.Add(segment5);
			this.periodSelector.Location = new System.Drawing.Point(978, 3);
			this.periodSelector.Name = "periodSelector";
			this.periodSelector.ResponsiveProfiles.Add(((Wisej.Base.ResponsiveProfile)(resources.GetObject("periodSelector.ResponsiveProfiles"))));
			this.periodSelector.SegmentSize = Wisej.Web.SegmentSize.Small;
			this.periodSelector.Size = new System.Drawing.Size(228, 28);
			this.periodSelector.TabIndex = 1;
			this.periodSelector.SelectionChanged += new System.EventHandler(this.periodSelector_SelectionChanged);
			//
			// headingLabel
			//
			this.headingLabel.AppearanceKey = "nw-heading";
			this.headingLabel.Location = new System.Drawing.Point(0, 0);
			this.headingLabel.Name = "headingLabel";
			this.headingLabel.Size = new System.Drawing.Size(600, 34);
			this.headingLabel.TabIndex = 0;
			this.headingLabel.Text = "Overview";
			this.headingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			// commandPalette
			//
			this.commandPalette.FilterMode = Wisej.Web.CommandFilterMode.Server;
			this.commandPalette.PlaceholderText = "Search pages, periods, customers, people, products or order numbers…";
			this.commandPalette.Shortcut = ((Wisej.Web.Keys)((Wisej.Web.Keys.Control | Wisej.Web.Keys.K)));
			this.commandPalette.Width = 600;
			this.commandPalette.QueryChanged += new System.EventHandler<Wisej.Web.CommandQueryEventArgs>(this.commandPalette_QueryChanged);
			this.commandPalette.CommandExecuted += new System.EventHandler<Wisej.Web.CommandEventArgs>(this.commandPalette_CommandExecuted);
			//
			// MainPage
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = Wisej.Web.AutoScaleMode.Font;
			this.Controls.Add(this.contentPanel);
			this.Controls.Add(this.sidebar);
			this.Controls.Add(this.statusBar);
			this.Controls.Add(this.appBar);
			this.Name = "MainPage";
			this.Size = new System.Drawing.Size(1400, 900);
			this.Text = "Northwind Dashboard";
			this.appBar.ResumeLayout(false);
			this.toolsPanel.ResumeLayout(false);
			this.statusBar.ResumeLayout(false);
			this.sidebar.ResumeLayout(false);
			this.contentPanel.ResumeLayout(false);
			this.headerPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.Panel appBar;
		private Wisej.Web.Label brandLabel;
		private Wisej.Web.Label versionLabel;
		private Wisej.Web.Button menuButton;
		private Wisej.Web.FlowLayoutPanel toolsPanel;
		private Wisej.Web.Label asOfLabel;
		private Wisej.Web.Button searchButton;
		private Wisej.Web.Avatar userAvatar;
		private Wisej.Web.Panel sidebar;
		private Wisej.Web.Button overviewButton;
		private Wisej.Web.Button salesButton;
		private Wisej.Web.Button ordersButton;
		private Wisej.Web.Button fulfillmentButton;
		private Wisej.Web.Button productsButton;
		private Wisej.Web.Button customersButton;
		private Wisej.Web.Button teamButton;
		private Wisej.Web.Panel contentPanel;
		private Wisej.Web.Panel headerPanel;
		private Wisej.Web.Label headingLabel;
		private Wisej.Web.SegmentedButton periodSelector;
		private Wisej.Web.Panel viewHost;
		private Wisej.Web.Panel statusBar;
		private Wisej.Web.Label statusLabel;
		private Wisej.Web.CommandPalette commandPalette;
	}
}
