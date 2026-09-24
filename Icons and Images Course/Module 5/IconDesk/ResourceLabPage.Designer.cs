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
            this.appTitleBar = new IconDesk.AppTitleBar();
            this.pnlBody = new Wisej.Web.Panel();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnShowResources = new Wisej.Web.Button();
            this.lblBuildAction = new Wisej.Web.Label();
            this.layoutTiles = new Wisej.Web.TableLayoutPanel();
            this.tileLogo = new IconDesk.AssetTile();
            this.tileOk = new IconDesk.AssetTile();
            this.tileWarning = new IconDesk.AssetTile();
            this.tilePhoto = new IconDesk.AssetTile();
            this.tileBadge = new IconDesk.AssetTile();
            this.lblResourceReport = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // appTitleBar
            //
            this.appTitleBar.Size = new System.Drawing.Size(1000, 36);
            this.appTitleBar.Title = "IconDesk — ResourceLab";
            //
            // btnShowResources
            //
            // The one control on this screen. Its caption names the next step, so the lab can be
            // walked through without a second button being added to the page.
            this.btnShowResources.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.btnShowResources.CssStyle = "border:none;border-radius:7px;";
            this.btnShowResources.Font = new System.Drawing.Font("default", 10.1F, System.Drawing.FontStyle.Bold);
            this.btnShowResources.ForeColor = System.Drawing.Color.White;
            this.btnShowResources.Location = new System.Drawing.Point(0, 0);
            this.btnShowResources.Name = "btnShowResources";
            this.btnShowResources.Size = new System.Drawing.Size(170, 32);
            this.btnShowResources.TabIndex = 0;
            this.btnShowResources.Text = "Show resources";
            this.btnShowResources.Click += this.btnShowResources_Click;
            //
            // lblBuildAction
            //
            this.lblBuildAction.AllowHtml = true;
            this.lblBuildAction.AutoSize = false;
            this.lblBuildAction.Font = new System.Drawing.Font("default", 9.4F);
            this.lblBuildAction.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblBuildAction.Location = new System.Drawing.Point(180, 0);
            this.lblBuildAction.Name = "lblBuildAction";
            this.lblBuildAction.Size = new System.Drawing.Size(560, 32);
            this.lblBuildAction.Text = "Build Action = <b>Embedded Resource</b> on all five files";
            this.lblBuildAction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlActions
            //
            this.pnlActions.Dock = Wisej.Web.DockStyle.Top;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(960, 46);
            this.pnlActions.Controls.Add(this.btnShowResources);
            this.pnlActions.Controls.Add(this.lblBuildAction);
            //
            // the five asset tiles
            //
            this.tileLogo.Dock = Wisej.Web.DockStyle.Fill;
            this.tileLogo.FileName = "logo.svg";
            this.tileLogo.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.tileLogo.Name = "tileLogo";
            this.tileLogo.Picture.Name = "picLogo";
            this.tileLogo.SourceShape = "resource.wx/IconDesk/…";

            this.tileOk.Dock = Wisej.Web.DockStyle.Fill;
            this.tileOk.FileName = "status-ok.svg";
            this.tileOk.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.tileOk.Name = "tileOk";
            this.tileOk.Picture.Name = "picOk";

            this.tileWarning.Dock = Wisej.Web.DockStyle.Fill;
            this.tileWarning.FileName = "status-warning.svg";
            this.tileWarning.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.tileWarning.Name = "tileWarning";
            this.tileWarning.Picture.Name = "picWarning";

            this.tilePhoto.Dock = Wisej.Web.DockStyle.Fill;
            this.tilePhoto.FileName = "photo.png";
            this.tilePhoto.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.tilePhoto.Name = "tilePhoto";
            this.tilePhoto.Picture.Name = "picPhoto";

            this.tileBadge.Dock = Wisej.Web.DockStyle.Fill;
            this.tileBadge.FileName = "badge.gif";
            this.tileBadge.Name = "tileBadge";
            this.tileBadge.Picture.Name = "picBadge";
            //
            // layoutTiles
            //
            this.layoutTiles.ColumnCount = 5;
            this.layoutTiles.RowCount = 1;
            for (var column = 0; column < 5; column++)
                this.layoutTiles.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutTiles.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 140F));
            this.layoutTiles.Dock = Wisej.Web.DockStyle.Top;
            this.layoutTiles.Name = "layoutTiles";
            this.layoutTiles.Size = new System.Drawing.Size(960, 140);
            this.layoutTiles.Controls.Add(this.tileLogo, 0, 0);
            this.layoutTiles.Controls.Add(this.tileOk, 1, 0);
            this.layoutTiles.Controls.Add(this.tileWarning, 2, 0);
            this.layoutTiles.Controls.Add(this.tilePhoto, 3, 0);
            this.layoutTiles.Controls.Add(this.tileBadge, 4, 0);
            //
            // lblResourceReport
            //
            // The status strip. A missing asset says so here rather than leaving a silent empty
            // rectangle for a customer to find first.
            this.lblResourceReport.AllowHtml = true;
            this.lblResourceReport.AutoSize = false;
            this.lblResourceReport.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.lblResourceReport.CssStyle = "border-top:1px solid #e4eaf1;";
            this.lblResourceReport.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblResourceReport.Font = new System.Drawing.Font("default", 9.4F, System.Drawing.FontStyle.Bold);
            this.lblResourceReport.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblResourceReport.Name = "lblResourceReport";
            this.lblResourceReport.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.lblResourceReport.Size = new System.Drawing.Size(1000, 30);
            this.lblResourceReport.Text = "Ready";
            this.lblResourceReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlBody
            //
            this.pnlBody.BackColor = System.Drawing.Color.White;
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new Wisej.Web.Padding(18, 16, 18, 10);
            this.pnlBody.Controls.Add(this.layoutTiles);
            this.pnlBody.Controls.Add(this.pnlActions);
            //
            // ResourceLabPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Name = "ResourceLabPage";
            this.Size = new System.Drawing.Size(1000, 640);
            this.Text = "IconDesk — ResourceLab";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.lblResourceReport);
            this.Controls.Add(this.appTitleBar);
            this.ResumeLayout(false);
        }

        #endregion

        private IconDesk.AppTitleBar appTitleBar;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnShowResources;
        private Wisej.Web.Label lblBuildAction;
        private Wisej.Web.TableLayoutPanel layoutTiles;
        private IconDesk.AssetTile tileLogo;
        private IconDesk.AssetTile tileOk;
        private IconDesk.AssetTile tileWarning;
        private IconDesk.AssetTile tilePhoto;
        private IconDesk.AssetTile tileBadge;
        private Wisej.Web.Label lblResourceReport;
    }
}
