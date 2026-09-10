namespace AdaptiveOps.Dialogs
{
    partial class TicketEditorForm
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
            this.SuspendLayout();
            //
            // TicketEditorForm
            //
            // A plain dialog: caption bar with the close glyph, no maximize/minimize, centred. On the Phone
            // profile the page sets WindowState = Maximized before ShowDialog so the editor fills the screen.
            //
            this.ClientSize = new System.Drawing.Size(420, 620);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TicketEditorForm";
            this.Padding = new Wisej.Web.Padding(8);
            this.ShowInTaskbar = false;
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "Ticket details";
            this.ResumeLayout(false);
        }

        #endregion
    }
}
