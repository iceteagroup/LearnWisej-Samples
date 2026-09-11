namespace WisejTrainingApp.Views
{
    partial class ArchitectureView
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
            this.lblPageTitle = new Wisej.Web.Label();
            this.lblDataFlow = new Wisej.Web.Label();
            this.lblFlow = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblPageTitle
            //
            this.lblPageTitle.AutoSize = true;
            this.lblPageTitle.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(0, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Text = "Architecture";
            //
            // lblDataFlow
            //
            this.lblDataFlow.Font = new System.Drawing.Font("monospace", 10F);
            this.lblDataFlow.Location = new System.Drawing.Point(0, 50);
            this.lblDataFlow.Name = "lblDataFlow";
            this.lblDataFlow.Size = new System.Drawing.Size(600, 180);
            this.lblDataFlow.Text = "Program.cs     ->  starts Window1\r\n" +
                "Window1.cs     ->  shell, navigation, pages\r\n" +
                "Ticket.cs      ->  ticket data model\r\n" +
                "TicketDialog   ->  create/edit form + validation\r\n" +
                "TicketService  ->  CRUD logic and a fake repository\r\n" +
                "DataGridView   ->  displays tickets\r\n" +
                "Theme          ->  polished UI\r\n" +
                "Deployment     ->  release checklist";
            //
            // lblFlow
            //
            this.lblFlow.Location = new System.Drawing.Point(0, 244);
            this.lblFlow.Name = "lblFlow";
            this.lblFlow.Size = new System.Drawing.Size(600, 60);
            this.lblFlow.Text = "The flow: the user clicks Create/Edit, the dialog opens, validation runs, TicketService updates the ticket list, the DataGridView refreshes, and the status bar records what changed.";
            //
            // ArchitectureView
            //
            this.Controls.Add(this.lblFlow);
            this.Controls.Add(this.lblDataFlow);
            this.Controls.Add(this.lblPageTitle);
            this.Name = "ArchitectureView";
            this.Size = new System.Drawing.Size(800, 500);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.Label lblDataFlow;
        private Wisej.Web.Label lblFlow;
    }
}
