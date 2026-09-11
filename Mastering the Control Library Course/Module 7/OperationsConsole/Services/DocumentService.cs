using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OperationsConsole.Models;

namespace OperationsConsole.Services
{
    /// <summary>
    /// The service boundary of the document explorer. The tree and the list resolve a stable ID and call one of the
    /// four methods below. Everything here is in memory — a few hundred generated documents — but every call costs
    /// time (<see cref="SimulatedLatencyMs"/>) and can fail (<see cref="SimulateFailure"/>).
    /// <para>
    /// Three customers each own a folder called <b>Contracts</b> (IDs 7, 10 and 13), two categories are empty, and
    /// one top-level branch has no children. Only the IDs tell them apart.
    /// </para>
    /// </summary>
    public class DocumentService
    {
        /// <summary>How long a call to this "remote" service takes. Long enough that the loading state is visible.</summary>
        public const int SimulatedLatencyMs = 300;

        /// <summary>A shorter latency for the cheap navigation calls (top categories, children).</summary>
        public const int TreeLatencyMs = 150;

        /// <summary>When true, every expand, page load and document load throws <see cref="InvalidOperationException"/>.</summary>
        public bool SimulateFailure { get; set; }

        // ------------------------------------------------------------------------------------------------
        // The public surface the lab asks for
        // ------------------------------------------------------------------------------------------------

        /// <summary>The top level of the tree only — three customers and three shared libraries. Never the whole tree.</summary>
        public IList<CategoryNode> GetTopCategories()
        {
            Thread.Sleep(TreeLatencyMs);
            ThrowIfSimulatingFailure("GetTopCategories()");

            var list = Catalog.Where(c => c.ParentId == 0).Select(ToNode).ToList();
            return list;
        }

        /// <summary>The children of one category — called by <c>AfterExpand</c>, never at startup.</summary>
        public IList<CategoryNode> GetChildren(int categoryId)
        {
            Thread.Sleep(TreeLatencyMs);
            ThrowIfSimulatingFailure("GetChildren(" + categoryId + ")");

            var list = Catalog.Where(c => c.ParentId == categoryId).Select(ToNode).ToList();
            return list;
        }

        /// <summary>
        /// The server-side page the virtual-mode ListView reads from: every <see cref="DocumentSummary"/> of the
        /// category plus <see cref="DocumentPage.Total"/>. Synchronous version (blocks for <see cref="SimulatedLatencyMs"/>).
        /// </summary>
        public DocumentPage GetDocumentPage(int categoryId)
        {
            Thread.Sleep(SimulatedLatencyMs);
            return BuildPage(categoryId);
        }

        /// <summary>
        /// Awaitable version of <see cref="GetDocumentPage"/> — the page uses this one so
        /// <c>documentList.ShowLoader = true</c> is actually visible while the "service" runs.
        /// </summary>
        public async Task<DocumentPage> GetDocumentPageAsync(int categoryId)
        {
            await Task.Delay(SimulatedLatencyMs);
            return BuildPage(categoryId);
        }

        /// <summary>One whole document, for the detail UserControl. Synchronous version.</summary>
        public DocumentModel GetDocument(int documentId)
        {
            Thread.Sleep(SimulatedLatencyMs);
            return BuildDocument(documentId);
        }

        /// <summary>Awaitable version of <see cref="GetDocument"/> — used by the selection handlers.</summary>
        public async Task<DocumentModel> GetDocumentAsync(int documentId)
        {
            await Task.Delay(SimulatedLatencyMs);
            return BuildDocument(documentId);
        }

        /// <summary>"Contoso ▸ Contracts" for any category ID.</summary>
        public string GetCategoryPath(int categoryId)
        {
            var row = Catalog.FirstOrDefault(c => c.Id == categoryId);
            return row == null ? "—" : row.Path;
        }

        // ------------------------------------------------------------------------------------------------
        // The work, kept out of the public methods so the latency/failure wrapper stays readable
        // ------------------------------------------------------------------------------------------------

