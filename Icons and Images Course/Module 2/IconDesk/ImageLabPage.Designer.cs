namespace IconDesk
{
    partial class ImageLabPage
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
            this.btnFetchSlow = new Wisej.Web.Button();
            this.btnFetchRemote = new Wisej.Web.Button();
            this.btnReport = new Wisej.Web.Button();
            this.pnlBody = new Wisej.Web.Panel();
            this.lblSizeModesHeader = new Wisej.Web.Label();
            this.layoutSizeModes = new Wisej.Web.TableLayoutPanel();
            this.pnlNormal = new Wisej.Web.Panel();
            this.lblNormal = new Wisej.Web.Label();
            this.picNormal = new Wisej.Web.PictureBox();
            this.pnlZoom = new Wisej.Web.Panel();
            this.lblZoom = new Wisej.Web.Label();
            this.picZoom = new Wisej.Web.PictureBox();
            this.pnlCover = new Wisej.Web.Panel();
            this.lblCover = new Wisej.Web.Label();
            this.picCover = new Wisej.Web.PictureBox();
            this.pnlStretch = new Wisej.Web.Panel();
            this.lblStretch = new Wisej.Web.Label();
            this.picStretch = new Wisej.Web.PictureBox();
            this.pnlLower = new Wisej.Web.Panel();
            this.pnlRemote = new Wisej.Web.Panel();
            this.lblRemoteHeader = new Wisej.Web.Label();
            this.picRemote = new Wisej.Web.PictureBox();
            this.pnlVector = new Wisej.Web.Panel();
            this.lblVectorHeader = new Wisej.Web.Label();
            this.pnlVectorSizes = new Wisej.Web.FlowLayoutPanel();
            this.picSvg16 = new Wisej.Web.PictureBox();
            this.picSvg24 = new Wisej.Web.PictureBox();
            this.picSvg32 = new Wisej.Web.PictureBox();
            this.picSvg48 = new Wisej.Web.PictureBox();
            this.lblReport = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(360, 28);
            this.lblTitle.Text = "Image lab";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblSubtitle.Location = new System.Drawing.Point(20, 42);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(900, 22);
            this.lblSubtitle.Text = "One raster in four size modes, one JPEG fetched asynchronously, and one SVG at four sizes.";
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
            // btnBack
            //
            this.btnBack.Location = new System.Drawing.Point(20, 9);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(170, 38);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "Back to commands";
            this.btnBack.Click += this.btnBack_Click;
            //
            // btnFetchSlow
            //
            this.btnFetchSlow.Location = new System.Drawing.Point(202, 9);
            this.btnFetchSlow.Name = "btnFetchSlow";
            this.btnFetchSlow.Size = new System.Drawing.Size(230, 38);
            this.btnFetchSlow.TabIndex = 1;
            this.btnFetchSlow.Text = "Fetch the slow JPEG (2.5 s)";
            this.btnFetchSlow.Click += this.btnFetchSlow_Click;
            //
            // btnFetchRemote
            //
            this.btnFetchRemote.Location = new System.Drawing.Point(444, 9);
            this.btnFetchRemote.Name = "btnFetchRemote";
            this.btnFetchRemote.Size = new System.Drawing.Size(230, 38);
            this.btnFetchRemote.TabIndex = 2;
            this.btnFetchRemote.Text = "Fetch a remote JPEG";
            this.btnFetchRemote.Click += this.btnFetchRemote_Click;
            //
            // btnReport
            //
            this.btnReport.Location = new System.Drawing.Point(686, 9);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(230, 38);
            this.btnReport.TabIndex = 3;
            this.btnReport.Text = "Report every PictureBox";
            this.btnReport.Click += this.btnReport_Click;
            //
            // pnlActions
            //
            this.pnlActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1000, 56);
            this.pnlActions.Controls.Add(this.btnReport);
            this.pnlActions.Controls.Add(this.btnFetchRemote);
            this.pnlActions.Controls.Add(this.btnFetchSlow);
            this.pnlActions.Controls.Add(this.btnBack);
            //
            // lblSizeModesHeader
            //
            this.lblSizeModesHeader.AutoSize = false;
            this.lblSizeModesHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblSizeModesHeader.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblSizeModesHeader.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblSizeModesHeader.Name = "lblSizeModesHeader";
            this.lblSizeModesHeader.Size = new System.Drawing.Size(960, 26);
            this.lblSizeModesHeader.Text = "One 480 x 300 raster, four SizeMode values, identical 220 x 150 boxes";
            //
            // the four size-mode cells
            //
            this.lblNormal.AutoSize = false;
            this.lblNormal.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblNormal.Name = "lblNormal";
            this.lblNormal.Size = new System.Drawing.Size(220, 22);
            this.lblNormal.Text = "Normal - no scaling, clipped";
            this.picNormal.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.picNormal.Dock = Wisej.Web.DockStyle.Fill;
            this.picNormal.Name = "picNormal";
            this.picNormal.SizeMode = Wisej.Web.PictureBoxSizeMode.Normal;
            this.pnlNormal.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlNormal.Name = "pnlNormal";
            this.pnlNormal.Padding = new Wisej.Web.Padding(0, 0, 12, 0);
            this.pnlNormal.Controls.Add(this.picNormal);
            this.pnlNormal.Controls.Add(this.lblNormal);

            this.lblZoom.AutoSize = false;
            this.lblZoom.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblZoom.Name = "lblZoom";
            this.lblZoom.Size = new System.Drawing.Size(220, 22);
            this.lblZoom.Text = "Zoom - fits, whole picture";
            this.picZoom.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.picZoom.Dock = Wisej.Web.DockStyle.Fill;
            this.picZoom.Name = "picZoom";
            this.picZoom.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.pnlZoom.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlZoom.Name = "pnlZoom";
            this.pnlZoom.Padding = new Wisej.Web.Padding(0, 0, 12, 0);
            this.pnlZoom.Controls.Add(this.picZoom);
            this.pnlZoom.Controls.Add(this.lblZoom);

            this.lblCover.AutoSize = false;
            this.lblCover.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblCover.Name = "lblCover";
            this.lblCover.Size = new System.Drawing.Size(220, 22);
            this.lblCover.Text = "Cover - fills, edges cropped";
            this.picCover.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.picCover.Dock = Wisej.Web.DockStyle.Fill;
            this.picCover.Name = "picCover";
            this.picCover.SizeMode = Wisej.Web.PictureBoxSizeMode.Cover;
            this.pnlCover.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlCover.Name = "pnlCover";
            this.pnlCover.Padding = new Wisej.Web.Padding(0, 0, 12, 0);
            this.pnlCover.Controls.Add(this.picCover);
            this.pnlCover.Controls.Add(this.lblCover);

            this.lblStretch.AutoSize = false;
            this.lblStretch.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStretch.Name = "lblStretch";
            this.lblStretch.Size = new System.Drawing.Size(220, 22);
            this.lblStretch.Text = "StretchImage - fills, distorted";
            this.picStretch.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.picStretch.Dock = Wisej.Web.DockStyle.Fill;
            this.picStretch.Name = "picStretch";
            this.picStretch.SizeMode = Wisej.Web.PictureBoxSizeMode.StretchImage;
            this.pnlStretch.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlStretch.Name = "pnlStretch";
            this.pnlStretch.Controls.Add(this.picStretch);
            this.pnlStretch.Controls.Add(this.lblStretch);
            //
            // layoutSizeModes
            //
            this.layoutSizeModes.ColumnCount = 4;
            this.layoutSizeModes.RowCount = 1;
            this.layoutSizeModes.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 25F));
            this.layoutSizeModes.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 25F));
            this.layoutSizeModes.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 25F));
            this.layoutSizeModes.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 25F));
            this.layoutSizeModes.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
            this.layoutSizeModes.Dock = Wisej.Web.DockStyle.Top;
            this.layoutSizeModes.Name = "layoutSizeModes";
            this.layoutSizeModes.Size = new System.Drawing.Size(960, 180);
            this.layoutSizeModes.Controls.Add(this.pnlNormal, 0, 0);
            this.layoutSizeModes.Controls.Add(this.pnlZoom, 1, 0);
            this.layoutSizeModes.Controls.Add(this.pnlCover, 2, 0);
            this.layoutSizeModes.Controls.Add(this.pnlStretch, 3, 0);
            //
            // the remote picture
            //
            this.lblRemoteHeader.AutoSize = false;
            this.lblRemoteHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblRemoteHeader.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblRemoteHeader.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblRemoteHeader.Name = "lblRemoteHeader";
            this.lblRemoteHeader.Size = new System.Drawing.Size(360, 26);
            this.lblRemoteHeader.Text = "A JPEG fetched through LoadAsync";
            this.picRemote.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.picRemote.Dock = Wisej.Web.DockStyle.Fill;
            this.picRemote.Name = "picRemote";
            this.picRemote.ShowLoader = true;
            this.picRemote.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.picRemote.LoadCompleted += this.picRemote_LoadCompleted;
            this.pnlRemote.Dock = Wisej.Web.DockStyle.Left;
            this.pnlRemote.Name = "pnlRemote";
            this.pnlRemote.Padding = new Wisej.Web.Padding(0, 12, 24, 0);
            this.pnlRemote.Size = new System.Drawing.Size(360, 200);
            this.pnlRemote.Controls.Add(this.picRemote);
            this.pnlRemote.Controls.Add(this.lblRemoteHeader);
            //
            // the vector at four sizes
            //
            this.lblVectorHeader.AutoSize = false;
            this.lblVectorHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblVectorHeader.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblVectorHeader.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblVectorHeader.Name = "lblVectorHeader";
            this.lblVectorHeader.Size = new System.Drawing.Size(500, 26);
            this.lblVectorHeader.Text = "One SVG on ImageSource at 16, 24, 32 and 48 pixels";
            this.picSvg16.Name = "picSvg16";
            this.picSvg16.Size = new System.Drawing.Size(16, 16);
            this.picSvg16.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.picSvg24.Name = "picSvg24";
            this.picSvg24.Size = new System.Drawing.Size(24, 24);
            this.picSvg24.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.picSvg32.Name = "picSvg32";
            this.picSvg32.Size = new System.Drawing.Size(32, 32);
            this.picSvg32.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.picSvg48.Name = "picSvg48";
            this.picSvg48.Size = new System.Drawing.Size(48, 48);
            this.picSvg48.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.pnlVectorSizes.Dock = Wisej.Web.DockStyle.Top;
            this.pnlVectorSizes.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.pnlVectorSizes.Name = "pnlVectorSizes";
            this.pnlVectorSizes.Size = new System.Drawing.Size(500, 64);
            this.pnlVectorSizes.Controls.Add(this.picSvg16);
            this.pnlVectorSizes.Controls.Add(this.picSvg24);
            this.pnlVectorSizes.Controls.Add(this.picSvg32);
            this.pnlVectorSizes.Controls.Add(this.picSvg48);
            //
            // lblReport
            //
            this.lblReport.AllowHtml = true;
            this.lblReport.AutoSize = false;
            this.lblReport.Dock = Wisej.Web.DockStyle.Fill;
            this.lblReport.Name = "lblReport";
            this.lblReport.Padding = new Wisej.Web.Padding(0, 10, 0, 0);
            this.lblReport.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // pnlVector
            //
            this.pnlVector.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlVector.Name = "pnlVector";
            this.pnlVector.Padding = new Wisej.Web.Padding(0, 12, 0, 0);
            this.pnlVector.Controls.Add(this.lblReport);
            this.pnlVector.Controls.Add(this.pnlVectorSizes);
            this.pnlVector.Controls.Add(this.lblVectorHeader);
            //
            // pnlLower
            //
            this.pnlLower.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlLower.Name = "pnlLower";
            this.pnlLower.Controls.Add(this.pnlVector);
            this.pnlLower.Controls.Add(this.pnlRemote);
            //
            // pnlBody
            //
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new Wisej.Web.Padding(20);
            this.pnlBody.Controls.Add(this.pnlLower);
            this.pnlBody.Controls.Add(this.layoutSizeModes);
            this.pnlBody.Controls.Add(this.lblSizeModesHeader);
            //
            // ImageLabPage
            //
            this.Name = "ImageLabPage";
            this.Size = new System.Drawing.Size(1000, 640);
            this.Text = "IconDesk - Image lab";
            this.Controls.Add(this.pnlBody);
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
        private Wisej.Web.Button btnFetchSlow;
        private Wisej.Web.Button btnFetchRemote;
        private Wisej.Web.Button btnReport;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Label lblSizeModesHeader;
        private Wisej.Web.TableLayoutPanel layoutSizeModes;
        private Wisej.Web.Panel pnlNormal;
        private Wisej.Web.Label lblNormal;
        private Wisej.Web.PictureBox picNormal;
        private Wisej.Web.Panel pnlZoom;
        private Wisej.Web.Label lblZoom;
        private Wisej.Web.PictureBox picZoom;
        private Wisej.Web.Panel pnlCover;
        private Wisej.Web.Label lblCover;
        private Wisej.Web.PictureBox picCover;
        private Wisej.Web.Panel pnlStretch;
        private Wisej.Web.Label lblStretch;
        private Wisej.Web.PictureBox picStretch;
        private Wisej.Web.Panel pnlLower;
        private Wisej.Web.Panel pnlRemote;
        private Wisej.Web.Label lblRemoteHeader;
        private Wisej.Web.PictureBox picRemote;
        private Wisej.Web.Panel pnlVector;
        private Wisej.Web.Label lblVectorHeader;
        private Wisej.Web.FlowLayoutPanel pnlVectorSizes;
        private Wisej.Web.PictureBox picSvg16;
        private Wisej.Web.PictureBox picSvg24;
        private Wisej.Web.PictureBox picSvg32;
        private Wisej.Web.PictureBox picSvg48;
        private Wisej.Web.Label lblReport;
    }
}
