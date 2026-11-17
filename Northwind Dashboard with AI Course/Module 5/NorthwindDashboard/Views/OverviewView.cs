using System;
using System.Linq;
using NorthwindDashboard.Data;
using NorthwindDashboard.Dialogs;
using Wisej.Web;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Module 5 overview: the metric cards and order stages of Module 2, the activity <see cref="Timeline"/> and the
	/// twelve-month order <see cref="HeatmapCalendar"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A <see cref="KpiPanel"/> calculates nothing. <see cref="SalesAnalytics.Summarize"/> returns the figures and this
	/// view assigns <c>Value</c>, <c>Trend</c> and <c>SparklineValues</c> (or <c>ProgressValue</c> for the gauge), so the
	/// headline, the change and the miniature always come from the same summary.
	/// </para>
	/// <para>
	/// The timeline lists what the session did (orders shipped, purchase orders placed) above the most recent orders.
	/// Each <see cref="TimelineItem"/> carries its record in <c>Tag</c>, and <c>LoadMore</c> appends the next page
	/// without touching the items already shown. A heatmap cell carries its date: clicking it opens Orders for that day.
	/// </para>
	/// </remarks>
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
		}

		#endregion

		#region Methods

		/// <summary>
		/// Reloads the cards, the order stages, the activity and the notes from the session context.
		/// </summary>
		protected override void RefreshView()
		{
			var period = Dashboard.Period;
			var summary = SalesAnalytics.Summarize(Dashboard);

			UpdateKpis(summary);
			UpdateStages();
			UpdateActivity();

			// The heatmap always covers the last twelve months, whatever the period: load it once.
			if (!heatmapLoaded)
				UpdateHeatmap();

			comparisonLabel.Text = period.HasComparison
				? "Comparison: " + ComparisonName(period) + " · " + period.ComparisonText.Substring(4)
				: "No comparison period: Trend suppressed";
			periodLabel.Text = "Period label: " + period.Name + " · " +
				period.Start.ToString("yyyy-MM-dd") + " to " + period.End.ToString("yyyy-MM-dd");

			// The shell writes its own line when SetPeriod returns; posting ours makes the data path the line that stays.
			if (shownPeriod != period)
			{
				shownPeriod = period;
				Application.Post(() => SetStatus("SalesAnalytics.Summary → KpiPanel.Value, Trend, SparklineValues"));
			}
		}

		#endregion

		#region Implementation

		// Orders added to the timeline by the first load and by each LoadMore.
		private const int OrdersPerPage = 15;

		private DashboardPeriod shownPeriod;
		private int loadedOrders;
		private bool heatmapLoaded;

		private void UpdateKpis(PeriodSummary summary)
		{
			// A null trend hides the indicator: "All time" has no earlier period to compare with.
			revenueKpi.Value = DashboardStyle.MoneyCents(summary.Revenue);
			revenueKpi.Trend = summary.RevenueChange;
			revenueKpi.SparklineValues = summary.RevenueTrend;

			ordersKpi.Value = DashboardStyle.Count(summary.Orders);
			ordersKpi.Trend = summary.OrdersChange;
			ordersKpi.SparklineValues = summary.OrdersTrend;

			averageKpi.Value = DashboardStyle.MoneyCents(summary.AverageOrder);
			averageKpi.Trend = summary.AverageOrderChange;
			averageKpi.SparklineValues = summary.AverageOrderTrend;

			// No shipments is not 0% on time: the card shows a dash, an empty gauge and says why.
			double rate = summary.OnTimeRate;
			onTimeKpi.Value = summary.Shipped == 0 ? "—" : DashboardStyle.Percent(rate);
			onTimeKpi.ProgressValue = rate;
			onTimeKpi.Trend = summary.OnTimeChange;
			onTimeKpi.Tone = summary.Shipped == 0 ? KpiTone.Neutral
				: rate >= 95 ? KpiTone.Success : rate >= 90 ? KpiTone.Warning : KpiTone.Danger;
			onTimeKpi.Caption = summary.Shipped == 0
				? "No shipments yet"
				: summary.ShippedLate + " late of " + DashboardStyle.Count(summary.Shipped) + " shipped";
		}

		// The segments are parts of one total, the open orders. Clear before adding: this runs on every refresh.
		private void UpdateStages()
		{
			var open = Dashboard.OpenOrders().ToArray();
			int created = open.Count(o => Dashboard.StageOf(o) == FulfillmentStage.New);
			int picking = open.Count(o => Dashboard.StageOf(o) == FulfillmentStage.Picking);
			int packed = open.Count(o => Dashboard.StageOf(o) == FulfillmentStage.Packed);

			stagesMeter.Maximum = Math.Max(1, open.Length);
			stagesMeter.Value = open.Length;
			stagesMeter.Segments.Clear();
			stagesMeter.Segments.Add(new MeterSegment("New", created, MeterTone.Neutral));
			stagesMeter.Segments.Add(new MeterSegment("Picking", picking, MeterTone.Primary));
			stagesMeter.Segments.Add(new MeterSegment("Packed", packed, MeterTone.Success));
			stagesMeter.ValueText = created + " new · " + picking + " picking · " + packed + " packed · " +
				open.Length + " open";
		}

		// Rebuilt on every refresh: the session's own events first, then the first page of recent orders.
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

		// Adds the next page after the ones already shown, newest first; never re-adds an order.
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

		private static string ComparisonName(DashboardPeriod period) => period.Kind switch
		{
			PeriodKind.Last30Days => "previous 30 days",
			PeriodKind.Last90Days => "previous 90 days",
			_ => "same dates one year earlier"
		};

		private void revenueKpi_Click(object sender, EventArgs e)
		{
			SetStatus("KpiPanel.Click → Sales");
			Navigate(DashboardArea.Sales);
		}

		private void ordersKpi_Click(object sender, EventArgs e)
		{
			SetStatus("KpiPanel.Click → Orders");
			Navigate(DashboardArea.Orders);
		}

		private void onTimeKpi_Click(object sender, EventArgs e)
		{
			SetStatus("KpiPanel.Click → Fulfillment");
			Navigate(DashboardArea.Fulfillment);
		}

		// The Tag says what the item is about: an order opens its details, a product opens the catalog on that product.
		private void activityTimeline_ItemClick(object sender, TimelineItemEventArgs e)
		{
			switch (e.Item.Tag)
			{
				case Order order:
					SetStatus("Timeline.ItemClick → e.Item.Tag → OrderDialog");
					OrderDialog.ShowOrder(Dashboard, order);
					break;

				case Product product:
					SetStatus("Timeline.ItemClick → e.Item.Tag → Products");
					Navigate(DashboardArea.Products, product);
					break;
			}
		}

		private void activityTimeline_LoadMore(object sender, EventArgs e)
		{
			AppendOrders();
			SetStatus("Timeline.LoadMore → next " + OrdersPerPage + " orders · " + loadedOrders + " shown");
		}

		// The cell's date is the target: Orders lists that day whatever the selected period is.
		private void ordersHeatmap_CellClick(object sender, HeatmapCellEventArgs e)
		{
			SetStatus("HeatmapCalendar.CellClick → day scope");
			Navigate(DashboardArea.Orders, e.Date);
		}

		#endregion

	}
}
