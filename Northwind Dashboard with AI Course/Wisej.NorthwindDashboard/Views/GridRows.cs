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
using System.Linq;
using Wisej.NorthwindDashboard.Data;
using Wisej.Web;

namespace Wisej.NorthwindDashboard.Views
{

	/// <summary>
	/// Grid row for a product with its period sales and stock position.
	/// </summary>
	/// <example>
	/// <code>
	/// grid.DataSource = SalesAnalytics.ProductSales(context).Select(s => new ProductRow(s, context)).ToList();
	/// </code>
	/// </example>
	public sealed class ProductRow
	{

		#region Constructors

		/// <summary>
		/// Creates the row.
		/// </summary>
		/// <param name="sales">Period sales of the product.</param>
		/// <param name="context">Session context for stock and purchase orders.</param>
		public ProductRow(ProductSales sales, DashboardContext context)
		{
			Sales = sales;
			Product = sales.Product;
			StockStatus = context.StockStatusOf(Product);
			StatusText = DashboardStyle.StockText(StockStatus);
			StatusTone = DashboardStyle.StockTone(StockStatus);
			UnitsOnOrder = context.UnitsOnOrder(Product);
			StockCoverage = Product.Discontinued ? 0 : Math.Round((double)Product.UnitsInStock / Math.Max(Product.ReorderLevel, 1), 2);
			StockText = Product.Discontinued
				? "Discontinued"
				: Product.UnitsInStock + " units" + (Product.ReorderLevel > 0 ? " · min " + Product.ReorderLevel : "");
			ChangeText = DashboardStyle.Change(sales.Change);
			ChangeTone = Tone(sales.Change);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the product.
		/// </summary>
		public Product Product { get; }

		/// <summary>
		/// Gets the period sales.
		/// </summary>
		public ProductSales Sales { get; }

		/// <summary>
		/// Gets the product name.
		/// </summary>
		public string Name => Product.Name;

		/// <summary>
		/// Gets the category name.
		/// </summary>
		public string CategoryName => Product.Category.Name;

		/// <summary>
		/// Gets the supplier and packaging.
		/// </summary>
		public string SupplierText => Product.Supplier.Company + " · " + Product.QuantityPerUnit;

		/// <summary>
		/// Gets the category and supplier.
		/// </summary>
		public string DetailText => Product.Category.Name + " · " + Product.Supplier.Company;

		/// <summary>
		/// Gets the units sold per bucket.
		/// </summary>
		public double[] UnitsTrend => Sales.UnitsTrend;

		/// <summary>
		/// Gets the units sold.
		/// </summary>
		public int Units => Sales.Units;

		/// <summary>
		/// Gets the revenue.
		/// </summary>
		public decimal Revenue => Sales.Revenue;

		/// <summary>
		/// Gets the share of period revenue, in percent.
		/// </summary>
		public double Share => Math.Round(Sales.Share, 2);

		/// <summary>
		/// Gets the formatted share.
		/// </summary>
		public string ShareText => DashboardStyle.Percent(Sales.Share);

		/// <summary>
		/// Gets the formatted revenue change.
		/// </summary>
		public string ChangeText { get; }

		/// <summary>
		/// Gets the chip tone of the revenue change.
		/// </summary>
		public ChipTone ChangeTone { get; }

		/// <summary>
		/// Gets the list price.
		/// </summary>
		public decimal UnitPrice => Product.UnitPrice;

		/// <summary>
		/// Gets the units in stock.
		/// </summary>
		public int UnitsInStock => Product.UnitsInStock;

		/// <summary>
		/// Gets the units on order, including session purchase orders.
		/// </summary>
		public int UnitsOnOrder { get; }

		/// <summary>
		/// Gets the stock as a multiple of the reorder level (1 = at the reorder level).
		/// </summary>
		public double StockCoverage { get; }

		/// <summary>
		/// Gets the stock caption.
		/// </summary>
		public string StockText { get; }

		/// <summary>
		/// Gets the stock position.
		/// </summary>
		public StockStatus StockStatus { get; }

		/// <summary>
		/// Gets the stock position caption.
		/// </summary>
		public string StatusText { get; }

		/// <summary>
		/// Gets the chip tone of the stock position.
		/// </summary>
		public ChipTone StatusTone { get; }

		#endregion

		#region Implementation

		internal static ChipTone Tone(decimal? change) =>
			change is decimal value ? value > 0 ? ChipTone.Success : value < 0 ? ChipTone.Danger : ChipTone.Neutral : ChipTone.Neutral;

		#endregion

	}

