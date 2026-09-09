namespace IntegrationLab.Data
{
    /// <summary>
    /// One row of the editable grid. Serialized camelCase, so the vendor sees
    /// {"id":"WO-1001","asset":"Pump 3","status":"Open","priority":"High","assignee":"Ana","hours":4.5,"site":"Plant A"}.
    /// "id" is the key the vendor's schema.model.id / CustomStore.key points at.
    /// </summary>
    public sealed class WorkOrder
    {
        public string Id { get; set; }
        public string Asset { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Assignee { get; set; }
        public double Hours { get; set; }
        public string Site { get; set; }

        public WorkOrder Clone() => (WorkOrder)this.MemberwiseClone();

        /// <summary>Reads a field by its wire (camelCase) name. Used by sort, filter and pivot.</summary>
        public object Get(string field)
        {
            switch ((field ?? "").ToLowerInvariant())
            {
                case "id": return this.Id;
                case "asset": return this.Asset;
                case "status": return this.Status;
                case "priority": return this.Priority;
                case "assignee": return this.Assignee;
                case "hours": return this.Hours;
                case "site": return this.Site;
                default: return null;
            }
        }
    }
}
