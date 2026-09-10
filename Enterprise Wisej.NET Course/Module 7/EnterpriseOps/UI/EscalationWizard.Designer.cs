namespace EnterpriseOps.UI
{
    partial class EscalationWizard
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
            this.components = new System.ComponentModel.Container();
            this.stepsRail = new Wisej.Web.FlowLayoutPanel();
            this.lblStepReason = new Wisej.Web.Label();
            this.lblStepAttachments = new Wisej.Web.Label();
            this.lblStepApprover = new Wisej.Web.Label();
            this.lblStepDueDate = new Wisej.Web.Label();
            this.lblStepNotifications = new Wisej.Web.Label();
            this.lblStepReview = new Wisej.Web.Label();
            this.lblDraftState = new Wisej.Web.Label();
            this.lblWorkOrder = new Wisej.Web.Label();
            this.pnlReason = new Wisej.Web.Panel();
            this.lblReasonPrompt = new Wisej.Web.Label();
            this.txtReason = new Wisej.Web.TextBox();
            this.lblReasonHint = new Wisej.Web.Label();
            this.pnlAttachments = new Wisej.Web.Panel();
            this.lblAttachmentsPrompt = new Wisej.Web.Label();
            this.btnStageFile = new Wisej.Web.Button();
            this.btnDiscardFile = new Wisej.Web.Button();
            this.lstAttachments = new Wisej.Web.ListBox();
            this.lblAttachmentsHint = new Wisej.Web.Label();
            this.pnlApprover = new Wisej.Web.Panel();
            this.lblApproverPrompt = new Wisej.Web.Label();
            this.cboApprover = new Wisej.Web.ComboBox();
            this.btnLookupApprovers = new Wisej.Web.Button();
            this.lblApproverHint = new Wisej.Web.Label();
            this.pnlDueDate = new Wisej.Web.Panel();
            this.lblDueDatePrompt = new Wisej.Web.Label();
            this.dtpDueDate = new Wisej.Web.DateTimePicker();
            this.lblDueDateHint = new Wisej.Web.Label();
            this.pnlNotifications = new Wisej.Web.Panel();
            this.lblNotificationsPrompt = new Wisej.Web.Label();
            this.chkNotifyEmail = new Wisej.Web.CheckBox();
            this.chkNotifyInApp = new Wisej.Web.CheckBox();
            this.chkNotifySms = new Wisej.Web.CheckBox();
            this.lblNotificationsHint = new Wisej.Web.Label();
            this.pnlReview = new Wisej.Web.Panel();
            this.lblReviewPrompt = new Wisej.Web.Label();
            this.dgvSummary = new Wisej.Web.DataGridView();
            this.colField = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colValue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblResultBanner = new Wisej.Web.Label();
            this.lblResultDetail = new Wisej.Web.Label();
            this.lblValidation = new Wisej.Web.Label();
            this.lblOrchestration = new Wisej.Web.Label();
            this.pnlWizardButtons = new Wisej.Web.Panel();
            this.btnCancel = new Wisej.Web.Button();
            this.lblStepCounter = new Wisej.Web.Label();
            this.btnBack = new Wisej.Web.Button();
            this.btnNext = new Wisej.Web.Button();
            this.lblWizardStatus = new Wisej.Web.Label();
            this.stepsRail.SuspendLayout();
            this.pnlReason.SuspendLayout();
            this.pnlAttachments.SuspendLayout();
            this.pnlApprover.SuspendLayout();
            this.pnlDueDate.SuspendLayout();
            this.pnlNotifications.SuspendLayout();
            this.pnlReview.SuspendLayout();
            this.pnlWizardButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // stepsRail  (the six steps, in order — the wizard's only navigation)
            //
            this.stepsRail.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.stepsRail.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.stepsRail.Controls.Add(this.lblStepReason);
            this.stepsRail.Controls.Add(this.lblStepAttachments);
            this.stepsRail.Controls.Add(this.lblStepApprover);
            this.stepsRail.Controls.Add(this.lblStepDueDate);
            this.stepsRail.Controls.Add(this.lblStepNotifications);
            this.stepsRail.Controls.Add(this.lblStepReview);
            this.stepsRail.Controls.Add(this.lblDraftState);
            this.stepsRail.FlowDirection = Wisej.Web.FlowDirection.TopDown;
            this.stepsRail.Location = new System.Drawing.Point(0, 0);
            this.stepsRail.Name = "stepsRail";
            this.stepsRail.Padding = new Wisej.Web.Padding(0, 16, 0, 0);
            this.stepsRail.Size = new System.Drawing.Size(218, 566);
            this.stepsRail.WrapContents = false;
            //
            // the six step labels — text and colour are set by RenderRail(), never by a business rule
            //
            this.lblStepReason.AutoSize = false;
            this.lblStepReason.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStepReason.Location = new System.Drawing.Point(0, 16);
            this.lblStepReason.Name = "lblStepReason";
            this.lblStepReason.Padding = new Wisej.Web.Padding(18, 0, 0, 0);
            this.lblStepReason.Size = new System.Drawing.Size(214, 34);
            this.lblStepReason.Text = "1  Reason";
            this.lblStepReason.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStepAttachments.AutoSize = false;
            this.lblStepAttachments.Font = new System.Drawing.Font("default", 10F);
            this.lblStepAttachments.Location = new System.Drawing.Point(0, 50);
            this.lblStepAttachments.Name = "lblStepAttachments";
            this.lblStepAttachments.Padding = new Wisej.Web.Padding(18, 0, 0, 0);
            this.lblStepAttachments.Size = new System.Drawing.Size(214, 34);
            this.lblStepAttachments.Text = "2  Attachments";
            this.lblStepAttachments.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStepApprover.AutoSize = false;
            this.lblStepApprover.Font = new System.Drawing.Font("default", 10F);
            this.lblStepApprover.Location = new System.Drawing.Point(0, 84);
            this.lblStepApprover.Name = "lblStepApprover";
            this.lblStepApprover.Padding = new Wisej.Web.Padding(18, 0, 0, 0);
            this.lblStepApprover.Size = new System.Drawing.Size(214, 34);
            this.lblStepApprover.Text = "3  Approver";
            this.lblStepApprover.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStepDueDate.AutoSize = false;
            this.lblStepDueDate.Font = new System.Drawing.Font("default", 10F);
            this.lblStepDueDate.Location = new System.Drawing.Point(0, 118);
            this.lblStepDueDate.Name = "lblStepDueDate";
            this.lblStepDueDate.Padding = new Wisej.Web.Padding(18, 0, 0, 0);
            this.lblStepDueDate.Size = new System.Drawing.Size(214, 34);
            this.lblStepDueDate.Text = "4  Due date";
            this.lblStepDueDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStepNotifications.AutoSize = false;
            this.lblStepNotifications.Font = new System.Drawing.Font("default", 10F);
            this.lblStepNotifications.Location = new System.Drawing.Point(0, 152);
            this.lblStepNotifications.Name = "lblStepNotifications";
            this.lblStepNotifications.Padding = new Wisej.Web.Padding(18, 0, 0, 0);
            this.lblStepNotifications.Size = new System.Drawing.Size(214, 34);
            this.lblStepNotifications.Text = "5  Notifications";
            this.lblStepNotifications.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStepReview.AutoSize = false;
            this.lblStepReview.Font = new System.Drawing.Font("default", 10F);
            this.lblStepReview.Location = new System.Drawing.Point(0, 186);
            this.lblStepReview.Name = "lblStepReview";
            this.lblStepReview.Padding = new Wisej.Web.Padding(18, 0, 0, 0);
            this.lblStepReview.Size = new System.Drawing.Size(214, 34);
            this.lblStepReview.Text = "6  Review";
            this.lblStepReview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDraftState  (what the server holds: the draft id and the saved progress)
            //
            this.lblDraftState.AutoSize = false;
            this.lblDraftState.Font = new System.Drawing.Font("monospace", 8F);
            this.lblDraftState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblDraftState.Location = new System.Drawing.Point(0, 220);
            this.lblDraftState.Name = "lblDraftState";
            this.lblDraftState.Padding = new Wisej.Web.Padding(18, 0, 8, 0);
            this.lblDraftState.Size = new System.Drawing.Size(214, 64);
            this.lblDraftState.Text = "draft —";
            //
            // lblWorkOrder  (which work order this wizard is escalating)
            //
            this.lblWorkOrder.AutoSize = false;
            this.lblWorkOrder.Font = new System.Drawing.Font("default", 9F);
            this.lblWorkOrder.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblWorkOrder.Location = new System.Drawing.Point(244, 12);
            this.lblWorkOrder.Name = "lblWorkOrder";
            this.lblWorkOrder.Size = new System.Drawing.Size(660, 24);
            this.lblWorkOrder.Text = "WO-…";
            //
            // pnlReason  (step 1 — collect only)
            //
            this.pnlReason.Controls.Add(this.lblReasonPrompt);
            this.pnlReason.Controls.Add(this.txtReason);
            this.pnlReason.Controls.Add(this.lblReasonHint);
            this.pnlReason.Location = new System.Drawing.Point(244, 44);
            this.pnlReason.Name = "pnlReason";
            this.pnlReason.Size = new System.Drawing.Size(660, 340);
            this.lblReasonPrompt.AutoSize = false;
            this.lblReasonPrompt.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblReasonPrompt.Location = new System.Drawing.Point(0, 0);
            this.lblReasonPrompt.Name = "lblReasonPrompt";
            this.lblReasonPrompt.Size = new System.Drawing.Size(660, 28);
            this.lblReasonPrompt.Text = "Why is this work order being escalated?";
            this.txtReason.Location = new System.Drawing.Point(0, 36);
            this.txtReason.MaxLength = 600;
            this.txtReason.Multiline = true;
            this.txtReason.Name = "txtReason";
            this.txtReason.Size = new System.Drawing.Size(660, 100);
            this.txtReason.TextChanged += new System.EventHandler(this.txtReason_TextChanged);
            this.lblReasonHint.AutoSize = false;
            this.lblReasonHint.Font = new System.Drawing.Font("default", 9F);
            this.lblReasonHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblReasonHint.Location = new System.Drawing.Point(0, 144);
            this.lblReasonHint.Name = "lblReasonHint";
            this.lblReasonHint.Size = new System.Drawing.Size(660, 60);
            this.lblReasonHint.Text = "0 characters. The rule lives in EscalationWorkflow.ValidateStep — this page only counts.";
            //
            // pnlAttachments  (step 2)
            //
            this.pnlAttachments.Controls.Add(this.lblAttachmentsPrompt);
            this.pnlAttachments.Controls.Add(this.btnStageFile);
            this.pnlAttachments.Controls.Add(this.btnDiscardFile);
            this.pnlAttachments.Controls.Add(this.lstAttachments);
            this.pnlAttachments.Controls.Add(this.lblAttachmentsHint);
            this.pnlAttachments.Location = new System.Drawing.Point(244, 44);
            this.pnlAttachments.Name = "pnlAttachments";
            this.pnlAttachments.Size = new System.Drawing.Size(660, 340);
            this.pnlAttachments.Visible = false;
            this.lblAttachmentsPrompt.AutoSize = false;
            this.lblAttachmentsPrompt.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblAttachmentsPrompt.Location = new System.Drawing.Point(0, 0);
            this.lblAttachmentsPrompt.Name = "lblAttachmentsPrompt";
            this.lblAttachmentsPrompt.Size = new System.Drawing.Size(660, 28);
            this.lblAttachmentsPrompt.Text = "What should the approver look at?";
            this.btnStageFile.Location = new System.Drawing.Point(0, 36);
            this.btnStageFile.Name = "btnStageFile";
            this.btnStageFile.Size = new System.Drawing.Size(200, 36);
            this.btnStageFile.Text = "Stage a file";
            this.btnStageFile.ToolTipText = "AttachmentStaging.StageNextSample(): the upload waits in staging until the workflow persists — or the wizard is cancelled.";
            this.btnStageFile.Click += new System.EventHandler(this.btnStageFile_Click);
            this.btnDiscardFile.Location = new System.Drawing.Point(210, 36);
            this.btnDiscardFile.Name = "btnDiscardFile";
            this.btnDiscardFile.Size = new System.Drawing.Size(200, 36);
            this.btnDiscardFile.Text = "Discard selected";
            this.btnDiscardFile.ToolTipText = "Removes the selected staged upload.";
            this.btnDiscardFile.Click += new System.EventHandler(this.btnDiscardFile_Click);
            this.lstAttachments.Font = new System.Drawing.Font("monospace", 9F);
            this.lstAttachments.Location = new System.Drawing.Point(0, 82);
            this.lstAttachments.Name = "lstAttachments";
            this.lstAttachments.Size = new System.Drawing.Size(660, 150);
            this.lblAttachmentsHint.AutoSize = false;
            this.lblAttachmentsHint.Font = new System.Drawing.Font("default", 9F);
            this.lblAttachmentsHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblAttachmentsHint.Location = new System.Drawing.Point(0, 240);
            this.lblAttachmentsHint.Name = "lblAttachmentsHint";
            this.lblAttachmentsHint.Size = new System.Drawing.Size(660, 60);
            this.lblAttachmentsHint.Text = "Staged uploads are not part of the escalation yet. Cancelling the wizard discards them (failure path matrix · cancel).";
            //
            // pnlApprover  (step 3 — the external directory lookup, with a timeout)
            //
            this.pnlApprover.Controls.Add(this.lblApproverPrompt);
            this.pnlApprover.Controls.Add(this.cboApprover);
            this.pnlApprover.Controls.Add(this.btnLookupApprovers);
            this.pnlApprover.Controls.Add(this.lblApproverHint);
            this.pnlApprover.Location = new System.Drawing.Point(244, 44);
            this.pnlApprover.Name = "pnlApprover";
            this.pnlApprover.Size = new System.Drawing.Size(660, 340);
            this.pnlApprover.Visible = false;
            this.lblApproverPrompt.AutoSize = false;
            this.lblApproverPrompt.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblApproverPrompt.Location = new System.Drawing.Point(0, 0);
            this.lblApproverPrompt.Name = "lblApproverPrompt";
            this.lblApproverPrompt.Size = new System.Drawing.Size(660, 28);
            this.lblApproverPrompt.Text = "Who approves this escalation?";
            this.cboApprover.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboApprover.Location = new System.Drawing.Point(0, 36);
            this.cboApprover.Name = "cboApprover";
            this.cboApprover.Size = new System.Drawing.Size(400, 36);
            this.cboApprover.SelectedIndexChanged += new System.EventHandler(this.cboApprover_SelectedIndexChanged);
            this.btnLookupApprovers.Location = new System.Drawing.Point(412, 36);
            this.btnLookupApprovers.Name = "btnLookupApprovers";
            this.btnLookupApprovers.Size = new System.Drawing.Size(248, 36);
            this.btnLookupApprovers.Text = "Look up the directory again";
            this.btnLookupApprovers.ToolTipText = "IApproverDirectory.LookupAsync with a 5 s CancellationTokenSource: an external call the wizard must be able to survive.";
            this.btnLookupApprovers.Click += new System.EventHandler(this.btnLookupApprovers_Click);
            this.lblApproverHint.AutoSize = false;
            this.lblApproverHint.Font = new System.Drawing.Font("default", 9F);
            this.lblApproverHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblApproverHint.Location = new System.Drawing.Point(0, 84);
            this.lblApproverHint.Name = "lblApproverHint";
            this.lblApproverHint.Size = new System.Drawing.Size(660, 80);
            this.lblApproverHint.Text = "The directory is an external system. Whether a listed person may approve is decided by PermissionService, not here.";
            //
            // pnlDueDate  (step 4)
            //
            this.pnlDueDate.Controls.Add(this.lblDueDatePrompt);
            this.pnlDueDate.Controls.Add(this.dtpDueDate);
            this.pnlDueDate.Controls.Add(this.lblDueDateHint);
            this.pnlDueDate.Location = new System.Drawing.Point(244, 44);
            this.pnlDueDate.Name = "pnlDueDate";
            this.pnlDueDate.Size = new System.Drawing.Size(660, 340);
            this.pnlDueDate.Visible = false;
            this.lblDueDatePrompt.AutoSize = false;
            this.lblDueDatePrompt.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblDueDatePrompt.Location = new System.Drawing.Point(0, 0);
            this.lblDueDatePrompt.Name = "lblDueDatePrompt";
            this.lblDueDatePrompt.Size = new System.Drawing.Size(660, 28);
            this.lblDueDatePrompt.Text = "When does the approver have to decide?";
            this.dtpDueDate.CustomFormat = "MMM d, yyyy  HH:mm";
            this.dtpDueDate.Format = Wisej.Web.DateTimePickerFormat.Custom;
            this.dtpDueDate.Location = new System.Drawing.Point(0, 36);
            this.dtpDueDate.Name = "dtpDueDate";
            this.dtpDueDate.ShowUpDown = true;
            this.dtpDueDate.Size = new System.Drawing.Size(320, 36);
            this.dtpDueDate.ValueChanged += new System.EventHandler(this.dtpDueDate_ValueChanged);
            this.lblDueDateHint.AutoSize = false;
            this.lblDueDateHint.Font = new System.Drawing.Font("default", 9F);
            this.lblDueDateHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblDueDateHint.Location = new System.Drawing.Point(0, 84);
            this.lblDueDateHint.Name = "lblDueDateHint";
            this.lblDueDateHint.Size = new System.Drawing.Size(660, 80);
            this.lblDueDateHint.Text = "How soon a Critical work order must be resolved is a business rule — the wizard does not know it, it only shows what the workflow answers.";
            //
            // pnlNotifications  (step 5)
            //
            this.pnlNotifications.Controls.Add(this.lblNotificationsPrompt);
            this.pnlNotifications.Controls.Add(this.chkNotifyEmail);
            this.pnlNotifications.Controls.Add(this.chkNotifyInApp);
            this.pnlNotifications.Controls.Add(this.chkNotifySms);
            this.pnlNotifications.Controls.Add(this.lblNotificationsHint);
            this.pnlNotifications.Location = new System.Drawing.Point(244, 44);
            this.pnlNotifications.Name = "pnlNotifications";
            this.pnlNotifications.Size = new System.Drawing.Size(660, 340);
            this.pnlNotifications.Visible = false;
            this.lblNotificationsPrompt.AutoSize = false;
            this.lblNotificationsPrompt.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblNotificationsPrompt.Location = new System.Drawing.Point(0, 0);
            this.lblNotificationsPrompt.Name = "lblNotificationsPrompt";
            this.lblNotificationsPrompt.Size = new System.Drawing.Size(660, 28);
            this.lblNotificationsPrompt.Text = "How should the approver hear about it?";
            this.chkNotifyEmail.AutoSize = false;
            this.chkNotifyEmail.Checked = true;
            this.chkNotifyEmail.Location = new System.Drawing.Point(0, 40);
            this.chkNotifyEmail.Name = "chkNotifyEmail";
            this.chkNotifyEmail.Size = new System.Drawing.Size(320, 30);
            this.chkNotifyEmail.Text = "E-mail";
            this.chkNotifyEmail.CheckedChanged += new System.EventHandler(this.notification_CheckedChanged);
            this.chkNotifyInApp.AutoSize = false;
            this.chkNotifyInApp.Checked = true;
            this.chkNotifyInApp.Location = new System.Drawing.Point(0, 76);
            this.chkNotifyInApp.Name = "chkNotifyInApp";
            this.chkNotifyInApp.Size = new System.Drawing.Size(320, 30);
            this.chkNotifyInApp.Text = "In-app notification";
            this.chkNotifyInApp.CheckedChanged += new System.EventHandler(this.notification_CheckedChanged);
            this.chkNotifySms.AutoSize = false;
            this.chkNotifySms.Location = new System.Drawing.Point(0, 112);
            this.chkNotifySms.Name = "chkNotifySms";
            this.chkNotifySms.Size = new System.Drawing.Size(320, 30);
            this.chkNotifySms.Text = "SMS (on-call escalations only)";
            this.chkNotifySms.CheckedChanged += new System.EventHandler(this.notification_CheckedChanged);
            this.lblNotificationsHint.AutoSize = false;
            this.lblNotificationsHint.Font = new System.Drawing.Font("default", 9F);
            this.lblNotificationsHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNotificationsHint.Location = new System.Drawing.Point(0, 156);
            this.lblNotificationsHint.Name = "lblNotificationsHint";
            this.lblNotificationsHint.Size = new System.Drawing.Size(660, 80);
            this.lblNotificationsHint.Text = "Sending is an external step: it sits outside every database transaction, which is why it needs a compensating action.";
            //
            // pnlReview  (step 6 — the summary, then the typed result)
            //
            this.pnlReview.Controls.Add(this.lblReviewPrompt);
            this.pnlReview.Controls.Add(this.dgvSummary);
            this.pnlReview.Controls.Add(this.lblResultBanner);
            this.pnlReview.Controls.Add(this.lblResultDetail);
            this.pnlReview.Location = new System.Drawing.Point(244, 44);
            this.pnlReview.Name = "pnlReview";
            this.pnlReview.Size = new System.Drawing.Size(660, 340);
            this.pnlReview.Visible = false;
            this.lblReviewPrompt.AutoSize = false;
            this.lblReviewPrompt.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblReviewPrompt.Location = new System.Drawing.Point(0, 0);
            this.lblReviewPrompt.Name = "lblReviewPrompt";
            this.lblReviewPrompt.Size = new System.Drawing.Size(660, 28);
            this.lblReviewPrompt.Text = "Review && finish";
            //
            // dgvSummary  (the wizard state as the command will carry it)
            //
            this.dgvSummary.AllowUserToAddRows = false;
            this.dgvSummary.AutoGenerateColumns = false;
            this.dgvSummary.BackColor = System.Drawing.Color.White;
            this.dgvSummary.ColumnHeadersVisible = false;
            this.dgvSummary.Columns.Add(this.colField);
            this.dgvSummary.Columns.Add(this.colValue);
            this.dgvSummary.Location = new System.Drawing.Point(0, 36);
            this.dgvSummary.MultiSelect = false;
            this.dgvSummary.Name = "dgvSummary";
            this.dgvSummary.ReadOnly = true;
            this.dgvSummary.RowHeadersVisible = false;
            this.dgvSummary.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSummary.Size = new System.Drawing.Size(660, 176);
            this.colField.DataPropertyName = "Field";
            this.colField.HeaderText = "Field";
            this.colField.Name = "colField";
            this.colField.Width = 150;
            this.colValue.DataPropertyName = "Value";
            this.colValue.HeaderText = "Value";
            this.colValue.Name = "colValue";
            this.colValue.Width = 480;
            //
            // lblResultBanner  (the WorkflowResult, in the user's words)
            //
            this.lblResultBanner.AutoSize = false;
            this.lblResultBanner.BackColor = System.Drawing.Color.FromArgb(255, 248, 236);
            this.lblResultBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblResultBanner.ForeColor = System.Drawing.Color.FromArgb(122, 82, 16);
            this.lblResultBanner.Location = new System.Drawing.Point(0, 224);
            this.lblResultBanner.Name = "lblResultBanner";
            this.lblResultBanner.Padding = new Wisej.Web.Padding(14, 0, 14, 0);
            this.lblResultBanner.Size = new System.Drawing.Size(660, 38);
            this.lblResultBanner.Text = "";
            this.lblResultBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblResultBanner.Visible = false;
            //
            // lblResultDetail  (the compensation line: persist ✓ · notify ✕ · CompensationAction …)
            //
            this.lblResultDetail.AutoSize = false;
            this.lblResultDetail.BackColor = System.Drawing.Color.FromArgb(255, 248, 236);
            this.lblResultDetail.Font = new System.Drawing.Font("monospace", 9F);
            this.lblResultDetail.ForeColor = System.Drawing.Color.FromArgb(154, 122, 58);
            this.lblResultDetail.Location = new System.Drawing.Point(0, 262);
            this.lblResultDetail.Name = "lblResultDetail";
            this.lblResultDetail.Padding = new Wisej.Web.Padding(14, 0, 14, 0);
            this.lblResultDetail.Size = new System.Drawing.Size(660, 54);
            this.lblResultDetail.Text = "";
            this.lblResultDetail.Visible = false;
            //
            // lblValidation  (what the workflow said about this step — never a rule of this page)
            //
            this.lblValidation.AutoSize = false;
            this.lblValidation.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblValidation.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblValidation.Location = new System.Drawing.Point(244, 392);
            this.lblValidation.Name = "lblValidation";
            this.lblValidation.Size = new System.Drawing.Size(660, 40);
            this.lblValidation.Text = "";
            //
            // lblOrchestration  (validate → authorize → persist → notify → audit, live)
            //
            this.lblOrchestration.AutoSize = false;
            this.lblOrchestration.Font = new System.Drawing.Font("monospace", 9F);
            this.lblOrchestration.ForeColor = System.Drawing.Color.FromArgb(46, 88, 138);
            this.lblOrchestration.Location = new System.Drawing.Point(244, 436);
            this.lblOrchestration.Name = "lblOrchestration";
            this.lblOrchestration.Size = new System.Drawing.Size(660, 26);
            this.lblOrchestration.Text = "EscalationWorkflow:  · validate  · authorize  · persist  · notify  · audit";
            //
            // pnlWizardButtons
            //
            this.pnlWizardButtons.Controls.Add(this.btnCancel);
            this.pnlWizardButtons.Controls.Add(this.lblStepCounter);
            this.pnlWizardButtons.Controls.Add(this.btnBack);
            this.pnlWizardButtons.Controls.Add(this.btnNext);
            this.pnlWizardButtons.Location = new System.Drawing.Point(244, 470);
            this.pnlWizardButtons.Name = "pnlWizardButtons";
            this.pnlWizardButtons.Size = new System.Drawing.Size(660, 46);
            this.btnCancel.Location = new System.Drawing.Point(0, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 38);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.ToolTipText = "Failure path matrix · cancel: keep the draft, or discard it and clean up the staged uploads.";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.lblStepCounter.AutoSize = false;
            this.lblStepCounter.Font = new System.Drawing.Font("default", 9F);
            this.lblStepCounter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblStepCounter.Location = new System.Drawing.Point(122, 4);
            this.lblStepCounter.Name = "lblStepCounter";
            this.lblStepCounter.Size = new System.Drawing.Size(280, 38);
            this.lblStepCounter.Text = "Step 1 of 6";
            this.lblStepCounter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBack.Location = new System.Drawing.Point(410, 4);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(110, 38);
            this.btnBack.Text = "Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            this.btnNext.Location = new System.Drawing.Point(530, 4);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(130, 38);
            this.btnNext.Text = "Next";
            this.btnNext.ToolTipText = "Asks EscalationWorkflow.ValidateStep whether this step may be left; on Review it runs EscalateAsync.";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            //
            // lblWizardStatus  (the dark strip from the video: what the workflow is doing right now)
            //
            this.lblWizardStatus.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblWizardStatus.AutoSize = false;
            this.lblWizardStatus.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblWizardStatus.Font = new System.Drawing.Font("monospace", 9F);
            this.lblWizardStatus.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblWizardStatus.Location = new System.Drawing.Point(0, 536);
            this.lblWizardStatus.Name = "lblWizardStatus";
            this.lblWizardStatus.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.lblWizardStatus.Size = new System.Drawing.Size(940, 30);
            this.lblWizardStatus.Text = "Step 1 of 6 — the pages collect, EscalationWorkflow decides.";
            this.lblWizardStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // EscalationWizard
            //
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.stepsRail);
            this.Controls.Add(this.lblWorkOrder);
            this.Controls.Add(this.pnlReason);
            this.Controls.Add(this.pnlAttachments);
            this.Controls.Add(this.pnlApprover);
            this.Controls.Add(this.pnlDueDate);
            this.Controls.Add(this.pnlNotifications);
            this.Controls.Add(this.pnlReview);
            this.Controls.Add(this.lblValidation);
            this.Controls.Add(this.lblOrchestration);
            this.Controls.Add(this.pnlWizardButtons);
            this.Controls.Add(this.lblWizardStatus);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EscalationWizard";
            this.Size = new System.Drawing.Size(940, 604);
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Escalate work order";
            this.Load += new System.EventHandler(this.EscalationWizard_Load);
            this.FormClosing += new Wisej.Web.FormClosingEventHandler(this.EscalationWizard_FormClosing);
            this.stepsRail.ResumeLayout(false);
            this.pnlReason.ResumeLayout(false);
            this.pnlAttachments.ResumeLayout(false);
            this.pnlApprover.ResumeLayout(false);
            this.pnlDueDate.ResumeLayout(false);
            this.pnlNotifications.ResumeLayout(false);
            this.pnlReview.ResumeLayout(false);
            this.pnlWizardButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.FlowLayoutPanel stepsRail;
        private Wisej.Web.Label lblStepReason;
        private Wisej.Web.Label lblStepAttachments;
        private Wisej.Web.Label lblStepApprover;
        private Wisej.Web.Label lblStepDueDate;
        private Wisej.Web.Label lblStepNotifications;
        private Wisej.Web.Label lblStepReview;
        private Wisej.Web.Label lblDraftState;
        private Wisej.Web.Label lblWorkOrder;
        private Wisej.Web.Panel pnlReason;
        private Wisej.Web.Label lblReasonPrompt;
        private Wisej.Web.TextBox txtReason;
        private Wisej.Web.Label lblReasonHint;
        private Wisej.Web.Panel pnlAttachments;
        private Wisej.Web.Label lblAttachmentsPrompt;
        private Wisej.Web.Button btnStageFile;
        private Wisej.Web.Button btnDiscardFile;
        private Wisej.Web.ListBox lstAttachments;
        private Wisej.Web.Label lblAttachmentsHint;
        private Wisej.Web.Panel pnlApprover;
        private Wisej.Web.Label lblApproverPrompt;
        private Wisej.Web.ComboBox cboApprover;
        private Wisej.Web.Button btnLookupApprovers;
        private Wisej.Web.Label lblApproverHint;
        private Wisej.Web.Panel pnlDueDate;
        private Wisej.Web.Label lblDueDatePrompt;
        private Wisej.Web.DateTimePicker dtpDueDate;
        private Wisej.Web.Label lblDueDateHint;
        private Wisej.Web.Panel pnlNotifications;
        private Wisej.Web.Label lblNotificationsPrompt;
        private Wisej.Web.CheckBox chkNotifyEmail;
        private Wisej.Web.CheckBox chkNotifyInApp;
        private Wisej.Web.CheckBox chkNotifySms;
        private Wisej.Web.Label lblNotificationsHint;
        private Wisej.Web.Panel pnlReview;
        private Wisej.Web.Label lblReviewPrompt;
        private Wisej.Web.DataGridView dgvSummary;
        private Wisej.Web.DataGridViewTextBoxColumn colField;
        private Wisej.Web.DataGridViewTextBoxColumn colValue;
        private Wisej.Web.Label lblResultBanner;
        private Wisej.Web.Label lblResultDetail;
        private Wisej.Web.Label lblValidation;
        private Wisej.Web.Label lblOrchestration;
        private Wisej.Web.Panel pnlWizardButtons;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Label lblStepCounter;
        private Wisej.Web.Button btnBack;
        private Wisej.Web.Button btnNext;
        private Wisej.Web.Label lblWizardStatus;
    }
}
