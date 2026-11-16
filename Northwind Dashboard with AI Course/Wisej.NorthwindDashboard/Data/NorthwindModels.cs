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
using System.Text.Json.Serialization;

namespace Wisej.NorthwindDashboard.Data
{

	/// <summary>
	/// Product category, such as Beverages or Seafood.
	/// </summary>
	public sealed class Category
	{

		#region Properties

		/// <summary>
		/// Gets or sets the Northwind category identifier (1 to 8).
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the category name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the category description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets the products in this category.
		/// </summary>
		[JsonIgnore]
		public IReadOnlyList<Product> Products { get; internal set; }

		#endregion

		#region Methods

		/// <inheritdoc/>
		public override string ToString() => Name;

		#endregion

	}

	/// <summary>
	/// Company that supplies products.
	/// </summary>
	public sealed class Supplier
	{

		#region Properties

		/// <summary>
		/// Gets or sets the supplier identifier.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the company name.
		/// </summary>
		public string Company { get; set; }

		/// <summary>
		/// Gets or sets the primary contact.
		/// </summary>
		public string Contact { get; set; }

		/// <summary>
		/// Gets or sets the contact's title.
		/// </summary>
		public string ContactTitle { get; set; }

		/// <summary>
		/// Gets or sets the city.
		/// </summary>
		public string City { get; set; }

		/// <summary>
		/// Gets or sets the region or state, if any.
		/// </summary>
		public string Region { get; set; }

		/// <summary>
		/// Gets or sets the country.
		/// </summary>
		public string Country { get; set; }

		/// <summary>
		/// Gets or sets the phone number.
		/// </summary>
		public string Phone { get; set; }

		#endregion

		#region Methods

		/// <inheritdoc/>
		public override string ToString() => Company;

		#endregion

	}

	/// <summary>
	/// Carrier that delivers orders.
	/// </summary>
	public sealed class Shipper
	{

		#region Properties

		/// <summary>
		/// Gets or sets the shipper identifier (1 to 3).
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the company name.
		/// </summary>
		public string Company { get; set; }

		/// <summary>
		/// Gets or sets the phone number.
		/// </summary>
		public string Phone { get; set; }

		#endregion

		#region Methods

		/// <inheritdoc/>
		public override string ToString() => Company;

		#endregion

	}

	/// <summary>
	/// Sales region that groups territories.
	/// </summary>
	public sealed class SalesRegion
	{

		#region Properties

		/// <summary>
		/// Gets or sets the region identifier.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the region name.
		/// </summary>
		public string Name { get; set; }

		#endregion

	}

	/// <summary>
	/// Sales territory covered by one or more employees.
	/// </summary>
	public sealed class Territory
	{

		#region Properties

		/// <summary>
		/// Gets or sets the territory identifier (a US postal code).
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the territory name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the region that contains the territory.
		/// </summary>
		public int RegionId { get; set; }

		/// <summary>
		/// Gets the region that contains the territory.
		/// </summary>
		[JsonIgnore]
		public SalesRegion Region { get; internal set; }

		#endregion

	}

	/// <summary>
	/// Northwind employee, with the reporting hierarchy used by the team view.
	/// </summary>
	public sealed class Employee
	{

		#region Properties

		/// <summary>
		/// Gets or sets the employee identifier.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the first name.
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the job title.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the salutation, such as Ms. or Dr.
		/// </summary>
		public string TitleOfCourtesy { get; set; }

		/// <summary>
		/// Gets or sets the birth date.
		/// </summary>
		public DateTime BirthDate { get; set; }

		/// <summary>
		/// Gets or sets the hire date.
		/// </summary>
		public DateTime HireDate { get; set; }

		/// <summary>
		/// Gets or sets the office city.
		/// </summary>
		public string City { get; set; }

		/// <summary>
		/// Gets or sets the region or state, if any.
		/// </summary>
		public string Region { get; set; }

		/// <summary>
		/// Gets or sets the office country.
		/// </summary>
		public string Country { get; set; }

