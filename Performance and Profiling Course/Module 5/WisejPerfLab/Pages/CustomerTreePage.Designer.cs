namespace WisejPerfLab.Pages
{
    partial class CustomerTreePage
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
            this.btnLoadTree = new Wisej.Web.Button();
            this.btnExpandFirst = new Wisej.Web.Button();
            this.treeView1 = new Wisej.Web.TreeView();
            this.lblTreeStatus = new Wisej.Web.Label();
            this.lblScenario = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // btnLoadTree
            //
            this.btnLoadTree.Location = new System.Drawing.Point(14, 10);
            this.btnLoadTree.Name = "btnLoadTree";
            this.btnLoadTree.Size = new System.Drawing.Size(190, 34);
            this.btnLoadTree.TabIndex = 0;
            this.btnLoadTree.Text = "Load the customer tree";
            this.btnLoadTree.Click += this.btnLoadTree_Click;
            //
            // btnExpandFirst
            //
            this.btnExpandFirst.Location = new System.Drawing.Point(212, 10);
            this.btnExpandFirst.Name = "btnExpandFirst";
            this.btnExpandFirst.Size = new System.Drawing.Size(190, 34);
            this.btnExpandFirst.TabIndex = 1;
            this.btnExpandFirst.Text = "Expand the first branch";
            this.btnExpandFirst.Click += this.btnExpandFirst_Click;
            //
            // treeView1
            //
            this.treeView1.Location = new System.Drawing.Point(14, 54);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(846, 312);
            this.treeView1.TabIndex = 2;
            this.treeView1.BeforeExpand += this.treeView1_BeforeExpand;
            //
            // lblTreeStatus
            //
            this.lblTreeStatus.Font = new System.Drawing.Font("monospace", 9F);
            this.lblTreeStatus.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTreeStatus.Location = new System.Drawing.Point(14, 374);
            this.lblTreeStatus.Name = "lblTreeStatus";
            this.lblTreeStatus.Size = new System.Drawing.Size(846, 18);
            this.lblTreeStatus.TabIndex = 3;
            this.lblTreeStatus.Text = "tree not loaded yet";
            //
            // lblScenario
            //
            this.lblScenario.Font = new System.Drawing.Font("monospace", 8F);
            this.lblScenario.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblScenario.Location = new System.Drawing.Point(14, 398);
            this.lblScenario.Name = "lblScenario";
            this.lblScenario.Size = new System.Drawing.Size(846, 32);
            this.lblScenario.TabIndex = 4;
            this.lblScenario.Text = "scenarios Customers/LoadTree and Customers/ExpandNode — budget 150 ms / 100 ms — Module 5: roots only, children on expand";
            //
            // CustomerTreePage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblScenario);
            this.Controls.Add(this.lblTreeStatus);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.btnExpandFirst);
            this.Controls.Add(this.btnLoadTree);
            this.Name = "CustomerTreePage";
            this.Size = new System.Drawing.Size(876, 440);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Button btnLoadTree;
        private Wisej.Web.Button btnExpandFirst;
        private Wisej.Web.TreeView treeView1;
        private Wisej.Web.Label lblTreeStatus;
        private Wisej.Web.Label lblScenario;
    }
}
