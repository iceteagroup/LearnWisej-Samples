using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk.Pages
{
    /// <summary>
    /// Reports: the catalogue of what Module 6 produces on the server (PDF invoice, managed .xlsx, queued
    /// report batch) plus the Manager-only export guarded in this module. A list, not a generator — the
    /// generators live in Module 6's sample.
    /// </summary>
    public sealed class ReportsView : ViewBase
    {
        private readonly Panel _card = Card();
        private readonly Label _title = CardTitle("Reports");
        private readonly ListBox _list = new ListBox { Font = F(9F), BorderStyle = BorderStyle.None };
        private readonly Label _note = Lbl("Every report is generated on the server and reaches the browser as a PdfViewer stream or an Application.Download — no printer, no Excel, no C:\\Orders (Module 6). Sensitive exports carry a role check (Security review).", F(9F), Palette.MutedText);

        public ReportsView()
        {
            _list.Items.Add("Invoice (PDF) — per order · server-side PdfWriter → PdfViewer / Download");
            _list.Items.Add("Orders (.xlsx) — managed SpreadsheetML writer → Application.Download, replaces Excel Interop");
            _list.Items.Add("Orders (.csv) — LocalExport.ToCsv reused → Application.Download, replaces C:\\Orders\\out.csv");
            _list.Items.Add("All invoices (batch) — ReportQueue worker under App_Data/reports, progress pushed with Application.Update");
            _list.Items.Add("Customers with TaxId (.csv) — Manager only · DownloadGuard → 403 for Clerks (this module)");
            _card.Controls.Add(_title);
            _card.Controls.Add(_list);
            _card.Controls.Add(_note);
            this.Controls.Add(_card);
        }

        protected override void Relayout()
        {
            int w = this.Width - 2 * Pad, h = this.Height - 2 * Pad;
            if (w < 100 || h < 100) return;
            _card.SetBounds(Pad, Pad, w, h);
            _title.SetBounds(12, 6, w - 24, 22);
            _list.SetBounds(0, 32, w - 2, h - 32 - 60);
            _note.SetBounds(12, h - 56, w - 24, 48);
        }
    }
}
