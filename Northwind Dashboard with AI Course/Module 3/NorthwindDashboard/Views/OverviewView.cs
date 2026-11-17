using System;
using System.Linq;
using NorthwindDashboard.Data;
using Wisej.Web;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Module 2 overview: four metric cards with their miniature visual, the open orders by warehouse step and the
	/// comparison the trends are measured against.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A <see cref="KpiPanel"/> calculates nothing. <see cref="SalesAnalytics.Summarize"/> returns the figures and this
	/// view assigns <c>Value</c>, <c>Trend</c> and <c>SparklineValues</c> (or <c>ProgressValue</c> for the gauge), so the
	/// headline, the change and the miniature always come from the same summary.
	/// </para>
	/// <para>
	/// A period without an earlier period to compare with has a null trend: the card shows no indicator and the note
	/// under the cards says so. That is different from a change of zero.
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
		/// Reloads the cards, the order stages and the notes from the session context.
		/// </summary>
		protected override void RefreshView()
		{
			var period = Dashboard.Period;
			var summary = SalesAnalytics.Summarize(Dashboard);

			UpdateKpis(summary);
			UpdateStages();

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

		private DashboardPeriod shownPeriod;

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

		#endregion

	}
}
