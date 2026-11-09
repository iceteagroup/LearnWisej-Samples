#if DEBUG
using System.Drawing;
using AccessOps.Identity;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// DEBUG BUILD ONLY - the whole file is inside <c>#if DEBUG</c>, so a Release build or publish
    /// does not contain this type. It is not on any route: the only way in is the Debug-only
    /// button of the admin console. It shows the authentication type, the authenticated flag, a
    /// redacted subject (last four characters of the account ID), the host request principal and
    /// the effective OS identity of the process - never a raw session ID, cookie or token.
    /// </summary>
    public class IdentityDiagnosticsView : UserControl
    {
        public IdentityDiagnosticsView()
        {
            this.Name = "IdentityDiagnosticsView";
            this.BackColor = Color.White;

            var heading = new Label
            {
                Name = "lblDebugOnly",
                Text = "DEBUG BUILD ONLY",
                Dock = DockStyle.Top,
                Height = 30,
                Padding = new Padding(16, 10, 16, 0),
                Font = Theme.Px(11.5f, FontStyle.Bold),
                ForeColor = Theme.Warn
            };

            var authType = IdentityProbe.AuthenticationType();
            var isAuthenticated = IdentityProbe.IsAuthenticated();
            var rows = new[]
            {
                ("Authentication type", authType, Theme.Text),
                ("IsAuthenticated", isAuthenticated, isAuthenticated == "True" ? Theme.Ok : Theme.Deny),
                ("Subject (redacted)", IdentityProbe.Subject(), Theme.Text),
                ("Host request principal", IdentityProbe.HostPrincipal(), Theme.Text),
                ("Effective OS identity", IdentityProbe.EffectiveOsIdentity(), Theme.Warn),
            };

            var overridden = authType == ApplicationPrincipal.AuthenticationType;
            var note = new Label
            {
                Name = "lblDiagnosticsNote",
                Text = overridden
                    ? "The session override answers Application.User."
                    : "No override: the getter falls back to the host principal.",
                Dock = DockStyle.Top,
                Height = 38,
                Padding = new Padding(16, 0, 16, 0),
                Font = Theme.Px(12.5f),
                ForeColor = Theme.Body,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Dock order is the reverse of Controls.Add: add bottom-up.
            this.Controls.Add(note);
            for (int i = rows.Length - 1; i >= 0; i--)
                this.Controls.Add(Row(i, rows[i].Item1, rows[i].Item2, rows[i].Item3));
            this.Controls.Add(heading);
        }

        private static Panel Row(int index, string name, string value, Color valueColor)
        {
            var row = new Panel
            {
                Name = "pnlDiag" + index,
                Dock = DockStyle.Top,
                Height = 38,
                Padding = new Padding(16, 0, 16, 0),
                BackColor = index % 2 == 1 ? ColorTranslator.FromHtml("#fafbfd") : Color.White,
                CssStyle = "border-bottom:1px solid #f0f3f7;"
            };
            row.Controls.Add(new Label
            {
                Name = "lblDiagValue" + index,
                Text = value,
                Dock = DockStyle.Fill,
                Font = Theme.Mono(13.5f, FontStyle.Bold),
                ForeColor = valueColor,
                TextAlign = ContentAlignment.MiddleLeft
            });
            row.Controls.Add(new Label
            {
                Name = "lblDiagName" + index,
                Text = name,
                Dock = DockStyle.Left,
                Width = 230,
                Font = Theme.Px(13f, FontStyle.Bold),
                ForeColor = Theme.Muted,
                TextAlign = ContentAlignment.MiddleLeft
            });
            return row;
        }
    }
}
#endif
