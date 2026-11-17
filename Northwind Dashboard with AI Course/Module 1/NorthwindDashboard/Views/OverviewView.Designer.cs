namespace NorthwindDashboard.Views
{
	partial class OverviewView
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
			this.revenueKpi = new Wisej.Web.KpiPanel();
			this.ordersKpi = new Wisej.Web.KpiPanel();
			this.averageKpi = new Wisej.Web.KpiPanel();
			this.periodLabel = new Wisej.Web.Label();
			this.SuspendLayout();
			//
			// revenueKpi
			//
			this.revenueKpi.Location = new System.Drawing.Point(0, 0);
			this.revenueKpi.Name = "revenueKpi";
			this.revenueKpi.Size = new System.Drawing.Size(250, 104);
			this.revenueKpi.TabIndex = 0;
			this.revenueKpi.Title = "Revenue";
			this.revenueKpi.TrendFormat = "0.0\'%\';0.0\'%\';0\'%\'";
			this.revenueKpi.Value = "$0.00";
			//
			// ordersKpi
			//
			this.ordersKpi.Location = new System.Drawing.Point(262, 0);
			this.ordersKpi.Name = "ordersKpi";
			this.ordersKpi.Size = new System.Drawing.Size(250, 104);
			this.ordersKpi.TabIndex = 1;
			this.ordersKpi.Title = "Orders";
			this.ordersKpi.TrendFormat = "0.0\'%\';0.0\'%\';0\'%\'";
			this.ordersKpi.Value = "0";
			//
			// averageKpi
			//
			this.averageKpi.Location = new System.Drawing.Point(524, 0);
			this.averageKpi.Name = "averageKpi";
			this.averageKpi.Size = new System.Drawing.Size(250, 104);
			this.averageKpi.TabIndex = 2;
			this.averageKpi.Title = "Average order";
			this.averageKpi.TrendFormat = "0.0\'%\';0.0\'%\';0\'%\'";
			this.averageKpi.Value = "$0.00";
			//
			// periodLabel
			//
			this.periodLabel.AppearanceKey = "nw-subtitle";
			this.periodLabel.AutoSize = true;
			this.periodLabel.Location = new System.Drawing.Point(0, 120);
			this.periodLabel.Name = "periodLabel";
			this.periodLabel.Size = new System.Drawing.Size(280, 18);
			this.periodLabel.TabIndex = 3;
			this.periodLabel.Text = "Period label: Last 30 days · 1998-04-07 to 1998-05-06";
			//
			// OverviewView
			//
			this.AppearanceKey = "nw-view";
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = Wisej.Web.AutoScaleMode.Font;
			this.Controls.Add(this.periodLabel);
			this.Controls.Add(this.averageKpi);
			this.Controls.Add(this.ordersKpi);
			this.Controls.Add(this.revenueKpi);
			this.Name = "OverviewView";
			this.Size = new System.Drawing.Size(1206, 752);
			this.Text = "Overview";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Wisej.Web.KpiPanel revenueKpi;
		private Wisej.Web.KpiPanel ordersKpi;
		private Wisej.Web.KpiPanel averageKpi;
		private Wisej.Web.Label periodLabel;
	}
}
