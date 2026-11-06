using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FieldOps.Model;

namespace FieldOps.Services
{
    /// <summary>
    /// Builds the one-page work-order report as PDF bytes. A deliberately small, dependency-free
    /// PDF writer (Helvetica, one page, text only): swap in your reporting library and only this
    /// class changes. Where the bytes GO is not decided here - that is the page's job, and it
    /// depends on Device.Valid.
    /// </summary>
    public sealed class ReportService
    {
        public byte[] BuildWorkOrderReport(IEnumerable<WorkOrder> orders, string technician)
        {
            var lines = new List<(float size, string text)>
            {
                (18, "FieldOps - work-order report"),
                (11, $"{technician} - {DateTime.Now:dddd d MMMM yyyy, HH:mm}"),
                (11, ""),
                (10, "Order      Site                              Task                         Status"),
            };
            int count = 0;
            foreach (var o in orders)
            {
                lines.Add((10, $"{o.Number,-10} {Cut(o.Customer, 33),-33} {Cut(o.Fault, 28),-28} {WorkOrder.StatusText(o.Status)}"));
                count++;
            }
            lines.Add((10, ""));
            lines.Add((10, $"{count} work orders."));

            var content = new StringBuilder();
            content.Append("BT\n");
            float y = 800;
            foreach (var (size, text) in lines)
            {
                content.Append($"/{(size > 12 ? "F2" : "F1")} {F(size)} Tf 1 0 0 1 50 {F(y)} Tm ({Escape(text)}) Tj\n");
                y -= size + 8;
            }
            content.Append("ET\n");
            return Assemble(content.ToString());
        }

        private static byte[] Assemble(string stream)
        {
            var objects = new[]
            {
                "<< /Type /Catalog /Pages 2 0 R >>",
                "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
                "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 4 0 R /Resources << /Font << /F1 5 0 R /F2 6 0 R >> >> >>",
                $"<< /Length {Encoding.Latin1.GetByteCount(stream)} >>\nstream\n{stream}endstream",
                "<< /Type /Font /Subtype /Type1 /BaseFont /Courier /Encoding /WinAnsiEncoding >>",
                "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>",
            };

            using var ms = new MemoryStream();
            void W(string s) { var b = Encoding.Latin1.GetBytes(s); ms.Write(b, 0, b.Length); }
            W("%PDF-1.4\n");
            var offsets = new long[objects.Length];
            for (int i = 0; i < objects.Length; i++)
            {
                offsets[i] = ms.Position;
                W($"{i + 1} 0 obj\n{objects[i]}\nendobj\n");
            }
            long xref = ms.Position;
            W($"xref\n0 {objects.Length + 1}\n0000000000 65535 f \n");
            foreach (var off in offsets) W($"{off:D10} 00000 n \n");
            W($"trailer\n<< /Size {objects.Length + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF\n");
            return ms.ToArray();
        }

        private static string Cut(string s, int max) => s == null ? "" : s.Length <= max ? s : s.Substring(0, max - 1) + ".";

        private static string F(float f) => f.ToString("0.##", CultureInfo.InvariantCulture);

        private static string Escape(string s)
        {
            var sb = new StringBuilder();
            foreach (var ch in s)
            {
                if (ch == '(' || ch == ')' || ch == '\\') sb.Append('\\').Append(ch);
                else if (ch > 255) sb.Append('-');
                else sb.Append(ch);
            }
            return sb.ToString();
        }
    }
}
