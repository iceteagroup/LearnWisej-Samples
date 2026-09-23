namespace VisualOperationsStudio
{
    partial class TopologyPage
    {
        private System.ComponentModel.IContainer components = null;

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
            this.pnlActions = new Wisej.Web.Panel();
            this.btnBack = new Wisej.Web.Button();
            this.btnZoomIn = new Wisej.Web.Button();
            this.btnZoomOut = new Wisej.Web.Button();
            this.btnResetView = new Wisej.Web.Button();
            this.pnlNodes = new Wisej.Web.Panel();
            this.lblNodesHeader = new Wisej.Web.Label();
            this.listNodes = new Wisej.Web.ListBox();
            this.canvasTopology = new Wisej.Web.Canvas();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(500, 28);
            this.lblTitle.Text = "Plant topology";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblSubtitle.Location = new System.Drawing.Point(20, 42);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(900, 22);
            this.lblSubtitle.Text = "Click to select, drag to move, drag empty space to pan, wheel to zoom. Tab and the arrow keys work too.";
            //
            // pnlHeader
            //
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1080, 72);
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.lblTitle);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(20, 8, 20, 8);
            this.lblStatus.Size = new System.Drawing.Size(1080, 34);
            this.lblStatus.Text = "Nothing selected.";
            //
            // btnBack
            //
            this.btnBack.Location = new System.Drawing.Point(20, 9);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(170, 38);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "Back to operations";
            this.btnBack.Click += this.btnBack_Click;
            //
            // btnZoomIn
            //
            this.btnZoomIn.Location = new System.Drawing.Point(202, 9);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(90, 38);
            this.btnZoomIn.TabIndex = 1;
            this.btnZoomIn.Text = "Zoom in";
            this.btnZoomIn.Click += this.btnZoomIn_Click;
            //
            // btnZoomOut
            //
            this.btnZoomOut.Location = new System.Drawing.Point(300, 9);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(100, 38);
            this.btnZoomOut.TabIndex = 2;
            this.btnZoomOut.Text = "Zoom out";
            this.btnZoomOut.Click += this.btnZoomOut_Click;
            //
            // btnResetView
            //
            this.btnResetView.Location = new System.Drawing.Point(408, 9);
            this.btnResetView.Name = "btnResetView";
            this.btnResetView.Size = new System.Drawing.Size(110, 38);
            this.btnResetView.TabIndex = 3;
            this.btnResetView.Text = "Reset view";
            this.btnResetView.Click += this.btnResetView_Click;
            //
            // pnlActions
            //
            this.pnlActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1080, 56);
            this.pnlActions.Controls.Add(this.btnResetView);
            this.pnlActions.Controls.Add(this.btnZoomOut);
            this.pnlActions.Controls.Add(this.btnZoomIn);
            this.pnlActions.Controls.Add(this.btnBack);
            //
            // lblNodesHeader
            //
            this.lblNodesHeader.AutoSize = false;
            this.lblNodesHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblNodesHeader.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblNodesHeader.Name = "lblNodesHeader";
            this.lblNodesHeader.Padding = new Wisej.Web.Padding(12, 6, 12, 6);
            this.lblNodesHeader.Size = new System.Drawing.Size(280, 32);
            this.lblNodesHeader.Text = "The same nodes, as a list";
            //
            // listNodes
            //
            this.listNodes.Dock = Wisej.Web.DockStyle.Fill;
            this.listNodes.Name = "listNodes";
            this.listNodes.SelectedIndexChanged += this.listNodes_SelectedIndexChanged;
            //
            // pnlNodes
            //
            this.pnlNodes.Dock = Wisej.Web.DockStyle.Right;
            this.pnlNodes.Name = "pnlNodes";
            this.pnlNodes.Size = new System.Drawing.Size(280, 400);
            this.pnlNodes.Controls.Add(this.listNodes);
            this.pnlNodes.Controls.Add(this.lblNodesHeader);
            //
            // canvasTopology
            //
            this.canvasTopology.Dock = Wisej.Web.DockStyle.Fill;
            this.canvasTopology.Focusable = true;
            this.canvasTopology.LiveUpdate = false;
            this.canvasTopology.Name = "canvasTopology";
            this.canvasTopology.TabStop = true;
            this.canvasTopology.Redraw += this.canvasTopology_Redraw;
            this.canvasTopology.MouseDown += this.canvasTopology_MouseDown;
            this.canvasTopology.MouseMove += this.canvasTopology_MouseMove;
            this.canvasTopology.MouseUp += this.canvasTopology_MouseUp;
            this.canvasTopology.MouseWheel += this.canvasTopology_MouseWheel;
            this.canvasTopology.KeyDown += this.canvasTopology_KeyDown;
            //
            // TopologyPage
            //
            this.Name = "TopologyPage";
            this.Size = new System.Drawing.Size(1080, 660);
            this.Text = "VisualOperationsStudio - Plant topology";
            this.Controls.Add(this.canvasTopology);
            this.Controls.Add(this.pnlNodes);
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
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnBack;
        private Wisej.Web.Button btnZoomIn;
        private Wisej.Web.Button btnZoomOut;
        private Wisej.Web.Button btnResetView;
        private Wisej.Web.Panel pnlNodes;
        private Wisej.Web.Label lblNodesHeader;
        private Wisej.Web.ListBox listNodes;
        private Wisej.Web.Canvas canvasTopology;
    }
}
