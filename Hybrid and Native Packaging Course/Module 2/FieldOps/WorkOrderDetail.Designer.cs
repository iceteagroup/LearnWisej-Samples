using System.Drawing;
using Wisej.Web;

namespace FieldOps
{
    partial class WorkOrderDetail
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
            this.pnlAppBar = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.btnBack = new Wisej.Web.Button();
            this.pnlBody = new Wisej.Web.Panel();
            this.lblHeader = new Wisej.Web.Label();
            this.lblFacts = new Wisej.Web.Label();
            this.lblStatusCaption = new Wisej.Web.Label();
            this.pnlStatusChoice = new Wisej.Web.FlowLayoutPanel();
            this.rbAccepted = new Wisej.Web.RadioButton();
            this.rbOnSite = new Wisej.Web.RadioButton();
            this.rbCompleted = new Wisej.Web.RadioButton();
            this.btnSaveStatus = new Wisej.Web.Button();
            this.lblMessage = new Wisej.Web.Label();
            this.lblPhotoCaption = new Wisej.Web.Label();
            this.pnlPhoto = new Wisej.Web.FlowLayoutPanel();
            this.btnTakePhoto = new Wisej.Web.Button();
            this.uploadPhoto = new Wisej.Web.Upload();
            this.lblPhotos = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // ── app bar: back arrow + "FieldOps · WO-nnnn" ───────────────────────────────────
            //
            this.btnBack.Name = "btnBack";
            this.btnBack.Text = "←";
            this.btnBack.Dock = Wisej.Web.DockStyle.Left;
            this.btnBack.Size = new Size(44, 40);
            this.btnBack.BackColor = Ops.Accent;
            this.btnBack.ForeColor = Color.White;
            this.btnBack.Font = Ops.Px(18, FontStyle.Bold);
            this.btnBack.Display = Wisej.Web.Display.Label;
            this.btnBack.ToolTipText = "Back to my work orders";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);

            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.AllowHtml = true;
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblAppTitle.ForeColor = Color.White;
            this.lblAppTitle.Font = Ops.Px(14.5f, FontStyle.Bold);
            this.lblAppTitle.TextAlign = ContentAlignment.MiddleLeft;

            this.pnlAppBar.Name = "pnlAppBar";
            this.pnlAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlAppBar.Size = new Size(820, 40);
            this.pnlAppBar.BackColor = Ops.Accent;
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            this.pnlAppBar.Controls.Add(this.btnBack);
            //
            // ── body ─────────────────────────────────────────────────────────────────────────
            //
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.AllowHtml = true;
            this.lblHeader.AutoSize = false;
            this.lblHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblHeader.Size = new Size(780, 58);

            this.lblFacts.Name = "lblFacts";
            this.lblFacts.AllowHtml = true;
            this.lblFacts.AutoSize = false;
            this.lblFacts.Dock = Wisej.Web.DockStyle.Top;
            this.lblFacts.Size = new Size(780, 112);

            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Text = "UPDATE STATUS";
            this.lblStatusCaption.AutoSize = false;
            this.lblStatusCaption.Dock = Wisej.Web.DockStyle.Top;
            this.lblStatusCaption.Size = new Size(780, 30);
            this.lblStatusCaption.Font = Ops.Px(11, FontStyle.Bold);
            this.lblStatusCaption.ForeColor = Ops.Muted;
            this.lblStatusCaption.TextAlign = ContentAlignment.BottomLeft;

            this.rbAccepted.Name = "rbAccepted";
            this.rbAccepted.Text = "Accepted";
            this.rbAccepted.AutoSize = true;
            this.rbAccepted.Margin = new Wisej.Web.Padding(0, 6, 18, 0);
            this.rbAccepted.Font = Ops.Px(13);

            this.rbOnSite.Name = "rbOnSite";
            this.rbOnSite.Text = "On site";
            this.rbOnSite.AutoSize = true;
            this.rbOnSite.Margin = new Wisej.Web.Padding(0, 6, 18, 0);
            this.rbOnSite.Font = Ops.Px(13);

            this.rbCompleted.Name = "rbCompleted";
            this.rbCompleted.Text = "Completed";
            this.rbCompleted.AutoSize = true;
            this.rbCompleted.Margin = new Wisej.Web.Padding(0, 6, 24, 0);
            this.rbCompleted.Font = Ops.Px(13);

            this.btnSaveStatus.Name = "btnSaveStatus";
            this.btnSaveStatus.Text = "Update status";
            this.btnSaveStatus.AutoSize = true;
            this.btnSaveStatus.MinimumSize = new Size(0, 32);
            this.btnSaveStatus.BackColor = Ops.Accent;
            this.btnSaveStatus.ForeColor = Color.White;
            this.btnSaveStatus.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnSaveStatus.Click += new System.EventHandler(this.btnSaveStatus_Click);

            this.pnlStatusChoice.Name = "pnlStatusChoice";
            this.pnlStatusChoice.Dock = Wisej.Web.DockStyle.Top;
            this.pnlStatusChoice.Size = new Size(780, 44);
            this.pnlStatusChoice.Controls.Add(this.rbAccepted);
            this.pnlStatusChoice.Controls.Add(this.rbOnSite);
            this.pnlStatusChoice.Controls.Add(this.rbCompleted);
            this.pnlStatusChoice.Controls.Add(this.btnSaveStatus);

            this.lblMessage.Name = "lblMessage";
            this.lblMessage.AllowHtml = true;
            this.lblMessage.AutoSize = false;
            this.lblMessage.Dock = Wisej.Web.DockStyle.Top;
            this.lblMessage.Size = new Size(780, 56);
            this.lblMessage.Visible = false;

            this.lblPhotoCaption.Name = "lblPhotoCaption";
            this.lblPhotoCaption.Text = "PHOTOS";
            this.lblPhotoCaption.AutoSize = false;
            this.lblPhotoCaption.Dock = Wisej.Web.DockStyle.Top;
            this.lblPhotoCaption.Size = new Size(780, 30);
            this.lblPhotoCaption.Font = Ops.Px(11, FontStyle.Bold);
            this.lblPhotoCaption.ForeColor = Ops.Muted;
            this.lblPhotoCaption.TextAlign = ContentAlignment.BottomLeft;

            this.btnTakePhoto.Name = "btnTakePhoto";
            this.btnTakePhoto.Text = "Take photo";
            this.btnTakePhoto.AutoSize = true;
            this.btnTakePhoto.MinimumSize = new Size(0, 30);
            this.btnTakePhoto.Margin = new Wisej.Web.Padding(0, 6, 12, 0);
            this.btnTakePhoto.BackColor = Color.White;
            this.btnTakePhoto.ForeColor = Ops.SecondaryInk;
            this.btnTakePhoto.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnTakePhoto.Visible = false;
            this.btnTakePhoto.Click += new System.EventHandler(this.btnTakePhoto_Click);

            this.uploadPhoto.Name = "uploadPhoto";
            this.uploadPhoto.Text = "Attach photo (upload)";
            this.uploadPhoto.HideValue = true;
            this.uploadPhoto.AllowedFileTypes = "image/*";
            this.uploadPhoto.Size = new Size(196, 30);
            this.uploadPhoto.Margin = new Wisej.Web.Padding(0, 6, 12, 0);
            this.uploadPhoto.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.uploadPhoto.Uploaded += new Wisej.Web.UploadedEventHandler(this.uploadPhoto_Uploaded);

            this.lblPhotos.Name = "lblPhotos";
            this.lblPhotos.AutoSize = true;
            this.lblPhotos.Margin = new Wisej.Web.Padding(0, 12, 0, 0);
            this.lblPhotos.Font = Ops.Px(12.5f);
            this.lblPhotos.ForeColor = Ops.Muted;

            this.pnlPhoto.Name = "pnlPhoto";
            this.pnlPhoto.Dock = Wisej.Web.DockStyle.Top;
            this.pnlPhoto.Size = new Size(780, 44);
            this.pnlPhoto.Controls.Add(this.btnTakePhoto);
            this.pnlPhoto.Controls.Add(this.uploadPhoto);
            this.pnlPhoto.Controls.Add(this.lblPhotos);

            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.AutoScroll = true;
            this.pnlBody.Padding = new Wisej.Web.Padding(18, 12, 18, 12);
            // Docked Top: the last one added sits highest, so the list below is bottom-to-top.
            this.pnlBody.Controls.Add(this.pnlPhoto);
            this.pnlBody.Controls.Add(this.lblPhotoCaption);
            this.pnlBody.Controls.Add(this.lblMessage);
            this.pnlBody.Controls.Add(this.pnlStatusChoice);
            this.pnlBody.Controls.Add(this.lblStatusCaption);
            this.pnlBody.Controls.Add(this.lblFacts);
            this.pnlBody.Controls.Add(this.lblHeader);
            //
            // WorkOrderDetail
            //
            this.Name = "WorkOrderDetail";
            this.Text = "FieldOps";
            this.BackColor = Color.White;
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlAppBar);
            this.Load += new System.EventHandler(this.WorkOrderDetail_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlAppBar;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Button btnBack;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Label lblHeader;
        private Wisej.Web.Label lblFacts;
        private Wisej.Web.Label lblStatusCaption;
        private Wisej.Web.FlowLayoutPanel pnlStatusChoice;
        private Wisej.Web.RadioButton rbAccepted;
        private Wisej.Web.RadioButton rbOnSite;
        private Wisej.Web.RadioButton rbCompleted;
        private Wisej.Web.Button btnSaveStatus;
        private Wisej.Web.Label lblMessage;
        private Wisej.Web.Label lblPhotoCaption;
        private Wisej.Web.FlowLayoutPanel pnlPhoto;
        private Wisej.Web.Button btnTakePhoto;
        private Wisej.Web.Upload uploadPhoto;
        private Wisej.Web.Label lblPhotos;
    }
}
