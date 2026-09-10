using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OrderDesk.Domain;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// Copied from LegacyOrderDesk/Legacy/ExcelExport.cs. Desktop assumption: "Export to Excel"
    /// automates the Excel that is installed on the user's PC. The COM object model below is the shape
    /// of Microsoft.Office.Interop.Excel (the real reference is not part of the course package; the stub
    /// behaves like Interop does without an interactive desktop). Microsoft does not support server-side
    /// Office Automation — on a web server it hangs or throws unpredictably. Module 6 replaces this with
    /// a managed spreadsheet writer + Application.Download.
    /// </summary>
    public static class ExcelExport
    {
        public static string ExportToExcel(IEnumerable<Order> orders)
        {
            Excel.Application xl = null;
            try
            {
                xl = new Excel.Application();          // ✕ COM server, needs an interactive user session
                var wb = xl.Workbooks.Add();
                var ws = wb.ActiveSheet;
                int row = 1;
                ws.Cells[row, 1] = "Order"; ws.Cells[row, 2] = "Customer"; ws.Cells[row, 3] = "Total"; ws.Cells[row, 4] = "Status";
                foreach (var o in orders)
                {
                    row++;
                    ws.Cells[row, 1] = o.Id; ws.Cells[row, 2] = o.CustomerName; ws.Cells[row, 3] = o.Total; ws.Cells[row, 4] = o.Status.ToString();
                }
                var path = LocalExport.ExportFolder + @"\out.xlsx";      // ✕ local path on the machine running Excel
                wb.SaveAs(path);
                return path;
            }
            finally
            {
                xl?.Quit();
            }
        }
    }
}

namespace OrderDesk.Legacy.Excel
{
    /// <summary>Stand-in for Microsoft.Office.Interop.Excel.Application (same call shape).</summary>
    public sealed class Application
    {
        public Application()
        {
            // Real Interop: CoCreateInstance("Excel.Application"). Without Excel and an interactive
            // desktop it fails with 0x80040154 (REGDB_E_CLASSNOTREG) or hangs on a server.
            if (!Environment.UserInteractive || Environment.GetEnvironmentVariable("ORDERDESK_HAS_EXCEL") != "1")
                throw new COMException("Retrieving the COM class factory for component with CLSID {00024500-0000-0000-C000-000000000046} failed (Excel.Application). Office Automation needs Excel installed in an interactive user session.", unchecked((int)0x80040154));
        }

        public Workbooks Workbooks { get; } = new Workbooks();
        public void Quit() { }
    }

    public sealed class Workbooks
    {
        public Workbook Add() => new Workbook();
    }

    public sealed class Workbook
    {
        public Worksheet ActiveSheet { get; } = new Worksheet();
        /// <summary>Stub: the real Interop writes the file on the machine running Excel. The course copy writes nothing.</summary>
        public void SaveAs(string path) { }
    }

    public sealed class Worksheet
    {
        public Range Cells { get; } = new Range();
    }

    public sealed class Range
    {
        public object this[int row, int column]
        {
            get => null;
            set { }
        }
    }
}
