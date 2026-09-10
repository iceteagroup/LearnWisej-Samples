namespace OrderDesk
{
    partial class MainPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelShell = new Wisej.Web.Panel();
            this.labelShellTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.shell = new OrderDesk.Shell.AppShell();
            this.panelDialogs = new Wisej.Web.Panel();
            this.labelDialogsTitle = new Wisej.Web.Label();
            this.labelDialogCount = new Wisej.Web.Label();
            this.buttonEditGood = new Wisej.Web.Button();
            this.buttonEditLeak = new Wisej.Web.Button();
            this.buttonReuse = new Wisej.Web.Button();
            this.buttonDelete = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.buttonBlocking = new Wisej.Web.Button();
            this.buttonStartTask = new Wisej.Web.Button();
            this.progressOp = new Wisej.Web.ProgressBar();
            this.labelProgress = new Wisej.Web.Label();
            this.timerProgress = new Wisej.Web.Timer(this.components);
            this.trace = new OrderDesk.Views.TracePanel();
            this.panelReview = new Wisej.Web.Panel();
            this.labelReviewTitle = new Wisej.Web.Label();
            this.labelBanner = new Wisej.Web.Label();
            this.panelShell.SuspendLayout();
            this.panelDialogs.SuspendLayout();
            this.panelReview.SuspendLayout();
            this.SuspendLayout();
            //
            // panelShell  (the ported application: MenuBar + ToolBar + screen host + StatusBar)
            //
            this.panelShell.BackColor = System.Drawing.Color.White;
            this.panelShell.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelShell.Controls.Add(this.labelShellTitle);
            this.panelShell.Controls.Add(this.labelStatus);
            this.panelShell.Controls.Add(this.shell);
            this.panelShell.Location = new System.Drawing.Point(30, 30);
            this.panelShell.Name = "panelShell";
            this.panelShell.Size = new System.Drawing.Size(640, 452);
            //
            // labelShellTitle
            //
            this.labelShellTitle.AutoSize = false;
            this.labelShellTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelShellTitle.Location = new System.Drawing.Point(20, 10);
            this.labelShellTitle.Name = "labelShellTitle";
            this.labelShellTitle.Size = new System.Drawing.Size(360, 30);
            this.labelShellTitle.Text = "OrderDesk.Web · the ported shell";
            //
            // labelStatus
            //
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(380, 14);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(242, 24);
            this.labelStatus.Text = "● loading";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // shell  (Shell/AppShell — MenuBar, ToolBar, screen host, StatusBar; screens under Screens/)
            //
            this.shell.Location = new System.Drawing.Point(12, 44);
            this.shell.Name = "shell";
            this.shell.Size = new System.Drawing.Size(616, 396);
            this.shell.Trace += new System.EventHandler<OrderDesk.Views.TraceEventArgs>(this.shell_Trace);
            this.shell.Navigated += new System.EventHandler(this.shell_Navigated);
            //
            // panelDialogs  (dialog lifetime: the leak counter and the blocking-vs-StartTask test)
            //
            this.panelDialogs.BackColor = System.Drawing.Color.White;
            this.panelDialogs.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelDialogs.Controls.Add(this.labelDialogsTitle);
            this.panelDialogs.Controls.Add(this.labelDialogCount);
            this.panelDialogs.Controls.Add(this.buttonEditGood);
            this.panelDialogs.Controls.Add(this.buttonEditLeak);
            this.panelDialogs.Controls.Add(this.buttonReuse);
            this.panelDialogs.Controls.Add(this.buttonDelete);
            this.panelDialogs.Controls.Add(this.buttonClear);
            this.panelDialogs.Controls.Add(this.buttonBlocking);
            this.panelDialogs.Controls.Add(this.buttonStartTask);
            this.panelDialogs.Controls.Add(this.progressOp);
            this.panelDialogs.Controls.Add(this.labelProgress);
            this.panelDialogs.Location = new System.Drawing.Point(30, 494);
            this.panelDialogs.Name = "panelDialogs";
            this.panelDialogs.Size = new System.Drawing.Size(640, 160);
            //
            // labelDialogsTitle
            //
            this.labelDialogsTitle.AutoSize = false;
            this.labelDialogsTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelDialogsTitle.Location = new System.Drawing.Point(20, 10);
            this.labelDialogsTitle.Name = "labelDialogsTitle";
            this.labelDialogsTitle.Size = new System.Drawing.Size(602, 26);
            this.labelDialogsTitle.Text = "Dialog lifetime · DialogTracker";
            //
            // labelDialogCount  (live EditOrderDialog instances: this session / process-wide)
            //
            this.labelDialogCount.AutoSize = false;
            this.labelDialogCount.Font = new System.Drawing.Font("monospace", 9F);
            this.labelDialogCount.Location = new System.Drawing.Point(20, 38);
            this.labelDialogCount.Name = "labelDialogCount";
            this.labelDialogCount.Size = new System.Drawing.Size(602, 36);
            this.labelDialogCount.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // row 1: the four dialog paths + Clear
            //
            this.buttonEditGood.Location = new System.Drawing.Point(20, 80);
            this.buttonEditGood.Name = "buttonEditGood";
            this.buttonEditGood.Size = new System.Drawing.Size(120, 30);
            this.buttonEditGood.Text = "Edit (disposed)";
            this.buttonEditGood.ToolTipText = "Success path: await ShowDialogAsync inside a using block — the same path as double-clicking a row.";
            this.buttonEditGood.Click += new System.EventHandler(this.buttonEditGood_Click);
            this.buttonEditLeak.Location = new System.Drawing.Point(146, 80);
            this.buttonEditLeak.Name = "buttonEditLeak";
            this.buttonEditLeak.Size = new System.Drawing.Size(120, 30);
            this.buttonEditLeak.Text = "Edit (leak ×1)";
            this.buttonEditLeak.ToolTipText = "Failure path: the desktop habit — new + ShowDialog, never Dispose. The live count rises and stays.";
            this.buttonEditLeak.Click += new System.EventHandler(this.buttonEditLeak_Click);
            this.buttonReuse.Location = new System.Drawing.Point(272, 80);
            this.buttonReuse.Name = "buttonReuse";
            this.buttonReuse.Size = new System.Drawing.Size(130, 30);
            this.buttonReuse.Text = "Reuse one dialog";
            this.buttonReuse.ToolTipText = "Alternative: one instance kept in a field, Bind(order) before each show, disposed with the page.";
            this.buttonReuse.Click += new System.EventHandler(this.buttonReuse_Click);
            this.buttonDelete.Location = new System.Drawing.Point(408, 80);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(110, 30);
            this.buttonDelete.Text = "Delete order…";
            this.buttonDelete.ToolTipText = "A decision that stays modal: await MessageBox.ShowAsync(YesNo).";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            this.buttonClear.Location = new System.Drawing.Point(564, 80);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(58, 30);
            this.buttonClear.Text = "Clear";
            this.buttonClear.ToolTipText = "Clear the trace.";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // row 2: blocking vs StartTask, with a Timer-driven progress bar as the responsiveness probe
            //
            this.buttonBlocking.Location = new System.Drawing.Point(20, 116);
            this.buttonBlocking.Name = "buttonBlocking";
            this.buttonBlocking.Size = new System.Drawing.Size(130, 30);
            this.buttonBlocking.Text = "Blocking op (3 s)";
            this.buttonBlocking.ToolTipText = "Failure path: Thread.Sleep(3000) in the handler — the session answers nothing for 3 s.";
            this.buttonBlocking.Click += new System.EventHandler(this.buttonBlocking_Click);
            this.buttonStartTask.Location = new System.Drawing.Point(156, 116);
            this.buttonStartTask.Name = "buttonStartTask";
            this.buttonStartTask.Size = new System.Drawing.Size(130, 30);
            this.buttonStartTask.Text = "StartTask (3 s)";
            this.buttonStartTask.ToolTipText = "Progress path: Application.StartTask + Application.Update — the timer keeps ticking.";
            this.buttonStartTask.Click += new System.EventHandler(this.buttonStartTask_Click);
            this.progressOp.Location = new System.Drawing.Point(292, 118);
            this.progressOp.Maximum = 100;
            this.progressOp.Minimum = 0;
            this.progressOp.Name = "progressOp";
            this.progressOp.Size = new System.Drawing.Size(200, 26);
            this.progressOp.Value = 0;
            this.labelProgress.AutoSize = false;
            this.labelProgress.Font = new System.Drawing.Font("monospace", 9F);
            this.labelProgress.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelProgress.Location = new System.Drawing.Point(498, 116);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(124, 30);
            this.labelProgress.Text = "idle";
            this.labelProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // timerProgress
            //
            this.timerProgress.Interval = 200;
            this.timerProgress.Tick += new System.EventHandler(this.timerProgress_Tick);
            //
            // trace  (the migration log)
            //
            this.trace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.trace.Location = new System.Drawing.Point(690, 30);
            this.trace.Name = "trace";
            this.trace.Size = new System.Drawing.Size(628, 440);
            //
            // panelReview  (status + the banner that explains each path)
            //
            this.panelReview.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelReview.BackColor = System.Drawing.Color.White;
            this.panelReview.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelReview.Controls.Add(this.labelReviewTitle);
            this.panelReview.Controls.Add(this.labelBanner);
            this.panelReview.Location = new System.Drawing.Point(690, 484);
            this.panelReview.Name = "panelReview";
            this.panelReview.Size = new System.Drawing.Size(628, 170);
            //
            // labelReviewTitle
            //
            this.labelReviewTitle.AutoSize = false;
            this.labelReviewTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelReviewTitle.Location = new System.Drawing.Point(20, 10);
            this.labelReviewTitle.Name = "labelReviewTitle";
            this.labelReviewTitle.Size = new System.Drawing.Size(590, 26);
            this.labelReviewTitle.Text = "Review · what the desktop assumed, what the web does instead";
            //
            // labelBanner
            //
            this.labelBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(230, 247, 237);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(22, 101, 52);
            this.labelBanner.Location = new System.Drawing.Point(20, 42);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 6, 12, 6);
            this.labelBanner.Size = new System.Drawing.Size(590, 112);
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.labelBanner.Visible = false;
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelShell);
            this.Controls.Add(this.panelDialogs);
            this.Controls.Add(this.trace);
            this.Controls.Add(this.panelReview);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 684);
            this.Text = "OrderDesk — Forms, Navigation & Modal Workflow";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelShell.ResumeLayout(false);
            this.panelDialogs.ResumeLayout(false);
            this.panelReview.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelShell;
        private Wisej.Web.Label labelShellTitle;
        private Wisej.Web.Label labelStatus;
        private OrderDesk.Shell.AppShell shell;
        private Wisej.Web.Panel panelDialogs;
        private Wisej.Web.Label labelDialogsTitle;
        private Wisej.Web.Label labelDialogCount;
        private Wisej.Web.Button buttonEditGood;
        private Wisej.Web.Button buttonEditLeak;
        private Wisej.Web.Button buttonReuse;
        private Wisej.Web.Button buttonDelete;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Button buttonBlocking;
        private Wisej.Web.Button buttonStartTask;
        private Wisej.Web.ProgressBar progressOp;
        private Wisej.Web.Label labelProgress;
        private Wisej.Web.Timer timerProgress;
        private OrderDesk.Views.TracePanel trace;
        private Wisej.Web.Panel panelReview;
        private Wisej.Web.Label labelReviewTitle;
        private Wisej.Web.Label labelBanner;
    }
}
