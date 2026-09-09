using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IntegrationLab.Contracts;

namespace IntegrationLab.Data
{
    /// <summary>
    /// In-memory, per-session, editable store of ~150 work orders. It implements the four
    /// grid operations (load / insert / update / delete) and the pivot query against the
    /// vendor-free contract types only: it has never heard of Kendo or DevExtreme.
    ///
    /// Everything the browser may influence is validated here: page size, sort and filter
    /// fields, editable fields, allowed values. A violation is a <see cref="DataContractException"/>
    /// (400), an unknown key is a 404; the controller maps both to status codes.
    /// </summary>
    public sealed class WorkOrderStore
    {
        public static readonly string[] Statuses = { "Open", "In progress", "On hold", "Closed" };
        public static readonly string[] Priorities = { "Low", "Medium", "High", "Critical" };
        public static readonly string[] Sites = { "Plant A", "Plant B", "Plant C", "Depot", "Field" };
        private static readonly string[] Assets = { "Pump", "Boiler", "Conveyor", "Compressor", "Chiller", "Forklift", "Mixer", "Turbine" };
        private static readonly string[] Assignees = { "Ana", "Ben", "Chloe", "Dev", "Eli", "Fay", "Gus", "Hana" };

        /// <summary>Fields the client may sort or filter by.</summary>
        public static readonly string[] QueryableFields = { "id", "asset", "status", "priority", "assignee", "hours", "site" };

        /// <summary>Fields the client may change through update/insert. "id" is never editable.</summary>
        public static readonly string[] EditableFields = { "asset", "status", "priority", "assignee", "hours", "site" };

        /// <summary>Fields the pivot may put on an axis.</summary>
        public static readonly string[] PivotFields = { "site", "status", "priority", "assignee" };

        public static readonly string[] PivotMeasures = { "hours", "count" };

        private static readonly string[] FilterOps = { "eq", "neq", "contains", "gt", "gte", "lt", "lte" };

        private readonly object _sync = new object();
        private readonly List<WorkOrder> _rows;
        private int _nextNumber;

        public WorkOrderStore(int count = 150)
        {
            _rows = Seed(count);
            _nextNumber = 1001 + count;
        }

        public int Count { get { lock (_sync) return _rows.Count; } }

        #region load

        public GridOperationResult<WorkOrder> Load(GridOperationRequest request)
        {
            if (request == null) throw new DataContractException(400, "A GridOperationRequest is required.");
            if (request.Skip < 0) throw new DataContractException(400, "skip must be >= 0.");
            if (request.Take < 1 || request.Take > GridOperationRequest.MaxTake)
                throw new DataContractException(400, $"take must be between 1 and {GridOperationRequest.MaxTake} (received {request.Take}).");

            foreach (var s in request.Sort ?? new List<SortDescriptor>())
                if (!QueryableFields.Contains((s.Field ?? "").ToLowerInvariant()))
                    throw new DataContractException(400, $"\"{s.Field}\" is not a sortable field.");

            foreach (var f in request.Filter ?? new List<FilterDescriptor>())
            {
                if (!QueryableFields.Contains((f.Field ?? "").ToLowerInvariant()))
                    throw new DataContractException(400, $"\"{f.Field}\" is not a filterable field.");
                if (!FilterOps.Contains((f.Op ?? "").ToLowerInvariant()))
                    throw new DataContractException(400, $"\"{f.Op}\" is not an allowed filter operator.");
            }

            lock (_sync)
            {
                IEnumerable<WorkOrder> query = _rows;

                foreach (var f in request.Filter ?? new List<FilterDescriptor>())
                    query = query.Where(r => Matches(r, f));

                if (request.Sort != null && request.Sort.Count > 0)
                {
                    IOrderedEnumerable<WorkOrder> ordered = null;
                    foreach (var s in request.Sort)
                    {
                        string field = s.Field.ToLowerInvariant();
                        Func<WorkOrder, object> key = r => r.Get(field);
                        var comparer = new FieldComparer();
                        ordered = ordered == null
                            ? (s.Desc ? query.OrderByDescending(key, comparer) : query.OrderBy(key, comparer))
                            : (s.Desc ? ordered.ThenByDescending(key, comparer) : ordered.ThenBy(key, comparer));
                    }
                    query = ordered;
                }

                var filtered = query.ToList();
                var page = filtered.Skip(request.Skip).Take(request.Take).Select(r => r.Clone()).ToList();
                return new GridOperationResult<WorkOrder>(page, filtered.Count, request.Skip, request.Take);
            }
        }