	/// <summary>
	/// Grid row for an order and its delivery state in the session.
	/// </summary>
	/// <example>
	/// <code>
	/// grid.DataSource = context.Database.Orders.Select(o => new OrderRow(o, context)).ToList();
	/// </code>
	/// </example>
	public sealed class OrderRow
	{

		#region Constructors

		/// <summary>
		/// Creates the row.
		/// </summary>
		/// <param name="order">Order.</param>
		/// <param name="context">Session context for shipping changes.</param>
		public OrderRow(Order order, DashboardContext context)
		{
			Order = order;
			ShippedDate = context.ShippedDate(order);
			Status = context.StatusOf(order);
			StatusText = DashboardStyle.StatusText(Status);
			StatusTone = DashboardStyle.StatusTone(Status);
			DeliveryText = ShippedDate is DateTime shipped
				? "Shipped " + DashboardStyle.Date(shipped)
				: Status == OrderStatus.Overdue
					? "Overdue since " + DashboardStyle.Date(order.RequiredDate)
					: "Due " + DashboardStyle.Date(order.RequiredDate);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the order.
		/// </summary>
		public Order Order { get; }

		/// <summary>
		/// Gets the order number.
		/// </summary>
		public int Id => Order.Id;

		/// <summary>
		/// Gets the order number formatted as "#10248".
		/// </summary>
		public string Number => Order.Number;

		/// <summary>
		/// Gets the order date.
		/// </summary>
		public DateTime OrderDate => Order.OrderDate;

		/// <summary>
		/// Gets the formatted order date.
		/// </summary>
		public string DateText => DashboardStyle.Date(Order.OrderDate);

		/// <summary>
		/// Gets the customer company.
		/// </summary>
		public string Customer => Order.Customer.Company;

		/// <summary>
		/// Gets the customer's city and country.
		/// </summary>
		public string CustomerPlace => Order.Customer.City + ", " + Order.Customer.Country;

		/// <summary>
		/// Gets the customer country.
		/// </summary>
		public string Country => Order.Customer.Country;

		/// <summary>
		/// Gets the sales representative's name.
		/// </summary>
		public string SalesRep => Order.Employee.FullName;

		/// <summary>
		/// Gets the sales representative's title.
		/// </summary>
		public string SalesRepTitle => Order.Employee.Title;

		/// <summary>
		/// Gets the shipper name.
		/// </summary>
		public string Shipper => Order.Shipper.Company;

		/// <summary>
		/// Gets the number of order lines.
		/// </summary>
		public int Items => Order.Lines.Count;

		/// <summary>
		/// Gets the number of units.
		/// </summary>
		public int Units => Order.Units;

		/// <summary>
		/// Gets the net merchandise value.
		/// </summary>
		public decimal Amount => Order.Subtotal;

		/// <summary>
		/// Gets the freight charge.
		/// </summary>
		public decimal Freight => Order.Freight;

		/// <summary>
		/// Gets the required delivery date.
		/// </summary>
		public DateTime RequiredDate => Order.RequiredDate;

		/// <summary>
		/// Gets the shipping date, including session shipments.
		/// </summary>
		public DateTime? ShippedDate { get; }

		/// <summary>
		/// Gets the delivery state.
		/// </summary>
		public OrderStatus Status { get; }

		/// <summary>
		/// Gets the delivery state caption.
		/// </summary>
		public string StatusText { get; }

		/// <summary>
		/// Gets the chip tone of the delivery state.
		/// </summary>
		public ChipTone StatusTone { get; }

		/// <summary>
		/// Gets the shipping or due date caption.
		/// </summary>
		public string DeliveryText { get; }

		#endregion

	}

