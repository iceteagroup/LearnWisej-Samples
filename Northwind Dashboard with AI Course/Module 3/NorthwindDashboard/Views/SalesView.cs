using System;
using System.Collections.Generic;
using System.Linq;
using NorthwindDashboard.Data;
using Wisej.Web;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Module 3 sales view: revenue by category over time with its table, the revenue bridge, the revenue trend, the
	/// largest markets and the product mix.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The view shows the two ways a <see cref="ChartView"/> gets its points. The stacked chart and the bridge are built
	/// from explicit <see cref="ChartPoint"/> objects, because their points are computed (one series per category, signed
	/// deltas between two totals). The trend, the country bars and the product mix are bound: the chart reads the rows
	/// assigned to <c>DataSource</c> through the series' <c>CategoryMember</c> and <c>ValueMember</c>, and each bound
	/// point carries its row in <c>Tag</c>.
	/// </para>
	/// <para>
	/// No chart here opens Orders. A country opens Customers and a product opens Products, each with the record the
	/// clicked point carries.
	/// </para>
	/// </remarks>
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

			comparisonSeries = revenueChart.Series[1];
			categoryTrendChart.Palette = DashboardStyle.CategoryPalette;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Reloads every chart and the table for the current period.
		/// </summary>
		protected override void RefreshView()
		{
			var period = Dashboard.Period;
			var categories = SalesAnalytics.CategorySales(Dashboard);

			UpdateCategoryTrend(period, categories);
			UpdateCategoryTable(period, categories);
			UpdateBridge(period, categories);
			UpdateRevenue(period);
			UpdateCountries();
			UpdateMix(period);

			// The shell writes its own line when Navigate or SetPeriod returns; posting ours makes the data path the line that stays.
			if (shownPeriod != period)
			{
				shownPeriod = period;
				Application.Post(() => SetStatus("Series.DataSource = rows · CategoryMember, ValueMember"));
			}
		}

		#endregion

		#region Implementation

		// Names of the categories the user switched off in the legend; kept so a period change does not bring them back.
		private readonly HashSet<string> hiddenCategories = new HashSet<string>();

		private readonly ChartSeries comparisonSeries;
		private DashboardPeriod shownPeriod;

		// One series per category, in identifier order, so a category keeps its palette slot in every period.
		// Every series has one point per bucket: stacking needs the series aligned on the same categories.
		private void UpdateCategoryTrend(DashboardPeriod period, IReadOnlyList<CategorySales> categories)
		{
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

		// The axis of a stacked chart ends at the largest sum of the visible series in one bucket,
		// not at the largest single point.
		private void RescaleCategoryTrend()
		{
			var visible = categoryTrendChart.Series.Where(s => s.Visible).ToArray();
			int buckets = visible.Length == 0 ? 0 : visible.Max(s => s.Points.Count);
			double max = Enumerable.Range(0, buckets).Select(i => visible.Sum(s => i < s.Points.Count ? s.Points[i].Value : 0)).DefaultIfEmpty(0).Max();
			categoryTrendChart.YAxis.Max = SalesAnalytics.NiceMaximum(max);

			// With every series switched off the chart shows its EmptyState text; say what is really the case.
			categoryTrendChart.EmptyState = visible.Length == 0 && categoryTrendChart.Series.Count > 0
				? "Every category is hidden · select a legend entry to show it"
				: "No orders in this period";
		}

		private void UpdateCategoryTable(DashboardPeriod period, IReadOnlyList<CategorySales> categories)
		{
			categoryGrid.DataSource = categories.OrderByDescending(c => c.Revenue).Select(c => new CategoryTableRow(c)).ToList();
			categoryGrid.CurrentCell = null;
			categoryGrid.ClearSelection();
			colCategoryChange.Visible = period.HasComparison;
		}

		// Total, signed change of each category, total. Without a comparison period there is no change to explain:
		// the series stays empty and the chart shows its EmptyState text.
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

		// Bound chart: both series read the same rows. The comparison series is removed, not zeroed, when the period
		// has nothing to compare with, and the same object is added back so there is never more than one.
		private void UpdateRevenue(DashboardPeriod period)
		{
			var trend = SalesAnalytics.RevenueTrend(Dashboard);

			bool compare = period.HasComparison;
			if (compare && !revenueChart.Series.Contains(comparisonSeries))
				revenueChart.Series.Add(comparisonSeries);
			else if (!compare)
				revenueChart.Series.Remove(comparisonSeries);

			revenueChart.Legend = compare ? ChartLegend.Top : ChartLegend.Hidden;
			revenueChart.YAxis.Max = SalesAnalytics.NiceMaximum(trend.Max(t => Math.Max(t.Revenue, compare ? t.PreviousRevenue : 0)));
			revenueChart.DataSource = trend;

			string unit = period.BucketSize switch { BucketSize.Day => "day", BucketSize.Week => "week", _ => "month" };
			revenueNote.Text = "Net revenue per " + unit + (compare ? " · dashed line: " + period.ComparisonText.Substring(4) : "");
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
				? "Top 20 products sized by revenue · blue grew and gray declined against the comparison period"
				: "Top 20 products sized by revenue";
		}

		// ChartView has already toggled e.Series.Visible when it raises LegendItemClick.
		private void categoryTrendChart_LegendItemClick(object sender, SeriesPointEventArgs e)
		{
			if (e.Series.Visible)
				hiddenCategories.Remove(e.Series.Name);
			else
				hiddenCategories.Add(e.Series.Name);

			RescaleCategoryTrend();
			SetStatus("LegendItemClick → hidden series → axis max");
		}

		private void categoryGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && categoryGrid.Rows[e.RowIndex].DataBoundItem is CategoryTableRow row)
			{
				SetStatus("CellDoubleClick → DataBoundItem → Products");
				Navigate(DashboardArea.Products, row.Sales.Category);
			}
		}

		private void bridgeChart_PointClick(object sender, SeriesPointEventArgs e) =>
			SetStatus("ChartPoint.IsTotal · signed deltas");

		// A bound point carries its row in Tag: the handler never reads the bar's caption to find the country.
		private void countriesChart_PointClick(object sender, SeriesPointEventArgs e)
		{
			if (e.Point.Tag is CountrySales country)
			{
				SetStatus("PointClick → e.Point.Tag → Customers");
				Navigate(DashboardArea.Customers, country.Country);
			}
		}

		private void mixChart_PointClick(object sender, SeriesPointEventArgs e)
		{
			if (e.Point.Tag is MixItem item)
			{
				SetStatus("PointClick → e.Point.Tag → Products");
				Navigate(DashboardArea.Products, item.Product);
			}
		}

		// Table row: the same category figures the stacked chart and the bridge are built from.
		private sealed class CategoryTableRow
		{
			public CategoryTableRow(CategorySales sales)
			{
				Sales = sales;
			}

			public CategorySales Sales { get; }

			public string Name => Sales.Name;

			public decimal Revenue => Sales.Revenue;

			public string ShareText => DashboardStyle.Percent(Sales.Share);

			public string ChangeText => DashboardStyle.Change(Sales.Change);
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
