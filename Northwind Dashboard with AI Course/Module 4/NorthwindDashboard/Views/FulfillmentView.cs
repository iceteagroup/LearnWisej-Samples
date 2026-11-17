using System;
using System.Globalization;
using NorthwindDashboard.Data;
using Wisej.Web;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Module 2 fulfillment view: one <see cref="RadialGauge"/> per shipper with the on-time rate of the period.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The rate is the share of the period's shipped orders that left on or before their required date, passed to the
	/// gauge on a 0 to 100 scale. Three <see cref="MeterThreshold"/> entries set the tone: danger from 0, warning from
	/// 90 and success from 95.
	/// </para>
	/// <para>
	/// A shipper without shipments in the period has no rate. Its gauge shows a dash on an empty track instead of
	/// 0%, which would read as the worst possible result.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// context.Navigate(DashboardArea.Fulfillment);
	/// </code>
	/// </example>
	public partial class FulfillmentView : DashboardView
	{

		#region Constructors

		/// <summary>
		/// Creates the view.
		/// </summary>
		public FulfillmentView()
		{
			InitializeComponent();

			gauges = new[] { shipper1Gauge, shipper2Gauge, shipper3Gauge };
			names = new[] { shipper1Name, shipper2Name, shipper3Name };
			stats = new[] { shipper1Stats, shipper2Stats, shipper3Stats };
			foreach (var gauge in gauges)
			{
				gauge.Thresholds.Add(new MeterThreshold(0, MeterTone.Danger));
				gauge.Thresholds.Add(new MeterThreshold(WarningFrom, MeterTone.Warning));
				gauge.Thresholds.Add(new MeterThreshold(SuccessFrom, MeterTone.Success));
			}

			thresholdsLabel.Text = "On-time thresholds" + Environment.NewLine +
				"warning ≥ " + WarningFrom + " · success ≥ " + SuccessFrom;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Reloads the shipper gauges for the current period.
		/// </summary>
		protected override void RefreshView()
		{
			UpdateShippers();

			if (Visible)
				PostStatus(GaugeStatus);
		}

		/// <summary>
		/// Names the gauge calculation in the status strip when the view is shown.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);

			if (Visible && Dashboard != null)
				PostStatus(GaugeStatus);
		}

		#endregion

		#region Implementation

		private const int WarningFrom = 90;
		private const int SuccessFrom = 95;
		private const string RateFormat = "{0:N1}%";

		// A format without a placeholder: the gauge shows the dash whatever its value is.
		private const string NoRateFormat = "—";

		private const string GaugeStatus = "RadialGauge.Value = onTime / shipped";

		private readonly RadialGauge[] gauges;
		private readonly Label[] names;
		private readonly Label[] stats;

		private void UpdateShippers()
		{
			var performance = SalesAnalytics.ShipperPerformance(Dashboard);
			for (int i = 0; i < gauges.Length && i < performance.Count; i++)
			{
				var shipper = performance[i];
				names[i].Text = shipper.Shipper.Company;

				if (shipper.Shipped == 0)
				{
					// No shipments means no rate. The value stays at the minimum so only the empty track is drawn.
					gauges[i].ValueFormat = NoRateFormat;
					gauges[i].Value = gauges[i].Minimum;
					stats[i].Text = "No shipments" + Environment.NewLine + "in this period" + Environment.NewLine + OpenText(shipper);
					continue;
				}

				// The scale is 0 to 100, so the rate is passed as 95.1, never as the fraction 0.951.
				gauges[i].ValueFormat = RateFormat;
				gauges[i].Value = Math.Round(shipper.OnTimeRate, 1);
				stats[i].Text =
					DashboardStyle.Count(shipper.Shipped) + " shipped · " + shipper.Late + " late" + Environment.NewLine +
					shipper.AverageDays.ToString("0.0", CultureInfo.CurrentCulture) + " days to ship" + Environment.NewLine +
					OpenText(shipper);
			}

			periodLabel.Text = "Orders placed " + Dashboard.Period.RangeText;
		}

		// The shell writes its own line after a navigation or a period change returns; posting ours makes the
		// view's line the one that stays.
		private void PostStatus(string text) => Application.Post(() => SetStatus(text));

		private static string OpenText(ShipperPerformance shipper) => shipper.OpenOrders + " open now";

		#endregion

	}
}
