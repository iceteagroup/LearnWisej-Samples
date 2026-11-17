using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using NorthwindDashboard.Data;
using Wisej.Web;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Module 1 customers view: the customer map and the card of the selected customer.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Every customer is one <see cref="MapMarker"/> whose <c>Tag</c> is the <see cref="Customer"/>. A customer arrives
	/// either as a navigation target (the command palette's customer commands) or from a marker click, and both end in
	/// the same selection code.
	/// </para>
	/// <para>
	/// Countries are shaded by revenue quintile for the period: five equal groups of ordering countries, because revenue
	/// per country spans two orders of magnitude and equal value bands would leave most countries in the lightest shade.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// context.Navigate(DashboardArea.Customers, context.Database.FindCustomer("AROUT"));
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
		/// Selects the customer a navigation request carries.
		/// </summary>
		/// <param name="target">A <see cref="Customer"/>; other targets are ignored.</param>
		public override void ShowTarget(object target)
		{
			if (Dashboard != null && target is Customer customer)
				Select(customer, true, "Selected through CommandExecuted");
		}

		/// <summary>
		/// Shades the countries for the current period and creates the markers on first use.
		/// </summary>
		protected override void RefreshView()
		{
			map.Regions = BuildRegions();

			if (markers.Count == 0)
				CreateMarkers();
		}

		#endregion

		#region Implementation

		private const ChipTone MarkerTone = ChipTone.Primary;
		private const ChipTone SelectedTone = ChipTone.Warning;

		// Close enough to tell the cities of one country apart while the shaded countries stay in view.
		private const int SelectionZoom = 5;

		private readonly Dictionary<Customer, MapMarker> markers = new Dictionary<Customer, MapMarker>();
		private Customer selected;

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

		private void map_MarkerClick(object sender, MapMarkerEventArgs e)
		{
			if (e.Marker.Tag is Customer customer)
			{
				Select(customer, false, "Selected through MarkerClick");
				SetStatus("MapView.MarkerClick → e.Marker.Tag → customer");
			}
		}

		#endregion

	}
}
