using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using NorthwindDashboard.Data;
using Wisej.Web;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Module 3 customers view: the customer map, the card of the selected customer and the customer list, which a
	/// country arriving from the Sales view filters.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Every customer is one <see cref="MapMarker"/> whose <c>Tag</c> is the <see cref="Customer"/>. A customer arrives
	/// as a navigation target, from a marker click or from a row of the list, and all three end in the same selection
	/// code.
	/// </para>
	/// <para>
	/// A country arrives as a navigation target too: the Sales view's country chart passes the Northwind country name
	/// (a <see cref="string"/>, such as "UK"). The view shows it as a closable <see cref="ChipLabel"/> and lists that
	/// country's customers; closing the chip lists every customer again.
	/// </para>
	/// <para>
	/// Countries are shaded by revenue quintile for the period: five equal groups of ordering countries, because revenue
	/// per country spans two orders of magnitude and equal value bands would leave most countries in the lightest shade.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// context.Navigate(DashboardArea.Customers, context.Database.FindCustomer("AROUT"));
	/// context.Navigate(DashboardArea.Customers, "UK");
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
					Select(customer, true, "Selected through a navigation target");
					break;

				case string name:
					country = name;
					ApplyFilter();
					FitCountry();
					break;
			}
		}

		/// <summary>
		/// Shades the countries for the current period, creates the markers on first use and fills the list.
		/// </summary>
		protected override void RefreshView()
		{
			map.Regions = BuildRegions();

			if (markers.Count == 0)
				CreateMarkers();

			ApplyFilter();
		}

		#endregion

		#region Implementation

		private const ChipTone MarkerTone = ChipTone.Primary;
		private const ChipTone SelectedTone = ChipTone.Warning;

		// Close enough to tell the cities of one country apart while the shaded countries stay in view.
		private const int SelectionZoom = 5;

		private readonly Dictionary<Customer, MapMarker> markers = new Dictionary<Customer, MapMarker>();
		private Customer selected;
		private string country;

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

					var marker = new MapMarker { Location = point, Text = group[i].Company, Tone = MarkerTone, Tag = group[i] };
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

		// The list holds the customers of the country, or every customer; the chip names the country and removes it.
		private void ApplyFilter()
		{
			var rows = Dashboard.Database.Customers
				.Where(c => country == null || c.Country == country)
				.OrderBy(c => c.Company)
				.Select(c => new CustomerListRow(c))
				.ToList();

			customersGrid.DataSource = rows;
			customersGrid.CurrentCell = null;

			// A shorter list keeps the old scroll offset: show the selected row, or the top when it is not listed.
			if (!SelectRow(selected) && rows.Count > 0)
				customersGrid.ScrollRowIntoView(0);

			listLabel.Text = "CUSTOMERS · " + rows.Count;
			countryChip.Text = "Country: " + country;
			countryChip.Visible = country != null;
		}

		private void FitCountry()
		{
			var inCountry = markers.Where(m => m.Key.Country == country).Select(m => m.Value).ToArray();
			if (inCountry.Length > 0)
				map.FitBounds(inCountry);
		}

		private void Select(Customer customer, bool pan, string source)
		{
			if (selected != null && markers.TryGetValue(selected, out var previous))
			{
				previous.Tone = MarkerTone;
				previous.Badge = "";
			}

			selected = customer;

			customerAvatar.Text = customer.Company;
			customerAvatar.Label = customer.Company;
			customerAvatar.SubLabel = customer.City + " · " + customer.Country;
			sourceLabel.Text = source;
			sourceLabel.Visible = true;

			SelectRow(customer);

			if (!markers.TryGetValue(customer, out var marker))
				return;

			marker.Tone = SelectedTone;
			marker.Badge = customer.City;

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
				if (customersGrid.Rows[i].DataBoundItem is CustomerListRow row && row.Customer == customer)
				{
					customersGrid.Rows[i].Selected = true;
					customersGrid.ScrollRowIntoView(i);
					return true;
				}
			}

			return false;
		}

		private void map_MarkerClick(object sender, MapMarkerEventArgs e)
		{
			if (e.Marker.Tag is Customer customer)
			{
				Select(customer, false, "Selected through MarkerClick");
				SetStatus("MapView.MarkerClick → e.Marker.Tag → customer");
			}
		}

		private void customersGrid_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && customersGrid.Rows[e.RowIndex].DataBoundItem is CustomerListRow row)
			{
				Select(row.Customer, true, "Selected through the customer list");
				SetStatus("DataGridView.CellClick → DataBoundItem → customer");
			}
		}

		private void countryChip_CloseClick(object sender, EventArgs e)
		{
			country = null;
			ApplyFilter();
			map.FitBounds();
			SetStatus("ChipLabel.CloseClick → country filter removed");
		}

		/// <summary>
		/// One row of the customer list: the company, and the city and country in one cell.
		/// </summary>
		private sealed class CustomerListRow
		{
			public CustomerListRow(Customer customer) => Customer = customer;

			public Customer Customer { get; }

			public string Company => Customer.Company;

			public string Place => Customer.City + " · " + Customer.Country;
		}

		#endregion

	}
}
