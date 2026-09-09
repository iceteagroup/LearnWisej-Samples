using System.Collections.Generic;

namespace IntegrationLab.Contracts
{
    /// <summary>
    /// Body of the "update" operation: POST {"rowKey":"WO-1043","changes":{"status":"Closed"}}.
    /// Only the changed fields travel; the server validates every one of them.
    /// </summary>
    public sealed class RowUpdateRequest
    {
        public string RowKey { get; set; }
        public Dictionary<string, object> Changes { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Body of the "create" operation: POST {"values":{"asset":"Pump 7","status":"Open",...}}.
    /// The server assigns the key; the client never invents one.
    /// </summary>
    public sealed class RowInsertRequest
    {
        public Dictionary<string, object> Values { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>Body of the "destroy" operation: POST {"rowKey":"WO-1043"}.</summary>
    public sealed class RowDeleteRequest
    {
        public string RowKey { get; set; }
    }
}
