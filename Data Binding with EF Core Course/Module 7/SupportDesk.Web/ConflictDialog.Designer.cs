namespace SupportDesk.Web
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
            this.labelTitle = new Wisej.Web.Label();
            this.labelExplanation = new Wisej.Web.Label();
            this.labelPolicy = new Wisej.Web.Label();
            this.conflictGridView = new Wisej.Web.DataGridView();
            this.colField = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colYourValue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDatabaseValue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOriginalValue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.btnCancel = new Wisej.Web.Button();
            this.btnReload = new Wisej.Web.Button();
            this.btnOverwrite = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(20, 16);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(560, 26);
            this.labelTitle.Text = "This ticket changed while you were editing it";
            //
            // labelExplanation  (set from the ConflictSet in the constructor)
            //
            this.labelExplanation.AutoSize = false;
            this.labelExplanation.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelExplanation.Location = new System.Drawing.Point(20, 46);
            this.labelExplanation.Name = "labelExplanation";
            this.labelExplanation.Size = new System.Drawing.Size(560, 40);
            this.labelExplanation.Text = "";
            this.labelExplanation.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // conflictGridView  (the lab control — Field / Your value / Database value / Original value)
            //
            this.conflictGridView.AllowUserToAddRows = false;
            this.conflictGridView.AllowUserToDeleteRows = false;
            this.conflictGridView.AutoGenerateColumns = false;
            this.conflictGridView.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.conflictGridView.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colField,
            this.colYourValue,
            this.colDatabaseValue,
            this.colOriginalValue});
            this.conflictGridView.Location = new System.Drawing.Point(20, 92);
            this.conflictGridView.MultiSelect = false;
            this.conflictGridView.Name = "conflictGridView";
            this.conflictGridView.ReadOnly = true;
            this.conflictGridView.RowHeadersVisible = false;
            this.conflictGridView.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.conflictGridView.Size = new System.Drawing.Size(560, 220);
            //
            // colField
            //
            this.colField.DataPropertyName = "Field";
            this.colField.FillWeight = 150F;
            this.colField.HeaderText = "Field";
            this.colField.Name = "colField";
            this.colField.Width = 150;
            //
            // colYourValue
            //
            this.colYourValue.DataPropertyName = "YourValue";
            this.colYourValue.FillWeight = 140F;
            this.colYourValue.HeaderText = "Your value";
            this.colYourValue.Name = "colYourValue";
            this.colYourValue.Width = 140;
            //
            // colDatabaseValue
            //
            this.colDatabaseValue.DataPropertyName = "DatabaseValue";
            this.colDatabaseValue.FillWeight = 140F;
            this.colDatabaseValue.HeaderText = "Database value";
            this.colDatabaseValue.Name = "colDatabaseValue";
            this.colDatabaseValue.Width = 140;
            //
            // colOriginalValue
            //
            this.colOriginalValue.DataPropertyName = "OriginalValue";
            this.colOriginalValue.FillWeight = 130F;
            this.colOriginalValue.HeaderText = "Original value";
            this.colOriginalValue.Name = "colOriginalValue";
            this.colOriginalValue.Width = 130;
            //
            // labelPolicy  (set from ConflictResolution.CanOverwrite in the constructor)
            //
            this.labelPolicy.AutoSize = false;
            this.labelPolicy.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Italic);
            this.labelPolicy.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelPolicy.Location = new System.Drawing.Point(20, 320);
            this.labelPolicy.Name = "labelPolicy";
            this.labelPolicy.Size = new System.Drawing.Size(560, 22);
            this.labelPolicy.Text = "";
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(20, 356);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 36);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.ToolTipText = "Leaves the editor exactly as it is — your edits are still there, nothing is saved.";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnReload  (always offered)
            //
            this.btnReload.Location = new System.Drawing.Point(340, 356);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(100, 36);
            this.btnReload.Text = "Reload";
            this.btnReload.ToolTipText = "Discards your edits and loads the ticket fresh — the database's current values and the new RowVersion.";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            //
            // btnOverwrite  (visible only when ConflictResolution.CanOverwrite(role) is true)
            //
            this.btnOverwrite.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnOverwrite.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.btnOverwrite.Location = new System.Drawing.Point(452, 356);
            this.btnOverwrite.Name = "btnOverwrite";
            this.btnOverwrite.Size = new System.Drawing.Size(128, 36);
            this.btnOverwrite.Text = "Overwrite";
            this.btnOverwrite.ToolTipText = "Saves your values anyway: the database's current RowVersion becomes the OriginalValue and SaveChangesAsync runs again. This discards the other operator's change — offered to a Supervisor only.";
            this.btnOverwrite.Click += new System.EventHandler(this.btnOverwrite_Click);
            //
            // ConflictDialog
            //
            this.AcceptButton = this.btnReload;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.labelExplanation);
            this.Controls.Add(this.conflictGridView);
            this.Controls.Add(this.labelPolicy);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnOverwrite);
            this.FormBorderStyle = Wisej.Web.FormBorderStyle.Fixed;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConflictDialog";
            this.ShowInTaskbar = false;
            this.Size = new System.Drawing.Size(600, 420);
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Conflict";
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Label labelExplanation;
        private Wisej.Web.Label labelPolicy;
        private Wisej.Web.DataGridView conflictGridView;
        private Wisej.Web.DataGridViewTextBoxColumn colField;
        private Wisej.Web.DataGridViewTextBoxColumn colYourValue;
        private Wisej.Web.DataGridViewTextBoxColumn colDatabaseValue;
        private Wisej.Web.DataGridViewTextBoxColumn colOriginalValue;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Button btnReload;
        private Wisej.Web.Button btnOverwrite;
    }
}
