namespace AdaptiveOps.Shell
{
    partial class TicketEditor
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
            this.lblDetailsTitle = new Wisej.Web.Label();
            this.lblDetailsSubtitle = new Wisej.Web.Label();
            this.lblTitleCaption = new Wisej.Web.Label();
            this.txtTitle = new Wisej.Web.TextBox();
            this.lblPriorityCaption = new Wisej.Web.Label();
            this.cboPriority = new Wisej.Web.ComboBox();
            this.lblStatusCaption = new Wisej.Web.Label();
            this.cboStatus = new Wisej.Web.ComboBox();
            this.lblOwnerCaption = new Wisej.Web.Label();
            this.txtOwner = new Wisej.Web.TextBox();
            this.lblDueCaption = new Wisej.Web.Label();
            this.dtpDue = new Wisej.Web.DateTimePicker();
            this.lblNotesCaption = new Wisej.Web.Label();
            this.txtNotes = new Wisej.Web.TextBox();
            this.lblMessage = new Wisej.Web.Label();
            this.btnSave = new Wisej.Web.Button();
            this.btnClose = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // lblDetailsTitle / lblDetailsSubtitle
            //
            this.lblDetailsTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblDetailsTitle.AutoSize = false;
            this.lblDetailsTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblDetailsTitle.Location = new System.Drawing.Point(12, 12);
            this.lblDetailsTitle.Name = "lblDetailsTitle";
            this.lblDetailsTitle.Size = new System.Drawing.Size(306, 24);
            this.lblDetailsTitle.Text = "Ticket details";
            this.lblDetailsSubtitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblDetailsSubtitle.AutoEllipsis = true;
            this.lblDetailsSubtitle.AutoSize = false;
            this.lblDetailsSubtitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblDetailsSubtitle.Location = new System.Drawing.Point(12, 38);
            this.lblDetailsSubtitle.Name = "lblDetailsSubtitle";
            this.lblDetailsSubtitle.Size = new System.Drawing.Size(306, 18);
            this.lblDetailsSubtitle.Text = "Select a ticket in the grid";
            //
            // Title
            //
            this.lblTitleCaption.AutoSize = false;
            this.lblTitleCaption.Location = new System.Drawing.Point(12, 66);
            this.lblTitleCaption.Name = "lblTitleCaption";
            this.lblTitleCaption.Size = new System.Drawing.Size(140, 16);
            this.lblTitleCaption.Text = "Title";
            this.txtTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtTitle.Location = new System.Drawing.Point(12, 84);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(306, 28);
            this.txtTitle.Watermark = "Short summary (required, max 80 characters)";
            //
            // Priority / Status
            //
            this.lblPriorityCaption.AutoSize = false;
            this.lblPriorityCaption.Location = new System.Drawing.Point(12, 122);
            this.lblPriorityCaption.Name = "lblPriorityCaption";
            this.lblPriorityCaption.Size = new System.Drawing.Size(140, 16);
            this.lblPriorityCaption.Text = "Priority";
            this.cboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPriority.Location = new System.Drawing.Point(12, 140);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.Size = new System.Drawing.Size(146, 28);
            this.lblStatusCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblStatusCaption.AutoSize = false;
            this.lblStatusCaption.Location = new System.Drawing.Point(172, 122);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(146, 16);
            this.lblStatusCaption.Text = "Status";
            this.cboStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(172, 140);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(146, 28);
            //
            // Owner / Due
            //
            this.lblOwnerCaption.AutoSize = false;
            this.lblOwnerCaption.Location = new System.Drawing.Point(12, 178);
            this.lblOwnerCaption.Name = "lblOwnerCaption";
            this.lblOwnerCaption.Size = new System.Drawing.Size(140, 16);
            this.lblOwnerCaption.Text = "Owner";
            this.txtOwner.Location = new System.Drawing.Point(12, 196);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.Size = new System.Drawing.Size(146, 28);
            this.txtOwner.Watermark = "Agent name (required)";
            this.lblDueCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblDueCaption.AutoSize = false;
            this.lblDueCaption.Location = new System.Drawing.Point(172, 178);
            this.lblDueCaption.Name = "lblDueCaption";
            this.lblDueCaption.Size = new System.Drawing.Size(146, 16);
            this.lblDueCaption.Text = "Due";
            this.dtpDue.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.dtpDue.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpDue.Location = new System.Drawing.Point(172, 196);
            this.dtpDue.Name = "dtpDue";
            this.dtpDue.Size = new System.Drawing.Size(146, 28);
            //
            // Notes  (the only field that grows with the host: Anchor Top|Bottom)
            //
            this.lblNotesCaption.AutoSize = false;
            this.lblNotesCaption.Location = new System.Drawing.Point(12, 234);
            this.lblNotesCaption.Name = "lblNotesCaption";
            this.lblNotesCaption.Size = new System.Drawing.Size(140, 16);
            this.lblNotesCaption.Text = "Notes";
            this.txtNotes.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtNotes.Location = new System.Drawing.Point(12, 252);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(306, 236);
            this.txtNotes.Watermark = "Anything the next agent needs";
            //
            // lblMessage  (save outcome, shown INSIDE the editor so it is visible in the modal too)
            //
            this.lblMessage.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblMessage.AutoEllipsis = true;
            this.lblMessage.AutoSize = false;
            this.lblMessage.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblMessage.Location = new System.Drawing.Point(12, 500);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(306, 22);
            this.lblMessage.Text = "";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMessage.Visible = false;
            //
            // btnSave  (validates on the server through the host, then writes back to the repository and the grid)
            //
            this.btnSave.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnSave.ImageSource = "icon-save";
            this.btnSave.Location = new System.Drawing.Point(12, 532);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(96, 32);
            this.btnSave.Text = "Save";
            this.btnSave.ToolTipText = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnClose  (visible only while the editor is hosted in the phone dialog)
            //
            this.btnClose.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnClose.ImageSource = "icon-close";
            this.btnClose.Location = new System.Drawing.Point(116, 532);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(96, 32);
            this.btnClose.Text = "Close";
            this.btnClose.ToolTipText = "Close the editor dialog";
            this.btnClose.Visible = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // TicketEditor
            //
            // MinimumSize keeps the form usable when the details region is docked under the grid on Tablet
            // (320 px tall): the hosting card has AutoScroll, so the editor scrolls instead of collapsing.
            //
            this.Controls.Add(this.lblDetailsTitle);
            this.Controls.Add(this.lblDetailsSubtitle);
            this.Controls.Add(this.lblTitleCaption);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblPriorityCaption);
            this.Controls.Add(this.cboPriority);
            this.Controls.Add(this.lblStatusCaption);
            this.Controls.Add(this.cboStatus);
            this.Controls.Add(this.lblOwnerCaption);
            this.Controls.Add(this.txtOwner);
            this.Controls.Add(this.lblDueCaption);
            this.Controls.Add(this.dtpDue);
            this.Controls.Add(this.lblNotesCaption);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.MinimumSize = new System.Drawing.Size(0, 440);
            this.Name = "TicketEditor";
            this.Size = new System.Drawing.Size(330, 576);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblDetailsTitle;
        private Wisej.Web.Label lblDetailsSubtitle;
        private Wisej.Web.Label lblTitleCaption;
        private Wisej.Web.TextBox txtTitle;
        private Wisej.Web.Label lblPriorityCaption;
        private Wisej.Web.ComboBox cboPriority;
        private Wisej.Web.Label lblStatusCaption;
        private Wisej.Web.ComboBox cboStatus;
        private Wisej.Web.Label lblOwnerCaption;
        private Wisej.Web.TextBox txtOwner;
        private Wisej.Web.Label lblDueCaption;
        private Wisej.Web.DateTimePicker dtpDue;
        private Wisej.Web.Label lblNotesCaption;
        private Wisej.Web.TextBox txtNotes;
        private Wisej.Web.Label lblMessage;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnClose;
    }
}
