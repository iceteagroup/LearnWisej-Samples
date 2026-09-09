namespace WisejTrainingApp.Views
{
    partial class DeploymentView
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
            this.components = new System.ComponentModel.Container();
            this.pnlChecklist = new Wisej.Web.Panel();
            this.labelChecklistCard = new Wisej.Web.Label();
            this.btnCheckAll = new Wisej.Web.Button();
            this.btnResetChecklist = new Wisej.Web.Button();
            this.chkRequired = new Wisej.Web.CheckedListBox();
            this.lblPackageStatus = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlNotes = new Wisej.Web.Panel();
            this.labelNotesCard = new Wisej.Web.Label();
            this.lblDeploymentNotes = new Wisej.Web.Label();
            this.lblNotesFooter = new Wisej.Web.Label();
            this.pnlChecklist.SuspendLayout();
            this.pnlNotes.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlChecklist  (left card: the nine required checks + the gate)
            //
            this.pnlChecklist.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.pnlChecklist.BackColor = System.Drawing.Color.White;
            this.pnlChecklist.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlChecklist.Controls.Add(this.labelChecklistCard);
            this.pnlChecklist.Controls.Add(this.btnCheckAll);
            this.pnlChecklist.Controls.Add(this.btnResetChecklist);
            this.pnlChecklist.Controls.Add(this.chkRequired);
            this.pnlChecklist.Controls.Add(this.lblPackageStatus);
            this.pnlChecklist.Controls.Add(this.lblStatus);
            this.pnlChecklist.Location = new System.Drawing.Point(0, 0);
            this.pnlChecklist.Name = "pnlChecklist";
            this.pnlChecklist.Size = new System.Drawing.Size(560, 532);
            //
            // labelChecklistCard
            //
            this.labelChecklistCard.AutoSize = false;
            this.labelChecklistCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelChecklistCard.Location = new System.Drawing.Point(24, 14);
            this.labelChecklistCard.Name = "labelChecklistCard";
            this.labelChecklistCard.Size = new System.Drawing.Size(320, 28);
            this.labelChecklistCard.Text = "Required release checklist  ·  s42 §4";
            //
            // btnCheckAll  (recovery)
            //
            this.btnCheckAll.Location = new System.Drawing.Point(350, 12);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(100, 32);
            this.btnCheckAll.Text = "Check all";
            this.btnCheckAll.ToolTipText = "Marks every required item done — the gate opens.";
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            //
            // btnResetChecklist
            //
            this.btnResetChecklist.Location = new System.Drawing.Point(458, 12);
            this.btnResetChecklist.Name = "btnResetChecklist";
            this.btnResetChecklist.Size = new System.Drawing.Size(78, 32);
            this.btnResetChecklist.Text = "Reset";
            this.btnResetChecklist.Click += new System.EventHandler(this.btnResetChecklist_Click);
            //
            // chkRequired
            //
            this.chkRequired.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.chkRequired.CheckOnClick = true;
            this.chkRequired.Location = new System.Drawing.Point(24, 56);
            this.chkRequired.Name = "chkRequired";
            this.chkRequired.Size = new System.Drawing.Size(512, 340);
            this.chkRequired.AfterItemCheck += new Wisej.Web.ItemCheckEventHandler(this.chkRequired_AfterItemCheck);
            //
            // lblPackageStatus  (the gate)
            //
            this.lblPackageStatus.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblPackageStatus.AutoSize = false;
            this.lblPackageStatus.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblPackageStatus.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblPackageStatus.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblPackageStatus.Location = new System.Drawing.Point(24, 412);
            this.lblPackageStatus.Name = "lblPackageStatus";
            this.lblPackageStatus.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblPackageStatus.Size = new System.Drawing.Size(512, 48);
            this.lblPackageStatus.Text = "Package status: NOT READY";
            this.lblPackageStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.lblStatus.Location = new System.Drawing.Point(24, 476);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(512, 26);
            this.lblStatus.Text = "● 0 of 9 required checks done";
            //
            // pnlNotes  (right card: what each check means for this project)
            //
            this.pnlNotes.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlNotes.BackColor = System.Drawing.Color.White;
            this.pnlNotes.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlNotes.Controls.Add(this.labelNotesCard);
            this.pnlNotes.Controls.Add(this.lblDeploymentNotes);
            this.pnlNotes.Controls.Add(this.lblNotesFooter);
            this.pnlNotes.Location = new System.Drawing.Point(576, 0);
            this.pnlNotes.Name = "pnlNotes";
            this.pnlNotes.Size = new System.Drawing.Size(456, 532);
            //
            // labelNotesCard
            //
            this.labelNotesCard.AutoSize = false;
            this.labelNotesCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelNotesCard.Location = new System.Drawing.Point(24, 14);
            this.labelNotesCard.Name = "labelNotesCard";
            this.labelNotesCard.Size = new System.Drawing.Size(408, 28);
            this.labelNotesCard.Text = "Deployment notes  ·  this project";
            //
            // lblDeploymentNotes
            //
            this.lblDeploymentNotes.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblDeploymentNotes.AutoSize = false;
            this.lblDeploymentNotes.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblDeploymentNotes.Font = new System.Drawing.Font("monospace", 8F);
            this.lblDeploymentNotes.ForeColor = System.Drawing.Color.FromArgb(40, 52, 70);
            this.lblDeploymentNotes.Location = new System.Drawing.Point(24, 56);
            this.lblDeploymentNotes.Name = "lblDeploymentNotes";
            this.lblDeploymentNotes.Padding = new Wisej.Web.Padding(12, 10, 12, 10);
            this.lblDeploymentNotes.Size = new System.Drawing.Size(408, 404);
            this.lblDeploymentNotes.Text = "";
            this.lblDeploymentNotes.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // lblNotesFooter
            //
            this.lblNotesFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblNotesFooter.AutoSize = false;
            this.lblNotesFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNotesFooter.Location = new System.Drawing.Point(24, 476);
            this.lblNotesFooter.Name = "lblNotesFooter";
            this.lblNotesFooter.Size = new System.Drawing.Size(408, 26);
            this.lblNotesFooter.Text = "Full notes: docs/DeploymentChecklist.md · docs/ReadinessNote.md";
            //
            // DeploymentView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlChecklist);
            this.Controls.Add(this.pnlNotes);
            this.Name = "DeploymentView";
            this.Size = new System.Drawing.Size(1032, 532);
            this.pnlChecklist.ResumeLayout(false);
            this.pnlNotes.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlChecklist;
        private Wisej.Web.Label labelChecklistCard;
        private Wisej.Web.Button btnCheckAll;
        private Wisej.Web.Button btnResetChecklist;
        private Wisej.Web.CheckedListBox chkRequired;
        private Wisej.Web.Label lblPackageStatus;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlNotes;
        private Wisej.Web.Label labelNotesCard;
        private Wisej.Web.Label lblDeploymentNotes;
        private Wisej.Web.Label lblNotesFooter;
    }
}
