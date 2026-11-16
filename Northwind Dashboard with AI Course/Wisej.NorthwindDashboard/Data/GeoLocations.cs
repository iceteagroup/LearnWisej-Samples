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

using System.Collections.Generic;
using Wisej.Web;

namespace Wisej.NorthwindDashboard.Data
{

	/// <summary>
	/// Approximate city-center coordinates for every Northwind customer city.
	/// </summary>
	/// <example>
	/// <code>
	/// if (GeoLocations.TryGetLocation(customer, out var point))
	/// 	map.Markers.Add(new MapMarker { Location = point, Text = customer.Company });
	/// </code>
	/// </example>
	public static class GeoLocations
	{

		#region Methods

		/// <summary>
		/// Returns the coordinates of the customer's city.
		/// </summary>
		/// <param name="customer">Customer to locate.</param>
		/// <param name="location">Receives the city coordinates.</param>
		/// <returns>True when the city is known.</returns>
		public static bool TryGetLocation(Customer customer, out GeoPoint location)
		{
			location = default;
			if (customer == null || !cities.TryGetValue(customer.City + "|" + customer.Country, out var point))
				return false;

			location = new GeoPoint(point.Latitude, point.Longitude);
			return true;
		}

		#endregion

		#region Implementation

		// Keyed by "City|Country" exactly as spelled in Northwind.
		private static readonly Dictionary<string, (double Latitude, double Longitude)> cities = new Dictionary<string, (double, double)>
		{
			["Buenos Aires|Argentina"] = (-34.60, -58.38),
			["Graz|Austria"] = (47.07, 15.44),
			["Salzburg|Austria"] = (47.81, 13.06),
			["Bruxelles|Belgium"] = (50.85, 4.35),
			["Charleroi|Belgium"] = (50.41, 4.44),
			["Campinas|Brazil"] = (-22.91, -47.06),
			["Resende|Brazil"] = (-22.47, -44.45),
			["Rio de Janeiro|Brazil"] = (-22.91, -43.17),
			["Sao Paulo|Brazil"] = (-23.55, -46.63),
			["Montréal|Canada"] = (45.50, -73.57),
			["Tsawassen|Canada"] = (49.01, -123.08),
			["Vancouver|Canada"] = (49.28, -123.12),
			["Århus|Denmark"] = (56.16, 10.20),
			["Kobenhavn|Denmark"] = (55.68, 12.57),
			["Helsinki|Finland"] = (60.17, 24.94),
			["Oulu|Finland"] = (65.01, 25.47),
			["Lille|France"] = (50.63, 3.06),
			["Lyon|France"] = (45.76, 4.84),
			["Marseille|France"] = (43.30, 5.37),
			["Nantes|France"] = (47.22, -1.55),
			["Paris|France"] = (48.86, 2.35),
			["Reims|France"] = (49.26, 4.03),
			["Strasbourg|France"] = (48.57, 7.75),
			["Toulouse|France"] = (43.60, 1.44),
			["Versailles|France"] = (48.80, 2.13),
			["Aachen|Germany"] = (50.78, 6.08),
			["Berlin|Germany"] = (52.52, 13.40),
			["Brandenburg|Germany"] = (52.41, 12.53),
			["Cunewalde|Germany"] = (51.10, 14.51),
			["Frankfurt a.M.|Germany"] = (50.11, 8.68),
			["Köln|Germany"] = (50.94, 6.96),
			["Leipzig|Germany"] = (51.34, 12.37),
			["Mannheim|Germany"] = (49.49, 8.47),
			["München|Germany"] = (48.14, 11.58),
			["Münster|Germany"] = (51.96, 7.63),
			["Stuttgart|Germany"] = (48.78, 9.18),
			["Cork|Ireland"] = (51.90, -8.47),
			["Bergamo|Italy"] = (45.70, 9.67),
			["Reggio Emilia|Italy"] = (44.70, 10.63),
			["Torino|Italy"] = (45.07, 7.69),
			["México D.F.|Mexico"] = (19.43, -99.13),
			["Stavern|Norway"] = (59.00, 10.04),
			["Warszawa|Poland"] = (52.23, 21.01),
			["Lisboa|Portugal"] = (38.72, -9.14),
			["Barcelona|Spain"] = (41.39, 2.17),
			["Madrid|Spain"] = (40.42, -3.70),
			["Sevilla|Spain"] = (37.39, -5.98),
			["Bräcke|Sweden"] = (62.75, 15.42),
			["Luleå|Sweden"] = (65.58, 22.15),
			["Bern|Switzerland"] = (46.95, 7.45),
			["Genève|Switzerland"] = (46.20, 6.14),
			["Cowes|UK"] = (50.76, -1.30),
			["London|UK"] = (51.51, -0.13),
			["Albuquerque|USA"] = (35.08, -106.65),
			["Anchorage|USA"] = (61.22, -149.90),
			["Boise|USA"] = (43.62, -116.21),
			["Butte|USA"] = (46.00, -112.53),
			["Elgin|USA"] = (45.57, -117.92),
			["Eugene|USA"] = (44.05, -123.09),
			["Kirkland|USA"] = (47.68, -122.21),
			["Lander|USA"] = (42.83, -108.73),
			["Portland|USA"] = (45.52, -122.68),
			["San Francisco|USA"] = (37.77, -122.42),
			["Seattle|USA"] = (47.61, -122.33),
			["Walla Walla|USA"] = (46.06, -118.34),
			["Barquisimeto|Venezuela"] = (10.07, -69.32),
			["Caracas|Venezuela"] = (10.49, -66.88),
			["I. de Margarita|Venezuela"] = (10.99, -63.93),
			["San Cristóbal|Venezuela"] = (7.77, -72.22)
		};

		#endregion

	}
}
