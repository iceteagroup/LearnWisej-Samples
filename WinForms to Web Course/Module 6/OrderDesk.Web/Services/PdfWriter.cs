using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using OrderDesk.Domain;

namespace OrderDesk.Services
{
    /// <summary>
    /// The server replacement for PrintDocument: a hand-written single-page PDF 1.4 (Letter, Helvetica
    /// and Helvetica-Bold text objects, filled rectangles and rules, a correct cross-reference table).
    /// No printer, no Office, no third-party library — the bytes go to a PdfViewer, an
    /// Application.Download or a file under the storage root. A real project swaps this for a PDF
    /// library; <see cref="Invoice"/> shows that the invoice content itself is business logic reused as-is.
    /// </summary>
    public sealed class PdfWriter
    {
        public const double PageWidth = 612;    // Letter, points
        public const double PageHeight = 792;

        private readonly StringBuilder _content = new StringBuilder();
        private static readonly Encoding Latin1 = Encoding.Latin1;

        public string Title { get; set; } = "Document";
        public string Producer { get; set; } = "OrderDesk.Web PdfWriter (Module 6)";

        // ── drawing ───────────────────────────────────────────────────────────────

        /// <summary>Left-aligned text; (x, y) is the baseline start, PDF coordinates (origin bottom-left).</summary>
        public PdfWriter Text(double x, double y, double size, string text, bool bold = false, Color? color = null)
        {
            _content.Append("BT\n");
            _content.Append(Rgb(color ?? Color.Black, "rg")).Append('\n');
            _content.Append(bold ? "/F2 " : "/F1 ").Append(F(size)).Append(" Tf\n");
            _content.Append(F(x)).Append(' ').Append(F(y)).Append(" Td\n");
            _content.Append('(').Append(Escape(text)).Append(") Tj\nET\n");
            return this;
        }

        /// <summary>Right-aligned text ending at <paramref name="rightX"/> (uses the Helvetica metrics below).</summary>
        public PdfWriter TextRight(double rightX, double y, double size, string text, bool bold = false, Color? color = null)
            => Text(rightX - Width(text, size, bold), y, size, text, bold, color);

        public PdfWriter Line(double x1, double y1, double x2, double y2, double width = 0.5, Color? color = null)
        {
            _content.Append("q ").Append(Rgb(color ?? Color.Black, "RG")).Append(' ').Append(F(width)).Append(" w ")
                    .Append(F(x1)).Append(' ').Append(F(y1)).Append(" m ").Append(F(x2)).Append(' ').Append(F(y2)).Append(" l S Q\n");
            return this;
        }

        public PdfWriter Rect(double x, double y, double w, double h, Color fill)
        {
            _content.Append("q ").Append(Rgb(fill, "rg")).Append(' ')
                    .Append(F(x)).Append(' ').Append(F(y)).Append(' ').Append(F(w)).Append(' ').Append(F(h)).Append(" re f Q\n");
            return this;
        }

        // ── the PDF file ─────────────────────────────────────────────────────────

        /// <summary>
        /// Serializes objects 1-7 (catalog, pages, page, content stream, two fonts, info), then the xref
        /// table with the byte offset of every object and the trailer. Offsets are measured on the
        /// output stream itself, so they are exact by construction.
        /// </summary>
        public byte[] ToBytes()
        {
            var content = _content.ToString();
            var objects = new List<string>
            {
                "<< /Type /Catalog /Pages 2 0 R >>",
                "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
                "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 " + F(PageWidth) + " " + F(PageHeight) + "] /Resources << /Font << /F1 5 0 R /F2 6 0 R >> >> /Contents 4 0 R >>",
                "<< /Length " + Latin1.GetByteCount(content).ToString(CultureInfo.InvariantCulture) + " >>\nstream\n" + content + "endstream",
                "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>",
                "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>",
                "<< /Title (" + Escape(Title) + ") /Producer (" + Escape(Producer) + ") /Creator (OrderDesk.Web) /CreationDate (D:" + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + ") >>",
            };

            using (var ms = new MemoryStream())
            {
                Write(ms, "%PDF-1.4\n");
                ms.Write(new byte[] { 0x25, 0xE2, 0xE3, 0xCF, 0xD3, 0x0A }, 0, 6);   // binary marker comment

                var offsets = new long[objects.Count];
                for (int i = 0; i < objects.Count; i++)
                {
                    offsets[i] = ms.Position;
                    Write(ms, (i + 1).ToString(CultureInfo.InvariantCulture) + " 0 obj\n" + objects[i] + "\nendobj\n");
                }

                long xref = ms.Position;
                var sb = new StringBuilder();
                sb.Append("xref\n0 ").Append(objects.Count + 1).Append('\n');
                sb.Append("0000000000 65535 f \n");
                foreach (var off in offsets)
                    sb.Append(off.ToString("D10", CultureInfo.InvariantCulture)).Append(" 00000 n \n");
                sb.Append("trailer\n<< /Size ").Append(objects.Count + 1).Append(" /Root 1 0 R /Info 7 0 R >>\n");
                sb.Append("startxref\n").Append(xref.ToString(CultureInfo.InvariantCulture)).Append("\n%%EOF\n");
                Write(ms, sb.ToString());
                return ms.ToArray();
            }
        }

