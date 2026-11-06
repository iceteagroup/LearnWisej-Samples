using System.Drawing;
using Wisej.Web;

namespace FieldOps
{
    /// <summary>"Leave WO-4821? The reading 412.8 has not been submitted yet." - Stay / Leave.</summary>
    public class LeaveDialog : Form
    {
        public LeaveDialog(string number, string reading)
        {
            Name = "LeaveDialog";
            Text = "";
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(300, 150);
            BackColor = Color.White;
            CssStyle = "border-radius:12px;box-shadow:0 14px 34px rgba(0,0,0,.35)";

            var body = new Label
            {
                Name = "lblLeave",
                AllowHtml = true,
                AutoSize = false,
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 14, 16, 0),
                Text = $"<div style=\"font-size:13px;font-weight:800;color:#0d1b2a\">Leave {System.Net.WebUtility.HtmlEncode(number)}?</div>" +
                       $"<div style=\"font-size:12px;color:#5a6b7d;margin-top:6px;line-height:1.4\">The reading {System.Net.WebUtility.HtmlEncode(reading)} has not been submitted yet.</div>",
            };

            var btnLeave = new Button { Name = "btnLeave", Text = "Leave", AutoSize = true, BackColor = Ops.Bad, ForeColor = Color.White, Margin = new Padding(8, 0, 0, 0) };
            var btnStay = new Button { Name = "btnStay", Text = "Stay", AutoSize = true, BackColor = Color.White, ForeColor = Ops.SecondaryInk };
            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 48,
                Padding = new Padding(0, 8, 16, 0),
                FlowDirection = FlowDirection.RightToLeft,
            };
            // The result is set and the dialog closed in the handlers: that is what completes
            // ShowDialogAsync() for the caller.
            btnLeave.Click += (s, e) => { DialogResult = DialogResult.Yes; Close(); };
            btnStay.Click += (s, e) => { DialogResult = DialogResult.No; Close(); };
            buttons.Controls.Add(btnLeave);
            buttons.Controls.Add(btnStay);

            Controls.Add(body);
            Controls.Add(buttons);
            CancelButton = btnStay;
        }
    }
}
