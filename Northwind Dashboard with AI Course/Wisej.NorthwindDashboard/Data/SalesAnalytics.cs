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
	/// Headline metrics for a period and its comparison period.
	/// </summary>
	public sealed class PeriodSummary
	{

		#region Properties

		/// <summary>
		/// Gets the net merchandise revenue.
		/// </summary>
		public decimal Revenue { get; init; }

		/// <summary>
		/// Gets the revenue change in percent, or null without a comparison.
		/// </summary>
		public decimal? RevenueChange { get; init; }

		/// <summary>
		/// Gets the number of orders.
		/// </summary>
		public int Orders { get; init; }

		/// <summary>
		/// Gets the order count change in percent, or null without a comparison.
		/// </summary>
		public decimal? OrdersChange { get; init; }

		/// <summary>
		/// Gets the average order value.
		/// </summary>
		public decimal AverageOrder { get; init; }

		/// <summary>
		/// Gets the average order value change in percent, or null without a comparison.
		/// </summary>
		public decimal? AverageOrderChange { get; init; }

		/// <summary>
		/// Gets the number of distinct ordering customers.
		/// </summary>
		public int Customers { get; init; }

		/// <summary>
		/// Gets the number of units sold.
		/// </summary>
		public int Units { get; init; }

		/// <summary>
		/// Gets the revenue per bucket.
		/// </summary>
		public double[] RevenueTrend { get; init; }

		/// <summary>
		/// Gets the order count per bucket.
		/// </summary>
		public double[] OrdersTrend { get; init; }

		/// <summary>
		/// Gets the average order value per bucket.
		/// </summary>
		public double[] AverageOrderTrend { get; init; }

		/// <summary>
		/// Gets the number of period orders that have shipped.
		/// </summary>
		public int Shipped { get; init; }

		/// <summary>
		/// Gets the number of period orders that shipped after their required date.
		/// </summary>
		public int ShippedLate { get; init; }

		/// <summary>
		/// Gets the share of shipped orders that shipped on time, in percent.
		/// </summary>
		public double OnTimeRate => Shipped == 0 ? 0 : (Shipped - ShippedLate) * 100.0 / Shipped;

		/// <summary>
		/// Gets the on-time rate change in percentage points, or null without a comparison.
		/// </summary>
		public decimal? OnTimeChange { get; init; }

		/// <summary>
		/// Gets the average days between order and shipment.
		/// </summary>
		public double AverageDaysToShip { get; init; }

		#endregion

	}

	/// <summary>
	/// Revenue and orders of one bucket, with the aligned comparison bucket.
	/// </summary>
	public sealed class TrendPoint
	{

		#region Properties

		/// <summary>
		/// Gets the axis label.
		/// </summary>
		public string Label { get; init; }

		/// <summary>
		/// Gets the revenue.
		/// </summary>
		public double Revenue { get; init; }

		/// <summary>
		/// Gets the comparison revenue.
		/// </summary>
		public double PreviousRevenue { get; init; }

		/// <summary>
		/// Gets the number of orders.
		/// </summary>
		public int Orders { get; init; }

		/// <summary>
		/// Gets the average order value.
		/// </summary>
		public double AverageOrder { get; init; }

		/// <summary>
		/// Gets the bucket.
		/// </summary>
		public PeriodBucket Bucket { get; init; }

		#endregion

	}

	/// <summary>
	/// Sales of one category.
	/// </summary>
	public sealed class CategorySales
	{

		#region Properties

		/// <summary>
		/// Gets the category.
		/// </summary>
		public Category Category { get; init; }

		/// <summary>
		/// Gets the category name.
		/// </summary>
		public string Name => Category.Name;

		/// <summary>
		/// Gets the revenue.
		/// </summary>
		public decimal Revenue { get; init; }

		/// <summary>
		/// Gets the comparison revenue.
		/// </summary>
		public decimal PreviousRevenue { get; init; }

		/// <summary>
		/// Gets the revenue change in percent, or null without a comparison.
		/// </summary>
		public decimal? Change { get; init; }

		/// <summary>
		/// Gets the share of the period revenue, in percent.
		/// </summary>
		public double Share { get; init; }

		/// <summary>
		/// Gets the units sold.
		/// </summary>
		public int Units { get; init; }

		/// <summary>
		/// Gets the revenue per bucket.
		/// </summary>
		public double[] Trend { get; init; }

		#endregion

	}

	/// <summary>
	/// Sales of one product.
	/// </summary>
	public sealed class ProductSales
	{

		#region Properties

		/// <summary>
		/// Gets the product.
		/// </summary>
		public Product Product { get; init; }

		/// <summary>
		/// Gets the revenue.
		/// </summary>
		public decimal Revenue { get; init; }

		/// <summary>
		/// Gets the comparison revenue.
		/// </summary>
		public decimal PreviousRevenue { get; init; }

		/// <summary>
		/// Gets the revenue change in percent, or null without a comparison.
		/// </summary>
		public decimal? Change { get; init; }

		/// <summary>
		/// Gets the units sold.
		/// </summary>
		public int Units { get; init; }

		/// <summary>
		/// Gets the number of orders containing the product.
		/// </summary>
		public int Orders { get; init; }

		/// <summary>
		/// Gets the share of the period revenue, in percent.
		/// </summary>
		public double Share { get; init; }

		/// <summary>
		/// Gets the units sold per bucket.
		/// </summary>
		public double[] UnitsTrend { get; init; }

		#endregion

	}

	/// <summary>
	/// Sales to one country.
	/// </summary>
	public sealed class CountrySales
	{

		#region Properties

		/// <summary>
		/// Gets the Northwind country name.
		/// </summary>
		public string Country { get; init; }

		/// <summary>
		/// Gets the revenue.
		/// </summary>
		public decimal Revenue { get; init; }

		/// <summary>
		/// Gets the revenue change in percent, or null without a comparison.
		/// </summary>
		public decimal? Change { get; init; }

		/// <summary>
		/// Gets the number of orders.
		/// </summary>
		public int Orders { get; init; }

		/// <summary>
		/// Gets the number of ordering customers.
		/// </summary>
		public int Customers { get; init; }

		#endregion

	}

	/// <summary>
	/// Sales performance of one employee.
	/// </summary>
	public sealed class EmployeeSales
	{

		#region Properties

		/// <summary>
		/// Gets the employee.
		/// </summary>
		public Employee Employee { get; init; }

		/// <summary>
		/// Gets the revenue booked.
		/// </summary>
		public decimal Revenue { get; init; }

		/// <summary>
		/// Gets the revenue change in percent, or null without a comparison.
		/// </summary>
		public decimal? Change { get; init; }

		/// <summary>
		/// Gets the number of orders.
		/// </summary>
		public int Orders { get; init; }

		/// <summary>
		/// Gets the average order value.
		/// </summary>
		public decimal AverageOrder { get; init; }

		/// <summary>
		/// Gets the number of distinct customers served.
		/// </summary>
		public int Customers { get; init; }

		/// <summary>
		/// Gets the share of shipped orders delivered on time, in percent.
		/// </summary>
		public double OnTimeRate { get; init; }

		/// <summary>
		/// Gets the share of the team revenue, in percent.
		/// </summary>
		public double Share { get; init; }

		/// <summary>
		/// Gets the average discount granted, in percent of list value.
		/// </summary>
		public double AverageDiscount { get; init; }

		/// <summary>
		/// Gets the revenue per bucket.
		/// </summary>
		public double[] Trend { get; init; }

		#endregion

	}

	/// <summary>
	/// Sales to one customer.
	/// </summary>
	public sealed class CustomerSales
	{

		#region Properties

		/// <summary>
		/// Gets the customer.
		/// </summary>
		public Customer Customer { get; init; }

		/// <summary>
		/// Gets the revenue.
		/// </summary>
		public decimal Revenue { get; init; }

		/// <summary>
		/// Gets the revenue change in percent, or null without a comparison.
		/// </summary>
		public decimal? Change { get; init; }

		/// <summary>
		/// Gets the number of orders.
		/// </summary>
		public int Orders { get; init; }

		/// <summary>
		/// Gets the revenue per bucket.
		/// </summary>
		public double[] Trend { get; init; }

		#endregion

	}

	/// <summary>
	/// Delivery performance of one shipper.
	/// </summary>
	public sealed class ShipperPerformance
	{

		#region Properties

		/// <summary>
		/// Gets the shipper.
		/// </summary>
		public Shipper Shipper { get; init; }

		/// <summary>
		/// Gets the number of period orders assigned to the shipper.
		/// </summary>
		public int Orders { get; init; }

		/// <summary>
		/// Gets the number of those orders that have shipped.
		/// </summary>
		public int Shipped { get; init; }

		/// <summary>
		/// Gets the number of those orders that shipped late.
		/// </summary>
		public int Late { get; init; }

		/// <summary>
		/// Gets the on-time rate in percent.
		/// </summary>
		public double OnTimeRate => Shipped == 0 ? 0 : (Shipped - Late) * 100.0 / Shipped;

		/// <summary>
		/// Gets the freight charged.
		/// </summary>
		public decimal Freight { get; init; }

		/// <summary>
		/// Gets the average days between order and shipment.
		/// </summary>
		public double AverageDays { get; init; }

		/// <summary>
		/// Gets the number of orders still open for the shipper.
		/// </summary>
		public int OpenOrders { get; init; }

		#endregion

	}

	/// <summary>
	/// Value of one calendar day, for heatmaps.
	/// </summary>
	public sealed class DailyValue
	{

		#region Properties

		/// <summary>
		/// Gets the day.
		/// </summary>
		public DateTime Date { get; init; }

		/// <summary>
		/// Gets the value.
		/// </summary>
		public double Value { get; init; }

		#endregion

	}

	/// <summary>
	/// Aggregations used by the dashboard views.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Revenue is the net merchandise value of an order (discounted line totals, excluding freight), attributed
	/// to its order date. All methods read the shared database and the session's shipping changes from the context.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// var summary = SalesAnalytics.Summarize(context);
	/// kpiRevenue.Value = summary.Revenue.ToString("C0");
	/// kpiRevenue.Trend = summary.RevenueChange;
	/// </code>
	/// </example>
	public static class SalesAnalytics
	{

		#region Methods

		/// <summary>
		/// Computes the headline metrics of the context period.
		/// </summary>
		/// <param name="context">Session context.</param>
		public static PeriodSummary Summarize(DashboardContext context)
		{
			var period = context.Period;
			var current = context.Database.Orders.Where(o => period.Contains(o.OrderDate)).ToArray();
			var previous = context.Database.Orders.Where(o => period.InComparison(o.OrderDate)).ToArray();

			decimal revenue = current.Sum(o => o.Subtotal), previousRevenue = previous.Sum(o => o.Subtotal);
			decimal average = current.Length == 0 ? 0 : revenue / current.Length;
			decimal previousAverage = previous.Length == 0 ? 0 : previousRevenue / previous.Length;

			var revenueTrend = new double[period.Buckets.Count];
			var ordersTrend = new double[period.Buckets.Count];
			foreach (var order in current)
			{
				int bucket = period.BucketOf(order.OrderDate);
				revenueTrend[bucket] += (double)order.Subtotal;
				ordersTrend[bucket]++;
			}

			var shipped = current.Select(o => (Order: o, Shipped: context.ShippedDate(o))).Where(x => x.Shipped != null).ToArray();
			var previousShipped = previous.Select(o => (Order: o, Shipped: context.ShippedDate(o))).Where(x => x.Shipped != null).ToArray();
			double OnTime((Order Order, DateTime? Shipped)[] items) =>
				items.Length == 0 ? 0 : items.Count(x => x.Shipped <= x.Order.RequiredDate) * 100.0 / items.Length;

			return new PeriodSummary
			{
				Revenue = revenue,
				RevenueChange = period.Change(revenue, previousRevenue),
				Orders = current.Length,
				OrdersChange = period.Change(current.Length, previous.Length),
				AverageOrder = average,
				AverageOrderChange = period.Change(average, previousAverage),
				Customers = current.Select(o => o.CustomerId).Distinct().Count(),
				Units = current.Sum(o => o.Units),
				RevenueTrend = revenueTrend,
				OrdersTrend = ordersTrend,
				AverageOrderTrend = revenueTrend.Zip(ordersTrend, (r, n) => n == 0 ? 0 : Math.Round(r / n, 2)).ToArray(),
				Shipped = shipped.Length,
				ShippedLate = shipped.Count(x => x.Shipped > x.Order.RequiredDate),
				OnTimeChange = period.HasComparison && shipped.Length > 0 && previousShipped.Length > 0
					? Math.Round((decimal)(OnTime(shipped) - OnTime(previousShipped)), 1)
					: null,
				AverageDaysToShip = shipped.Length == 0 ? 0 : shipped.Average(x => (x.Shipped.Value - x.Order.OrderDate).TotalDays)
			};
		}

		/// <summary>
		/// Returns revenue and orders per bucket with the aligned comparison revenue.
		/// </summary>
		/// <param name="context">Session context.</param>
		public static IReadOnlyList<TrendPoint> RevenueTrend(DashboardContext context)
		{
			var period = context.Period;
			var revenue = new double[period.Buckets.Count];
			var previous = new double[period.Buckets.Count];
			var orders = new int[period.Buckets.Count];
			foreach (var order in context.Database.Orders)
			{
				int bucket = period.BucketOf(order.OrderDate);
				if (bucket >= 0)
				{
					revenue[bucket] += (double)order.Subtotal;
					orders[bucket]++;
				}
				else if ((bucket = period.PreviousBucketOf(order.OrderDate)) >= 0)
				{
					previous[bucket] += (double)order.Subtotal;
				}
			}

			return period.Buckets.Select(b => new TrendPoint
			{
				Label = b.Label,
				Revenue = Math.Round(revenue[b.Index], 2),
				PreviousRevenue = Math.Round(previous[b.Index], 2),
				Orders = orders[b.Index],
				AverageOrder = orders[b.Index] == 0 ? 0 : Math.Round(revenue[b.Index] / orders[b.Index], 2),
				Bucket = b
			}).ToArray();
		}

		/// <summary>
		/// Returns the sales of every category, ordered by category identifier.
		/// </summary>
		/// <param name="context">Session context.</param>
		public static IReadOnlyList<CategorySales> CategorySales(DashboardContext context)
		{
			var period = context.Period;
			var total = context.Database.Orders.Where(o => period.Contains(o.OrderDate)).Sum(o => o.Subtotal);
			return context.Database.Categories.Select(category =>
			{
				var lines = category.Products.SelectMany(p => p.Lines);
				var current = lines.Where(l => period.Contains(l.Order.OrderDate)).ToArray();
				decimal revenue = current.Sum(l => l.Total);
				decimal previous = lines.Where(l => period.InComparison(l.Order.OrderDate)).Sum(l => l.Total);
				var trend = new double[period.Buckets.Count];
				foreach (var line in current)
					trend[period.BucketOf(line.Order.OrderDate)] += (double)line.Total;

				return new CategorySales
				{
					Category = category,
					Revenue = revenue,
					PreviousRevenue = previous,
					Change = period.Change(revenue, previous),
					Share = total == 0 ? 0 : (double)(revenue / total * 100),
					Units = current.Sum(l => l.Quantity),
					Trend = trend.Select(v => Math.Round(v, 2)).ToArray()
				};
			}).ToArray();
		}

		/// <summary>
		/// Returns the sales of every product, highest revenue first.
		/// </summary>
		/// <param name="context">Session context.</param>
		public static IReadOnlyList<ProductSales> ProductSales(DashboardContext context)
		{
			var period = context.Period;
			var total = context.Database.Orders.Where(o => period.Contains(o.OrderDate)).Sum(o => o.Subtotal);
			return context.Database.Products.Select(product =>
			{
				var current = product.Lines.Where(l => period.Contains(l.Order.OrderDate)).ToArray();
				decimal revenue = current.Sum(l => l.Total);
				decimal previous = product.Lines.Where(l => period.InComparison(l.Order.OrderDate)).Sum(l => l.Total);
				var units = new double[period.Buckets.Count];
				foreach (var line in current)
					units[period.BucketOf(line.Order.OrderDate)] += line.Quantity;

				return new ProductSales
				{
					Product = product,
					Revenue = revenue,
					PreviousRevenue = previous,
					Change = period.Change(revenue, previous),
					Units = current.Sum(l => l.Quantity),
					Orders = current.Length,
					Share = total == 0 ? 0 : (double)(revenue / total * 100),
					UnitsTrend = units
				};
			}).OrderByDescending(p => p.Revenue).ThenBy(p => p.Product.Name).ToArray();
		}

		/// <summary>
		/// Returns the sales per customer country, highest revenue first; countries without orders are included with zero.
		/// </summary>
		/// <param name="context">Session context.</param>
		public static IReadOnlyList<CountrySales> CountrySales(DashboardContext context)
		{
			var period = context.Period;
			return context.Database.Customers.GroupBy(c => c.Country).Select(group =>
			{
				var orders = group.SelectMany(c => c.Orders).ToArray();
				var current = orders.Where(o => period.Contains(o.OrderDate)).ToArray();
				decimal revenue = current.Sum(o => o.Subtotal);
				decimal previous = orders.Where(o => period.InComparison(o.OrderDate)).Sum(o => o.Subtotal);
				return new CountrySales
				{
					Country = group.Key,
					Revenue = revenue,
					Change = period.Change(revenue, previous),
					Orders = current.Length,
					Customers = current.Select(o => o.CustomerId).Distinct().Count()
				};
			}).OrderByDescending(c => c.Revenue).ThenBy(c => c.Country).ToArray();
		}

		/// <summary>
		/// Returns the performance of every employee, highest revenue first.
		/// </summary>
		/// <param name="context">Session context.</param>
		public static IReadOnlyList<EmployeeSales> EmployeeSales(DashboardContext context)
		{
			var period = context.Period;
			var total = context.Database.Orders.Where(o => period.Contains(o.OrderDate)).Sum(o => o.Subtotal);
			return context.Database.Employees.Select(employee =>
			{
				var current = employee.Orders.Where(o => period.Contains(o.OrderDate)).ToArray();
				decimal revenue = current.Sum(o => o.Subtotal);
				decimal previous = employee.Orders.Where(o => period.InComparison(o.OrderDate)).Sum(o => o.Subtotal);
				var shipped = current.Select(o => (Order: o, Shipped: context.ShippedDate(o))).Where(x => x.Shipped != null).ToArray();
				var lines = current.SelectMany(o => o.Lines).ToArray();
				decimal listValue = lines.Sum(l => l.UnitPrice * l.Quantity);
				var trend = new double[period.Buckets.Count];
				foreach (var order in current)
					trend[period.BucketOf(order.OrderDate)] += (double)order.Subtotal;

				return new EmployeeSales
				{
					Employee = employee,
					Revenue = revenue,
					Change = period.Change(revenue, previous),
					Orders = current.Length,
					AverageOrder = current.Length == 0 ? 0 : revenue / current.Length,
					Customers = current.Select(o => o.CustomerId).Distinct().Count(),
					OnTimeRate = shipped.Length == 0 ? 0 : shipped.Count(x => x.Shipped <= x.Order.RequiredDate) * 100.0 / shipped.Length,
					Share = total == 0 ? 0 : (double)(revenue / total * 100),
					AverageDiscount = listValue == 0 ? 0 : (double)((listValue - lines.Sum(l => l.Total)) / listValue * 100),
					Trend = trend.Select(v => Math.Round(v, 2)).ToArray()
				};
			}).OrderByDescending(e => e.Revenue).ToArray();
		}

		/// <summary>
		/// Returns the sales of every customer, highest revenue first.
		/// </summary>
		/// <param name="context">Session context.</param>
		public static IReadOnlyList<CustomerSales> CustomerSales(DashboardContext context)
		{
			var period = context.Period;
			return context.Database.Customers.Select(customer =>
			{
				var current = customer.Orders.Where(o => period.Contains(o.OrderDate)).ToArray();
				decimal revenue = current.Sum(o => o.Subtotal);
				decimal previous = customer.Orders.Where(o => period.InComparison(o.OrderDate)).Sum(o => o.Subtotal);
				var trend = new double[period.Buckets.Count];
				foreach (var order in current)
					trend[period.BucketOf(order.OrderDate)] += (double)order.Subtotal;

				return new CustomerSales
				{
					Customer = customer,
					Revenue = revenue,
					Change = period.Change(revenue, previous),
					Orders = current.Length,
					Trend = trend.Select(v => Math.Round(v, 2)).ToArray()
				};
			}).OrderByDescending(c => c.Revenue).ThenBy(c => c.Customer.Company).ToArray();
		}

		/// <summary>
		/// Returns the delivery performance of every shipper for the context period.
		/// </summary>
		/// <param name="context">Session context.</param>
		public static IReadOnlyList<ShipperPerformance> ShipperPerformance(DashboardContext context) =>
			ShipperPerformance(context, context.Period);

		/// <summary>
		/// Returns the delivery performance of every shipper for the specified period.
		/// </summary>
		/// <param name="context">Session context.</param>
		/// <param name="period">Period whose orders are measured.</param>
		public static IReadOnlyList<ShipperPerformance> ShipperPerformance(DashboardContext context, DashboardPeriod period)
		{
			return context.Database.Shippers.Select(shipper =>
			{
				var all = context.Database.Orders.Where(o => o.ShipVia == shipper.Id).ToArray();
				var current = all.Where(o => period.Contains(o.OrderDate)).ToArray();
				var shipped = current.Select(o => (Order: o, Shipped: context.ShippedDate(o))).Where(x => x.Shipped != null).ToArray();
				return new ShipperPerformance
				{
					Shipper = shipper,
					Orders = current.Length,
					Shipped = shipped.Length,
					Late = shipped.Count(x => x.Shipped > x.Order.RequiredDate),
					Freight = current.Sum(o => o.Freight),
					AverageDays = shipped.Length == 0 ? 0 : shipped.Average(x => (x.Shipped.Value - x.Order.OrderDate).TotalDays),
					OpenOrders = all.Count(o => context.ShippedDate(o) == null)
				};
			}).ToArray();
		}

		/// <summary>
		/// Returns the number of orders per day between two dates; days without orders are omitted.
		/// </summary>
		/// <param name="context">Session context.</param>
		/// <param name="start">First day.</param>
		/// <param name="end">Last day.</param>
		public static IReadOnlyList<DailyValue> DailyOrders(DashboardContext context, DateTime start, DateTime end) =>
			context.Database.Orders
				.Where(o => o.OrderDate >= start && o.OrderDate <= end)
				.GroupBy(o => o.OrderDate.Date)
				.Select(g => new DailyValue { Date = g.Key, Value = g.Count() })
				.ToArray();

		/// <summary>
		/// Rounds a positive maximum up to a value whose four axis intervals are round numbers.
		/// </summary>
		/// <param name="value">Largest plotted value.</param>
		public static double NiceMaximum(double value)
		{
			if (!(value > 0))
				return 1;

			double raw = value / 4;
			double magnitude = Math.Pow(10, Math.Floor(Math.Log10(raw)));
			double step = steps.First(s => s * magnitude >= raw) * magnitude;
			return step * 4;
		}

		#endregion

		#region Implementation

		private static readonly double[] steps = { 1, 1.2, 1.25, 1.5, 2, 2.5, 3, 4, 5, 6, 8, 10 };

		#endregion

	}
}