        private static void Write(Stream s, string text)
        {
            var bytes = Latin1.GetBytes(text);
            s.Write(bytes, 0, bytes.Length);
        }

        // ── the invoice (what InvoicePrinter drew on the PrintDocument) ──────────

        /// <summary>The Invoice-NNNN.pdf every report path produces (button, queue, download).</summary>
        public static byte[] Invoice(Order o, string generatedBy = null)
        {
            if (o == null) throw new ArgumentNullException(nameof(o));
            var en = CultureInfo.GetCultureInfo("en-US");
            var accent = Color.FromArgb(21, 101, 216);
            var accentSoft = Color.FromArgb(234, 243, 255);
            var ink = Color.FromArgb(31, 45, 58);
            var muted = Color.FromArgb(90, 107, 125);
            var rule = Color.FromArgb(220, 228, 236);
            var pdf = new PdfWriter { Title = "Invoice " + o.Id };

            // header band
            pdf.Rect(54, 716, 504, 34, accent);
            pdf.Text(66, 727, 16, "OrderDesk - Invoice " + o.Id, true, Color.White);
            pdf.TextRight(546, 728, 9, "generated on the server " + DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), false, Color.White);

            // meta block
            double y = 690;
            Meta(pdf, 54, ref y, "Customer", o.CustomerName, muted, ink);
            Meta(pdf, 54, ref y, "PO number", o.PoNumber ?? "-", muted, ink);
            Meta(pdf, 54, ref y, "Order date", o.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), muted, ink);
            y = 690;
            Meta(pdf, 330, ref y, "Owner", string.IsNullOrEmpty(o.Owner) ? "(unassigned)" : o.Owner, muted, ink);
            Meta(pdf, 330, ref y, "Status", o.Status.ToString(), muted, ink);
            Meta(pdf, 330, ref y, "Country", o.Customer?.Country ?? "-", muted, ink);

            // lines table
            y = 620;
            pdf.Rect(54, y - 6, 504, 20, accentSoft);
            pdf.Text(60, y, 9, "Qty", true, ink);
            pdf.Text(100, y, 9, "SKU", true, ink);
            pdf.Text(190, y, 9, "Description", true, ink);
            pdf.TextRight(470, y, 9, "Unit price", true, ink);
            pdf.TextRight(552, y, 9, "Line total", true, ink);
            y -= 24;
            foreach (var line in o.Lines)
            {
                pdf.Text(60, y, 10, line.Quantity.ToString(CultureInfo.InvariantCulture), false, ink);
                pdf.Text(100, y, 10, line.Sku ?? "", false, ink);
                pdf.Text(190, y, 10, line.Description ?? "", false, ink);
                pdf.TextRight(470, y, 10, line.UnitPrice.ToString("C", en), false, ink);
                pdf.TextRight(552, y, 10, line.LineTotal.ToString("C", en), false, ink);
                pdf.Line(54, y - 6, 558, y - 6, 0.4, rule);
                y -= 18;
                if (y < 160) { pdf.Text(60, y, 9, "... (" + (o.Lines.Count) + " lines, truncated to one page)", false, muted); y -= 18; break; }
            }

            // totals
            var subtotal = o.Lines.Sum(l => l.LineTotal);
            var discount = OrderCalculator.DiscountFor(o.Customer, subtotal);
            var tax = OrderCalculator.Tax(o.Customer, subtotal);
            y -= 6;
            Total(pdf, ref y, "Subtotal", subtotal.ToString("C", en), false, muted, ink);
            Total(pdf, ref y, "Discount" + (discount > 0 ? " (Gold 5%)" : ""), (-discount).ToString("C", en), false, muted, ink);
            Total(pdf, ref y, "Tax" + (o.Customer != null && o.Customer.TaxRate > 0 ? " (" + (o.Customer.TaxRate * 100).ToString("0.##", en) + "%)" : ""), tax.ToString("C", en), false, muted, ink);
            pdf.Line(400, y + 8, 558, y + 8, 0.8, ink);
            y -= 4;
            Total(pdf, ref y, "Total", o.Total.ToString("C", en), true, ink, accent);

