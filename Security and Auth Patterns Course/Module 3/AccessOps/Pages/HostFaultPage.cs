using System.Drawing;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// Shown by <c>Program.Main</c> when the request reached Wisej.NET without a Windows identity:
    /// the host is misconfigured (Anonymous left on, Windows Authentication missing, a proxy that
    /// drops the handshake). This is a host-layer fault, not an application deny, so the page says
    /// nothing about documents or grants and offers nothing to click.
    /// </summary>
    public class HostFaultPage : Page
    {
        public HostFaultPage()
        {
            this.Name = "HostFaultPage";
            this.Text = "AccessOps";
            this.BackColor = Color.White;

            var appBar = new AppBar { Screen = "Sign-in problem" };

            var card = new Panel
            {
                Name = "pnlHostFault",
                Location = new Point(40, 36),
                Size = new Size(560, 150),
                Padding = new Padding(20, 16, 20, 16),
                CssStyle = "border:1px solid #f0c2bd; border-left:4px solid #b42318; border-radius:8px;"
            };
            var title = new Label
            {
                Name = "lblHostFaultTitle",
                Text = "Windows sign-in did not reach AccessOps",
                Dock = DockStyle.Top,
                Height = 30,
                Font = Theme.Px(15f, FontStyle.Bold),
                ForeColor = Theme.Deny
            };
            var body = new Label
            {
                Name = "lblHostFaultBody",
                Text = "The intranet route needs Windows sign-in, and this request arrived without it. "
                     + "This is a server configuration problem, not a decision about your access. "
                     + "Please contact the service desk.",
                Dock = DockStyle.Fill,
                Font = Theme.Px(13f),
                ForeColor = Theme.Text
            };
            card.Controls.Add(body);
            card.Controls.Add(title);

            var host = new Panel { Name = "pnlScreen", Dock = DockStyle.Fill };
            host.Controls.Add(card);

            this.Controls.Add(host);
            this.Controls.Add(appBar);
        }
    }
}