		/// <summary>
		/// Gets or sets the phone extension.
		/// </summary>
		public string Extension { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the manager, or null for the head of sales.
		/// </summary>
		public int? ReportsTo { get; set; }

		/// <summary>
		/// Gets or sets the biography notes.
		/// </summary>
		public string Notes { get; set; }

		/// <summary>
		/// Gets or sets the identifiers of the territories covered by the employee.
		/// </summary>
		[JsonPropertyName("territories")]
		public string[] TerritoryIds { get; set; } = Array.Empty<string>();

		/// <summary>
		/// Gets the first and last name.
		/// </summary>
		[JsonIgnore]
		public string FullName => FirstName + " " + LastName;

		/// <summary>
		/// Gets the manager, or null for the head of sales.
		/// </summary>
		[JsonIgnore]
		public Employee Manager { get; internal set; }

		/// <summary>
		/// Gets the direct reports.
		/// </summary>
		[JsonIgnore]
		public IReadOnlyList<Employee> Reports { get; internal set; }

		/// <summary>
		/// Gets the covered territories.
		/// </summary>
		[JsonIgnore]
		public IReadOnlyList<Territory> Territories { get; internal set; }

		/// <summary>
		/// Gets the orders booked by the employee, oldest first.
		/// </summary>
		[JsonIgnore]
		public IReadOnlyList<Order> Orders { get; internal set; }

		#endregion

		#region Methods

		/// <inheritdoc/>
		public override string ToString() => FullName;

		#endregion

	}

	/// <summary>
	/// Customer company.
	/// </summary>
	public sealed class Customer
	{

		#region Properties

		/// <summary>
		/// Gets or sets the five-letter customer code, such as ALFKI.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the company name.
		/// </summary>
		public string Company { get; set; }

		/// <summary>
		/// Gets or sets the primary contact.
		/// </summary>
		public string Contact { get; set; }

		/// <summary>
		/// Gets or sets the contact's title.
		/// </summary>
		public string ContactTitle { get; set; }

		/// <summary>
		/// Gets or sets the street address.
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Gets or sets the city.
		/// </summary>
		public string City { get; set; }

		/// <summary>
		/// Gets or sets the region or state, if any.
		/// </summary>
		public string Region { get; set; }

		/// <summary>
		/// Gets or sets the postal code.
		/// </summary>
		public string PostalCode { get; set; }

		/// <summary>
		/// Gets or sets the country.
		/// </summary>
		public string Country { get; set; }

		/// <summary>
		/// Gets or sets the phone number.
		/// </summary>
		public string Phone { get; set; }

		/// <summary>
		/// Gets the city, followed by the region when one exists.
		/// </summary>
		[JsonIgnore]
		public string Place => string.IsNullOrEmpty(Region) ? City + ", " + Country : City + ", " + Region + ", " + Country;

		/// <summary>
		/// Gets the orders placed by the customer, oldest first.
		/// </summary>
		[JsonIgnore]
		public IReadOnlyList<Order> Orders { get; internal set; }

		#endregion

		#region Methods

		/// <inheritdoc/>
		public override string ToString() => Company;

		#endregion

	}

	/// <summary>
	/// Product sold by Northwind.
	/// </summary>
	public sealed class Product
	{

		#region Properties

		/// <summary>
		/// Gets or sets the product identifier.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the product name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the supplier identifier.
		/// </summary>
		public int SupplierId { get; set; }

		/// <summary>
		/// Gets or sets the category identifier.
		/// </summary>
		public int CategoryId { get; set; }

		/// <summary>
		/// Gets or sets the packaging description, such as "10 boxes x 20 bags".
		/// </summary>
		public string QuantityPerUnit { get; set; }

		/// <summary>
		/// Gets or sets the current list price in US dollars.
		/// </summary>
		public decimal UnitPrice { get; set; }

		/// <summary>
		/// Gets or sets the units in stock.
		/// </summary>
		public int UnitsInStock { get; set; }

		/// <summary>
		/// Gets or sets the units already ordered from the supplier.
		/// </summary>
		public int UnitsOnOrder { get; set; }

		/// <summary>
		/// Gets or sets the stock level that triggers a reorder.
		/// </summary>
		public int ReorderLevel { get; set; }

		/// <summary>
		/// Gets or sets whether the product is no longer sold.
		/// </summary>
		public bool Discontinued { get; set; }

		/// <summary>
		/// Gets the category.
		/// </summary>
		[JsonIgnore]
		public Category Category { get; internal set; }

		/// <summary>
		/// Gets the supplier.
		/// </summary>
		[JsonIgnore]
		public Supplier Supplier { get; internal set; }

		/// <summary>
		/// Gets every order line for the product.
		/// </summary>
		[JsonIgnore]
		public IReadOnlyList<OrderLine> Lines { get; internal set; }

		#endregion

