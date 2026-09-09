namespace EnterpriseOps.Domain
{
    /// <summary>
    /// One line of an import file after parsing. It is data, not behaviour: the job validates it, the
    /// writer upserts it by <see cref="ExternalRef"/>, and a per-row error names it in the job result.
    /// </summary>
    public sealed class ImportRow
    {
        public int LineNumber { get; set; }
        public string ExternalRef { get; set; }
        public string Title { get; set; }
        public string Customer { get; set; }
        public string Site { get; set; }
        public string AssetCode { get; set; }
        public Priority Priority { get; set; }

        /// <summary>
        /// Fake-data knob: how many times the writer should time out on this row before it succeeds.
        /// A real file has no such column — the flaky database does it for free. Kept on the row so the
        /// fake repository stays deterministic and the README can say exactly which rows retry.
        /// </summary>
        public int TransientFailuresBeforeSuccess { get; set; }
    }
}
