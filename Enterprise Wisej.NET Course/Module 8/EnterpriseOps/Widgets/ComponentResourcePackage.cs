using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EnterpriseOps.Widgets
{
    /// <summary>What kind of file a component resource is, which decides how it is delivered.</summary>
    public enum ComponentResourceKind
    {
        /// <summary>A third-party script, registered as a Widget package. Loaded once per page, in list order.</summary>
        VendorScript,
        /// <summary>The component's own stylesheet, registered as a Widget package AFTER the vendor so its rules win.</summary>
        ComponentStyle,
        /// <summary>The client adapter, handed to the widget as its InitScript. Never a package, never a loose file.</summary>
        Adapter,
    }

    /// <summary>One entry of the component's resource package.</summary>
    public sealed class ComponentResource
    {
        public ComponentResource(int order, ComponentResourceKind kind, string name, string version, string embeddedName, string source)
        {
            this.Order = order;
            this.Kind = kind;
            this.Name = name;
            this.Version = version;
            this.EmbeddedName = embeddedName;
            this.Source = source;
        }

        /// <summary>Load order. Lower loads first; the component's stylesheet always loads after the vendor's script.</summary>
        public int Order { get; }

        public ComponentResourceKind Kind { get; }

        /// <summary>Package name (the framework de-duplicates by this name across every widget on the page).</summary>
        public string Name { get; }

        /// <summary>The vendor version this wrapper was written against — visible in the manifest and in the release notes.</summary>
        public string Version { get; }

        /// <summary>The logical name of the copy compiled into this assembly (see EnterpriseOps.csproj).</summary>
        public string EmbeddedName { get; }

        /// <summary>The path the browser fetches it from, or null for the adapter (which is never served).</summary>
        public string Source { get; }
    }

    /// <summary>
    /// <b>The embedded resource package of the EnterpriseOps component library</b> — the lab's third
    /// deliverable, and the "one place that lists what is loaded and in what order" the lesson asks for.
    ///
    /// <para>
    /// Every file the components need is compiled into this assembly as an embedded resource. Referencing
    /// the assembly therefore brings the vendor script, the component stylesheet and the client adapter with
    /// it — there are no loose files for an application team to copy, forget, or let drift out of sync.
    /// </para>
    ///
    /// <para>
    /// <b>The convention</b> (one folder per component, versioned vendor names, one ordered list):
    /// </para>
    /// <list type="number">
    ///   <item><description><c>vendor-opschart.js</c> — the vendor library, version <see cref="VendorVersion"/>.</description></item>
    ///   <item><description><c>opschart.css</c> — the component's own stylesheet; loads <b>after</b> the vendor so its overrides win.</description></item>
    ///   <item><description><c>opschart-init.js</c> — the client adapter, handed to the widget as its InitScript.</description></item>
    /// </list>
    ///
    /// <para>
    /// <b>Why the first two are also on disk under <c>Widgets/</c>:</b> a Widget package is fetched by the
    /// browser over HTTP, so a served copy has to exist somewhere. The embedded copy is the source of truth
    /// and <see cref="Verify"/> compares the two, so a stale hand-edited file on disk is reported instead of
    /// silently shipping. Upgrading the vendor means changing one folder and this one list.
    /// </para>
    /// </summary>
    public static class ComponentResourcePackage
    {
        /// <summary>The name of the package as a whole (what a release note names).</summary>
        public const string PackageName = "EnterpriseOps.Components";

        /// <summary>The version of the package as a whole.</summary>
        public const string PackageVersion = "1.2.0";

        /// <summary>The version of the third-party chart library this release was written against.</summary>
        public const string VendorVersion = "1.2";

        private static readonly ComponentResource[] _all =
        {
            new ComponentResource(1, ComponentResourceKind.VendorScript, "eops-chart-vendor", VendorVersion,
                "EnterpriseOps.Widgets.vendor-opschart.js", "Widgets/vendor-opschart.js"),

            new ComponentResource(2, ComponentResourceKind.ComponentStyle, "eops-chart-style", PackageVersion,
                "EnterpriseOps.Widgets.opschart.css", "Widgets/opschart.css"),

            new ComponentResource(3, ComponentResourceKind.Adapter, "eops-chart-adapter", PackageVersion,
                "EnterpriseOps.Widgets.opschart-init.js", null),
        };

        /// <summary>Every resource of the package, in load order.</summary>
        public static IReadOnlyList<ComponentResource> All => _all;

        /// <summary>The resources the widget registers as packages, in load order (vendor script, then component style).</summary>
        public static IEnumerable<ComponentResource> Packages
            => _all.Where(r => r.Kind != ComponentResourceKind.Adapter).OrderBy(r => r.Order);

        /// <summary>The client adapter entry.</summary>
        public static ComponentResource Adapter
            => _all.First(r => r.Kind == ComponentResourceKind.Adapter);

        /// <summary>Reads one embedded resource as text, or null when it was not compiled in.</summary>
        public static string Read(string embeddedName)
        {
            using (Stream stream = typeof(ComponentResourcePackage).Assembly.GetManifestResourceStream(embeddedName))
            {
                if (stream == null)
                    return null;

                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        /// <summary>The size in bytes of the embedded copy, or -1 when it was not compiled in.</summary>
        public static int SizeOf(string embeddedName)
        {
            using (Stream stream = typeof(ComponentResourcePackage).Assembly.GetManifestResourceStream(embeddedName))
                return stream == null ? -1 : (int)stream.Length;
        }

        /// <summary>
        /// The manifest a reviewer reads: what is in the package, in which order, how big the embedded copy
        /// is, and whether the served copy still matches it.
        /// </summary>
        public static IEnumerable<string> Describe()
        {
            yield return $"{PackageName} {PackageVersion} · vendor EnterpriseOpsChart {VendorVersion} · assembly {typeof(ComponentResourcePackage).Assembly.GetName().Name}";

            foreach (ComponentResource resource in _all.OrderBy(r => r.Order))
            {
                int size = SizeOf(resource.EmbeddedName);
                string state = size < 0 ? "MISSING from the assembly" : $"{size} bytes embedded";
                string delivery = resource.Kind == ComponentResourceKind.Adapter
                    ? "InitScript (never served)"
                    : $"package '{resource.Name}' → /{resource.Source}";

                yield return $"  {resource.Order}. {resource.Kind} {Path.GetFileName(resource.EmbeddedName.Replace("EnterpriseOps.Widgets.", ""))} v{resource.Version} · {delivery} · {state}";
            }
        }

        /// <summary>
        /// Compares the embedded copy of every served resource with the file on disk under
        /// <paramref name="applicationRoot"/>. Returns one line per problem; an empty result means the
        /// package is intact.
        /// </summary>
        public static IReadOnlyList<string> Verify(string applicationRoot)
        {
            var problems = new List<string>();

            foreach (ComponentResource resource in _all.OrderBy(r => r.Order))
            {
                string embedded = Read(resource.EmbeddedName);
                if (embedded == null)
                {
                    problems.Add($"{resource.EmbeddedName} is declared in the package but is not an EmbeddedResource in the csproj.");
                    continue;
                }

                if (resource.Source == null)
                    continue;                       // the adapter is never served: nothing to compare

                string path = Path.Combine(applicationRoot ?? ".", resource.Source.Replace('/', Path.DirectorySeparatorChar));
                if (!File.Exists(path))
                {
                    problems.Add($"{resource.Source} is registered as a package but no file is served from there (the browser would get a 404).");
                    continue;
                }

                string served = File.ReadAllText(path);
                if (Normalize(served) != Normalize(embedded))
                    problems.Add($"{resource.Source} on disk differs from the copy embedded in the assembly — the served file was edited by hand.");
            }

            return problems;
        }

        private static string Normalize(string text)
            => (text ?? "").Replace("\r\n", "\n").TrimEnd();
    }
}
