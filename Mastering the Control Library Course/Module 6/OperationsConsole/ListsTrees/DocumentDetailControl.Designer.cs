namespace OperationsConsole.ListsTrees
{
    partial class DocumentDetailControl
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
            this.pnlFields = new Wisej.Web.Panel();
            this.lblIdCaption = new Wisej.Web.Label();
            this.lblId = new Wisej.Web.Label();
            this.lblOwnerCaption = new Wisej.Web.Label();
            this.lblOwner = new Wisej.Web.Label();
            this.lblTitleCaption = new Wisej.Web.Label();
            this.lblDocTitle = new Wisej.Web.Label();
            this.lblCategoryCaption = new Wisej.Web.Label();
            this.lblCategory = new Wisej.Web.Label();
            this.lblTypeCaption = new Wisej.Web.Label();
            this.lblType = new Wisej.Web.Label();
            this.lblSizeCaption = new Wisej.Web.Label();
            this.lblSize = new Wisej.Web.Label();
            this.lblModifiedCaption = new Wisej.Web.Label();
            this.lblModified = new Wisej.Web.Label();
            this.lblStatusCaption = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.lblSummaryCaption = new Wisej.Web.Label();
            this.lblSummary = new Wisej.Web.Label();
            this.lblMessage = new Wisej.Web.Label();
            this.pnlFields.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlFields  (the document card: one Label per property of DocumentModel)
            //
            this.pnlFields.BackColor = System.Drawing.Color.White;
            this.pnlFields.Controls.Add(this.lblIdCaption);
            this.pnlFields.Controls.Add(this.lblId);
            this.pnlFields.Controls.Add(this.lblOwnerCaption);
            this.pnlFields.Controls.Add(this.lblOwner);
            this.pnlFields.Controls.Add(this.lblTitleCaption);
            this.pnlFields.Controls.Add(this.lblDocTitle);
            this.pnlFields.Controls.Add(this.lblCategoryCaption);
            this.pnlFields.Controls.Add(this.lblCategory);
            this.pnlFields.Controls.Add(this.lblTypeCaption);
            this.pnlFields.Controls.Add(this.lblType);
            this.pnlFields.Controls.Add(this.lblSizeCaption);
            this.pnlFields.Controls.Add(this.lblSize);
            this.pnlFields.Controls.Add(this.lblModifiedCaption);
            this.pnlFields.Controls.Add(this.lblModified);
            this.pnlFields.Controls.Add(this.lblStatusCaption);
            this.pnlFields.Controls.Add(this.lblStatus);
            this.pnlFields.Controls.Add(this.lblSummaryCaption);
            this.pnlFields.Controls.Add(this.lblSummary);
            this.pnlFields.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlFields.Name = "pnlFields";
            this.pnlFields.Size = new System.Drawing.Size(508, 206);
            this.pnlFields.Visible = false;
            //
            // row 1 — Document ID | Owner
            //
            this.lblIdCaption.AutoSize = false;
            this.lblIdCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblIdCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblIdCaption.Location = new System.Drawing.Point(16, 10);
            this.lblIdCaption.Name = "lblIdCaption";
            this.lblIdCaption.Size = new System.Drawing.Size(96, 22);
            this.lblIdCaption.Text = "DOCUMENT ID";
            this.lblIdCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            this.lblId.AutoSize = false;
            this.lblId.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblId.Location = new System.Drawing.Point(118, 10);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(170, 22);
            this.lblId.Text = "—";
            this.lblId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            this.lblOwnerCaption.AutoSize = false;
            this.lblOwnerCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblOwnerCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblOwnerCaption.Location = new System.Drawing.Point(300, 10);
            this.lblOwnerCaption.Name = "lblOwnerCaption";
            this.lblOwnerCaption.Size = new System.Drawing.Size(80, 22);
            this.lblOwnerCaption.Text = "OWNER";
            this.lblOwnerCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            this.lblOwner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblOwner.AutoSize = false;
            this.lblOwner.Location = new System.Drawing.Point(386, 10);
            this.lblOwner.Name = "lblOwner";
            this.lblOwner.Size = new System.Drawing.Size(106, 22);
            this.lblOwner.Text = "—";
            this.lblOwner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // row 2 — Title
            //
            this.lblTitleCaption.AutoSize = false;
            this.lblTitleCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblTitleCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTitleCaption.Location = new System.Drawing.Point(16, 36);
            this.lblTitleCaption.Name = "lblTitleCaption";
            this.lblTitleCaption.Size = new System.Drawing.Size(96, 22);
            this.lblTitleCaption.Text = "TITLE";
            this.lblTitleCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            this.lblDocTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblDocTitle.AutoSize = false;
            this.lblDocTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblDocTitle.Location = new System.Drawing.Point(118, 36);
            this.lblDocTitle.Name = "lblDocTitle";
            this.lblDocTitle.Size = new System.Drawing.Size(374, 22);
            this.lblDocTitle.Text = "—";
            this.lblDocTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // row 3 — Category path
            //
            this.lblCategoryCaption.AutoSize = false;
            this.lblCategoryCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblCategoryCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCategoryCaption.Location = new System.Drawing.Point(16, 62);
            this.lblCategoryCaption.Name = "lblCategoryCaption";
            this.lblCategoryCaption.Size = new System.Drawing.Size(96, 22);
            this.lblCategoryCaption.Text = "CATEGORY";
            this.lblCategoryCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            this.lblCategory.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblCategory.AutoSize = false;
            this.lblCategory.Location = new System.Drawing.Point(118, 62);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(374, 22);
            this.lblCategory.Text = "—";
            this.lblCategory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // row 4 — Type | Size
            //
            this.lblTypeCaption.AutoSize = false;
            this.lblTypeCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblTypeCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTypeCaption.Location = new System.Drawing.Point(16, 88);
            this.lblTypeCaption.Name = "lblTypeCaption";
            this.lblTypeCaption.Size = new System.Drawing.Size(96, 22);
            this.lblTypeCaption.Text = "TYPE";
            this.lblTypeCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            this.lblType.AutoSize = false;
            this.lblType.Location = new System.Drawing.Point(118, 88);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(170, 22);
            this.lblType.Text = "—";
            this.lblType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            this.lblSizeCaption.AutoSize = false;
            this.lblSizeCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblSizeCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSizeCaption.Location = new System.Drawing.Point(300, 88);
            this.lblSizeCaption.Name = "lblSizeCaption";
            this.lblSizeCaption.Size = new System.Drawing.Size(80, 22);
            this.lblSizeCaption.Text = "SIZE";
            this.lblSizeCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            this.lblSize.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblSize.AutoSize = false;
            this.lblSize.Location = new System.Drawing.Point(386, 88);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(106, 22);
            this.lblSize.Text = "—";
            this.lblSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // row 5 — Modified | Status
            //
            this.lblModifiedCaption.AutoSize = false;
            this.lblModifiedCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblModifiedCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblModifiedCaption.Location = new System.Drawing.Point(16, 114);
            this.lblModifiedCaption.Name = "lblModifiedCaption";
            this.lblModifiedCaption.Size = new System.Drawing.Size(96, 22);
            this.lblModifiedCaption.Text = "MODIFIED";
            this.lblModifiedCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            this.lblModified.AutoSize = false;
            this.lblModified.Location = new System.Drawing.Point(118, 114);
            this.lblModified.Name = "lblModified";
            this.lblModified.Size = new System.Drawing.Size(170, 22);
            this.lblModified.Text = "—";
            this.lblModified.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            this.lblStatusCaption.AutoSize = false;
            this.lblStatusCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblStatusCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblStatusCaption.Location = new System.Drawing.Point(300, 114);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(80, 22);
            this.lblStatusCaption.Text = "STATUS";
            this.lblStatusCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            this.lblStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(386, 114);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(106, 22);
            this.lblStatus.Text = "—";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // row 6 — Summary
            //
            this.lblSummaryCaption.AutoSize = false;
            this.lblSummaryCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblSummaryCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSummaryCaption.Location = new System.Drawing.Point(16, 142);
            this.lblSummaryCaption.Name = "lblSummaryCaption";
            this.lblSummaryCaption.Size = new System.Drawing.Size(96, 22);
            this.lblSummaryCaption.Text = "SUMMARY";
            this.lblSummaryCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            this.lblSummary.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblSummary.AutoSize = false;
            this.lblSummary.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblSummary.Location = new System.Drawing.Point(118, 142);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(374, 52);
            this.lblSummary.Text = "—";
            this.lblSummary.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // lblMessage  (the empty / loading / error state — the only other thing this control can show)
            //
            this.lblMessage.AutoSize = false;
            this.lblMessage.Dock = Wisej.Web.DockStyle.Fill;
            this.lblMessage.Font = new System.Drawing.Font("default", 10F);
            this.lblMessage.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Padding = new Wisej.Web.Padding(24, 0, 24, 0);
            this.lblMessage.Text = "Select a category on the left, then a document.";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // DocumentDetailControl
            //
            this.BackColor = System.Drawing.Color.White;
            // Exactly one of the two is Visible at any moment: the field card, or the message.
            this.Controls.Add(this.pnlFields);
            this.Controls.Add(this.lblMessage);
            this.Name = "DocumentDetailControl";
            this.Size = new System.Drawing.Size(508, 206);
            this.pnlFields.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlFields;
        private Wisej.Web.Label lblIdCaption;
        private Wisej.Web.Label lblId;
        private Wisej.Web.Label lblOwnerCaption;
        private Wisej.Web.Label lblOwner;
        private Wisej.Web.Label lblTitleCaption;
        private Wisej.Web.Label lblDocTitle;
        private Wisej.Web.Label lblCategoryCaption;
        private Wisej.Web.Label lblCategory;
        private Wisej.Web.Label lblTypeCaption;
        private Wisej.Web.Label lblType;
        private Wisej.Web.Label lblSizeCaption;
        private Wisej.Web.Label lblSize;
        private Wisej.Web.Label lblModifiedCaption;
        private Wisej.Web.Label lblModified;
        private Wisej.Web.Label lblStatusCaption;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblSummaryCaption;
        private Wisej.Web.Label lblSummary;
        private Wisej.Web.Label lblMessage;
    }
}