	/// <summary>
	/// Grid row for a customer and its period sales.
	/// </summary>
	/// <example>
	/// <code>
	/// grid.DataSource = SalesAnalytics.CustomerSales(context).Select(s => new CustomerRow(s)).ToList();
	/// </code>
	/// </example>
	public sealed class CustomerRow
	{

		#region Constructors

		/// <summary>
		/// Creates the row.
		/// </summary>
		/// <param name="sales">Period sales of the customer.</param>
		public CustomerRow(CustomerSales sales)
		{
			Sales = sales;
			var last = sales.Customer.Orders.LastOrDefault();
			LastOrderText = last == null ? "No orders" : DashboardStyle.Date(last.OrderDate);
			ChangeText = DashboardStyle.Change(sales.Change);
			ChangeTone = ProductRow.Tone(sales.Change);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the period sales.
		/// </summary>
		public CustomerSales Sales { get; }

		/// <summary>
		/// Gets the customer.
		/// </summary>
		public Customer Customer => Sales.Customer;

		/// <summary>
		/// Gets the company name.
		/// </summary>
		public string Company => Customer.Company;

		/// <summary>
		/// Gets the contact and title.
		/// </summary>
		public string ContactText => Customer.Contact + " · " + Customer.ContactTitle;

		/// <summary>
		/// Gets the city and country.
		/// </summary>
		public string Place => Customer.City + ", " + Customer.Country;

		/// <summary>
		/// Gets the country.
		/// </summary>
		public string Country => Customer.Country;

		/// <summary>
		/// Gets the number of period orders.
		/// </summary>
		public int Orders => Sales.Orders;

		/// <summary>
		/// Gets the period revenue.
		/// </summary>
		public decimal Revenue => Sales.Revenue;

		/// <summary>
		/// Gets the revenue per bucket.
		/// </summary>
		public double[] Trend => Sales.Trend;

		/// <summary>
		/// Gets the date of the most recent order.
		/// </summary>
		public string LastOrderText { get; }

		/// <summary>
		/// Gets the formatted revenue change.
		/// </summary>
		public string ChangeText { get; }

		/// <summary>
		/// Gets the chip tone of the revenue change.
		/// </summary>
		public ChipTone ChangeTone { get; }

		#endregion

	}

	/// <summary>
	/// Grid row for an employee's period performance.
	/// </summary>
	/// <example>
	/// <code>
	/// grid.DataSource = SalesAnalytics.EmployeeSales(context).Select(s => new EmployeeRow(s)).ToList();
	/// </code>
	/// </example>
	public sealed class EmployeeRow
	{

		#region Constructors

		/// <summary>
		/// Creates the row.
		/// </summary>
		/// <param name="sales">Period performance of the employee.</param>
		public EmployeeRow(EmployeeSales sales)
		{
			Sales = sales;
			ChangeText = DashboardStyle.Change(sales.Change);
			ChangeTone = ProductRow.Tone(sales.Change);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the period performance.
		/// </summary>
		public EmployeeSales Sales { get; }

		/// <summary>
		/// Gets the employee.
		/// </summary>
		public Employee Employee => Sales.Employee;

		/// <summary>
		/// Gets the full name.
		/// </summary>
		public string Name => Employee.FullName;

		/// <summary>
		/// Gets the title and office.
		/// </summary>
		public string TitleText => Employee.Title + " · " + Employee.City;

		/// <summary>
		/// Gets the revenue booked.
		/// </summary>
		public decimal Revenue => Sales.Revenue;

		/// <summary>
		/// Gets the number of orders.
		/// </summary>
		public int Orders => Sales.Orders;

		/// <summary>
		/// Gets the average order value.
		/// </summary>
		public decimal AverageOrder => Sales.AverageOrder;

		/// <summary>
		/// Gets the share of team revenue, in percent.
		/// </summary>
		public double Share => Math.Round(Sales.Share, 2);

		/// <summary>
		/// Gets the formatted share.
		/// </summary>
		public string ShareText => DashboardStyle.Percent(Sales.Share);

		/// <summary>
		/// Gets the revenue per bucket.
		/// </summary>
		public double[] Trend => Sales.Trend;

		/// <summary>
		/// Gets the formatted revenue change.
		/// </summary>
		public string ChangeText { get; }

		/// <summary>
		/// Gets the chip tone of the revenue change.
		/// </summary>
		public ChipTone ChangeTone { get; }

		#endregion

	}

