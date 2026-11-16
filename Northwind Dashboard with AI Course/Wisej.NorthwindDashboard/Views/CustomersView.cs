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
using System.Text.Json.Nodes;
using Wisej.NorthwindDashboard.Data;
using Wisej.NorthwindDashboard.Dialogs;
using Wisej.Web;

namespace Wisej.NorthwindDashboard.Views
{

	/// <summary>
	/// Customer map with revenue-shaded countries, account details and the customer list.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Countries are shaded by revenue quintile for the period (five equal groups of ordering countries), because
	/// revenue per country spans two orders of magnitude and equal value bands would leave most countries in the
	/// lightest shade. Tooltips show the exact revenue.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// context.Navigate(DashboardArea.Customers, "Germany");
	/// </code>
	/// </example>
	public partial class CustomersView : DashboardView
	{

		#region Constructors

		/// <summary>
		/// Creates the view.
		/// </summary>
		public CustomersView()
		{
			InitializeComponent();

			clearTool = new ComponentTool { Name = "clear", ImageSource = DashboardStyle.Icon("close"), ToolTipText = "Show customers in every country", Visible = false };
			customersCard.Tools.Add(clearTool);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Selects a customer, or filters the list to a country.
		/// </summary>
		/// <param name="target">A <see cref="Customer"/> or a Northwind country name.</param>
		public override void ShowTarget(object target)
		{
			if (Dashboard == null)
				return;

			switch (target)
			{
				case Customer customer:
					country = null;
					ApplyFilter();
					Select(customer, true);
					break;

				case string name:
					country = name;
					ApplyFilter();
					var inCountry = markers.Where(m => m.Key.Country == name).Select(m => m.Value).ToArray();
					if (inCountry.Length > 0)
						map.FitBounds(inCountry);
					break;
			}
		}

		/// <summary>
		/// Reloads the map, the list and the selected customer for the current period.
		/// </summary>
		protected override void RefreshView()
		{
			sales = SalesAnalytics.CustomerSales(Dashboard).ToDictionary(s => s.Customer);
			UpdateMap();
			ApplyFilter();

			if (selected != null)
				ShowDetails(selected);
		}

		#endregion

		#region Implementation

		private readonly ComponentTool clearTool;
		private readonly Dictionary<Customer, MapMarker> markers = new Dictionary<Customer, MapMarker>();
		private Dictionary<Customer, CustomerSales> sales = new Dictionary<Customer, CustomerSales>();
		private Customer selected;
		private string country;

		private void UpdateMap()
		{
			map.Regions = BuildRegions();

			if (markers.Count == 0)
				CreateMarkers();

			foreach (var pair in markers)
			{
				var customer = sales[pair.Key];
				pair.Value.Badge = customer.Orders > 0 ? DashboardStyle.CompactMoney(customer.Revenue) : "";
				pair.Value.Tone = customer.Orders > 0 ? ChipTone.Primary : ChipTone.Neutral;
				pair.Value.Pulse = pair.Key.Orders.Any(o => Dashboard.ShippedDate(o) == null);
			}
		}

		// Customers sharing a city are spread on a small circle so each marker stays clickable at high zoom.
		private void CreateMarkers()
		{
			foreach (var city in Dashboard.Database.Customers.GroupBy(c => c.City + "|" + c.Country))
			{
				var group = city.ToArray();
				for (int i = 0; i < group.Length; i++)
				{
					if (!GeoLocations.TryGetLocation(group[i], out var point))
						continue;

					if (group.Length > 1)
					{
						double angle = 2 * Math.PI * i / group.Length;
						point = new GeoPoint(point.Latitude + 0.018 * Math.Sin(angle), point.Longitude + 0.025 * Math.Cos(angle));
					}

					var marker = new MapMarker { Location = point, Text = group[i].Company, Tag = group[i] };
					markers.Add(group[i], marker);
					map.Markers.Add(marker);
				}
			}
		}

		private string BuildRegions()
		{
			var countries = SalesAnalytics.CountrySales(Dashboard).ToDictionary(c => c.Country);
			var ranked = countries.Values.Where(c => c.Revenue > 0).OrderByDescending(c => c.Revenue).ToList();
			var outlines = JsonNode.Parse(Dashboard.Database.CountryOutlines);
			foreach (var feature in outlines["features"].AsArray())
			{
				string name = (string)feature["id"];
				var properties = feature["properties"].AsObject();
				if (countries.TryGetValue(name, out var sales) && sales.Revenue > 0)
				{
					properties["value"] = 5 - ranked.IndexOf(sales) * 5 / ranked.Count;
					properties["name"] = name + " · " + DashboardStyle.Money(sales.Revenue) + " · " + sales.Customers + " customers · " + sales.Orders + " orders";
				}
				else
				{
					properties["name"] = name + " · no orders in this period";
				}
			}

			return outlines.ToJsonString();
		}

		private void ApplyFilter()
		{
			var rows = sales.Values
				.Where(s => country == null || s.Customer.Country == country)
				.OrderByDescending(s => s.Revenue)
				.ThenBy(s => s.Customer.Company)
				.Select(s => new CustomerRow(s))
				.ToList();

			customersGrid.DataSource = rows;
			customersGrid.CurrentCell = null;
			customersCard.Text = (country == null ? "Customers" : "Customers in " + country) + " · " + rows.Count +
				" · sales " + Dashboard.Period.Name.ToLowerInvariant();
			clearTool.Visible = country != null;
			colChange.Visible = Dashboard.Period.HasComparison;
		}

		private void Select(Customer customer, bool pan)
		{
			selected = customer;
			ShowDetails(customer);

			for (int i = 0; i < customersGrid.RowCount; i++)
			{
				if (customersGrid.Rows[i].DataBoundItem is CustomerRow row && row.Customer == customer)
				{
					customersGrid.Rows[i].Selected = true;
					customersGrid.ScrollRowIntoView(i);
					break;
				}
			}

			if (pan && markers.TryGetValue(customer, out var marker))
			{
				map.Zoom = Math.Max(map.Zoom, 6);
				map.PanTo(marker.Location);
			}
		}

		private void ShowDetails(Customer customer)
		{
			var period = Dashboard.Period;
			var customerSales = sales[customer];
			detailEmpty.Visible = false;
			detailCard.Text = customer.Company;

			customerAvatar.Text = customer.Company;
			customerAvatar.Label = customer.Company;
			customerAvatar.SubLabel = customer.Contact + " · " + customer.ContactTitle;
			placeLabel.Text = customer.Address + ", " + customer.Place + Environment.NewLine + customer.Phone;

			revenueValue.Text = DashboardStyle.Money(customerSales.Revenue);
			revenueCaption.Text = "Revenue in period";
			ordersValue.Text = DashboardStyle.Count(customerSales.Orders);
			ordersCaption.Text = "Orders in period";
			var last = customer.Orders.LastOrDefault();
			lastOrderValue.Text = last == null ? "—" : last.OrderDate.ToString("MMM d, yyyy");

			string unit = period.BucketSize switch { BucketSize.Day => "DAY", BucketSize.Week => "WEEK", _ => "MONTH" };
			trendLabel.Text = "REVENUE PER " + unit + " · " + period.Name.ToUpperInvariant();
			trendSparkline.Values = customerSales.Trend;

			recentOrders.Items.Clear();
			foreach (var order in customer.Orders.Reverse().Take(8))
			{
				var status = Dashboard.StatusOf(order);
				recentOrders.Items.Add(new TimelineItem(order.OrderDate, "Order " + order.Number + " · " + DashboardStyle.MoneyCents(order.Subtotal))
				{
					Description = order.Lines.Count + " lines · " + order.Employee.FullName + " · " + DashboardStyle.StatusText(status),
					Tone = status switch
					{
						OrderStatus.Shipped => TimelineTone.Success,
						OrderStatus.ShippedLate => TimelineTone.Warning,
						OrderStatus.Overdue => TimelineTone.Danger,
						_ => TimelineTone.Primary
					},
					Tag = order
				});
			}

			recentLabel.Text = customer.Orders.Count == 0 ? "NO ORDERS YET" : "RECENT ORDERS · " + customer.Orders.Count + " IN TOTAL";
		}

		private void map_MarkerClick(object sender, MapMarkerEventArgs e)
		{
			if (e.Marker.Tag is Customer customer)
				Select(customer, false);
		}

		private void map_RegionClick(object sender, MapRegionEventArgs e)
		{
			country = e.FeatureId;
			ApplyFilter();
		}

		private void mapCard_ToolClick(object sender, ToolClickEventArgs e) => map.FitBounds();

		private void customersCard_ToolClick(object sender, ToolClickEventArgs e)
		{
			country = null;
			ApplyFilter();
		}

		private void detailCard_ToolClick(object sender, ToolClickEventArgs e)
		{
			if (selected != null)
				Dashboard.Navigate(DashboardArea.Orders, selected);
		}

		private void customersGrid_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && customersGrid.Rows[e.RowIndex].DataBoundItem is CustomerRow row)
				Select(row.Customer, true);
		}

		private void recentOrders_ItemClick(object sender, TimelineItemEventArgs e)
		{
			if (e.Item.Tag is Order order)
				OrderDialog.ShowOrder(Dashboard, order);
		}

		#endregion

	}
}
