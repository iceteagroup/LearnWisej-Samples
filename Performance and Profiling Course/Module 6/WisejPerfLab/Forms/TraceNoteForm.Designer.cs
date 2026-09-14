namespace WisejPerfLab.Forms
{
    partial class TraceNoteForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Designer generated code

        private void InitializeComponent()
        {
            this.lblLead = new Wisej.Web.Label();
            this.txtNote = new Wisej.Web.TextBox();
            this.btnClose = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // lblLead
            //
            this.lblLead.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblLead.Location = new System.Drawing.Point(18, 14);
            this.lblLead.Name = "lblLead";
            this.lblLead.Size = new System.Drawing.Size(600, 34);
            this.lblLead.TabIndex = 0;
            this.lblLead.Text = "Save this beside the .diagsession file, under the same name. A trace nobody can place is a trace nobody will trust in three weeks.";
            //
            // txtNote
            //
            this.txtNote.Font = new System.Drawing.Font("monospace", 9F);
            this.txtNote.Location = new System.Drawing.Point(18, 56);
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.ReadOnly = true;
            this.txtNote.Size = new System.Drawing.Size(600, 280);
            this.txtNote.TabIndex = 1;
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(518, 346);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 34);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.Click += this.btnClose_Click;
            //
            // TraceNoteForm
            //
            this.AcceptButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(636, 394);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtNote);
            this.Controls.Add(this.lblLead);
            this.Name = "TraceNoteForm";
            this.ShowInTaskbar = false;
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Trace note";
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblLead;
        private Wisej.Web.TextBox txtNote;
        private Wisej.Web.Button btnClose;
    }
}
