using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using NorthwindDashboard.Data;
using NorthwindDashboard.Dialogs;
using Wisej.Web;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Module 6 customers view: the customer map with revenue-shaded countries, the profile of the selected customer
	/// with its recent orders and revenue trend, and the customer list.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Every customer with a known city is one <see cref="MapMarker"/>: <c>Location</c> is the city's
	/// <see cref="GeoPoint"/>, <c>Text</c> the company and <c>Tag</c> the <see cref="Customer"/>. The coordinates are
	/// approximate city positions, not addresses, and customers sharing a city are spread on a small circle so each
	/// marker stays selectable. A marker click, a row of the list and a navigation target all end in the same
	/// selection code, which reads the customer from the <c>Tag</c> or the bound row and never from a caption.
	/// </para>
	/// <para>
	/// Countries are shaded by revenue quintile for the period: five equal groups of ordering countries, because
	/// revenue per country spans two orders of magnitude and equal value bands would leave most countries in the
	/// lightest shade. The darkest shade therefore names a rank group, not an amount; the tooltip of a country and the
	/// list show the amounts. A country without orders in the period has no value and keeps the map's neutral color.
	/// </para>
	/// <para>
	/// A country filters the list. It arrives from a region click (<see cref="MapRegionEventArgs.FeatureId"/>) or as a
	/// navigation target from the Sales view's country chart, and shows as a closable <see cref="ChipLabel"/>.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// context.Navigate(DashboardArea.Customers, context.Database.FindCustomer("AROUT"));
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
		}

		#endregion

		#region Methods

		/// <summary>
		/// Selects the customer, or filters the list to the country, a navigation request carries.
		/// </summary>
		/// <param name="target">A <see cref="Customer"/> or a Northwind country name; other targets are ignored.</param>
		/// <remarks>
		/// The status strip is left alone: the sender of the request has already written its event path there.
		/// </remarks>
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
					FitCountry();
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

		/// <summary>
		/// Says what the map shows when the view opens.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);

			if (Visible && Dashboard != null)
				SetStatus("Markers from customers · Regions shaded by quintile");
		}

		#endregion

		#region Implementation

		private const ChipTone SelectedTone = ChipTone.Warning;

		// Close enough to tell the cities of one country apart while the shaded countries stay in view.
		private const int SelectionZoom = 5;

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
				StyleMarker(pair.Key, pair.Value);
		}

		// Customers sharing a city are spread on a small circle so each marker stays clickable at high zoom.
		// A customer whose city is not in GeoLocations gets no marker; it is still in the list.
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

		// The badge is the period revenue, a gray marker has no orders in the period, a pulsing one has open orders.
		// The selected customer's marker is orange.
		private void StyleMarker(Customer customer, MapMarker marker)
		{
			var customerSales = sales[customer];
			marker.Badge = customerSales.Orders > 0 ? DashboardStyle.CompactMoney(customerSales.Revenue) : "";
			marker.Tone = customer == selected ? SelectedTone : customerSales.Orders > 0 ? ChipTone.Primary : ChipTone.Neutral;
			marker.Pulse = customer.Orders.Any(o => Dashboard.ShippedDate(o) == null);
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
				if (countries.TryGetValue(name, out var countrySales) && countrySales.Revenue > 0)
				{
					properties["value"] = 5 - ranked.IndexOf(countrySales) * 5 / ranked.Count;
					properties["name"] = name + " · " + DashboardStyle.Money(countrySales.Revenue) + " · " + countrySales.Customers + " customers · " + countrySales.Orders + " orders";
				}
				else
				{
					properties["name"] = name + " · no orders in this period";
				}
			}

			return outlines.ToJsonString();
		}

		// The list holds the customers of the country, or every customer; the chip names the country and removes it.
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

			// A shorter list keeps the old scroll offset: show the selected row, or the top when it is not listed.
			if (!SelectRow(selected) && rows.Count > 0)
				customersGrid.ScrollRowIntoView(0);

			customersCard.Text = (country == null ? "Customers" : "Customers in " + country) + " · " + rows.Count +
				" · sales " + Dashboard.Period.Name.ToLowerInvariant();
			countryChip.Text = "Country: " + country;
			filterPanel.Visible = country != null;
			colChange.Visible = Dashboard.Period.HasComparison;
		}

		private void FitCountry()
		{
			var inCountry = markers.Where(m => m.Key.Country == country).Select(m => m.Value).ToArray();
			if (inCountry.Length > 0)
				map.FitBounds(inCountry);
		}

		private void Select(Customer customer, bool pan)
		{
			var previous = selected;
			selected = customer;

			if (previous != null && markers.TryGetValue(previous, out var previousMarker))
				StyleMarker(previous, previousMarker);

			ShowDetails(customer);
			SelectRow(customer);

			if (!markers.TryGetValue(customer, out var marker))
				return;

			StyleMarker(customer, marker);

			// Markers are drawn in collection order: the last one stays above its neighbors in the same city.
			map.Markers.Remove(marker);
			map.Markers.Add(marker);

			if (pan)
			{
				map.Zoom = Math.Max(map.Zoom, SelectionZoom);
				map.PanTo(marker.Location);
			}
		}

		// Highlights the customer's row and returns true when the list shows it; a customer outside the country filter
		// has no row.
		private bool SelectRow(Customer customer)
		{
			customersGrid.ClearSelection();
			if (customer == null)
				return false;

			for (int i = 0; i < customersGrid.RowCount; i++)
			{
				if (customersGrid.Rows[i].DataBoundItem is CustomerRow row && row.Customer == customer)
				{
					customersGrid.Rows[i].Selected = true;
					customersGrid.ScrollRowIntoView(i);
					return true;
				}
			}

			return false;
		}

		private void ShowDetails(Customer customer)
		{
			var period = Dashboard.Period;
			var customerSales = sales[customer];
			detailEmpty.Visible = false;
			detailCard.Text = customer.Company;

			customerAvatar.Text = customer.Company;
			customerAvatar.Label = customer.Company;
			customerAvatar.SubLabel = customer.City + " · " + customer.Contact;
			placeLabel.Text = customer.Address + ", " + customer.Place + Environment.NewLine + customer.Phone;

			revenueValue.Text = DashboardStyle.Money(customerSales.Revenue);
			revenueCaption.Text = "Revenue in period";
			ordersValue.Text = DashboardStyle.Count(customerSales.Orders);
			ordersCaption.Text = "Orders in period";
			var last = customer.Orders.LastOrDefault();
			lastOrderValue.Text = last == null ? "—" : DashboardStyle.Date(last.OrderDate);

			string unit = period.BucketSize switch { BucketSize.Day => "DAY", BucketSize.Week => "WEEK", _ => "MONTH" };
			trendLabel.Text = "TREND · REVENUE PER " + unit + " · " + period.Name.ToUpperInvariant();
			trendSparkline.Values = customerSales.Trend;

			// Clear before adding: the same Timeline shows one customer after another.
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
			{
				Select(customer, false);
				SetStatus("MapView.MarkerClick → e.Marker.Tag → customer");
			}
		}

		private void map_RegionClick(object sender, MapRegionEventArgs e)
		{
			country = e.FeatureId;
			ApplyFilter();
			SetStatus("MapView.RegionClick → e.FeatureId → country");
		}

		private void mapCard_ToolClick(object sender, ToolClickEventArgs e)
		{
			map.FitBounds();
			SetStatus("Panel.ToolClick → MapView.FitBounds()");
		}

		private void countryChip_CloseClick(object sender, EventArgs e)
		{
			country = null;
			ApplyFilter();
			SetStatus("ChipLabel.CloseClick → country filter removed");
		}

		private void detailCard_ToolClick(object sender, ToolClickEventArgs e)
		{
			if (selected != null)
			{
				Navigate(DashboardArea.Orders, selected);
				SetStatus("Panel.ToolClick → Navigate(Orders, customer)");
			}
		}

		private void customersGrid_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && customersGrid.Rows[e.RowIndex].DataBoundItem is CustomerRow row)
			{
				Select(row.Customer, true);
				SetStatus("DataGridView.CellClick → DataBoundItem → customer");
			}
		}

		private void recentOrders_ItemClick(object sender, TimelineItemEventArgs e)
		{
			if (e.Item.Tag is Order order)
			{
				OrderDialog.ShowOrder(Dashboard, order);
				SetStatus("Timeline.ItemClick → e.Item.Tag → OrderDialog");
			}
		}

		#endregion
	}
}
