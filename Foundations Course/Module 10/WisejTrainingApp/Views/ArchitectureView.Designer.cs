namespace WisejTrainingApp.Views
{
    partial class ArchitectureView
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
            this.pnlDataFlow = new Wisej.Web.Panel();
            this.labelDataFlowCard = new Wisej.Web.Label();
            this.lblDataFlow = new Wisej.Web.Label();
            this.lblFlowStory = new Wisej.Web.Label();
            this.pnlLayers = new Wisej.Web.Panel();
            this.labelLayersCard = new Wisej.Web.Label();
            this.lstLayers = new Wisej.Web.ListBox();
            this.lblLayerTitle = new Wisej.Web.Label();
            this.lblLayerDetail = new Wisej.Web.Label();
            this.lblLayerFile = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlDataFlow.SuspendLayout();
            this.pnlLayers.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlDataFlow  (left card: the lesson's list, verbatim, in monospace)
            //
            this.pnlDataFlow.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.pnlDataFlow.BackColor = System.Drawing.Color.White;
            this.pnlDataFlow.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlDataFlow.Controls.Add(this.labelDataFlowCard);
            this.pnlDataFlow.Controls.Add(this.lblDataFlow);
            this.pnlDataFlow.Controls.Add(this.lblFlowStory);
            this.pnlDataFlow.Location = new System.Drawing.Point(0, 0);
            this.pnlDataFlow.Name = "pnlDataFlow";
            this.pnlDataFlow.Size = new System.Drawing.Size(500, 532);
            //
            // labelDataFlowCard
            //
            this.labelDataFlowCard.AutoSize = false;
            this.labelDataFlowCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelDataFlowCard.Location = new System.Drawing.Point(24, 14);
            this.labelDataFlowCard.Name = "labelDataFlowCard";
            this.labelDataFlowCard.Size = new System.Drawing.Size(452, 28);
            this.labelDataFlowCard.Text = "Data flow  ·  lesson s46 §3";
            //
            // lblDataFlow
            //
            this.lblDataFlow.AutoSize = false;
            this.lblDataFlow.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblDataFlow.Font = new System.Drawing.Font("monospace", 10F);
            this.lblDataFlow.Location = new System.Drawing.Point(24, 56);
            this.lblDataFlow.Name = "lblDataFlow";
            this.lblDataFlow.Padding = new Wisej.Web.Padding(16, 12, 16, 12);
            this.lblDataFlow.Size = new System.Drawing.Size(452, 220);
            this.lblDataFlow.Text = "";
            this.lblDataFlow.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // lblFlowStory
            //
            this.lblFlowStory.AutoSize = false;
            this.lblFlowStory.ForeColor = System.Drawing.Color.FromArgb(40, 52, 70);
            this.lblFlowStory.Location = new System.Drawing.Point(24, 292);
            this.lblFlowStory.Name = "lblFlowStory";
            this.lblFlowStory.Size = new System.Drawing.Size(452, 200);
            this.lblFlowStory.Text = "The flow: the user clicks Create or Edit on the Tickets screen, TicketDialog opens, ValidateForm() runs TicketValidator, TicketService updates the list through TicketRepository, RefreshTicketGrid() rebinds dgvTickets, and lblStatus plus the Dashboard's activity log record what changed.\n\nEach arrow is one file. Nothing skips a layer: the dialog never touches the service, the shell never touches a ticket, the repository never touches a control.";
            this.lblFlowStory.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // pnlLayers  (right card: one job per layer)
            //
            this.pnlLayers.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlLayers.BackColor = System.Drawing.Color.White;
            this.pnlLayers.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlLayers.Controls.Add(this.labelLayersCard);
            this.pnlLayers.Controls.Add(this.lstLayers);
            this.pnlLayers.Controls.Add(this.lblLayerTitle);
            this.pnlLayers.Controls.Add(this.lblLayerDetail);
            this.pnlLayers.Controls.Add(this.lblLayerFile);
            this.pnlLayers.Controls.Add(this.lblStatus);
            this.pnlLayers.Location = new System.Drawing.Point(516, 0);
            this.pnlLayers.Name = "pnlLayers";
            this.pnlLayers.Size = new System.Drawing.Size(516, 532);
            //
            // labelLayersCard
            //
            this.labelLayersCard.AutoSize = false;
            this.labelLayersCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelLayersCard.Location = new System.Drawing.Point(24, 14);
            this.labelLayersCard.Name = "labelLayersCard";
            this.labelLayersCard.Size = new System.Drawing.Size(468, 28);
            this.labelLayersCard.Text = "Each layer has one job  ·  select one";
            //
            // lstLayers
            //
            this.lstLayers.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstLayers.Font = new System.Drawing.Font("monospace", 9F);
            this.lstLayers.Location = new System.Drawing.Point(24, 56);
            this.lstLayers.Name = "lstLayers";
            this.lstLayers.Size = new System.Drawing.Size(468, 200);
            this.lstLayers.SelectedIndexChanged += new System.EventHandler(this.lstLayers_SelectedIndexChanged);
            //
            // lblLayerTitle
            //
            this.lblLayerTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblLayerTitle.AutoSize = false;
            this.lblLayerTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblLayerTitle.Location = new System.Drawing.Point(24, 272);
            this.lblLayerTitle.Name = "lblLayerTitle";
            this.lblLayerTitle.Size = new System.Drawing.Size(468, 26);
            this.lblLayerTitle.Text = "";
            //
            // lblLayerDetail
            //
            this.lblLayerDetail.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblLayerDetail.AutoSize = false;
            this.lblLayerDetail.ForeColor = System.Drawing.Color.FromArgb(40, 52, 70);
            this.lblLayerDetail.Location = new System.Drawing.Point(24, 302);
            this.lblLayerDetail.Name = "lblLayerDetail";
            this.lblLayerDetail.Size = new System.Drawing.Size(468, 130);
            this.lblLayerDetail.Text = "";
            this.lblLayerDetail.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // lblLayerFile
            //
            this.lblLayerFile.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblLayerFile.AutoSize = false;
            this.lblLayerFile.Font = new System.Drawing.Font("monospace", 9F);
            this.lblLayerFile.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblLayerFile.Location = new System.Drawing.Point(24, 444);
            this.lblLayerFile.Name = "lblLayerFile";
            this.lblLayerFile.Size = new System.Drawing.Size(468, 24);
            this.lblLayerFile.Text = "";
            //
            // lblStatus
            //
            this.lblStatus.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(24, 476);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(468, 26);
            this.lblStatus.Text = "● select a layer";
            //
            // ArchitectureView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlDataFlow);
            this.Controls.Add(this.pnlLayers);
            this.Name = "ArchitectureView";
            this.Size = new System.Drawing.Size(1032, 532);
            this.pnlDataFlow.ResumeLayout(false);
            this.pnlLayers.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlDataFlow;
        private Wisej.Web.Label labelDataFlowCard;
        private Wisej.Web.Label lblDataFlow;
        private Wisej.Web.Label lblFlowStory;
        private Wisej.Web.Panel pnlLayers;
        private Wisej.Web.Label labelLayersCard;
        private Wisej.Web.ListBox lstLayers;
        private Wisej.Web.Label lblLayerTitle;
        private Wisej.Web.Label lblLayerDetail;
        private Wisej.Web.Label lblLayerFile;
        private Wisej.Web.Label lblStatus;
    }
}
