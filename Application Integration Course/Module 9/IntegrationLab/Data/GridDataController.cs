using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using IntegrationLab.Contracts;

namespace IntegrationLab.Data
{
    /// <summary>
    /// The vendor-free translation layer between "an action name plus some arguments" and
    /// the store. There is exactly one <see cref="Handle"/> and two entry points call it:
    ///
    ///   - WorkOrderGrid's postback handler (Kendo-style DataSource → HTTP GET/POST to the
    ///     widget's postback URL) passes the query string and the POST body;
    ///   - MainPage.LoadPivot (DevExtreme-style CustomStore → [WebMethod]) passes typed
    ///     arguments as a NameValueCollection.
    ///
    /// Both get the same validation and the same status codes. Switching vendors changes
    /// the InitScript translation, never this class.
    /// </summary>
    public sealed class GridDataController
    {
        /// <summary>The only actions a client may name. Anything else is a 400 before any parsing.</summary>
        public static readonly string[] Actions = { "load", "create", "update", "destroy", "pivot" };

        private readonly WorkOrderStore _store;

        public GridDataController(WorkOrderStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public WorkOrderStore Store => _store;

        /// <summary>
        /// Runs one operation.
        /// </summary>
        /// <param name="action">load | create | update | destroy | pivot</param>
        /// <param name="args">Query-string style arguments (skip, take, sort, filter, rowField, columnField, measure).</param>
        /// <param name="body">JSON body for create/update/destroy; null for load/pivot.</param>
        public OperationResult Handle(string action, NameValueCollection args, string body)
        {
            action = (action ?? "").Trim().ToLowerInvariant();
            if (!Actions.Contains(action))
                return OperationResult.Fail(400, $"Unknown action \"{action}\". Allowed: {string.Join(", ", Actions)}.");

            try
            {
                switch (action)
                {
                    case "load":
                        {
                            var request = ParseLoadArguments(args);
                            var result = _store.Load(request);
                            return OperationResult.Ok(
                                new { rows = result.Rows, total = result.Total, skip = result.Skip, take = result.Take },
                                $"({result.Rows.Count}/{result.Total})");
                        }
                    case "create":
                        {
                            var request = JsonCodec.Deserialize<RowInsertRequest>(body);
                            var row = _store.Insert(request);
                            return OperationResult.Ok(new { row }, $"→ {row.Id}");
                        }
                    case "update":
                        {
                            var request = JsonCodec.Deserialize<RowUpdateRequest>(body);
                            var row = _store.Update(request);
                            return OperationResult.Ok(new { row }, $"{row.Id} saved");
                        }
                    case "destroy":
                        {
                            var request = JsonCodec.Deserialize<RowDeleteRequest>(body);
                            _store.Delete(request);
                            return OperationResult.Ok(new { rowKey = request.RowKey, deleted = true }, $"{request.RowKey} removed ({_store.Count} left)");
                        }
                    case "pivot":
                        {
                            var request = new PivotRequest(args?["rowField"], args?["columnField"], args?["measure"]);
                            var result = _store.Pivot(request);
                            return OperationResult.Ok(
                                new
                                {
                                    status = 200,
                                    rowField = result.RowField,
                                    columnField = result.ColumnField,
                                    measure = result.Measure,
                                    rowKeys = result.RowKeys,
                                    columnKeys = result.ColumnKeys,
                                    // A WebMethod return value is NOT camel-cased by Wisej.NET (unlike Options), so
                                    // PivotCell would reach the vendor as {Row,Column,Value} and every cell would read
                                    // as empty. The wire names are spelled out here, like the fields around them.
                                    cells = result.Cells
                                        .Select(c => new { row = c.Row, column = c.Column, value = c.Value })
                                        .ToList()
                                },
                                $"({result.RowKeys.Count}×{result.ColumnKeys.Count}, {result.Cells.Count} cells)");
                        }
                }
                return OperationResult.Fail(400, "Unhandled action.");
            }
            catch (DataContractException ex)
            {
                // the contract was violated (400) or the key is unknown (404): a clean status, never a stack trace.
                return OperationResult.Fail(ex.StatusCode, ex.Message);
            }
        }

        /// <summary>
        /// Translates the vendor's read arguments into the typed request:
        ///   skip=20  take=20  sort=status asc,priority desc  filter=[{"field":"site","op":"eq","value":"Plant A"}]
        /// Kendo sends "sort" as a string, DevExtreme as JSON; both shapes are accepted here so
        /// the InitScripts stay thin.
        /// </summary>
        public static GridOperationRequest ParseLoadArguments(NameValueCollection args)
        {
            var request = new GridOperationRequest();
            if (args == null) return request;

            request.Skip = ParseInt(args["skip"], 0, "skip");
            request.Take = ParseInt(args["take"], 20, "take");

            string sort = args["sort"];
            if (!string.IsNullOrWhiteSpace(sort))
            {
                sort = sort.Trim();
                if (sort.StartsWith("["))
                {
                    // DevExtreme shape: [{"selector":"status","desc":false}] or [{"field":"status","desc":false}]
                    var list = JsonCodec.Deserialize<List<Dictionary<string, object>>>(sort);
                    foreach (var item in list)
                    {
                        var d = JsonCodec.Normalize(item);
                        string field = (d.TryGetValue("field", out var f) ? f : d.TryGetValue("selector", out var s) ? s : null) as string;
                        bool desc = d.TryGetValue("desc", out var v) && v is bool b && b;
                        request.Sort.Add(new SortDescriptor(field ?? "", desc));
                    }
                }
                else
                {
                    // Kendo shape: "status asc,priority desc"
                    foreach (var term in sort.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        var parts = term.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 0) continue;
                        bool desc = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
                        request.Sort.Add(new SortDescriptor(parts[0], desc));
                    }
                }
            }

            string filter = args["filter"];
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var list = JsonCodec.Deserialize<List<Dictionary<string, object>>>(filter);
                foreach (var item in list)
                {
                    var d = JsonCodec.Normalize(item);
                    string value = d.TryGetValue("value", out var v) ? Convert.ToString(v, CultureInfo.InvariantCulture) : "";
                    request.Filter.Add(new FilterDescriptor(
                        d.TryGetValue("field", out var f) ? f as string : "",
                        d.TryGetValue("op", out var o) ? o as string : "eq",
                        value));
                }
            }

            return request;
        }

        /// <summary>Trace helper: the compact request, or the raw arguments when they do not parse.</summary>
        public static string ParseLoadArgumentsSafe(NameValueCollection args)
        {
            try { return ParseLoadArguments(args).ToTraceString(); }
            catch (DataContractException)
            {
                return "{skip:" + (args?["skip"] ?? "") + ",take:" + (args?["take"] ?? "") + ",sort:\"" + (args?["sort"] ?? "") + "\"}";
            }
        }

        private static int ParseInt(string text, int fallback, string name)
        {
            if (string.IsNullOrWhiteSpace(text)) return fallback;
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)) return value;
            throw new DataContractException(400, $"{name} must be an integer (received \"{text}\").");
        }
    }
}
