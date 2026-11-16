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

using System;
using System.Collections.Generic;
using System.Linq;
using Wisej.NorthwindDashboard.Data;
using Wisej.NorthwindDashboard.Dialogs;
using Wisej.NorthwindDashboard.Views;
using Wisej.Web;

namespace Wisej.NorthwindDashboard
{

	/// <summary>
	/// Application shell with sidebar navigation, the period selector, global search (Ctrl+K) and the dashboard views.
	/// </summary>
	/// <example>
	/// <code>
	/// Application.MainPage = new MainPage();
	/// </code>
	/// </example>
	public partial class MainPage : Page
	{

		#region Constructors

		/// <summary>
		/// Creates the shell and shows the overview.
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

			sourceCaption.Text = "Orders " + DashboardStyle.Date(dashboard.Database.FirstOrderDate) + Environment.NewLine +
				"to " + DashboardStyle.Date(dashboard.AsOf);
			periodSelector.SelectedIndex = (int)dashboard.Period.Kind;

			BuildCommands();
			ShowArea(DashboardArea.Overview);
		}

		#endregion

		#region Implementation

		private readonly DashboardContext dashboard;
		private readonly Dictionary<DashboardArea, Button> navigation;
		private readonly Dictionary<DashboardArea, DashboardView> views = new Dictionary<DashboardArea, DashboardView>();
		private readonly List<PaletteCommand> pageCommands = new List<PaletteCommand>();
		private readonly List<PaletteCommand> periodCommands = new List<PaletteCommand>();
		private readonly List<PaletteCommand> recordCommands = new List<PaletteCommand>();
		private readonly Dictionary<int, PaletteCommand> orderCommands = new Dictionary<int, PaletteCommand>();
		private DashboardView current;
		private ContextMenu areaMenu;

		private void ShowArea(DashboardArea area, object target = null)
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

		// Views are created on first use and kept, so selections and scroll positions survive navigation.
		private DashboardView GetView(DashboardArea area)
		{
			if (views.TryGetValue(area, out var view))
				return view;

			view = area switch
			{
				DashboardArea.Sales => new SalesView(),
				DashboardArea.Orders => new OrdersView(),
				DashboardArea.Fulfillment => new FulfillmentView(),
				DashboardArea.Products => new ProductsView(),
				DashboardArea.Customers => new CustomersView(),
				DashboardArea.Team => new TeamView(),
				_ => new OverviewView()
			};

			view.Dock = DockStyle.Fill;
			view.Visible = false;
			contentPanel.Controls.Add(view);
			view.Dashboard = dashboard;
			views.Add(area, view);
			return view;
		}

