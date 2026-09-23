namespace IconDesk
{
    partial class IconPackPage
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
            this.btnRecolour = new Wisej.Web.Button();
            this.btnVerify = new Wisej.Web.Button();
            this.pnlBody = new Wisej.Web.Panel();
            this.lblDesignerHeader = new Wisej.Web.Label();
            this.pnlDesigner = new Wisej.Web.FlowLayoutPanel();
            this.btnAdd = new Wisej.Web.Button();
            this.btnEdit = new Wisej.Web.Button();
            this.btnRemove = new Wisej.Web.Button();
            this.lblCatalogHeader = new Wisej.Web.Label();
            this.pnlCatalog = new Wisej.Web.FlowLayoutPanel();
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
            this.lblTitle.Text = "The project icon pack";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblSubtitle.Location = new System.Drawing.Point(20, 42);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(900, 22);
            this.lblSubtitle.Text = "IconDesk.Icons - twelve embedded SVGs and a catalog, consumed two ways.";
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

            this.btnRecolour.Name = "btnRecolour";
            this.btnRecolour.Size = new System.Drawing.Size(240, 38);
            this.btnRecolour.TabIndex = 1;
            this.btnRecolour.Text = "Recolour three catalog icons";
            this.btnRecolour.Click += this.btnRecolour_Click;

            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(280, 38);
            this.btnVerify.TabIndex = 2;
            this.btnVerify.Text = "Check the catalog against the assembly";
            this.btnVerify.Click += this.btnVerify_Click;
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
            this.pnlActions.Controls.Add(this.btnRecolour);
            this.pnlActions.Controls.Add(this.btnVerify);
            //
            // lblDesignerHeader
            //
            this.lblDesignerHeader.AutoSize = false;
            this.lblDesignerHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblDesignerHeader.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblDesignerHeader.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblDesignerHeader.Name = "lblDesignerHeader";
            this.lblDesignerHeader.Size = new System.Drawing.Size(960, 26);
            this.lblDesignerHeader.Text = "Assigned in the designer - the picker writes the pack's resource.wx path";
            //
            // three buttons whose icons the designer wrote as literal pack URLs
            //
            this.btnAdd.ImageSource = "resource.wx/IconDesk.Icons/add.svg";
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(150, 44);
            this.btnAdd.Text = "Add";
            this.btnAdd.TextImageRelation = Wisej.Web.TextImageRelation.ImageBeforeText;

            this.btnEdit.ImageSource = "resource.wx/IconDesk.Icons/edit.svg";
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(150, 44);
            this.btnEdit.Text = "Edit";
            this.btnEdit.TextImageRelation = Wisej.Web.TextImageRelation.ImageBeforeText;

            this.btnRemove.ImageSource = "resource.wx/IconDesk.Icons/delete.svg";
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(150, 44);
            this.btnRemove.Text = "Delete";
            this.btnRemove.TextImageRelation = Wisej.Web.TextImageRelation.ImageBeforeText;
            //
            // pnlDesigner
            //
            this.pnlDesigner.Dock = Wisej.Web.DockStyle.Top;
            this.pnlDesigner.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.pnlDesigner.Name = "pnlDesigner";
            this.pnlDesigner.Size = new System.Drawing.Size(960, 56);
            this.pnlDesigner.WrapContents = true;
            this.pnlDesigner.Controls.Add(this.btnAdd);
            this.pnlDesigner.Controls.Add(this.btnEdit);
            this.pnlDesigner.Controls.Add(this.btnRemove);
            //
            // lblCatalogHeader
            //
            this.lblCatalogHeader.AutoSize = false;
            this.lblCatalogHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblCatalogHeader.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblCatalogHeader.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblCatalogHeader.Name = "lblCatalogHeader";
            this.lblCatalogHeader.Padding = new Wisej.Web.Padding(0, 14, 0, 0);
            this.lblCatalogHeader.Size = new System.Drawing.Size(960, 40);
            this.lblCatalogHeader.Text = "Assigned from AppIcons in C# - the whole pack, built from AppIcons.All";
            //
            // pnlCatalog - filled in code from the catalog
            //
            this.pnlCatalog.Dock = Wisej.Web.DockStyle.Top;
            this.pnlCatalog.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.pnlCatalog.Name = "pnlCatalog";
            this.pnlCatalog.Size = new System.Drawing.Size(960, 120);
            this.pnlCatalog.WrapContents = true;
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
            this.pnlBody.Controls.Add(this.pnlCatalog);
            this.pnlBody.Controls.Add(this.lblCatalogHeader);
            this.pnlBody.Controls.Add(this.pnlDesigner);
            this.pnlBody.Controls.Add(this.lblDesignerHeader);
            //
            // IconPackPage
            //
            this.Name = "IconPackPage";
            this.Size = new System.Drawing.Size(1000, 640);
            this.Text = "IconDesk - Project icon pack";
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
        private Wisej.Web.FlowLayoutPanel pnlActions;
        private Wisej.Web.Button btnBack;
        private Wisej.Web.Button btnRecolour;
        private Wisej.Web.Button btnVerify;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Label lblDesignerHeader;
        private Wisej.Web.FlowLayoutPanel pnlDesigner;
        private Wisej.Web.Button btnAdd;
        private Wisej.Web.Button btnEdit;
        private Wisej.Web.Button btnRemove;
        private Wisej.Web.Label lblCatalogHeader;
        private Wisej.Web.FlowLayoutPanel pnlCatalog;
        private Wisej.Web.Label lblReport;
    }
}
