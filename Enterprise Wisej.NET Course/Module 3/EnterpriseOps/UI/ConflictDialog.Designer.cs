namespace EnterpriseOps.UI
{
    partial class ConflictDialog
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
            this.lblIcon = new Wisej.Web.Label();
            this.lblHeadline = new Wisej.Web.Label();
            this.lblExplain = new Wisej.Web.Label();
            this.dgvVersions = new Wisej.Web.DataGridView();
            this.colWhich = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colVersion = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlActions = new Wisej.Web.FlowLayoutPanel();
            this.btnReload = new Wisej.Web.Button();
            this.btnCompare = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.lblFootnote = new Wisej.Web.Label();
            this.pnlCompare = new Wisej.Web.Panel();
            this.lblCompareTitle = new Wisej.Web.Label();
            this.dgvCompare = new Wisej.Web.DataGridView();
            this.colField = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colYours = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCurrent = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colVerdict = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlActions.SuspendLayout();
            this.pnlCompare.SuspendLayout();
            this.SuspendLayout();
            //
            // lblIcon  (the amber "!" from the walkthrough — a warning, not an error: nothing was lost)
            //
            this.lblIcon.AutoSize = false;
            this.lblIcon.BackColor = System.Drawing.Color.FromArgb(255, 244, 227);
            this.lblIcon.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblIcon.ForeColor = System.Drawing.Color.FromArgb(185, 119, 14);
            this.lblIcon.Location = new System.Drawing.Point(26, 20);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.Size = new System.Drawing.Size(44, 44);
            this.lblIcon.Text = "!";
            this.lblIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblHeadline
            //
            this.lblHeadline.AutoSize = false;
            this.lblHeadline.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblHeadline.ForeColor = System.Drawing.Color.FromArgb(13, 27, 42);
            this.lblHeadline.Location = new System.Drawing.Point(86, 18);
            this.lblHeadline.Name = "lblHeadline";
            this.lblHeadline.Size = new System.Drawing.Size(740, 26);
            this.lblHeadline.Text = "This work order changed while you were editing";
            //
            // lblExplain  (what happened, in the user's words — no exception text, no version numbers yet)
            //
            this.lblExplain.AutoSize = false;
            this.lblExplain.Font = new System.Drawing.Font("default", 9F);
            this.lblExplain.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblExplain.Location = new System.Drawing.Point(86, 46);
            this.lblExplain.Name = "lblExplain";
            this.lblExplain.Size = new System.Drawing.Size(740, 44);
            this.lblExplain.Text = "Another session saved a newer version. Your edit is based on a stale copy — nothing has been overwritten.";
            //
            // dgvVersions  (two rows: YOUR EDIT vs CURRENT — bound to ConflictSummaryRow, built by the service)
            //
            this.dgvVersions.AllowUserToAddRows = false;
            this.dgvVersions.AllowUserToDeleteRows = false;
            this.dgvVersions.AutoGenerateColumns = false;
            this.dgvVersions.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvVersions.BackColor = System.Drawing.Color.White;
            this.dgvVersions.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colWhich,
            this.colTitle,
            this.colStatus,
            this.colVersion});
            this.dgvVersions.Location = new System.Drawing.Point(26, 104);
            this.dgvVersions.MultiSelect = false;
            this.dgvVersions.Name = "dgvVersions";
            this.dgvVersions.ReadOnly = true;
            this.dgvVersions.RowHeadersVisible = false;
            this.dgvVersions.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVersions.Size = new System.Drawing.Size(800, 106);
            //
            // colWhich
            //
            this.colWhich.DataPropertyName = "Which";
            this.colWhich.FillWeight = 20F;
            this.colWhich.HeaderText = "";
            this.colWhich.Name = "colWhich";
            this.colWhich.ReadOnly = true;
            //
            // colTitle
            //
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.FillWeight = 44F;
            this.colTitle.HeaderText = "Title";
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.FillWeight = 16F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            //
            // colVersion
            //
            this.colVersion.DataPropertyName = "Version";
            this.colVersion.FillWeight = 20F;
            this.colVersion.HeaderText = "Version";
            this.colVersion.Name = "colVersion";
            this.colVersion.ReadOnly = true;
            //
            // pnlActions  (the three honest paths — Reload · Compare · Cancel)
            //
            this.pnlActions.Controls.Add(this.btnReload);
            this.pnlActions.Controls.Add(this.btnCompare);
            this.pnlActions.Controls.Add(this.btnCancel);
            this.pnlActions.Location = new System.Drawing.Point(26, 226);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(620, 48);
            //
            // btnReload  (the recovery: discard the local edits, show the current record)
            //
            this.btnReload.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnReload.Location = new System.Drawing.Point(0, 0);
            this.btnReload.Margin = new Wisej.Web.Padding(0, 0, 12, 0);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(180, 40);
            this.btnReload.Text = "Reload latest";
            this.btnReload.ToolTipText = "Discards the local edits and shows the current record. Recorded in the audit trail with the correlation id.";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            //
            // btnCompare  (shows both versions field by field so the edit can be merged — the dialog stays open)
            //
            this.btnCompare.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnCompare.Location = new System.Drawing.Point(192, 0);
            this.btnCompare.Margin = new Wisej.Web.Padding(0, 0, 12, 0);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(200, 40);
            this.btnCompare.Text = "Compare changes";
            this.btnCompare.ToolTipText = "Shows your edit and the current record field by field. Nothing is saved by comparing.";
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            //
            // btnCancel  (leave the screen as it is and decide later)
            //
            this.btnCancel.Location = new System.Drawing.Point(404, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 40);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.ToolTipText = "Leaves the screen as it is — nothing saved, nothing lost. Your edit stays on the stale version.";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // lblFootnote  ("correlation 8f3a21c4 — expected v7, found v8")
            //
            this.lblFootnote.AutoSize = false;
            this.lblFootnote.Font = new System.Drawing.Font("monospace", 9F);
            this.lblFootnote.ForeColor = System.Drawing.Color.FromArgb(138, 151, 164);
            this.lblFootnote.Location = new System.Drawing.Point(26, 282);
            this.lblFootnote.Name = "lblFootnote";
            this.lblFootnote.Size = new System.Drawing.Size(800, 22);
            this.lblFootnote.Text = "correlation — expected —, found —";
            //
            // pnlCompare  (hidden until "Compare changes" is clicked)
            //
            this.pnlCompare.BackColor = System.Drawing.Color.White;
            this.pnlCompare.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCompare.Controls.Add(this.lblCompareTitle);
            this.pnlCompare.Controls.Add(this.dgvCompare);
            this.pnlCompare.Location = new System.Drawing.Point(26, 306);
            this.pnlCompare.Name = "pnlCompare";
            this.pnlCompare.Size = new System.Drawing.Size(800, 168);
            this.pnlCompare.Visible = false;
            //
            // lblCompareTitle
            //
            this.lblCompareTitle.AutoSize = false;
            this.lblCompareTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblCompareTitle.Location = new System.Drawing.Point(14, 8);
            this.lblCompareTitle.Name = "lblCompareTitle";
            this.lblCompareTitle.Size = new System.Drawing.Size(770, 22);
            this.lblCompareTitle.Text = "Field by field — your edit against the current record";
            //
            // dgvCompare
            //
            this.dgvCompare.AllowUserToAddRows = false;
            this.dgvCompare.AllowUserToDeleteRows = false;
            this.dgvCompare.AutoGenerateColumns = false;
            this.dgvCompare.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCompare.BackColor = System.Drawing.Color.White;
            this.dgvCompare.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colField,
            this.colYours,
            this.colCurrent,
            this.colVerdict});
            this.dgvCompare.Location = new System.Drawing.Point(14, 34);
            this.dgvCompare.MultiSelect = false;
            this.dgvCompare.Name = "dgvCompare";
            this.dgvCompare.ReadOnly = true;
            this.dgvCompare.RowHeadersVisible = false;
            this.dgvCompare.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCompare.Size = new System.Drawing.Size(770, 130);
            //
            // colField
            //
            this.colField.DataPropertyName = "Field";
            this.colField.FillWeight = 20F;
            this.colField.HeaderText = "Field";
            this.colField.Name = "colField";
            this.colField.ReadOnly = true;
            //
            // colYours
            //
            this.colYours.DataPropertyName = "Yours";
            this.colYours.FillWeight = 34F;
            this.colYours.HeaderText = "Your edit";
            this.colYours.Name = "colYours";
            this.colYours.ReadOnly = true;
            //
            // colCurrent
            //
            this.colCurrent.DataPropertyName = "Current";
            this.colCurrent.FillWeight = 34F;
            this.colCurrent.HeaderText = "Current";
            this.colCurrent.Name = "colCurrent";
            this.colCurrent.ReadOnly = true;
            //
            // colVerdict
            //
            this.colVerdict.DataPropertyName = "Verdict";
            this.colVerdict.FillWeight = 12F;
            this.colVerdict.HeaderText = "";
            this.colVerdict.Name = "colVerdict";
            this.colVerdict.ReadOnly = true;
            //
            // ConflictDialog
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.lblIcon);
            this.Controls.Add(this.lblHeadline);
            this.Controls.Add(this.lblExplain);
            this.Controls.Add(this.dgvVersions);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.lblFootnote);
            this.Controls.Add(this.pnlCompare);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConflictDialog";
            this.Size = new System.Drawing.Size(860, 540);
            this.Text = "Work order changed — choose how to continue";
            this.Load += new System.EventHandler(this.ConflictDialog_Load);
            this.pnlActions.ResumeLayout(false);
            this.pnlCompare.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblIcon;
        private Wisej.Web.Label lblHeadline;
        private Wisej.Web.Label lblExplain;
        private Wisej.Web.DataGridView dgvVersions;
        private Wisej.Web.DataGridViewTextBoxColumn colWhich;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colVersion;
        private Wisej.Web.FlowLayoutPanel pnlActions;
        private Wisej.Web.Button btnReload;
        private Wisej.Web.Button btnCompare;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Label lblFootnote;
        private Wisej.Web.Panel pnlCompare;
        private Wisej.Web.Label lblCompareTitle;
        private Wisej.Web.DataGridView dgvCompare;
        private Wisej.Web.DataGridViewTextBoxColumn colField;
        private Wisej.Web.DataGridViewTextBoxColumn colYours;
        private Wisej.Web.DataGridViewTextBoxColumn colCurrent;
        private Wisej.Web.DataGridViewTextBoxColumn colVerdict;
    }
}
