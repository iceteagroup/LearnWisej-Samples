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
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblTitle = new Wisej.Web.Label();
            this.lblSubtitle = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlCommands = new Wisej.Web.Panel();
            this.lblCommandsHeader = new Wisej.Web.Label();
            this.pnlButtons = new Wisej.Web.FlowLayoutPanel();
            this.btnOpen = new Wisej.Web.Button();
            this.btnSave = new Wisej.Web.Button();
            this.btnPrint = new Wisej.Web.Button();
            this.btnDelete = new Wisej.Web.Button();
            this.lblDeleteEcho = new Wisej.Web.Label();
            this.btnConflict = new Wisej.Web.Button();
            this.lblDiagnostics = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnInspect = new Wisej.Web.Button();
            this.btnSwapDelete = new Wisej.Web.Button();
            this.btnSetBoth = new Wisej.Web.Button();
            this.btnImageLab = new Wisej.Web.Button();
            this.btnGallery = new Wisej.Web.Button();
            this.imagesCommands = new Wisej.Web.ImageList(this.components);
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(360, 28);
            this.lblTitle.Text = "IconDesk";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblSubtitle.Location = new System.Drawing.Point(20, 42);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(760, 22);
            this.lblSubtitle.Text = "Four commands, four different ways of carrying an icon.";
            //
            // pnlHeader
            //
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1000, 72);
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.lblTitle);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(20, 8, 20, 8);
            this.lblStatus.Size = new System.Drawing.Size(1000, 34);
            this.lblStatus.Text = "Ready.";
            //
            // imagesCommands
            //
            // One collection, two controls, addressed by key. The pictures are added in code so the
            // Module 1 swap can replace one of them without touching a single control.
            this.imagesCommands.ImageSize = new System.Drawing.Size(24, 24);
            //
            // btnOpen
            //
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(150, 44);
            this.btnOpen.Text = "Open";
            this.btnOpen.TextImageRelation = Wisej.Web.TextImageRelation.ImageBeforeText;
            //
            // btnSave
            //
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(150, 44);
            this.btnSave.Text = "Save";
            this.btnSave.TextImageRelation = Wisej.Web.TextImageRelation.ImageBeforeText;
            //
            // btnPrint
            //
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(150, 44);
            this.btnPrint.Text = "Print";
            this.btnPrint.TextImageRelation = Wisej.Web.TextImageRelation.ImageBeforeText;
            //
            // btnDelete
            //
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(150, 44);
            this.btnDelete.Text = "Delete";
            this.btnDelete.TextImageRelation = Wisej.Web.TextImageRelation.ImageBeforeText;
            //
            // lblDeleteEcho
            //
            // A second control on the same key, so replacing the picture can be seen reaching more
            // than the button that triggered it.
            this.lblDeleteEcho.AutoSize = false;
            this.lblDeleteEcho.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDeleteEcho.Name = "lblDeleteEcho";
            this.lblDeleteEcho.Padding = new Wisej.Web.Padding(34, 0, 12, 0);
            this.lblDeleteEcho.Size = new System.Drawing.Size(230, 44);
            this.lblDeleteEcho.Text = "on the same key";
            this.lblDeleteEcho.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnConflict
            //
            this.btnConflict.Name = "btnConflict";
            this.btnConflict.Size = new System.Drawing.Size(210, 44);
            this.btnConflict.Text = "Two properties";
            this.btnConflict.TextImageRelation = Wisej.Web.TextImageRelation.ImageBeforeText;
            //
            // pnlButtons
            //
            this.pnlButtons.Dock = Wisej.Web.DockStyle.Top;
            this.pnlButtons.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(960, 108);
            this.pnlButtons.WrapContents = true;
            this.pnlButtons.Controls.Add(this.btnOpen);
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnPrint);
            this.pnlButtons.Controls.Add(this.btnDelete);
            this.pnlButtons.Controls.Add(this.lblDeleteEcho);
            this.pnlButtons.Controls.Add(this.btnConflict);
            //
            // lblCommandsHeader
            //
            this.lblCommandsHeader.AutoSize = false;
            this.lblCommandsHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblCommandsHeader.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblCommandsHeader.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblCommandsHeader.Name = "lblCommandsHeader";
            this.lblCommandsHeader.Size = new System.Drawing.Size(960, 26);
            this.lblCommandsHeader.Text = "The command panel";
            //
            // lblDiagnostics
            //
            // Reads every control through IImage and names the mechanism actually in play.
            this.lblDiagnostics.AllowHtml = true;
            this.lblDiagnostics.AutoSize = false;
            this.lblDiagnostics.Dock = Wisej.Web.DockStyle.Fill;
            this.lblDiagnostics.Name = "lblDiagnostics";
            this.lblDiagnostics.Padding = new Wisej.Web.Padding(0, 12, 0, 0);
            this.lblDiagnostics.Size = new System.Drawing.Size(960, 300);
            this.lblDiagnostics.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // pnlCommands
            //
            this.pnlCommands.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlCommands.Name = "pnlCommands";
            this.pnlCommands.Padding = new Wisej.Web.Padding(20);
            this.pnlCommands.Controls.Add(this.lblDiagnostics);
            this.pnlCommands.Controls.Add(this.pnlButtons);
            this.pnlCommands.Controls.Add(this.lblCommandsHeader);
            //
            // btnInspect
            //
            this.btnInspect.Location = new System.Drawing.Point(20, 9);
            this.btnInspect.Name = "btnInspect";
            this.btnInspect.Size = new System.Drawing.Size(190, 38);
            this.btnInspect.TabIndex = 0;
            this.btnInspect.Text = "Inspect through IImage";
            this.btnInspect.Click += this.btnInspect_Click;
            //
            // btnSwapDelete
            //
            this.btnSwapDelete.Location = new System.Drawing.Point(222, 9);
            this.btnSwapDelete.Name = "btnSwapDelete";
            this.btnSwapDelete.Size = new System.Drawing.Size(256, 38);
            this.btnSwapDelete.TabIndex = 1;
            this.btnSwapDelete.Text = "Replace the image under the key";
            this.btnSwapDelete.Click += this.btnSwapDelete_Click;
            //
            // btnSetBoth
            //
            this.btnSetBoth.Location = new System.Drawing.Point(490, 9);
            this.btnSetBoth.Name = "btnSetBoth";
            this.btnSetBoth.Size = new System.Drawing.Size(290, 38);
            this.btnSetBoth.TabIndex = 2;
            this.btnSetBoth.Text = "Set Image and ImageSource on one button";
            this.btnSetBoth.Click += this.btnSetBoth_Click;
            //
            // btnImageLab
            //
            this.btnImageLab.Location = new System.Drawing.Point(790, 9);
            this.btnImageLab.Name = "btnImageLab";
            this.btnImageLab.Size = new System.Drawing.Size(150, 38);
            this.btnImageLab.TabIndex = 3;
            this.btnImageLab.Text = "Image lab";
            this.btnImageLab.Click += this.btnImageLab_Click;
            //
            // btnGallery
            //
            this.btnGallery.Location = new System.Drawing.Point(946, 9);
            this.btnGallery.Name = "btnGallery";
            this.btnGallery.Size = new System.Drawing.Size(150, 38);
            this.btnGallery.TabIndex = 4;
            this.btnGallery.Text = "Icon gallery";
            this.btnGallery.Click += this.btnGallery_Click;
            //
            // pnlActions
            //
            this.pnlActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1000, 56);
            this.pnlActions.Controls.Add(this.btnGallery);
            this.pnlActions.Controls.Add(this.btnImageLab);
            this.pnlActions.Controls.Add(this.btnSetBoth);
            this.pnlActions.Controls.Add(this.btnSwapDelete);
            this.pnlActions.Controls.Add(this.btnInspect);
            //
            // CommandPage
            //
            this.Name = "CommandPage";
            this.Size = new System.Drawing.Size(1000, 640);
            this.Text = "IconDesk - Commands";
            this.Controls.Add(this.pnlCommands);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblSubtitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlCommands;
        private Wisej.Web.Label lblCommandsHeader;
        private Wisej.Web.FlowLayoutPanel pnlButtons;
        private Wisej.Web.Button btnOpen;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnPrint;
        private Wisej.Web.Button btnDelete;
        private Wisej.Web.Label lblDeleteEcho;
        private Wisej.Web.Button btnConflict;
        private Wisej.Web.Label lblDiagnostics;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnInspect;
        private Wisej.Web.Button btnSwapDelete;
        private Wisej.Web.Button btnSetBoth;
        private Wisej.Web.Button btnImageLab;
        private Wisej.Web.Button btnGallery;
        private Wisej.Web.ImageList imagesCommands;
    }
}
