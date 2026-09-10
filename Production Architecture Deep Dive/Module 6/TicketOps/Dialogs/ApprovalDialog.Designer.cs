namespace TicketOps.Dialogs
{
    partial class ApprovalDialog
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
            this.labelWorkOrder = new Wisej.Web.Label();
            this.labelDecision = new Wisej.Web.Label();
            this.radioApprove = new Wisej.Web.RadioButton();
            this.radioReject = new Wisej.Web.RadioButton();
            this.labelComments = new Wisej.Web.Label();
            this.labelRequired = new Wisej.Web.Label();
            this.textComments = new Wisej.Web.TextBox();
            this.labelError = new Wisej.Web.Label();
            this.buttonCancel = new Wisej.Web.Button();
            this.buttonConfirm = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // labelWorkOrder  (read-only context: the dialog displays the work order, never edits it)
            //
            this.labelWorkOrder.AutoSize = false;
            this.labelWorkOrder.Font = new System.Drawing.Font("default", 9.5F);
            this.labelWorkOrder.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelWorkOrder.Location = new System.Drawing.Point(40, 18);
            this.labelWorkOrder.Name = "labelWorkOrder";
            this.labelWorkOrder.Size = new System.Drawing.Size(520, 22);
            this.labelWorkOrder.Text = "WO-0000 · (no work order)";
            //
            // labelDecision
            //
            this.labelDecision.AutoSize = false;
            this.labelDecision.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelDecision.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelDecision.Location = new System.Drawing.Point(40, 52);
            this.labelDecision.Name = "labelDecision";
            this.labelDecision.Size = new System.Drawing.Size(200, 20);
            this.labelDecision.Text = "DECISION";
            //
            // radioApprove / radioReject  (one decision: approve or reject — nothing else)
            //
            this.radioApprove.Checked = true;
            this.radioApprove.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.radioApprove.Location = new System.Drawing.Point(40, 78);
            this.radioApprove.Name = "radioApprove";
            this.radioApprove.Size = new System.Drawing.Size(150, 30);
            this.radioApprove.Text = "Approve";
            this.radioApprove.CheckedChanged += new System.EventHandler(this.radioAction_CheckedChanged);
            this.radioReject.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.radioReject.Location = new System.Drawing.Point(210, 78);
            this.radioReject.Name = "radioReject";
            this.radioReject.Size = new System.Drawing.Size(150, 30);
            this.radioReject.Text = "Reject";
            this.radioReject.CheckedChanged += new System.EventHandler(this.radioAction_CheckedChanged);
            //
            // labelComments + labelRequired ("required when rejecting")
            //
            this.labelComments.AutoSize = false;
            this.labelComments.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelComments.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelComments.Location = new System.Drawing.Point(40, 124);
            this.labelComments.Name = "labelComments";
            this.labelComments.Size = new System.Drawing.Size(110, 20);
            this.labelComments.Text = "COMMENTS";
            this.labelRequired.AutoSize = false;
            this.labelRequired.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelRequired.ForeColor = System.Drawing.Color.FromArgb(185, 119, 14);
            this.labelRequired.Location = new System.Drawing.Point(150, 124);
            this.labelRequired.Name = "labelRequired";
            this.labelRequired.Size = new System.Drawing.Size(300, 20);
            this.labelRequired.Text = "required when rejecting";
            this.labelRequired.Visible = false;
            //
            // textComments
            //
            this.textComments.Location = new System.Drawing.Point(40, 148);
            this.textComments.Multiline = true;
            this.textComments.Name = "textComments";
            this.textComments.Size = new System.Drawing.Size(520, 96);
            this.textComments.Watermark = "Add context for this decision…";
            //
            // labelError  (validation message — the dialog stays open while it shows)
            //
            this.labelError.AutoSize = false;
            this.labelError.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelError.ForeColor = System.Drawing.Color.FromArgb(207, 63, 63);
            this.labelError.Location = new System.Drawing.Point(40, 250);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(520, 24);
            this.labelError.Text = "";
            this.labelError.Visible = false;
            //
            // buttonCancel / buttonConfirm  (explicit exits; the ✕ in the title bar is the third)
            //
            this.buttonCancel.Location = new System.Drawing.Point(326, 296);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(110, 40);
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.ToolTipText = "Stops the workflow: Result stays unconfirmed, the caller does nothing";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            this.buttonConfirm.Location = new System.Drawing.Point(450, 296);
            this.buttonConfirm.Name = "buttonConfirm";
            this.buttonConfirm.Size = new System.Drawing.Size(110, 40);
            this.buttonConfirm.Text = "Confirm";
            this.buttonConfirm.ToolTipText = "Validates inside the dialog, then builds the typed Result and closes with DialogResult.OK";
            this.buttonConfirm.Click += new System.EventHandler(this.buttonConfirm_Click);
            //
            // ApprovalDialog
            //
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(600, 356);
            this.Controls.Add(this.labelWorkOrder);
            this.Controls.Add(this.labelDecision);
            this.Controls.Add(this.radioApprove);
            this.Controls.Add(this.radioReject);
            this.Controls.Add(this.labelComments);
            this.Controls.Add(this.labelRequired);
            this.Controls.Add(this.textComments);
            this.Controls.Add(this.labelError);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonConfirm);
            this.FormBorderStyle = Wisej.Web.FormBorderStyle.Fixed;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApprovalDialog";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Approve Work Order";
            this.FormClosed += new Wisej.Web.FormClosedEventHandler(this.ApprovalDialog_FormClosed);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label labelWorkOrder;
        private Wisej.Web.Label labelDecision;
        private Wisej.Web.RadioButton radioApprove;
        private Wisej.Web.RadioButton radioReject;
        private Wisej.Web.Label labelComments;
        private Wisej.Web.Label labelRequired;
        private Wisej.Web.TextBox textComments;
        private Wisej.Web.Label labelError;
        private Wisej.Web.Button buttonCancel;
        private Wisej.Web.Button buttonConfirm;
    }
}
