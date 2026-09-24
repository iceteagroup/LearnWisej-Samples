namespace IconDesk
{
    partial class CommandPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.appTitleBar = new IconDesk.AppTitleBar();
            this.pnlBody = new Wisej.Web.Panel();
            this.lblCommands = new Wisej.Web.Label();
            this.pnlButtons = new Wisej.Web.FlowLayoutPanel();
            this.btnOpen = new Wisej.Web.Button();
            this.btnSave = new Wisej.Web.Button();
            this.btnPrint = new Wisej.Web.Button();
            this.btnDelete = new Wisej.Web.Button();
            this.pnlDiagnostics = new Wisej.Web.Panel();
            this.lblDiagnosticsCaption = new Wisej.Web.Label();
            this.lblDiagnostics = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.imagesCommands = new Wisej.Web.ImageList(this.components);
            this.SuspendLayout();
            //
            // appTitleBar
            //
            this.appTitleBar.Title = "IconDesk — Command panel";
            //
            // imagesCommands
            //
            // One collection, two controls, addressed by key. ImageSize is what the list establishes for
            // the controls that point at it; 18 is the size the Bootstrap-4 theme gives a button icon,
            // so all four command glyphs come out the same size. The pictures are added in code so the
            // lab can replace one of them without touching a single control.
            this.imagesCommands.ImageSize = new System.Drawing.Size(18, 18);
            //
            // btnOpen
            //
            this.btnOpen.BackColor = System.Drawing.Color.White;
            this.btnOpen.CssStyle = "border:1.5px solid #c9d4e0;border-radius:9px;";
            this.btnOpen.Font = new System.Drawing.Font("default", 10.9F, System.Drawing.FontStyle.Bold);
            this.btnOpen.ForeColor = System.Drawing.Color.FromArgb(52, 70, 90);
            this.btnOpen.ImageSpacing = 7;
            this.btnOpen.Margin = new Wisej.Web.Padding(0, 0, 18, 0);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(150, 76);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Text = "Open";
            this.btnOpen.TextImageRelation = Wisej.Web.TextImageRelation.ImageAboveText;
            this.btnOpen.Click += this.btnOpen_Click;
            //
            // btnSave
            //
            this.btnSave.BackColor = System.Drawing.Color.White;
            this.btnSave.CssStyle = "border:1.5px solid #c9d4e0;border-radius:9px;";
            this.btnSave.Font = new System.Drawing.Font("default", 10.9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(52, 70, 90);
            this.btnSave.ImageSpacing = 7;
            this.btnSave.Margin = new Wisej.Web.Padding(0, 0, 18, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(150, 76);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.TextImageRelation = Wisej.Web.TextImageRelation.ImageAboveText;
            this.btnSave.Click += this.btnSave_Click;
            //
            // btnPrint
            //
            this.btnPrint.BackColor = System.Drawing.Color.White;
            this.btnPrint.CssStyle = "border:1.5px solid #c9d4e0;border-radius:9px;";
            this.btnPrint.Font = new System.Drawing.Font("default", 10.9F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.FromArgb(52, 70, 90);
            this.btnPrint.ImageSpacing = 7;
            this.btnPrint.Margin = new Wisej.Web.Padding(0, 0, 18, 0);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(150, 76);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "Print";
            this.btnPrint.TextImageRelation = Wisej.Web.TextImageRelation.ImageAboveText;
            this.btnPrint.Click += this.btnPrint_Click;
            //
            // btnDelete
            //
            this.btnDelete.BackColor = System.Drawing.Color.White;
            this.btnDelete.CssStyle = "border:1.5px solid #c9d4e0;border-radius:9px;";
            this.btnDelete.Font = new System.Drawing.Font("default", 10.9F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(52, 70, 90);
            this.btnDelete.ImageSpacing = 7;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(150, 76);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete";
            this.btnDelete.TextImageRelation = Wisej.Web.TextImageRelation.ImageAboveText;
            this.btnDelete.Click += this.btnDelete_Click;
            //
            // pnlButtons
            //
            // 76-pixel buttons on one row with an 18-pixel gap; the extra 18 pixels of height are
            // the gap between the row and the diagnostics card.
            this.pnlButtons.Dock = Wisej.Web.DockStyle.Top;
            this.pnlButtons.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(960, 94);
            this.pnlButtons.WrapContents = true;
            this.pnlButtons.Controls.Add(this.btnOpen);
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnPrint);
            this.pnlButtons.Controls.Add(this.btnDelete);
            //
            // lblCommands
            //
            this.lblCommands.AutoSize = false;
            this.lblCommands.Dock = Wisej.Web.DockStyle.Top;
            this.lblCommands.Font = new System.Drawing.Font("default", 12.4F, System.Drawing.FontStyle.Bold);
            this.lblCommands.ForeColor = System.Drawing.Color.FromArgb(13, 27, 42);
            this.lblCommands.Name = "lblCommands";
            this.lblCommands.Size = new System.Drawing.Size(960, 28);
            this.lblCommands.Text = "Commands";
            this.lblCommands.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // lblDiagnosticsCaption
            //
            this.lblDiagnosticsCaption.AutoSize = false;
            this.lblDiagnosticsCaption.CssStyle = "letter-spacing:.05em;";
            this.lblDiagnosticsCaption.Dock = Wisej.Web.DockStyle.Top;
            this.lblDiagnosticsCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblDiagnosticsCaption.ForeColor = System.Drawing.Color.FromArgb(138, 152, 168);
            this.lblDiagnosticsCaption.Name = "lblDiagnosticsCaption";
            this.lblDiagnosticsCaption.Size = new System.Drawing.Size(928, 22);
            this.lblDiagnosticsCaption.Text = "LBLDIAGNOSTICS — READ THROUGH IIMAGE";
            //
            // lblDiagnostics
            //
            // One line per button, written by DescribeIcon after that button is clicked. The lines
            // report the property that is populated, never the picture.
            this.lblDiagnostics.AllowHtml = true;
            this.lblDiagnostics.AutoSize = false;
            this.lblDiagnostics.Dock = Wisej.Web.DockStyle.Fill;
            this.lblDiagnostics.Name = "lblDiagnostics";
            this.lblDiagnostics.Size = new System.Drawing.Size(928, 112);
            this.lblDiagnostics.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // pnlDiagnostics
            //
            this.pnlDiagnostics.BackColor = System.Drawing.Color.FromArgb(247, 250, 253);
            this.pnlDiagnostics.CssStyle = "border:1px solid #e0e8f1;border-radius:9px;";
            this.pnlDiagnostics.Dock = Wisej.Web.DockStyle.Top;
            this.pnlDiagnostics.Name = "pnlDiagnostics";
            this.pnlDiagnostics.Padding = new Wisej.Web.Padding(16, 12, 16, 12);
            this.pnlDiagnostics.Size = new System.Drawing.Size(960, 162);
            this.pnlDiagnostics.Controls.Add(this.lblDiagnostics);
            this.pnlDiagnostics.Controls.Add(this.lblDiagnosticsCaption);
            //
            // pnlBody
            //
            this.pnlBody.BackColor = System.Drawing.Color.White;
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new Wisej.Web.Padding(26, 24, 26, 0);
            this.pnlBody.Controls.Add(this.pnlDiagnostics);
            this.pnlBody.Controls.Add(this.pnlButtons);
            this.pnlBody.Controls.Add(this.lblCommands);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.lblStatus.CssStyle = "border-top:1px solid #e4eaf1;";
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("default", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(138, 152, 168);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(26, 0, 26, 0);
            this.lblStatus.Size = new System.Drawing.Size(1000, 42);
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // CommandPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Name = "CommandPage";
            this.Size = new System.Drawing.Size(1000, 640);
            this.Text = "IconDesk — Command panel";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.appTitleBar);
            this.ResumeLayout(false);
        }

        #endregion

        private IconDesk.AppTitleBar appTitleBar;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Label lblCommands;
        private Wisej.Web.FlowLayoutPanel pnlButtons;
        private Wisej.Web.Button btnOpen;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnPrint;
        private Wisej.Web.Button btnDelete;
        private Wisej.Web.Panel pnlDiagnostics;
        private Wisej.Web.Label lblDiagnosticsCaption;
        private Wisej.Web.Label lblDiagnostics;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.ImageList imagesCommands;
    }
}