        private static bool Matches(WorkOrder row, FilterDescriptor f)
        {
            object v = row.Get(f.Field);
            string op = f.Op.ToLowerInvariant();
            if (v is double d)
            {
                if (!double.TryParse(f.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double n))
                    throw new DataContractException(400, $"\"{f.Value}\" is not a number for field \"{f.Field}\".");
                switch (op)
                {
                    case "eq": return d == n;
                    case "neq": return d != n;
                    case "gt": return d > n;
                    case "gte": return d >= n;
                    case "lt": return d < n;
                    case "lte": return d <= n;
                    default: throw new DataContractException(400, $"\"{op}\" cannot be applied to the numeric field \"{f.Field}\".");
                }
            }
            string s = (v as string) ?? "";
            string value = f.Value ?? "";
            switch (op)
            {
                case "eq": return string.Equals(s, value, StringComparison.OrdinalIgnoreCase);
                case "neq": return !string.Equals(s, value, StringComparison.OrdinalIgnoreCase);
                case "contains": return s.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
                case "gt": return string.Compare(s, value, StringComparison.OrdinalIgnoreCase) > 0;
                case "gte": return string.Compare(s, value, StringComparison.OrdinalIgnoreCase) >= 0;
                case "lt": return string.Compare(s, value, StringComparison.OrdinalIgnoreCase) < 0;
                case "lte": return string.Compare(s, value, StringComparison.OrdinalIgnoreCase) <= 0;
                default: return false;
            }
        }

        private sealed class FieldComparer : IComparer<object>
        {
            public int Compare(object x, object y)
            {
                if (x is double a && y is double b) return a.CompareTo(b);
                return string.Compare(x as string ?? "", y as string ?? "", StringComparison.OrdinalIgnoreCase);
            }
        }

        #endregion

        #region insert / update / delete

        public WorkOrder Insert(RowInsertRequest request)
        {
            if (request == null || request.Values == null || request.Values.Count == 0)
                throw new DataContractException(400, "insert requires a \"values\" object.");

            var values = JsonCodec.Normalize(request.Values);
            foreach (var key in values.Keys)
                if (!EditableFields.Contains(key.ToLowerInvariant()))
                    throw new DataContractException(400, $"\"{key}\" is not an insertable field.");

            var row = new WorkOrder
            {
                Asset = "New asset",
                Status = "Open",
                Priority = "Medium",
                Assignee = "",
                Hours = 0,
                Site = Sites[0]
            };
            Apply(row, values);

            lock (_sync)
            {
                row.Id = "WO-" + _nextNumber++;
                _rows.Add(row);
                return row.Clone();
            }
        }

        /// <summary>Applies the changes and returns the persisted row. Unknown key: 404.</summary>
        public WorkOrder Update(RowUpdateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RowKey))
                throw new DataContractException(400, "update requires a \"rowKey\".");
            if (request.Changes == null || request.Changes.Count == 0)
                throw new DataContractException(400, "update requires a non-empty \"changes\" object.");

            var changes = JsonCodec.Normalize(request.Changes);
            foreach (var key in changes.Keys)
                if (!EditableFields.Contains(key.ToLowerInvariant()))
                    throw new DataContractException(400, $"\"{key}\" is not an editable field.");

