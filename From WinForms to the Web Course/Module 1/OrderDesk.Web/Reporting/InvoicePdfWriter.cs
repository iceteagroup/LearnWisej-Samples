using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace OrderDesk.Reporting
{
    /// <summary>
    /// A tiny, dependency-free PDF writer: monospaced text lines on Letter pages. It replaces the
    /// desktop PrintDocument → local printer path with a document the server can generate for any
    /// session and hand to the browser (PdfViewer or download). A real project would use a
    /// server-safe PDF library; the point here is that the invoice is produced on the server.
    /// </summary>
    public static class InvoicePdfWriter
    {
        private const int PageWidth = 612, PageHeight = 792, Margin = 54, FontSize = 10, Leading = 14;
        private const int LinesPerPage = (PageHeight - 2 * Margin) / Leading;

        public static byte[] Write(IList<string> lines, string title = "Invoice")
        {
            var pages = new List<List<string>>();
            var current = new List<string>();
            foreach (var line in lines ?? Array.Empty<string>())
            {
                if (current.Count >= LinesPerPage) { pages.Add(current); current = new List<string>(); }
                current.Add(line ?? "");
            }
            pages.Add(current);

            // objects: 1 catalog, 2 pages, 3 font, 4 info, then (page, content) per page
            var objects = new List<string>();
            int firstPageObj = 5;
            var kids = new StringBuilder();
            for (int i = 0; i < pages.Count; i++)
                kids.Append(firstPageObj + i * 2).Append(" 0 R ");

            objects.Add("<< /Type /Catalog /Pages 2 0 R >>");
            objects.Add($"<< /Type /Pages /Kids [ {kids}] /Count {pages.Count} >>");
            objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Courier >>");
            objects.Add($"<< /Title ({Escape(title)}) /Producer (OrderDesk.Web InvoicePdfWriter) >>");

            for (int i = 0; i < pages.Count; i++)
            {
                int pageObj = firstPageObj + i * 2, contentObj = pageObj + 1;
                var content = new StringBuilder();
                content.Append("BT\n/F1 ").Append(FontSize).Append(" Tf\n");
                content.Append(Margin).Append(' ').Append(PageHeight - Margin).Append(" Td\n");
                content.Append(Leading).Append(" TL\n");
                foreach (var line in pages[i])
                    content.Append('(').Append(Escape(line)).Append(") Tj T*\n");
                content.Append("ET\n");
                string stream = content.ToString();
                objects.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {PageWidth} {PageHeight}] /Resources << /Font << /F1 3 0 R >> >> /Contents {contentObj} 0 R >>");
                objects.Add($"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}endstream");
            }

            var pdf = new StringBuilder();
            pdf.Append("%PDF-1.4\n");
            var offsets = new List<int>();
            for (int i = 0; i < objects.Count; i++)
            {
                offsets.Add(Encoding.ASCII.GetByteCount(pdf.ToString()));
                pdf.Append(i + 1).Append(" 0 obj\n").Append(objects[i]).Append("\nendobj\n");
            }
            int xref = Encoding.ASCII.GetByteCount(pdf.ToString());
            pdf.Append("xref\n0 ").Append(objects.Count + 1).Append('\n');
            pdf.Append("0000000000 65535 f \n");
            foreach (var offset in offsets)
                pdf.Append(offset.ToString("D10", CultureInfo.InvariantCulture)).Append(" 00000 n \n");
            pdf.Append("trailer\n<< /Size ").Append(objects.Count + 1).Append(" /Root 1 0 R /Info 4 0 R >>\nstartxref\n").Append(xref).Append("\n%%EOF\n");
            return Encoding.ASCII.GetBytes(pdf.ToString());
        }

        public static MemoryStream WriteToStream(IList<string> lines, string title = "Invoice") => new MemoryStream(Write(lines, title));

        /// <summary>Courier (WinAnsi) only: escape PDF delimiters and fold characters outside ASCII.</summary>
        private static string Escape(string text)
        {
            var sb = new StringBuilder(text.Length + 8);
            foreach (var ch in text)
            {
                switch (ch)
                {
                    case '(': sb.Append("\\("); break;
                    case ')': sb.Append("\\)"); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '—': case '–': sb.Append('-'); break;
                    case '…': sb.Append("..."); break;
                    case '€': sb.Append("EUR"); break;
                    default: sb.Append(ch < 32 || ch > 126 ? '?' : ch); break;
                }
            }
            return sb.ToString();
        }
    }
}
