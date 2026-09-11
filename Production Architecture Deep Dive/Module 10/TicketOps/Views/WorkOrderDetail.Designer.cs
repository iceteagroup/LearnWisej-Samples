namespace TicketOps.Views
{
    partial class WorkOrderDetail
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.labelTitle = new Wisej.Web.Label();
            this.chipStatus = new TicketOps.Controls.StatusChip();
            this.labelCreatedCaption = new Wisej.Web.Label();
            this.labelCreatedValue = new Wisej.Web.Label();
            this.labelDueCaption = new Wisej.Web.Label();
            this.labelDueValue = new Wisej.Web.Label();
            this.labelCostCaption = new Wisej.Web.Label();
            this.labelCostValue = new Wisej.Web.Label();
            this.labelHoursCaption = new Wisej.Web.Label();
            this.labelHoursValue = new Wisej.Web.Label();
            this.buttonNextStatus = new Wisej.Web.Button();
            this.buttonClose = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // header: title (data) + the same StatusChip as the dashboard
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 20);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(330, 30);
            this.labelTitle.Text = "";
            this.chipStatus.Location = new System.Drawing.Point(370, 22);
            this.chipStatus.Name = "chipStatus";
            this.chipStatus.Size = new System.Drawing.Size(132, 26);
            //
            // fields (captions from Resources: Detail.*; values formatted with the session's culture)
            //
            this.labelCreatedCaption.AutoSize = false;
            this.labelCreatedCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelCreatedCaption.Location = new System.Drawing.Point(24, 72);
            this.labelCreatedCaption.Name = "labelCreatedCaption";
            this.labelCreatedCaption.Size = new System.Drawing.Size(170, 22);
            this.labelCreatedCaption.Text = "Created";
            this.labelCreatedValue.AutoSize = false;
            this.labelCreatedValue.Location = new System.Drawing.Point(210, 72);
            this.labelCreatedValue.Name = "labelCreatedValue";
            this.labelCreatedValue.Size = new System.Drawing.Size(300, 22);
            this.labelCreatedValue.Text = "";
            this.labelDueCaption.AutoSize = false;
            this.labelDueCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelDueCaption.Location = new System.Drawing.Point(24, 102);
            this.labelDueCaption.Name = "labelDueCaption";
            this.labelDueCaption.Size = new System.Drawing.Size(170, 22);
            this.labelDueCaption.Text = "Due";
            this.labelDueValue.AutoSize = false;
            this.labelDueValue.Location = new System.Drawing.Point(210, 102);
            this.labelDueValue.Name = "labelDueValue";
            this.labelDueValue.Size = new System.Drawing.Size(300, 22);
            this.labelDueValue.Text = "";
            this.labelCostCaption.AutoSize = false;
            this.labelCostCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelCostCaption.Location = new System.Drawing.Point(24, 132);
            this.labelCostCaption.Name = "labelCostCaption";
            this.labelCostCaption.Size = new System.Drawing.Size(170, 22);
            this.labelCostCaption.Text = "Labor cost";
            this.labelCostValue.AutoSize = false;
            this.labelCostValue.Location = new System.Drawing.Point(210, 132);
            this.labelCostValue.Name = "labelCostValue";
            this.labelCostValue.Size = new System.Drawing.Size(300, 22);
            this.labelCostValue.Text = "";
            this.labelHoursCaption.AutoSize = false;
            this.labelHoursCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelHoursCaption.Location = new System.Drawing.Point(24, 162);
            this.labelHoursCaption.Name = "labelHoursCaption";
            this.labelHoursCaption.Size = new System.Drawing.Size(170, 22);
            this.labelHoursCaption.Text = "Hours";
            this.labelHoursValue.AutoSize = false;
            this.labelHoursValue.Location = new System.Drawing.Point(210, 162);
            this.labelHoursValue.Name = "labelHoursValue";
            this.labelHoursValue.Size = new System.Drawing.Size(300, 22);
            this.labelHoursValue.Text = "";
            //
            // buttons
            //
            this.buttonNextStatus.Location = new System.Drawing.Point(24, 204);
            this.buttonNextStatus.Name = "buttonNextStatus";
            this.buttonNextStatus.Size = new System.Drawing.Size(170, 36);
            this.buttonNextStatus.Text = "Next status";
            this.buttonNextStatus.Click += new System.EventHandler(this.buttonNextStatus_Click);
            this.buttonClose.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClose.Location = new System.Drawing.Point(370, 204);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(140, 36);
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            //
            // WorkOrderDetail  (NO BackColor: the theme paints the dialog)
            //
            this.ClientSize = new System.Drawing.Size(534, 264);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.chipStatus);
            this.Controls.Add(this.labelCreatedCaption);
            this.Controls.Add(this.labelCreatedValue);
            this.Controls.Add(this.labelDueCaption);
            this.Controls.Add(this.labelDueValue);
            this.Controls.Add(this.labelCostCaption);
            this.Controls.Add(this.labelCostValue);
            this.Controls.Add(this.labelHoursCaption);
            this.Controls.Add(this.labelHoursValue);
            this.Controls.Add(this.buttonNextStatus);
            this.Controls.Add(this.buttonClose);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WorkOrderDetail";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Work order";
            this.Load += new System.EventHandler(this.WorkOrderDetail_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label labelTitle;
        private TicketOps.Controls.StatusChip chipStatus;
        private Wisej.Web.Label labelCreatedCaption;
        private Wisej.Web.Label labelCreatedValue;
        private Wisej.Web.Label labelDueCaption;
        private Wisej.Web.Label labelDueValue;
        private Wisej.Web.Label labelCostCaption;
        private Wisej.Web.Label labelCostValue;
        private Wisej.Web.Label labelHoursCaption;
        private Wisej.Web.Label labelHoursValue;
        private Wisej.Web.Button buttonNextStatus;
        private Wisej.Web.Button buttonClose;
    }
}
