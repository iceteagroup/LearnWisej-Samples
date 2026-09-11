using System;

namespace IntegrationLab.Controls
{
    /// <summary>
    /// Data for <see cref="HeatmapWidget.CellSelected"/>: the cell the user clicked. <see cref="Value"/> is the
    /// authoritative server value for that cell; <see cref="ReportedValue"/> is what the browser sent.
    /// </summary>
    public class HeatmapCellEventArgs : EventArgs
    {
        public HeatmapCellEventArgs(int day, int hour, double value, double reportedValue)
        {
            this.Day = day;
            this.Hour = hour;
            this.Value = value;
            this.ReportedValue = reportedValue;
        }

        public int Day { get; }
        public int Hour { get; }
        public double Value { get; }
        public double ReportedValue { get; }
    }

    /// <summary>Data for <see cref="HeatmapWidget.DataLoaded"/>: the postback answered and the vendor accepted the cells.</summary>
    public class HeatmapLoadedEventArgs : EventArgs
    {
        public HeatmapLoadedEventArgs(int count) { this.Count = count; }
        public int Count { get; }
    }

    /// <summary>
    /// Data for <see cref="HeatmapWidget.LoadFailed"/>: the client adapter or the vendor reported a failure
    /// instead of crashing the page. <see cref="Phase"/> is "init", "load", "update" or "call".
    /// </summary>
    public class HeatmapErrorEventArgs : EventArgs
    {
        public HeatmapErrorEventArgs(string phase, int status, string message)
        {
            this.Phase = phase;
            this.Status = status;
            this.Message = message;
        }

        public string Phase { get; }
        /// <summary>HTTP status of the failed load, 0 when the failure was not an HTTP response.</summary>
        public int Status { get; }
        public string Message { get; }
    }
}
