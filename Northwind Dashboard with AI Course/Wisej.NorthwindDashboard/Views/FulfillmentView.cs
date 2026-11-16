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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Wisej.NorthwindDashboard.Data;
using Wisej.NorthwindDashboard.Dialogs;
using Wisej.Web;

namespace Wisej.NorthwindDashboard.Views
{

	/// <summary>
	/// Operations snapshot: shipper performance, lead time, and the open orders as a board, calendar or timeline.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Northwind records only order, required and shipping dates. The warehouse steps shown on the board are session
	/// workflow state (see <see cref="DashboardContext.StageOf"/>); moving a card to Shipped records the as-of date as
	/// its shipping date for the rest of the session.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// var view = new FulfillmentView { Dock = DockStyle.Fill, Dashboard = context };
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
				gauge.Thresholds.Add(new MeterThreshold(90, MeterTone.Warning));
				gauge.Thresholds.Add(new MeterThreshold(95, MeterTone.Success));
			}

			viewSwitcher.SelectedIndex = 0;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets false: the view shows the state on the as-of date, not a reporting period.
		/// </summary>
		public override bool UsesPeriod => false;

		#endregion

		#region Methods

		/// <summary>
		/// Reloads the shipper cards and the open-order views.
		/// </summary>
		protected override void RefreshView()
		{
			UpdateShippers();
			UpdateBoard();
			UpdateSchedule();
		}

		/// <summary>
		/// Refreshes after another view ships an order; moves made on this board update in place.
		/// </summary>
		protected override void OnOrdersChanged()
		{
			if (!moving)
				base.OnOrdersChanged();
		}

		#endregion

		#region Implementation

		private readonly RadialGauge[] gauges;
		private readonly Label[] names;
		private readonly Label[] stats;
		private bool moving;

		private DashboardPeriod LastYear => DashboardPeriod.Create(PeriodKind.Last12Months, Dashboard.AsOf, Dashboard.Database.FirstOrderDate);

		private void UpdateShippers()
		{
			var year = LastYear;
			var performance = SalesAnalytics.ShipperPerformance(Dashboard, year);
			for (int i = 0; i < gauges.Length && i < performance.Count; i++)
			{
				var shipper = performance[i];
				gauges[i].Value = Math.Round(shipper.OnTimeRate, 1);
				names[i].Text = shipper.Shipper.Company;
				stats[i].Text =
					DashboardStyle.Count(shipper.Orders) + " orders · " + shipper.Late + " late" + Environment.NewLine +
					shipper.AverageDays.ToString("0.0", CultureInfo.CurrentCulture) + " days to ship" + Environment.NewLine +
					DashboardStyle.Money(shipper.Freight) + " freight" + Environment.NewLine +
					shipper.OpenOrders + " open now";
			}

			// Average days from order to shipment per month, skipping months without shipments.
			var shipped = Dashboard.Database.Orders
				.Where(o => year.Contains(o.OrderDate))
				.Select(o => (Order: o, Shipped: Dashboard.ShippedDate(o)))
				.Where(x => x.Shipped != null)
				.ToArray();
			leadTimeKpi.SparklineValues = year.Buckets
				.Select(b => shipped.Where(x => year.BucketOf(x.Order.OrderDate) == b.Index).Select(x => (x.Shipped.Value - x.Order.OrderDate).TotalDays).ToArray())
				.Where(days => days.Length > 0)
				.Select(days => Math.Round(days.Average(), 1))
				.ToArray();
			double average = shipped.Length == 0 ? 0 : shipped.Average(x => (x.Shipped.Value - x.Order.OrderDate).TotalDays);
			leadTimeKpi.Value = average.ToString("0.0", CultureInfo.CurrentCulture) + " days";
			leadTimeKpi.Caption = "Order to shipment, monthly · last 12 months";
		}

		private void UpdateBoard()
		{
			var db = Dashboard.Database;
			var recent = db.Orders.Where(o => o.ShippedDate != null).OrderByDescending(o => o.ShippedDate).ThenByDescending(o => o.Id).Take(6);
			var cards = db.Orders.Where(o => o.ShippedDate == null).Concat(recent).Select(o => new FulfillmentCard(o, Dashboard)).ToList();

			foreach (var lane in cards.GroupBy(c => c.Stage))
			{
				var ordered = lane.Key == nameof(FulfillmentStage.Shipped)
					? lane.OrderByDescending(c => Dashboard.ShippedDate(c.Order)).ThenByDescending(c => c.Order.Id)
					: lane.OrderBy(c => c.Order.RequiredDate).ThenBy(c => c.Order.Id);
				int position = 0;
				foreach (var card in ordered)
					card.Position = position++;
			}

			kanbanBoard.DataSource = new BindingList<FulfillmentCard>(cards);
			UpdateSummary();
		}

		private void UpdateSchedule()
		{
			var db = Dashboard.Database;
			var open = Dashboard.OpenOrders().ToArray();

			scheduler.Resources.Clear();
			foreach (var shipper in db.Shippers)
				scheduler.Resources.Add(new SchedulerResource { Id = shipper.Id.ToString(CultureInfo.InvariantCulture), Text = shipper.Company });

			scheduler.Appointments.Clear();
			foreach (var order in open)
			{
				scheduler.Appointments.Add(new SchedulerAppointment
				{
					Id = "due-" + order.Id,
					Text = "Due · " + order.Number + " " + order.Customer.Company,
					Start = order.RequiredDate,
					End = order.RequiredDate.AddDays(1),
					AllDay = true,
					Tone = DueTone(order),
					ResourceId = order.ShipVia.ToString(CultureInfo.InvariantCulture),
					Tag = order
				});
			}

			var since = Dashboard.AsOf.AddDays(-35);
			foreach (var order in db.Orders)
			{
				if (!(Dashboard.ShippedDate(order) is DateTime shipped) || shipped < since)
					continue;

				scheduler.Appointments.Add(new SchedulerAppointment
				{
					Id = "shipped-" + order.Id,
					Text = "Shipped · " + order.Number + " " + order.Customer.Company,
					Start = shipped,
					End = shipped.AddDays(1),
					AllDay = true,
					Tone = shipped > order.RequiredDate ? MeterTone.Warning : MeterTone.Success,
					ResourceId = order.ShipVia.ToString(CultureInfo.InvariantCulture),
					Tag = order
				});
			}

			scheduler.Date = Dashboard.AsOf;

			// Lead-time timeline: one bar per open order from order date to required date, grouped by shipper.
			gantt.Tasks.Clear();
			foreach (var group in open.GroupBy(o => o.Shipper).OrderBy(g => g.Key.Id))
			{
				string parent = "shipper-" + group.Key.Id;
				gantt.Tasks.Add(new GanttTask
				{
					Id = parent,
					Text = group.Key.Company + " (" + group.Count() + ")",
					Start = group.Min(o => o.OrderDate),
					End = group.Max(o => o.RequiredDate),
					Progress = Math.Round(group.Average(o => StageProgress(Dashboard.StageOf(o)))),
					Tone = MeterTone.Neutral
				});

				foreach (var order in group.OrderBy(o => o.RequiredDate))
				{
					gantt.Tasks.Add(new GanttTask
					{
						Id = "order-" + order.Id,
						ParentId = parent,
						Text = order.Number + " " + order.Customer.Company,
						Start = order.OrderDate,
						End = order.RequiredDate,
						Progress = StageProgress(Dashboard.StageOf(order)),
						Tone = DueTone(order),
						Tag = order
					});
				}
			}

			if (open.Length > 0)
				gantt.ScrollTo(open.Min(o => o.OrderDate));
		}

		private void UpdateSummary()
		{
			var open = Dashboard.OpenOrders().ToArray();
			int dueSoon = open.Count(o => o.RequiredDate <= Dashboard.AsOf.AddDays(7));
			workCard.Text = "Open orders · " + open.Length + (dueSoon > 0 ? " · " + dueSoon + " due within 7 days" : "");
			workSummary.Text = (viewSwitcher.SelectedItem?.Value as string) switch
			{
				"calendar" => "Due dates of open orders and shipments of the last five weeks",
				"timeline" => "Each open order from its order date to its required date, grouped by shipper",
				_ => "Drag a card to another step · moving it to Shipped records " + DashboardStyle.Date(Dashboard.AsOf) + " as the ship date"
			};
		}

		private MeterTone DueTone(Order order)
		{
			int days = (order.RequiredDate - Dashboard.AsOf).Days;
			return days < 0 ? MeterTone.Danger : days <= 7 ? MeterTone.Warning : MeterTone.Primary;
		}

		private static double StageProgress(FulfillmentStage stage) => stage switch
		{
			FulfillmentStage.New => 10,
			FulfillmentStage.Picking => 45,
			FulfillmentStage.Packed => 80,
			_ => 100
		};

		private void viewSwitcher_SelectionChanged(object sender, EventArgs e)
		{
			string selected = viewSwitcher.SelectedItem?.Value as string ?? "board";
			kanbanBoard.Visible = selected == "board";
			scheduler.Visible = selected == "calendar";
			gantt.Visible = selected == "timeline";

			if (Dashboard != null)
				UpdateSummary();
		}

		private void kanbanBoard_CardMoving(object sender, KanbanCardMoveEventArgs e)
		{
			if (e.OldColumn != e.Column && e.OldColumn.Value == nameof(FulfillmentStage.Shipped))
			{
				e.Cancel = true;
				AlertBox.Show("Order " + ((FulfillmentCard)e.Item).Order.Number + " has already shipped and cannot move back.", MessageBoxIcon.Warning, allowHtml: false);
			}
		}

		private void kanbanBoard_CardMoved(object sender, KanbanCardMoveEventArgs e)
		{
			if (e.OldColumn == e.Column)
				return;

			var card = (FulfillmentCard)e.Item;
			var stage = Enum.Parse<FulfillmentStage>(e.Column.Value);
			moving = true;
			try
			{
				Dashboard.SetStage(card.Order, stage);
			}
			finally
			{
				moving = false;
			}

			kanbanBoard.RefreshCard(card);
			UpdateShippers();
			UpdateSchedule();
			UpdateSummary();

			if (stage == FulfillmentStage.Shipped)
				AlertBox.Show("Order " + card.Order.Number + " shipped via " + card.Order.Shipper.Company + ".", MessageBoxIcon.Information, allowHtml: false);
		}

		private void kanbanBoard_WipLimitExceeded(object sender, KanbanCardMoveEventArgs e) =>
			AlertBox.Show(e.Column.Text + " already holds " + e.Column.WipLimit + " orders. Pack or ship one first.", MessageBoxIcon.Warning, allowHtml: false);

		private void kanbanBoard_CardDoubleClick(object sender, KanbanCardEventArgs e) =>
			OrderDialog.ShowOrder(Dashboard, ((FulfillmentCard)e.Item).Order);

		private void scheduler_AppointmentClick(object sender, SchedulerAppointmentEventArgs e)
		{
			if (e.Appointment.Tag is Order order)
				OrderDialog.ShowOrder(Dashboard, order);
		}

		private void gantt_TaskDoubleClick(object sender, GanttTaskEventArgs e)
		{
			if (e.Task.Tag is Order order)
				OrderDialog.ShowOrder(Dashboard, order);
		}

		// Board record; the board writes Stage and Position when a card moves.
		private sealed class FulfillmentCard
		{
			public FulfillmentCard(Order order, DashboardContext context)
			{
				Order = order;
				this.context = context;
				Stage = context.StageOf(order).ToString();
			}

			public Order Order { get; }

			public string Stage { get; set; }

			public int Position { get; set; }

			public string Title => Order.Number + " · " + Order.Customer.Company;

			public string SalesRep => Order.Employee.FullName;

			public string Meta => DashboardStyle.MoneyCents(Order.Subtotal) + " · " + Order.Lines.Count + " lines · " + Order.Shipper.Company;

			public double Progress => StageProgress(Enum.Parse<FulfillmentStage>(Stage));

			public string TagText
			{
				get
				{
					if (context.ShippedDate(Order) is DateTime shipped)
						return "Shipped " + shipped.ToString("MMM d", CultureInfo.CurrentCulture);

					int days = (Order.RequiredDate - context.AsOf).Days;
					return days < 0 ? "Overdue" : days == 0 ? "Due today" : days <= 7 ? "Due in " + days + (days == 1 ? " day" : " days") : "Due " + Order.RequiredDate.ToString("MMM d", CultureInfo.CurrentCulture);
				}
			}

			public MeterTone TagTone
			{
				get
				{
					if (context.ShippedDate(Order) != null)
						return MeterTone.Success;

					int days = (Order.RequiredDate - context.AsOf).Days;
					return days < 0 ? MeterTone.Danger : days <= 7 ? MeterTone.Warning : MeterTone.Neutral;
				}
			}

			private readonly DashboardContext context;
		}

		#endregion

	}
}