        private DocumentPage BuildPage(int categoryId)
        {
            ThrowIfSimulatingFailure("GetDocumentPage(" + categoryId + ")");

            List<DocumentSummary> rows;
            if (!DocumentsByCategory.TryGetValue(categoryId, out rows))
                rows = new List<DocumentSummary>();

            var page = new DocumentPage
            {
                CategoryId = categoryId,
                CategoryPath = GetCategoryPath(categoryId),
                Items = rows,
                Total = rows.Count
            };

            return page;
        }

        private DocumentModel BuildDocument(int documentId)
        {
            ThrowIfSimulatingFailure("GetDocument(" + documentId + ")");

            DocumentRow row;
            if (!DocumentsById.TryGetValue(documentId, out row))
                throw new InvalidOperationException("Document " + documentId + " does not exist.");

            var model = new DocumentModel
            {
                Id = row.Summary.Id,
                Title = row.Summary.Name,
                CategoryId = row.CategoryId,
                CategoryPath = GetCategoryPath(row.CategoryId),
                Kind = row.Summary.Kind,
                TypeLabel = row.Summary.TypeLabel,
                SizeKb = row.Summary.SizeKb,
                Modified = row.Summary.Modified,
                Owner = row.Owner,
                Summary = row.Description,
                IsWarning = row.Summary.IsWarning
            };

            return model;
        }

        private void ThrowIfSimulatingFailure(string call)
        {
            if (!this.SimulateFailure)
                return;

            throw new InvalidOperationException("DocumentService." + call + ": the document store did not answer.");
        }

        private static CategoryNode ToNode(CategoryRow row)
        {
            return new CategoryNode
            {
                Id = row.Id,
                Title = row.Title,
                Path = row.Path,
                HasChildren = Catalog.Any(c => c.ParentId == row.Id),
                DocumentCount = row.DocumentCount
            };
        }

        // ------------------------------------------------------------------------------------------------
        // In-memory catalogue (generated once per process; no database — that is the point of the sample)
        // ------------------------------------------------------------------------------------------------

        private sealed class CategoryRow
        {
            public int Id;
            public int ParentId;
            public string Title;
            public string Path;
            public string Kind;            // which document kind this folder holds
            public int DocumentCount;
        }

        private sealed class DocumentRow
        {
            public DocumentSummary Summary;
            public int CategoryId;
            public string Owner;
            public string Description;
        }

        private static readonly List<CategoryRow> Catalog = new List<CategoryRow>();
        private static readonly List<DocumentRow> AllDocuments = new List<DocumentRow>();
        private static readonly Dictionary<int, List<DocumentSummary>> DocumentsByCategory = new Dictionary<int, List<DocumentSummary>>();
        private static readonly Dictionary<int, DocumentRow> DocumentsById = new Dictionary<int, DocumentRow>();

        private static readonly string[] Owners =
        {
            "A. Bergman", "L. Ferrari", "M. Okonkwo", "S. Nakamura", "P. Duarte", "R. Vasquez"
        };

        static DocumentService()
        {
            // ---- top level (IDs 1..6): three customers plus three shared libraries -----------------------
            AddCategory(0, "Contoso");             // 1
            AddCategory(0, "Fabrikam");            // 2
            AddCategory(0, "Northwind");           // 3
            AddCategory(0, "Shared templates");    // 4
            AddCategory(0, "Compliance");          // 5
            AddCategory(0, "Archive 2019-2023");   // 6 — expands to nothing: a real branch with no children

            // ---- second level (IDs 7..18) ---------------------------------------------------------------
            // Note the three folders called "Contracts": 7, 10 and 13. Same text, three different businesses.
            AddCategory(1, "Contracts", "contract", 312);           //  7  ← the walkthrough's "312 rows"
            AddCategory(1, "Invoices", "invoice", 48);              //  8
            AddCategory(1, "Drawings", "drawing", 24);              //  9
            AddCategory(2, "Contracts", "contract", 36);            // 10  same title as 7
            AddCategory(2, "Invoices", "invoice", 52);              // 11
            AddCategory(2, "Drawings", "drawing", 18);              // 12
            AddCategory(3, "Contracts", "contract", 27);            // 13  same title as 7 and 10
            AddCategory(3, "Invoices", "invoice", 19);              // 14
            AddCategory(4, "Contract templates", "contract", 0);    // 15  empty on purpose
            AddCategory(4, "Invoice templates", "invoice", 12);     // 16
            AddCategory(5, "Audits", "contract", 9);                // 17
            AddCategory(5, "Certificates", "contract", 0);          // 18  empty on purpose

            GenerateDocuments();
        }

