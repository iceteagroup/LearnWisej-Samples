namespace NorthwindDashboard.Views
{
	partial class FulfillmentView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Wisej Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			Wisej.Web.KanbanColumn kanbanColumn1 = new Wisej.Web.KanbanColumn();
			Wisej.Web.KanbanColumn kanbanColumn2 = new Wisej.Web.KanbanColumn();
			Wisej.Web.KanbanColumn kanbanColumn3 = new Wisej.Web.KanbanColumn();
			Wisej.Web.KanbanColumn kanbanColumn4 = new Wisej.Web.KanbanColumn();
			Wisej.Web.Segment segment1 = new Wisej.Web.Segment();
			Wisej.Web.Segment segment2 = new Wisej.Web.Segment();
			Wisej.Web.Segment segment3 = new Wisej.Web.Segment();
			this.layout = new Wisej.Web.TableLayoutPanel();
			this.shipper1Card = new Wisej.Web.Panel();
			this.shipper1Stats = new Wisej.Web.Label();
			this.shipper1Name = new Wisej.Web.Label();
			this.shipper1Gauge = new Wisej.Web.RadialGauge();
			this.shipper2Card = new Wisej.Web.Panel();
			this.shipper2Stats = new Wisej.Web.Label();
			this.shipper2Name = new Wisej.Web.Label();
			this.shipper2Gauge = new Wisej.Web.RadialGauge();
			this.shipper3Card = new Wisej.Web.Panel();
			this.shipper3Stats = new Wisej.Web.Label();
			this.shipper3Name = new Wisej.Web.Label();
			this.shipper3Gauge = new Wisej.Web.RadialGauge();
			this.notePanel = new Wisej.Web.Panel();
			this.periodLabel = new Wisej.Web.Label();
			this.thresholdsLabel = new Wisej.Web.Label();
			this.workCard = new Wisej.Web.Panel();
			this.kanbanBoard = new Wisej.Web.KanbanBoard();
			this.scheduler = new Wisej.Web.SchedulerView();
			this.gantt = new Wisej.Web.GanttView();
			this.workToolbar = new Wisej.Web.Panel();
			this.workSummary = new Wisej.Web.Label();
			this.viewSwitcher = new Wisej.Web.SegmentedButton();
			this.activityCard = new Wisej.Web.Panel();
			this.activityTimeline = new Wisej.Web.Timeline();
			this.layout.SuspendLayout();
			this.shipper1Card.SuspendLayout();
			this.shipper2Card.SuspendLayout();
			this.shipper3Card.SuspendLayout();
			this.notePanel.SuspendLayout();
			this.workCard.SuspendLayout();
			this.workToolbar.SuspendLayout();
			this.activityCard.SuspendLayout();
			this.SuspendLayout();
			//
			// layout
			//
			this.layout.ColumnCount = 12;
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 8.333333F));
			this.layout.Controls.Add(this.shipper1Card, 0, 0);
			this.layout.Controls.Add(this.shipper2Card, 3, 0);
			this.layout.Controls.Add(this.shipper3Card, 6, 0);
			this.layout.Controls.Add(this.notePanel, 9, 0);
			this.layout.Controls.Add(this.workCard, 0, 1);
			this.layout.Controls.Add(this.activityCard, 9, 1);
			this.layout.Dock = Wisej.Web.DockStyle.Fill;
			this.layout.Location = new System.Drawing.Point(0, 0);
			this.layout.Name = "layout";
			this.layout.Padding = new Wisej.Web.Padding(0, 0, 0, 8);
			this.layout.RowCount = 2;
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 150F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
			this.layout.SetColumnSpan(this.shipper1Card, 3);
			this.layout.SetColumnSpan(this.shipper2Card, 3);
			this.layout.SetColumnSpan(this.shipper3Card, 3);
			this.layout.SetColumnSpan(this.notePanel, 3);
			this.layout.SetColumnSpan(this.workCard, 9);
			this.layout.SetColumnSpan(this.activityCard, 3);
			this.layout.Size = new System.Drawing.Size(1200, 758);
			this.layout.TabIndex = 0;
			//
			// shipper1Card
			//
			this.shipper1Card.AppearanceKey = "nw-card";
			this.shipper1Card.Controls.Add(this.shipper1Stats);
			this.shipper1Card.Controls.Add(this.shipper1Name);
			this.shipper1Card.Controls.Add(this.shipper1Gauge);
			this.shipper1Card.Dock = Wisej.Web.DockStyle.Fill;
			this.shipper1Card.Location = new System.Drawing.Point(0, 8);
			this.shipper1Card.Margin = new Wisej.Web.Padding(0, 8, 16, 8);
			this.shipper1Card.Name = "shipper1Card";
			this.shipper1Card.Size = new System.Drawing.Size(284, 134);
			this.shipper1Card.TabIndex = 0;
			//
			// shipper1Stats
			//
			this.shipper1Stats.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper1Stats.AppearanceKey = "nw-subtitle";
			this.shipper1Stats.Location = new System.Drawing.Point(132, 44);
			this.shipper1Stats.Name = "shipper1Stats";
			this.shipper1Stats.Size = new System.Drawing.Size(142, 72);
			this.shipper1Stats.TabIndex = 2;
			this.shipper1Stats.Text = "Shipped";
			//
			// shipper1Name
			//
			this.shipper1Name.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper1Name.AppearanceKey = "nw-strong";
			this.shipper1Name.AutoEllipsis = true;
			this.shipper1Name.Location = new System.Drawing.Point(132, 18);
			this.shipper1Name.Name = "shipper1Name";
			this.shipper1Name.Size = new System.Drawing.Size(142, 22);
			this.shipper1Name.TabIndex = 1;
			this.shipper1Name.Text = "Shipper";
			//
			// shipper1Gauge
			//
			this.shipper1Gauge.Label = "ON TIME";
			this.shipper1Gauge.Location = new System.Drawing.Point(16, 16);
			this.shipper1Gauge.Name = "shipper1Gauge";
			this.shipper1Gauge.Size = new System.Drawing.Size(102, 102);
			this.shipper1Gauge.TabIndex = 0;
			this.shipper1Gauge.Thickness = 10;
			this.shipper1Gauge.ValueFormat = "{0:N1}%";
			//
			// shipper2Card
			//
			this.shipper2Card.AppearanceKey = "nw-card";
			this.shipper2Card.Controls.Add(this.shipper2Stats);
			this.shipper2Card.Controls.Add(this.shipper2Name);
			this.shipper2Card.Controls.Add(this.shipper2Gauge);
			this.shipper2Card.Dock = Wisej.Web.DockStyle.Fill;
			this.shipper2Card.Location = new System.Drawing.Point(300, 8);
			this.shipper2Card.Margin = new Wisej.Web.Padding(0, 8, 16, 8);
			this.shipper2Card.Name = "shipper2Card";
			this.shipper2Card.Size = new System.Drawing.Size(284, 134);
			this.shipper2Card.TabIndex = 1;
			//
			// shipper2Stats
			//
			this.shipper2Stats.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper2Stats.AppearanceKey = "nw-subtitle";
			this.shipper2Stats.Location = new System.Drawing.Point(132, 44);
			this.shipper2Stats.Name = "shipper2Stats";
			this.shipper2Stats.Size = new System.Drawing.Size(142, 72);
			this.shipper2Stats.TabIndex = 2;
			this.shipper2Stats.Text = "Shipped";
			//
			// shipper2Name
			//
			this.shipper2Name.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper2Name.AppearanceKey = "nw-strong";
			this.shipper2Name.AutoEllipsis = true;
			this.shipper2Name.Location = new System.Drawing.Point(132, 18);
			this.shipper2Name.Name = "shipper2Name";
			this.shipper2Name.Size = new System.Drawing.Size(142, 22);
			this.shipper2Name.TabIndex = 1;
			this.shipper2Name.Text = "Shipper";
			//
			// shipper2Gauge
			//
			this.shipper2Gauge.Label = "ON TIME";
			this.shipper2Gauge.Location = new System.Drawing.Point(16, 16);
			this.shipper2Gauge.Name = "shipper2Gauge";
			this.shipper2Gauge.Size = new System.Drawing.Size(102, 102);
			this.shipper2Gauge.TabIndex = 0;
			this.shipper2Gauge.Thickness = 10;
			this.shipper2Gauge.ValueFormat = "{0:N1}%";
			//
			// shipper3Card
			//
			this.shipper3Card.AppearanceKey = "nw-card";
			this.shipper3Card.Controls.Add(this.shipper3Stats);
			this.shipper3Card.Controls.Add(this.shipper3Name);
			this.shipper3Card.Controls.Add(this.shipper3Gauge);
			this.shipper3Card.Dock = Wisej.Web.DockStyle.Fill;
			this.shipper3Card.Location = new System.Drawing.Point(600, 8);
			this.shipper3Card.Margin = new Wisej.Web.Padding(0, 8, 16, 8);
			this.shipper3Card.Name = "shipper3Card";
			this.shipper3Card.Size = new System.Drawing.Size(284, 134);
			this.shipper3Card.TabIndex = 2;
			//
			// shipper3Stats
			//
			this.shipper3Stats.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper3Stats.AppearanceKey = "nw-subtitle";
			this.shipper3Stats.Location = new System.Drawing.Point(132, 44);
			this.shipper3Stats.Name = "shipper3Stats";
			this.shipper3Stats.Size = new System.Drawing.Size(142, 72);
			this.shipper3Stats.TabIndex = 2;
			this.shipper3Stats.Text = "Shipped";
			//
			// shipper3Name
			//
			this.shipper3Name.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper3Name.AppearanceKey = "nw-strong";
			this.shipper3Name.AutoEllipsis = true;
			this.shipper3Name.Location = new System.Drawing.Point(132, 18);
			this.shipper3Name.Name = "shipper3Name";
			this.shipper3Name.Size = new System.Drawing.Size(142, 22);
			this.shipper3Name.TabIndex = 1;
			this.shipper3Name.Text = "Shipper";
			//
			// shipper3Gauge
			//
			this.shipper3Gauge.Label = "ON TIME";
			this.shipper3Gauge.Location = new System.Drawing.Point(16, 16);
			this.shipper3Gauge.Name = "shipper3Gauge";
			this.shipper3Gauge.Size = new System.Drawing.Size(102, 102);
			this.shipper3Gauge.TabIndex = 0;
			this.shipper3Gauge.Thickness = 10;
			this.shipper3Gauge.ValueFormat = "{0:N1}%";
			//
			// notePanel
			//
			this.notePanel.AppearanceKey = "nw-view";
			this.notePanel.Controls.Add(this.periodLabel);
			this.notePanel.Controls.Add(this.thresholdsLabel);
			this.notePanel.Dock = Wisej.Web.DockStyle.Fill;
			this.notePanel.Location = new System.Drawing.Point(900, 8);
			this.notePanel.Margin = new Wisej.Web.Padding(0, 8, 0, 8);
			this.notePanel.Name = "notePanel";
			this.notePanel.Size = new System.Drawing.Size(284, 134);
			this.notePanel.TabIndex = 3;
			//
			// periodLabel
			//
			this.periodLabel.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.periodLabel.AppearanceKey = "nw-caption";
			this.periodLabel.AutoEllipsis = true;
			this.periodLabel.Location = new System.Drawing.Point(8, 86);
			this.periodLabel.Name = "periodLabel";
			this.periodLabel.Size = new System.Drawing.Size(268, 18);
			this.periodLabel.TabIndex = 1;
			this.periodLabel.Text = "Orders placed";
			//
			// thresholdsLabel
			//
			this.thresholdsLabel.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.thresholdsLabel.AppearanceKey = "nw-subtitle";
			this.thresholdsLabel.Location = new System.Drawing.Point(8, 44);
			this.thresholdsLabel.Name = "thresholdsLabel";
			this.thresholdsLabel.Size = new System.Drawing.Size(268, 40);
			this.thresholdsLabel.TabIndex = 0;
			this.thresholdsLabel.Text = "On-time thresholds";
			//
			// workCard
			//
			this.workCard.AppearanceKey = "nw-card";
			this.workCard.Controls.Add(this.kanbanBoard);
			this.workCard.Controls.Add(this.scheduler);
			this.workCard.Controls.Add(this.gantt);
			this.workCard.Controls.Add(this.workToolbar);
			this.workCard.Dock = Wisej.Web.DockStyle.Fill;
			this.workCard.HeaderSize = 46;
			this.workCard.Location = new System.Drawing.Point(0, 158);
			this.workCard.Margin = new Wisej.Web.Padding(0, 8, 16, 8);
			this.workCard.Name = "workCard";
			this.workCard.Padding = new Wisej.Web.Padding(16, 0, 16, 16);
			this.workCard.ShowHeader = true;
			this.workCard.Size = new System.Drawing.Size(884, 584);
			this.workCard.TabIndex = 4;
			this.workCard.Text = "Open orders";
			//
			// kanbanBoard
			//
			this.kanbanBoard.AccessibleName = "Fulfillment board";
			this.kanbanBoard.AvatarMember = "SalesRep";
			this.kanbanBoard.ColumnWidth = 190;
			kanbanColumn1.Text = "New";
			kanbanColumn1.Tone = Wisej.Web.MeterTone.Neutral;
			kanbanColumn1.Value = "New";
			kanbanColumn2.Text = "Picking";
			kanbanColumn2.Value = "Picking";
			kanbanColumn2.WipLimit = 8;
			kanbanColumn3.Text = "Packed";
			kanbanColumn3.Tone = Wisej.Web.MeterTone.Warning;
			kanbanColumn3.Value = "Packed";
			kanbanColumn4.Text = "Shipped";
			kanbanColumn4.Tone = Wisej.Web.MeterTone.Success;
			kanbanColumn4.Value = "Shipped";
			this.kanbanBoard.Columns.Add(kanbanColumn1);
			this.kanbanBoard.Columns.Add(kanbanColumn2);
			this.kanbanBoard.Columns.Add(kanbanColumn3);
			this.kanbanBoard.Columns.Add(kanbanColumn4);
			this.kanbanBoard.Dock = Wisej.Web.DockStyle.Fill;
			this.kanbanBoard.Location = new System.Drawing.Point(16, 52);
			this.kanbanBoard.MaxVisibleCards = 8;
			this.kanbanBoard.MetaMember = "Meta";
			this.kanbanBoard.Name = "kanbanBoard";
			this.kanbanBoard.OrderMember = "Position";
			this.kanbanBoard.ProgressMember = "Progress";
			this.kanbanBoard.Size = new System.Drawing.Size(852, 470);
			this.kanbanBoard.StatusMember = "Stage";
			this.kanbanBoard.TabIndex = 1;
			this.kanbanBoard.TagMember = "TagText";
			this.kanbanBoard.TagToneMember = "TagTone";
			this.kanbanBoard.CardMoving += new System.EventHandler<Wisej.Web.KanbanCardMoveEventArgs>(this.kanbanBoard_CardMoving);
			this.kanbanBoard.CardMoved += new System.EventHandler<Wisej.Web.KanbanCardMoveEventArgs>(this.kanbanBoard_CardMoved);
			this.kanbanBoard.CardDoubleClick += new System.EventHandler<Wisej.Web.KanbanCardEventArgs>(this.kanbanBoard_CardDoubleClick);
			this.kanbanBoard.WipLimitExceeded += new System.EventHandler<Wisej.Web.KanbanCardMoveEventArgs>(this.kanbanBoard_WipLimitExceeded);
			//
			// scheduler
			//
			this.scheduler.AccessibleName = "Shipper schedule";
			this.scheduler.AllowCreate = false;
			this.scheduler.AllowMove = false;
			this.scheduler.AllowResize = false;
			this.scheduler.Dock = Wisej.Web.DockStyle.Fill;
			this.scheduler.Location = new System.Drawing.Point(16, 52);
			this.scheduler.Name = "scheduler";
			this.scheduler.ShowCurrentTime = false;
			this.scheduler.Size = new System.Drawing.Size(852, 470);
			this.scheduler.TabIndex = 2;
			this.scheduler.View = Wisej.Web.SchedulerViewMode.Timeline;
			this.scheduler.Visible = false;
			this.scheduler.AppointmentClick += new System.EventHandler<Wisej.Web.SchedulerAppointmentEventArgs>(this.scheduler_AppointmentClick);
			//
			// gantt
			//
			this.gantt.AccessibleName = "Open order lead times";
			this.gantt.AllowDrag = false;
			this.gantt.AllowLink = false;
			this.gantt.AllowResize = false;
			this.gantt.Dock = Wisej.Web.DockStyle.Fill;
			this.gantt.Location = new System.Drawing.Point(16, 52);
			this.gantt.Name = "gantt";
			this.gantt.ShowToday = false;
			this.gantt.Size = new System.Drawing.Size(852, 470);
			this.gantt.TabIndex = 3;
			this.gantt.TaskListWidth = 280;
			this.gantt.Visible = false;
			this.gantt.TaskDoubleClick += new System.EventHandler<Wisej.Web.GanttTaskEventArgs>(this.gantt_TaskDoubleClick);
			//
			// workToolbar
			//
			this.workToolbar.AppearanceKey = "nw-view";
			this.workToolbar.Controls.Add(this.workSummary);
			this.workToolbar.Controls.Add(this.viewSwitcher);
			this.workToolbar.Dock = Wisej.Web.DockStyle.Top;
			this.workToolbar.Location = new System.Drawing.Point(16, 0);
			this.workToolbar.Name = "workToolbar";
			this.workToolbar.Size = new System.Drawing.Size(852, 52);
			this.workToolbar.TabIndex = 0;
			//
			// workSummary
			//
			this.workSummary.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.workSummary.AppearanceKey = "nw-subtitle";
			this.workSummary.AutoEllipsis = true;
			this.workSummary.Location = new System.Drawing.Point(356, 12);
			this.workSummary.Name = "workSummary";
			this.workSummary.Size = new System.Drawing.Size(496, 24);
			this.workSummary.TabIndex = 1;
			this.workSummary.Text = "Drag a card to change its warehouse step";
			this.workSummary.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			//
			// viewSwitcher
			//
			this.viewSwitcher.AccessibleName = "Open order view";
			segment1.Icon = "Assets/Icons/board.svg";
			segment1.Text = "Board";
			segment1.Value = "board";
			segment2.Icon = "Assets/Icons/calendar.svg";
			segment2.Text = "Calendar";
			segment2.Value = "calendar";
			segment3.Icon = "Assets/Icons/timeline.svg";
			segment3.Text = "Timeline";
			segment3.Value = "timeline";
			this.viewSwitcher.Items.Add(segment1);
			this.viewSwitcher.Items.Add(segment2);
			this.viewSwitcher.Items.Add(segment3);
			this.viewSwitcher.Location = new System.Drawing.Point(0, 2);
			this.viewSwitcher.Name = "viewSwitcher";
			this.viewSwitcher.SegmentSize = Wisej.Web.SegmentSize.Small;
			this.viewSwitcher.Size = new System.Drawing.Size(340, 40);
			this.viewSwitcher.TabIndex = 0;
			this.viewSwitcher.SelectionChanged += new System.EventHandler(this.viewSwitcher_SelectionChanged);
			//
			// activityCard
			//
			this.activityCard.AppearanceKey = "nw-card";
			this.activityCard.Controls.Add(this.activityTimeline);
			this.activityCard.Dock = Wisej.Web.DockStyle.Fill;
			this.activityCard.HeaderSize = 46;
			this.activityCard.Location = new System.Drawing.Point(900, 158);
			this.activityCard.Margin = new Wisej.Web.Padding(0, 8, 0, 8);
			this.activityCard.Name = "activityCard";
			this.activityCard.Padding = new Wisej.Web.Padding(8, 0, 8, 8);
			this.activityCard.ShowHeader = true;
			this.activityCard.Size = new System.Drawing.Size(284, 584);
			this.activityCard.TabIndex = 5;
			this.activityCard.Text = "Activity";
			//
			// activityTimeline
			//
			this.activityTimeline.AccessibleName = "Activity";
			this.activityTimeline.Dock = Wisej.Web.DockStyle.Fill;
			this.activityTimeline.Location = new System.Drawing.Point(8, 0);
			this.activityTimeline.Name = "activityTimeline";
			this.activityTimeline.Size = new System.Drawing.Size(268, 530);
			this.activityTimeline.TabIndex = 0;
			this.activityTimeline.TimeFormat = "yyyy-MM-dd";
			this.activityTimeline.TimePosition = Wisej.Web.TimelineTimePosition.Inline;
			this.activityTimeline.ItemClick += new System.EventHandler<Wisej.Web.TimelineItemEventArgs>(this.activityTimeline_ItemClick);
			//
			// FulfillmentView
			//
			this.AppearanceKey = "nw-view";
			this.Controls.Add(this.layout);
			this.Name = "FulfillmentView";
			this.Size = new System.Drawing.Size(1200, 758);
			this.Text = "Fulfillment";
			this.layout.ResumeLayout(false);
			this.shipper1Card.ResumeLayout(false);
			this.shipper2Card.ResumeLayout(false);
			this.shipper3Card.ResumeLayout(false);
			this.notePanel.ResumeLayout(false);
			this.workCard.ResumeLayout(false);
			this.workToolbar.ResumeLayout(false);
			this.activityCard.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.TableLayoutPanel layout;
		private Wisej.Web.Panel shipper1Card;
		private Wisej.Web.RadialGauge shipper1Gauge;
		private Wisej.Web.Label shipper1Name;
		private Wisej.Web.Label shipper1Stats;
		private Wisej.Web.Panel shipper2Card;
		private Wisej.Web.RadialGauge shipper2Gauge;
		private Wisej.Web.Label shipper2Name;
		private Wisej.Web.Label shipper2Stats;
		private Wisej.Web.Panel shipper3Card;
		private Wisej.Web.RadialGauge shipper3Gauge;
		private Wisej.Web.Label shipper3Name;
		private Wisej.Web.Label shipper3Stats;
		private Wisej.Web.Panel notePanel;
		private Wisej.Web.Label thresholdsLabel;
		private Wisej.Web.Label periodLabel;
		private Wisej.Web.Panel workCard;
		private Wisej.Web.Panel workToolbar;
		private Wisej.Web.SegmentedButton viewSwitcher;
		private Wisej.Web.Label workSummary;
		private Wisej.Web.KanbanBoard kanbanBoard;
		private Wisej.Web.SchedulerView scheduler;
		private Wisej.Web.GanttView gantt;
		private Wisej.Web.Panel activityCard;
		private Wisej.Web.Timeline activityTimeline;
	}
}
