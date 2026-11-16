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

namespace Wisej.NorthwindDashboard.Views
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
			this.leadTimeKpi = new Wisej.Web.KpiPanel();
			this.workCard = new Wisej.Web.Panel();
			this.kanbanBoard = new Wisej.Web.KanbanBoard();
			this.scheduler = new Wisej.Web.SchedulerView();
			this.gantt = new Wisej.Web.GanttView();
			this.workToolbar = new Wisej.Web.Panel();
			this.workSummary = new Wisej.Web.Label();
			this.viewSwitcher = new Wisej.Web.SegmentedButton();
			this.layout.SuspendLayout();
			this.shipper1Card.SuspendLayout();
			this.shipper2Card.SuspendLayout();
			this.shipper3Card.SuspendLayout();
			this.workCard.SuspendLayout();
			this.workToolbar.SuspendLayout();
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
			this.layout.Controls.Add(this.leadTimeKpi, 9, 0);
			this.layout.Controls.Add(this.workCard, 0, 1);
			this.layout.Dock = Wisej.Web.DockStyle.Fill;
			this.layout.Location = new System.Drawing.Point(0, 0);
			this.layout.Name = "layout";
			this.layout.Padding = new Wisej.Web.Padding(16);
			this.layout.RowCount = 2;
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 172F));
			this.layout.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
			this.layout.SetColumnSpan(this.shipper1Card, 3);
			this.layout.SetColumnSpan(this.shipper2Card, 3);
			this.layout.SetColumnSpan(this.shipper3Card, 3);
			this.layout.SetColumnSpan(this.leadTimeKpi, 3);
			this.layout.SetColumnSpan(this.workCard, 12);
			this.layout.Size = new System.Drawing.Size(1208, 828);
			this.layout.TabIndex = 0;
			//
			// shipper1Card
			//
			this.shipper1Card.AppearanceKey = "nw-card";
			this.shipper1Card.Controls.Add(this.shipper1Stats);
			this.shipper1Card.Controls.Add(this.shipper1Name);
			this.shipper1Card.Controls.Add(this.shipper1Gauge);
			this.shipper1Card.Dock = Wisej.Web.DockStyle.Fill;
			this.shipper1Card.Location = new System.Drawing.Point(24, 24);
			this.shipper1Card.Margin = new Wisej.Web.Padding(8);
			this.shipper1Card.Name = "shipper1Card";
			this.shipper1Card.Size = new System.Drawing.Size(278, 156);
			this.shipper1Card.TabIndex = 0;
			//
			// shipper1Stats
			//
			this.shipper1Stats.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper1Stats.AppearanceKey = "nw-subtitle";
			this.shipper1Stats.Location = new System.Drawing.Point(142, 44);
			this.shipper1Stats.Name = "shipper1Stats";
			this.shipper1Stats.Size = new System.Drawing.Size(126, 96);
			this.shipper1Stats.TabIndex = 2;
			this.shipper1Stats.Text = "Orders";
			//
			// shipper1Name
			//
			this.shipper1Name.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper1Name.AppearanceKey = "nw-strong";
			this.shipper1Name.AutoEllipsis = true;
			this.shipper1Name.Location = new System.Drawing.Point(142, 18);
			this.shipper1Name.Name = "shipper1Name";
			this.shipper1Name.Size = new System.Drawing.Size(126, 22);
			this.shipper1Name.TabIndex = 1;
			this.shipper1Name.Text = "Shipper";
			//
			// shipper1Gauge
			//
			this.shipper1Gauge.Label = "ON TIME";
			this.shipper1Gauge.Location = new System.Drawing.Point(16, 20);
			this.shipper1Gauge.Name = "shipper1Gauge";
			this.shipper1Gauge.Size = new System.Drawing.Size(114, 114);
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
			this.shipper2Card.Location = new System.Drawing.Point(318, 24);
			this.shipper2Card.Margin = new Wisej.Web.Padding(8);
			this.shipper2Card.Name = "shipper2Card";
			this.shipper2Card.Size = new System.Drawing.Size(278, 156);
			this.shipper2Card.TabIndex = 1;
			//
			// shipper2Stats
			//
			this.shipper2Stats.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper2Stats.AppearanceKey = "nw-subtitle";
			this.shipper2Stats.Location = new System.Drawing.Point(142, 44);
			this.shipper2Stats.Name = "shipper2Stats";
			this.shipper2Stats.Size = new System.Drawing.Size(126, 96);
			this.shipper2Stats.TabIndex = 2;
			this.shipper2Stats.Text = "Orders";
			//
			// shipper2Name
			//
			this.shipper2Name.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper2Name.AppearanceKey = "nw-strong";
			this.shipper2Name.AutoEllipsis = true;
			this.shipper2Name.Location = new System.Drawing.Point(142, 18);
			this.shipper2Name.Name = "shipper2Name";
			this.shipper2Name.Size = new System.Drawing.Size(126, 22);
			this.shipper2Name.TabIndex = 1;
			this.shipper2Name.Text = "Shipper";
			//
			// shipper2Gauge
			//
			this.shipper2Gauge.Label = "ON TIME";
			this.shipper2Gauge.Location = new System.Drawing.Point(16, 20);
			this.shipper2Gauge.Name = "shipper2Gauge";
			this.shipper2Gauge.Size = new System.Drawing.Size(114, 114);
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
			this.shipper3Card.Location = new System.Drawing.Point(612, 24);
			this.shipper3Card.Margin = new Wisej.Web.Padding(8);
			this.shipper3Card.Name = "shipper3Card";
			this.shipper3Card.Size = new System.Drawing.Size(278, 156);
			this.shipper3Card.TabIndex = 2;
			//
			// shipper3Stats
			//
			this.shipper3Stats.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper3Stats.AppearanceKey = "nw-subtitle";
			this.shipper3Stats.Location = new System.Drawing.Point(142, 44);
			this.shipper3Stats.Name = "shipper3Stats";
			this.shipper3Stats.Size = new System.Drawing.Size(126, 96);
			this.shipper3Stats.TabIndex = 2;
			this.shipper3Stats.Text = "Orders";
			//
			// shipper3Name
			//
			this.shipper3Name.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
			this.shipper3Name.AppearanceKey = "nw-strong";
			this.shipper3Name.AutoEllipsis = true;
			this.shipper3Name.Location = new System.Drawing.Point(142, 18);
			this.shipper3Name.Name = "shipper3Name";
			this.shipper3Name.Size = new System.Drawing.Size(126, 22);
			this.shipper3Name.TabIndex = 1;
			this.shipper3Name.Text = "Shipper";
			//
			// shipper3Gauge
			//
			this.shipper3Gauge.Label = "ON TIME";
			this.shipper3Gauge.Location = new System.Drawing.Point(16, 20);
			this.shipper3Gauge.Name = "shipper3Gauge";
			this.shipper3Gauge.Size = new System.Drawing.Size(114, 114);
			this.shipper3Gauge.TabIndex = 0;
			this.shipper3Gauge.Thickness = 10;
			this.shipper3Gauge.ValueFormat = "{0:N1}%";
			//
			// leadTimeKpi
			//
			this.leadTimeKpi.Dock = Wisej.Web.DockStyle.Fill;
			this.leadTimeKpi.Icon = "Assets/Icons/fulfillment.svg";
			this.leadTimeKpi.Location = new System.Drawing.Point(906, 24);
			this.leadTimeKpi.Margin = new Wisej.Web.Padding(8);
			this.leadTimeKpi.Name = "leadTimeKpi";
			this.leadTimeKpi.Size = new System.Drawing.Size(278, 156);
			this.leadTimeKpi.TabIndex = 3;
			this.leadTimeKpi.Title = "DAYS TO SHIP";
			this.leadTimeKpi.Value = "0";
			this.leadTimeKpi.Visual = Wisej.Web.KpiVisual.Sparkline;
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
			this.workCard.Location = new System.Drawing.Point(24, 196);
			this.workCard.Margin = new Wisej.Web.Padding(8);
			this.workCard.Name = "workCard";
			this.workCard.Padding = new Wisej.Web.Padding(16, 0, 16, 16);
			this.workCard.ShowHeader = true;
			this.workCard.Size = new System.Drawing.Size(1160, 608);
			this.workCard.TabIndex = 4;
			this.workCard.Text = "Open orders";
			//
			// kanbanBoard
			//
			this.kanbanBoard.AccessibleName = "Fulfillment board";
			this.kanbanBoard.AvatarMember = "SalesRep";
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
			this.kanbanBoard.Size = new System.Drawing.Size(1128, 494);
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
			this.scheduler.AccessibleName = "Delivery calendar";
			this.scheduler.AllowCreate = false;
			this.scheduler.AllowMove = false;
			this.scheduler.AllowResize = false;
			this.scheduler.Dock = Wisej.Web.DockStyle.Fill;
			this.scheduler.Location = new System.Drawing.Point(16, 52);
			this.scheduler.Name = "scheduler";
			this.scheduler.ShowCurrentTime = false;
			this.scheduler.Size = new System.Drawing.Size(1128, 494);
			this.scheduler.TabIndex = 2;
			this.scheduler.View = Wisej.Web.SchedulerViewMode.Month;
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
			this.gantt.Size = new System.Drawing.Size(1128, 494);
			this.gantt.TabIndex = 3;
			this.gantt.TaskListWidth = 300;
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
			this.workToolbar.Size = new System.Drawing.Size(1128, 52);
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
			this.workSummary.Size = new System.Drawing.Size(772, 24);
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
			// FulfillmentView
			//
			this.AppearanceKey = "nw-view";
			this.Controls.Add(this.layout);
			this.Name = "FulfillmentView";
			this.Size = new System.Drawing.Size(1208, 828);
			this.Text = "Fulfillment";
			this.layout.ResumeLayout(false);
			this.shipper1Card.ResumeLayout(false);
			this.shipper2Card.ResumeLayout(false);
			this.shipper3Card.ResumeLayout(false);
			this.workCard.ResumeLayout(false);
			this.workToolbar.ResumeLayout(false);
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
		private Wisej.Web.KpiPanel leadTimeKpi;
		private Wisej.Web.Panel workCard;
		private Wisej.Web.Panel workToolbar;
		private Wisej.Web.SegmentedButton viewSwitcher;
		private Wisej.Web.Label workSummary;
		private Wisej.Web.KanbanBoard kanbanBoard;
		private Wisej.Web.SchedulerView scheduler;
		private Wisej.Web.GanttView gantt;
	}
}
