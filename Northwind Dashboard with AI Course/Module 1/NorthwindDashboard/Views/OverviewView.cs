using NorthwindDashboard.Data;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Module 1 overview: revenue, orders and average order for the selected period, and the period's label.
	/// </summary>
	/// <remarks>
	/// The three cards and the label are filled from the session context only, so changing the period selector in the
	/// shell is observable here without any code that links the two.
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
		/// Reloads the cards and the period label from the session context.
		/// </summary>
		protected override void RefreshView()
		{
			var period = Dashboard.Period;
			var summary = SalesAnalytics.Summarize(Dashboard);

			// A null trend hides the indicator: "All time" has no earlier period to compare with.
			revenueKpi.Value = DashboardStyle.MoneyCents(summary.Revenue);
			revenueKpi.Trend = summary.RevenueChange;

			ordersKpi.Value = DashboardStyle.Count(summary.Orders);
			ordersKpi.Trend = summary.OrdersChange;

			averageKpi.Value = DashboardStyle.MoneyCents(summary.AverageOrder);
			averageKpi.Trend = summary.AverageOrderChange;

			periodLabel.Text = "Period label: " + period.Name + " · " +
				period.Start.ToString("yyyy-MM-dd") + " to " + period.End.ToString("yyyy-MM-dd");
		}

		#endregion

	}
}
