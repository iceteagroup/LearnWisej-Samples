using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using OrderDesk.Domain;

namespace OrderDesk.Services
{
    /// <summary>
    /// The managed replacement for Excel Interop: a minimal OpenXML SpreadsheetML writer built on
    /// ZipArchive (framework only, no Office, no third-party package). It writes the six parts a
    /// workbook needs — [Content_Types].xml, _rels/.rels, xl/workbook.xml, xl/_rels/workbook.xml.rels,
    /// xl/styles.xml, xl/worksheets/sheet1.xml — with inline strings and plain numbers, which Excel,
    /// LibreOffice and Google Sheets all open. A real project swaps this for a spreadsheet library
    /// (Open XML SDK, EPPlus, Syncfusion, Aspose, DevExpress…); the call site stays "bytes → Download".
    /// </summary>
    public static class XlsxWriter
    {
        private const string NsMain = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        private const string NsRel = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        private const string NsPkgRel = "http://schemas.openxmlformats.org/package/2006/relationships";
        private const string NsTypes = "http://schemas.openxmlformats.org/package/2006/content-types";

        public static readonly string[] Parts =
        {
            "[Content_Types].xml", "_rels/.rels", "xl/workbook.xml", "xl/_rels/workbook.xml.rels", "xl/styles.xml", "xl/worksheets/sheet1.xml"
        };

        /// <summary>The Orders export: the same columns LocalExport.ToCsv writes, as a real workbook.</summary>
        public static byte[] OrdersWorkbook(IEnumerable<Order> orders)
        {
            var rows = orders.Select(o => (IList<object>)new object[]
            {
                o.Id, o.CustomerName, o.Owner ?? "", o.Total, o.Status.ToString(), o.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            });
            return Write("Orders", new[] { "Order", "Customer", "Owner", "Total", "Status", "Date" }, rows, new[] { 9.0, 26.0, 12.0, 14.0, 12.0, 12.0 });
        }

        /// <summary>
        /// One sheet: a bold header row, then one row per item. Numbers (int/long/decimal/double) become
        /// numeric cells (decimals with the built-in #,##0.00 format), everything else an inline string.
        /// </summary>
        public static byte[] Write(string sheetName, IList<string> headers, IEnumerable<IList<object>> rows, IList<double> columnWidths = null)
        {
            if (headers == null || headers.Count == 0) throw new ArgumentException("At least one header is required.", nameof(headers));
            var rowList = (rows ?? Enumerable.Empty<IList<object>>()).ToList();

            using (var ms = new MemoryStream())
            {
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
                {
                    Add(zip, Parts[0], ContentTypes());
                    Add(zip, Parts[1], RootRels());
                    Add(zip, Parts[2], Workbook(sheetName));
                    Add(zip, Parts[3], WorkbookRels());
                    Add(zip, Parts[4], Styles());
                    Add(zip, Parts[5], Sheet(headers, rowList, columnWidths));
                }
                return ms.ToArray();
            }
        }

        private static void Add(ZipArchive zip, string name, string xml)
        {
            var entry = zip.CreateEntry(name, CompressionLevel.Optimal);
            using (var s = entry.Open())
            {
                var bytes = new UTF8Encoding(false).GetBytes(xml);
                s.Write(bytes, 0, bytes.Length);
            }
        }

        private const string Decl = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\n";

        private static string ContentTypes() =>
            Decl +
            "<Types xmlns=\"" + NsTypes + "\">" +
            "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>" +
            "<Default Extension=\"xml\" ContentType=\"application/xml\"/>" +
            "<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>" +
            "<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>" +
            "<Override PartName=\"/xl/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml\"/>" +
            "</Types>";

        private static string RootRels() =>
            Decl +
            "<Relationships xmlns=\"" + NsPkgRel + "\">" +
            "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/>" +
            "</Relationships>";

        private static string Workbook(string sheetName) =>
            Decl +
            "<workbook xmlns=\"" + NsMain + "\" xmlns:r=\"" + NsRel + "\">" +
            "<sheets><sheet name=\"" + Esc(SafeSheetName(sheetName)) + "\" sheetId=\"1\" r:id=\"rId1\"/></sheets>" +
            "</workbook>";

        private static string WorkbookRels() =>
            Decl +
            "<Relationships xmlns=\"" + NsPkgRel + "\">" +
            "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/>" +
            "<Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/>" +
            "</Relationships>";

        /// <summary>Style 0 = default, 1 = bold header, 2 = number #,##0.00 (built-in numFmtId 4).</summary>
        private static string Styles() =>
            Decl +
            "<styleSheet xmlns=\"" + NsMain + "\">" +
            "<fonts count=\"2\"><font><sz val=\"11\"/><name val=\"Calibri\"/></font><font><b/><sz val=\"11\"/><name val=\"Calibri\"/></font></fonts>" +
            "<fills count=\"2\"><fill><patternFill patternType=\"none\"/></fill><fill><patternFill patternType=\"gray125\"/></fill></fills>" +
            "<borders count=\"1\"><border><left/><right/><top/><bottom/><diagonal/></border></borders>" +
            "<cellStyleXfs count=\"1\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/></cellStyleXfs>" +
            "<cellXfs count=\"3\">" +
            "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"/>" +
            "<xf numFmtId=\"0\" fontId=\"1\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyFont=\"1\"/>" +
            "<xf numFmtId=\"4\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyNumberFormat=\"1\"/>" +
            "</cellXfs>" +
            "<cellStyles count=\"1\"><cellStyle name=\"Normal\" xfId=\"0\" builtinId=\"0\"/></cellStyles>" +
            "</styleSheet>";

