using System.Drawing;
using OperationsConsole.Models;
using Wisej.Web;

namespace OperationsConsole.ListsTrees
{
    /// <summary>
    /// The detail half of the document explorer. <see cref="Show(DocumentModel)"/> renders a document and
    /// <see cref="ShowMessage(string)"/> renders the empty / loading / error state. It has no reference to the tree,
    /// the list or the service: the page hands it a finished <see cref="DocumentModel"/>.
    /// </summary>
    public partial class DocumentDetailControl : UserControl
    {
        private static readonly Color OkGreen = Color.FromArgb(31, 157, 87);
        private static readonly Color WarningRed = Color.FromArgb(224, 86, 59);

        public DocumentDetailControl()
        {
            InitializeComponent();
        }

        /// <summary>Renders one document.</summary>
        public void Show(DocumentModel model)
        {
            if (model == null)
            {
                ShowMessage("No document selected.");
                return;
            }

            this.lblId.Text = model.DocumentId;
            this.lblDocTitle.Text = model.Title;
            this.lblCategory.Text = model.CategoryPath;
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

        /// <summary>The empty, loading and error states: one sentence instead of a half-filled card.</summary>
        public void ShowMessage(string text)
        {
            this.lblMessage.Text = text;
            this.pnlFields.Visible = false;
            this.lblMessage.Visible = true;
        }
    }
}