		private void UpdateHeader()
		{
			var period = dashboard.Period;
			titleLabel.Text = current.Title;
			subtitleLabel.Text = current.UsesPeriod
				? period.Name + " · " + period.RangeText + " · " + period.ComparisonText
				: "Snapshot as of " + DashboardStyle.Date(dashboard.AsOf) + ", the last recorded order date";
			periodSelector.Visible = current.UsesPeriod;
			Text = "Northwind · " + current.Title;
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
					Text = "Show " + period.Name.ToLowerInvariant(),
					Description = period.RangeText,
					Category = "Reporting period",
					Icon = DashboardStyle.Icon("calendar"),
					Keywords = "period range date filter",
					Tag = kind
				});
			}

			recordCommands.AddRange(db.Customers.Select(c => new PaletteCommand
			{
				Text = c.Company,
				Description = c.Contact + " · " + c.Place,
				Category = "Customers",
				Icon = DashboardStyle.Icon("customers"),
				Keywords = c.Id + " " + c.ContactTitle,
				Tag = c
			}));

			recordCommands.AddRange(db.Products.Select(p => new PaletteCommand
			{
				Text = p.Name,
				Description = p.Category.Name + " · " + p.Supplier.Company + " · " + DashboardStyle.MoneyCents(p.UnitPrice),
				Category = "Products",
				Icon = DashboardStyle.Icon("products"),
				Keywords = p.QuantityPerUnit,
				Tag = p
			}));

			recordCommands.AddRange(db.Employees.Select(e => new PaletteCommand
			{
				Text = e.FullName,
				Description = e.Title + " · " + e.City,
				Category = "Team",
				Icon = DashboardStyle.Icon("team"),
				Tag = e
			}));
		}

		// Server-side search: every word must match; numbers also find orders by number.
		private IEnumerable<PaletteCommand> Search(string query)
		{
			var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			bool Matches(PaletteCommand command) => words.All(word =>
				(command.Text + " " + command.Description + " " + command.Keywords).Contains(word, StringComparison.CurrentCultureIgnoreCase));

			foreach (var command in pageCommands.Where(Matches).Concat(periodCommands.Where(Matches)))
				yield return command;

			foreach (var group in recordCommands.Where(Matches).GroupBy(c => c.Category))
			{
				foreach (var command in group.Take(6))
					yield return command;
			}

			string number = query.TrimStart('#');
			if (number.Length >= 2 && number.All(char.IsDigit))
			{
				foreach (var order in dashboard.Database.Orders.Where(o => o.Id.ToString().StartsWith(number, StringComparison.Ordinal)).Take(6))
					yield return OrderCommand(order);
			}
		}

		// Reuses command objects so recent history recognizes the same order.
		private PaletteCommand OrderCommand(Order order)
		{
			if (!orderCommands.TryGetValue(order.Id, out var command))
			{
				command = new PaletteCommand
				{
					Text = "Order " + order.Number,
					Description = order.Customer.Company + " · " + DashboardStyle.Date(order.OrderDate) + " · " + DashboardStyle.MoneyCents(order.Subtotal),
					Category = "Orders",
					Icon = DashboardStyle.Icon("orders"),
					Tag = order
				};
				orderCommands.Add(order.Id, command);
			}

			return command;
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

		private void navButton_Click(object sender, EventArgs e) => ShowArea((DashboardArea)((Button)sender).Tag);

		// The Phone profile hides the sidebar and shows the menu button; the menu lists the same areas.
		private void menuButton_Click(object sender, EventArgs e)
		{
			if (areaMenu == null)
			{
				areaMenu = new ContextMenu(components);
				foreach (var area in Enum.GetValues<DashboardArea>())
					areaMenu.MenuItems.Add(new MenuItem(area.ToString()) { IconSource = DashboardStyle.AreaIcon(area), Tag = area });

				areaMenu.MenuItemClicked += (s, args) => ShowArea((DashboardArea)args.MenuItem.Tag);
			}

			areaMenu.Show(menuButton, Placement.BottomLeft);
		}

		/// <summary>
		/// Completes what the responsive properties in the designer cannot express.
		/// </summary>
		/// <param name="e">Event data.</param>
		/// <remarks>
		/// The designer sets each profile's layout (MainPage.resx): the Tablet profile turns the sidebar into an icon
		/// rail, and the Phone profile hides it behind the menu button. Tooltips name the icons while they have no text,
		/// and the command palette narrows to the screen.
		/// </remarks>
		protected override void OnResponsiveProfileChanged(ResponsiveProfileChangedEventArgs e)
		{
			base.OnResponsiveProfileChanged(e);

			bool iconRail = e.CurrentProfile.Name == "Tablet";
			foreach (var pair in navigation)
				pair.Value.ToolTipText = iconRail ? pair.Key.ToString() : null;

			commandPalette.Width = e.CurrentProfile.Name == "Phone" ? Math.Min(640, Application.Browser.Size.Width - 24) : 640;
		}

		private void periodSelector_SelectionChanged(object sender, EventArgs e)
		{
			if (dashboard != null && periodSelector.SelectedIndex >= 0)
				dashboard.SetPeriod((PeriodKind)periodSelector.SelectedIndex);
		}

		private void searchButton_Click(object sender, EventArgs e) => commandPalette.Show();

		private void userAvatar_Click(object sender, EventArgs e) =>
			ShowArea(DashboardArea.Team, dashboard.Database.Employees.First(emp => emp.ReportsTo == null));

		private void commandPalette_QueryChanged(object sender, CommandQueryEventArgs e)
		{
			string query = e.Query.Trim();
			e.Results = query.Length == 0 ? pageCommands.Concat(periodCommands).ToArray() : Search(query).ToArray();
		}

		private void commandPalette_CommandExecuted(object sender, CommandEventArgs e)
		{
			switch (e.Command.Tag)
			{
				case DashboardArea area:
					ShowArea(area);
					break;

				case PeriodKind kind:
					periodSelector.SelectedIndex = (int)kind;
					break;

				case Customer customer:
					ShowArea(DashboardArea.Customers, customer);
					break;

				case Product product:
					ShowArea(DashboardArea.Products, product);
					break;

				case Employee employee:
					ShowArea(DashboardArea.Team, employee);
					break;

				case Order order:
					OrderDialog.ShowOrder(dashboard, order);
					break;
			}
		}

		private void dashboard_NavigationRequested(object sender, NavigationRequestEventArgs e) => ShowArea(e.Area, e.Target);

		private void dashboard_PeriodChanged(object sender, EventArgs e)
		{
			if (periodSelector.SelectedIndex != (int)dashboard.Period.Kind)
				periodSelector.SelectedIndex = (int)dashboard.Period.Kind;

			UpdateHeader();
		}

		#endregion

	}
}
