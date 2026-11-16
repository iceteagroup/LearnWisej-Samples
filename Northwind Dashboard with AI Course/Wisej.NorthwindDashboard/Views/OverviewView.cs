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
using System.Linq;
using Wisej.NorthwindDashboard.Data;
using Wisej.NorthwindDashboard.Dialogs;
using Wisej.Web;

namespace Wisej.NorthwindDashboard.Views
{

	/// <summary>
	/// Headline metrics, revenue trend, category mix, top products, recent orders and order activity.
	/// </summary>
	/// <example>
	/// <code>
	/// var view = new OverviewView { Dock = DockStyle.Fill, Dashboard = context };
	/// </code>
	/// </example>
	public partial class OverviewView : DashboardView
	{

		#region Constructors

		/// <summary>
		/// Creates the view.
		/// </summary>
		public OverviewView()
		{
			InitializeComponent();

			comparisonSeries = revenueChart.Series[1];

			dueMeter.Thresholds.Add(new MeterThreshold(0, MeterTone.Success));
			dueMeter.Thresholds.Add(new MeterThreshold(1, MeterTone.Warning));
		}

		#endregion

		#region Methods

		/// <summary>
		/// Reloads every card from the session context.
		/// </summary>
		protected override void RefreshView()
		{
			var period = Dashboard.Period;
			var summary = SalesAnalytics.Summarize(Dashboard);

			UpdateKpis(summary, period);
			UpdateRevenue(period);
			UpdateCategories();
			UpdateProducts();
			UpdateActivity();
			UpdateOpenOrders();

			if (!heatmapLoaded)
				UpdateHeatmap();
		}

		#endregion

		#region Implementation

		private const int OrdersPerPage = 15;

		private readonly ChartSeries comparisonSeries;
		private int loadedOrders;
		private bool heatmapLoaded;

		private void UpdateKpis(PeriodSummary summary, DashboardPeriod period)
		{
			revenueKpi.Value = DashboardStyle.CompactMoney(summary.Revenue);
			revenueKpi.Trend = summary.RevenueChange;
			revenueKpi.Caption = period.ComparisonText;
			revenueKpi.SparklineValues = summary.RevenueTrend;

			ordersKpi.Value = DashboardStyle.Count(summary.Orders);
			ordersKpi.Trend = summary.OrdersChange;
			ordersKpi.Caption = DashboardStyle.Count(summary.Customers) + " customers · " + DashboardStyle.Count(summary.Units) + " units";
			ordersKpi.SparklineValues = summary.OrdersTrend;

			averageKpi.Value = DashboardStyle.Money(summary.AverageOrder);
			averageKpi.Trend = summary.AverageOrderChange;
			averageKpi.Caption = period.ComparisonText;
			averageKpi.SparklineValues = summary.AverageOrderTrend;

			double rate = summary.OnTimeRate;
			onTimeKpi.Value = summary.Shipped == 0 ? "—" : DashboardStyle.Percent(rate);
			onTimeKpi.ProgressValue = rate;
			onTimeKpi.Trend = summary.OnTimeChange;
			onTimeKpi.Tone = rate >= 95 ? KpiTone.Success : rate >= 90 ? KpiTone.Warning : KpiTone.Danger;
			onTimeKpi.Caption = summary.Shipped == 0
				? "No shipments yet"
				: summary.ShippedLate + " late of " + DashboardStyle.Count(summary.Shipped) + " shipped · " + summary.AverageDaysToShip.ToString("0.0") + " days avg.";
		}

		private void UpdateRevenue(DashboardPeriod period)
		{
			var trend = SalesAnalytics.RevenueTrend(Dashboard);

			// A single series needs no legend; the comparison is removed, not hidden, when no earlier data exists.
			bool compare = period.HasComparison;
			if (compare && !revenueChart.Series.Contains(comparisonSeries))
				revenueChart.Series.Add(comparisonSeries);
			else if (!compare)
				revenueChart.Series.Remove(comparisonSeries);

			revenueChart.Legend = compare ? ChartLegend.Top : ChartLegend.Hidden;
			revenueChart.YAxis.Max = SalesAnalytics.NiceMaximum(trend.Max(t => Math.Max(t.Revenue, compare ? t.PreviousRevenue : 0)));
			revenueChart.DataSource = trend;

			string unit = period.BucketSize switch { BucketSize.Day => "day", BucketSize.Week => "week", _ => "month" };
			revenueNote.Text = "Net merchandise value per " + unit
				+ (compare ? " · dashed line: " + period.ComparisonText.Substring(4) : "")
				+ (period.HasPartialBuckets && period.BucketSize == BucketSize.Month ? " · * partial month" : "");
		}

		private void UpdateCategories()
		{
			var categories = SalesAnalytics.CategorySales(Dashboard).OrderByDescending(c => c.Revenue).ToArray();
			categoryChart.YAxis.Max = SalesAnalytics.NiceMaximum((double)categories.Max(c => c.Revenue));
			categoryChart.DataSource = categories;
		}

