namespace IconDesk
{
    partial class IconComparePage
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
            this.btnRecolour = new Wisej.Web.Button();
            this.btnReport = new Wisej.Web.Button();
            this.pnlBody = new Wisej.Web.Panel();
            this.lblCompareHeader = new Wisej.Web.Label();
            this.layoutCompare = new Wisej.Web.TableLayoutPanel();
            this.lblCorner = new Wisej.Web.Label();
            this.lblConceptSave = new Wisej.Web.Label();
            this.lblConceptDelete = new Wisej.Web.Label();
            this.lblConceptSearch = new Wisej.Web.Label();
            this.lblConceptUser = new Wisej.Web.Label();
            this.lblConceptSettings = new Wisej.Web.Label();
            this.lblFamilyFa = new Wisej.Web.Label();
            this.picFaSave = new Wisej.Web.PictureBox();
            this.picFaDelete = new Wisej.Web.PictureBox();
            this.picFaSearch = new Wisej.Web.PictureBox();
            this.picFaUser = new Wisej.Web.PictureBox();
            this.picFaSettings = new Wisej.Web.PictureBox();
            this.lblFamilyMd = new Wisej.Web.Label();
            this.picMdSave = new Wisej.Web.PictureBox();
            this.picMdDelete = new Wisej.Web.PictureBox();
            this.picMdSearch = new Wisej.Web.PictureBox();
            this.picMdUser = new Wisej.Web.PictureBox();
            this.picMdSettings = new Wisej.Web.PictureBox();
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
            this.lblTitle.Text = "Icon packs, compared";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblSubtitle.Location = new System.Drawing.Point(20, 42);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(900, 22);
            this.lblSubtitle.Text = "The same five concepts drawn by FontAwesome and by Material Design.";
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
            // btnRecolour
            //
            this.btnRecolour.Location = new System.Drawing.Point(202, 9);
            this.btnRecolour.Name = "btnRecolour";
            this.btnRecolour.Size = new System.Drawing.Size(250, 38);
            this.btnRecolour.TabIndex = 1;
            this.btnRecolour.Text = "Recolour the two Delete icons";
            this.btnRecolour.Click += this.btnRecolour_Click;
            //
            // btnReport
            //
            this.btnReport.Location = new System.Drawing.Point(464, 9);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(250, 38);
            this.btnReport.TabIndex = 2;
            this.btnReport.Text = "Report the pack resource URLs";
            this.btnReport.Click += this.btnReport_Click;
            //
            // pnlActions
            //
            this.pnlActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1000, 56);
            this.pnlActions.Controls.Add(this.btnReport);
            this.pnlActions.Controls.Add(this.btnRecolour);
            this.pnlActions.Controls.Add(this.btnBack);
            //
            // lblCompareHeader
            //
            this.lblCompareHeader.AutoSize = false;
            this.lblCompareHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblCompareHeader.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblCompareHeader.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblCompareHeader.Name = "lblCompareHeader";
            this.lblCompareHeader.Size = new System.Drawing.Size(960, 26);
            this.lblCompareHeader.Text = "Five concepts, two families, one row each";
            //
            // the concept headings
            //
            this.lblCorner.AutoSize = false;
            this.lblCorner.Name = "lblCorner";
            this.lblCorner.Text = "";
            SetHeading(this.lblConceptSave, "lblConceptSave", "Save");
            SetHeading(this.lblConceptDelete, "lblConceptDelete", "Delete");
            SetHeading(this.lblConceptSearch, "lblConceptSearch", "Search");
            SetHeading(this.lblConceptUser, "lblConceptUser", "User");
            SetHeading(this.lblConceptSettings, "lblConceptSettings", "Settings");
            //
            // the family headings
            //
            SetHeading(this.lblFamilyFa, "lblFamilyFa", "FontAwesome");
            SetHeading(this.lblFamilyMd, "lblFamilyMd", "Material Design");
            //
            // the ten icons. The five FontAwesome sources below are exactly what the Visual Studio
            // image explorer writes when you pick an icon out of an installed pack: a resource.wx
            // path into the pack assembly, with no file copied into the project.
            //
            this.picFaSave.ImageSource = "resource.wx/Wisej.Ext.FontAwesome/floppy-o.svg";
            SetIcon(this.picFaSave, "picFaSave");
            this.picFaDelete.ImageSource = "resource.wx/Wisej.Ext.FontAwesome/trash.svg";
            SetIcon(this.picFaDelete, "picFaDelete");
            this.picFaSearch.ImageSource = "resource.wx/Wisej.Ext.FontAwesome/search.svg";
            SetIcon(this.picFaSearch, "picFaSearch");
            this.picFaUser.ImageSource = "resource.wx/Wisej.Ext.FontAwesome/user.svg";
            SetIcon(this.picFaUser, "picFaUser");
            this.picFaSettings.ImageSource = "resource.wx/Wisej.Ext.FontAwesome/cog.svg";
            SetIcon(this.picFaSettings, "picFaSettings");
            //
            // The Material Design row is assigned from the pack catalog in IconComparePage.cs
            // instead, so the lab has both routes side by side.
            //
            SetIcon(this.picMdSave, "picMdSave");
            SetIcon(this.picMdDelete, "picMdDelete");
            SetIcon(this.picMdSearch, "picMdSearch");
            SetIcon(this.picMdUser, "picMdUser");
            SetIcon(this.picMdSettings, "picMdSettings");
            //
            // layoutCompare
            //
            this.layoutCompare.ColumnCount = 6;
            this.layoutCompare.RowCount = 3;
            this.layoutCompare.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Absolute, 150F));
            for (var column = 0; column < 5; column++)
                this.layoutCompare.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutCompare.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 34F));
            this.layoutCompare.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 64F));
            this.layoutCompare.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 64F));
            this.layoutCompare.Dock = Wisej.Web.DockStyle.Top;
            this.layoutCompare.Name = "layoutCompare";
            this.layoutCompare.Size = new System.Drawing.Size(960, 170);
            this.layoutCompare.Controls.Add(this.lblCorner, 0, 0);
            this.layoutCompare.Controls.Add(this.lblConceptSave, 1, 0);
            this.layoutCompare.Controls.Add(this.lblConceptDelete, 2, 0);
            this.layoutCompare.Controls.Add(this.lblConceptSearch, 3, 0);
            this.layoutCompare.Controls.Add(this.lblConceptUser, 4, 0);
            this.layoutCompare.Controls.Add(this.lblConceptSettings, 5, 0);
            this.layoutCompare.Controls.Add(this.lblFamilyFa, 0, 1);
            this.layoutCompare.Controls.Add(this.picFaSave, 1, 1);
            this.layoutCompare.Controls.Add(this.picFaDelete, 2, 1);
            this.layoutCompare.Controls.Add(this.picFaSearch, 3, 1);
            this.layoutCompare.Controls.Add(this.picFaUser, 4, 1);
            this.layoutCompare.Controls.Add(this.picFaSettings, 5, 1);
            this.layoutCompare.Controls.Add(this.lblFamilyMd, 0, 2);
            this.layoutCompare.Controls.Add(this.picMdSave, 1, 2);
            this.layoutCompare.Controls.Add(this.picMdDelete, 2, 2);
            this.layoutCompare.Controls.Add(this.picMdSearch, 3, 2);
            this.layoutCompare.Controls.Add(this.picMdUser, 4, 2);
            this.layoutCompare.Controls.Add(this.picMdSettings, 5, 2);
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
            this.pnlBody.Controls.Add(this.layoutCompare);
            this.pnlBody.Controls.Add(this.lblCompareHeader);
            //
            // IconComparePage
            //
            this.Name = "IconComparePage";
            this.Size = new System.Drawing.Size(1000, 640);
            this.Text = "IconDesk - Icon packs";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }

        private static void SetHeading(Wisej.Web.Label label, string name, string text)
        {
            label.AutoSize = false;
            label.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            label.Name = name;
            label.Text = text;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        }

        private static void SetIcon(Wisej.Web.PictureBox box, string name)
        {
            box.Name = name;
            box.Size = new System.Drawing.Size(40, 40);
            box.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblSubtitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnBack;
        private Wisej.Web.Button btnRecolour;
        private Wisej.Web.Button btnReport;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Label lblCompareHeader;
        private Wisej.Web.TableLayoutPanel layoutCompare;
        private Wisej.Web.Label lblCorner;
        private Wisej.Web.Label lblConceptSave;
        private Wisej.Web.Label lblConceptDelete;
        private Wisej.Web.Label lblConceptSearch;
        private Wisej.Web.Label lblConceptUser;
        private Wisej.Web.Label lblConceptSettings;
        private Wisej.Web.Label lblFamilyFa;
        private Wisej.Web.PictureBox picFaSave;
        private Wisej.Web.PictureBox picFaDelete;
        private Wisej.Web.PictureBox picFaSearch;
        private Wisej.Web.PictureBox picFaUser;
        private Wisej.Web.PictureBox picFaSettings;
        private Wisej.Web.Label lblFamilyMd;
        private Wisej.Web.PictureBox picMdSave;
        private Wisej.Web.PictureBox picMdDelete;
        private Wisej.Web.PictureBox picMdSearch;
        private Wisej.Web.PictureBox picMdUser;
        private Wisej.Web.PictureBox picMdSettings;
        private Wisej.Web.Label lblReport;
    }
}
