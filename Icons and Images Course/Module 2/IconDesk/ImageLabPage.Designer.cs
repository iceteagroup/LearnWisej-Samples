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
            this.appTitleBar = new IconDesk.AppTitleBar();
            this.pnlBody = new Wisej.Web.Panel();
            this.lblModesHeader = new Wisej.Web.Label();
            this.pnlModes = new Wisej.Web.Panel();
            this.picNormal = new Wisej.Web.PictureBox();
            this.lblNormalName = new Wisej.Web.Label();
            this.lblNormalNote = new Wisej.Web.Label();
            this.picZoom = new Wisej.Web.PictureBox();
            this.lblZoomName = new Wisej.Web.Label();
            this.lblZoomNote = new Wisej.Web.Label();
            this.picCover = new Wisej.Web.PictureBox();
            this.lblCoverName = new Wisej.Web.Label();
            this.lblCoverNote = new Wisej.Web.Label();
            this.picStretch = new Wisej.Web.PictureBox();
            this.lblStretchName = new Wisej.Web.Label();
            this.lblStretchNote = new Wisej.Web.Label();
            this.pnlLower = new Wisej.Web.Panel();
            this.pnlRemote = new Wisej.Web.Panel();
            this.lblRemoteHeader = new Wisej.Web.Label();
            this.pnlRemoteBox = new Wisej.Web.Panel();
            this.lblRemoteWaiting = new Wisej.Web.Label();
            this.picRemote = new Wisej.Web.PictureBox();
            this.pnlVector = new Wisej.Web.Panel();
            this.lblVectorHeader = new Wisej.Web.Label();
            this.pnlVectorBox = new Wisej.Web.Panel();
            this.lblSvgRow = new Wisej.Web.Label();
            this.picSvg16 = new Wisej.Web.PictureBox();
            this.picSvg24 = new Wisej.Web.PictureBox();
            this.picSvg32 = new Wisej.Web.PictureBox();
            this.picSvg48 = new Wisej.Web.PictureBox();
            this.lblLegacyRow = new Wisej.Web.Label();
            this.picLegacy16 = new Wisej.Web.PictureBox();
            this.picLegacy24 = new Wisej.Web.PictureBox();
            this.picLegacy32 = new Wisej.Web.PictureBox();
            this.picLegacy48 = new Wisej.Web.PictureBox();
            this.pnlReport = new Wisej.Web.Panel();
            this.lblReport = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // appTitleBar
            //
            this.appTitleBar.Size = new System.Drawing.Size(1000, 42);
            this.appTitleBar.Title = "IconDesk — ImageLab";
            //
            // lblModesHeader
            //
            this.lblModesHeader.AutoSize = false;
            this.lblModesHeader.CssStyle = "letter-spacing:.05em;";
            this.lblModesHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblModesHeader.Font = new System.Drawing.Font("default", 9.4F, System.Drawing.FontStyle.Bold);
            this.lblModesHeader.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblModesHeader.Name = "lblModesHeader";
            this.lblModesHeader.Size = new System.Drawing.Size(960, 20);
            this.lblModesHeader.Text = "ONE RASTER ASSET · FOUR SIZE MODES";
            //
            // the four size-mode frames - identical 220 x 150 boxes, one SizeMode each
            //
            this.picNormal.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.picNormal.CssStyle = "border:1.5px solid #8a93a0;border-radius:6px;";
            this.picNormal.Location = new System.Drawing.Point(0, 10);
            this.picNormal.Name = "picNormal";
            this.picNormal.Size = new System.Drawing.Size(220, 150);
            this.picNormal.SizeMode = Wisej.Web.PictureBoxSizeMode.Normal;
            this.lblNormalName.AutoSize = false;
            this.lblNormalName.Font = new System.Drawing.Font("Consolas", 10.1F, System.Drawing.FontStyle.Bold);
            this.lblNormalName.ForeColor = System.Drawing.Color.FromArgb(138, 147, 160);
            this.lblNormalName.Location = new System.Drawing.Point(0, 167);
            this.lblNormalName.Name = "lblNormalName";
            this.lblNormalName.Size = new System.Drawing.Size(220, 20);
            this.lblNormalName.Text = "Normal";
            this.lblNormalNote.AutoSize = false;
            this.lblNormalNote.Font = new System.Drawing.Font("default", 9.4F);
            this.lblNormalNote.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNormalNote.Location = new System.Drawing.Point(0, 187);
            this.lblNormalNote.Name = "lblNormalNote";
            this.lblNormalNote.Size = new System.Drawing.Size(220, 18);
            this.lblNormalNote.Text = "original size · clipped";

            this.picZoom.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.picZoom.CssStyle = "border:1.5px solid #1fae5a;border-radius:6px;";
            this.picZoom.Location = new System.Drawing.Point(236, 10);
            this.picZoom.Name = "picZoom";
            this.picZoom.Size = new System.Drawing.Size(220, 150);
            this.picZoom.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.lblZoomName.AutoSize = false;
            this.lblZoomName.Font = new System.Drawing.Font("Consolas", 10.1F, System.Drawing.FontStyle.Bold);
            this.lblZoomName.ForeColor = System.Drawing.Color.FromArgb(31, 174, 90);
            this.lblZoomName.Location = new System.Drawing.Point(236, 167);
            this.lblZoomName.Name = "lblZoomName";
            this.lblZoomName.Size = new System.Drawing.Size(220, 20);
            this.lblZoomName.Text = "Zoom";
            this.lblZoomNote.AutoSize = false;
            this.lblZoomNote.Font = new System.Drawing.Font("default", 9.4F);
            this.lblZoomNote.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblZoomNote.Location = new System.Drawing.Point(236, 187);
            this.lblZoomNote.Name = "lblZoomNote";
            this.lblZoomNote.Size = new System.Drawing.Size(220, 18);
            this.lblZoomNote.Text = "whole picture · ratio kept";

            this.picCover.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.picCover.CssStyle = "border:1.5px solid #1a86ff;border-radius:6px;";
            this.picCover.Location = new System.Drawing.Point(472, 10);
            this.picCover.Name = "picCover";
            this.picCover.Size = new System.Drawing.Size(220, 150);
            this.picCover.SizeMode = Wisej.Web.PictureBoxSizeMode.Cover;
            this.lblCoverName.AutoSize = false;
            this.lblCoverName.Font = new System.Drawing.Font("Consolas", 10.1F, System.Drawing.FontStyle.Bold);
            this.lblCoverName.ForeColor = System.Drawing.Color.FromArgb(26, 134, 255);
            this.lblCoverName.Location = new System.Drawing.Point(472, 167);
            this.lblCoverName.Name = "lblCoverName";
            this.lblCoverName.Size = new System.Drawing.Size(220, 20);
            this.lblCoverName.Text = "Cover";
            this.lblCoverNote.AutoSize = false;
            this.lblCoverNote.Font = new System.Drawing.Font("default", 9.4F);
            this.lblCoverNote.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCoverNote.Location = new System.Drawing.Point(472, 187);
            this.lblCoverNote.Name = "lblCoverNote";
            this.lblCoverNote.Size = new System.Drawing.Size(220, 18);
            this.lblCoverNote.Text = "fills the box · edges cropped";

            this.picStretch.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.picStretch.CssStyle = "border:1.5px solid #e05a3b;border-radius:6px;";
            this.picStretch.Location = new System.Drawing.Point(708, 10);
            this.picStretch.Name = "picStretch";
            this.picStretch.Size = new System.Drawing.Size(220, 150);
            this.picStretch.SizeMode = Wisej.Web.PictureBoxSizeMode.StretchImage;
            this.lblStretchName.AutoSize = false;
            this.lblStretchName.Font = new System.Drawing.Font("Consolas", 10.1F, System.Drawing.FontStyle.Bold);
            this.lblStretchName.ForeColor = System.Drawing.Color.FromArgb(224, 90, 59);
            this.lblStretchName.Location = new System.Drawing.Point(708, 167);
            this.lblStretchName.Name = "lblStretchName";
            this.lblStretchName.Size = new System.Drawing.Size(220, 20);
            this.lblStretchName.Text = "StretchImage";
            this.lblStretchNote.AutoSize = false;
            this.lblStretchNote.Font = new System.Drawing.Font("default", 9.4F);
            this.lblStretchNote.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblStretchNote.Location = new System.Drawing.Point(708, 187);
            this.lblStretchNote.Name = "lblStretchNote";
            this.lblStretchNote.Size = new System.Drawing.Size(220, 18);
            this.lblStretchNote.Text = "fills the box · ratio lost";
            //
            // pnlModes
            //
            this.pnlModes.Dock = Wisej.Web.DockStyle.Top;
            this.pnlModes.Name = "pnlModes";
            this.pnlModes.Size = new System.Drawing.Size(960, 205);
            this.pnlModes.Controls.Add(this.picNormal);
            this.pnlModes.Controls.Add(this.lblNormalName);
            this.pnlModes.Controls.Add(this.lblNormalNote);
            this.pnlModes.Controls.Add(this.picZoom);
            this.pnlModes.Controls.Add(this.lblZoomName);
            this.pnlModes.Controls.Add(this.lblZoomNote);
            this.pnlModes.Controls.Add(this.picCover);
            this.pnlModes.Controls.Add(this.lblCoverName);
            this.pnlModes.Controls.Add(this.lblCoverNote);
            this.pnlModes.Controls.Add(this.picStretch);
            this.pnlModes.Controls.Add(this.lblStretchName);
            this.pnlModes.Controls.Add(this.lblStretchNote);
            //
            // the remote picture
            //
            this.lblRemoteHeader.AutoSize = false;
            this.lblRemoteHeader.CssStyle = "letter-spacing:.05em;";
            this.lblRemoteHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblRemoteHeader.Font = new System.Drawing.Font("default", 9.4F, System.Drawing.FontStyle.Bold);
            this.lblRemoteHeader.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblRemoteHeader.Name = "lblRemoteHeader";
            this.lblRemoteHeader.Size = new System.Drawing.Size(300, 20);
            this.lblRemoteHeader.Text = "LOADASYNC · REMOTE JPEG";
            //
            // lblRemoteWaiting
            //
            // Shown until LoadCompleted arrives, so the box says what it is waiting for instead of
            // looking broken.
            this.lblRemoteWaiting.AutoSize = false;
            this.lblRemoteWaiting.Dock = Wisej.Web.DockStyle.Fill;
            this.lblRemoteWaiting.Font = new System.Drawing.Font("Consolas", 10.1F);
            this.lblRemoteWaiting.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblRemoteWaiting.Name = "lblRemoteWaiting";
            this.lblRemoteWaiting.Size = new System.Drawing.Size(300, 128);
            this.lblRemoteWaiting.Text = "waiting for the host…";
            this.lblRemoteWaiting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // picRemote
            //
            this.picRemote.Dock = Wisej.Web.DockStyle.Fill;
            this.picRemote.Name = "picRemote";
            this.picRemote.SizeMode = Wisej.Web.PictureBoxSizeMode.Cover;
            this.picRemote.Visible = false;
            this.picRemote.LoadCompleted += this.picRemote_LoadCompleted;
            //
            // pnlRemoteBox
            //
            this.pnlRemoteBox.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.pnlRemoteBox.CssStyle = "border:1.5px solid #c9d4e0;border-radius:6px;overflow:hidden;";
            this.pnlRemoteBox.Location = new System.Drawing.Point(0, 28);
            this.pnlRemoteBox.Name = "pnlRemoteBox";
            this.pnlRemoteBox.Size = new System.Drawing.Size(300, 128);
            this.pnlRemoteBox.Controls.Add(this.lblRemoteWaiting);
            this.pnlRemoteBox.Controls.Add(this.picRemote);
            //
            // pnlRemote
            //
            this.pnlRemote.Dock = Wisej.Web.DockStyle.Left;
            this.pnlRemote.Name = "pnlRemote";
            this.pnlRemote.Size = new System.Drawing.Size(320, 172);
            this.pnlRemote.Controls.Add(this.pnlRemoteBox);
            this.pnlRemote.Controls.Add(this.lblRemoteHeader);
            //
            // the SVG row and the bitmap row
            //
            this.lblVectorHeader.AutoSize = false;
            this.lblVectorHeader.CssStyle = "letter-spacing:.05em;";
            this.lblVectorHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblVectorHeader.Font = new System.Drawing.Font("default", 9.4F, System.Drawing.FontStyle.Bold);
            this.lblVectorHeader.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblVectorHeader.Name = "lblVectorHeader";
            this.lblVectorHeader.Size = new System.Drawing.Size(600, 20);
            this.lblVectorHeader.Text = "IMAGESOURCE SVG VS BITMAP · 16 · 24 · 32 · 48";

            this.lblSvgRow.AutoSize = false;
            this.lblSvgRow.Font = new System.Drawing.Font("default", 9.4F, System.Drawing.FontStyle.Bold);
            this.lblSvgRow.ForeColor = System.Drawing.Color.FromArgb(31, 174, 90);
            this.lblSvgRow.Location = new System.Drawing.Point(0, 30);
            this.lblSvgRow.Name = "lblSvgRow";
            this.lblSvgRow.Size = new System.Drawing.Size(96, 20);
            this.lblSvgRow.Text = "SVG";
            this.picSvg16.Location = new System.Drawing.Point(96, 38);
            this.picSvg16.Name = "picSvg16";
            this.picSvg16.Size = new System.Drawing.Size(16, 16);
            this.picSvg16.ForeColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.picSvg16.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.picSvg24.Location = new System.Drawing.Point(134, 30);
            this.picSvg24.Name = "picSvg24";
            this.picSvg24.Size = new System.Drawing.Size(24, 24);
            this.picSvg24.ForeColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.picSvg24.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.picSvg32.Location = new System.Drawing.Point(180, 22);
            this.picSvg32.Name = "picSvg32";
            this.picSvg32.Size = new System.Drawing.Size(32, 32);
            this.picSvg32.ForeColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.picSvg32.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.picSvg48.Location = new System.Drawing.Point(234, 6);
            this.picSvg48.Name = "picSvg48";
            this.picSvg48.Size = new System.Drawing.Size(48, 48);
            this.picSvg48.ForeColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.picSvg48.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;

            this.lblLegacyRow.AutoSize = false;
            this.lblLegacyRow.Font = new System.Drawing.Font("default", 9.4F, System.Drawing.FontStyle.Bold);
            this.lblLegacyRow.ForeColor = System.Drawing.Color.FromArgb(224, 90, 59);
            this.lblLegacyRow.Location = new System.Drawing.Point(0, 84);
            this.lblLegacyRow.Name = "lblLegacyRow";
            this.lblLegacyRow.Size = new System.Drawing.Size(96, 20);
            this.lblLegacyRow.Text = "bitmap";
            this.picLegacy16.Location = new System.Drawing.Point(96, 92);
            this.picLegacy16.Name = "picLegacy16";
            this.picLegacy16.Size = new System.Drawing.Size(16, 16);
            this.picLegacy16.SizeMode = Wisej.Web.PictureBoxSizeMode.StretchImage;
            this.picLegacy24.Location = new System.Drawing.Point(134, 84);
            this.picLegacy24.Name = "picLegacy24";
            this.picLegacy24.Size = new System.Drawing.Size(24, 24);
            this.picLegacy24.SizeMode = Wisej.Web.PictureBoxSizeMode.StretchImage;
            this.picLegacy32.Location = new System.Drawing.Point(180, 76);
            this.picLegacy32.Name = "picLegacy32";
            this.picLegacy32.Size = new System.Drawing.Size(32, 32);
            this.picLegacy32.SizeMode = Wisej.Web.PictureBoxSizeMode.StretchImage;
            this.picLegacy48.Location = new System.Drawing.Point(234, 60);
            this.picLegacy48.Name = "picLegacy48";
            this.picLegacy48.Size = new System.Drawing.Size(48, 48);
            this.picLegacy48.SizeMode = Wisej.Web.PictureBoxSizeMode.StretchImage;
            //
            // pnlVectorBox
            //
            this.pnlVectorBox.BackColor = System.Drawing.Color.White;
            this.pnlVectorBox.CssStyle = "border:1.5px solid #c9d4e0;border-radius:6px;";
            this.pnlVectorBox.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlVectorBox.Name = "pnlVectorBox";
            this.pnlVectorBox.Padding = new Wisej.Web.Padding(14, 10, 14, 10);
            this.pnlVectorBox.Controls.Add(this.lblSvgRow);
            this.pnlVectorBox.Controls.Add(this.picSvg16);
            this.pnlVectorBox.Controls.Add(this.picSvg24);
            this.pnlVectorBox.Controls.Add(this.picSvg32);
            this.pnlVectorBox.Controls.Add(this.picSvg48);
            this.pnlVectorBox.Controls.Add(this.lblLegacyRow);
            this.pnlVectorBox.Controls.Add(this.picLegacy16);
            this.pnlVectorBox.Controls.Add(this.picLegacy24);
            this.pnlVectorBox.Controls.Add(this.picLegacy32);
            this.pnlVectorBox.Controls.Add(this.picLegacy48);
            //
            // pnlVector
            //
            this.pnlVector.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlVector.Name = "pnlVector";
            this.pnlVector.Padding = new Wisej.Web.Padding(0, 28, 0, 16);
            this.pnlVector.Controls.Add(this.pnlVectorBox);
            this.pnlVector.Controls.Add(this.lblVectorHeader);
            //
            // pnlLower
            //
            this.pnlLower.Dock = Wisej.Web.DockStyle.Top;
            this.pnlLower.Name = "pnlLower";
            this.pnlLower.Padding = new Wisej.Web.Padding(0, 16, 0, 0);
            this.pnlLower.Size = new System.Drawing.Size(960, 188);
            this.pnlLower.Controls.Add(this.pnlVector);
            this.pnlLower.Controls.Add(this.pnlRemote);
            //
            // lblReport
            //
            // One line per PictureBox, naming the property that actually supplied its picture.
            this.lblReport.AllowHtml = true;
            this.lblReport.AutoSize = false;
            this.lblReport.Dock = Wisej.Web.DockStyle.Fill;
            this.lblReport.Name = "lblReport";
            this.lblReport.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // pnlReport
            //
            this.pnlReport.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.pnlReport.CssStyle = "border:1px solid #dde4ec;border-left:4px solid #1565d8;border-radius:6px;";
            this.pnlReport.Dock = Wisej.Web.DockStyle.Top;
            this.pnlReport.Name = "pnlReport";
            this.pnlReport.Padding = new Wisej.Web.Padding(16, 10, 16, 10);
            this.pnlReport.Size = new System.Drawing.Size(960, 164);
            this.pnlReport.Controls.Add(this.lblReport);
            //
            // pnlBody
            //
            this.pnlBody.BackColor = System.Drawing.Color.White;
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new Wisej.Web.Padding(24, 18, 24, 20);
            this.pnlBody.Controls.Add(this.pnlReport);
            this.pnlBody.Controls.Add(this.pnlLower);
            this.pnlBody.Controls.Add(this.pnlModes);
            this.pnlBody.Controls.Add(this.lblModesHeader);
            //
            // ImageLabPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Name = "ImageLabPage";
            this.Size = new System.Drawing.Size(1010, 640);
            this.Text = "IconDesk — ImageLab";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.appTitleBar);
            this.ResumeLayout(false);
        }

        #endregion

        private IconDesk.AppTitleBar appTitleBar;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Label lblModesHeader;
        private Wisej.Web.Panel pnlModes;
        private Wisej.Web.PictureBox picNormal;
        private Wisej.Web.Label lblNormalName;
        private Wisej.Web.Label lblNormalNote;
        private Wisej.Web.PictureBox picZoom;
        private Wisej.Web.Label lblZoomName;
        private Wisej.Web.Label lblZoomNote;
        private Wisej.Web.PictureBox picCover;
        private Wisej.Web.Label lblCoverName;
        private Wisej.Web.Label lblCoverNote;
        private Wisej.Web.PictureBox picStretch;
        private Wisej.Web.Label lblStretchName;
        private Wisej.Web.Label lblStretchNote;
        private Wisej.Web.Panel pnlLower;
        private Wisej.Web.Panel pnlRemote;
        private Wisej.Web.Label lblRemoteHeader;
        private Wisej.Web.Panel pnlRemoteBox;
        private Wisej.Web.Label lblRemoteWaiting;
        private Wisej.Web.PictureBox picRemote;
        private Wisej.Web.Panel pnlVector;
        private Wisej.Web.Label lblVectorHeader;
        private Wisej.Web.Panel pnlVectorBox;
        private Wisej.Web.Label lblSvgRow;
        private Wisej.Web.PictureBox picSvg16;
        private Wisej.Web.PictureBox picSvg24;
        private Wisej.Web.PictureBox picSvg32;
        private Wisej.Web.PictureBox picSvg48;
        private Wisej.Web.Label lblLegacyRow;
        private Wisej.Web.PictureBox picLegacy16;
        private Wisej.Web.PictureBox picLegacy24;
        private Wisej.Web.PictureBox picLegacy32;
        private Wisej.Web.PictureBox picLegacy48;
        private Wisej.Web.Panel pnlReport;
        private Wisej.Web.Label lblReport;
    }
}
