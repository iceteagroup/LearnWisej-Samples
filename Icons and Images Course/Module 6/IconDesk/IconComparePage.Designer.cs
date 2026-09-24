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

        /// <summary>Column geometry: two 240-pixel family columns pinned to the right edge.</summary>
        private const int ColumnWidth = 240;
        private const int DesignWidth = 960;
        private const int SidePadding = 26;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.appTitleBar = new IconDesk.AppTitleBar();
            this.pnlBody = new Wisej.Web.Panel();
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblColConcept = new Wisej.Web.Label();
            this.lblColFamilyOne = new Wisej.Web.Label();
            this.lblColFamilyTwo = new Wisej.Web.Label();
            this.rowSave = new Wisej.Web.Panel();
            this.lblConceptSave = new Wisej.Web.Label();
            this.picFaSave = new Wisej.Web.PictureBox();
            this.lblFaSave = new Wisej.Web.Label();
            this.picMdSave = new Wisej.Web.PictureBox();
            this.lblMdSave = new Wisej.Web.Label();
            this.rowDelete = new Wisej.Web.Panel();
            this.lblConceptDelete = new Wisej.Web.Label();
            this.picFaDelete = new Wisej.Web.PictureBox();
            this.lblFaDelete = new Wisej.Web.Label();
            this.picMdDelete = new Wisej.Web.PictureBox();
            this.lblMdDelete = new Wisej.Web.Label();
            this.rowSearch = new Wisej.Web.Panel();
            this.lblConceptSearch = new Wisej.Web.Label();
            this.picFaSearch = new Wisej.Web.PictureBox();
            this.lblFaSearch = new Wisej.Web.Label();
            this.picMdSearch = new Wisej.Web.PictureBox();
            this.lblMdSearch = new Wisej.Web.Label();
            this.rowUser = new Wisej.Web.Panel();
            this.lblConceptUser = new Wisej.Web.Label();
            this.picFaUser = new Wisej.Web.PictureBox();
            this.lblFaUser = new Wisej.Web.Label();
            this.picMdUser = new Wisej.Web.PictureBox();
            this.lblMdUser = new Wisej.Web.Label();
            this.rowSettings = new Wisej.Web.Panel();
            this.lblConceptSettings = new Wisej.Web.Label();
            this.picFaSettings = new Wisej.Web.PictureBox();
            this.lblFaSettings = new Wisej.Web.Label();
            this.picMdSettings = new Wisej.Web.PictureBox();
            this.lblMdSettings = new Wisej.Web.Label();
            this.pnlFooter = new Wisej.Web.Panel();
            this.btnCompare = new Wisej.Web.Button();
            this.lblFooterNote = new Wisej.Web.Label();
            this.lblRecommendation = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // appTitleBar
            //
            this.appTitleBar.Title = "IconDesk — IconCompare";
            //
            // pnlHeader - the column headings
            //
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.pnlHeader.CssStyle = "border-bottom:1px solid #e0e7ef;";
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(DesignWidth, 54);
            BuildHeading(this.lblColConcept, "lblColConcept", "CONCEPT", SidePadding, 400);
            BuildHeading(this.lblColFamilyOne, "lblColFamilyOne", "WISEJ-4-FONTAWESOME", DesignWidth - SidePadding - 2 * ColumnWidth, ColumnWidth);
            BuildHeading(this.lblColFamilyTwo, "lblColFamilyTwo", "WISEJ-4-MATERIALDESIGN", DesignWidth - SidePadding - ColumnWidth, ColumnWidth);
            this.pnlHeader.Controls.Add(this.lblColConcept);
            this.pnlHeader.Controls.Add(this.lblColFamilyOne);
            this.pnlHeader.Controls.Add(this.lblColFamilyTwo);
            //
            // the five concept rows
            //
            BuildRow(this.rowSave, "rowSave", 0, this.lblConceptSave, "lblConceptSave", "Save",
                this.picFaSave, "picFaSave", this.lblFaSave, "lblFaSave",
                this.picMdSave, "picMdSave", this.lblMdSave, "lblMdSave");
            BuildRow(this.rowDelete, "rowDelete", 1, this.lblConceptDelete, "lblConceptDelete", "Delete",
                this.picFaDelete, "picFaDelete", this.lblFaDelete, "lblFaDelete",
                this.picMdDelete, "picMdDelete", this.lblMdDelete, "lblMdDelete");
            BuildRow(this.rowSearch, "rowSearch", 2, this.lblConceptSearch, "lblConceptSearch", "Search",
                this.picFaSearch, "picFaSearch", this.lblFaSearch, "lblFaSearch",
                this.picMdSearch, "picMdSearch", this.lblMdSearch, "lblMdSearch");
            BuildRow(this.rowUser, "rowUser", 3, this.lblConceptUser, "lblConceptUser", "User",
                this.picFaUser, "picFaUser", this.lblFaUser, "lblFaUser",
                this.picMdUser, "picMdUser", this.lblMdUser, "lblMdUser");
            BuildRow(this.rowSettings, "rowSettings", 4, this.lblConceptSettings, "lblConceptSettings", "Settings",
                this.picFaSettings, "picFaSettings", this.lblFaSettings, "lblFaSettings",
                this.picMdSettings, "picMdSettings", this.lblMdSettings, "lblMdSettings");
            //
            // The five FontAwesome sources are exactly what the Visual Studio image explorer
            // writes when you pick an icon out of an installed pack: a resource.wx path into the
            // pack assembly, with no file copied into the project. The Material Design column is
            // assigned from the pack catalog in IconComparePage.cs instead, so the lab has both
            // routes side by side.
            //
            this.picFaSave.ImageSource = "resource.wx/Wisej.Ext.FontAwesome/floppy-o.svg";
            this.picFaDelete.ImageSource = "resource.wx/Wisej.Ext.FontAwesome/trash.svg";
            this.picFaSearch.ImageSource = "resource.wx/Wisej.Ext.FontAwesome/search.svg";
            this.picFaUser.ImageSource = "resource.wx/Wisej.Ext.FontAwesome/user.svg";
            this.picFaSettings.ImageSource = "resource.wx/Wisej.Ext.FontAwesome/cog.svg";
            //
            // pnlFooter - the count chip, which is also the control that rebuilds both columns,
            // and the sentence about the two recoloured icons.
            //
            this.btnCompare.BackColor = System.Drawing.Color.FromArgb(234, 243, 255);
            this.btnCompare.CssStyle = "border:1px solid #c5ddff;border-radius:999px;";
            this.btnCompare.Font = new System.Drawing.Font("default", 10.1F, System.Drawing.FontStyle.Bold);
            this.btnCompare.ForeColor = System.Drawing.Color.FromArgb(11, 106, 230);
            this.btnCompare.Location = new System.Drawing.Point(SidePadding, 16);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(210, 30);
            this.btnCompare.TabIndex = 0;
            this.btnCompare.Text = "10 icons · 2 packs · 1 page";
            this.btnCompare.Click += this.btnCompare_Click;

            this.lblFooterNote.AllowHtml = true;
            this.lblFooterNote.AutoSize = false;
            this.lblFooterNote.Font = new System.Drawing.Font("default", 10.1F);
            this.lblFooterNote.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblFooterNote.Location = new System.Drawing.Point(248, 16);
            this.lblFooterNote.Name = "lblFooterNote";
            this.lblFooterNote.Size = new System.Drawing.Size(700, 30);
            this.lblFooterNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.pnlFooter.Dock = Wisej.Web.DockStyle.Top;
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(DesignWidth, 62);
            this.pnlFooter.Controls.Add(this.btnCompare);
            this.pnlFooter.Controls.Add(this.lblFooterNote);
            //
            // lblRecommendation
            //
            this.lblRecommendation.AllowHtml = true;
            this.lblRecommendation.AutoSize = false;
            this.lblRecommendation.BackColor = System.Drawing.Color.FromArgb(238, 247, 241);
            this.lblRecommendation.CssStyle = "border:1px solid #bfe2cd;border-radius:9px;";
            this.lblRecommendation.Dock = Wisej.Web.DockStyle.Top;
            this.lblRecommendation.Font = new System.Drawing.Font("default", 10.5F);
            this.lblRecommendation.ForeColor = System.Drawing.Color.FromArgb(28, 107, 66);
            this.lblRecommendation.Name = "lblRecommendation";
            this.lblRecommendation.Padding = new Wisej.Web.Padding(16, 14, 16, 14);
            this.lblRecommendation.Size = new System.Drawing.Size(DesignWidth - 2 * SidePadding, 64);
            this.lblRecommendation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlBody
            //
            this.pnlBody.BackColor = System.Drawing.Color.White;
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new Wisej.Web.Padding(0, 0, 0, 20);
            this.pnlBody.Controls.Add(this.lblRecommendation);
            this.pnlBody.Controls.Add(this.pnlFooter);
            this.pnlBody.Controls.Add(this.rowSettings);
            this.pnlBody.Controls.Add(this.rowUser);
            this.pnlBody.Controls.Add(this.rowSearch);
            this.pnlBody.Controls.Add(this.rowDelete);
            this.pnlBody.Controls.Add(this.rowSave);
            this.pnlBody.Controls.Add(this.pnlHeader);
            //
            // IconComparePage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Name = "IconComparePage";
            this.Size = new System.Drawing.Size(1000, 640);
            this.Text = "IconDesk — IconCompare";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.appTitleBar);
            this.ResumeLayout(false);
        }

        /// <summary>One uppercase column heading.</summary>
        private static void BuildHeading(Wisej.Web.Label label, string name, string text, int left, int width)
        {
            label.Anchor = left > 400
                ? Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right
                : Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left;
            label.AutoSize = false;
            label.CssStyle = "letter-spacing:.04em;";
            label.Font = new System.Drawing.Font("default", 9.4F, System.Drawing.FontStyle.Bold);
            label.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            label.Location = new System.Drawing.Point(left, 0);
            label.Name = name;
            label.Size = new System.Drawing.Size(width, 54);
            label.Text = text;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        }

        /// <summary>
        /// One concept row: the caption on the left and the same concept from both families,
        /// each with the file name the pack actually served.
        /// </summary>
        private static void BuildRow(
            Wisej.Web.Panel row, string rowName, int index,
            Wisej.Web.Label concept, string conceptName, string conceptText,
            Wisej.Web.PictureBox one, string oneName, Wisej.Web.Label oneFile, string oneFileName,
            Wisej.Web.PictureBox two, string twoName, Wisej.Web.Label twoFile, string twoFileName)
        {
            row.BackColor = index % 2 == 1
                ? System.Drawing.Color.FromArgb(251, 252, 254)
                : System.Drawing.Color.White;
            row.CssStyle = "border-bottom:1px solid #f0f3f7;";
            row.Dock = Wisej.Web.DockStyle.Top;
            row.Name = rowName;
            row.Size = new System.Drawing.Size(DesignWidth, 68);

            concept.AutoSize = false;
            concept.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            concept.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            concept.Location = new System.Drawing.Point(SidePadding, 0);
            concept.Name = conceptName;
            concept.Size = new System.Drawing.Size(400, 68);
            concept.Text = conceptText;
            concept.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            var oneLeft = DesignWidth - SidePadding - 2 * ColumnWidth;
            var twoLeft = DesignWidth - SidePadding - ColumnWidth;

            BuildCell(one, oneName, oneFile, oneFileName, oneLeft);
            BuildCell(two, twoName, twoFile, twoFileName, twoLeft);

            row.Controls.Add(concept);
            row.Controls.Add(one);
            row.Controls.Add(oneFile);
            row.Controls.Add(two);
            row.Controls.Add(twoFile);
        }

        private static void BuildCell(Wisej.Web.PictureBox picture, string pictureName, Wisej.Web.Label file, string fileName, int left)
        {
            picture.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            picture.Location = new System.Drawing.Point(left, 19);
            picture.Name = pictureName;
            picture.Size = new System.Drawing.Size(30, 30);
            picture.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;

            file.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            file.AutoSize = false;
            file.Font = new System.Drawing.Font("Consolas", 9.4F);
            file.ForeColor = System.Drawing.Color.FromArgb(125, 141, 160);
            file.Location = new System.Drawing.Point(left + 44, 0);
            file.Name = fileName;
            file.Size = new System.Drawing.Size(ColumnWidth - 44, 68);
            file.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        }

        #endregion

        private IconDesk.AppTitleBar appTitleBar;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblColConcept;
        private Wisej.Web.Label lblColFamilyOne;
        private Wisej.Web.Label lblColFamilyTwo;
        private Wisej.Web.Panel rowSave;
        private Wisej.Web.Label lblConceptSave;
        private Wisej.Web.PictureBox picFaSave;
        private Wisej.Web.Label lblFaSave;
        private Wisej.Web.PictureBox picMdSave;
        private Wisej.Web.Label lblMdSave;
        private Wisej.Web.Panel rowDelete;
        private Wisej.Web.Label lblConceptDelete;
        private Wisej.Web.PictureBox picFaDelete;
        private Wisej.Web.Label lblFaDelete;
        private Wisej.Web.PictureBox picMdDelete;
        private Wisej.Web.Label lblMdDelete;
        private Wisej.Web.Panel rowSearch;
        private Wisej.Web.Label lblConceptSearch;
        private Wisej.Web.PictureBox picFaSearch;
        private Wisej.Web.Label lblFaSearch;
        private Wisej.Web.PictureBox picMdSearch;
        private Wisej.Web.Label lblMdSearch;
        private Wisej.Web.Panel rowUser;
        private Wisej.Web.Label lblConceptUser;
        private Wisej.Web.PictureBox picFaUser;
        private Wisej.Web.Label lblFaUser;
        private Wisej.Web.PictureBox picMdUser;
        private Wisej.Web.Label lblMdUser;
        private Wisej.Web.Panel rowSettings;
        private Wisej.Web.Label lblConceptSettings;
        private Wisej.Web.PictureBox picFaSettings;
        private Wisej.Web.Label lblFaSettings;
        private Wisej.Web.PictureBox picMdSettings;
        private Wisej.Web.Label lblMdSettings;
        private Wisej.Web.Panel pnlFooter;
        private Wisej.Web.Button btnCompare;
        private Wisej.Web.Label lblFooterNote;
        private Wisej.Web.Label lblRecommendation;
    }
}
