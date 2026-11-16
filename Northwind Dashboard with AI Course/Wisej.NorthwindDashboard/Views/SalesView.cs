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
using Wisej.NorthwindDashboard.Data;
using Wisej.Web;

namespace Wisej.NorthwindDashboard.Views
{

	/// <summary>
	/// Revenue analysis: category trend and table, revenue bridge, markets, product mix and a per-period table.
	/// </summary>
	/// <example>
	/// <code>
	/// var view = new SalesView { Dock = DockStyle.Fill, Dashboard = context };
	/// </code>
	/// </example>
	public partial class SalesView : DashboardView
	{

		#region Constructors

		/// <summary>
		/// Creates the view.
		/// </summary>
		public SalesView()
		{
			InitializeComponent();

			categoryTrendChart.Palette = DashboardStyle.CategoryPalette;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Reloads every chart and table for the current period.
		/// </summary>
		protected override void RefreshView()
		{
			var period = Dashboard.Period;
			var categories = SalesAnalytics.CategorySales(Dashboard);

			UpdateCategoryTrend(period, categories);
			categoryGrid.DataSource = categories.OrderByDescending(c => c.Revenue).Select(c => new CategoryRow(c)).ToList();
			categoryGrid.CurrentCell = null;
			colCategoryChange.Visible = period.HasComparison;

			UpdateBridge(period, categories);
			UpdateCountries();
			UpdateMix(period);
			UpdatePeriods(period);
		}

		#endregion

		#region Implementation

		private readonly HashSet<string> hiddenCategories = new HashSet<string>();

		private void UpdateCategoryTrend(DashboardPeriod period, IReadOnlyList<CategorySales> categories)
		{
			// One series per category in identifier order keeps each category on the same palette slot.
			categoryTrendChart.Series.Clear();
			foreach (var category in categories)
			{
				var series = new ChartSeries
				{
					Name = category.Name,
					ChartType = ChartType.StackedColumn,
					LabelFormat = "{0:C0}",
					Visible = !hiddenCategories.Contains(category.Name)
				};
				for (int i = 0; i < period.Buckets.Count; i++)
					series.Points.Add(new ChartPoint { Category = period.Buckets[i].Label, Value = category.Trend[i], Tag = category });

				categoryTrendChart.Series.Add(series);
			}

			RescaleCategoryTrend();

			string unit = period.BucketSize switch { BucketSize.Day => "day", BucketSize.Week => "week", _ => "month" };
			categoryTrendNote.Text = "Stacked net revenue per " + unit + " · select a legend entry to hide a category" +
				(period.HasPartialBuckets && period.BucketSize == BucketSize.Month ? " · * partial month" : "");
		}

		private void RescaleCategoryTrend()
		{
			var visible = categoryTrendChart.Series.Where(s => s.Visible).ToArray();
			int buckets = visible.Length == 0 ? 0 : visible.Max(s => s.Points.Count);
			double max = Enumerable.Range(0, buckets).Select(i => visible.Sum(s => i < s.Points.Count ? s.Points[i].Value : 0)).DefaultIfEmpty(0).Max();
			categoryTrendChart.YAxis.Max = SalesAnalytics.NiceMaximum(max);
		}

		private void UpdateBridge(DashboardPeriod period, IReadOnlyList<CategorySales> categories)
		{
			var series = bridgeChart.Series[0];
			series.Points.Clear();

			if (!period.HasComparison)
			{
				bridgeNote.Text = period.Name + " has no complete comparison period in the recorded history";
				return;
			}

			decimal previous = categories.Sum(c => c.PreviousRevenue);
			decimal current = categories.Sum(c => c.Revenue);
			series.Points.Add(new ChartPoint { Category = "Comparison period", Value = (double)previous, IsTotal = true });

			decimal running = previous, peak = Math.Max(previous, current);
			foreach (var category in categories.OrderByDescending(c => c.Revenue - c.PreviousRevenue))
			{
				decimal delta = category.Revenue - category.PreviousRevenue;
				running += delta;
				peak = Math.Max(peak, running);
				series.Points.Add(new ChartPoint { Category = category.Name, Value = (double)delta, Tag = category });
			}

			series.Points.Add(new ChartPoint { Category = "This period", Value = (double)current, IsTotal = true });
			bridgeChart.YAxis.Min = 0;
			bridgeChart.YAxis.Max = SalesAnalytics.NiceMaximum((double)peak);
			bridgeNote.Text = "From " + DashboardStyle.Money(previous) + " (" + period.ComparisonText.Substring(4) + ") to " +
				DashboardStyle.Money(current) + " (" + period.RangeText + ") · " + DashboardStyle.Change(period.Change(current, previous));
		}

		private void UpdateCountries()
		{
			var countries = SalesAnalytics.CountrySales(Dashboard).Where(c => c.Revenue > 0).Take(10).ToArray();
			countriesChart.YAxis.Max = SalesAnalytics.NiceMaximum(countries.Length == 0 ? 0 : (double)countries.Max(c => c.Revenue));
			countriesChart.DataSource = countries;
		}

		private void UpdateMix(DashboardPeriod period)
		{
			bool compare = period.HasComparison;
			mixChart.DataSource = SalesAnalytics.ProductSales(Dashboard)
				.Where(p => p.Revenue > 0)
				.Take(20)
				.Select(p => new MixItem(p, compare))
				.ToArray();
			mixNote.Text = compare
				? "Top 20 products sized by revenue · indigo grew and gray declined against the comparison period"
				: "Top 20 products sized by revenue";
		}

		private void UpdatePeriods(DashboardPeriod period)
		{
			periodGrid.DataSource = SalesAnalytics.RevenueTrend(Dashboard).Reverse().Select(p => new TrendRow(p, period.HasComparison)).ToList();
			periodGrid.CurrentCell = null;
			colPeriodPrevious.Visible = colPeriodChange.Visible = period.HasComparison;
		}

		private void categoryTrendCard_ToolClick(object sender, ToolClickEventArgs e)
		{
			if (e.Tool.Name == "export")
				categoryTrendChart.ExportImage(ChartImageFormat.Png, "northwind-category-revenue");
		}

		private void categoryTrendChart_LegendItemClick(object sender, SeriesPointEventArgs e)
		{
			if (e.Series.Visible)
				hiddenCategories.Remove(e.Series.Name);
			else
				hiddenCategories.Add(e.Series.Name);

			RescaleCategoryTrend();
		}

		private void categoryGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && categoryGrid.Rows[e.RowIndex].DataBoundItem is CategoryRow row)
				Dashboard?.Navigate(DashboardArea.Products, row.Sales.Category);
		}

		private void countriesChart_PointClick(object sender, SeriesPointEventArgs e)
		{
			if (e.Point.Tag is CountrySales country)
				Dashboard?.Navigate(DashboardArea.Customers, country.Country);
		}

		private void mixChart_PointClick(object sender, SeriesPointEventArgs e)
		{
			if (e.Point.Tag is MixItem item)
				Dashboard?.Navigate(DashboardArea.Products, item.Product);
		}

		// Treemap tile: size is revenue; the sign of Change selects the grew/declined color.
		private sealed class MixItem
		{
			public MixItem(ProductSales sales, bool compare)
			{
				Product = sales.Product;
				Revenue = (double)sales.Revenue;
				if (compare)
					Change = sales.PreviousRevenue == 0 ? 1 : (double)(sales.Revenue - sales.PreviousRevenue);
			}

			public Product Product { get; }

			public string Name => Product.Name;

			public double Revenue { get; }

			public double? Change { get; }
		}

		#endregion

	}
}
