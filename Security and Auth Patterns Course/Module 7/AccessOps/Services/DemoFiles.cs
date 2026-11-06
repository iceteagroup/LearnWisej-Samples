using System.IO;
using System.IO.Compression;
using System.Text;

namespace AccessOps.Services
{
    /// <summary>
    /// Small, valid demo files for the seeded documents: a one-page PDF, a minimal Word document and
    /// a minimal Excel workbook, each saying it is demo content. Used only to fill the store at first
    /// run, so downloads and share links have real bytes to stream.
    /// </summary>
    public static class DemoFiles
    {
        public static (byte[] Content, string ContentType) For(string title)
        {
            var ext = Path.GetExtension(title).ToLowerInvariant();
            return ext switch
            {
                ".docx" => (Docx(title), "application/vnd.openxmlformats-officedocument.wordprocessingml.document"),
                ".xlsx" => (Xlsx(title), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
                _ => (Pdf(title), "application/pdf")
            };
        }

        public static byte[] Pdf(string title)
        {
            var text = Escape("AccessOps demo document: " + title);
            var objects = new[]
            {
                "<< /Type /Catalog /Pages 2 0 R >>",
                "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
                "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>",
                null, // content stream, below
                "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"
            };
            var stream = $"BT /F1 14 Tf 60 780 Td ({text}) Tj ET";
            objects[3] = $"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}\nendstream";

            var sb = new StringBuilder("%PDF-1.4\n");
            var offsets = new int[objects.Length];
            for (int i = 0; i < objects.Length; i++)
            {
                offsets[i] = Encoding.ASCII.GetByteCount(sb.ToString());
                sb.Append($"{i + 1} 0 obj\n{objects[i]}\nendobj\n");
            }
            var xref = Encoding.ASCII.GetByteCount(sb.ToString());
            sb.Append($"xref\n0 {objects.Length + 1}\n0000000000 65535 f \n");
            foreach (var o in offsets)
                sb.Append(o.ToString("D10")).Append(" 00000 n \n");
            sb.Append($"trailer\n<< /Size {objects.Length + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF\n");
            return Encoding.ASCII.GetBytes(sb.ToString());
        }

        public static byte[] Docx(string title) => Zip(
            ("[Content_Types].xml", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/><Override PartName=\"/word/document.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml\"/></Types>"),
            ("_rels/.rels", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"word/document.xml\"/></Relationships>"),
            ("word/document.xml", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><w:document xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\"><w:body><w:p><w:r><w:t>" + Xml("AccessOps demo document: " + title) + "</w:t></w:r></w:p></w:body></w:document>"));

        public static byte[] Xlsx(string title) => Zip(
            ("[Content_Types].xml", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/><Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/><Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/></Types>"),
            ("_rels/.rels", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/></Relationships>"),
            ("xl/workbook.xml", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets><sheet name=\"Demo\" sheetId=\"1\" r:id=\"rId1\"/></sheets></workbook>"),
            ("xl/_rels/workbook.xml.rels", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/></Relationships>"),
            ("xl/worksheets/sheet1.xml", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData><row r=\"1\"><c r=\"A1\" t=\"inlineStr\"><is><t>" + Xml("AccessOps demo document: " + title) + "</t></is></c></row></sheetData></worksheet>"));

        private static byte[] Zip(params (string Name, string Content)[] parts)
        {
            using var buffer = new MemoryStream();
            using (var zip = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var (name, content) in parts)
                {
                    using var entry = new StreamWriter(zip.CreateEntry(name).Open(), new UTF8Encoding(false));
                    entry.Write(content);
                }
            }
            return buffer.ToArray();
        }

        private static string Escape(string s)
        {
            var ascii = new StringBuilder();
            foreach (var c in s)
                ascii.Append(c < 32 || c > 126 ? '-' : c);
            return ascii.ToString().Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        }

        private static string Xml(string s) => System.Security.SecurityElement.Escape(s);
    }
}
