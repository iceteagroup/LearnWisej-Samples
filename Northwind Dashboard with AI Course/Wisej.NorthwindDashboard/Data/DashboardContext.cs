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

namespace Wisej.NorthwindDashboard.Data
{

	/// <summary>
	/// Dashboard areas reachable from the sidebar and the command palette.
	/// </summary>
	public enum DashboardArea
	{

		/// <summary>
		/// Headline metrics, revenue trend, top products and recent orders.
		/// </summary>
		Overview,

		/// <summary>
		/// Revenue analysis by category, country, product and period.
		/// </summary>
		Sales,

		/// <summary>
		/// Searchable and filterable order list.
		/// </summary>
		Orders,

		/// <summary>
		/// Open-order board, delivery calendar and lead-time timeline.
		/// </summary>
		Fulfillment,

		/// <summary>
		/// Product catalog, stock levels and reorder suggestions.
		/// </summary>
		Products,

		/// <summary>
		/// Customer map and account details.
		/// </summary>
		Customers,

		/// <summary>
		/// Organization chart and sales performance per employee.
		/// </summary>
		Team
	}

	/// <summary>
	/// Delivery state of an order on the as-of date.
	/// </summary>
	public enum OrderStatus
	{

		/// <summary>
		/// Not shipped and not yet due.
		/// </summary>
		Open,

		/// <summary>
		/// Not shipped and past its required date.
		/// </summary>
		Overdue,

		/// <summary>
		/// Shipped on or before its required date.
		/// </summary>
		Shipped,

		/// <summary>
		/// Shipped after its required date.
		/// </summary>
		ShippedLate
	}

	/// <summary>
	/// Warehouse step of an order on the fulfillment board.
	/// </summary>
	/// <remarks>
	/// Northwind records only order and shipping dates, so the steps before shipping are session workflow state.
	/// </remarks>
	public enum FulfillmentStage
	{

		/// <summary>
		/// Received, not yet picked.
		/// </summary>
		New,

		/// <summary>
		/// Items are being picked.
		/// </summary>
		Picking,

		/// <summary>
		/// Packed and waiting for the carrier.
		/// </summary>
		Packed,

		/// <summary>
		/// Handed to the carrier.
		/// </summary>
		Shipped
	}

	/// <summary>
	/// Stock position of a product.
	/// </summary>
	public enum StockStatus
	{

		/// <summary>
		/// Stock is above the reorder level.
		/// </summary>
		InStock,

		/// <summary>
		/// Stock is at or below the reorder level, but open purchase orders cover it.
		/// </summary>
		Incoming,

		/// <summary>
		/// Stock and open purchase orders are at or below the reorder level.
		/// </summary>
		Reorder,

		/// <summary>
		/// No units in stock.
		/// </summary>
		OutOfStock,

		/// <summary>
		/// The product is no longer sold.
		/// </summary>
		Discontinued
	}

	/// <summary>
	/// Request to show a dashboard area, optionally focused on a record.
	/// </summary>
	public sealed class NavigationRequestEventArgs : EventArgs
	{

		#region Constructors

		/// <summary>
		/// Creates the request.
		/// </summary>
		/// <param name="area">Area to show.</param>
		/// <param name="target">Optional record to focus, such as a <see cref="Customer"/> or <see cref="Category"/>.</param>
		public NavigationRequestEventArgs(DashboardArea area, object target)
		{
			Area = area;
			Target = target;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the area to show.
		/// </summary>
		public DashboardArea Area { get; }

		/// <summary>
		/// Gets the record to focus, or null.
		/// </summary>
		public object Target { get; }

		#endregion

	}

	/// <summary>
	/// Session activity shown in the overview timeline, such as shipping an order.
	/// </summary>
	public sealed class ActivityEntry
	{

		#region Constructors

		internal ActivityEntry(DateTime time, string title, string description, object target)
		{
			Time = time;
			Title = title;
			Description = description;
			Target = target;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the time of the activity.
		/// </summary>
		public DateTime Time { get; }

		/// <summary>
		/// Gets the title.
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// Gets the detail text.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Gets the affected record.
		/// </summary>
		public object Target { get; }

		#endregion

	}

