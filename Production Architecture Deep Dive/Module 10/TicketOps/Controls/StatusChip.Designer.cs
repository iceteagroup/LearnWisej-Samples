namespace TicketOps.Controls
{
    partial class StatusChip
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
            this.labelText = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // labelText  (no ForeColor/BackColor: the chip's theme appearance + state paint it)
            //
            this.labelText.AutoSize = false;
            this.labelText.Dock = Wisej.Web.DockStyle.Fill;
            this.labelText.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelText.Name = "labelText";
            this.labelText.Text = "Open";
            this.labelText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // StatusChip  (padding & radius fixed in one place: the "chip" appearance)
            //
            this.Controls.Add(this.labelText);
            this.Name = "StatusChip";
            this.Size = new System.Drawing.Size(132, 26);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label labelText;
    }
}
