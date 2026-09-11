using System;
using System.Collections.Generic;

namespace OperationsConsole.Models
{
    /// <summary>
    /// The dashboard view model: six month labels, six opened counts, six closed counts, one completion percentage,
    /// one document reference and the time the snapshot was produced. It is the only thing
    /// <c>DashboardPage.RefreshDashboard(model)</c> reads. It never contains the ticket table itself.
    /// </summary>
    public sealed record DashboardModel(
        IReadOnlyList<string> Months,
        IReadOnlyList<int> Opened,
        IReadOnlyList<int> Closed,
        int CompletionPercent,
        int TargetPercent,
        PreviewDocument PreviewDocument,
        DateTime GeneratedAt)
    {
        /// <summary>The label of the month the completion percentage refers to (the last chart label).</summary>
        public string CurrentMonth => Months.Count == 0 ? "—" : Months[Months.Count - 1];

        /// <summary>Tickets closed in the current month — what the completion value is computed from.</summary>
        public int ClosedThisMonth => Closed.Count == 0 ? 0 : Closed[Closed.Count - 1];

        /// <summary>True when this month's completion has reached the target.</summary>
        public bool OnTarget => CompletionPercent >= TargetPercent;
    }

    /// <summary>
    /// The document the dashboard previews: the report shipped in <c>wwwroot/</c> or the last uploaded file that
    /// <c>DocumentStore</c> holds. A reference (id / title / url), never the bytes.
    /// </summary>
    /// <param name="Id">Stable id ("DOC-SAMPLE", "DOC-000001").</param>
    /// <param name="Title">What the user sees.</param>
    /// <param name="Url">Relative URL for <c>PdfViewer.PdfSource</c>; null for an uploaded document.</param>
    /// <param name="IsUploaded">True when the bytes live in <c>DocumentStore</c> (shown with <c>PdfViewer.PdfStream</c>).</param>
    /// <param name="SizeBytes">Size of the document.</param>
    /// <param name="ReceivedAt">When the document was shipped / uploaded.</param>
    public sealed record PreviewDocument(
        string Id,
        string Title,
        string Url,
        bool IsUploaded,
        long SizeBytes,
        DateTime ReceivedAt);
}
