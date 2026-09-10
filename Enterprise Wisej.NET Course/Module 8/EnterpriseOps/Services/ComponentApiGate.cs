using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EnterpriseOps.Services
{
    /// <summary>One thing a screen does that a good component API would have made unnecessary.</summary>
    public class ComponentApiFinding
    {
        public ComponentApiFinding(string token, int count, string why)
        {
            this.Token = token;
            this.Count = count;
            this.Why = why;
        }

        /// <summary>The internal the screen touched.</summary>
        public string Token { get; }

        /// <summary>How many times.</summary>
        public int Count { get; }

        /// <summary>Why that is a leak, in one line.</summary>
        public string Why { get; }

        public override string ToString() => $"{this.Token} ×{this.Count} — {this.Why}";
    }

    /// <summary>What the gate found in one file.</summary>
    public class ComponentApiReport
    {
        public ComponentApiReport(string fileName, int codeLines, IReadOnlyList<ComponentApiFinding> findings, int inlineControls)
        {
            this.FileName = fileName;
            this.CodeLines = codeLines;
            this.Findings = findings;
            this.InlineControls = inlineControls;
        }

        public string FileName { get; }

        /// <summary>Lines left after comments and string literals were removed.</summary>
        public int CodeLines { get; }

        public IReadOnlyList<ComponentApiFinding> Findings { get; }

        /// <summary>How many controls the screen builds by hand (a copy-pasted component looks like this).</summary>
        public int InlineControls { get; }

        public bool Passed => this.Findings.Count == 0;

        public string Verdict => this.Passed
            ? $"{this.FileName}: uses the component API only — 0 internals touched, {this.InlineControls} controls built by hand."
            : $"{this.FileName}: {this.Findings.Count} kind(s) of component internals touched, {this.InlineControls} controls built by hand.";
    }

    /// <summary>
    /// <b>The component API gate.</b> It answers the lab's first review question mechanically:
    /// <i>can another developer use the component without reading its internals?</i>
    ///
    /// <para>
    /// It reads a screen's source file, throws away comments and string literals (so a file that merely
    /// <i>talks</i> about <c>InitScript</c> in a comment is not punished for it) and counts the places where
    /// the screen reaches past the component's public API into its plumbing: registering packages, editing
    /// raw options, wiring client events, evaluating JavaScript, knowing the vendor's global name, or
    /// hand-writing the wire payload.
    /// </para>
    ///
    /// <para>
    /// The lab screen runs it over two files: <c>Controls/Samples/LeakyChartScreen.cs.txt</c> — the
    /// anti-pattern the walkthrough opens with, a screen that rebuilds the timeline inline and drives the
    /// chart's JavaScript itself — and <c>UI/WorkOrderHistoryPage.cs</c>, the same screen written against
    /// the components' public API.
    /// </para>
    /// </summary>
    public class ComponentApiGate
    {
        private readonly IActivityTrace _trace;

        public ComponentApiGate(IActivityTrace trace)
        {
            _trace = trace;
        }

        /// <summary>The internals a screen must not need. Token → why it is a leak.</summary>
        private static readonly (string Token, string Why)[] Rules =
        {
            ("Packages.Add", "registers vendor packages on a screen — one forgotten screen ships a broken chart"),
            ("InitScript", "pastes a client adapter into a screen — five screens, five versions of the adapter"),
            ("WiredEvents", "declares the client/server event contract per screen instead of once in the component"),
            (".Options", "edits the raw widget options — the vendor's option names leak into the application"),
            ("Application.Eval", "drives the browser from a screen; nothing on the server knows what happened"),
            ("EnterpriseOpsChart", "hard-codes the vendor's global name; a vendor swap becomes a search-and-replace"),
            ("new Widget", "uses a raw Widget instead of a wrapper, so failure handling is nobody's job"),
            ("WidgetEvent", "handles the raw, untyped widget event instead of a named server event with typed arguments"),
        };

        /// <summary>Runs the gate over one source file and traces the result.</summary>
        public ComponentApiReport Check(string path)
        {
            string fileName = Path.GetFileName(path ?? "");
            _trace.Write($"Service: ComponentApiGate.Check('{fileName}') — comments and string literals removed first");

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                _trace.Write($"Service: '{fileName}' not found next to the running application — nothing checked");
                return new ComponentApiReport(fileName, 0, new List<ComponentApiFinding>(), 0);
            }

            string code = StripCommentsAndStrings(File.ReadAllText(path));

            var findings = new List<ComponentApiFinding>();
            foreach ((string token, string why) in Rules)
            {
                int count = CountOf(code, token);
                if (count > 0)
                    findings.Add(new ComponentApiFinding(token, count, why));
            }

            int codeLines = code.Split('\n').Count(l => l.Trim().Length > 0);
            int inlineControls = CountOf(code, "new Label") + CountOf(code, "new Panel") + CountOf(code, "new Wisej.Web.Label");

            var report = new ComponentApiReport(fileName, codeLines, findings, inlineControls);
            _trace.Write("Service: " + report.Verdict);
            foreach (ComponentApiFinding finding in findings)
                _trace.Write("Service:   ✗ " + finding);

            if (report.Passed)
                _trace.Write("Service:   ✓ every component interaction goes through a typed property, a method or a named event");

            return report;
        }

        private static int CountOf(string text, string token)
        {
            int count = 0, index = 0;
            while ((index = text.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += token.Length;
            }
            return count;
        }

        /// <summary>
        /// Removes line comments, block comments and string literals (verbatim ones included). A screen that
        /// documents the contract in a comment is doing the right thing; only executable code counts.
        /// </summary>
        private static string StripCommentsAndStrings(string source)
        {
            var output = new StringBuilder(source.Length);
            int i = 0;

            while (i < source.Length)
            {
                char c = source[i];

                // line comment
                if (c == '/' && i + 1 < source.Length && source[i + 1] == '/')
                {
                    while (i < source.Length && source[i] != '\n') i++;
                    continue;
                }

                // block comment
                if (c == '/' && i + 1 < source.Length && source[i + 1] == '*')
                {
                    i += 2;
                    while (i + 1 < source.Length && !(source[i] == '*' && source[i + 1] == '/')) i++;
                    i = Math.Min(source.Length, i + 2);
                    continue;
                }

                // verbatim string
                if (c == '@' && i + 1 < source.Length && source[i + 1] == '"')
                {
                    i += 2;
                    while (i < source.Length)
                    {
                        if (source[i] == '"')
                        {
                            if (i + 1 < source.Length && source[i + 1] == '"') { i += 2; continue; }
                            i++;
                            break;
                        }
                        if (source[i] == '\n') output.Append('\n');
                        i++;
                    }
                    continue;
                }

                // regular string (interpolated or not)
                if (c == '"')
                {
                    i++;
                    while (i < source.Length && source[i] != '"')
                    {
                        if (source[i] == '\\') i++;
                        i++;
                    }
                    i++;
                    continue;
                }

                // char literal
                if (c == '\'')
                {
                    i++;
                    while (i < source.Length && source[i] != '\'')
                    {
                        if (source[i] == '\\') i++;
                        i++;
                    }
                    i++;
                    continue;
                }

                output.Append(c);
                i++;
            }

            return output.ToString();
        }
    }
}
