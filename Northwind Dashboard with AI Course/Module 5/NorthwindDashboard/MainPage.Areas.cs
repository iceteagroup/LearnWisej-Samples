using System.Collections.Generic;
using NorthwindDashboard.Data;
using NorthwindDashboard.Views;

namespace NorthwindDashboard
{

	public partial class MainPage
	{

		// The one table that says what each area shows: a view type, or the module that builds the view.
		// A later module replaces an entry with AreaEntry.View<T>() and adds the view's files; nothing else changes.
		private static readonly Dictionary<DashboardArea, AreaEntry> areas = new Dictionary<DashboardArea, AreaEntry>
		{
			[DashboardArea.Overview] = AreaEntry.View<OverviewView>(),
			[DashboardArea.Sales] = AreaEntry.View<SalesView>(),
			[DashboardArea.Orders] = AreaEntry.View<OrdersView>(),
			[DashboardArea.Fulfillment] = AreaEntry.View<FulfillmentView>(),
			[DashboardArea.Products] = AreaEntry.View<ProductsView>(),
			[DashboardArea.Customers] = AreaEntry.View<CustomersView>(),
			[DashboardArea.Team] = AreaEntry.BuiltIn(6)
		};

	}
}