		#region Methods

		/// <inheritdoc/>
		public override string ToString() => Name;

		#endregion

	}

	/// <summary>
	/// Customer order.
	/// </summary>
	public sealed class Order
	{

		#region Properties

		/// <summary>
		/// Gets or sets the order number.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the customer code.
		/// </summary>
		public string CustomerId { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the employee who booked the order.
		/// </summary>
		public int EmployeeId { get; set; }

		/// <summary>
		/// Gets or sets the order date.
		/// </summary>
		public DateTime OrderDate { get; set; }

		/// <summary>
		/// Gets or sets the date the customer requires the delivery.
		/// </summary>
		public DateTime RequiredDate { get; set; }

		/// <summary>
		/// Gets or sets the shipping date recorded in Northwind, or null when the order is open.
		/// </summary>
		public DateTime? ShippedDate { get; set; }

		/// <summary>
		/// Gets or sets the shipper identifier.
		/// </summary>
		public int ShipVia { get; set; }

		/// <summary>
		/// Gets or sets the freight charge in US dollars.
		/// </summary>
		public decimal Freight { get; set; }

		/// <summary>
		/// Gets or sets the recipient name.
		/// </summary>
		public string ShipName { get; set; }

		/// <summary>
		/// Gets or sets the delivery address.
		/// </summary>
		public string ShipAddress { get; set; }

		/// <summary>
		/// Gets or sets the delivery city.
		/// </summary>
		public string ShipCity { get; set; }

		/// <summary>
		/// Gets or sets the delivery region, if any.
		/// </summary>
		public string ShipRegion { get; set; }

		/// <summary>
		/// Gets or sets the delivery postal code.
		/// </summary>
		public string ShipPostalCode { get; set; }

		/// <summary>
		/// Gets or sets the delivery country.
		/// </summary>
		public string ShipCountry { get; set; }

		/// <summary>
		/// Gets the customer.
		/// </summary>
		[JsonIgnore]
		public Customer Customer { get; internal set; }

		/// <summary>
		/// Gets the employee who booked the order.
		/// </summary>
		[JsonIgnore]
		public Employee Employee { get; internal set; }

		/// <summary>
		/// Gets the shipper.
		/// </summary>
		[JsonIgnore]
		public Shipper Shipper { get; internal set; }

		/// <summary>
		/// Gets the order lines.
		/// </summary>
		[JsonIgnore]
		public IReadOnlyList<OrderLine> Lines { get; internal set; }

		/// <summary>
		/// Gets the net merchandise value: the sum of the discounted line totals, excluding freight.
		/// </summary>
		[JsonIgnore]
		public decimal Subtotal { get; internal set; }

		/// <summary>
		/// Gets the number of units across all lines.
		/// </summary>
		[JsonIgnore]
		public int Units { get; internal set; }

		/// <summary>
		/// Gets the order number formatted for display, such as "#10248".
		/// </summary>
		[JsonIgnore]
		public string Number => "#" + Id;

		#endregion

		#region Methods

		/// <inheritdoc/>
		public override string ToString() => Number;

		#endregion

	}

	/// <summary>
	/// Product line of an order.
	/// </summary>
	public sealed class OrderLine
	{

		#region Properties

		/// <summary>
		/// Gets or sets the order number.
		/// </summary>
		public int OrderId { get; set; }

		/// <summary>
		/// Gets or sets the product identifier.
		/// </summary>
		public int ProductId { get; set; }

		/// <summary>
		/// Gets or sets the unit price charged on the order.
		/// </summary>
		public decimal UnitPrice { get; set; }

		/// <summary>
		/// Gets or sets the ordered quantity.
		/// </summary>
		public int Quantity { get; set; }

		/// <summary>
		/// Gets or sets the discount as a fraction, such as 0.15 for 15%.
		/// </summary>
		public decimal Discount { get; set; }

		/// <summary>
		/// Gets the order.
		/// </summary>
		[JsonIgnore]
		public Order Order { get; internal set; }

		/// <summary>
		/// Gets the product.
		/// </summary>
		[JsonIgnore]
		public Product Product { get; internal set; }

		/// <summary>
		/// Gets the discounted line total, rounded to cents like Northwind's Order Subtotals view.
		/// </summary>
		[JsonIgnore]
		public decimal Total => Math.Round(UnitPrice * Quantity * (1 - Discount), 2, MidpointRounding.AwayFromZero);

		#endregion

	}
}
