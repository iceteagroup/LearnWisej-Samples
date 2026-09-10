namespace AdaptiveOps.Shell
{
    partial class DetailsEditor
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new Wisej.Web.Label();
            this.lblSubtitle = new Wisej.Web.Label();
            this.tableEditor = new Wisej.Web.TableLayoutPanel();
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
            this.footerPanel = new Wisej.Web.Panel();
            this.btnSave = new Wisej.Web.Button();
            this.lblValidation = new Wisej.Web.Label();
            this.tableEditor.SuspendLayout();
            this.footerPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle / lblSubtitle  (header, Dock = Top)
            //
            this.lblTitle.AppearanceKey = "subheading-label";
            this.lblTitle.AutoSize = false;
            this.lblTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(308, 24);
            this.lblTitle.TabStop = false;
            this.lblTitle.Text = "Ticket details";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSubtitle.AppearanceKey = "muted-label";
            this.lblSubtitle.AutoEllipsis = true;
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Padding = new Wisej.Web.Padding(0, 0, 0, 6);
            this.lblSubtitle.Size = new System.Drawing.Size(308, 26);
            this.lblSubtitle.TabStop = false;
            this.lblSubtitle.Text = "Select a ticket in the grid";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tableEditor  (Dock = Fill · 2 columns: captions Absolute 84, editors Percent 100 · rows: 5 × Absolute 40 + Notes Percent 100)
            //
            this.tableEditor.ColumnCount = 2;
            this.tableEditor.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Absolute, 84F));
            this.tableEditor.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 100F));
            this.tableEditor.Dock = Wisej.Web.DockStyle.Fill;
            this.tableEditor.Name = "tableEditor";
            this.tableEditor.RowCount = 6;
            this.tableEditor.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 40F));
            this.tableEditor.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 40F));
            this.tableEditor.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 40F));
            this.tableEditor.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 40F));
            this.tableEditor.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 40F));
            this.tableEditor.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
            this.tableEditor.TabStop = false;
            //
            // captions (column 0) and editors (column 1); editors Dock = Fill inside their cell
            //
            InitCaption(this.lblTitleCaption, "Title", 0);
            this.txtTitle.AccessibleName = "Title";
            this.txtTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.txtTitle.Margin = new Wisej.Web.Padding(0, 4, 0, 4);
            this.txtTitle.MaxLength = 120;
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.TabIndex = 1;
            this.txtTitle.ToolTipText = "Short summary (required, max 80 characters)";
            this.txtTitle.Watermark = "Short summary (required, max 80 characters)";
            InitCaption(this.lblPriorityCaption, "Priority", 1);
            this.cboPriority.AccessibleName = "Priority";
            this.cboPriority.Dock = Wisej.Web.DockStyle.Fill;
            this.cboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPriority.Margin = new Wisej.Web.Padding(0, 4, 0, 4);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.TabIndex = 2;
            InitCaption(this.lblStatusCaption, "Status", 2);
            this.cboStatus.AccessibleName = "Status";
            this.cboStatus.Dock = Wisej.Web.DockStyle.Fill;
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Margin = new Wisej.Web.Padding(0, 4, 0, 4);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.TabIndex = 3;
            InitCaption(this.lblOwnerCaption, "Owner", 3);
            this.txtOwner.AccessibleName = "Owner";
            this.txtOwner.Dock = Wisej.Web.DockStyle.Fill;
            this.txtOwner.Margin = new Wisej.Web.Padding(0, 4, 0, 4);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.TabIndex = 4;
            this.txtOwner.ToolTipText = "Agent name (required)";
            this.txtOwner.Watermark = "Agent name (required)";
            InitCaption(this.lblDueCaption, "Due", 4);
            this.dtpDue.AccessibleName = "Due date";
            this.dtpDue.Dock = Wisej.Web.DockStyle.Fill;
            this.dtpDue.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpDue.Margin = new Wisej.Web.Padding(0, 4, 0, 4);
            this.dtpDue.Name = "dtpDue";
            this.dtpDue.TabIndex = 5;
            InitCaption(this.lblNotesCaption, "Notes", 5);
            this.lblNotesCaption.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.lblNotesCaption.Padding = new Wisej.Web.Padding(0, 8, 0, 0);
            this.txtNotes.AccessibleName = "Notes";
            this.txtNotes.Dock = Wisej.Web.DockStyle.Fill;
            this.txtNotes.Margin = new Wisej.Web.Padding(0, 4, 0, 4);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.TabIndex = 6;
            this.txtNotes.Watermark = "Anything the next agent needs";
            this.tableEditor.Controls.Add(this.lblTitleCaption, 0, 0);
            this.tableEditor.Controls.Add(this.txtTitle, 1, 0);
            this.tableEditor.Controls.Add(this.lblPriorityCaption, 0, 1);
            this.tableEditor.Controls.Add(this.cboPriority, 1, 1);
            this.tableEditor.Controls.Add(this.lblStatusCaption, 0, 2);
            this.tableEditor.Controls.Add(this.cboStatus, 1, 2);
            this.tableEditor.Controls.Add(this.lblOwnerCaption, 0, 3);
            this.tableEditor.Controls.Add(this.txtOwner, 1, 3);
            this.tableEditor.Controls.Add(this.lblDueCaption, 0, 4);
            this.tableEditor.Controls.Add(this.dtpDue, 1, 4);
            this.tableEditor.Controls.Add(this.lblNotesCaption, 0, 5);
            this.tableEditor.Controls.Add(this.txtNotes, 1, 5);
            //
            // footerPanel  (Dock = Bottom · Save + validation label)
            //
            this.footerPanel.Controls.Add(this.lblValidation);
            this.footerPanel.Controls.Add(this.btnSave);
            this.footerPanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Padding = new Wisej.Web.Padding(0, 8, 0, 0);
            this.footerPanel.Size = new System.Drawing.Size(308, 44);
            this.footerPanel.TabStop = false;
            //
            // btnSave  (the primary command: action-button, last in the editor's tab order)
            //
            this.btnSave.AccessibleName = "Save ticket";
            this.btnSave.AppearanceKey = "action-button";
            this.btnSave.Dock = Wisej.Web.DockStyle.Left;
            this.btnSave.ImageSource = "icon-save?color=white";
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(96, 36);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.ToolTipText = "Validate on the server and store the ticket";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // lblValidation  (announces the validation message as text + icon; theme state "error")
            //
            this.lblValidation.AccessibleName = "Validation status";
            this.lblValidation.AppearanceKey = "validation-label";
            this.lblValidation.AutoEllipsis = true;
            this.lblValidation.AutoSize = false;
            this.lblValidation.Dock = Wisej.Web.DockStyle.Fill;
            this.lblValidation.Name = "lblValidation";
            this.lblValidation.Padding = new Wisej.Web.Padding(8, 0, 0, 0);
            this.lblValidation.TabStop = false;
            this.lblValidation.Text = "Validation runs on the server.";
            this.lblValidation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // DetailsEditor  (theme appearance surface-card)
            //
            this.AppearanceKey = "surface-card";
            this.Controls.Add(this.tableEditor);
            this.Controls.Add(this.footerPanel);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.lblTitle);
            this.Name = "DetailsEditor";
            this.Padding = new Wisej.Web.Padding(12);
            this.Size = new System.Drawing.Size(332, 588);
            this.TabStop = false;
            this.tableEditor.ResumeLayout(false);
            this.footerPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private static void InitCaption(Wisej.Web.Label label, string text, int row)
        {
            label.AppearanceKey = "muted-label";
            label.AutoSize = false;
            label.Dock = Wisej.Web.DockStyle.Fill;
            label.Margin = new Wisej.Web.Padding(0);
            label.Name = "lbl" + text + "Caption";
            label.TabStop = false;
            label.Text = text;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        }

        #endregion

        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblSubtitle;
        private Wisej.Web.TableLayoutPanel tableEditor;
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
        private Wisej.Web.Panel footerPanel;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Label lblValidation;
    }
}
