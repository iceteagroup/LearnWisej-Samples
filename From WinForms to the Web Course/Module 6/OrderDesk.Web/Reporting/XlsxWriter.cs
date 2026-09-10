using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using OrderDesk.Domain;

namespace OrderDesk.Reporting
{
    /// <summary>
    /// ✓ A minimal, dependency-free .xlsx writer — the managed replacement for Excel Interop.
    /// An .xlsx file is a ZIP package of XML parts (Open Packaging Conventions + SpreadsheetML), so
    /// System.IO.Compression.ZipArchive and five small XML strings are enough for a single sheet:
    /// text goes into inline-string cells (t="inlineStr"), numbers into plain value cells.
    ///
    /// In a real project use a library built for unattended generation (EPPlus, Open XML SDK,
    /// Aspose.Cells, Syncfusion, DevExpress …) — this class exists because the course samples take
    /// no NuGet dependencies. What matters is what it is NOT: no Excel.exe, no COM, no dialog,
    /// no per-user process — any number of sessions can call it at once.
    /// </summary>
    public static class XlsxWriter
    {
        private const string NsMain = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        private const string NsRel = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        private const string NsPkgRel = "http://schemas.openxmlformats.org/package/2006/relationships";
        private const string NsContentTypes = "http://schemas.openxmlformats.org/package/2006/content-types";

        private static readonly string[] Headers = { "Order", "Customer", "Owner", "PO", "Status", "Total" };
        private static readonly int[] Widths = { 9, 26, 10, 12, 11, 12 };

        /// <summary>The orders as a one-sheet workbook ("Orders"), header row + one row per order.</summary>
        public static byte[] OrdersWorkbook(IEnumerable<Order> orders)
        {
            var sheet = new StringBuilder();
            sheet.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sheet.Append("<worksheet xmlns=\"").Append(NsMain).Append("\"><cols>");
            for (int c = 0; c < Widths.Length; c++)
                sheet.Append("<col min=\"").Append(c + 1).Append("\" max=\"").Append(c + 1).Append("\" width=\"").Append(Widths[c]).Append("\" customWidth=\"1\"/>");
            sheet.Append("</cols><sheetData>");

            sheet.Append("<row r=\"1\">");
            for (int c = 0; c < Headers.Length; c++)
                AppendText(sheet, c, 1, Headers[c]);
            sheet.Append("</row>");

            int row = 2;
            foreach (var o in orders)
            {
                sheet.Append("<row r=\"").Append(row).Append("\">");
                AppendNumber(sheet, 0, row, o.Id.ToString(CultureInfo.InvariantCulture));
                AppendText(sheet, 1, row, o.CustomerName);
                AppendText(sheet, 2, row, o.Owner);
                AppendText(sheet, 3, row, o.PoNumber);
                AppendText(sheet, 4, row, o.Status.ToString());
                AppendNumber(sheet, 5, row, o.Total.ToString("0.00", CultureInfo.InvariantCulture));
                sheet.Append("</row>");
                row++;
            }
            sheet.Append("</sheetData></worksheet>");

            return Package(new Dictionary<string, string>
            {
                ["[Content_Types].xml"] =
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<Types xmlns=\"" + NsContentTypes + "\">" +
                    "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>" +
                    "<Default Extension=\"xml\" ContentType=\"application/xml\"/>" +
                    "<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>" +
                    "<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>" +
                    "</Types>",
                ["_rels/.rels"] =
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<Relationships xmlns=\"" + NsPkgRel + "\">" +
                    "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/>" +
                    "</Relationships>",
                ["xl/workbook.xml"] =
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<workbook xmlns=\"" + NsMain + "\" xmlns:r=\"" + NsRel + "\">" +
                    "<sheets><sheet name=\"Orders\" sheetId=\"1\" r:id=\"rId1\"/></sheets>" +
                    "</workbook>",
                ["xl/_rels/workbook.xml.rels"] =
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<Relationships xmlns=\"" + NsPkgRel + "\">" +
                    "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/>" +
                    "</Relationships>",
                ["xl/worksheets/sheet1.xml"] = sheet.ToString(),
            });
        }

        public static MemoryStream OrdersWorkbookStream(IEnumerable<Order> orders) => new MemoryStream(OrdersWorkbook(orders));

        // ── cells ────────────────────────────────────────────────────────────────────────────────

        private static void AppendText(StringBuilder sb, int column, int row, string text)
        {
            sb.Append("<c r=\"").Append(CellRef(column, row)).Append("\" t=\"inlineStr\"><is><t>")
              .Append(EscapeXml(text ?? "")).Append("</t></is></c>");
        }

        private static void AppendNumber(StringBuilder sb, int column, int row, string invariantNumber)
        {
            sb.Append("<c r=\"").Append(CellRef(column, row)).Append("\"><v>").Append(invariantNumber).Append("</v></c>");
        }

        /// <summary>Column 0, row 1 → "A1"; column 26 → "AA".</summary>
        private static string CellRef(int column, int row)
        {
            var letters = new StringBuilder();
            int c = column;
            do
            {
                letters.Insert(0, (char)('A' + c % 26));
                c = c / 26 - 1;
            }
            while (c >= 0);
            return letters.Append(row).ToString();
        }

        private static string EscapeXml(string s) =>
            s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");

        // ── the package ──────────────────────────────────────────────────────────────────────────

        private static byte[] Package(Dictionary<string, string> parts)
        {
            var utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            using (var ms = new MemoryStream())
            {
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
                {
                    foreach (var part in parts)
                    {
                        var entry = zip.CreateEntry(part.Key, CompressionLevel.Optimal);
                        using (var writer = new StreamWriter(entry.Open(), utf8))
                            writer.Write(part.Value);
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