            // footer
            pdf.Line(54, 72, 558, 72, 0.5, rule);
            pdf.Text(54, 58, 8, "LegacyOrderDesk sent this invoice to the default printer of the user's PC (System.Drawing.Printing.PrintDocument).", false, muted);
            pdf.Text(54, 46, 8, "OrderDesk.Web renders it on the server (Services/PdfWriter.cs) and hands it to the browser: PdfViewer, Application.Download, or a file under App_Data/reports.", false, muted);
            pdf.Text(54, 34, 8, "No printer, no Office, no local path" + (string.IsNullOrEmpty(generatedBy) ? "." : " - session " + generatedBy + "."), false, muted);
            return pdf.ToBytes();
        }

        private static void Meta(PdfWriter pdf, double x, ref double y, string label, string value, Color labelColor, Color valueColor)
        {
            pdf.Text(x, y, 9, label, false, labelColor);
            pdf.Text(x + 70, y, 10, value ?? "", true, valueColor);
            y -= 16;
        }

        private static void Total(PdfWriter pdf, ref double y, string label, string value, bool bold, Color labelColor, Color valueColor)
        {
            pdf.Text(400, y, bold ? 12 : 10, label, bold, labelColor);
            pdf.TextRight(552, y, bold ? 12 : 10, value, bold, valueColor);
            y -= bold ? 20 : 16;
        }

        // ── helpers ──────────────────────────────────────────────────────────────

        private static string F(double v) => v.ToString("0.###", CultureInfo.InvariantCulture);

        private static string Rgb(Color c, string op)
            => F(c.R / 255.0) + " " + F(c.G / 255.0) + " " + F(c.B / 255.0) + " " + op;

        /// <summary>PDF string literal: escape delimiters, map a few typographic characters, WinAnsi range only.</summary>
        public static string Escape(string text)
        {
            var sb = new StringBuilder((text ?? "").Length + 8);
            foreach (var ch in text ?? "")
            {
                switch (ch)
                {
                    case '(': sb.Append("\\("); break;
                    case ')': sb.Append("\\)"); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\r': case '\n': sb.Append(' '); break;
                    case '—': case '–': sb.Append('-'); break;
                    case '…': sb.Append("..."); break;
                    case '‘': case '’': sb.Append('\''); break;
                    case '“': case '”': sb.Append('"'); break;
                    case '€': sb.Append("EUR"); break;
                    default: sb.Append(ch < 32 || ch > 255 ? '?' : ch); break;
                }
            }
            return sb.ToString();
        }

        /// <summary>Approximate Helvetica advance widths (per 1000 em) — enough for right-aligned columns.</summary>
        public static double Width(string text, double size, bool bold = false)
        {
            double units = 0;
            foreach (var ch in text ?? "") units = units + Advance(ch);
            return units / 1000.0 * size * (bold ? 1.04 : 1.0);
        }

        private static int Advance(char ch)
        {
            if (ch >= '0' && ch <= '9') return 556;
            if (ch >= 'a' && ch <= 'z') return LowerWidths[ch - 'a'];
            if (ch >= 'A' && ch <= 'Z') return UpperWidths[ch - 'A'];
            switch (ch)
            {
                case ' ': case '.': case ',': case ':': case ';': case '/': case '!': case '|': return 278;
                case '-': case '(': case ')': case '[': case ']': return 333;
                case '\'': return 191;
                case '"': return 355;
                case '*': return 389;
                case '+': case '=': case '<': case '>': case '~': return 584;
                case '&': return 667;
                case '%': return 889;
                case '@': return 1015;
                default: return 556;
            }
        }

        private static readonly int[] LowerWidths =
        {
            556, 556, 500, 556, 556, 278, 556, 556, 222, 222, 500, 222, 833, 556, 556, 556, 556, 333, 500, 278, 556, 500, 722, 500, 500, 500
        };

        private static readonly int[] UpperWidths =
        {
            667, 667, 722, 722, 667, 611, 778, 722, 278, 500, 667, 556, 833, 722, 778, 667, 778, 722, 667, 611, 722, 667, 944, 667, 667, 611
        };
    }
}
