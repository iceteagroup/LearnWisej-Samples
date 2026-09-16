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
    /// Module 4 fixed this form. It still does everything it did before — subscribes to the process-wide
    /// ticket bus, runs a timer, holds its rows and an image — and it now takes all of that back in
    /// <see cref="Dispose(bool)"/>. The rule is the one in <c>docs/DisposalChecklist.md</c>: whatever a
    /// screen subscribes to, starts, binds or allocates, the same screen releases, in the same class.
    /// </remarks>
    public partial class TicketDetailForm : Form
    {
        // Counters for the lab readings. Static, tiny and never growing — the only kind of static state
        // this course is happy with.
        private static int _created;
        private static int _disposed;

        /// <summary>Detail forms constructed since the process started.</summary>
        public static int CreatedCount => Volatile.Read(ref _created);

        /// <summary>Detail forms that ran their disposal.</summary>
        public static int DisposedCount => Volatile.Read(ref _disposed);

        private readonly TicketGridRow _row;
        private readonly DateTime _openedAt = DateTime.Now;

        private System.Timers.Timer _refreshTimer;
        private Image _largeImage;
        private byte[] _attachmentBuffer;
        private bool _released;

        public TicketDetailForm(TicketGridRow row)
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
            // an attachment. Half a megabyte that is fine while the form is open — and, before Module 4,
            // still there an hour after it closed.
            _largeImage = new Bitmap(600, 400);
            _attachmentBuffer = new byte[512 * 1024];

            GlobalTicketBus.TicketChanged += OnTicketChanged;

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
            if (ticketId != _row.Id || IsDisposed)
                return;

            lblStatus.Text = _row.Status + " · updated elsewhere";
        }

        /// <summary>
        /// Runs on a timer thread, outside the session. It asks whether the form is still alive before
        /// touching a control: a refresh in flight while the form closes must not throw, and a closed
        /// form must not be kept alive by the callback that is about to fail.
        /// </summary>
        private void OnRefreshTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_released || IsDisposed)
                return;

            try
            {
                lblAge.Text = "age " + _row.AgeText + " · open for " +
                              (DateTime.Now - _openedAt).TotalSeconds.ToString("F0") + " s";
            }
            catch (ObjectDisposedException)
            {
                // The form went away between the check and the assignment. Nothing to do, and nothing
                // to hide: the timer is stopped in Dispose, so this can only happen once.
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();

        #region Disposal — the Module 4 fix

        /// <summary>
        /// Takes back every root this form created, in the order it created them, and then lets the base
        /// class dispose the controls. Closing the form by its <b>X</b>, by the Close button, or by the
        /// session ending all arrive here, so there is exactly one disposal path to get right.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_released)
            {
                _released = true;

                GlobalTicketBus.TicketChanged -= OnTicketChanged;   // the static root

                if (_refreshTimer != null)                          // the timer root
                {
                    _refreshTimer.Stop();
                    _refreshTimer.Elapsed -= OnRefreshTimerElapsed;
                    _refreshTimer.Dispose();
                    _refreshTimer = null;
                }

                bindingSource1.DataSource = null;                   // release the rows

                _largeImage?.Dispose();                             // the unmanaged handle
                _largeImage = null;
                _attachmentBuffer = null;

                components?.Dispose();                              // what the designer used to do

                Interlocked.Increment(ref _disposed);
            }

            base.Dispose(disposing);
        }

        #endregion
    }

    /// <summary>A comment row. Small, and there are 25 of them per open form.</summary>
    public sealed class TicketComment
    {
        public string When { get; set; }

        public string Text { get; set; }
    }
}