        private static string Sheet(IList<string> headers, List<IList<object>> rows, IList<double> widths)
        {
            var sb = new StringBuilder();
            sb.Append(Decl);
            sb.Append("<worksheet xmlns=\"" + NsMain + "\" xmlns:r=\"" + NsRel + "\">");
            sb.Append("<dimension ref=\"A1:").Append(Col(headers.Count)).Append(rows.Count + 1).Append("\"/>");
            sb.Append("<sheetViews><sheetView workbookViewId=\"0\"><pane ySplit=\"1\" topLeftCell=\"A2\" activePane=\"bottomLeft\" state=\"frozen\"/></sheetView></sheetViews>");
            sb.Append("<sheetFormatPr defaultRowHeight=\"15\"/>");
            if (widths != null && widths.Count > 0)
            {
                sb.Append("<cols>");
                for (int i = 0; i < widths.Count && i < headers.Count; i++)
                    sb.Append("<col min=\"").Append(i + 1).Append("\" max=\"").Append(i + 1).Append("\" width=\"").Append(widths[i].ToString("0.##", CultureInfo.InvariantCulture)).Append("\" customWidth=\"1\"/>");
                sb.Append("</cols>");
            }
            sb.Append("<sheetData>");

            sb.Append("<row r=\"1\">");
            for (int c = 0; c < headers.Count; c++)
                AppendInlineString(sb, Col(c + 1) + "1", headers[c], 1);
            sb.Append("</row>");

            for (int r = 0; r < rows.Count; r++)
            {
                int rowNo = r + 2;
                sb.Append("<row r=\"").Append(rowNo).Append("\">");
                var row = rows[r];
                for (int c = 0; c < headers.Count; c++)
                {
                    var value = c < row.Count ? row[c] : null;
                    var cellRef = Col(c + 1) + rowNo.ToString(CultureInfo.InvariantCulture);
                    switch (value)
                    {
                        case null: break;
                        case int i: AppendNumber(sb, cellRef, i.ToString(CultureInfo.InvariantCulture), 0); break;
                        case long l: AppendNumber(sb, cellRef, l.ToString(CultureInfo.InvariantCulture), 0); break;
                        case decimal d: AppendNumber(sb, cellRef, d.ToString("0.00##", CultureInfo.InvariantCulture), 2); break;
                        case double dbl: AppendNumber(sb, cellRef, dbl.ToString("R", CultureInfo.InvariantCulture), 2); break;
                        case DateTime dt: AppendInlineString(sb, cellRef, dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), 0); break;
                        default: AppendInlineString(sb, cellRef, Convert.ToString(value, CultureInfo.InvariantCulture), 0); break;
                    }
                }
                sb.Append("</row>");
            }

            sb.Append("</sheetData></worksheet>");
            return sb.ToString();
        }

        private static void AppendNumber(StringBuilder sb, string cellRef, string text, int style)
        {
            sb.Append("<c r=\"").Append(cellRef).Append('"');
            if (style != 0) sb.Append(" s=\"").Append(style).Append('"');
            sb.Append("><v>").Append(text).Append("</v></c>");
        }

        private static void AppendInlineString(StringBuilder sb, string cellRef, string text, int style)
        {
            sb.Append("<c r=\"").Append(cellRef).Append("\" t=\"inlineStr\"");
            if (style != 0) sb.Append(" s=\"").Append(style).Append('"');
            sb.Append("><is><t").Append(text != null && (text.StartsWith(" ") || text.EndsWith(" ")) ? " xml:space=\"preserve\"" : "").Append('>')
              .Append(Esc(text)).Append("</t></is></c>");
        }

        /// <summary>1 → A, 26 → Z, 27 → AA.</summary>
        public static string Col(int index)
        {
            var s = "";
            while (index > 0)
            {
                int rem = (index - 1) % 26;
                s = (char)('A' + rem) + s;
                index = (index - 1) / 26;
            }
            return s;
        }

        private static string SafeSheetName(string name)
        {
            var n = string.IsNullOrWhiteSpace(name) ? "Sheet1" : name;
            foreach (var ch in new[] { ':', '\\', '/', '?', '*', '[', ']' }) n = n.Replace(ch, '_');
            return n.Length > 31 ? n.Substring(0, 31) : n;
        }

        /// <summary>XML text escaping; control characters (illegal in XML 1.0) are dropped.</summary>
        private static string Esc(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var sb = new StringBuilder(s.Length + 8);
            foreach (var ch in s)
            {
                switch (ch)
                {
                    case '&': sb.Append("&amp;"); break;
                    case '<': sb.Append("&lt;"); break;
                    case '>': sb.Append("&gt;"); break;
                    case '"': sb.Append("&quot;"); break;
                    default:
                        if (ch >= 0x20 || ch == '\t' || ch == '\n' || ch == '\r') sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
