using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Timers;
using WisejPerfLab.Models;
using WisejPerfLab.Services;
using Wisej.Web;

namespace WisejPerfLab.Forms
{
    /// <summary>One ticket, opened from the grid.</summary>
    /// <remarks>
    /// Nothing here looks wrong, and every line of it is a retention root:
    /// <list type="bullet">
    /// <item>it subscribes to the static <see cref="GlobalTicketBus.TicketChanged"/> event and never unsubscribes;</item>
    /// <item>it starts a <see cref="System.Timers.Timer"/> whose callback captures <c>this</c>, and never stops it;</item>
    /// <item>it holds a binding source with its rows and an image it never disposes.</item>
    /// </list>
    /// Closing the form removes it from the screen. It does not remove any of that from the heap, because
    /// the static event and the running timer still reference it. Module 4 finds this form in a Memory
    /// Usage snapshot comparison, reads its path to root, and adds the <c>Dispose(bool)</c> override that
    /// takes every root away.
    /// </remarks>
    public partial class TicketDetailForm : Form
    {
        // Counters for the Module 4 readings. "Disposed" really does reach 50 when you close 50 forms —
        // and the heap stays up anyway, which is the whole point: disposal is not collection.
        private static int _created;
        private static int _disposed;

        /// <summary>Detail forms constructed since the process started.</summary>
        public static int CreatedCount => Volatile.Read(ref _created);

        /// <summary>Detail forms that ran their disposal.</summary>
        public static int DisposedCount => Volatile.Read(ref _disposed);

        internal static void CountDisposal() => Interlocked.Increment(ref _disposed);

        private readonly TicketDisplayRow _row;
        private readonly DateTime _openedAt = DateTime.Now;

        private System.Timers.Timer _refreshTimer;
        private Image _largeImage;
        private byte[] _attachmentBuffer;

        public TicketDetailForm(TicketDisplayRow row)
        {
            InitializeComponent();

            _row = row;
            Interlocked.Increment(ref _created);

            Text = "Ticket " + row.Number;
            lblNumber.Text = row.Number;
            lblCustomer.Text = row.Customer;
            lblStatus.Text = row.Status + " · " + row.Priority;
            lblAge.Text = "age " + row.AgeText;

            BuildCommentGrid();

            // What a detail form typically carries: the rows it shows, a preview image and the bytes of
            // an attachment. Half a megabyte that is fine while the form is open and is the whole problem
            // once it is not.
            _largeImage = new Bitmap(600, 400);
            _attachmentBuffer = new byte[512 * 1024];

            // Root 1: a static event. Every open adds a handler, no close removes one.
            GlobalTicketBus.TicketChanged += OnTicketChanged;

            // Root 2: a running timer that captured this form.
            _refreshTimer = new System.Timers.Timer(1000);
            _refreshTimer.Elapsed += OnRefreshTimerElapsed;
            _refreshTimer.AutoReset = true;
            _refreshTimer.Start();

            lblRoots.Text =
                $"bus subscribers: {GlobalTicketBus.SubscriberCount}   timer: running\r\n" +
                "opened " + _openedAt.ToString("HH:mm:ss");
        }

        private void BuildCommentGrid()
        {
            gridComments.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colWhen", DataPropertyName = nameof(TicketComment.When), HeaderText = "When", Width = 140 });
            gridComments.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colText", DataPropertyName = nameof(TicketComment.Text), HeaderText = "Comment", Width = 300 });

            var comments = new List<TicketComment>();
            for (var i = 0; i < 25; i++)
                comments.Add(new TicketComment
                {
                    When = DateTime.Now.AddHours(-i).ToString("g"),
                    Text = "Handover note " + (i + 1) + " for " + _row.Number
                });

            bindingSource1.DataSource = comments;
            gridComments.DataSource = bindingSource1;
        }

        private void OnTicketChanged(object sender, int ticketId)
        {
            if (ticketId != _row.Id)
                return;

            lblStatus.Text = _row.Status + " · updated elsewhere";
        }

        /// <summary>
        /// Runs on a timer thread, outside the session. It touches a control without asking whether the
        /// form is still alive — after the form is closed this throws, and nobody sees it.
        /// </summary>
        private void OnRefreshTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                lblAge.Text = "age " + _row.AgeText + " · open for " +
                              (DateTime.Now - _openedAt).TotalSeconds.ToString("F0") + " s";
            }
            catch (Exception)
            {
                // Swallowed, like most timer callbacks are.
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }

    /// <summary>A comment row. Small, and there are 25 of them per open form.</summary>
    public sealed class TicketComment
    {
        public string When { get; set; }

        public string Text { get; set; }
    }
}
