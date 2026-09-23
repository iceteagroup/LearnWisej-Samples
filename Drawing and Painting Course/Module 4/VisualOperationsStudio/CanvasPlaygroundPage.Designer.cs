namespace VisualOperationsStudio
{
    partial class CanvasPlaygroundPage
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
            this.btnProgressive = new Wisej.Web.Button();
            this.canvasPlayground = new Wisej.Web.Canvas();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(500, 28);
            this.lblTitle.Text = "Canvas playground";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblSubtitle.Location = new System.Drawing.Point(20, 42);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(900, 22);
            this.lblSubtitle.Text = "Browser Canvas 2D examples ported to Wisej.Web.Canvas - one Redraw, one scene";
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
            this.lblStatus.Text = "LiveUpdate is off: the whole scene arrives in one update.";
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
            // btnProgressive
            //
            this.btnProgressive.Location = new System.Drawing.Point(202, 9);
            this.btnProgressive.Name = "btnProgressive";
            this.btnProgressive.Size = new System.Drawing.Size(230, 38);
            this.btnProgressive.TabIndex = 1;
            this.btnProgressive.Text = "Progressive draw (LiveUpdate)";
            this.btnProgressive.Click += this.btnProgressive_Click;
            //
            // pnlActions
            //
            this.pnlActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1080, 56);
            this.pnlActions.Controls.Add(this.btnProgressive);
            this.pnlActions.Controls.Add(this.btnBack);
            //
            // canvasPlayground
            //
            this.canvasPlayground.Dock = Wisej.Web.DockStyle.Fill;
            this.canvasPlayground.LiveUpdate = false;
            this.canvasPlayground.Name = "canvasPlayground";
            this.canvasPlayground.Redraw += this.canvasPlayground_Redraw;
            //
            // CanvasPlaygroundPage
            //
            this.Name = "CanvasPlaygroundPage";
            this.Size = new System.Drawing.Size(1080, 660);
            this.Text = "VisualOperationsStudio - Canvas playground";
            this.Controls.Add(this.canvasPlayground);
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
        private Wisej.Web.Button btnProgressive;
        private Wisej.Web.Canvas canvasPlayground;
    }
}
