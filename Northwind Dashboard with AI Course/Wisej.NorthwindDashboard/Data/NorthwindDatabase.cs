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
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace Wisej.NorthwindDashboard.Data
{

	/// <summary>
	/// Read-only, in-memory copy of the Northwind sample database shared by all sessions.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The data comes from Microsoft's instnwnd.sql script (MIT license), converted to the embedded
	/// Data/northwind.json file. Dates are the original 1996–1998 values. The instance is created once,
	/// on first use, and is never modified afterwards, so sessions can query it concurrently.
	/// Session-specific changes, such as shipping an order, are kept by <see cref="DashboardContext"/>.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// var db = NorthwindDatabase.Instance;
	/// var revenue = db.Orders.Where(o => o.OrderDate.Year == 1997).Sum(o => o.Subtotal);
	/// </code>
	/// </example>
	public sealed class NorthwindDatabase
	{

		#region Constructors

		private NorthwindDatabase()
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the shared database instance, loading it on first access.
		/// </summary>
		public static NorthwindDatabase Instance => instance.Value;

		/// <summary>
		/// Gets the eight product categories ordered by identifier.
		/// </summary>
		public IReadOnlyList<Category> Categories { get; private set; }

		/// <summary>
		/// Gets the suppliers.
		/// </summary>
		public IReadOnlyList<Supplier> Suppliers { get; private set; }

		/// <summary>
		/// Gets the three shippers ordered by identifier.
		/// </summary>
		public IReadOnlyList<Shipper> Shippers { get; private set; }

		/// <summary>
		/// Gets the employees ordered by identifier.
		/// </summary>
		public IReadOnlyList<Employee> Employees { get; private set; }

		/// <summary>
		/// Gets the customers ordered by company name.
		/// </summary>
		public IReadOnlyList<Customer> Customers { get; private set; }

		/// <summary>
		/// Gets the products ordered by name.
		/// </summary>
		public IReadOnlyList<Product> Products { get; private set; }

		/// <summary>
		/// Gets the orders ordered by date and number.
		/// </summary>
		public IReadOnlyList<Order> Orders { get; private set; }

		/// <summary>
		/// Gets the date of the first order (July 4, 1996).
		/// </summary>
		public DateTime FirstOrderDate { get; private set; }

		/// <summary>
		/// Gets the date of the last recorded order (May 6, 1998), used as the dashboard's "today".
		/// </summary>
		public DateTime AsOfDate { get; private set; }

		/// <summary>
		/// Gets GeoJSON outlines (Natural Earth, public domain) of the customer countries; feature ids are Northwind country names.
		/// </summary>
		public string CountryOutlines { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Returns the customer with the specified code, or null.
		/// </summary>
		/// <param name="id">Five-letter customer code.</param>
		public Customer FindCustomer(string id) => id != null && customersById.TryGetValue(id, out var customer) ? customer : null;

		/// <summary>
		/// Returns the order with the specified number, or null.
		/// </summary>
		/// <param name="id">Order number.</param>
		public Order FindOrder(int id) => ordersById.TryGetValue(id, out var order) ? order : null;

		/// <summary>
		/// Returns the product with the specified identifier, or null.
		/// </summary>
		/// <param name="id">Product identifier.</param>
		public Product FindProduct(int id) => productsById.TryGetValue(id, out var product) ? product : null;

		/// <summary>
		/// Returns the employee with the specified identifier, or null.
		/// </summary>
		/// <param name="id">Employee identifier.</param>
		public Employee FindEmployee(int id) => employeesById.TryGetValue(id, out var employee) ? employee : null;

		#endregion

		#region Implementation

		private const string DataResource = "Wisej.NorthwindDashboard.Data.northwind.json";
		private const string OutlinesResource = "Wisej.NorthwindDashboard.Data.countries.geojson";

		private static readonly Lazy<NorthwindDatabase> instance = new Lazy<NorthwindDatabase>(Load, LazyThreadSafetyMode.ExecutionAndPublication);

		private Dictionary<string, Customer> customersById;
		private Dictionary<int, Order> ordersById;
		private Dictionary<int, Product> productsById;
		private Dictionary<int, Employee> employeesById;

		private static NorthwindDatabase Load()
		{
			NorthwindFile file;
			using (var stream = ReadResource(DataResource))
				file = JsonSerializer.Deserialize<NorthwindFile>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			var categories = file.Categories.OrderBy(c => c.Id).ToArray();
			var suppliers = file.Suppliers.ToDictionary(s => s.Id);
			var shippers = file.Shippers.OrderBy(s => s.Id).ToArray();
			var regions = file.Regions.ToDictionary(r => r.Id);
			var territories = file.Territories.ToDictionary(t => t.Id);
			var employees = file.Employees.OrderBy(e => e.Id).ToArray();
			var customers = file.Customers.OrderBy(c => c.Company, StringComparer.CurrentCulture).ToArray();
			var products = file.Products.OrderBy(p => p.Name, StringComparer.CurrentCulture).ToArray();
			var orders = file.Orders.OrderBy(o => o.OrderDate).ThenBy(o => o.Id).ToArray();

			foreach (var territory in territories.Values)
				territory.Region = regions[territory.RegionId];

			var employeesById = employees.ToDictionary(e => e.Id);
			foreach (var employee in employees)
			{
				employee.Manager = employee.ReportsTo is int managerId ? employeesById[managerId] : null;
				employee.Reports = employees.Where(e => e.ReportsTo == employee.Id).ToArray();
				employee.Territories = employee.TerritoryIds.Select(id => territories[id]).ToArray();
			}

			foreach (var category in categories)
				category.Products = products.Where(p => p.CategoryId == category.Id).ToArray();

			var categoriesById = categories.ToDictionary(c => c.Id);
			var productsById = products.ToDictionary(p => p.Id);
			foreach (var product in products)
			{
				product.Category = categoriesById[product.CategoryId];
				product.Supplier = suppliers[product.SupplierId];
			}

			var customersById = customers.ToDictionary(c => c.Id);
			var shippersById = shippers.ToDictionary(s => s.Id);
			var ordersById = orders.ToDictionary(o => o.Id);
			foreach (var order in orders)
			{
				order.Customer = customersById[order.CustomerId];
				order.Employee = employeesById[order.EmployeeId];
				order.Shipper = shippersById[order.ShipVia];
			}

			foreach (var line in file.OrderDetails)
			{
				line.Order = ordersById[line.OrderId];
				line.Product = productsById[line.ProductId];
			}

			var linesByOrder = file.OrderDetails.ToLookup(l => l.OrderId);
			foreach (var order in orders)
			{
				order.Lines = linesByOrder[order.Id].OrderBy(l => l.Product.Name, StringComparer.CurrentCulture).ToArray();
				order.Subtotal = order.Lines.Sum(l => l.Total);
				order.Units = order.Lines.Sum(l => l.Quantity);
			}

			var linesByProduct = file.OrderDetails.ToLookup(l => l.ProductId);
			foreach (var product in products)
				product.Lines = linesByProduct[product.Id].OrderBy(l => l.Order.OrderDate).ToArray();

			var ordersByCustomer = orders.ToLookup(o => o.CustomerId);
			foreach (var customer in customers)
				customer.Orders = ordersByCustomer[customer.Id].ToArray();

			var ordersByEmployee = orders.ToLookup(o => o.EmployeeId);
			foreach (var employee in employees)
				employee.Orders = ordersByEmployee[employee.Id].ToArray();

			string outlines;
			using (var reader = new StreamReader(ReadResource(OutlinesResource)))
				outlines = reader.ReadToEnd();

			return new NorthwindDatabase
			{
				Categories = categories,
				Suppliers = suppliers.Values.OrderBy(s => s.Company, StringComparer.CurrentCulture).ToArray(),
				Shippers = shippers,
				Employees = employees,
				Customers = customers,
				Products = products,
				Orders = orders,
				FirstOrderDate = orders[0].OrderDate,
				AsOfDate = orders[orders.Length - 1].OrderDate,
				CountryOutlines = outlines,
				customersById = customersById,
				ordersById = ordersById,
				productsById = productsById,
				employeesById = employeesById
			};
		}

		private static Stream ReadResource(string name) =>
			typeof(NorthwindDatabase).Assembly.GetManifestResourceStream(name)
				?? throw new InvalidOperationException("Missing embedded resource " + name + ".");

		// Shape of the embedded JSON file.
		private sealed class NorthwindFile
		{
			public Category[] Categories { get; set; }

			public Supplier[] Suppliers { get; set; }

			public Shipper[] Shippers { get; set; }

			public SalesRegion[] Regions { get; set; }

			public Territory[] Territories { get; set; }

			public Employee[] Employees { get; set; }

			public Customer[] Customers { get; set; }

			public Product[] Products { get; set; }

			public Order[] Orders { get; set; }

			public OrderLine[] OrderDetails { get; set; }
		}

		#endregion

	}
}
