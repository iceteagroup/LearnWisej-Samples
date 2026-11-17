namespace NorthwindDashboard.Views
{
	partial class PlaceholderView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Wisej Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.messageLabel = new Wisej.Web.Label();
			this.SuspendLayout();
			//
			// messageLabel
			//
			this.messageLabel.AppearanceKey = "nw-text";
			this.messageLabel.Dock = Wisej.Web.DockStyle.Fill;
			this.messageLabel.Location = new System.Drawing.Point(0, 0);
			this.messageLabel.Name = "messageLabel";
			this.messageLabel.Size = new System.Drawing.Size(1206, 752);
			this.messageLabel.TabIndex = 0;
			this.messageLabel.Text = "This view is built in a later module.";
			this.messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			// PlaceholderView
			//
			this.AppearanceKey = "nw-view";
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = Wisej.Web.AutoScaleMode.Font;
			this.Controls.Add(this.messageLabel);
			this.Name = "PlaceholderView";
			this.Size = new System.Drawing.Size(1206, 752);
			this.Text = "Placeholder";
			this.ResumeLayout(false);

		}

		#endregion

		private Wisej.Web.Label messageLabel;
	}
}
