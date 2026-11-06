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
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblJob = new Wisej.Web.Label();
            this.lblSla = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.flowActions = new Wisej.Web.FlowLayoutPanel();
            this.btnPhoto = new Wisej.Web.Button();
            this.uploadPhoto = new Wisej.Web.Upload();
            this.btnSignature = new Wisej.Web.Button();
            this.btnNavigate = new Wisej.Web.Button();
            this.btnSettings = new Wisej.Web.Button();
            this.lblFootNote = new Wisej.Web.Label();
            this.lblBanner = new Wisej.Web.Label();
            this.pnlMedia = new Wisej.Web.Panel();
            this.pnlPhotos = new Wisej.Web.Panel();
            this.lblPhotoStrip = new Wisej.Web.Label();
            this.lblPhotosCaption = new Wisej.Web.Label();
            this.pnlSignature = new Wisej.Web.Panel();
            this.txtSigner = new Wisej.Web.TextBox();
            this.lblSignaturePad = new Wisej.Web.Label();
            this.lblSignatureCaption = new Wisej.Web.Label();
            this.pnlStatusFlow = new Wisej.Web.FlowLayoutPanel();
            this.lblStatusFlowCaption = new Wisej.Web.Label();
            this.rbAccepted = new Wisej.Web.RadioButton();
            this.rbOnSite = new Wisej.Web.RadioButton();
            this.rbCompleted = new Wisej.Web.RadioButton();
            this.btnSaveStatus = new Wisej.Web.Button();
            this.pnlStatus = new Wisej.Web.Panel();
            this.lblStatusCaption = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.lblCount = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // ── app bar: back + "FieldOps · WO-4187" ─────────────────────────────────────────
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
            this.pnlAppBar.Size = new Size(960, 40);
            this.pnlAppBar.BackColor = Ops.Accent;
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            this.pnlAppBar.Controls.Add(this.btnBack);
            //
            // ── job header: fault / customer · address ... SLA chip ──────────────────────────
            //
            this.lblJob.Name = "lblJob";
            this.lblJob.AllowHtml = true;
            this.lblJob.AutoSize = false;
            this.lblJob.Dock = Wisej.Web.DockStyle.Fill;

            this.lblSla.Name = "lblSla";
            this.lblSla.AllowHtml = true;
            this.lblSla.AutoSize = false;
            this.lblSla.Dock = Wisej.Web.DockStyle.Right;
            this.lblSla.Size = new Size(130, 60);
            this.lblSla.TextAlign = ContentAlignment.MiddleRight;

            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Size = new Size(960, 62);
            this.pnlHeader.Padding = new Wisej.Web.Padding(18, 0, 18, 0);
            this.pnlHeader.BackColor = Ops.Strip;
            this.pnlHeader.CssStyle = "border-bottom:1px solid #e4eaf1";
            this.pnlHeader.Controls.Add(this.lblJob);
            this.pnlHeader.Controls.Add(this.lblSla);
            //
            // ── device actions: each one shown only where it works ──────────────────────────
            //
            this.btnPhoto.Name = "btnPhoto";
            this.btnPhoto.AllowHtml = true;
            this.btnPhoto.Text = "Take Photo <span style=\"font-size:10.5px;opacity:.85;margin-left:6px\">DEVICE</span>";
            this.btnPhoto.AutoSize = true;
            this.btnPhoto.MinimumSize = new Size(0, 34);
            this.btnPhoto.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.btnPhoto.BackColor = Ops.Accent;
            this.btnPhoto.ForeColor = Color.White;
            this.btnPhoto.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnPhoto.Visible = false;
            this.btnPhoto.Click += new System.EventHandler(this.btnPhoto_Click);

            this.uploadPhoto.Name = "uploadPhoto";
            this.uploadPhoto.Text = "Upload Photo";
            this.uploadPhoto.HideValue = true;
            this.uploadPhoto.AllowedFileTypes = "image/*";
            this.uploadPhoto.Size = new Size(150, 34);
            this.uploadPhoto.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.uploadPhoto.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.uploadPhoto.Visible = false;
            this.uploadPhoto.Uploaded += new Wisej.Web.UploadedEventHandler(this.uploadPhoto_Uploaded);

            this.btnSignature.Name = "btnSignature";
            this.btnSignature.Text = "Signature";
            this.btnSignature.AutoSize = true;
            this.btnSignature.MinimumSize = new Size(0, 34);
            this.btnSignature.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.btnSignature.BackColor = Color.White;
            this.btnSignature.ForeColor = Ops.Accent;
            this.btnSignature.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnSignature.Click += new System.EventHandler(this.btnSignature_Click);

            this.btnNavigate.Name = "btnNavigate";
            this.btnNavigate.AllowHtml = true;
            this.btnNavigate.AutoSize = true;
            this.btnNavigate.MinimumSize = new Size(0, 34);
            this.btnNavigate.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.btnNavigate.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnNavigate.Click += new System.EventHandler(this.btnNavigate_Click);

            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Text = "Open Settings";
            this.btnSettings.AutoSize = true;
            this.btnSettings.MinimumSize = new Size(0, 34);
            this.btnSettings.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.btnSettings.BackColor = Color.White;
            this.btnSettings.ForeColor = Ops.Bad;
            this.btnSettings.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnSettings.Visible = false;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);

            this.flowActions.Name = "flowActions";
            this.flowActions.Dock = Wisej.Web.DockStyle.Fill;
            this.flowActions.Controls.Add(this.btnPhoto);
            this.flowActions.Controls.Add(this.uploadPhoto);
            this.flowActions.Controls.Add(this.btnSignature);
            this.flowActions.Controls.Add(this.btnNavigate);
            this.flowActions.Controls.Add(this.btnSettings);

            this.lblFootNote.Name = "lblFootNote";
            this.lblFootNote.AutoSize = false;
            this.lblFootNote.Dock = Wisej.Web.DockStyle.Right;
            this.lblFootNote.Size = new Size(170, 34);
            this.lblFootNote.Font = Ops.Mono(11.5f);
            this.lblFootNote.ForeColor = Color.FromArgb(0x7D, 0x8E, 0xA0);
            this.lblFootNote.TextAlign = ContentAlignment.MiddleRight;

            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Dock = Wisej.Web.DockStyle.Top;
            this.pnlActions.Size = new Size(960, 58);
            this.pnlActions.Padding = new Wisej.Web.Padding(18, 12, 18, 12);
            this.pnlActions.CssStyle = "border-bottom:1px solid #eef2f7";
            this.pnlActions.Controls.Add(this.flowActions);
            this.pnlActions.Controls.Add(this.lblFootNote);
            //
            // ── banner: reason before a permission prompt, outcomes, fallbacks ───────────────
            //
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.AllowHtml = true;
            this.lblBanner.AutoSize = false;
            this.lblBanner.Dock = Wisej.Web.DockStyle.Top;
            this.lblBanner.Size = new Size(960, 58);
            this.lblBanner.Padding = new Wisej.Web.Padding(18, 10, 18, 0);
            this.lblBanner.Visible = false;
            //
            // ── photos strip + signature pad ─────────────────────────────────────────────────
            //
            this.lblPhotosCaption.Name = "lblPhotosCaption";
            this.lblPhotosCaption.Text = "PHOTOS";
            this.lblPhotosCaption.AutoSize = false;
            this.lblPhotosCaption.Dock = Wisej.Web.DockStyle.Top;
            this.lblPhotosCaption.Size = new Size(500, 22);
            this.lblPhotosCaption.Font = Ops.Px(11.5f, FontStyle.Bold);
            this.lblPhotosCaption.ForeColor = Ops.Muted;

            this.lblPhotoStrip.Name = "lblPhotoStrip";
            this.lblPhotoStrip.AllowHtml = true;
            this.lblPhotoStrip.AutoSize = false;
            this.lblPhotoStrip.Dock = Wisej.Web.DockStyle.Fill;

            this.pnlPhotos.Name = "pnlPhotos";
            this.pnlPhotos.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlPhotos.Controls.Add(this.lblPhotoStrip);
            this.pnlPhotos.Controls.Add(this.lblPhotosCaption);

            this.lblSignatureCaption.Name = "lblSignatureCaption";
            this.lblSignatureCaption.Text = "SIGNATURE";
            this.lblSignatureCaption.AutoSize = false;
            this.lblSignatureCaption.Dock = Wisej.Web.DockStyle.Top;
            this.lblSignatureCaption.Size = new Size(264, 22);
            this.lblSignatureCaption.Font = Ops.Px(11.5f, FontStyle.Bold);
            this.lblSignatureCaption.ForeColor = Ops.Muted;

            // The drawing surface: an HTML canvas, identical in the browser and in the client.
            this.lblSignaturePad.Name = "lblSignaturePad";
            this.lblSignaturePad.AllowHtml = true;
            this.lblSignaturePad.AutoSize = false;
            this.lblSignaturePad.Dock = Wisej.Web.DockStyle.Top;
            this.lblSignaturePad.Size = new Size(264, 64);

            this.txtSigner.Name = "txtSigner";
            this.txtSigner.Dock = Wisej.Web.DockStyle.Top;
            this.txtSigner.Size = new Size(264, 30);
            this.txtSigner.Watermark = "Signer's name";
            this.txtSigner.Font = Ops.Px(12);

            this.pnlSignature.Name = "pnlSignature";
            this.pnlSignature.Dock = Wisej.Web.DockStyle.Right;
            this.pnlSignature.Size = new Size(264, 130);
            this.pnlSignature.Controls.Add(this.txtSigner);
            this.pnlSignature.Controls.Add(this.lblSignaturePad);
            this.pnlSignature.Controls.Add(this.lblSignatureCaption);

            this.pnlMedia.Name = "pnlMedia";
            this.pnlMedia.Dock = Wisej.Web.DockStyle.Top;
            this.pnlMedia.Size = new Size(960, 150);
            this.pnlMedia.Padding = new Wisej.Web.Padding(18, 14, 18, 6);
            this.pnlMedia.Controls.Add(this.pnlPhotos);
            this.pnlMedia.Controls.Add(this.pnlSignature);
            //
            // ── status flow (Module 1): Accepted → On site → Completed ──────────────────────
            //
            this.lblStatusFlowCaption.Name = "lblStatusFlowCaption";
            this.lblStatusFlowCaption.Text = "UPDATE STATUS";
            this.lblStatusFlowCaption.AutoSize = true;
            this.lblStatusFlowCaption.Margin = new Wisej.Web.Padding(0, 9, 16, 0);
            this.lblStatusFlowCaption.Font = Ops.Px(11.5f, FontStyle.Bold);
            this.lblStatusFlowCaption.ForeColor = Ops.Muted;

            this.rbAccepted.Name = "rbAccepted";
            this.rbAccepted.Text = "Accepted";
            this.rbAccepted.AutoSize = true;
            this.rbAccepted.Margin = new Wisej.Web.Padding(0, 6, 14, 0);

            this.rbOnSite.Name = "rbOnSite";
            this.rbOnSite.Text = "On site";
            this.rbOnSite.AutoSize = true;
            this.rbOnSite.Margin = new Wisej.Web.Padding(0, 6, 14, 0);

            this.rbCompleted.Name = "rbCompleted";
            this.rbCompleted.Text = "Completed";
            this.rbCompleted.AutoSize = true;
            this.rbCompleted.Margin = new Wisej.Web.Padding(0, 6, 18, 0);

            this.btnSaveStatus.Name = "btnSaveStatus";
            this.btnSaveStatus.Text = "Update status";
            this.btnSaveStatus.AutoSize = true;
            this.btnSaveStatus.MinimumSize = new Size(0, 30);
            this.btnSaveStatus.BackColor = Color.White;
            this.btnSaveStatus.ForeColor = Ops.SecondaryInk;
            this.btnSaveStatus.Font = Ops.Px(12, FontStyle.Bold);
            this.btnSaveStatus.Click += new System.EventHandler(this.btnSaveStatus_Click);

            this.pnlStatusFlow.Name = "pnlStatusFlow";
            this.pnlStatusFlow.Dock = Wisej.Web.DockStyle.Top;
            this.pnlStatusFlow.Size = new Size(960, 44);
            this.pnlStatusFlow.Padding = new Wisej.Web.Padding(18, 4, 18, 0);
            this.pnlStatusFlow.Controls.Add(this.lblStatusFlowCaption);
            this.pnlStatusFlow.Controls.Add(this.rbAccepted);
            this.pnlStatusFlow.Controls.Add(this.rbOnSite);
            this.pnlStatusFlow.Controls.Add(this.rbCompleted);
            this.pnlStatusFlow.Controls.Add(this.btnSaveStatus);
            //
            // ── status strip: STATUS <what the last action did> ... "1 photo · signed" ─────
            //
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Text = "STATUS";
            this.lblStatusCaption.AutoSize = false;
            this.lblStatusCaption.Dock = Wisej.Web.DockStyle.Left;
            this.lblStatusCaption.Size = new Size(70, 40);
            this.lblStatusCaption.Font = Ops.Px(11.5f, FontStyle.Bold);
            this.lblStatusCaption.ForeColor = Ops.Muted;
            this.lblStatusCaption.TextAlign = ContentAlignment.MiddleLeft;

            this.lblStatus.Name = "lblStatus";
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Fill;
            this.lblStatus.Font = Ops.Px(13, FontStyle.Bold);
            this.lblStatus.ForeColor = Ops.Muted;
            this.lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            this.lblStatus.Text = "Ready";

            this.lblCount.Name = "lblCount";
            this.lblCount.AutoSize = false;
            this.lblCount.Dock = Wisej.Web.DockStyle.Right;
            this.lblCount.Size = new Size(200, 40);
            this.lblCount.Font = Ops.Mono(11.5f);
            this.lblCount.ForeColor = Color.FromArgb(0x7D, 0x8E, 0xA0);
            this.lblCount.TextAlign = ContentAlignment.MiddleRight;

            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlStatus.Size = new Size(960, 42);
            this.pnlStatus.Padding = new Wisej.Web.Padding(18, 0, 18, 0);
            this.pnlStatus.BackColor = Ops.Strip;
            this.pnlStatus.CssStyle = "border-top:1px solid #e4eaf1";
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Controls.Add(this.lblCount);
            this.pnlStatus.Controls.Add(this.lblStatusCaption);
            //
            // WorkOrderDetail - docked Top: the last one added sits highest.
            //
            this.Name = "WorkOrderDetail";
            this.Text = "FieldOps";
            this.BackColor = Color.White;
            this.Controls.Add(this.pnlStatusFlow);
            this.Controls.Add(this.pnlMedia);
            this.Controls.Add(this.lblBanner);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.pnlAppBar);
            this.Load += new System.EventHandler(this.WorkOrderDetail_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlAppBar;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Button btnBack;
        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblJob;
        private Wisej.Web.Label lblSla;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.FlowLayoutPanel flowActions;
        private Wisej.Web.Button btnPhoto;
        private Wisej.Web.Upload uploadPhoto;
        private Wisej.Web.Button btnSignature;
        private Wisej.Web.Button btnNavigate;
        private Wisej.Web.Button btnSettings;
        private Wisej.Web.Label lblFootNote;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Panel pnlMedia;
        private Wisej.Web.Panel pnlPhotos;
        private Wisej.Web.Label lblPhotoStrip;
        private Wisej.Web.Label lblPhotosCaption;
        private Wisej.Web.Panel pnlSignature;
        private Wisej.Web.TextBox txtSigner;
        private Wisej.Web.Label lblSignaturePad;
        private Wisej.Web.Label lblSignatureCaption;
        private Wisej.Web.FlowLayoutPanel pnlStatusFlow;
        private Wisej.Web.Label lblStatusFlowCaption;
        private Wisej.Web.RadioButton rbAccepted;
        private Wisej.Web.RadioButton rbOnSite;
        private Wisej.Web.RadioButton rbCompleted;
        private Wisej.Web.Button btnSaveStatus;
        private Wisej.Web.Panel pnlStatus;
        private Wisej.Web.Label lblStatusCaption;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblCount;
    }
}
