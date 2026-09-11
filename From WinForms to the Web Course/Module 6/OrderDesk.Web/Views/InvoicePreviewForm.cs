using System;
using System.IO;
using Wisej.Web;

namespace OrderDesk.Views
{
    /// <summary>
    /// The browser replacement for PrintPreviewDialog: a modal window with a PdfViewer fed from a
    /// server-side stream, plus a Download button. The PDF never touches a printer or a local path.
    /// </summary>
    public class InvoicePreviewForm : Form
    {
        private readonly byte[] _pdf;
        private readonly string _fileName;
        private PdfViewer _viewer;
        private Button _download;
        private Button _close;

        public InvoicePreviewForm(byte[] pdf, string fileName)
        {
            _pdf = pdf ?? throw new ArgumentNullException(nameof(pdf));
            _fileName = fileName ?? "document.pdf";
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this._viewer = new PdfViewer();
            this._download = new Button();
            this._close = new Button();
            this.SuspendLayout();
            //
            // _viewer
            //
            this._viewer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this._viewer.Location = new System.Drawing.Point(0, 0);
            this._viewer.Name = "pdfViewer";
            this._viewer.Size = new System.Drawing.Size(720, 480);
            this._viewer.ViewerType = PdfViewerType.Auto;
            this._viewer.PdfStream = new MemoryStream(_pdf);
            //
            // _download
            //
            this._download.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this._download.Location = new System.Drawing.Point(470, 492);
            this._download.Name = "buttonDownload";
            this._download.Size = new System.Drawing.Size(130, 34);
            this._download.Text = "⬇ Download";
            this._download.Click += (s, e) =>
            {
                Application.Download(new MemoryStream(_pdf), _fileName);
            };
            //
            // _close
            //
            this._close.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this._close.DialogResult = DialogResult.OK;
            this._close.Location = new System.Drawing.Point(610, 492);
            this._close.Name = "buttonClose";
            this._close.Size = new System.Drawing.Size(100, 34);
            this._close.Text = "Close";
            this._close.Click += (s, e) => this.Close();
            //
            // InvoicePreviewForm
            //
            this.ClientSize = new System.Drawing.Size(720, 536);
            this.Controls.Add(this._viewer);
            this.Controls.Add(this._download);
            this.Controls.Add(this._close);
            this.Name = "InvoicePreviewForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = _fileName;
            this.ResumeLayout(false);
        }
    }
}
