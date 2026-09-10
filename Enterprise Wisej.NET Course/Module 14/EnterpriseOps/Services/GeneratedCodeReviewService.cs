using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>What a review run returns: the command outcome (was the reviewer allowed to run it) and the report.</summary>
    public sealed class ReviewOutcome
    {
        public CommandResult Result { get; set; }
        public ReviewReport Report { get; set; }
    }

    /// <summary>
    /// The generated-code review checklist, executed instead of remembered.
    ///
    /// The rule the module teaches is that generated code is a draft from an unknown contributor: it goes
    /// through the same review as any pull request. This service is that review — ten fixed questions run
    /// over the text of a change, each one reporting the line it fired on, why it fails and what the accepted
    /// version does instead. Nothing here calls a model or a network: the checks are deterministic string and
    /// structure analysis, which is the point — a review gate that needs an AI to run is not a gate.
    ///
    /// It is a reviewer's assistant, not a compiler. It answers the questions a reviewer would otherwise have
    /// to remember to ask, and it is deliberately loud: one Reject finding stops the change.
    /// </summary>
    public sealed class GeneratedCodeReviewService
    {
        private readonly PermissionService _permissions;
        private readonly AuditLog _audit;
        private readonly ActivityTrace _trace;
        private readonly List<ReviewDecision> _decisions = new List<ReviewDecision>();

        public GeneratedCodeReviewService(PermissionService permissions, AuditLog audit, ActivityTrace trace)
        {
            _permissions = permissions;
            _audit = audit;
            _trace = trace;
        }

        /// <summary>The checklist itself — the same list as docs/GeneratedCodeReviewChecklist.md, in the same order.</summary>
        public IReadOnlyList<ReviewRule> Rules => RuleSet;

        /// <summary>Which generated code was accepted, by whom and why (the lab's first review question).</summary>
        public IReadOnlyList<ReviewDecision> Decisions => _decisions;

        /// <summary>The report the screen is showing, so a decision can be signed against it.</summary>
        public ReviewReport LastReport { get; private set; }

        private static readonly List<ReviewRule> RuleSet = new List<ReviewRule>
        {
            new ReviewRule { Id = "Q1", Severity = ReviewSeverity.Reject,  DocReference = "docs/GeneratedCodeReviewChecklist.md#q1", Question = "Did the model invent a Wisej.NET API?" },
            new ReviewRule { Id = "Q2", Severity = ReviewSeverity.Reject,  DocReference = "docs/GeneratedCodeReviewChecklist.md#q2", Question = "Does any generated code store user, tenant, selected entity or workflow state in static fields?" },
            new ReviewRule { Id = "Q3", Severity = ReviewSeverity.Reject,  DocReference = "docs/GeneratedCodeReviewChecklist.md#q3", Question = "Is authorization enforced in services?" },
            new ReviewRule { Id = "Q4", Severity = ReviewSeverity.Warning, DocReference = "docs/GeneratedCodeReviewChecklist.md#q4", Question = "Are HTML-capable paths reviewed?" },
            new ReviewRule { Id = "Q5", Severity = ReviewSeverity.Reject,  DocReference = "docs/GeneratedCodeReviewChecklist.md#q5", Question = "Are background tasks tied unsafely to controls or sessions?" },
            new ReviewRule { Id = "Q6", Severity = ReviewSeverity.Warning, DocReference = "docs/GeneratedCodeReviewChecklist.md#q6", Question = "Can the code be tested without the UI?" },
            new ReviewRule { Id = "R7", Severity = ReviewSeverity.Reject,  DocReference = "docs/GeneratedCodeReviewChecklist.md#r7", Question = "Is every failure visible — no exception swallowed?" },
            new ReviewRule { Id = "R8", Severity = ReviewSeverity.Warning, DocReference = "docs/GeneratedCodeReviewChecklist.md#r8", Question = "Is every disposable resource disposed?" },
            new ReviewRule { Id = "R9", Severity = ReviewSeverity.Reject,  DocReference = "docs/GeneratedCodeReviewChecklist.md#r9", Question = "Is client-supplied identity or scope re-established on the server, never trusted?" },
            new ReviewRule { Id = "R10", Severity = ReviewSeverity.Warning, DocReference = "docs/GeneratedCodeReviewChecklist.md#r10", Question = "Does the event handler stay thin — a service call, not the logic?" },
        };

        /// <summary>
        /// Runs the checklist. Authorization first: a review is signed by a reviewer, not by the author, so
        /// a Technician gets the same refusal here as anywhere else in the Command Center.
        /// </summary>
        public async Task<ReviewOutcome> ReviewAsync(string code, string pullRequest, CommandContext ctx)
        {
            var outcome = new ReviewOutcome();

            string refusal = _permissions.Check(ctx.User, Permission.ReviewGeneratedCode);
            if (refusal != null)
            {
                _audit.Write(ctx.TenantId, ctx.UserName, "ReviewGeneratedCode", pullRequest, false, refusal, ctx.CorrelationId);
                _trace.Security($"ReviewGeneratedCode denied — {refusal}");
                outcome.Result = CommandResult.Fail(ctx.CorrelationId, refusal);
                return outcome;
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                outcome.Result = CommandResult.Fail(ctx.CorrelationId, "There is nothing to review — load a draft or paste a change first.");
                return outcome;
            }

            _trace.Review($"running {RuleSet.Count} checklist rules over {pullRequest} · catalog {DocumentedApiCatalog.MemberCount} documented members ({DocumentedApiCatalog.Source})");
            await Task.Delay(150).ConfigureAwait(true);      // the review takes a moment, like a human one

            ReviewReport report = Run(code, pullRequest, ctx);
            LastReport = report;

            foreach (ReviewFinding finding in report.Findings)
                _trace.Review($"{finding.RuleId} L{finding.Line} [{finding.Severity}] {finding.Message}");

            _audit.Write(ctx.TenantId, ctx.UserName, "ReviewGeneratedCode", pullRequest, true,
                $"{report.Verdict}, {report.Findings.Count} finding(s) over {report.LinesReviewed} lines", ctx.CorrelationId);
            _trace.Service($"review verdict: {report.Verdict} — {report.Findings.Count} finding(s), {report.RulesRun} rules run");

            outcome.Report = report;
            outcome.Result = report.Verdict == ReviewVerdict.Rejected
                ? CommandResult.Fail(ctx.CorrelationId, $"{report.Findings.Count(f => f.Severity == ReviewSeverity.Reject)} blocking finding(s) — the change goes back with the prompt header attached.")
                : CommandResult.Ok(ctx.CorrelationId);
            return outcome;
        }

        /// <summary>
        /// Records the decision the lab asks for: which generated code was accepted, by whom and why.
        /// docs/AIUsageNotes.md is the written copy; this is the in-session log the screen shows.
        /// </summary>
        public CommandResult RecordDecision(ReviewReport report, string reason, CommandContext ctx)
        {
            if (report == null)
                return CommandResult.Fail(ctx.CorrelationId, "Run the checklist before signing a decision.");

            string refusal = _permissions.Check(ctx.User, Permission.ReviewGeneratedCode);
            if (refusal != null)
            {
                _audit.Write(ctx.TenantId, ctx.UserName, "SignReviewDecision", report.PullRequest, false, refusal, ctx.CorrelationId);
                _trace.Security($"SignReviewDecision denied — {refusal}");
                return CommandResult.Fail(ctx.CorrelationId, refusal);
            }

            if (string.Equals(report.Author, ctx.UserName, StringComparison.OrdinalIgnoreCase))
                return CommandResult.Fail(ctx.CorrelationId, "The author cannot sign their own review.");

            report.ReviewedBy = ctx.UserName;
            report.ReviewedUtc = DateTime.UtcNow;

            var decision = new ReviewDecision
            {
                DecidedUtc = DateTime.UtcNow,
                PullRequest = report.PullRequest,
                Verdict = report.Verdict,
                Reviewer = ctx.UserName,
                Author = report.Author,
                Reason = reason,
                CorrelationId = ctx.CorrelationId,
            };
            _decisions.Add(decision);

            _audit.Write(ctx.TenantId, ctx.UserName, "SignReviewDecision", report.PullRequest, true, $"{report.Verdict}: {reason}", ctx.CorrelationId);
            _trace.Review($"decision recorded — {report.PullRequest} {report.Verdict} by {ctx.UserName}: {reason}");
            return CommandResult.Ok(ctx.CorrelationId);
        }

        // ── the checklist ────────────────────────────────────────────────────────────────────────────────

        /// <summary>Runs every rule. Public and free of UI types, so the whole gate is unit-testable (checklist Q6, applied to itself).</summary>
        public ReviewReport Run(string code, string pullRequest, CommandContext ctx)
        {
            string[] original = (code ?? "").Replace("\r\n", "\n").Split('\n');
            string[] source = original.Select(StripCommentsAndStrings).ToArray();

            var report = new ReviewReport
            {
                PullRequest = pullRequest,
                Author = "ai-assistant",
                ReviewedUtc = DateTime.UtcNow,
                RulesRun = RuleSet.Count,
                LinesReviewed = original.Length,
                CorrelationId = ctx.CorrelationId,
            };

            CheckInventedApis(original, source, report);
            CheckStaticState(original, source, report);
            CheckAuthorization(original, source, report);
            CheckHtmlPaths(original, source, report);
            CheckBackgroundTasks(original, source, report);
            CheckTestableWithoutUi(original, source, report);
            CheckSwallowedExceptions(original, source, report);
            CheckDisposables(original, source, report);
            CheckTrustedClientInput(original, source, report);
            CheckThinHandlers(original, source, report);

            report.Verdict =
                report.Findings.Any(f => f.Severity == ReviewSeverity.Reject) ? ReviewVerdict.Rejected :
                report.Findings.Count > 0 ? ReviewVerdict.AcceptedWithWarnings :
                ReviewVerdict.Accepted;

            return report;
        }

        // Q1 — an API that is not in the documentation is not an API yet.
        private static readonly Regex MemberAccess = new Regex(@"(?<receiver>[A-Za-z_][A-Za-z0-9_]*)\s*\.\s*(?<member>[A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);

        /// <summary>Field / variable name prefixes the project's naming convention reserves for controls.</summary>
        private static readonly string[] ControlPrefixes =
        {
            "grid", "dgv", "btn", "txt", "lst", "lbl", "pnl", "cbo", "chk", "rad", "tab", "tree", "img",
            "pic", "dlg", "frm", "wnd", "menu", "tool", "gauge", "widget", "page", "form", "combo", "upload",
        };

        private static void CheckInventedApis(string[] original, string[] source, ReviewReport report)
        {
            for (int i = 0; i < source.Length; i++)
            {
                foreach (Match match in MemberAccess.Matches(source[i]))
                {
                    string receiver = match.Groups["receiver"].Value;
                    string member = match.Groups["member"].Value;
                    if (!IsPlatformReceiver(receiver) || DocumentedApiCatalog.IsDocumented(member))
                        continue;

                    Add(report, "Q1", i + 1, original[i],
                        $"{receiver}.{member} is not in the Wisej.NET documentation — the catalog built from {DocumentedApiCatalog.Source} has {DocumentedApiCatalog.MemberCount} documented members and this is not one of them.",
                        "Use a documented member and cite it in the change (rev 2 uses DataGridView.VirtualMode), or prove the API exists before merging.");
                }
            }
        }

        /// <summary>True when the receiver is a Wisej.NET type used statically, or a field named like a control.</summary>
        private static bool IsPlatformReceiver(string receiver)
        {
            if (DocumentedApiCatalog.IsDocumentedType(receiver))
                return true;

            string name = receiver.TrimStart('_');
            foreach (string prefix in ControlPrefixes)
            {
                if (name.Length < prefix.Length || !name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (name.Length == prefix.Length || char.IsUpper(name[prefix.Length]) || char.IsDigit(name[prefix.Length]))
                    return true;
            }
            return false;
        }

        // Q2 — a static field is shared by every session in the process.
        private static readonly Regex StaticField = new Regex(
            @"^\s*(?:public|private|internal|protected)?\s*static\s+(?!readonly\b|class\b|void\b|async\b)(?<type>[A-Za-z_][\w\.\?]*)\s+(?<name>[A-Za-z_]\w*)\s*(?:=[^;]*)?;",
            RegexOptions.Compiled);

        private static readonly Regex StaticMutableProperty = new Regex(
            @"^\s*(?:public|internal|protected)?\s*static\s+[\w<>,\.\?\[\]]+\s+\w+\s*\{[^}]*\bset\s*;", RegexOptions.Compiled);

        private static void CheckStaticState(string[] original, string[] source, ReviewReport report)
        {
            for (int i = 0; i < source.Length; i++)
            {
                Match field = StaticField.Match(source[i]);
                if (field.Success)
                {
                    Add(report, "Q2", i + 1, original[i],
                        $"static {field.Groups["type"].Value} {field.Groups["name"].Value} — one process serves every browser session, so every tenant would share this value.",
                        "Make it an instance field of a per-session service and pass the tenant and user in the CommandContext.");
                    continue;
                }

                if (StaticMutableProperty.IsMatch(source[i]))
                {
                    Add(report, "Q2", i + 1, original[i],
                        "a static property with a setter is session state in disguise — the second user overwrites the first.",
                        "Move it to SessionContext (one instance per browser session), never to a static.");
                }
            }
        }

        // Q3 — a command that changes data has to be authorised where the decision is made: in the service.
        private static readonly Regex AuthorizationToken = new Regex(@"Permission\.|IsGranted|\.Check\(|Authorize|_permissions|IsInRole|Demand\(", RegexOptions.Compiled);
        private static readonly Regex MutatingCall = new Regex(@"SaveChanges|\.Update\(|\.Insert\(|\.Delete\(|Status\s*=\s*\w*Status\.", RegexOptions.Compiled);
        private static readonly Regex MutatingMethod = new Regex(@"\b(?:Approve|Reject|Delete|Cancel|Escalate|Reassign)\w*\s*\(", RegexOptions.Compiled);

        private static void CheckAuthorization(string[] original, string[] source, ReviewReport report)
        {
            if (source.Any(line => AuthorizationToken.IsMatch(line)))
                return;

            for (int i = 0; i < source.Length; i++)
            {
                if (!MutatingCall.IsMatch(source[i]) && !MutatingMethod.IsMatch(source[i]))
                    continue;

                Add(report, "Q3", i + 1, original[i],
                    "this change writes (or commands a write) and the diff contains no permission check at all — authorization would depend on which screen happens to call it.",
                    "Check the permission inside the service before the write, return the refusal as a CommandResult, and audit the denial.");
                return;                                     // one finding is enough; the whole change is missing the gate
            }
        }

        // Q4 — anything that can render HTML is an injection path until someone has looked at it.
        private static readonly Regex HtmlPath = new Regex(@"AllowHtml\s*=\s*true|innerHTML|\.Html\s*=|Eval\([^)]*\+|document\.write", RegexOptions.Compiled);

        private static void CheckHtmlPaths(string[] original, string[] source, ReviewReport report)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (!HtmlPath.IsMatch(source[i]))
                    continue;
                Add(report, "Q4", i + 1, original[i],
                    "an HTML-capable path built from data — if any part of it can come from a user, this renders their markup.",
                    "Keep AllowHtml off, or encode the value and record in the change why the path is safe.");
            }
        }

        // Q5 — a background task that outlives its page or pushes without a guard takes the session with it.
        private static readonly Regex BackgroundTask = new Regex(@"Application\.StartTask|Task\.Run\(|new\s+Thread\(|BackgroundJob|new\s+Timer\(", RegexOptions.Compiled);

        private static void CheckBackgroundTasks(string[] original, string[] source, ReviewReport report)
        {
            bool guarded = source.Any(line => line.Contains("IsDisposed")) && source.Any(line => line.Contains("Application.Update("));
            if (guarded)
                return;

            for (int i = 0; i < source.Length; i++)
            {
                if (!BackgroundTask.IsMatch(source[i]))
                    continue;
                Add(report, "Q5", i + 1, original[i],
                    "a background task with no IsDisposed guard and no Application.Update — it will touch controls of a session that may already be gone.",
                    "Bound the loop, check IsDisposed before every push, call Application.Update(page) from the task and catch ObjectDisposedException.");
                return;
            }
        }

        // Q6 — logic reachable only through a control is logic that can only be tested by clicking.
        private static readonly Regex MethodDeclaration = new Regex(@"^\s*(?:public|private|internal|protected)[^;=]*?\b(?<name>\w+)\s*\((?<parameters>[^)]*)\)\s*$", RegexOptions.Compiled);
        private static readonly Regex PersistenceInBody = new Regex(@"\bDb\.|SaveChanges|\.ToList\(\)|\.Where\(|new\s+Sql|File\.(Read|Write)|HttpClient", RegexOptions.Compiled);

        private static void CheckTestableWithoutUi(string[] original, string[] source, ReviewReport report)
        {
            for (int i = 0; i < source.Length; i++)
            {
                Match declaration = MethodDeclaration.Match(source[i]);
                if (!declaration.Success || !TakesAControl(declaration.Groups["parameters"].Value))
                    continue;

                int end = EndOfBlock(source, i);
                for (int line = i + 1; line <= end && line < source.Length; line++)
                {
                    if (!PersistenceInBody.IsMatch(source[line]))
                        continue;
                    Add(report, "Q6", line + 1, original[line],
                        $"{declaration.Groups["name"].Value} takes a control and queries the database in the same method — the only way to test this rule is to open the screen.",
                        "Move the query behind a service that takes a CommandContext and returns rows; let the screen bind what it is given.");
                    break;
                }
            }
        }

        private static bool TakesAControl(string parameters)
        {
            foreach (string parameter in parameters.Split(','))
            {
                string[] parts = parameter.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && DocumentedApiCatalog.IsDocumentedType(parts[parts.Length - 2]))
                    return true;
            }
            return false;
        }

        // R7 — an empty catch turns a production incident into a mystery.
        private static void CheckSwallowedExceptions(string[] original, string[] source, ReviewReport report)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (!Regex.IsMatch(source[i], @"^\s*catch\b"))
                    continue;

                int end = EndOfBlock(source, i);
                var body = new List<string>();
                for (int line = i; line <= end && line < source.Length; line++)
                {
                    string text = source[line];
                    if (line == i)
                        text = Regex.Replace(text, @"^\s*catch\b[^{]*", "");
                    body.Add(text.Replace("{", "").Replace("}", "").Trim());
                }

                string content = string.Join(" ", body.Where(b => b.Length > 0)).Trim();
                if (content.Length != 0 && content != "return;" && content != "return null;")
                    continue;

                Add(report, "R7", i + 1, original[i],
                    "the exception is caught and thrown away — the user sees nothing, the log has nothing, and the incident is invisible.",
                    "Log with the correlation id and show a generic message; let unexpected failures be visible, just not detailed.");
            }
        }

        // R8 — a resource that is never released is a leak with a delay fuse.
        private static readonly Regex Disposable = new Regex(@"new\s+(SqlConnection|SqlCommand|StreamReader|StreamWriter|FileStream|HttpClient|MemoryStream|Bitmap|Graphics)\s*\(", RegexOptions.Compiled);

        private static void CheckDisposables(string[] original, string[] source, ReviewReport report)
        {
            bool disposedSomewhere = source.Any(line => line.Contains(".Dispose()"));
            for (int i = 0; i < source.Length; i++)
            {
                if (!Disposable.IsMatch(source[i]) || source[i].Contains("using") || disposedSomewhere)
                    continue;
                Add(report, "R8", i + 1, original[i],
                    "a disposable resource is created with no using statement and nothing disposes it in the diff.",
                    "Wrap it in a using statement, or own it in a class that disposes it.");
            }
        }

        // R9 — identity and scope are re-established on the server, never taken from the caller.
        private static readonly Regex TrustedClientInput = new Regex(
            @"\b_?(?:tenant|user|role|permission)\w*\s*=\s*[^;]*\b\w*(?:client|request|args|browser|payload|querystring)\w*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static void CheckTrustedClientInput(string[] original, string[] source, ReviewReport report)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (!TrustedClientInput.IsMatch(source[i]))
                    continue;
                Add(report, "R9", i + 1, original[i],
                    "the tenant (or user) is taken from the caller — anyone who can change the request can read another tenant's data.",
                    "Read the tenant and user from the server-side session context and ignore whatever the client sent.");
            }
        }

        // R10 — the handler shape the whole course is built on.
        private static readonly Regex HandlerDeclaration = new Regex(@"\b(?<name>\w+_(?:Click|Load|Changed|Selected|Closing|Tick|DoubleClick))\s*\(", RegexOptions.Compiled);

        private static void CheckThinHandlers(string[] original, string[] source, ReviewReport report)
        {
            for (int i = 0; i < source.Length; i++)
            {
                Match handler = HandlerDeclaration.Match(source[i]);
                if (!handler.Success)
                    continue;

                int end = EndOfBlock(source, i);
                int statements = 0;
                for (int line = i + 1; line <= end && line < source.Length; line++)
                {
                    if (source[line].Trim().EndsWith(";", StringComparison.Ordinal))
                        statements++;
                    if (!PersistenceInBody.IsMatch(source[line]))
                        continue;
                    Add(report, "R10", line + 1, original[line],
                        $"{handler.Groups["name"].Value} does the work itself — persistence inside an event handler cannot be reused, reviewed on its own or tested.",
                        "Keep the handler to try { var result = await _service.…Async(ctx); ShowResult(result); } catch { … }.");
                    statements = -1000;                     // already reported; do not report length as well
                    break;
                }

                if (statements > 12)
                {
                    Add(report, "R10", i + 1, original[i],
                        $"{handler.Groups["name"].Value} is {statements} statements long — a handler that big is a service that has not been written yet.",
                        "Move the decisions into a service and leave the handler asking one question.");
                }
            }
        }

        // ── helpers ──────────────────────────────────────────────────────────────────────────────────────

        private static void Add(ReviewReport report, string ruleId, int line, string evidence, string message, string fix)
        {
            ReviewRule rule = RuleSet.First(r => r.Id == ruleId);
            report.Findings.Add(new ReviewFinding
            {
                RuleId = rule.Id,
                Severity = rule.Severity,
                Line = line,
                Evidence = (evidence ?? "").Trim(),
                Message = message,
                Fix = fix,
                DocReference = rule.DocReference,
            });
        }

        /// <summary>Index of the line that closes the block opened at or after <paramref name="start"/>.</summary>
        private static int EndOfBlock(string[] source, int start)
        {
            int depth = 0;
            bool opened = false;
            for (int i = start; i < source.Length; i++)
            {
                foreach (char c in source[i])
                {
                    if (c == '{') { depth++; opened = true; }
                    else if (c == '}') depth--;
                }
                if (opened && depth <= 0)
                    return i;
            }
            return source.Length - 1;
        }

        /// <summary>
        /// Removes line comments and string literals before matching, so a rule never fires on prose. The
        /// finding still quotes the original line — the reviewer reads the code, not the stripped version.
        /// </summary>
        private static string StripCommentsAndStrings(string line)
        {
            string code = Regex.Replace(line, "@?\"(?:[^\"]|\"\")*\"", "\"\"");
            int comment = code.IndexOf("//", StringComparison.Ordinal);
            return comment >= 0 ? code.Substring(0, comment) : code;
        }
    }
}
