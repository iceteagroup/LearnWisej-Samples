using System;
using System.Collections.Specialized;
using System.Globalization;

namespace IntegrationLab.Data
{
    /// <summary>
    /// The validated paging/sorting request. Both endpoints build one of these before
    /// touching the service, so the postback handler and the WebMethod apply exactly
    /// the same rules:
    ///
    ///   page   integer, 1..MaxPage           (bounded)
    ///   size   integer, 1..MaxPageSize (50)  (bounded: "a million rows" is refused)
    ///   sort   mapped to a whitelist of known columns, never used as-is
    ///   desc   boolean
    ///
    /// The raw strings from a query string go through <see cref="TryParse"/>; the typed
    /// WebMethod arguments go through <see cref="TryCreate"/>. Neither throws: the caller
    /// decides how to report the error (HTTP 400 vs ArgumentException).
    /// </summary>
    public sealed class PageRequest
    {
        public const int MaxPage = 10000;
        public const int MaxPageSize = 50;
        public const int DefaultPageSize = 10;
        public const string DefaultSort = "id";

        /// <summary>Client field names the grid is allowed to sort by (canonical casing).</summary>
        public static readonly string[] SortableColumns =
            { "id", "asset", "status", "priority", "assignee", "dueDate", "hours" };

        private PageRequest(int page, int size, string sort, bool desc)
        {
            this.Page = page;
            this.Size = size;
            this.Sort = sort;
            this.Desc = desc;
        }

        public int Page { get; }
        public int Size { get; }
        public string Sort { get; }
        public bool Desc { get; }

        /// <summary>Validates typed values (the WebMethod path).</summary>
        public static bool TryCreate(int page, int size, string sort, bool desc, out PageRequest request, out string error)
        {
            request = null;

            if (page < 1 || page > MaxPage)
            {
                error = $"page must be between 1 and {MaxPage}";
                return false;
            }
            if (size < 1 || size > MaxPageSize)
            {
                error = $"size must be between 1 and {MaxPageSize}";
                return false;
            }

            string canonical = MapSort(sort);
            if (canonical == null)
            {
                error = "sort must be one of: " + string.Join(", ", SortableColumns);
                return false;
            }

            request = new PageRequest(page, size, canonical, desc);
            error = null;
            return true;
        }

        /// <summary>Parses and validates raw query-string values (the postback path).</summary>
        public static bool TryParse(NameValueCollection query, out PageRequest request, out string error)
        {
            request = null;

            if (!TryParseInt(query["page"], 1, out int page))
            {
                error = "page must be an integer";
                return false;
            }
            if (!TryParseInt(query["size"], DefaultPageSize, out int size))
            {
                error = "size must be an integer";
                return false;
            }

            string descRaw = query["desc"];
            bool desc = descRaw != null && (descRaw == "1" || descRaw.Equals("true", StringComparison.OrdinalIgnoreCase));

            return TryCreate(page, size, query["sort"], desc, out request, out error);
        }

        /// <summary>
        /// Maps a client-supplied sort name onto the whitelist. Returns the canonical
        /// name, or null when the value is not a known column. Empty means the default.
        /// </summary>
        public static string MapSort(string sort)
        {
            if (string.IsNullOrWhiteSpace(sort))
                return DefaultSort;

            foreach (string column in SortableColumns)
                if (string.Equals(column, sort.Trim(), StringComparison.OrdinalIgnoreCase))
                    return column;

            return null;
        }

        private static bool TryParseInt(string raw, int defaultValue, out int value)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                value = defaultValue;
                return true;
            }
            return int.TryParse(raw.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        public override string ToString()
            => $"{{\"page\":{Page},\"size\":{Size},\"sort\":\"{Sort}\",\"desc\":{(Desc ? "true" : "false")}}}";
    }
}
