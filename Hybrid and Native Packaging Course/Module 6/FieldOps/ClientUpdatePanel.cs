using System;
using System.Drawing;
using System.Net;
using FieldOps.Services;
using Wisej.Hybrid;
using Wisej.Web;

namespace FieldOps
{
    /// <summary>
    /// What an outdated Hybrid client gets INSTEAD of any work-order data: the installed and the
    /// required version, and one button that opens the right store page on the device.
    /// </summary>
    public class ClientUpdatePanel : UserControl
    {
        private readonly Label lblBody = new Label();
        private readonly Button btnOpenStore = new Button();
        private readonly Label lblHow = new Label();
        private string _storeUrl;

        public ClientUpdatePanel()
        {
            Name = "ClientUpdatePanel";
            BackColor = Color.White;

            lblHow.Name = "lblHow";
            lblHow.AutoSize = false;
            lblHow.Dock = DockStyle.Top;
            lblHow.Height = 24;
            lblHow.TextAlign = ContentAlignment.MiddleCenter;
            lblHow.Font = Ops.Mono(11);
            lblHow.ForeColor = Color.FromArgb(0x7D, 0x8E, 0xA0);
            lblHow.Text = "Device.Launcher.TryOpenAsync(storePage)";

            btnOpenStore.Name = "btnOpenStore";
            btnOpenStore.Dock = DockStyle.Top;
            btnOpenStore.Height = 40;
            btnOpenStore.BackColor = Ops.Accent;
            btnOpenStore.ForeColor = Color.White;
            btnOpenStore.Font = Ops.Px(13, FontStyle.Bold);
            btnOpenStore.Click += btnOpenStore_Click;

            lblBody.Name = "lblBody";
            lblBody.AllowHtml = true;
            lblBody.AutoSize = false;
            lblBody.Dock = DockStyle.Top;
            lblBody.Height = 250;

            var column = new Panel { Name = "pnlColumn", Width = 420, Dock = DockStyle.None, Anchor = AnchorStyles.Top, Padding = new Padding(0, 24, 0, 0) };
            column.Controls.Add(lblHow);
            column.Controls.Add(btnOpenStore);
            column.Controls.Add(lblBody);
            column.Height = 340;
            Controls.Add(column);
            Resize += (s, e) => column.Left = Math.Max(0, (Width - column.Width) / 2);
        }

        public void ShowFor(string installed, string required, ClientReleasePolicy.PlatformRule rule)
        {
            _storeUrl = rule.StoreUrl;
            btnOpenStore.Text = "Open " + rule.StoreName;
            lblBody.Text =
                "<div style=\"text-align:center\">" +
                "<div style=\"width:62px;height:62px;margin:0 auto;border-radius:999px;background:#fdeeea;border:2px solid #e0563b;display:flex;align-items:center;justify-content:center;color:#e0563b;font-size:30px;font-weight:800\">↑</div>" +
                "<div style=\"font-size:20px;font-weight:800;color:#1f2d3a;margin-top:16px\">This FieldOps app must be updated</div>" +
                "<div style=\"font-size:13px;color:#5a6b7d;margin-top:10px;line-height:1.5\">The server no longer supports this client build, so no work orders were loaded.</div>" +
                "<div style=\"display:flex;gap:10px;justify-content:center;margin-top:16px\">" +
                Chip("Installed", installed, "#e0563b") + Chip("Required", required, "#1f9d6b") +
                "</div></div>";
            Visible = true;
        }

        private static string Chip(string caption, string version, string color) =>
            $"<div style=\"padding:8px 16px;border-radius:8px;border:1px solid {color}55;background:{color}12\">" +
            $"<div style=\"font-size:9.5px;font-weight:800;color:#7d8ea0;text-transform:uppercase;letter-spacing:.05em\">{caption}</div>" +
            $"<div style=\"font-family:Consolas,monospace;font-size:15px;font-weight:800;color:{color};margin-top:2px\">{WebUtility.HtmlEncode(version)}</div></div>";

        private async void btnOpenStore_Click(object sender, EventArgs e)
        {
            try
            {
                bool opened = Device.Valid && await Device.Launcher.TryOpenAsync(_storeUrl);
                OpsLog.Info($"update: Device.Launcher.TryOpenAsync({_storeUrl}) -> {opened}");
                UsageTelemetry.Record("update.store", opened ? "opened" : "not-opened");
                if (!opened) lblHow.Text = "The store could not be opened here — ask your IT team for FieldOps " + _storeUrl;
            }
            catch (Exception ex)
            {
                OpsLog.Error("opening the store page", ex);
                lblHow.Text = "The store could not be opened. Ask your IT team for the FieldOps update.";
            }
            Application.Update(this);
        }
    }
}