	/// <summary>
	/// Grid row for a category's period sales.
	/// </summary>
	/// <example>
	/// <code>
	/// grid.DataSource = SalesAnalytics.CategorySales(context).Select(s => new CategoryRow(s)).ToList();
	/// </code>
	/// </example>
	public sealed class CategoryRow
	{

		#region Constructors

		/// <summary>
		/// Creates the row.
		/// </summary>
		/// <param name="sales">Period sales of the category.</param>
		public CategoryRow(CategorySales sales)
		{
			Sales = sales;
			ChangeText = DashboardStyle.Change(sales.Change);
			ChangeTone = ProductRow.Tone(sales.Change);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the period sales.
		/// </summary>
		public CategorySales Sales { get; }

		/// <summary>
		/// Gets the category name.
		/// </summary>
		public string Name => Sales.Category.Name;

		/// <summary>
		/// Gets the share caption.
		/// </summary>
		public string ShareText => DashboardStyle.Percent(Sales.Share) + " of revenue";

		/// <summary>
		/// Gets the revenue.
		/// </summary>
		public decimal Revenue => Sales.Revenue;

		/// <summary>
		/// Gets the revenue per bucket.
		/// </summary>
		public double[] Trend => Sales.Trend;

		/// <summary>
		/// Gets the formatted revenue change.
		/// </summary>
		public string ChangeText { get; }

		/// <summary>
		/// Gets the chip tone of the revenue change.
		/// </summary>
		public ChipTone ChangeTone { get; }

		#endregion

	}

	/// <summary>
	/// Grid row for one trend bucket, the table view of the revenue charts.
	/// </summary>
	/// <example>
	/// <code>
	/// grid.DataSource = SalesAnalytics.RevenueTrend(context).Reverse().Select(p => new TrendRow(p, true)).ToList();
	/// </code>
	/// </example>
	public sealed class TrendRow
	{

		#region Constructors

		/// <summary>
		/// Creates the row.
		/// </summary>
		/// <param name="point">Trend bucket.</param>
		/// <param name="hasComparison">Whether the comparison revenue is meaningful.</param>
		public TrendRow(TrendPoint point, bool hasComparison)
		{
			Point = point;
			PreviousRevenue = hasComparison ? (decimal)point.PreviousRevenue : null;
			decimal? change = hasComparison && point.PreviousRevenue > 0
				? Math.Round((decimal)((point.Revenue - point.PreviousRevenue) / point.PreviousRevenue * 100), 1)
				: null;
			ChangeText = DashboardStyle.Change(change);
			ChangeTone = ProductRow.Tone(change);
			PeriodText = point.Bucket.Start == point.Bucket.End
				? DashboardStyle.Date(point.Bucket.Start)
				: DashboardStyle.Date(point.Bucket.Start) + " – " + DashboardStyle.Date(point.Bucket.End);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the trend bucket.
		/// </summary>
		public TrendPoint Point { get; }

		/// <summary>
		/// Gets the bucket label.
		/// </summary>
		public string Label => Point.Label;

		/// <summary>
		/// Gets the covered dates.
		/// </summary>
		public string PeriodText { get; }

		/// <summary>
		/// Gets the number of orders.
		/// </summary>
		public int Orders => Point.Orders;

		/// <summary>
		/// Gets the revenue.
		/// </summary>
		public decimal Revenue => (decimal)Point.Revenue;

		/// <summary>
		/// Gets the average order value.
		/// </summary>
		public decimal AverageOrder => (decimal)Point.AverageOrder;

		/// <summary>
		/// Gets the revenue of the aligned comparison bucket, or null without a comparison.
		/// </summary>
		public decimal? PreviousRevenue { get; }

		/// <summary>
		/// Gets the formatted change against the aligned comparison bucket.
		/// </summary>
		public string ChangeText { get; }

		/// <summary>
		/// Gets the chip tone of the change.
		/// </summary>
		public ChipTone ChangeTone { get; }

		#endregion

	}
}
