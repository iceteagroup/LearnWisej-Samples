namespace OrderDesk
{
    partial class MainPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.shell = new OrderDesk.Shell.AppShell();
            this.SuspendLayout();
            //
            // shell
            //
            this.shell.Dock = Wisej.Web.DockStyle.Fill;
            this.shell.Location = new System.Drawing.Point(0, 0);
            this.shell.Name = "shell";
            this.shell.Size = new System.Drawing.Size(1024, 640);
            //
            // MainPage
            //
            this.Controls.Add(this.shell);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1024, 640);
            this.Text = "OrderDesk";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private OrderDesk.Shell.AppShell shell;
    }
}
