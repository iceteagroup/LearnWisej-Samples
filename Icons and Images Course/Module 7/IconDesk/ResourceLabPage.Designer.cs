namespace IconDesk
{
    partial class ResourceLabPage
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
            this.pnlActions = new Wisej.Web.FlowLayoutPanel();
            this.btnBack = new Wisej.Web.Button();
            this.btnOverride = new Wisej.Web.Button();
            this.btnRemoveOverride = new Wisej.Web.Button();
            this.btnMisspell = new Wisej.Web.Button();
            this.btnReport = new Wisej.Web.Button();
            this.pnlBody = new Wisej.Web.Panel();
            this.lblResourcesHeader = new Wisej.Web.Label();
            this.layoutResources = new Wisej.Web.TableLayoutPanel();
            this.picLogo = new Wisej.Web.PictureBox();
            this.lblLogo = new Wisej.Web.Label();
            this.picOk = new Wisej.Web.PictureBox();
            this.lblOk = new Wisej.Web.Label();
            this.picWarning = new Wisej.Web.PictureBox();
            this.lblWarning = new Wisej.Web.Label();
            this.picPhoto = new Wisej.Web.PictureBox();
            this.lblPhoto = new Wisej.Web.Label();
            this.picBadge = new Wisej.Web.PictureBox();
            this.lblBadge = new Wisej.Web.Label();
            this.lblReport = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(460, 28);
            this.lblTitle.Text = "Embedded resources";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblSubtitle.Location = new System.Drawing.Point(20, 42);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(900, 22);
            this.lblSubtitle.Text = "Five assets compiled into the assembly and served through resource.wx.";
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
            this.lblStatus.AllowHtml = true;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(20, 8, 20, 8);
            this.lblStatus.Size = new System.Drawing.Size(1000, 34);
            this.lblStatus.Text = "Ready.";
            //
            // the buttons
            //
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(170, 38);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "Back to commands";
            this.btnBack.Click += this.btnBack_Click;

            this.btnOverride.Name = "btnOverride";
            this.btnOverride.Size = new System.Drawing.Size(250, 38);
            this.btnOverride.TabIndex = 1;
            this.btnOverride.Text = "Drop an override beside the app";
            this.btnOverride.Click += this.btnOverride_Click;

            this.btnRemoveOverride.Name = "btnRemoveOverride";
            this.btnRemoveOverride.Size = new System.Drawing.Size(190, 38);
            this.btnRemoveOverride.TabIndex = 2;
            this.btnRemoveOverride.Text = "Remove the override";
            this.btnRemoveOverride.Click += this.btnRemoveOverride_Click;

            this.btnMisspell.Name = "btnMisspell";
            this.btnMisspell.Size = new System.Drawing.Size(210, 38);
            this.btnMisspell.TabIndex = 3;
            this.btnMisspell.Text = "Misspell a resource URL";
            this.btnMisspell.Click += this.btnMisspell_Click;

            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(210, 38);
            this.btnReport.TabIndex = 4;
            this.btnReport.Text = "Report the resource URLs";
            this.btnReport.Click += this.btnReport_Click;
            //
            // pnlActions
            //
            this.pnlActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlActions.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Padding = new Wisej.Web.Padding(14, 8, 14, 8);
            this.pnlActions.Size = new System.Drawing.Size(1000, 104);
            this.pnlActions.WrapContents = true;
            this.pnlActions.Controls.Add(this.btnBack);
            this.pnlActions.Controls.Add(this.btnOverride);
            this.pnlActions.Controls.Add(this.btnRemoveOverride);
            this.pnlActions.Controls.Add(this.btnMisspell);
            this.pnlActions.Controls.Add(this.btnReport);
            //
            // lblResourcesHeader
            //
            this.lblResourcesHeader.AutoSize = false;
            this.lblResourcesHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblResourcesHeader.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblResourcesHeader.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblResourcesHeader.Name = "lblResourcesHeader";
            this.lblResourcesHeader.Size = new System.Drawing.Size(960, 26);
            this.lblResourcesHeader.Text = "Five embedded assets - picLogo qualified with the assembly name, the other four not";
            //
            // the five assets
            //
            SetAsset(this.picLogo, "picLogo", this.lblLogo, "lblLogo", "logo.svg\r\nqualified");
            SetAsset(this.picOk, "picOk", this.lblOk, "lblOk", "status-ok.svg\r\nunqualified");
            SetAsset(this.picWarning, "picWarning", this.lblWarning, "lblWarning", "status-warning.svg\r\nunqualified");
            SetAsset(this.picPhoto, "picPhoto", this.lblPhoto, "lblPhoto", "photo.png\r\nunqualified");
            SetAsset(this.picBadge, "picBadge", this.lblBadge, "lblBadge", "badge.gif\r\nunqualified");
            //
            // layoutResources
            //
            this.layoutResources.ColumnCount = 5;
            this.layoutResources.RowCount = 2;
            for (var column = 0; column < 5; column++)
                this.layoutResources.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutResources.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 90F));
            this.layoutResources.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 46F));
            this.layoutResources.Dock = Wisej.Web.DockStyle.Top;
            this.layoutResources.Name = "layoutResources";
            this.layoutResources.Size = new System.Drawing.Size(960, 140);
            this.layoutResources.Controls.Add(this.picLogo, 0, 0);
            this.layoutResources.Controls.Add(this.picOk, 1, 0);
            this.layoutResources.Controls.Add(this.picWarning, 2, 0);
            this.layoutResources.Controls.Add(this.picPhoto, 3, 0);
            this.layoutResources.Controls.Add(this.picBadge, 4, 0);
            this.layoutResources.Controls.Add(this.lblLogo, 0, 1);
            this.layoutResources.Controls.Add(this.lblOk, 1, 1);
            this.layoutResources.Controls.Add(this.lblWarning, 2, 1);
            this.layoutResources.Controls.Add(this.lblPhoto, 3, 1);
            this.layoutResources.Controls.Add(this.lblBadge, 4, 1);
            //
            // lblReport
            //
            this.lblReport.AllowHtml = true;
            this.lblReport.AutoSize = false;
            this.lblReport.Dock = Wisej.Web.DockStyle.Fill;
            this.lblReport.Name = "lblReport";
            this.lblReport.Padding = new Wisej.Web.Padding(0, 14, 0, 0);
            this.lblReport.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // pnlBody
            //
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new Wisej.Web.Padding(20);
            this.pnlBody.Controls.Add(this.lblReport);
            this.pnlBody.Controls.Add(this.layoutResources);
            this.pnlBody.Controls.Add(this.lblResourcesHeader);
            //
            // ResourceLabPage
            //
            this.Name = "ResourceLabPage";
            this.Size = new System.Drawing.Size(1000, 640);
            this.Text = "IconDesk - Embedded resources";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }

        private static void SetAsset(Wisej.Web.PictureBox box, string boxName, Wisej.Web.Label label, string labelName, string caption)
        {
            box.Name = boxName;
            box.Size = new System.Drawing.Size(72, 72);
            box.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;

            label.AutoSize = false;
            label.Name = labelName;
            label.Text = caption;
            label.TextAlign = System.Drawing.ContentAlignment.TopLeft;
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblSubtitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.FlowLayoutPanel pnlActions;
        private Wisej.Web.Button btnBack;
        private Wisej.Web.Button btnOverride;
        private Wisej.Web.Button btnRemoveOverride;
        private Wisej.Web.Button btnMisspell;
        private Wisej.Web.Button btnReport;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Label lblResourcesHeader;
        private Wisej.Web.TableLayoutPanel layoutResources;
        private Wisej.Web.PictureBox picLogo;
        private Wisej.Web.Label lblLogo;
        private Wisej.Web.PictureBox picOk;
        private Wisej.Web.Label lblOk;
        private Wisej.Web.PictureBox picWarning;
        private Wisej.Web.Label lblWarning;
        private Wisej.Web.PictureBox picPhoto;
        private Wisej.Web.Label lblPhoto;
        private Wisej.Web.PictureBox picBadge;
        private Wisej.Web.Label lblBadge;
        private Wisej.Web.Label lblReport;
    }
}
