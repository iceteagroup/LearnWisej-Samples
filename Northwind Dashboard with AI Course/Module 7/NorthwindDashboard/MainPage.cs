using System;
using System.Collections.Generic;
using System.Linq;
using NorthwindDashboard.Data;
using NorthwindDashboard.Dialogs;
using NorthwindDashboard.Views;
using Wisej.Web;

namespace NorthwindDashboard
{

	/// <summary>
	/// Application shell: app bar, sidebar navigation, period selector, command palette (Ctrl+K), the view host and the
	/// status strip.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The page owns the session's one <see cref="DashboardContext"/> and hands it to every view. Views are created on
	/// first use from the area table and kept, so a view's selection survives navigation.
	/// </para>
	/// <para>
	/// The shell follows the client profiles of ClientProfiles.json. The designer holds each profile's property values
	/// (MainPage.resx): the Tablet profile turns the sidebar into an icon rail, the Phone profile hides it behind the
	/// menu button and moves the period selector under the heading. <see cref="OnResponsiveProfileChanged"/> adds
	/// what a property cannot express, and each view's cards are reflowed by a <see cref="CardLayout"/>.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// Application.MainPage = new MainPage();
	/// </code>
	/// </example>
	public partial class MainPage : Page
	{

		#region Constructors

		/// <summary>
		/// Creates the shell and shows the overview for the last 30 days.
		/// </summary>
		public MainPage()
		{
			InitializeComponent();

			navigation = new Dictionary<DashboardArea, Button>
			{
				[DashboardArea.Overview] = overviewButton,
				[DashboardArea.Sales] = salesButton,
				[DashboardArea.Orders] = ordersButton,
				[DashboardArea.Fulfillment] = fulfillmentButton,
				[DashboardArea.Products] = productsButton,
				[DashboardArea.Customers] = customersButton,
				[DashboardArea.Team] = teamButton
			};
			foreach (var pair in navigation)
				pair.Value.Tag = pair.Key;

			dashboard = new DashboardContext(NorthwindDatabase.Instance);
			dashboard.NavigationRequested += dashboard_NavigationRequested;
			dashboard.PeriodChanged += dashboard_PeriodChanged;

			asOfLabel.Text = "As of " + dashboard.AsOf.ToString("yyyy-MM-dd");

			BuildCommands();
			Navigate(DashboardArea.Overview);

			// Raises SelectionChanged, which moves the context to the same period: one route for every period change.
			periodSelector.SelectedIndex = (int)PeriodKind.Last30Days;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Shows an area and, when a record is given, brings it into focus there.
		/// </summary>
		/// <param name="area">Area to show.</param>
		/// <param name="target">Record handed to the view's <see cref="DashboardView.ShowTarget"/>, or null.</param>
		/// <remarks>
		/// A view may write the status strip when it opens. A navigation that carries a record keeps the text the
		/// sender wrote: the event path that led here says more than the view's opening line.
		/// </remarks>
		public void Navigate(DashboardArea area, object target = null)
		{
			string senderStatus = statusLabel.Text;

			// A view may also post its opening line to the end of the request. With a record, that line is ignored
			// too: the two callbacks below close and reopen the status strip around it (see SetStatus).
			if (target != null)
				Application.Post(() => senderKeepsStatus = true);

			try
			{
				var view = GetView(area);
				foreach (var other in views.Values)
				{
					if (other != view)
						other.Visible = false;
				}

				view.Visible = true;
				current = view;

				foreach (var pair in navigation)
					pair.Value.AppearanceKey = pair.Key == area ? "nw-nav-active" : "nw-nav";

				UpdateHeader();

				if (target != null)
				{
					SetStatus(senderStatus);
					view.ShowTarget(target);
				}
			}
			finally
			{
				if (target != null)
					Application.Post(() => senderKeepsStatus = false);
			}
		}

		/// <summary>
		/// Writes the event path of the last interaction to the status strip.
		/// </summary>
		/// <param name="text">One line, such as "CommandExecuted → Tag → Customers".</param>
		/// <remarks>
		/// While a navigation that a view asked for, or one that carries a record, is being completed, the opening line
		/// the target view posts to the end of the request is ignored: the sender's event path is the line that stays.
		/// </remarks>
		public void SetStatus(string text)
		{
			if (!senderKeepsStatus)
				statusLabel.Text = text;
		}

		/// <summary>
		/// Completes what the responsive properties in the designer cannot express.
		/// </summary>
		/// <param name="e">Event data.</param>
		/// <remarks>
		/// Wisej raises the event after it has applied the profile's property values, when the page is created under a
		/// profile other than Default and whenever the browser crosses into another profile. Both additions are
		/// assigned for every profile, so leaving a narrow profile restores them: tooltips name the navigation icons
		/// while they have no text, and the command palette is never wider than the browser. The status strip names the
		/// profile that was applied.
		/// </remarks>
		protected override void OnResponsiveProfileChanged(ResponsiveProfileChangedEventArgs e)
		{
			base.OnResponsiveProfileChanged(e);

			string profile = e.CurrentProfile.Name;

			bool iconRail = profile == "Tablet";
			foreach (var pair in navigation)
				pair.Value.ToolTipText = iconRail ? pair.Key.ToString() : null;

			commandPalette.Width = profile == "Phone"
				? Math.Min(PaletteWidth, Application.Browser.Size.Width - 24)
				: PaletteWidth;

			SetStatus(profile + " profile · " + profile switch
			{
				"Phone" => "sidebar collapsed · cards stacked",
				"Tablet" => "sidebar as icons · cards full width",
				_ => "designed layout restored"
			});
		}

		#endregion

		#region Implementation

		// Record results per search, so a broad query cannot fill the palette.
		private const int MaxRecords = 6;

		// Designed width of the command palette; the Phone profile narrows it to the browser.
		private const int PaletteWidth = 600;

		private readonly DashboardContext dashboard;
		private readonly Dictionary<DashboardArea, Button> navigation;
		private readonly Dictionary<DashboardArea, DashboardView> views = new Dictionary<DashboardArea, DashboardView>();
		private readonly Dictionary<DashboardView, CardLayout> cardLayouts = new Dictionary<DashboardView, CardLayout>();
		private readonly List<PaletteCommand> pageCommands = new List<PaletteCommand>();
		private readonly List<PaletteCommand> periodCommands = new List<PaletteCommand>();
		private readonly List<PaletteCommand> customerCommands = new List<PaletteCommand>();
		private readonly List<PaletteCommand> employeeCommands = new List<PaletteCommand>();
		private readonly List<PaletteCommand> productCommands = new List<PaletteCommand>();
		private readonly Dictionary<int, PaletteCommand> orderCommands = new Dictionary<int, PaletteCommand>();
		private DashboardView current;
		private bool senderKeepsStatus;
		private ContextMenu areaMenu;

		// Views are created on first use and kept, so selections and scroll positions survive navigation.
		private DashboardView GetView(DashboardArea area)
		{
			if (views.TryGetValue(area, out var view))
				return view;

			view = areas[area].Create(area);

			// Recorded while the view still has its designed layout; null when the view has no cards to reflow.
			var cards = CardLayout.For(view);
			if (cards != null)
				cardLayouts.Add(view, cards);

			view.Dock = DockStyle.Fill;
			view.Visible = false;
			view.ResponsiveProfileChanged += view_ResponsiveProfileChanged;
			viewHost.Controls.Add(view);
			view.Shell = this;
			view.Dashboard = dashboard;
			views.Add(area, view);

			// A view created while the browser is narrow starts in that profile's arrangement.
			cards?.Apply(Application.ActiveProfile?.Name ?? "Default");
			return view;
		}

		private void UpdateHeader()
		{
			headingLabel.Text = current.Title;
			periodSelector.Visible = current.UsesPeriod;
			Text = "Northwind Dashboard · " + current.Title;
		}

		private void BuildCommands()
		{
			var db = dashboard.Database;
			foreach (var area in Enum.GetValues<DashboardArea>())
			{
				pageCommands.Add(new PaletteCommand
				{
					Text = "Go to " + area,
					Description = AreaDescription(area),
					Category = "Pages",
					Icon = DashboardStyle.AreaIcon(area),
					Tag = area
				});
			}

			foreach (var kind in Enum.GetValues<PeriodKind>())
			{
				var period = DashboardPeriod.Create(kind, db.AsOfDate, db.FirstOrderDate);
				periodCommands.Add(new PaletteCommand
				{
					Text = "Period: " + period.Name,
					Description = period.RangeText,
					Category = "Reporting period",
					Icon = DashboardStyle.Icon("calendar"),
					Keywords = "show range date filter",
					Tag = kind
				});
			}

			// The Tag is the record itself: the handler never parses a caption to find out which customer was meant.
			customerCommands.AddRange(db.Customers.Select(c => new PaletteCommand
			{
				Text = "Open customer " + c.Company,
				Description = c.Contact + " · " + c.Place,
				Category = "Customers",
				Icon = DashboardStyle.Icon("customers"),
				Keywords = c.Id + " " + c.ContactTitle,
				Tag = c
			}));

			employeeCommands.AddRange(db.Employees.Select(e => new PaletteCommand
			{
				Text = "Open employee " + e.FullName,
				Description = e.Title + " · " + e.City,
				Category = "Team",
				Icon = DashboardStyle.Icon("team"),
				Tag = e
			}));

			productCommands.AddRange(db.Products.Select(p => new PaletteCommand
			{
				Text = "Open product " + p.Name,
				Description = p.Category.Name + " · " + p.Supplier.Company + " · " + DashboardStyle.MoneyCents(p.UnitPrice),
				Category = "Products",
				Icon = DashboardStyle.Icon("products"),
				Keywords = p.QuantityPerUnit,
				Tag = p
			}));
		}

		// Server-side search: every word must match. Matching records come first, then the pages and periods.
		private IEnumerable<PaletteCommand> Search(string query)
		{
			var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			bool Matches(PaletteCommand command) => words.All(word =>
				(command.Text + " " + command.Description + " " + command.Keywords).Contains(word, StringComparison.CurrentCultureIgnoreCase));

			var customers = customerCommands.Where(Matches).Take(MaxRecords).ToList();
			var employees = employeeCommands.Where(Matches).Take(MaxRecords).ToList();
			var products = productCommands.Where(Matches).Take(MaxRecords).ToList();
			var commands = pageCommands.Where(Matches).Concat(periodCommands.Where(Matches)).ToList();

			// Digits also find orders by number, with or without the leading #.
			string number = query.Trim().TrimStart('#');
			var orders = number.Length >= 2 && number.All(char.IsDigit)
				? dashboard.Database.Orders.Where(o => o.Id.ToString().StartsWith(number, StringComparison.Ordinal)).Take(MaxRecords).Select(OrderCommand).ToList()
				: new List<PaletteCommand>();

			// A record result is always followed by the page that lists such records.
			AddPage(commands, DashboardArea.Products, products.Count > 0);
			AddPage(commands, DashboardArea.Team, employees.Count > 0);
			AddPage(commands, DashboardArea.Customers, customers.Count > 0);

			return orders.Concat(customers).Concat(employees).Concat(products).Concat(commands);
		}

		// One command object per order, created on first use: the Tag is the order itself.
		private PaletteCommand OrderCommand(Order order)
		{
			if (!orderCommands.TryGetValue(order.Id, out var command))
			{
				command = new PaletteCommand
				{
					Text = "Open order " + order.Number,
					Description = order.Customer.Company + " · " + DashboardStyle.Date(order.OrderDate) + " · " + DashboardStyle.MoneyCents(order.Subtotal),
					Category = "Orders",
					Icon = DashboardStyle.Icon("orders"),
					Tag = order
				};
				orderCommands.Add(order.Id, command);
			}

			return command;
		}

		private void AddPage(List<PaletteCommand> commands, DashboardArea area, bool wanted)
		{
			var page = pageCommands.First(c => area.Equals(c.Tag));
			if (wanted && !commands.Contains(page))
				commands.Insert(0, page);
		}

		private static string AreaDescription(DashboardArea area) => area switch
		{
			DashboardArea.Sales => "Revenue by category, country and product",
			DashboardArea.Orders => "Search and filter every order",
			DashboardArea.Fulfillment => "Open orders, delivery calendar and lead times",
			DashboardArea.Products => "Catalog, stock levels and reorder suggestions",
			DashboardArea.Customers => "Customer map and account history",
			DashboardArea.Team => "Organization chart and sales performance",
			_ => "Headline metrics and recent activity"
		};

		// The shell writes its event path first, so a view that describes itself when it opens has the last word.
		private void navButton_Click(object sender, EventArgs e)
		{
			var area = (DashboardArea)((Button)sender).Tag;
			SetStatus("Button.Click → Tag → Navigate(" + area + ")");
			Navigate(area);
		}

		// The Phone profile hides the sidebar and shows the menu button; the menu lists the same areas.
		private void menuButton_Click(object sender, EventArgs e)
		{
			if (areaMenu == null)
			{
				areaMenu = new ContextMenu(components);
				foreach (var area in Enum.GetValues<DashboardArea>())
					areaMenu.MenuItems.Add(new MenuItem(area.ToString()) { IconSource = DashboardStyle.AreaIcon(area), Tag = area });

				areaMenu.MenuItemClicked += areaMenu_MenuItemClicked;
			}

			areaMenu.Show(menuButton, Placement.BottomLeft);
		}

		private void areaMenu_MenuItemClicked(object sender, MenuItemEventArgs e)
		{
			var area = (DashboardArea)e.MenuItem.Tag;
			SetStatus("ContextMenu.MenuItemClicked → Tag → Navigate(" + area + ")");
			Navigate(area);
		}

		// Raised for a view after Wisej has applied the profile's property values to the view's own controls.
		private void view_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
		{
			if (cardLayouts.TryGetValue((DashboardView)sender, out var cards))
				cards.Apply(e.CurrentProfile.Name);
		}

		// The segments are in PeriodKind order, so the selected index is the period kind.
		private void periodSelector_SelectionChanged(object sender, EventArgs e)
		{
			if (dashboard != null && periodSelector.SelectedIndex >= 0)
			{
				var kind = (PeriodKind)periodSelector.SelectedIndex;
				dashboard.SetPeriod(kind);
				SetStatus("SegmentedButton.SelectionChanged → context.SetPeriod(" + kind + ")");
			}
		}

		private void searchButton_Click(object sender, EventArgs e) => commandPalette.Show();

		private void userAvatar_Click(object sender, EventArgs e)
		{
			SetStatus("Avatar.Click → Navigate(Team, employee)");
			Navigate(DashboardArea.Team, dashboard.Database.Employees.First(emp => emp.ReportsTo == null));
		}

		private void commandPalette_QueryChanged(object sender, CommandQueryEventArgs e)
		{
			string query = e.Query.Trim();
			e.Results = query.Length == 0 ? pageCommands.Concat(periodCommands).ToArray() : Search(query).ToArray();
			SetStatus("CommandPalette.QueryChanged → e.Results");
		}

		private void commandPalette_CommandExecuted(object sender, CommandEventArgs e)
		{
			switch (e.Command.Tag)
			{
				case DashboardArea area:
					SetStatus("CommandExecuted → Tag → " + area);
					Navigate(area);
					break;

				case PeriodKind kind:
					periodSelector.SelectedIndex = (int)kind;
					SetStatus("CommandExecuted → Tag → periodSelector.SelectedIndex → context.SetPeriod(" + kind + ")");
					break;

				case Customer customer:
					SetStatus("CommandExecuted → Tag → " + DashboardArea.Customers);
					Navigate(DashboardArea.Customers, customer);
					break;

				case Employee employee:
					SetStatus("CommandExecuted → Tag → " + DashboardArea.Team);
					Navigate(DashboardArea.Team, employee);
					break;

				case Product product:
					SetStatus("CommandExecuted → Tag → " + DashboardArea.Products);
					Navigate(DashboardArea.Products, product);
					break;

				case Order order:
					SetStatus("CommandExecuted → Tag → OrderDialog");
					OrderDialog.ShowOrder(dashboard, order);
					break;
			}
		}

		// Views navigate through the context, so they need no reference to the page's navigation code. The view that
		// asked writes its event path in its own handler; the view that opens may post an opening line to the end of
		// the request. Posted callbacks run in the order they were posted, so the first one below closes the status
		// strip to that opening line and the second one opens it again.
		private void dashboard_NavigationRequested(object sender, NavigationRequestEventArgs e)
		{
			Application.Post(() => senderKeepsStatus = true);
			try
			{
				Navigate(e.Area, e.Target);
			}
			finally
			{
				Application.Post(() => senderKeepsStatus = false);
			}
		}

		private void dashboard_PeriodChanged(object sender, EventArgs e)
		{
			if (periodSelector.SelectedIndex != (int)dashboard.Period.Kind)
				periodSelector.SelectedIndex = (int)dashboard.Period.Kind;
		}

		/// <summary>
		/// One row of the area table: how to create the view of an area.
		/// </summary>
		private sealed class AreaEntry
		{
			private Func<DashboardArea, DashboardView> create;

			/// <summary>
			/// The area is shown by a view of type <typeparamref name="T"/>.
			/// </summary>
			public static AreaEntry View<T>() where T : DashboardView, new() =>
				new AreaEntry { create = area => new T() };

			/// <summary>
			/// The area is not built yet; a <see cref="PlaceholderView"/> names the module that builds it.
			/// </summary>
			public static AreaEntry BuiltIn(int module) =>
				new AreaEntry { create = area => new PlaceholderView(area, module) };

			public DashboardView Create(DashboardArea area) => create(area);
		}

		#endregion
	}
}
