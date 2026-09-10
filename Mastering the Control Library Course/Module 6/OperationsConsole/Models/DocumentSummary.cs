using System;
using System.Globalization;

namespace OperationsConsole.Models
{
    /// <summary>
    /// One row of the document list: exactly the four columns the <c>documentList</c> ListView shows,
    /// plus the stable ID and the image key. This is what a server-side page is made of — it is
    /// deliberately small, because in virtual mode <c>RetrieveVirtualItem</c> must be cheap.
    /// </summary>
    public class DocumentSummary
    {
        /// <summary>Stable business ID — what lands in <c>ListViewItem.Tag</c>.</summary>
        public int Id { get; set; }

        /// <summary>"DOC-000312" — the ID the user sees and the shell records (<c>ConsoleLog.Record</c>).</summary>
        public string DocumentId => "DOC-" + Id.ToString("000000", CultureInfo.InvariantCulture);

        /// <summary>Column 1. Not unique across categories, so never used as a key.</summary>
        public string Name { get; set; }

        /// <summary>"contract" | "invoice" | "drawing" — the ImageList key family.</summary>
        public string Kind { get; set; }

        /// <summary>Column 2, the human label for <see cref="Kind"/> ("Contract", "Invoice", "Drawing").</summary>
        public string TypeLabel { get; set; }

        /// <summary>Column 3, in KB.</summary>
        public int SizeKb { get; set; }

        /// <summary>Column 4.</summary>
        public DateTime Modified { get; set; }

        /// <summary>A document whose retention review is overdue — it gets the "warning" icon instead of its type icon.</summary>
        public bool IsWarning { get; set; }

        /// <summary>The shared ImageList key: the type icon, or "warning" when the row needs attention.</summary>
        public string ImageKey => this.IsWarning ? "warning" : this.Kind;

        /// <summary>Column 3 formatted.</summary>
        public string SizeText => this.SizeKb >= 1024
            ? (this.SizeKb / 1024.0).ToString("0.0", CultureInfo.InvariantCulture) + " MB"
            : this.SizeKb.ToString(CultureInfo.InvariantCulture) + " KB";

        /// <summary>Column 4 formatted.</summary>
        public string ModifiedText => this.Modified.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}
