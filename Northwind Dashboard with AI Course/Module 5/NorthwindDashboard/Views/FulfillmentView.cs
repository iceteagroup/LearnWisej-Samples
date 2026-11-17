using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using NorthwindDashboard.Data;
using NorthwindDashboard.Dialogs;
using Wisej.Web;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Module 5 fulfillment view: the shipper gauges, and the open orders as a board, a calendar or a timeline
	/// beside the session's activity.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The three modes show the same orders. Board is a <see cref="KanbanBoard"/> bound to one card per order, Calendar
	/// is a <see cref="SchedulerView"/> with one resource per shipper, and Timeline is a <see cref="GanttView"/> with one
	/// parent task per shipper. Every order travels in the <c>Tag</c> of its appointment and task, so a click opens
	/// the order without parsing a caption.
	/// </para>
	/// <para>
	/// Northwind records only order, required and shipping dates. The warehouse steps on the board are session
	/// workflow state (see <see cref="DashboardContext.StageOf"/>): <c>CardMoving</c> validates a move,
	/// <c>CardMoved</c> hands the accepted move to <see cref="DashboardContext.SetStage"/>, and moving a card to
	/// Shipped records the as-of date as its shipping date for the rest of the session.
	/// </para>
	/// <para>
	/// The gauge rate is the share of the period's shipped orders that left on or before their required date, on a
	/// 0 to 100 scale. A shipper without shipments in the period shows a dash on an empty track, not 0%.
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

			viewSwitcher.SelectedIndex = 0;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Reloads the shipper gauges, the three views of the open orders and the activity.
		/// </summary>
		protected override void RefreshView()
		{
			UpdateShippers();
			UpdateBoard();
			UpdateSchedule();
			UpdateActivity();
			UpdateSummary();

			if (Visible)
				PostStatus(ModeStatus);
		}

		/// <summary>
		/// Refreshes after another view ships an order; moves made on this board update in place.
		/// </summary>
		protected override void OnOrdersChanged()
		{
			if (!moving)
				base.OnOrdersChanged();
		}

		/// <summary>
		/// Names the current mode's event path in the status strip when the view is shown.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);

			if (Visible && Dashboard != null)
				PostStatus(ModeStatus);
		}

		#endregion

		#region Implementation

		private const int WarningFrom = 90;
		private const int SuccessFrom = 95;
		private const string RateFormat = "{0:N1}%";

		// A format without a placeholder: the gauge shows the dash whatever its value is.
		private const string NoRateFormat = "—";

		// Shipped orders kept on the board and in the activity list, newest first.
		private const int RecentShipments = 6;

		private readonly RadialGauge[] gauges;
		private readonly Label[] names;
		private readonly Label[] stats;
		private bool moving;

		private string Mode => viewSwitcher.SelectedItem?.Value as string ?? "board";

		private string ModeStatus => Mode switch
		{
			"calendar" => "SchedulerAppointment.ResourceId → shipper",
			"timeline" => "GanttTask.ParentId → shipper",
			_ => "KanbanBoard.CardMoving → validate the move"
		};

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

		// The board is a working set, not the order history: every order still open in the snapshot (including the
		// ones shipped in this session) plus the most recent shipments.
		private void UpdateBoard()
		{
			var db = Dashboard.Database;
			var recent = db.Orders.Where(o => o.ShippedDate != null).OrderByDescending(o => o.ShippedDate).ThenByDescending(o => o.Id).Take(RecentShipments);
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
		}

		// Both collections are rebuilt from the context, so each is cleared first: a second refresh or a card move
		// must leave the same number of appointments and tasks.
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

			// One parent task per shipper with open orders; each child runs from the order date to the required date.
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
		}

		// Session activity first (the context keeps it newest first), then the snapshot's latest shipments.
		private void UpdateActivity()
		{
			activityTimeline.Items.Clear();
			foreach (var entry in Dashboard.Activity)
			{
				activityTimeline.Items.Add(new TimelineItem(entry.Time, entry.Title)
				{
					Description = entry.Description,
					Tone = entry.Target is Order ? TimelineTone.Success : TimelineTone.Primary,
					Tag = entry.Target
				});
			}

			var recent = Dashboard.Database.Orders.Where(o => o.ShippedDate != null).OrderByDescending(o => o.ShippedDate).ThenByDescending(o => o.Id).Take(RecentShipments);
			foreach (var order in recent)
			{
				activityTimeline.Items.Add(new TimelineItem(order.ShippedDate.Value, "Order " + order.Number + " shipped")
				{
					Description = order.Customer.Company + " · via " + order.Shipper.Company,
					Tone = TimelineTone.Neutral,
					Tag = order
				});
			}
		}

		private void UpdateSummary()
		{
			var open = Dashboard.OpenOrders().ToArray();
			int dueSoon = open.Count(o => o.RequiredDate <= Dashboard.AsOf.AddDays(7));
			workCard.Text = "Open orders · " + open.Length + (dueSoon > 0 ? " · " + dueSoon + " due within 7 days" : "");

			int shippers = gantt.Tasks.Count(t => string.IsNullOrEmpty(t.ParentId));
			workSummary.Text = Mode switch
			{
				"calendar" => scheduler.Appointments.Count + " appointments for " + scheduler.Resources.Count + " shippers · due dates and the shipments of the last five weeks",
				"timeline" => (gantt.Tasks.Count - shippers) + " open orders under " + shippers + " shippers · order date to required date",
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

		private static string OpenText(ShipperPerformance shipper) => shipper.OpenOrders + " open now";

		// The shell writes its own line after a navigation or a period change returns; posting ours makes the
		// view's line the one that stays.
		private void PostStatus(string text) => Application.Post(() => SetStatus(text));

		private void viewSwitcher_SelectionChanged(object sender, EventArgs e)
		{
			string mode = Mode;
			kanbanBoard.Visible = mode == "board";
			scheduler.Visible = mode == "calendar";
			gantt.Visible = mode == "timeline";

			if (Dashboard == null)
				return;

			UpdateSummary();
			SetStatus(ModeStatus);
		}

		// The validation boundary: nothing has changed yet, and cancelling leaves the card and the context as they were.
		private void kanbanBoard_CardMoving(object sender, KanbanCardMoveEventArgs e)
		{
			if (e.OldColumn != e.Column && e.OldColumn.Value == nameof(FulfillmentStage.Shipped))
			{
				e.Cancel = true;
				SetStatus("CardMoving → e.Cancel = true · context unchanged");
				AlertBox.Show("Order " + ((FulfillmentCard)e.Item).Order.Number + " has already shipped and cannot move back.", MessageBoxIcon.Warning, allowHtml: false);
			}
		}

		// The accepted move: the column's Value is the stage name, and the context owns the change.
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
			UpdateActivity();
			UpdateSummary();
			SetStatus("CardMoved → e.Column.Value → context.SetStage");

			if (stage == FulfillmentStage.Shipped)
				AlertBox.Show("Order " + card.Order.Number + " shipped via " + card.Order.Shipper.Company + ".", MessageBoxIcon.Information, allowHtml: false);
		}

		private void kanbanBoard_WipLimitExceeded(object sender, KanbanCardMoveEventArgs e)
		{
			SetStatus("KanbanBoard.WipLimitExceeded → e.Column.WipLimit · context unchanged");
			AlertBox.Show(e.Column.Text + " already holds " + e.Column.WipLimit + " orders. Pack or ship one first.", MessageBoxIcon.Warning, allowHtml: false);
		}

		private void kanbanBoard_CardDoubleClick(object sender, KanbanCardEventArgs e)
		{
			SetStatus("KanbanBoard.CardDoubleClick → e.Item → OrderDialog");
			OrderDialog.ShowOrder(Dashboard, ((FulfillmentCard)e.Item).Order);
		}

		private void scheduler_AppointmentClick(object sender, SchedulerAppointmentEventArgs e)
		{
			if (e.Appointment.Tag is Order order)
			{
				SetStatus("SchedulerView.AppointmentClick → e.Appointment.Tag → OrderDialog");
				OrderDialog.ShowOrder(Dashboard, order);
			}
		}

		private void gantt_TaskDoubleClick(object sender, GanttTaskEventArgs e)
		{
			if (e.Task.Tag is Order order)
			{
				SetStatus("GanttView.TaskDoubleClick → e.Task.Tag → OrderDialog");
				OrderDialog.ShowOrder(Dashboard, order);
			}
		}

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
