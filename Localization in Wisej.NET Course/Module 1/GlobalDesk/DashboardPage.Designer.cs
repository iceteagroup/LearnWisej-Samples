namespace GlobalDesk
{
    partial class DashboardPage
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

        //
        // Nothing in this file carries a user-visible English sentence. Every caption is assigned
        // in ApplyTextResources, from a resource key. A literal here is a string no translator
        // will ever see.
        //
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblWelcome = new Wisej.Web.Label();
            this.lblSubtitle = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.FlowLayoutPanel();
            this.btnCustomers = new Wisej.Web.Button();
            this.pnlBody = new Wisej.Web.Panel();
            this.lblPreviewHeading = new Wisej.Web.Label();
            this.layoutPreview = new Wisej.Web.TableLayoutPanel();
            this.lblDateCaption = new Wisej.Web.Label();
            this.lblDateValue = new Wisej.Web.Label();
            this.lblQuantityCaption = new Wisej.Web.Label();
            this.lblQuantityValue = new Wisej.Web.Label();
            this.lblAmountCaption = new Wisej.Web.Label();
            this.lblAmountValue = new Wisej.Web.Label();
            this.lblCultureCaption = new Wisej.Web.Label();
            this.lblCultureValue = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblWelcome
            //
            this.lblWelcome.AutoSize = false;
            this.lblWelcome.Font = new System.Drawing.Font("default", 15F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.Location = new System.Drawing.Point(20, 12);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(620, 28);
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblSubtitle.Location = new System.Drawing.Point(20, 42);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(820, 22);
            //
            // pnlHeader
            //
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(900, 72);
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.lblWelcome);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(20, 8, 20, 8);
            this.lblStatus.Size = new System.Drawing.Size(900, 34);
            //
            // btnCustomers
            //
            // AutoSize, because a caption is a different length in every language and a fixed
            // width is a clipped word waiting to happen.
            this.btnCustomers.AutoSize = true;
            this.btnCustomers.MinimumSize = new System.Drawing.Size(140, 38);
            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.TabIndex = 0;
            this.btnCustomers.Click += this.btnCustomers_Click;
            //
            // pnlActions
            //
            this.pnlActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlActions.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Padding = new Wisej.Web.Padding(14, 8, 14, 8);
            this.pnlActions.Size = new System.Drawing.Size(900, 60);
            this.pnlActions.WrapContents = true;
            this.pnlActions.Controls.Add(this.btnCustomers);
            //
            // lblPreviewHeading
            //
            this.lblPreviewHeading.AutoSize = false;
            this.lblPreviewHeading.Dock = Wisej.Web.DockStyle.Top;
            this.lblPreviewHeading.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblPreviewHeading.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblPreviewHeading.Name = "lblPreviewHeading";
            this.lblPreviewHeading.Size = new System.Drawing.Size(860, 30);
            //
            // the preview rows
            //
            SetCaption(this.lblDateCaption, "lblDateCaption");
            SetValue(this.lblDateValue, "lblDateValue");
            SetCaption(this.lblQuantityCaption, "lblQuantityCaption");
            SetValue(this.lblQuantityValue, "lblQuantityValue");
            SetCaption(this.lblAmountCaption, "lblAmountCaption");
            SetValue(this.lblAmountValue, "lblAmountValue");
            SetCaption(this.lblCultureCaption, "lblCultureCaption");
            SetValue(this.lblCultureValue, "lblCultureValue");
            //
            // layoutPreview
            //
            // A two-column table rather than fixed positions: the caption column takes what the
            // longest translation needs and the value column keeps the rest.
            this.layoutPreview.ColumnCount = 2;
            this.layoutPreview.RowCount = 4;
            this.layoutPreview.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 42F));
            this.layoutPreview.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 58F));
            for (var row = 0; row < 4; row++)
                this.layoutPreview.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 34F));
            this.layoutPreview.Dock = Wisej.Web.DockStyle.Top;
            this.layoutPreview.Name = "layoutPreview";
            this.layoutPreview.Size = new System.Drawing.Size(560, 140);
            this.layoutPreview.Controls.Add(this.lblDateCaption, 0, 0);
            this.layoutPreview.Controls.Add(this.lblDateValue, 1, 0);
            this.layoutPreview.Controls.Add(this.lblQuantityCaption, 0, 1);
            this.layoutPreview.Controls.Add(this.lblQuantityValue, 1, 1);
            this.layoutPreview.Controls.Add(this.lblAmountCaption, 0, 2);
            this.layoutPreview.Controls.Add(this.lblAmountValue, 1, 2);
            this.layoutPreview.Controls.Add(this.lblCultureCaption, 0, 3);
            this.layoutPreview.Controls.Add(this.lblCultureValue, 1, 3);
            //
            // pnlBody
            //
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new Wisej.Web.Padding(20);
            this.pnlBody.Controls.Add(this.layoutPreview);
            this.pnlBody.Controls.Add(this.lblPreviewHeading);
            //
            // DashboardPage
            //
            this.Name = "DashboardPage";
            this.Size = new System.Drawing.Size(900, 560);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }

        private static void SetCaption(Wisej.Web.Label label, string name)
        {
            label.AutoSize = false;
            label.Dock = Wisej.Web.DockStyle.Fill;
            label.Name = name;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        }

        private static void SetValue(Wisej.Web.Label label, string name)
        {
            label.AutoSize = false;
            label.Dock = Wisej.Web.DockStyle.Fill;
            label.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            label.Name = name;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblWelcome;
        private Wisej.Web.Label lblSubtitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.FlowLayoutPanel pnlActions;
        private Wisej.Web.Button btnCustomers;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Label lblPreviewHeading;
        private Wisej.Web.TableLayoutPanel layoutPreview;
        private Wisej.Web.Label lblDateCaption;
        private Wisej.Web.Label lblDateValue;
        private Wisej.Web.Label lblQuantityCaption;
        private Wisej.Web.Label lblQuantityValue;
        private Wisej.Web.Label lblAmountCaption;
        private Wisej.Web.Label lblAmountValue;
        private Wisej.Web.Label lblCultureCaption;
        private Wisej.Web.Label lblCultureValue;
    }
}
