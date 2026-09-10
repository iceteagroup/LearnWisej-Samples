namespace OperationsConsole.Sections
{
    partial class LayoutsPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // the page listens to the application's profile events: let go of them when it is removed
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
            this.pnlCommands = new Wisej.Web.Panel();
            this.btnAddCard = new Wisej.Web.Button();
            this.chkNarrowLayout = new Wisej.Web.CheckBox();
            this.chkSimulateServiceFailure = new Wisej.Web.CheckBox();
            this.lblCommandHint = new Wisej.Web.Label();
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
            this.grpAnchor = new Wisej.Web.GroupBox();
            this.pnlAnchorDemo = new Wisej.Web.Panel();
            this.lblAnchorStretch = new Wisej.Web.Label();
            this.btnAnchorRight = new Wisej.Web.Button();
            this.btnAnchorBottom = new Wisej.Web.Button();
            this.lblAnchorFloat = new Wisej.Web.Label();
            this.pnlCommands.SuspendLayout();
            this.tblDemo.SuspendLayout();
            this.grpTable.SuspendLayout();
            this.tblForm.SuspendLayout();
            this.grpFlow.SuspendLayout();
            this.grpAnchor.SuspendLayout();
            this.pnlAnchorDemo.SuspendLayout();
            this.SuspendLayout();
            //
            // recordHeader  (the reusable UserControl · Dock = Top — three properties and one event, nothing else)
            //
            this.recordHeader.Dock = Wisej.Web.DockStyle.Top;
            this.recordHeader.Name = "recordHeader";
            this.recordHeader.RecordCount = 0;
            this.recordHeader.Size = new System.Drawing.Size(940, 58);
            this.recordHeader.TabIndex = 0;
            this.recordHeader.Title = "Layouts";
            this.recordHeader.RefreshRequested += new System.EventHandler(this.recordHeader_RefreshRequested);
            //
            // statusStripLayouts  (the same public surface, a different layout · Dock = Top)
            //
            this.statusStripLayouts.Dock = Wisej.Web.DockStyle.Top;
            this.statusStripLayouts.Name = "statusStripLayouts";
            this.statusStripLayouts.RecordCount = 0;
            this.statusStripLayouts.Size = new System.Drawing.Size(940, 30);
            this.statusStripLayouts.TabIndex = 1;
            this.statusStripLayouts.Title = "Layouts";
            this.statusStripLayouts.RefreshRequested += new System.EventHandler(this.statusStripLayouts_RefreshRequested);
            //
            // pnlCommands  (command row · Dock = Top — every path of this section is one click away)
            //
            this.pnlCommands.BackColor = System.Drawing.Color.White;
            this.pnlCommands.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCommands.Controls.Add(this.btnAddCard);
            this.pnlCommands.Controls.Add(this.chkNarrowLayout);
            this.pnlCommands.Controls.Add(this.chkSimulateServiceFailure);
            this.pnlCommands.Controls.Add(this.lblCommandHint);
            this.pnlCommands.Dock = Wisej.Web.DockStyle.Top;
            this.pnlCommands.Name = "pnlCommands";
            this.pnlCommands.Size = new System.Drawing.Size(940, 50);
            this.pnlCommands.TabIndex = 2;
            //
            // btnAddCard
            //
            this.btnAddCard.AccessibleName = "Add a layout card";
            this.btnAddCard.Location = new System.Drawing.Point(14, 9);
            this.btnAddCard.Name = "btnAddCard";
            this.btnAddCard.Size = new System.Drawing.Size(110, 32);
            this.btnAddCard.TabIndex = 1;
            this.btnAddCard.Text = "Add card";
            this.btnAddCard.ToolTipText = "Adds a layout card — the same command the ToolBar's New button calls.";
            this.btnAddCard.Click += new System.EventHandler(this.btnAddCard_Click);
            //
            // chkNarrowLayout  (the section reflows its own content; the shell reflows the shell)
            //
            this.chkNarrowLayout.AccessibleName = "Narrow layout for this section";
            this.chkNarrowLayout.Location = new System.Drawing.Point(142, 14);
            this.chkNarrowLayout.Name = "chkNarrowLayout";
            this.chkNarrowLayout.Size = new System.Drawing.Size(150, 24);
            this.chkNarrowLayout.TabIndex = 2;
            this.chkNarrowLayout.Text = "Narrow layout";
            this.chkNarrowLayout.ToolTipText = "Stacks the three demos in one column instead of two — the section's own narrow profile.";
            this.chkNarrowLayout.CheckedChanged += new System.EventHandler(this.chkNarrowLayout_CheckedChanged);
            //
            // chkSimulateServiceFailure  (failure path of this section: the save is refused by the service)
            //
            this.chkSimulateServiceFailure.AccessibleName = "Simulate a service failure on save";
            this.chkSimulateServiceFailure.Location = new System.Drawing.Point(302, 14);
            this.chkSimulateServiceFailure.Name = "chkSimulateServiceFailure";
            this.chkSimulateServiceFailure.Size = new System.Drawing.Size(210, 24);
            this.chkSimulateServiceFailure.TabIndex = 3;
            this.chkSimulateServiceFailure.Text = "Simulate service failure";
            this.chkSimulateServiceFailure.ToolTipText = "LayoutCardService.Save() throws: add a card, then Save, and the StatusBar turns red without showing the exception.";
            this.chkSimulateServiceFailure.CheckedChanged += new System.EventHandler(this.chkSimulateServiceFailure_CheckedChanged);
            //
            // lblCommandHint
            //
            this.lblCommandHint.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblCommandHint.AutoSize = false;
            this.lblCommandHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCommandHint.Location = new System.Drawing.Point(530, 14);
            this.lblCommandHint.Name = "lblCommandHint";
            this.lblCommandHint.Size = new System.Drawing.Size(396, 24);
            this.lblCommandHint.Text = "New / Refresh / Save on the ToolBar call the same methods.";
            this.lblCommandHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tblDemo  (the frame of the section · Dock = Fill — added FIRST so the Top strips claim their edges)
            //
            this.tblDemo.ColumnCount = 2;
            this.tblDemo.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 50F));
            this.tblDemo.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 50F));
            this.tblDemo.Controls.Add(this.grpTable, 0, 0);
            this.tblDemo.Controls.Add(this.grpFlow, 1, 0);
            this.tblDemo.Controls.Add(this.grpAnchor, 0, 1);
            this.tblDemo.Dock = Wisej.Web.DockStyle.Fill;
            this.tblDemo.Name = "tblDemo";
            this.tblDemo.Padding = new Wisej.Web.Padding(10, 10, 10, 10);
            this.tblDemo.RowCount = 2;
            this.tblDemo.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 52F));
            this.tblDemo.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 48F));
            this.tblDemo.SetColumnSpan(this.grpAnchor, 2);
            this.tblDemo.Size = new System.Drawing.Size(940, 566);
            this.tblDemo.TabIndex = 3;
            //
            // grpTable
            //
            this.grpTable.BackColor = System.Drawing.Color.White;
            this.grpTable.Controls.Add(this.tblForm);
            this.grpTable.Dock = Wisej.Web.DockStyle.Fill;
            this.grpTable.Margin = new Wisej.Web.Padding(6, 6, 6, 6);
            this.grpTable.Name = "grpTable";
            this.grpTable.Text = "TableLayoutPanel — aligned labels and editors";
            //
            // tblForm  (fixed label column, editor column Percent 100: the labels never move, the editors resize)
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
            this.txtRegion.ToolTipText = "The region of the console the selected card describes.";
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
            this.txtContainer.ToolTipText = "The container that was chosen for it.";
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
            this.txtRule.ToolTipText = "The layout decision rule behind the choice.";
            //
            // grpFlow
            //
            this.grpFlow.BackColor = System.Drawing.Color.White;
            this.grpFlow.Controls.Add(this.flowChips);
            this.grpFlow.Dock = Wisej.Web.DockStyle.Fill;
            this.grpFlow.Margin = new Wisej.Web.Padding(6, 6, 6, 6);
            this.grpFlow.Name = "grpFlow";
            this.grpFlow.Text = "FlowLayoutPanel — a collection that wraps";
            //
            // flowChips  (filled at runtime from LayoutCardService — the chips wrap instead of being clipped)
            //
            this.flowChips.AutoScroll = true;
            this.flowChips.Dock = Wisej.Web.DockStyle.Fill;
            this.flowChips.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.flowChips.Name = "flowChips";
            this.flowChips.Padding = new Wisej.Web.Padding(8, 8, 8, 8);
            this.flowChips.TabIndex = 20;
            this.flowChips.WrapContents = true;
            //
            // grpAnchor
            //
            this.grpAnchor.BackColor = System.Drawing.Color.White;
            this.grpAnchor.Controls.Add(this.pnlAnchorDemo);
            this.grpAnchor.Dock = Wisej.Web.DockStyle.Fill;
            this.grpAnchor.Margin = new Wisej.Web.Padding(6, 6, 6, 6);
            this.grpAnchor.Name = "grpAnchor";
            this.grpAnchor.Text = "Anchoring — inside a region: drag the splitter and watch the edges";
            //
            // pnlAnchorDemo
            //
            this.pnlAnchorDemo.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.pnlAnchorDemo.Controls.Add(this.lblAnchorStretch);
            this.pnlAnchorDemo.Controls.Add(this.btnAnchorRight);
            this.pnlAnchorDemo.Controls.Add(this.btnAnchorBottom);
            this.pnlAnchorDemo.Controls.Add(this.lblAnchorFloat);
            this.pnlAnchorDemo.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlAnchorDemo.Name = "pnlAnchorDemo";
            //
            // lblAnchorStretch  (Top | Left | Right — changes width with the panel)
            //
            this.lblAnchorStretch.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblAnchorStretch.AutoSize = false;
            this.lblAnchorStretch.BackColor = System.Drawing.Color.FromArgb(234, 243, 255);
            this.lblAnchorStretch.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.lblAnchorStretch.Location = new System.Drawing.Point(14, 14);
            this.lblAnchorStretch.Name = "lblAnchorStretch";
            this.lblAnchorStretch.Size = new System.Drawing.Size(760, 34);
            this.lblAnchorStretch.Text = "  Anchor = Top | Left | Right — I change width with the panel";
            this.lblAnchorStretch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnAnchorRight  (Top | Right — keeps its width and its distance from the right edge)
            //
            this.btnAnchorRight.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnAnchorRight.Location = new System.Drawing.Point(624, 60);
            this.btnAnchorRight.Name = "btnAnchorRight";
            this.btnAnchorRight.Size = new System.Drawing.Size(150, 32);
            this.btnAnchorRight.TabIndex = 30;
            this.btnAnchorRight.Text = "Top | Right";
            this.btnAnchorRight.ToolTipText = "Anchored Top and Right: same width, same gap to the right edge.";
            this.btnAnchorRight.Click += new System.EventHandler(this.btnAnchorRight_Click);
            //
            // btnAnchorBottom  (Bottom | Right — stays in the corner)
            //
            this.btnAnchorBottom.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.btnAnchorBottom.Location = new System.Drawing.Point(624, 150);
            this.btnAnchorBottom.Name = "btnAnchorBottom";
            this.btnAnchorBottom.Size = new System.Drawing.Size(150, 32);
            this.btnAnchorBottom.TabIndex = 31;
            this.btnAnchorBottom.Text = "Bottom | Right";
            this.btnAnchorBottom.ToolTipText = "Anchored Bottom and Right: it stays in the bottom-right corner.";
            this.btnAnchorBottom.Click += new System.EventHandler(this.btnAnchorBottom_Click);
            //
            // lblAnchorFloat  (no anchor — floats, keeping its relative placement)
            //
            this.lblAnchorFloat.AutoSize = false;
            this.lblAnchorFloat.BackColor = System.Drawing.Color.FromArgb(255, 245, 226);
            this.lblAnchorFloat.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.lblAnchorFloat.Location = new System.Drawing.Point(14, 60);
            this.lblAnchorFloat.Name = "lblAnchorFloat";
            this.lblAnchorFloat.Size = new System.Drawing.Size(330, 34);
            this.lblAnchorFloat.Text = "  Anchor = none — I float, I keep my place";
            this.lblAnchorFloat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // LayoutsPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            // Docking is applied from the LAST added control to the FIRST: the Fill frame goes in first,
            // then the Top strips in bottom-to-top order (commands, status strip, record header).
            this.Controls.Add(this.tblDemo);
            this.Controls.Add(this.pnlCommands);
            this.Controls.Add(this.statusStripLayouts);
            this.Controls.Add(this.recordHeader);
            this.Name = "LayoutsPage";
            this.Size = new System.Drawing.Size(940, 704);
            this.Load += new System.EventHandler(this.LayoutsPage_Load);
            this.pnlCommands.ResumeLayout(false);
            this.tblDemo.ResumeLayout(false);
            this.grpTable.ResumeLayout(false);
            this.tblForm.ResumeLayout(false);
            this.grpFlow.ResumeLayout(false);
            this.grpAnchor.ResumeLayout(false);
            this.pnlAnchorDemo.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // the two reusable UserControls (Shell/RecordHeader, Shell/StatusStrip)
        private OperationsConsole.Shell.RecordHeader recordHeader;
        private OperationsConsole.Shell.StatusStrip statusStripLayouts;

        // command row
        private Wisej.Web.Panel pnlCommands;
        private Wisej.Web.Button btnAddCard;
        private Wisej.Web.CheckBox chkNarrowLayout;
        private Wisej.Web.CheckBox chkSimulateServiceFailure;
        private Wisej.Web.Label lblCommandHint;

        // the layout demo
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
        private Wisej.Web.GroupBox grpAnchor;
        private Wisej.Web.Panel pnlAnchorDemo;
        private Wisej.Web.Label lblAnchorStretch;
        private Wisej.Web.Button btnAnchorRight;
        private Wisej.Web.Button btnAnchorBottom;
        private Wisej.Web.Label lblAnchorFloat;
    }
}