	/// <summary>
	/// Per-session dashboard state: the reporting period, navigation, and the workflow changes a user makes.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The shared <see cref="NorthwindDatabase"/> is never modified. Shipping an order or placing a purchase order
	/// is recorded here, visible to every view of the same session, and discarded when the session ends.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// var context = new DashboardContext(NorthwindDatabase.Instance);
	/// context.PeriodChanged += (s, e) => RefreshCharts();
	/// context.SetPeriod(PeriodKind.Last90Days);
	/// </code>
	/// </example>
	public sealed class DashboardContext
	{

		#region Constructors

		/// <summary>
		/// Creates the context over the shared database, starting with the year-to-date period.
		/// </summary>
		/// <param name="database">Shared read-only database.</param>
		public DashboardContext(NorthwindDatabase database)
		{
			Database = database ?? throw new ArgumentNullException(nameof(database));
			Period = DashboardPeriod.Create(PeriodKind.YearToDate, database.AsOfDate, database.FirstOrderDate);
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs after the reporting period changes.
		/// </summary>
		public event EventHandler PeriodChanged;

		/// <summary>
		/// Occurs when a view asks the shell to show another area.
		/// </summary>
		public event EventHandler<NavigationRequestEventArgs> NavigationRequested;

		/// <summary>
		/// Occurs after an order's fulfillment stage or shipping state changes.
		/// </summary>
		public event EventHandler OrdersChanged;

		/// <summary>
		/// Occurs after a purchase order changes a product's units on order.
		/// </summary>
		public event EventHandler ProductsChanged;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the shared database.
		/// </summary>
		public NorthwindDatabase Database { get; }

		/// <summary>
		/// Gets the dashboard's current date: the last recorded order date.
		/// </summary>
		public DateTime AsOf => Database.AsOfDate;

		/// <summary>
		/// Gets the reporting period.
		/// </summary>
		public DashboardPeriod Period { get; private set; }

		/// <summary>
		/// Gets the session activity, newest first.
		/// </summary>
		public IReadOnlyList<ActivityEntry> Activity => activity;

		#endregion

		#region Methods

		/// <summary>
		/// Changes the reporting period and raises <see cref="PeriodChanged"/>.
		/// </summary>
		/// <param name="kind">Range to show.</param>
		public void SetPeriod(PeriodKind kind)
		{
			if (Period.Kind == kind)
				return;

			Period = DashboardPeriod.Create(kind, Database.AsOfDate, Database.FirstOrderDate);
			PeriodChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Asks the shell to show an area.
		/// </summary>
		/// <param name="area">Area to show.</param>
		/// <param name="target">Optional record to focus.</param>
		public void Navigate(DashboardArea area, object target = null) =>
			NavigationRequested?.Invoke(this, new NavigationRequestEventArgs(area, target));

		/// <summary>
		/// Returns the shipping date, including orders shipped during this session.
		/// </summary>
		/// <param name="order">Order to check.</param>
		public DateTime? ShippedDate(Order order) =>
			order.ShippedDate ?? (stages.TryGetValue(order.Id, out var stage) && stage == FulfillmentStage.Shipped ? AsOf : null);

		/// <summary>
		/// Returns the delivery state on the as-of date.
		/// </summary>
		/// <param name="order">Order to check.</param>
		public OrderStatus StatusOf(Order order)
		{
			if (ShippedDate(order) is DateTime shipped)
				return shipped > order.RequiredDate ? OrderStatus.ShippedLate : OrderStatus.Shipped;

			return order.RequiredDate < AsOf ? OrderStatus.Overdue : OrderStatus.Open;
		}

		/// <summary>
		/// Returns the warehouse step of an order.
		/// </summary>
		/// <param name="order">Order to check.</param>
		/// <remarks>
		/// Open orders start as New when placed within the last 4 days, Picking when placed within 10 days,
		/// and Packed otherwise. Changes made on the fulfillment board replace this default.
		/// </remarks>
		public FulfillmentStage StageOf(Order order)
		{
			if (order.ShippedDate != null)
				return FulfillmentStage.Shipped;

			if (stages.TryGetValue(order.Id, out var stage))
				return stage;

			int age = (AsOf - order.OrderDate).Days;
			return age < 4 ? FulfillmentStage.New : age < 10 ? FulfillmentStage.Picking : FulfillmentStage.Packed;
		}

		/// <summary>
		/// Moves an open order to another warehouse step; moving it to Shipped records the as-of date as its shipping date.
		/// </summary>
		/// <param name="order">Order to move.</param>
		/// <param name="stage">New step.</param>
		/// <exception cref="InvalidOperationException">The order was already shipped.</exception>
		public void SetStage(Order order, FulfillmentStage stage)
		{
			if (StageOf(order) == stage)
				return;

			if (ShippedDate(order) != null)
				throw new InvalidOperationException("Order " + order.Number + " has already shipped.");

			stages[order.Id] = stage;
			if (stage == FulfillmentStage.Shipped)
			{
				AddActivity(
					"Order " + order.Number + " shipped",
					order.Customer.Company + " · via " + order.Shipper.Company + " · " + order.Subtotal.ToString("C0"),
					order);
			}

			OrdersChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Returns the units on order, including purchase orders placed during this session.
		/// </summary>
		/// <param name="product">Product to check.</param>
		public int UnitsOnOrder(Product product) =>
			product.UnitsOnOrder + (purchases.TryGetValue(product.Id, out int units) ? units : 0);

		/// <summary>
		/// Returns the stock position of a product.
		/// </summary>
		/// <param name="product">Product to check.</param>
		public StockStatus StockStatusOf(Product product)
		{
			if (product.Discontinued)
				return StockStatus.Discontinued;

			if (product.UnitsInStock == 0)
				return StockStatus.OutOfStock;

			if (product.UnitsInStock > product.ReorderLevel)
				return StockStatus.InStock;

			return product.UnitsInStock + UnitsOnOrder(product) > product.ReorderLevel ? StockStatus.Incoming : StockStatus.Reorder;
		}

		/// <summary>
		/// Returns the suggested purchase quantity: enough to restore twice the reorder level, in multiples of 10.
		/// </summary>
		/// <param name="product">Product to reorder.</param>
		public int SuggestedOrderQuantity(Product product)
		{
			int target = Math.Max(product.ReorderLevel, 5) * 2;
			int missing = Math.Max(target - product.UnitsInStock - UnitsOnOrder(product), 10);
			return (missing + 9) / 10 * 10;
		}

		/// <summary>
		/// Records a purchase order for a product and raises <see cref="ProductsChanged"/>.
		/// </summary>
		/// <param name="product">Product to order.</param>
		/// <param name="units">Units to order; must be positive.</param>
		public void PlacePurchaseOrder(Product product, int units)
		{
			if (units <= 0)
				throw new ArgumentOutOfRangeException(nameof(units));

			purchases[product.Id] = (purchases.TryGetValue(product.Id, out int existing) ? existing : 0) + units;
			AddActivity(
				"Purchase order for " + product.Name,
				units + " units from " + product.Supplier.Company,
				product);

			ProductsChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Returns the orders that were open on the as-of date and are still open in this session.
		/// </summary>
		public IEnumerable<Order> OpenOrders() => Database.Orders.Where(o => ShippedDate(o) == null);

		#endregion

		#region Implementation

		private readonly Dictionary<int, FulfillmentStage> stages = new Dictionary<int, FulfillmentStage>();
		private readonly Dictionary<int, int> purchases = new Dictionary<int, int>();
		private readonly List<ActivityEntry> activity = new List<ActivityEntry>();

		private void AddActivity(string title, string description, object target) =>
			activity.Insert(0, new ActivityEntry(AsOf.Add(DateTime.Now.TimeOfDay), title, description, target));

		#endregion

	}
}
