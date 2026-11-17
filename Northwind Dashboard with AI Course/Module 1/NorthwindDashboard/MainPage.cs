using System;
using System.Collections.Generic;
using System.Linq;
using NorthwindDashboard.Data;
using NorthwindDashboard.Views;
using Wisej.Web;

namespace NorthwindDashboard
{

	/// <summary>
	/// Application shell: app bar, sidebar navigation, period selector, command palette (Ctrl+K), the view host and the
	/// status strip.
	/// </summary>
	/// <remarks>
	/// The page owns the session's one <see cref="DashboardContext"/> and hands it to every view. Views are created on
	/// first use from the area table and kept, so a view's selection survives navigation.
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
		public void Navigate(DashboardArea area, object target = null)
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
				view.ShowTarget(target);
		}

		/// <summary>
		/// Writes the event path of the last interaction to the status strip.
		/// </summary>
		/// <param name="text">One line, such as "CommandExecuted → Tag → Customers".</param>
		/// <remarks>
		/// While a navigation that a view asked for is being completed, the opening line the target view posts to the
		/// end of the request is ignored: the event path of the view that asked is the line that stays.
		/// </remarks>
		public void SetStatus(string text)
		{
			if (!senderKeepsStatus)
				statusLabel.Text = text;
		}

		#endregion

		#region Implementation

		// Record results per search, so a broad query cannot fill the palette.
		private const int MaxRecords = 6;

		private readonly DashboardContext dashboard;
		private readonly Dictionary<DashboardArea, Button> navigation;
		private readonly Dictionary<DashboardArea, DashboardView> views = new Dictionary<DashboardArea, DashboardView>();
		private readonly List<PaletteCommand> pageCommands = new List<PaletteCommand>();
		private readonly List<PaletteCommand> periodCommands = new List<PaletteCommand>();
		private readonly List<PaletteCommand> customerCommands = new List<PaletteCommand>();
		private DashboardView current;
		private bool senderKeepsStatus;

		// Views are created on first use and kept, so selections and scroll positions survive navigation.
		private DashboardView GetView(DashboardArea area)
		{
			if (views.TryGetValue(area, out var view))
				return view;

			view = areas[area].Create(area);
			view.Dock = DockStyle.Fill;
			view.Visible = false;
			viewHost.Controls.Add(view);
			view.Shell = this;
			view.Dashboard = dashboard;
			views.Add(area, view);
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
		}

		// Server-side search: every word must match. Matching customers come first, then the pages and periods.
		private IEnumerable<PaletteCommand> Search(string query)
		{
			var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			bool Matches(PaletteCommand command) => words.All(word =>
				(command.Text + " " + command.Description + " " + command.Keywords).Contains(word, StringComparison.CurrentCultureIgnoreCase));

			var customers = customerCommands.Where(Matches).Take(MaxRecords).ToList();
			var commands = pageCommands.Where(Matches).Concat(periodCommands.Where(Matches)).ToList();

			// A customer result is always followed by the page that lists the customers.
			var customersPage = pageCommands.First(c => DashboardArea.Customers.Equals(c.Tag));
			if (customers.Count > 0 && !commands.Contains(customersPage))
				commands.Insert(0, customersPage);

			return customers.Concat(commands);
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

		private void navButton_Click(object sender, EventArgs e)
		{
			var area = (DashboardArea)((Button)sender).Tag;
			Navigate(area);
			SetStatus("Button.Click → Tag → Navigate(" + area + ")");
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

		private void userAvatar_Click(object sender, EventArgs e) =>
			Navigate(DashboardArea.Team, dashboard.Database.Employees.First(emp => emp.ReportsTo == null));

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
					Navigate(area);
					SetStatus("CommandExecuted → Tag → " + area);
					break;

				case PeriodKind kind:
					periodSelector.SelectedIndex = (int)kind;
					SetStatus("CommandExecuted → Tag → periodSelector.SelectedIndex → context.SetPeriod(" + kind + ")");
					break;

				case Customer customer:
					Navigate(DashboardArea.Customers, customer);
					SetStatus("CommandExecuted → Tag → " + DashboardArea.Customers);
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
