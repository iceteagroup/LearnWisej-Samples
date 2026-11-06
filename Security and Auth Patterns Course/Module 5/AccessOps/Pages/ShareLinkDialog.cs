using System;
using System.Drawing;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// Shows a freshly issued share link so it can be copied. The document title is a Label's Text -
    /// encoded by default - and the link is a read-only TextBox.
    /// </summary>
    internal sealed class ShareLinkDialog : Form
    {
        public ShareLinkDialog(string title, string url, DateTimeOffset expires)
        {
            this.Name = "ShareLinkDialog";
            this.Text = "Share link (1 h)";
            this.Size = new Size(620, 230);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Padding = new Padding(20, 16, 20, 16);

            var lblTitle = new Label { Name = "lblDocument", Text = title, Dock = DockStyle.Top, Height = 26, Font = Theme.Px(14f, FontStyle.Bold), ForeColor = Theme.Ink };
            var lblExpires = new Label
            {
                Name = "lblExpires",
                Text = $"Anyone with this link can open the document until {expires:HH:mm} UTC. The id and the expiry are signed - edit either and the link stops working.",
                Dock = DockStyle.Top,
                Height = 40,
                Font = Theme.Px(12.5f),
                ForeColor = Theme.Muted
            };
            var txtLink = new TextBox { Name = "txtLink", Text = url, ReadOnly = true, Dock = DockStyle.Top, Height = 32, Font = Theme.Mono(12f) };
            var btnClose = new Button { Name = "btnClose", Text = "Close", Dock = DockStyle.Right, Width = 90 };
            Theme.StyleButton(btnClose, primary: true);
            btnClose.Click += (s, e) => this.Close();
            var buttons = new Panel { Dock = DockStyle.Bottom, Height = 34 };
            buttons.Controls.Add(btnClose);

            this.Controls.Add(buttons);
            this.Controls.Add(txtLink);
            this.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 6 });
            this.Controls.Add(lblExpires);
            this.Controls.Add(lblTitle);
        }
    }
}
