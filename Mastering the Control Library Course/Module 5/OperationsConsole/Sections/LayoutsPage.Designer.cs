namespace OperationsConsole.Sections
{
    partial class LayoutsPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Wisej.Web.Application.ResponsiveProfileChanged -= this.Application_ResponsiveProfileChanged;

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.recordHeader = new OperationsConsole.Shell.RecordHeader();
            this.statusStripLayouts = new OperationsConsole.Shell.StatusStrip();
            this.pnlOptions = new Wisej.Web.Panel();
            this.chkSimulateServiceFailure = new Wisej.Web.CheckBox();
            this.tblDemo = new Wisej.Web.TableLayoutPanel();
            this.grpTable = new Wisej.Web.GroupBox();
            this.tblForm = new Wisej.Web.TableLayoutPanel();
            this.lblRegionCaption = new Wisej.Web.Label();
            this.txtRegion = new Wisej.Web.TextBox();
            this.lblContainerCaption = new Wisej.Web.Label();
            this.txtContainer = new Wisej.Web.TextBox();
            this.lblRuleCaption = new Wisej.Web.Label();
            this.txtRule = new Wisej.Web.TextBox();
            this.grpFlow = new Wisej.Web.GroupBox();
            this.flowChips = new Wisej.Web.FlowLayoutPanel();
            this.pnlOptions.SuspendLayout();
            this.tblDemo.SuspendLayout();
            this.grpTable.SuspendLayout();
            this.tblForm.SuspendLayout();
            this.grpFlow.SuspendLayout();
            this.SuspendLayout();
            //
            // recordHeader
            //
            this.recordHeader.Dock = Wisej.Web.DockStyle.Top;
            this.recordHeader.Name = "recordHeader";
            this.recordHeader.RecordCount = 0;
            this.recordHeader.Size = new System.Drawing.Size(940, 58);
            this.recordHeader.TabIndex = 0;
            this.recordHeader.Title = "Layouts";
            this.recordHeader.RefreshRequested += new System.EventHandler(this.recordHeader_RefreshRequested);
            //
            // statusStripLayouts
            //
            this.statusStripLayouts.Dock = Wisej.Web.DockStyle.Top;
            this.statusStripLayouts.Name = "statusStripLayouts";
            this.statusStripLayouts.RecordCount = 0;
            this.statusStripLayouts.Size = new System.Drawing.Size(940, 30);
            this.statusStripLayouts.TabIndex = 1;
            this.statusStripLayouts.Title = "Layouts";
            this.statusStripLayouts.RefreshRequested += new System.EventHandler(this.statusStripLayouts_RefreshRequested);
            //
            // pnlOptions
            //
            this.pnlOptions.BackColor = System.Drawing.Color.White;
            this.pnlOptions.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlOptions.Controls.Add(this.chkSimulateServiceFailure);
            this.pnlOptions.Dock = Wisej.Web.DockStyle.Top;
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(940, 44);
            this.pnlOptions.TabIndex = 2;
            //
            // chkSimulateServiceFailure
            //
            this.chkSimulateServiceFailure.AccessibleName = "Simulate a service failure on save";
            this.chkSimulateServiceFailure.Location = new System.Drawing.Point(14, 10);
            this.chkSimulateServiceFailure.Name = "chkSimulateServiceFailure";
            this.chkSimulateServiceFailure.Size = new System.Drawing.Size(210, 24);
            this.chkSimulateServiceFailure.TabIndex = 3;
            this.chkSimulateServiceFailure.Text = "Simulate service failure";
            this.chkSimulateServiceFailure.CheckedChanged += new System.EventHandler(this.chkSimulateServiceFailure_CheckedChanged);
            //
            // tblDemo
            //
            this.tblDemo.ColumnCount = 2;
            this.tblDemo.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 50F));
            this.tblDemo.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 50F));
            this.tblDemo.Controls.Add(this.grpTable, 0, 0);
            this.tblDemo.Controls.Add(this.grpFlow, 1, 0);
            this.tblDemo.Dock = Wisej.Web.DockStyle.Fill;
            this.tblDemo.Name = "tblDemo";
            this.tblDemo.Padding = new Wisej.Web.Padding(10, 10, 10, 10);
            this.tblDemo.RowCount = 1;
            this.tblDemo.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
            this.tblDemo.Size = new System.Drawing.Size(940, 572);
            this.tblDemo.TabIndex = 4;
            //
            // grpTable
            //
            this.grpTable.BackColor = System.Drawing.Color.White;
            this.grpTable.Controls.Add(this.tblForm);
            this.grpTable.Dock = Wisej.Web.DockStyle.Fill;
            this.grpTable.Margin = new Wisej.Web.Padding(6, 6, 6, 6);
            this.grpTable.Name = "grpTable";
            this.grpTable.Text = "Selected card";
            //
            // tblForm
            //
            this.tblForm.ColumnCount = 2;
            this.tblForm.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Absolute, 110F));
            this.tblForm.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 100F));
            this.tblForm.Controls.Add(this.lblRegionCaption, 0, 0);
            this.tblForm.Controls.Add(this.txtRegion, 1, 0);
            this.tblForm.Controls.Add(this.lblContainerCaption, 0, 1);
            this.tblForm.Controls.Add(this.txtContainer, 1, 1);
            this.tblForm.Controls.Add(this.lblRuleCaption, 0, 2);
            this.tblForm.Controls.Add(this.txtRule, 1, 2);
            this.tblForm.Dock = Wisej.Web.DockStyle.Fill;
            this.tblForm.Name = "tblForm";
            this.tblForm.Padding = new Wisej.Web.Padding(10, 10, 10, 10);
            this.tblForm.RowCount = 4;
            this.tblForm.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 42F));
            this.tblForm.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 42F));
            this.tblForm.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 42F));
            this.tblForm.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
            //
            // lblRegionCaption
            //
            this.lblRegionCaption.AutoSize = false;
            this.lblRegionCaption.Dock = Wisej.Web.DockStyle.Fill;
            this.lblRegionCaption.Name = "lblRegionCaption";
            this.lblRegionCaption.Text = "Region";
            this.lblRegionCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtRegion
            //
            this.txtRegion.Anchor = Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtRegion.Name = "txtRegion";
            this.txtRegion.ReadOnly = true;
            this.txtRegion.Size = new System.Drawing.Size(280, 30);
            this.txtRegion.TabIndex = 10;
            //
            // lblContainerCaption
            //
            this.lblContainerCaption.AutoSize = false;
            this.lblContainerCaption.Dock = Wisej.Web.DockStyle.Fill;
            this.lblContainerCaption.Name = "lblContainerCaption";
            this.lblContainerCaption.Text = "Container";
            this.lblContainerCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtContainer
            //
            this.txtContainer.Anchor = Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtContainer.Name = "txtContainer";
            this.txtContainer.ReadOnly = true;
            this.txtContainer.Size = new System.Drawing.Size(280, 30);
            this.txtContainer.TabIndex = 11;
            //
            // lblRuleCaption
            //
            this.lblRuleCaption.AutoSize = false;
            this.lblRuleCaption.Dock = Wisej.Web.DockStyle.Fill;
            this.lblRuleCaption.Name = "lblRuleCaption";
            this.lblRuleCaption.Text = "Why";
            this.lblRuleCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtRule
            //
            this.txtRule.Anchor = Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtRule.Name = "txtRule";
            this.txtRule.ReadOnly = true;
            this.txtRule.Size = new System.Drawing.Size(280, 30);
            this.txtRule.TabIndex = 12;
            //
            // grpFlow
            //
            this.grpFlow.BackColor = System.Drawing.Color.White;
            this.grpFlow.Controls.Add(this.flowChips);
            this.grpFlow.Dock = Wisej.Web.DockStyle.Fill;
            this.grpFlow.Margin = new Wisej.Web.Padding(6, 6, 6, 6);
            this.grpFlow.Name = "grpFlow";
            this.grpFlow.Text = "Cards";
            //
            // flowChips
            //
            this.flowChips.AutoScroll = true;
            this.flowChips.Dock = Wisej.Web.DockStyle.Fill;
            this.flowChips.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.flowChips.Name = "flowChips";
            this.flowChips.Padding = new Wisej.Web.Padding(8, 8, 8, 8);
            this.flowChips.TabIndex = 20;
            this.flowChips.WrapContents = true;
            //
            // LayoutsPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.tblDemo);
            this.Controls.Add(this.pnlOptions);
            this.Controls.Add(this.statusStripLayouts);
            this.Controls.Add(this.recordHeader);
            this.Name = "LayoutsPage";
            this.Size = new System.Drawing.Size(940, 704);
            this.Load += new System.EventHandler(this.LayoutsPage_Load);
            this.pnlOptions.ResumeLayout(false);
            this.tblDemo.ResumeLayout(false);
            this.grpTable.ResumeLayout(false);
            this.tblForm.ResumeLayout(false);
            this.grpFlow.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private OperationsConsole.Shell.RecordHeader recordHeader;
        private OperationsConsole.Shell.StatusStrip statusStripLayouts;
        private Wisej.Web.Panel pnlOptions;
        private Wisej.Web.CheckBox chkSimulateServiceFailure;
        private Wisej.Web.TableLayoutPanel tblDemo;
        private Wisej.Web.GroupBox grpTable;
        private Wisej.Web.TableLayoutPanel tblForm;
        private Wisej.Web.Label lblRegionCaption;
        private Wisej.Web.TextBox txtRegion;
        private Wisej.Web.Label lblContainerCaption;
        private Wisej.Web.TextBox txtContainer;
        private Wisej.Web.Label lblRuleCaption;
        private Wisej.Web.TextBox txtRule;
        private Wisej.Web.GroupBox grpFlow;
        private Wisej.Web.FlowLayoutPanel flowChips;
    }
}
