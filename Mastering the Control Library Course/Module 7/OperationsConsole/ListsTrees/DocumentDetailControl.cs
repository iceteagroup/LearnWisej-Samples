using System.Drawing;
using OperationsConsole.Models;
using Wisej.Web;

namespace OperationsConsole.ListsTrees
{
    /// <summary>
    /// The detail half of the document explorer (Module 4).
    /// <para>
    /// <b>Its whole public surface is two methods.</b> <see cref="Show(DocumentModel)"/> renders a document,
    /// <see cref="ShowMessage(string)"/> renders the empty / loading / error state. It has no reference to
    /// <c>categoryTree</c>, to <c>documentList</c>, to <c>DocumentService</c> or to anything that fetches data:
    /// the page resolves the stable ID, calls the service and hands the finished
    /// <see cref="DocumentModel"/> over. That is what makes the interesting half of the selection handler
    /// testable without a browser.
    /// </para>
    /// </summary>
    public partial class DocumentDetailControl : UserControl
    {
        private static readonly Color OkGreen = Color.FromArgb(31, 157, 87);
        private static readonly Color WarningRed = Color.FromArgb(224, 86, 59);

        public DocumentDetailControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Renders one document. This is the single entry point the lab asks for — the page calls it after the
        /// service returned, and nothing else in the application touches the Labels below.
        /// </summary>
        public void Show(DocumentModel model)
        {
            if (model == null)
            {
                ShowMessage("No document selected.");
                return;
            }

            this.lblId.Text = model.DocumentId;
            this.lblDocTitle.Text = model.Title;
            this.lblCategory.Text = model.CategoryPath + "   (category " + model.CategoryId + ")";
            this.lblType.Text = model.TypeLabel;
            this.lblSize.Text = model.SizeText;
            this.lblModified.Text = model.ModifiedText;
            this.lblOwner.Text = model.Owner;
            this.lblSummary.Text = model.Summary;

            this.lblStatus.Text = model.StatusText;
            this.lblStatus.ForeColor = model.IsWarning ? WarningRed : OkGreen;

            this.lblMessage.Visible = false;
            this.pnlFields.Visible = true;
        }

        /// <summary>
        /// The empty, loading and error states: one honest sentence instead of a half-filled card.
        /// The page never shows exception text here — that goes to the Event log only.
        /// </summary>
        public void ShowMessage(string text)
        {
            this.lblMessage.Text = text;
            this.pnlFields.Visible = false;
            this.lblMessage.Visible = true;
        }
    }
}
