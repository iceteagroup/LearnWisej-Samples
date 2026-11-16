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
using System.Drawing;
using System.Globalization;
using Wisej.NorthwindDashboard.Data;
using Wisej.Web;

namespace Wisej.NorthwindDashboard.Views
{

	/// <summary>
	/// Formatting, color and icon conventions shared by the dashboard views.
	/// </summary>
	/// <example>
	/// <code>
	/// kpi.Value = DashboardStyle.CompactMoney(summary.Revenue);
	/// chart.Palette = DashboardStyle.CategoryPalette;
	/// </code>
	/// </example>
	public static class DashboardStyle
	{

		#region Properties

		/// <summary>
		/// Gets the categorical chart palette, in fixed slot order, validated for color-vision deficiencies on white.
		/// </summary>
		/// <remarks>
		/// Slot 1 is the brand indigo. Assign slots by entity (category or shipper identifier), never by rank, so a
		/// category keeps its color across charts and filters. Slots 3–5 are below 3:1 contrast on white, so charts using
		/// them keep a legend and a table view with the same values.
		/// </remarks>
		public static Color[] CategoryPalette => new[]
		{
			ColorTranslator.FromHtml("#4f46e5"),
			ColorTranslator.FromHtml("#eb6834"),
			ColorTranslator.FromHtml("#1baf7a"),
			ColorTranslator.FromHtml("#eda100"),
			ColorTranslator.FromHtml("#e87ba4"),
			ColorTranslator.FromHtml("#008300"),
			ColorTranslator.FromHtml("#2a78d6"),
			ColorTranslator.FromHtml("#e34948")
		};

		/// <summary>
		/// Gets the accent color for the emphasized series.
		/// </summary>
		public static Color Accent => ColorTranslator.FromHtml("#4f46e5");

		/// <summary>
		/// Gets the de-emphasis gray for comparison series.
		/// </summary>
		public static Color Comparison => ColorTranslator.FromHtml("#a3adbf");

		#endregion

		#region Methods

		/// <summary>
		/// Formats an amount as whole US dollars, such as "$12,345".
		/// </summary>
		/// <param name="value">Amount.</param>
		public static string Money(decimal value) => value.ToString("C0", Culture);

		/// <summary>
		/// Formats an amount with cents, such as "$12.50".
		/// </summary>
		/// <param name="value">Amount.</param>
		public static string MoneyCents(decimal value) => value.ToString("C2", Culture);

		/// <summary>
		/// Formats an amount compactly, such as "$440.6K" or "$1.27M".
		/// </summary>
		/// <param name="value">Amount.</param>
		public static string CompactMoney(decimal value)
		{
			decimal magnitude = Math.Abs(value);
			if (magnitude >= 1_000_000)
				return (value / 1_000_000).ToString("$#,0.00M", Culture);

			if (magnitude >= 10_000)
				return (value / 1_000).ToString("$#,0.0K", Culture);

			return value.ToString("C0", Culture);
		}

		/// <summary>
		/// Formats a count, such as "1,234".
		/// </summary>
		/// <param name="value">Count.</param>
		public static string Count(int value) => value.ToString("N0", Culture);

		/// <summary>
		/// Formats a percentage value (0–100) with one decimal, such as "95.4%".
		/// </summary>
		/// <param name="value">Percentage.</param>
		public static string Percent(double value) => value.ToString("0.0", Culture) + "%";

		/// <summary>
		/// Formats a date as "May 6, 1998".
		/// </summary>
		/// <param name="value">Date.</param>
		public static string Date(DateTime value) => value.ToString("MMM d, yyyy", Culture);

		/// <summary>
		/// Formats a signed percentage change, such as "+12.4%", or "—" when unavailable.
		/// </summary>
		/// <param name="change">Change in percent.</param>
		public static string Change(decimal? change) =>
			change is decimal value ? value.ToString("+0.0'%';-0.0'%';0'%'", Culture) : "—";

		/// <summary>
		/// Returns the categorical color of a category.
		/// </summary>
		/// <param name="category">Category.</param>
		public static Color CategoryColor(Category category) => CategoryPalette[(category.Id - 1) % 8];

		/// <summary>
		/// Returns the caption of an order status.
		/// </summary>
		/// <param name="status">Status.</param>
		public static string StatusText(OrderStatus status) => status switch
		{
			OrderStatus.Open => "Open",
			OrderStatus.Overdue => "Overdue",
			OrderStatus.Shipped => "Shipped",
			_ => "Shipped late"
		};

		/// <summary>
		/// Returns the chip tone of an order status.
		/// </summary>
		/// <param name="status">Status.</param>
		public static ChipTone StatusTone(OrderStatus status) => status switch
		{
			OrderStatus.Open => ChipTone.Primary,
			OrderStatus.Overdue => ChipTone.Danger,
			OrderStatus.Shipped => ChipTone.Success,
			_ => ChipTone.Warning
		};

		/// <summary>
		/// Returns the caption of a stock status.
		/// </summary>
		/// <param name="status">Status.</param>
		public static string StockText(StockStatus status) => status switch
		{
			StockStatus.InStock => "In stock",
			StockStatus.Incoming => "Incoming",
			StockStatus.Reorder => "Reorder",
			StockStatus.OutOfStock => "Out of stock",
			_ => "Discontinued"
		};

		/// <summary>
		/// Returns the chip tone of a stock status.
		/// </summary>
		/// <param name="status">Status.</param>
		public static ChipTone StockTone(StockStatus status) => status switch
		{
			StockStatus.InStock => ChipTone.Success,
			StockStatus.Incoming => ChipTone.Primary,
			StockStatus.Reorder => ChipTone.Warning,
			StockStatus.OutOfStock => ChipTone.Danger,
			_ => ChipTone.Neutral
		};

		/// <summary>
		/// Returns the icon path of a dashboard area.
		/// </summary>
		/// <param name="area">Area.</param>
		public static string AreaIcon(DashboardArea area) => "Assets/Icons/" + area.ToString().ToLowerInvariant() + ".svg";

		/// <summary>
		/// Returns the path of a named icon in Assets/Icons.
		/// </summary>
		/// <param name="name">File name without extension, such as "search".</param>
		public static string Icon(string name) => "Assets/Icons/" + name + ".svg";

		#endregion

		#region Implementation

		private static CultureInfo Culture => CultureInfo.CurrentCulture;

		#endregion

	}
}
