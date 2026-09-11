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
            this.table = new Wisej.Web.TableLayoutPanel();
            this.lblTitle = new Wisej.Web.Label();
            this.lblSubtitle = new Wisej.Web.Label();
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
            this.btnSave = new Wisej.Web.Button();
            this.table.SuspendLayout();
            this.SuspendLayout();
            //
            // table  (TableLayoutPanel · Dock = Fill · 2 columns)
            //
            // The proportions of the form live in the styles, not in the children:
            //   column 0  Absolute 140  — captions, never move however long the translation is
            //   column 1  Percent 100   — editors, grow with the panel
            //   rows 0-7, 9  AutoSize   — as tall as the tallest child (+ its Margin)
            //   row 8        Percent 100 — the Notes editor takes the remaining height (min 120 from its MinimumSize)
            // Children are addressed by cell (Controls.Add(control, column, row)); the Notes editor spans both columns.
            //
            this.table.ColumnCount = 2;
            this.table.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Absolute, 140F));
            this.table.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 100F));
            this.table.Controls.Add(this.lblTitle, 0, 0);
            this.table.Controls.Add(this.lblSubtitle, 0, 1);
            this.table.Controls.Add(this.lblTitleCaption, 0, 2);
            this.table.Controls.Add(this.txtTitle, 1, 2);
            this.table.Controls.Add(this.lblPriorityCaption, 0, 3);
            this.table.Controls.Add(this.cboPriority, 1, 3);
            this.table.Controls.Add(this.lblStatusCaption, 0, 4);
            this.table.Controls.Add(this.cboStatus, 1, 4);
            this.table.Controls.Add(this.lblOwnerCaption, 0, 5);
            this.table.Controls.Add(this.txtOwner, 1, 5);
            this.table.Controls.Add(this.lblDueCaption, 0, 6);
            this.table.Controls.Add(this.dtpDue, 1, 6);
            this.table.Controls.Add(this.lblNotesCaption, 0, 7);
            this.table.Controls.Add(this.txtNotes, 0, 8);
            this.table.Controls.Add(this.btnSave, 0, 9);
            this.table.Dock = Wisej.Web.DockStyle.Fill;
            this.table.GrowStyle = Wisej.Web.TableLayoutPanelGrowStyle.FixedSize;
            this.table.Name = "table";
            this.table.Padding = new Wisej.Web.Padding(12);
            this.table.RowCount = 10;
            this.table.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.AutoSize));
            this.table.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.AutoSize));
            this.table.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.AutoSize));
            this.table.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.AutoSize));
            this.table.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.AutoSize));
            this.table.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.AutoSize));
            this.table.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.AutoSize));
            this.table.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.AutoSize));
            this.table.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
            this.table.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.AutoSize));
            this.table.SetColumnSpan(this.lblTitle, 2);
            this.table.SetColumnSpan(this.lblSubtitle, 2);
            this.table.SetColumnSpan(this.lblNotesCaption, 2);
            this.table.SetColumnSpan(this.txtNotes, 2);
            //
            // lblTitle / lblSubtitle  (rows 0 and 1, span both columns)
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Margin = new Wisej.Web.Padding(0, 0, 0, 2);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(292, 24);
            this.lblTitle.Text = "Ticket details";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSubtitle.AutoEllipsis = true;
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblSubtitle.Margin = new Wisej.Web.Padding(0, 0, 0, 8);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(292, 18);
            this.lblSubtitle.Text = "Select a ticket in the grid";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // captions  (column 0, docked Fill in the 140-px cell, vertically centred against the 28-px editors)
            //
            this.lblTitleCaption.AutoSize = false;
            this.lblTitleCaption.Dock = Wisej.Web.DockStyle.Fill;
            this.lblTitleCaption.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.lblTitleCaption.Name = "lblTitleCaption";
            this.lblTitleCaption.Size = new System.Drawing.Size(132, 28);
            this.lblTitleCaption.Text = "Title";
            this.lblTitleCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPriorityCaption.AutoSize = false;
            this.lblPriorityCaption.Dock = Wisej.Web.DockStyle.Fill;
            this.lblPriorityCaption.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.lblPriorityCaption.Name = "lblPriorityCaption";
            this.lblPriorityCaption.Size = new System.Drawing.Size(132, 28);
            this.lblPriorityCaption.Text = "Priority";
            this.lblPriorityCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatusCaption.AutoSize = false;
            this.lblStatusCaption.Dock = Wisej.Web.DockStyle.Fill;
            this.lblStatusCaption.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(132, 28);
            this.lblStatusCaption.Text = "Status";
            this.lblStatusCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblOwnerCaption.AutoSize = false;
            this.lblOwnerCaption.Dock = Wisej.Web.DockStyle.Fill;
            this.lblOwnerCaption.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.lblOwnerCaption.Name = "lblOwnerCaption";
            this.lblOwnerCaption.Size = new System.Drawing.Size(132, 28);
            this.lblOwnerCaption.Text = "Owner";
            this.lblOwnerCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDueCaption.AutoSize = false;
            this.lblDueCaption.Dock = Wisej.Web.DockStyle.Fill;
            this.lblDueCaption.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.lblDueCaption.Name = "lblDueCaption";
            this.lblDueCaption.Size = new System.Drawing.Size(132, 28);
            this.lblDueCaption.Text = "Due";
            this.lblDueCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNotesCaption.AutoSize = false;
            this.lblNotesCaption.Dock = Wisej.Web.DockStyle.Fill;
            this.lblNotesCaption.Margin = new Wisej.Web.Padding(0, 4, 0, 4);
            this.lblNotesCaption.Name = "lblNotesCaption";
            this.lblNotesCaption.Size = new System.Drawing.Size(292, 20);
            this.lblNotesCaption.Text = "Notes";
            this.lblNotesCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // editors  (column 1, docked Fill in their cells: the Percent column decides their width, the AutoSize row their height)
            //
            this.txtTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.txtTitle.Margin = new Wisej.Web.Padding(0, 0, 0, 6);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(160, 28);
            this.txtTitle.Watermark = "Short summary (required, max 80 characters)";
            this.cboPriority.Dock = Wisej.Web.DockStyle.Fill;
            this.cboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPriority.Margin = new Wisej.Web.Padding(0, 0, 0, 6);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.Size = new System.Drawing.Size(160, 28);
            this.cboStatus.Dock = Wisej.Web.DockStyle.Fill;
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Margin = new Wisej.Web.Padding(0, 0, 0, 6);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(160, 28);
            this.txtOwner.Dock = Wisej.Web.DockStyle.Fill;
            this.txtOwner.Margin = new Wisej.Web.Padding(0, 0, 0, 6);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.Size = new System.Drawing.Size(160, 28);
            this.txtOwner.Watermark = "Agent name (required)";
            this.dtpDue.Dock = Wisej.Web.DockStyle.Fill;
            this.dtpDue.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpDue.Margin = new Wisej.Web.Padding(0, 0, 0, 6);
            this.dtpDue.Name = "dtpDue";
            this.dtpDue.Size = new System.Drawing.Size(160, 28);
            //
            // txtNotes  (row 8, spans both columns, Percent 100 row: takes the remaining height, never below 120)
            //
            this.txtNotes.Dock = Wisej.Web.DockStyle.Fill;
            this.txtNotes.Margin = new Wisej.Web.Padding(0, 0, 0, 8);
            this.txtNotes.MinimumSize = new System.Drawing.Size(0, 120);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(292, 120);
            this.txtNotes.Watermark = "Anything the next agent needs";
            //
            // btnSave  (row 9: keeps its own size in the 140-px column)
            //
            this.btnSave.Margin = new Wisej.Web.Padding(0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 32);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // TicketEditor
            //
            this.Controls.Add(this.table);
            this.Name = "TicketEditor";
            this.Size = new System.Drawing.Size(316, 420);
            this.table.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.TableLayoutPanel table;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblSubtitle;
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
        private Wisej.Web.Button btnSave;
    }
}
