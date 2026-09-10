using System;
using System.Collections.Generic;
using System.IO;
using OrderDesk.Domain;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// Copied as-is from LegacyOrderDesk/Reporting/ExcelExport.cs so Module 6 can show WHY the
    /// desktop "Export to Excel" cannot be carried to the server. It is never called by the web
    /// app: the console only probes the ProgID (Legacy.DesktopBoundaries.ExcelProgIdInstalled) and
    /// explains the result. The migrated path is Reporting/XlsxWriter + Application.Download.
    ///
    /// ✕ Excel.Application is Office Automation from a service process: Microsoft does not support
    ///   it (KB 257757) — one interactive Excel per export, no concurrency, hangs on any dialog.
    /// ✕ C:\Orders\out.xlsx is a path on the user's PC; on the server it is the server's disk.
    /// </summary>
    public static class ExcelExport
    {
        public static string ExportOrders(IList<Order> orders, string path)
        {
            var excelType = Type.GetTypeFromProgID("Excel.Application");             // ✕ needs Office on the server
            if (excelType == null)
                throw new InvalidOperationException("Excel is not installed on this machine (Excel.Application ProgID not found).");

            Directory.CreateDirectory(Path.GetDirectoryName(path));                  // ✕ creates C:\Orders on the SERVER

            dynamic excel = Activator.CreateInstance(excelType);                     // ✕ one Excel.exe per export, per user
            try
            {
                excel.Visible = false;
                dynamic workbook = excel.Workbooks.Add();
                dynamic sheet = workbook.Worksheets[1];
                sheet.Cells[1, 1] = "Order"; sheet.Cells[1, 2] = "Customer"; sheet.Cells[1, 3] = "Total"; sheet.Cells[1, 4] = "Status";
                int row = 2;
                foreach (var order in orders)
                {
                    sheet.Cells[row, 1] = order.Id;
                    sheet.Cells[row, 2] = order.CustomerName;
                    sheet.Cells[row, 3] = (double)order.Total;
                    sheet.Cells[row, 4] = order.Status.ToString();
                    row++;
                }
                if (File.Exists(path)) File.Delete(path);
                workbook.SaveAs(path);                                               // ✕ a "Save As" prompt here would hang the request for ever
                workbook.Close(false);
                return path;
            }
            finally
            {
                excel.Quit();
            }
        }
    }
}
