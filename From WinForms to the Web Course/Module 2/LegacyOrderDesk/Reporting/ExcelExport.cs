using System;
using System.Collections.Generic;
using System.IO;
using OrderDesk.Domain;

namespace LegacyOrderDesk.Reporting
{
    /// <summary>
    /// "Export to Excel" the desktop way: automate the Excel that is installed on this PC
    /// (late-bound so the project builds without an Interop reference) and save to a local path.
    /// Neither an interactive Excel nor C:\Orders exists on a shared web server — Module 6
    /// replaces this with a managed spreadsheet writer and a browser download.
    /// </summary>
    public static class ExcelExport
    {
        public static string ExportOrders(IList<Order> orders, string path)
        {
            var excelType = Type.GetTypeFromProgID("Excel.Application");
            if (excelType == null)
                throw new InvalidOperationException("Excel is not installed on this machine (Excel.Application ProgID not found).");

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            dynamic excel = Activator.CreateInstance(excelType);
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
                workbook.SaveAs(path);
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
