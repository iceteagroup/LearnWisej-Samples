namespace WisejTrainingApp.Views
{
    partial class CodeReviewView
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
            this.pnlReview = new Wisej.Web.Panel();
            this.labelReviewCard = new Wisej.Web.Label();
            this.btnResetReview = new Wisej.Web.Button();
            this.chkReviewItems = new Wisej.Web.CheckedListBox();
            this.lblFocusTitle = new Wisej.Web.Label();
            this.lblWhereToLook = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlHabits = new Wisej.Web.Panel();
            this.labelHabitsCard = new Wisej.Web.Label();
            this.lstHabits = new Wisej.Web.ListBox();
            this.lblHabitsFooter = new Wisej.Web.Label();
            this.pnlReview.SuspendLayout();
            this.pnlHabits.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlReview  (left card: the five review points)
            //
            this.pnlReview.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlReview.BackColor = System.Drawing.Color.White;
            this.pnlReview.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlReview.Controls.Add(this.labelReviewCard);
            this.pnlReview.Controls.Add(this.btnResetReview);
            this.pnlReview.Controls.Add(this.chkReviewItems);
            this.pnlReview.Controls.Add(this.lblFocusTitle);
            this.pnlReview.Controls.Add(this.lblWhereToLook);
            this.pnlReview.Controls.Add(this.lblStatus);
            this.pnlReview.Location = new System.Drawing.Point(0, 0);
            this.pnlReview.Name = "pnlReview";
            this.pnlReview.Size = new System.Drawing.Size(620, 532);
            //
            // labelReviewCard
            //
            this.labelReviewCard.AutoSize = false;
            this.labelReviewCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelReviewCard.Location = new System.Drawing.Point(24, 14);
            this.labelReviewCard.Name = "labelReviewCard";
            this.labelReviewCard.Size = new System.Drawing.Size(440, 28);
            this.labelReviewCard.Text = "Peer code-review focus  ·  lesson s47 §2";
            //
            // btnResetReview
            //
            this.btnResetReview.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnResetReview.Location = new System.Drawing.Point(486, 12);
            this.btnResetReview.Name = "btnResetReview";
            this.btnResetReview.Size = new System.Drawing.Size(110, 32);
            this.btnResetReview.Text = "Reset";
            this.btnResetReview.Click += new System.EventHandler(this.btnResetReview_Click);
            //
            // chkReviewItems  (select = show where to look; tick = reviewed)
            //
            this.chkReviewItems.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.chkReviewItems.CheckOnClick = true;
            this.chkReviewItems.Location = new System.Drawing.Point(24, 56);
            this.chkReviewItems.Name = "chkReviewItems";
            this.chkReviewItems.Size = new System.Drawing.Size(572, 190);
            this.chkReviewItems.AfterItemCheck += new Wisej.Web.ItemCheckEventHandler(this.chkReviewItems_AfterItemCheck);
            this.chkReviewItems.SelectedIndexChanged += new System.EventHandler(this.chkReviewItems_SelectedIndexChanged);
            //
            // lblFocusTitle
            //
            this.lblFocusTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblFocusTitle.AutoSize = false;
            this.lblFocusTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblFocusTitle.Location = new System.Drawing.Point(24, 262);
            this.lblFocusTitle.Name = "lblFocusTitle";
            this.lblFocusTitle.Size = new System.Drawing.Size(572, 26);
            this.lblFocusTitle.Text = "Select a review point";
            //
            // lblWhereToLook
            //
            this.lblWhereToLook.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblWhereToLook.AutoSize = false;
            this.lblWhereToLook.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblWhereToLook.Font = new System.Drawing.Font("monospace", 9F);
            this.lblWhereToLook.ForeColor = System.Drawing.Color.FromArgb(40, 52, 70);
            this.lblWhereToLook.Location = new System.Drawing.Point(24, 292);
            this.lblWhereToLook.Name = "lblWhereToLook";
            this.lblWhereToLook.Padding = new Wisej.Web.Padding(12, 10, 12, 10);
            this.lblWhereToLook.Size = new System.Drawing.Size(572, 172);
            this.lblWhereToLook.Text = "";
            this.lblWhereToLook.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.lblStatus.Location = new System.Drawing.Point(24, 476);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(572, 26);
            this.lblStatus.Text = "● 0 of 5 review points covered";
            //
            // pnlHabits  (right card: s47 §1)
            //
            this.pnlHabits.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.pnlHabits.BackColor = System.Drawing.Color.White;
            this.pnlHabits.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlHabits.Controls.Add(this.labelHabitsCard);
            this.pnlHabits.Controls.Add(this.lstHabits);
            this.pnlHabits.Controls.Add(this.lblHabitsFooter);
            this.pnlHabits.Location = new System.Drawing.Point(636, 0);
            this.pnlHabits.Name = "pnlHabits";
            this.pnlHabits.Size = new System.Drawing.Size(396, 532);
            //
            // labelHabitsCard
            //
            this.labelHabitsCard.AutoSize = false;
            this.labelHabitsCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelHabitsCard.Location = new System.Drawing.Point(24, 14);
            this.labelHabitsCard.Name = "labelHabitsCard";
            this.labelHabitsCard.Size = new System.Drawing.Size(348, 28);
            this.labelHabitsCard.Text = "Habits  ·  lesson s47 §1";
            //
            // lstHabits
            //
            this.lstHabits.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstHabits.Font = new System.Drawing.Font("monospace", 9F);
            this.lstHabits.Location = new System.Drawing.Point(24, 56);
            this.lstHabits.Name = "lstHabits";
            this.lstHabits.Size = new System.Drawing.Size(348, 410);
            //
            // lblHabitsFooter
            //
            this.lblHabitsFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblHabitsFooter.AutoSize = false;
            this.lblHabitsFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblHabitsFooter.Location = new System.Drawing.Point(24, 476);
            this.lblHabitsFooter.Name = "lblHabitsFooter";
            this.lblHabitsFooter.Size = new System.Drawing.Size(348, 26);
            this.lblHabitsFooter.Text = "Full checklist: docs/CodeReviewChecklist.md";
            //
            // CodeReviewView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlReview);
            this.Controls.Add(this.pnlHabits);
            this.Name = "CodeReviewView";
            this.Size = new System.Drawing.Size(1032, 532);
            this.pnlReview.ResumeLayout(false);
            this.pnlHabits.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlReview;
        private Wisej.Web.Label labelReviewCard;
        private Wisej.Web.Button btnResetReview;
        private Wisej.Web.CheckedListBox chkReviewItems;
        private Wisej.Web.Label lblFocusTitle;
        private Wisej.Web.Label lblWhereToLook;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlHabits;
        private Wisej.Web.Label labelHabitsCard;
        private Wisej.Web.ListBox lstHabits;
        private Wisej.Web.Label lblHabitsFooter;
    }
}
