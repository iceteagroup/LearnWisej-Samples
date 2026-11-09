using System;
using System.Drawing;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>A small modal dialog that asks for a passkey label (Add a passkey, Rename).</summary>
    public class LabelDialog : Form
    {
        private readonly TextBox txtLabel = new TextBox();

        /// <summary>Creates the dialog with a caption, a prompt and a starting value.</summary>
        public LabelDialog(string caption, string prompt, string value)
        {
            this.Name = "LabelDialog";
            this.Text = caption;
            this.Size = new Size(420, 190);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Fixed;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Padding = new Padding(18, 14, 18, 14);

            var label = new Label { Text = prompt, Dock = DockStyle.Top, Height = 24, Font = Theme.Px(12.5f, FontStyle.Bold), ForeColor = Theme.Body };
            txtLabel.Name = "txtLabel";
            txtLabel.Text = value ?? "";
            txtLabel.MaxLength = 60;
            Theme.StyleInput(txtLabel);
            txtLabel.Height = 34;
            txtLabel.Dock = DockStyle.Top;

            var buttons = new Panel { Dock = DockStyle.Bottom, Height = 32 };
            var ok = new Button { Name = "btnOk", Text = "OK", Dock = DockStyle.Right, Width = 80 };
            Theme.StyleButton(ok, primary: true, size: 12.5f);
            ok.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };
            var cancel = new Button { Name = "btnCancel", Text = "Cancel", Dock = DockStyle.Right, Width = 80 };
            Theme.StyleButton(cancel, primary: false, size: 12.5f);
            cancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            buttons.Controls.Add(ok);
            buttons.Controls.Add(new Panel { Dock = DockStyle.Right, Width = 8 });
            buttons.Controls.Add(cancel);

            this.Controls.Add(txtLabel);
            this.Controls.Add(label);
            this.Controls.Add(buttons);
            this.AcceptButton = ok;
            this.CancelButton = cancel;
        }

        /// <summary>The label the user typed (plain text; trimmed, at most 60 characters).</summary>
        public string Value => (txtLabel.Text ?? "").Trim();
    }
}
