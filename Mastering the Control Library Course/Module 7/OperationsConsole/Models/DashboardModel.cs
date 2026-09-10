using System;
using System.Collections.Generic;

namespace OperationsConsole.Models
{
    /// <summary>
    /// The dashboard <b>view model</b> (Module 6 · Charts, Dashboards, Content, Media, and Documents).
    /// <para>
    /// It is shaped for <i>display</i>, not for storage: six month labels, six opened counts, six closed counts,
    /// one completion percentage, one document reference and the time the snapshot was produced. It is the only
    /// thing <c>DashboardPage.RefreshDashboard(model)</c> reads, which is why the chart, the indicator,
    /// the preview and the timestamp can never disagree with each other.
    /// </para>
    /// <para>
    /// What it deliberately does <b>not</b> contain: the ticket table. <c>DashboardService</c> aggregates a few
    /// hundred tickets down to these twelve numbers on the server; ten times more tickets would not make this
    /// object one byte bigger.
    /// </para>
    /// </summary>
    public sealed record DashboardModel(
        IReadOnlyList<string> Months,
        IReadOnlyList<int> Opened,
        IReadOnlyList<int> Closed,
        int CompletionPercent,
        int TargetPercent,
        PreviewDocument PreviewDocument,
        DateTime GeneratedAt,
        int TicketsAggregated)
    {
        /// <summary>The label of the month the completion percentage refers to (the last chart label, "Sep").</summary>
        public string CurrentMonth => Months.Count == 0 ? "—" : Months[Months.Count - 1];

        /// <summary>Tickets opened in the current month — the last point of the "Opened" series.</summary>
        public int OpenedThisMonth => Opened.Count == 0 ? 0 : Opened[Opened.Count - 1];

        /// <summary>Tickets closed in the current month — the number the completion gauge is computed from.</summary>
        public int ClosedThisMonth => Closed.Count == 0 ? 0 : Closed[Closed.Count - 1];

        /// <summary>True when this month's completion has reached the target — the gauge turns green.</summary>
        public bool OnTarget => CompletionPercent >= TargetPercent;
    }

    /// <summary>
    /// The document the dashboard previews: either the report that ships in <c>wwwroot/</c> or the last file a
    /// user uploaded through the <c>Upload</c> control and that <c>DocumentStore</c> kept in memory.
    /// The model carries a <b>reference</b> (id / title / url), never the bytes.
    /// </summary>
    /// <param name="Id">Stable id shown in the diagnostic panel ("DOC-SAMPLE", "DOC-000001").</param>
    /// <param name="Title">What the user sees ("SLA report — sample").</param>
    /// <param name="Url">Relative URL for <c>PdfViewer.PdfSource</c>; null for an uploaded document.</param>
    /// <param name="IsUploaded">True when the bytes live in <c>DocumentStore</c> (previewed with <c>PdfViewer.PdfStream</c>).</param>
    /// <param name="SizeBytes">Size of the document, for the preview caption.</param>
    /// <param name="ReceivedAt">When the document was shipped / uploaded.</param>
    public sealed record PreviewDocument(
        string Id,
        string Title,
        string Url,
        bool IsUploaded,
        long SizeBytes,
        DateTime ReceivedAt);
}
