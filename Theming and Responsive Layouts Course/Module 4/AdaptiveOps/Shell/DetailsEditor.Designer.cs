namespace AdaptiveOps.Shell
{
    partial class DetailsEditor
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
            this.headerPanel = new Wisej.Web.Panel();
            this.lblDetailsTitle = new Wisej.Web.Label();
            this.lblDetailsSubtitle = new Wisej.Web.Label();
            this.fieldsPanel = new Wisej.Web.Panel();
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
            this.lblIdCaption = new Wisej.Web.Label();
            this.txtId = new Wisej.Web.TextBox();
            this.lblNotesCaption = new Wisej.Web.Label();
            this.txtNotes = new Wisej.Web.TextBox();
            this.commandBar = new Wisej.Web.Panel();
            this.btnCancel = new Wisej.Web.Button();
            this.btnSave = new Wisej.Web.Button();
            this.headerPanel.SuspendLayout();
            this.fieldsPanel.SuspendLayout();
            this.commandBar.SuspendLayout();
            this.SuspendLayout();
            //
            // headerPanel  (Dock = Top · title + subtitle; never scrolls)
            //
            this.headerPanel.Controls.Add(this.lblDetailsTitle);
            this.headerPanel.Controls.Add(this.lblDetailsSubtitle);
            this.headerPanel.Dock = Wisej.Web.DockStyle.Top;
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(330, 62);
            //
            // lblDetailsTitle  (AutoSize = true: content-driven)
            //
            this.lblDetailsTitle.AutoSize = true;
            this.lblDetailsTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblDetailsTitle.Location = new System.Drawing.Point(12, 12);
            this.lblDetailsTitle.Name = "lblDetailsTitle";
            this.lblDetailsTitle.Text = "Ticket details";
            //
            // lblDetailsSubtitle  (AutoSize = false + Anchor Left|Right + AutoEllipsis: container-driven)
            //
            this.lblDetailsSubtitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblDetailsSubtitle.AutoEllipsis = true;
            this.lblDetailsSubtitle.AutoSize = false;
            this.lblDetailsSubtitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblDetailsSubtitle.Location = new System.Drawing.Point(12, 38);
            this.lblDetailsSubtitle.Name = "lblDetailsSubtitle";
            this.lblDetailsSubtitle.Size = new System.Drawing.Size(306, 18);
            this.lblDetailsSubtitle.Text = "Select a ticket in the grid";
            //
            // fieldsPanel  (Dock = Fill · the scrolling part of the editor)
            //
            // AutoScroll = true, ScrollBars = Hidden, AutoScrollMargin = (0, 24): when the browser is
            // short the eight fields scroll by wheel or touch instead of being clipped or hidden, no
            // scrollbar steals width from the anchored editors, and there is breathing room after
            // the notes. Nothing inside is anchored Bottom: in a scrolling container the bottom is
            // the virtual bottom of the content, so the command buttons live in commandBar instead.
            //
            this.fieldsPanel.AutoScroll = true;
            this.fieldsPanel.AutoScrollMargin = new System.Drawing.Size(0, 24);
            this.fieldsPanel.Controls.Add(this.lblTitleCaption);
            this.fieldsPanel.Controls.Add(this.txtTitle);
            this.fieldsPanel.Controls.Add(this.lblPriorityCaption);
            this.fieldsPanel.Controls.Add(this.cboPriority);
            this.fieldsPanel.Controls.Add(this.lblStatusCaption);
            this.fieldsPanel.Controls.Add(this.cboStatus);
            this.fieldsPanel.Controls.Add(this.lblOwnerCaption);
            this.fieldsPanel.Controls.Add(this.txtOwner);
            this.fieldsPanel.Controls.Add(this.lblDueCaption);
            this.fieldsPanel.Controls.Add(this.dtpDue);
            this.fieldsPanel.Controls.Add(this.lblIdCaption);
            this.fieldsPanel.Controls.Add(this.txtId);
            this.fieldsPanel.Controls.Add(this.lblNotesCaption);
            this.fieldsPanel.Controls.Add(this.txtNotes);
            this.fieldsPanel.Dock = Wisej.Web.DockStyle.Fill;
            this.fieldsPanel.Name = "fieldsPanel";
            this.fieldsPanel.ScrollBars = Wisej.Web.ScrollBars.Hidden;
            this.fieldsPanel.Size = new System.Drawing.Size(330, 476);
            //
            // Title  (caption Top|Left AutoSize · editor Top|Left|Right so it stretches with the panel)
            //
            this.lblTitleCaption.AutoSize = true;
            this.lblTitleCaption.Location = new System.Drawing.Point(12, 8);
            this.lblTitleCaption.Name = "lblTitleCaption";
            this.lblTitleCaption.Text = "Title";
            this.txtTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtTitle.Location = new System.Drawing.Point(12, 26);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(306, 28);
            this.txtTitle.Watermark = "Short summary (required, max 80 characters)";
            //
            // Priority (Top|Left, fixed width) / Status (Top|Right, keeps its distance from the right edge)
            //
            this.lblPriorityCaption.AutoSize = true;
            this.lblPriorityCaption.Location = new System.Drawing.Point(12, 64);
            this.lblPriorityCaption.Name = "lblPriorityCaption";
            this.lblPriorityCaption.Text = "Priority";
            this.cboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPriority.Location = new System.Drawing.Point(12, 82);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.Size = new System.Drawing.Size(146, 28);
            this.lblStatusCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblStatusCaption.AutoSize = true;
            this.lblStatusCaption.Location = new System.Drawing.Point(172, 64);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Text = "Status";
            this.cboStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(172, 82);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(146, 28);
            //
            // Owner (Top|Left) / Due (Top|Right)
            //
            this.lblOwnerCaption.AutoSize = true;
            this.lblOwnerCaption.Location = new System.Drawing.Point(12, 120);
            this.lblOwnerCaption.Name = "lblOwnerCaption";
            this.lblOwnerCaption.Text = "Owner";
            this.txtOwner.Location = new System.Drawing.Point(12, 138);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.Size = new System.Drawing.Size(146, 28);
            this.txtOwner.Watermark = "Agent name (required)";
            this.lblDueCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblDueCaption.AutoSize = true;
            this.lblDueCaption.Location = new System.Drawing.Point(172, 120);
            this.lblDueCaption.Name = "lblDueCaption";
            this.lblDueCaption.Text = "Due";
            this.dtpDue.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.dtpDue.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpDue.Location = new System.Drawing.Point(172, 138);
            this.dtpDue.Name = "dtpDue";
            this.dtpDue.Size = new System.Drawing.Size(146, 28);
            //
            // Ticket id  (read-only, Top|Left|Right)
            //
            this.lblIdCaption.AutoSize = true;
            this.lblIdCaption.Location = new System.Drawing.Point(12, 176);
            this.lblIdCaption.Name = "lblIdCaption";
            this.lblIdCaption.Text = "Ticket id";
            this.txtId.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtId.Location = new System.Drawing.Point(12, 194);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(306, 28);
            //
            // Notes  (wide editor: Top|Left|Right, fixed height - not Bottom, the container scrolls)
            //
            this.lblNotesCaption.AutoSize = true;
            this.lblNotesCaption.Location = new System.Drawing.Point(12, 232);
            this.lblNotesCaption.Name = "lblNotesCaption";
            this.lblNotesCaption.Text = "Notes";
            this.txtNotes.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtNotes.Location = new System.Drawing.Point(12, 250);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(306, 180);
            this.txtNotes.Watermark = "Anything the next agent needs";
            //
            // commandBar  (Dock = Bottom · fixed height, so Bottom|Right anchors inside it are stable)
            //
            this.commandBar.Controls.Add(this.btnCancel);
            this.commandBar.Controls.Add(this.btnSave);
            this.commandBar.Dock = Wisej.Web.DockStyle.Bottom;
            this.commandBar.Name = "commandBar";
            this.commandBar.Size = new System.Drawing.Size(330, 48);
            //
            // btnCancel
            //
            this.btnCancel.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.btnCancel.Location = new System.Drawing.Point(122, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 32);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnSave
            //
            this.btnSave.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.btnSave.Location = new System.Drawing.Point(218, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 32);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // DetailsEditor
            //
            // Child order = dock priority: fieldsPanel (Fill) is added first so it is docked LAST and
            // takes what the header (Top) and the command bar (Bottom) leave.
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.fieldsPanel);
            this.Controls.Add(this.commandBar);
            this.Controls.Add(this.headerPanel);
            this.Name = "DetailsEditor";
            this.Size = new System.Drawing.Size(332, 588);
            this.headerPanel.ResumeLayout(false);
            this.fieldsPanel.ResumeLayout(false);
            this.commandBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel headerPanel;
        private Wisej.Web.Label lblDetailsTitle;
        private Wisej.Web.Label lblDetailsSubtitle;
        private Wisej.Web.Panel fieldsPanel;
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
        private Wisej.Web.Label lblIdCaption;
        private Wisej.Web.TextBox txtId;
        private Wisej.Web.Label lblNotesCaption;
        private Wisej.Web.TextBox txtNotes;
        private Wisej.Web.Panel commandBar;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Button btnSave;
    }
}