        private static void AddCategory(int parentId, string title, string kind = null, int documentCount = 0)
        {
            var parent = Catalog.FirstOrDefault(c => c.Id == parentId);
            var row = new CategoryRow
            {
                Id = Catalog.Count + 1,
                ParentId = parentId,
                Title = title,
                Kind = kind,
                DocumentCount = documentCount,
                Path = parent == null ? title : parent.Path + " ▸ " + title
            };
            Catalog.Add(row);
        }

        /// <summary>
        /// Generates the documents category by category, in category-ID order, so document IDs are
        /// contiguous per folder: Contoso ▸ Contracts (category 7) owns DOC-000001 … DOC-000312.
        /// </summary>
        private static void GenerateDocuments()
        {
            var random = new Random(20260410);   // fixed seed: the same catalogue on every run
            var nextId = 1;

            foreach (var category in Catalog.Where(c => c.DocumentCount > 0).OrderBy(c => c.Id))
            {
                var rows = new List<DocumentSummary>(category.DocumentCount);
                var customer = RootTitle(category);

                for (var i = 1; i <= category.DocumentCount; i++)
                {
                    var id = nextId++;
                    var modified = new DateTime(2024, 1, 1).AddDays(random.Next(0, 640)).AddMinutes(random.Next(0, 1440));
                    var isWarning = id % 37 == 0;          // ~1 in 37 documents needs attention

                    var summary = new DocumentSummary
                    {
                        Id = id,
                        Name = BuildName(category.Kind, customer, i, modified.Year),
                        Kind = category.Kind,
                        TypeLabel = TypeLabelFor(category.Kind),
                        SizeKb = 24 + random.Next(0, 4800),
                        Modified = modified,
                        IsWarning = isWarning
                    };

                    var row = new DocumentRow
                    {
                        Summary = summary,
                        CategoryId = category.Id,
                        Owner = Owners[id % Owners.Length],
                        Description = BuildDescription(category.Kind, category.Path, isWarning)
                    };

                    rows.Add(summary);
                    AllDocuments.Add(row);
                    DocumentsById[id] = row;
                }

                DocumentsByCategory[category.Id] = rows;
            }
        }

        private static string RootTitle(CategoryRow category)
        {
            var current = category;
            while (current.ParentId != 0)
                current = Catalog.First(c => c.Id == current.ParentId);
            return current.Title;
        }

        private static string BuildName(string kind, string customer, int index, int year)
        {
            var n = index.ToString("0000", CultureInfo.InvariantCulture);
            switch (kind)
            {
                case "invoice": return "Invoice " + year + "-" + n + " · " + customer;
                case "drawing": return "Assembly drawing A-" + n + " rev " + (char)('A' + (index % 4)) + " · " + customer;
                default: return "Service agreement " + year + "-" + n + " · " + customer;
            }
        }

        private static string TypeLabelFor(string kind)
        {
            switch (kind)
            {
                case "invoice": return "Invoice";
                case "drawing": return "Drawing";
                default: return "Contract";
            }
        }

        private static string BuildDescription(string kind, string path, bool isWarning)
        {
            var what = kind == "invoice"
                ? "Billing document filed under " + path + "."
                : kind == "drawing"
                    ? "Technical drawing filed under " + path + "."
                    : "Signed agreement filed under " + path + ".";

            return isWarning
                ? what + " Flagged: the retention review is overdue, so the row carries the warning icon instead of its type icon."
                : what + " Indexed, retention review up to date.";
        }
    }
}
