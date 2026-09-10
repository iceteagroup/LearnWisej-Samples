using System;
using System.Globalization;

namespace OperationsConsole.Models
{
    /// <summary>
    /// One whole document, as <c>DocumentDetailControl.Show(...)</c> receives it.
    /// <para>
    /// This is the only type the detail UserControl knows: it never sees a TreeNode, a ListViewItem,
    /// the ListView, the TreeView or <c>DocumentService</c>. Everything the control renders is a
    /// property here, already formatted for display.
    /// </para>
    /// </summary>
    public class DocumentModel
    {
        /// <summary>Stable business ID (the integer the list carries in <c>Tag</c>).</summary>
        public int Id { get; set; }

        /// <summary>"DOC-000312".</summary>
        public string DocumentId => "DOC-" + this.Id.ToString("000000", CultureInfo.InvariantCulture);

        /// <summary>The document title.</summary>
        public string Title { get; set; }

        /// <summary>"Contoso ▸ Contracts" — which branch this document actually belongs to.</summary>
        public string CategoryPath { get; set; }

        /// <summary>The ID of the category the document lives in (proves two "Contracts" folders are different).</summary>
        public int CategoryId { get; set; }

        /// <summary>"contract" | "invoice" | "drawing".</summary>
        public string Kind { get; set; }

        /// <summary>"Contract", "Invoice", "Drawing".</summary>
        public string TypeLabel { get; set; }

        /// <summary>Size in KB.</summary>
        public int SizeKb { get; set; }

        /// <summary>Last modification date.</summary>
        public DateTime Modified { get; set; }

        /// <summary>Who owns the document.</summary>
        public string Owner { get; set; }

        /// <summary>A short human summary shown at the bottom of the detail card.</summary>
        public string Summary { get; set; }

        /// <summary>True when the document is flagged (retention review overdue).</summary>
        public bool IsWarning { get; set; }

        /// <summary>"Retention review overdue" / "OK" — the detail card colours this red when flagged.</summary>
        public string StatusText => this.IsWarning ? "Retention review overdue" : "OK";

        /// <summary>Size formatted for display.</summary>
        public string SizeText => this.SizeKb >= 1024
            ? (this.SizeKb / 1024.0).ToString("0.0", CultureInfo.InvariantCulture) + " MB"
            : this.SizeKb.ToString(CultureInfo.InvariantCulture) + " KB";

        /// <summary>Modification date formatted for display.</summary>
        public string ModifiedText => this.Modified.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
    }
}