            lock (_sync)
            {
                var row = _rows.FirstOrDefault(r => string.Equals(r.Id, request.RowKey, StringComparison.OrdinalIgnoreCase));
                if (row == null)
                    throw new DataContractException(404, $"Work order \"{request.RowKey}\" does not exist.");

                // validate on a copy first so a bad value leaves the row untouched.
                var candidate = row.Clone();
                Apply(candidate, changes);

                row.Asset = candidate.Asset; row.Status = candidate.Status; row.Priority = candidate.Priority;
                row.Assignee = candidate.Assignee; row.Hours = candidate.Hours; row.Site = candidate.Site;
                return row.Clone();
            }
        }

        public void Delete(RowDeleteRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RowKey))
                throw new DataContractException(400, "delete requires a \"rowKey\".");

            lock (_sync)
            {
                int index = _rows.FindIndex(r => string.Equals(r.Id, request.RowKey, StringComparison.OrdinalIgnoreCase));
                if (index < 0)
                    throw new DataContractException(404, $"Work order \"{request.RowKey}\" does not exist.");
                _rows.RemoveAt(index);
            }
        }

        private static void Apply(WorkOrder row, Dictionary<string, object> values)
        {
            foreach (var pair in values)
            {
                string field = pair.Key.ToLowerInvariant();
                object value = pair.Value;
                switch (field)
                {
                    case "asset":
                        row.Asset = RequireText(field, value, 60);
                        break;
                    case "assignee":
                        row.Assignee = value == null ? "" : RequireText(field, value, 40);
                        break;
                    case "status":
                        row.Status = RequireOneOf(field, value, Statuses);
                        break;
                    case "priority":
                        row.Priority = RequireOneOf(field, value, Priorities);
                        break;
                    case "site":
                        row.Site = RequireOneOf(field, value, Sites);
                        break;
                    case "hours":
                        {
                            double hours;
                            if (value is double d) hours = d;
                            else if (value is string s && double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed)) hours = parsed;
                            else throw new DataContractException(400, $"hours must be a number (received {value ?? "null"}).");
                            if (hours < 0 || hours > 1000) throw new DataContractException(400, "hours must be between 0 and 1000.");
                            row.Hours = Math.Round(hours, 2);
                            break;
                        }
                }
            }
        }

        private static string RequireText(string field, object value, int maxLength)
        {
            string s = (value as string)?.Trim();
            if (string.IsNullOrEmpty(s)) throw new DataContractException(400, $"{field} must be a non-empty string.");
            if (s.Length > maxLength) throw new DataContractException(400, $"{field} must be at most {maxLength} characters.");
            return s;
        }

        private static string RequireOneOf(string field, object value, string[] allowed)
        {
            string s = (value as string)?.Trim();
            var match = allowed.FirstOrDefault(a => string.Equals(a, s, StringComparison.OrdinalIgnoreCase));
            if (match == null)
                throw new DataContractException(400, $"{field} must be one of: {string.Join(", ", allowed)} (received \"{s}\").");
            return match;
        }

        #endregion

        #region pivot

        public PivotResult Pivot(PivotRequest request)
        {
            if (request == null) throw new DataContractException(400, "A PivotRequest is required.");
            string rowField = (request.RowField ?? "").ToLowerInvariant();
            string columnField = (request.ColumnField ?? "").ToLowerInvariant();
            string measure = (request.Measure ?? "").ToLowerInvariant();

            if (!PivotFields.Contains(rowField)) throw new DataContractException(400, $"\"{request.RowField}\" cannot be a pivot row field.");
            if (!PivotFields.Contains(columnField)) throw new DataContractException(400, $"\"{request.ColumnField}\" cannot be a pivot column field.");
            if (rowField == columnField) throw new DataContractException(400, "rowField and columnField must differ.");
            if (!PivotMeasures.Contains(measure)) throw new DataContractException(400, $"\"{request.Measure}\" is not a pivot measure (hours, count).");

            lock (_sync)
            {
                var result = new PivotResult { RowField = rowField, ColumnField = columnField, Measure = measure };
                var cells = new Dictionary<(string, string), double>();
                foreach (var r in _rows)
                {
                    string rk = (string)r.Get(rowField);
                    string ck = (string)r.Get(columnField);
                    cells.TryGetValue((rk, ck), out double current);
                    cells[(rk, ck)] = current + (measure == "hours" ? r.Hours : 1);
                }
                result.RowKeys = OrderKeys(rowField, cells.Keys.Select(k => k.Item1).Distinct());
                result.ColumnKeys = OrderKeys(columnField, cells.Keys.Select(k => k.Item2).Distinct());
                foreach (var rk in result.RowKeys)
                    foreach (var ck in result.ColumnKeys)
                        if (cells.TryGetValue((rk, ck), out double v))
                            result.Cells.Add(new PivotCell(rk, ck, Math.Round(v, 2)));
                return result;
            }
        }

        private static List<string> OrderKeys(string field, IEnumerable<string> keys)
        {
            string[] canonical = field == "status" ? Statuses : field == "priority" ? Priorities : field == "site" ? Sites : null;
            return canonical != null
                ? keys.OrderBy(k => Array.IndexOf(canonical, k)).ToList()
                : keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToList();
        }

        #endregion

        private static List<WorkOrder> Seed(int count)
        {
            var rnd = new Random(9);       // deterministic: every session sees the same data
            var list = new List<WorkOrder>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(new WorkOrder
                {
                    Id = "WO-" + (1001 + i),
                    Asset = Assets[rnd.Next(Assets.Length)] + " " + (1 + rnd.Next(9)),
                    Status = Statuses[WeightedIndex(rnd, 4, 3, 1, 4)],
                    Priority = Priorities[WeightedIndex(rnd, 2, 4, 3, 1)],
                    Assignee = Assignees[rnd.Next(Assignees.Length)],
                    Hours = Math.Round(0.5 + rnd.NextDouble() * 15.5, 1),
                    Site = Sites[WeightedIndex(rnd, 4, 3, 2, 2, 1)]
                });
            }
            return list;
        }

        private static int WeightedIndex(Random rnd, params int[] weights)
        {
            int total = weights.Sum(), roll = rnd.Next(total);
            for (int i = 0; i < weights.Length; i++)
            {
                if (roll < weights[i]) return i;
                roll -= weights[i];
            }
            return weights.Length - 1;
        }
    }
}
