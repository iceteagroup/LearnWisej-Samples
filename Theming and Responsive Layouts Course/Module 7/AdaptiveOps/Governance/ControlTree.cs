using System;
using System.Collections.Generic;
using System.Reflection;
using Wisej.Web;

namespace AdaptiveOps.Governance
{
    /// <summary>
    /// Read-only helpers over the live control tree. Used by the "Layout cost" card (how many controls
    /// each region owns) and by the governance review (which controls carry hard-coded colours or fonts,
    /// which appearance keys and CSS classes are in use). Nothing here changes a control.
    /// </summary>
    public static class ControlTree
    {
        private const BindingFlags AnyInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        // Control.ShouldSerializeBackColor / ForeColor / Font are the framework's own "was this property set
        // explicitly?" answers (non-public, WinForms convention). They return false while the theme owns the
        // value and true after code or the Designer assigned one — exactly the distinction the review needs.
        private static readonly MethodInfo ShouldSerializeBackColor = typeof(Control).GetMethod("ShouldSerializeBackColor", AnyInstance, null, Type.EmptyTypes, null);
        private static readonly MethodInfo ShouldSerializeForeColor = typeof(Control).GetMethod("ShouldSerializeForeColor", AnyInstance, null, Type.EmptyTypes, null);
        private static readonly MethodInfo ShouldSerializeFont = typeof(Control).GetMethod("ShouldSerializeFont", AnyInstance, null, Type.EmptyTypes, null);

        /// <summary>The control itself followed by every descendant, depth first.</summary>
        public static IEnumerable<Control> SelfAndDescendants(Control root)
        {
            if (root == null)
                yield break;

            yield return root;

            foreach (Control child in root.Controls)
                foreach (var c in SelfAndDescendants(child))
                    yield return c;
        }

        /// <summary>Number of controls in the subtree, the root included.</summary>
        public static int Count(Control root)
        {
            int n = 0;
            foreach (var _ in SelfAndDescendants(root))
                n++;
            return n;
        }

        /// <summary>True when code (or the Designer) assigned BackColor explicitly instead of leaving it to the theme.</summary>
        public static bool HasExplicitBackColor(Control c) => Invoke(ShouldSerializeBackColor, c);

        /// <summary>True when code (or the Designer) assigned ForeColor explicitly instead of leaving it to the theme.</summary>
        public static bool HasExplicitForeColor(Control c) => Invoke(ShouldSerializeForeColor, c);

        /// <summary>True when code (or the Designer) assigned Font explicitly instead of leaving it to the theme.</summary>
        public static bool HasExplicitFont(Control c) => Invoke(ShouldSerializeFont, c);

        /// <summary>Whether the reflection hooks were found; the review reports "not checkable" instead of a false pass when they were not.</summary>
        public static bool CanInspectExplicitValues =>
            ShouldSerializeBackColor != null && ShouldSerializeForeColor != null && ShouldSerializeFont != null;

        /// <summary>A short, stable label for a control in trace lines and evidence: "Panel cardOpen".</summary>
        public static string Describe(Control c)
        {
            if (c == null)
                return "(null)";
            string name = string.IsNullOrEmpty(c.Name) ? "(unnamed)" : c.Name;
            return c.GetType().Name + " " + name;
        }

        private static bool Invoke(MethodInfo method, Control c)
        {
            if (method == null || c == null)
                return false;
            try
            {
                return (bool)method.Invoke(c, null);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