		private void UpdateProducts()
		{
			var rows = SalesAnalytics.ProductSales(Dashboard).Take(8).Select(s => new ProductRow(s, Dashboard)).ToList();
			colShare.Maximum = Math.Max(1, Math.Ceiling(rows.Max(r => r.Share) / 5) * 5);
			colChange.Visible = Dashboard.Period.HasComparison;
			productsGrid.DataSource = rows;
			productsGrid.CurrentCell = null;
			productsGrid.ClearSelection();
		}

		private void UpdateActivity()
		{
			activityTimeline.Items.Clear();
			foreach (var entry in Dashboard.Activity)
			{
				activityTimeline.Items.Add(new TimelineItem(entry.Time, entry.Title)
				{
					Description = entry.Description,
					Tone = TimelineTone.Success,
					Tag = entry.Target
				});
			}

			loadedOrders = 0;
			AppendOrders();
		}

		private void AppendOrders()
		{
			var orders = Dashboard.Database.Orders;
			foreach (var order in orders.Reverse().Skip(loadedOrders).Take(OrdersPerPage))
			{
				var status = Dashboard.StatusOf(order);
				activityTimeline.Items.Add(new TimelineItem(order.OrderDate, order.Customer.Company)
				{
					Description = "Order " + order.Number + " · " + DashboardStyle.MoneyCents(order.Subtotal) + " · " + order.Employee.FullName + " · " + DashboardStyle.StatusText(status),
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

			loadedOrders = Math.Min(orders.Count, loadedOrders + OrdersPerPage);
			activityTimeline.HasMoreItems = loadedOrders < orders.Count;
		}

		private void UpdateHeatmap()
		{
			var end = Dashboard.AsOf;
			var start = end.AddDays(-364);
			ordersHeatmap.SetDateRange(start, end);
			ordersHeatmap.DataSource = SalesAnalytics.DailyOrders(Dashboard, start, end);
			heatmapLoaded = true;
		}

		private void UpdateOpenOrders()
		{
			var open = Dashboard.OpenOrders().ToArray();
			int overdue = open.Count(o => o.RequiredDate < Dashboard.AsOf);
			openOrdersValue.Text = open.Length == 1 ? "1 order" : DashboardStyle.Count(open.Length) + " orders";
			openOrdersCaption.Text = DashboardStyle.Money(open.Sum(o => o.Subtotal)) + " awaiting shipment" + (overdue > 0 ? " · " + overdue + " overdue" : "");

			int created = open.Count(o => Dashboard.StageOf(o) == FulfillmentStage.New);
			int picking = open.Count(o => Dashboard.StageOf(o) == FulfillmentStage.Picking);
			int packed = open.Count(o => Dashboard.StageOf(o) == FulfillmentStage.Packed);
			stagesMeter.Maximum = Math.Max(1, open.Length);
			stagesMeter.Value = open.Length;
			stagesMeter.Segments.Clear();
			stagesMeter.Segments.Add(new MeterSegment("New", created, MeterTone.Neutral));
			stagesMeter.Segments.Add(new MeterSegment("Picking", picking, MeterTone.Primary));
			stagesMeter.Segments.Add(new MeterSegment("Packed", packed, MeterTone.Success));
			stagesMeter.ValueText = created + " new · " + picking + " picking · " + packed + " packed";

			dueMeter.Maximum = Math.Max(1, open.Length);
			dueMeter.Value = open.Count(o => o.RequiredDate <= Dashboard.AsOf.AddDays(7));
		}

		private void revenueKpi_Click(object sender, EventArgs e) => Dashboard?.Navigate(DashboardArea.Sales);

		private void ordersKpi_Click(object sender, EventArgs e) => Dashboard?.Navigate(DashboardArea.Orders);

		private void onTimeKpi_Click(object sender, EventArgs e) => Dashboard?.Navigate(DashboardArea.Fulfillment);

		private void fulfillmentButton_Click(object sender, EventArgs e) => Dashboard?.Navigate(DashboardArea.Fulfillment);

		private void revenueCard_ToolClick(object sender, ToolClickEventArgs e)
		{
			if (e.Tool.Name == "export")
				revenueChart.ExportImage(ChartImageFormat.Png, "northwind-revenue");
		}

		private void productsCard_ToolClick(object sender, ToolClickEventArgs e) => Dashboard?.Navigate(DashboardArea.Products);

		private void categoryChart_PointClick(object sender, SeriesPointEventArgs e)
		{
			if (e.Point.Tag is CategorySales sales)
				Dashboard?.Navigate(DashboardArea.Products, sales.Category);
		}

		private void productsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && productsGrid.Rows[e.RowIndex].DataBoundItem is ProductRow row)
				Dashboard?.Navigate(DashboardArea.Products, row.Product);
		}

		private void activityTimeline_ItemClick(object sender, TimelineItemEventArgs e)
		{
			switch (e.Item.Tag)
			{
				case Order order:
					OrderDialog.ShowOrder(Dashboard, order);
					break;

				case Product product:
					Dashboard.Navigate(DashboardArea.Products, product);
					break;
			}
		}

		private void activityTimeline_LoadMore(object sender, EventArgs e) => AppendOrders();

		private void ordersHeatmap_CellClick(object sender, HeatmapCellEventArgs e) => Dashboard?.Navigate(DashboardArea.Orders, e.Date);

		#endregion

	}
}
