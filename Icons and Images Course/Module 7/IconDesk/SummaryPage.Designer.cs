namespace IconDesk
{
    partial class SummaryPage
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
            this.cboTheme = new Wisej.Web.ComboBox();
            this.btnQa = new Wisej.Web.Button();
            this.pnlBody = new Wisej.Web.Panel();
            this.lblCardHeader = new Wisej.Web.Label();
            this.lblToolbar = new Wisej.Web.Label();
            this.lblMechanismsHeader = new Wisej.Web.Label();
            this.layoutMechanisms = new Wisej.Web.TableLayoutPanel();
            this.picTheme = new Wisej.Web.PictureBox();
            this.lblTheme = new Wisej.Web.Label();
            this.picOfficial = new Wisej.Web.PictureBox();
            this.lblOfficial = new Wisej.Web.Label();
            this.picCustom = new Wisej.Web.PictureBox();
            this.lblCustom = new Wisej.Web.Label();
            this.picEmbedded = new Wisej.Web.PictureBox();
            this.lblEmbedded = new Wisej.Web.Label();
            this.picRecoloured = new Wisej.Web.PictureBox();
            this.lblRecoloured = new Wisej.Web.Label();
            this.lblGridHeader = new Wisej.Web.Label();
            this.gridAssets = new Wisej.Web.DataGridView();
            this.colAsset = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colKind = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colActions = new Wisej.Web.DataGridViewTextBoxColumn();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(560, 28);
            this.lblTitle.Text = "Every mechanism, on one page";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblSubtitle.Location = new System.Drawing.Point(20, 42);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(900, 22);
            this.lblSubtitle.Text = "Theme image, official pack, custom pack, embedded asset, recoloured SVG - and icon fonts.";
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

            this.cboTheme.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboTheme.Name = "cboTheme";
            this.cboTheme.Size = new System.Drawing.Size(200, 38);
            this.cboTheme.TabIndex = 1;
            this.cboTheme.SelectedIndexChanged += this.cboTheme_SelectedIndexChanged;

            this.btnQa.Name = "btnQa";
            this.btnQa.Size = new System.Drawing.Size(240, 38);
            this.btnQa.TabIndex = 2;
            this.btnQa.Text = "Run the theme QA pass";
            this.btnQa.Click += this.btnQa_Click;
            //
            // pnlActions
            //
            this.pnlActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlActions.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Padding = new Wisej.Web.Padding(14, 8, 14, 8);
            this.pnlActions.Size = new System.Drawing.Size(1000, 60);
            this.pnlActions.WrapContents = true;
            this.pnlActions.Controls.Add(this.btnBack);
            this.pnlActions.Controls.Add(this.cboTheme);
            this.pnlActions.Controls.Add(this.btnQa);
            //
            // lblCardHeader
            //
            this.lblCardHeader.AutoSize = false;
            this.lblCardHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblCardHeader.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblCardHeader.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblCardHeader.Name = "lblCardHeader";
            this.lblCardHeader.Size = new System.Drawing.Size(960, 26);
            this.lblCardHeader.Text = "An icon-font toolbar - one AllowHtml label, no images at all";
            //
            // lblToolbar - the icon-font card. Its content is set in code.
            //
            this.lblToolbar.AllowHtml = true;
            this.lblToolbar.AutoSize = false;
            this.lblToolbar.Dock = Wisej.Web.DockStyle.Top;
            this.lblToolbar.Name = "lblToolbar";
            this.lblToolbar.Size = new System.Drawing.Size(960, 46);
            this.lblToolbar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMechanismsHeader
            //
            this.lblMechanismsHeader.AutoSize = false;
            this.lblMechanismsHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblMechanismsHeader.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblMechanismsHeader.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblMechanismsHeader.Name = "lblMechanismsHeader";
            this.lblMechanismsHeader.Padding = new Wisej.Web.Padding(0, 12, 0, 0);
            this.lblMechanismsHeader.Size = new System.Drawing.Size(960, 38);
            this.lblMechanismsHeader.Text = "The five image-source mechanisms the course covered";
            //
            // the five mechanisms
            //
            SetCell(this.picTheme, "picTheme", this.lblTheme, "lblTheme", "Theme image");
            SetCell(this.picOfficial, "picOfficial", this.lblOfficial, "lblOfficial", "Official pack");
            SetCell(this.picCustom, "picCustom", this.lblCustom, "lblCustom", "Custom pack");
            SetCell(this.picEmbedded, "picEmbedded", this.lblEmbedded, "lblEmbedded", "Embedded asset");
            SetCell(this.picRecoloured, "picRecoloured", this.lblRecoloured, "lblRecoloured", "Recoloured SVG");
            //
            // layoutMechanisms
            //
            this.layoutMechanisms.ColumnCount = 5;
            this.layoutMechanisms.RowCount = 2;
            for (var column = 0; column < 5; column++)
                this.layoutMechanisms.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutMechanisms.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 54F));
            this.layoutMechanisms.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 26F));
            this.layoutMechanisms.Dock = Wisej.Web.DockStyle.Top;
            this.layoutMechanisms.Name = "layoutMechanisms";
            this.layoutMechanisms.Size = new System.Drawing.Size(960, 84);
            this.layoutMechanisms.Controls.Add(this.picTheme, 0, 0);
            this.layoutMechanisms.Controls.Add(this.picOfficial, 1, 0);
            this.layoutMechanisms.Controls.Add(this.picCustom, 2, 0);
            this.layoutMechanisms.Controls.Add(this.picEmbedded, 3, 0);
            this.layoutMechanisms.Controls.Add(this.picRecoloured, 4, 0);
            this.layoutMechanisms.Controls.Add(this.lblTheme, 0, 1);
            this.layoutMechanisms.Controls.Add(this.lblOfficial, 1, 1);
            this.layoutMechanisms.Controls.Add(this.lblCustom, 2, 1);
            this.layoutMechanisms.Controls.Add(this.lblEmbedded, 3, 1);
            this.layoutMechanisms.Controls.Add(this.lblRecoloured, 4, 1);
            //
            // lblGridHeader
            //
            this.lblGridHeader.AutoSize = false;
            this.lblGridHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblGridHeader.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblGridHeader.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblGridHeader.Name = "lblGridHeader";
            this.lblGridHeader.Padding = new Wisej.Web.Padding(0, 12, 0, 0);
            this.lblGridHeader.Size = new System.Drawing.Size(960, 38);
            this.lblGridHeader.Text = "Icon-font actions in a grid column - click a glyph, not the row";
            //
            // the grid
            //
            this.colAsset.DataPropertyName = "Asset";
            this.colAsset.HeaderText = "Asset";
            this.colAsset.Name = "colAsset";
            this.colAsset.Width = 220;

            this.colKind.DataPropertyName = "Kind";
            this.colKind.HeaderText = "Mechanism";
            this.colKind.Name = "colKind";
            this.colKind.Width = 300;

            // AllowHtml on the column is what lets the cell value be markup rather than text.
            this.colActions.AllowHtml = true;
            this.colActions.DataPropertyName = "Actions";
            this.colActions.HeaderText = "Actions";
            this.colActions.Name = "colActions";
            this.colActions.Width = 160;

            this.gridAssets.Dock = Wisej.Web.DockStyle.Fill;
            this.gridAssets.Name = "gridAssets";
            this.gridAssets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridAssets.Columns.Add(this.colAsset);
            this.gridAssets.Columns.Add(this.colKind);
            this.gridAssets.Columns.Add(this.colActions);
            this.gridAssets.CellClick += this.gridAssets_CellClick;
            //
            // pnlBody
            //
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new Wisej.Web.Padding(20);
            this.pnlBody.Controls.Add(this.gridAssets);
            this.pnlBody.Controls.Add(this.lblGridHeader);
            this.pnlBody.Controls.Add(this.layoutMechanisms);
            this.pnlBody.Controls.Add(this.lblMechanismsHeader);
            this.pnlBody.Controls.Add(this.lblToolbar);
            this.pnlBody.Controls.Add(this.lblCardHeader);
            //
            // SummaryPage
            //
            this.Name = "SummaryPage";
            this.Size = new System.Drawing.Size(1000, 680);
            this.Text = "IconDesk - Summary";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }

        private static void SetCell(Wisej.Web.PictureBox box, string boxName, Wisej.Web.Label label, string labelName, string caption)
        {
            box.Name = boxName;
            box.Size = new System.Drawing.Size(48, 48);
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
        private Wisej.Web.ComboBox cboTheme;
        private Wisej.Web.Button btnQa;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Label lblCardHeader;
        private Wisej.Web.Label lblToolbar;
        private Wisej.Web.Label lblMechanismsHeader;
        private Wisej.Web.TableLayoutPanel layoutMechanisms;
        private Wisej.Web.PictureBox picTheme;
        private Wisej.Web.Label lblTheme;
        private Wisej.Web.PictureBox picOfficial;
        private Wisej.Web.Label lblOfficial;
        private Wisej.Web.PictureBox picCustom;
        private Wisej.Web.Label lblCustom;
        private Wisej.Web.PictureBox picEmbedded;
        private Wisej.Web.Label lblEmbedded;
        private Wisej.Web.PictureBox picRecoloured;
        private Wisej.Web.Label lblRecoloured;
        private Wisej.Web.Label lblGridHeader;
        private Wisej.Web.DataGridView gridAssets;
        private Wisej.Web.DataGridViewTextBoxColumn colAsset;
        private Wisej.Web.DataGridViewTextBoxColumn colKind;
        private Wisej.Web.DataGridViewTextBoxColumn colActions;
    }
}
