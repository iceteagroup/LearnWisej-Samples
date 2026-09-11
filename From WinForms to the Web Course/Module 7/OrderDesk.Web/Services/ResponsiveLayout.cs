using System;
using System.Drawing;
using Wisej.Core;
using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>The three layouts the migrated Orders screen supports — and nothing else is claimed.</summary>
    public enum LayoutKind
    {
        Desktop,
        Tablet,
        Phone
    }

    /// <summary>
    /// The responsive properties of the Orders screen, expressed in code so the mapping profile → layout is
    /// readable in one place. In Visual Studio the same values would be set per profile in the designer's
    /// responsive-profile dropdown (Control.ResponsiveProfiles); the runtime effect is identical.
    ///
    ///   Desktop  grid and detail side by side · toolbar buttons show icon + full text · actions ComboBox hidden
    ///   Tablet   detail moves BELOW the grid · toolbar buttons icon + short text
    ///   Phone    detail hidden · toolbar hidden · the actions collapse into one ComboBox
    ///
    /// The page calls Apply on load (Application.ActiveProfile) and on Application.ResponsiveProfileChanged.
    /// </summary>
    public sealed class ResponsiveLayout
    {
        private readonly DataGridView _grid;
        private readonly Label _detailTitle;
        private readonly Label _detail;
        private readonly ToolBar _toolBar;
        private readonly ComboBox _actions;
        private readonly TextBox _search;

        private static readonly string[] FullText = { "New Order", "Print Invoice", "Export", "Refresh" };
        private static readonly string[] ShortText = { "New", "Print", "Export", "Refresh" };

        public ResponsiveLayout(DataGridView grid, Label detailTitle, Label detail, ToolBar toolBar, ComboBox actions, TextBox search)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _detailTitle = detailTitle ?? throw new ArgumentNullException(nameof(detailTitle));
            _detail = detail ?? throw new ArgumentNullException(nameof(detail));
            _toolBar = toolBar ?? throw new ArgumentNullException(nameof(toolBar));
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _search = search ?? throw new ArgumentNullException(nameof(search));
        }

        public LayoutKind Current { get; private set; } = LayoutKind.Desktop;

        /// <summary>
        /// Maps a client profile to a layout. Profile names come from ClientProfiles.json (Phone, Tablet,
        /// Desktop). A null profile or the framework's "Default" (nothing matched) is treated as Desktop; a
        /// profile name the screen does not know falls back to the browser width so the page never breaks.
        /// </summary>
        public static LayoutKind FromProfile(ClientProfile profile)
        {
            var name = profile?.Name ?? "";
            if (name.IndexOf("Phone", StringComparison.OrdinalIgnoreCase) >= 0) return LayoutKind.Phone;
            if (name.IndexOf("Tablet", StringComparison.OrdinalIgnoreCase) >= 0) return LayoutKind.Tablet;
            if (name.IndexOf("Desktop", StringComparison.OrdinalIgnoreCase) >= 0) return LayoutKind.Desktop;

            int width = BrowserWidth();
            if (width > 0 && width <= 600) return LayoutKind.Phone;
            if (width > 600 && width <= 1024) return LayoutKind.Tablet;
            return LayoutKind.Desktop;
        }

        public static int BrowserWidth()
        {
            try { return Application.Browser?.Size.Width ?? 0; }
            catch (Exception) { return 0; }
        }

        public static string Describe(ClientProfile profile)
        {
            var name = profile?.Name ?? "Default";
            int width = BrowserWidth();
            int height = 0;
            try { height = Application.Browser?.Size.Height ?? 0; } catch (Exception) { }
            return width > 0 ? $"{name} · browser {width}×{height}" : name;
        }

        /// <summary>Applies one of the three layouts. Returns a one-line description of the layout.</summary>
        public string Apply(LayoutKind kind)
        {
            Current = kind;
            switch (kind)
            {
                case LayoutKind.Phone:
                    _search.Size = new Size(602, 28);
                    _toolBar.Visible = false;
                    _actions.Visible = true;
                    _actions.Location = new Point(20, 116);
                    _actions.Size = new Size(602, 28);
                    _grid.Location = new Point(20, 150);
                    _grid.Size = new Size(602, 84);
                    _detailTitle.Visible = false;
                    _detail.Visible = false;
                    return "phone: detail hidden · toolbar hidden · actions in one ComboBox · grid 602×84";

                case LayoutKind.Tablet:
                    _search.Size = new Size(260, 28);
                    _toolBar.Visible = true;
                    _actions.Visible = false;
                    SetToolBarText(ShortText);
                    _grid.Location = new Point(20, 116);
                    _grid.Size = new Size(602, 66);
                    _detailTitle.Visible = true;
                    _detailTitle.Location = new Point(20, 186);
                    _detailTitle.Size = new Size(602, 18);
                    _detail.Visible = true;
                    _detail.Location = new Point(20, 204);
                    _detail.Size = new Size(602, 30);
                    return "tablet: detail below the grid · toolbar icon + short text · grid 602×66";

                default:
                    _search.Size = new Size(260, 28);
                    _toolBar.Visible = true;
                    _actions.Visible = false;
                    SetToolBarText(FullText);
                    _grid.Location = new Point(20, 116);
                    _grid.Size = new Size(380, 118);
                    _detailTitle.Visible = true;
                    _detailTitle.Location = new Point(416, 116);
                    _detailTitle.Size = new Size(206, 22);
                    _detail.Visible = true;
                    _detail.Location = new Point(416, 138);
                    _detail.Size = new Size(206, 96);
                    return "desktop: grid + detail side by side · toolbar icon + full text · grid 380×118";
            }
        }

        /// <summary>Detail text for the current layout: five lines on desktop, one line on tablet.</summary>
        public string FormatDetail(string customer, string po, string owner, int lines, int units, decimal total)
        {
            if (Current == LayoutKind.Tablet)
                return $"{customer} · PO {po} · owner {owner} · {lines} lines / {units} units · total {total:N2}";
            return $"Customer   {customer}\nPO #       {po}\nOwner      {owner}\nLines      {lines} items · {units} units\nTotal      {total:N2}";
        }

        private void SetToolBarText(string[] texts)
        {
            for (int i = 0; i < _toolBar.Buttons.Count && i < texts.Length; i++)
                _toolBar.Buttons[i].Text = texts[i];
        }
    }
}
