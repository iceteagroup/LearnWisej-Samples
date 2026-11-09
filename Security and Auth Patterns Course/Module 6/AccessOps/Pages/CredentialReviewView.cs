using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AccessOps.Identity;
using AccessOps.Security;
using AccessOps.UI;
using Wisej.Ext.WebAuthn;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// "AccessOps · My passkeys": the signed-in user's own passkeys - Label · Added · Last used ·
    /// Backup eligible - with <b>Add a passkey</b> (<c>btnEnrollPasskey</c>, shown only while the
    /// current grant passes and the sign-in is recent), <b>Rename</b> and <b>Delete</b>. The owner
    /// always comes from the session principal (<c>Application.User</c>), never from a grid cell: a
    /// row ID of another account deletes nothing.
    /// </summary>
    public class CredentialReviewView : Page
    {
        private readonly PasskeyService _passkeys = AppHost.Get<PasskeyService>();
        private readonly PasskeyCredentialStore _store = AppHost.Get<PasskeyCredentialStore>();

        private readonly Button btnEnrollPasskey = new Button();
        private readonly Button btnRename = new Button();
        private readonly Button btnDelete = new Button();
        private readonly Button btnRecords = new Button();
        private readonly Label lblHint = new Label();
        private readonly DataGridView grdPasskeys = new DataGridView();
        private readonly Banner _banner = new Banner();
        private bool _platformAuthenticator;

        /// <summary>Creates the review page for the session's principal.</summary>
        public CredentialReviewView()
        {
            this.Name = "CredentialReviewView";
            this.Text = "AccessOps";
            this.BackColor = Color.White;

            var principal = Application.User as ClaimsPrincipal;
            var name = principal?.FindFirst(ClaimTypes.Name)?.Value ?? "";

            var appBar = new AppBar { Screen = "My passkeys" };

            var toolbar = new Panel
            {
                Name = "pnlToolbar",
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(16, 10, 16, 10),
                BackColor = ColorTranslator.FromHtml("#f8fafc"),
                CssStyle = "border-bottom:1px solid #e4eaf1;"
            };
            btnEnrollPasskey.Name = "btnEnrollPasskey";
            btnEnrollPasskey.Text = "Add a passkey";
            Theme.StyleButton(btnEnrollPasskey, primary: true, size: 12.5f);
            btnEnrollPasskey.Dock = DockStyle.Left;
            btnEnrollPasskey.Width = 118;
            btnEnrollPasskey.Click += btnEnrollPasskey_Click;
            btnRename.Name = "btnRename";
            btnRename.Text = "Rename";
            Theme.StyleButton(btnRename, primary: false, size: 12.5f);
            btnRename.Dock = DockStyle.Left;
            btnRename.Width = 80;
            btnRename.Click += btnRename_Click;
            btnDelete.Name = "btnDelete";
            btnDelete.Text = "Delete";
            Theme.StyleButton(btnDelete, primary: false, size: 12.5f);
            btnDelete.ForeColor = ColorTranslator.FromHtml("#c0392b");
            btnDelete.CssStyle = "border-radius:7px; border:1px solid #f0b4ad;";
            btnDelete.Dock = DockStyle.Left;
            btnDelete.Width = 76;
            btnDelete.Click += btnDelete_Click;
            btnRecords.Name = "btnRecords";
            btnRecords.Text = "Records";
            Theme.StyleButton(btnRecords, primary: false, size: 12.5f);
            btnRecords.Dock = DockStyle.Left;
            btnRecords.Width = 80;
            btnRecords.Click += (s, e) => Program.Show(new ConsolePage());

            var signedInAs = new Label
            {
                Name = "lblSignedInAs",
                Text = "Signed in as " + name,
                Dock = DockStyle.Right,
                Width = 220,
                Font = Theme.Px(12f),
                ForeColor = Theme.Muted,
                TextAlign = ContentAlignment.MiddleRight
            };
            lblHint.Name = "lblHint";
            lblHint.Dock = DockStyle.Fill;
            lblHint.Font = Theme.Px(12f);
            lblHint.ForeColor = ColorTranslator.FromHtml("#7d8ea0");
            lblHint.TextAlign = ContentAlignment.MiddleRight;

            toolbar.Controls.Add(lblHint);
            toolbar.Controls.Add(signedInAs);
            toolbar.Controls.Add(btnRecords);
            toolbar.Controls.Add(Gap(10));
            toolbar.Controls.Add(btnDelete);
            toolbar.Controls.Add(Gap(10));
            toolbar.Controls.Add(btnRename);
            toolbar.Controls.Add(Gap(12));
            toolbar.Controls.Add(btnEnrollPasskey);

            grdPasskeys.Name = "grdPasskeys";
            grdPasskeys.Dock = DockStyle.Top;
            grdPasskeys.Height = 30 + 5 * 36 + 2;
            DataGridStyle.Apply(grdPasskeys, rowHeight: 36);
            grdPasskeys.ColumnHeadersHeight = 30;
            var label = DataGridStyle.Column("colLabel", "Label", nameof(Row.Label), 250);
            label.DefaultCellStyle.Font = Theme.Px(13f, FontStyle.Bold);
            grdPasskeys.Columns.Add(label);
            grdPasskeys.Columns.Add(DataGridStyle.MonoColumn("colAdded", "Added", nameof(Row.Added), 130, ColorTranslator.FromHtml("#3a4d63")));
            grdPasskeys.Columns.Add(DataGridStyle.MonoColumn("colLastUsed", "Last used", nameof(Row.LastUsed), 150, ColorTranslator.FromHtml("#3a4d63")));
            var backup = DataGridStyle.Column("colBackup", "Backup eligible", nameof(Row.BackupPill), 150);
            backup.AllowHtml = true;   // fixed Yes / No markup, never input
            grdPasskeys.Columns.Add(backup);
            grdPasskeys.Columns.Add(DataGridStyle.Column("colNote", "", nameof(Row.Note), 100, fill: true));

            var body = new Panel { Name = "pnlBody", Dock = DockStyle.Fill, Padding = new Padding(0, 0, 0, 0) };
            var bannerHost = new Panel { Dock = DockStyle.Top, Height = 62, Padding = new Padding(16, 8, 16, 8) };
            _banner.Dock = DockStyle.Fill;
            bannerHost.Controls.Add(_banner);
            body.Controls.Add(bannerHost);
            body.Controls.Add(grdPasskeys);

            this.Controls.Add(body);
            this.Controls.Add(toolbar);
            this.Controls.Add(appBar);

            LoadRows();
            _ = ShowHintAsync();
        }

        private static Panel Gap(int width) => new Panel { Dock = DockStyle.Left, Width = width };

        private ClaimsPrincipal Owner => Application.User as ClaimsPrincipal;

        private void LoadRows(string selectRowId = null)
        {
            var owner = Subject.From(this.Owner);
            var rows = owner.IsAnonymous ? new Row[0] : _store.ForAccount(owner.AccountId).Select(c => new Row(c)).ToArray();
            grdPasskeys.DataSource = rows;
            btnEnrollPasskey.Visible = _passkeys.MayEnroll(this.Owner);
            if (selectRowId != null)
            {
                foreach (DataGridViewRow r in grdPasskeys.Rows)
                    if (r.DataBoundItem is Row row && row.RowId == selectRowId)
                        grdPasskeys.CurrentCell = r.Cells[0];
            }
        }

        // The hint comes from the browser: is there a user-verifying platform authenticator?
        private async Task ShowHintAsync()
        {
            try
            {
                _platformAuthenticator = await WebAuthn.IsUserVerifyingPlatformAuthenticatorAvailableAsync();
                lblHint.Text = _platformAuthenticator
                    ? "This device offers a built-in authenticator (Windows Hello, Touch ID, Face ID)."
                    : "No built-in authenticator found · a security key can still work.";
            }
            catch (Exception)
            {
                lblHint.Text = "Passkeys need a secure context and a supporting browser.";
            }
            if (!btnEnrollPasskey.Visible)
                lblHint.Text = "Sign in again to add a passkey (a recent sign-in is required).";
            Application.Update(this);
        }

        private async void btnEnrollPasskey_Click(object sender, EventArgs e)
        {
            _banner.Clear();
            var dialog = new LabelDialog("Add a passkey", "Name this passkey", _platformAuthenticator ? "Work laptop · Windows Hello" : "Security key · USB");
            if (await dialog.ShowDialogAsync() != DialogResult.OK)
                return;
            var label = dialog.Value;

            // only after the current grant passes and the sign-in is recent
            var enrollment = _passkeys.BeginEnrollment(this.Owner);
            if (enrollment == null)
            {
                _banner.Show(PasskeyResult.EnrollRefused, ok: false);
                LoadRows();
                Application.Update(this);
                return;
            }

            PasskeyResult result;
            try
            {
                var response = await WebAuthn.CreateAsync(
                    enrollment.Challenge.Base64,
                    enrollment.RelyingParty,
                    enrollment.User,
                    new[] { new PublicKeyCredentialParameters(COSEAlgorithmIdentifier.ES256), new PublicKeyCredentialParameters(COSEAlgorithmIdentifier.RS256) },
                    new AuthenticatorSelectionCriteria(AuthenticatorAttachment.Platform, "preferred", false, ResidentKeyRequirement.Preferred),
                    60000,
                    AttestationConveyancePreference.None);
                result = _passkeys.CompleteEnrollment(this.Owner, enrollment.Challenge.Id, response, label);
            }
            catch (Exception)
            {
                result = _passkeys.BrowserFailed(enrollment.Challenge, PasskeyService.CreateType);
            }

            if (result.Succeeded)
            {
                _banner.Show($"Passkey \"{result.Credential.Label}\" added. It can sign in to AccessOps on this site.", ok: true);
                LoadRows(result.Credential.RowId);
            }
            else
            {
                _banner.Show(PasskeyResult.EnrollRefused, ok: false);
                AlertBox.Show(PasskeyResult.EnrollRefused, MessageBoxIcon.Warning, allowHtml: false);
            }
            Application.Update(this);
        }

        private async void btnRename_Click(object sender, EventArgs e)
        {
            if (grdPasskeys.CurrentRow?.DataBoundItem is not Row row)
                return;
            var dialog = new LabelDialog("Rename passkey", "New name", row.Label);
            if (await dialog.ShowDialogAsync() != DialogResult.OK)
                return;

            if (_passkeys.Rename(this.Owner, row.RowId, dialog.Value))
                _banner.Show($"Passkey renamed to \"{dialog.Value}\".", ok: true);
            else
                _banner.Show("The passkey was not renamed.", ok: false);
            LoadRows(row.RowId);
            Application.Update(this);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (grdPasskeys.CurrentRow?.DataBoundItem is not Row row)
                return;

            // the label is user input: the confirmation renders it as text
            MessageBox.Show($"Delete the passkey \"{row.Label}\"? It will no longer sign in.", "Delete passkey",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, allowHtml: false, onclose: answer =>
                {
                    if (answer != DialogResult.Yes)
                        return;
                    if (_passkeys.Delete(this.Owner, row.RowId))
                        _banner.Show($"Passkey \"{row.Label}\" deleted. It can no longer sign in.", ok: true);
                    else
                        _banner.Show("The passkey was not deleted.", ok: false);
                    LoadRows();
                });
        }

        /// <summary>A grid row: display values plus the row identity (no column: never sent to the browser).</summary>
        private sealed class Row
        {
            public Row(PasskeyCredential c)
            {
                this.RowId = c.RowId;
                this.Label = c.Label;
                this.Added = c.CreatedAt.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                this.LastUsed = c.LastUsedAt?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) ?? "never";
                this.BackupEligible = c.BackupEligible;
            }

            public string RowId { get; }
            public string Label { get; }
            public string Added { get; }
            public string LastUsed { get; }
            public bool BackupEligible { get; }
            public string Note => "";

            public string BackupPill => this.BackupEligible
                ? "<span style=\"font-size:11px;font-weight:800;color:#1565d8;background:#1565d81a;border:1px solid #1565d855;border-radius:999px;padding:2px 9px\">Yes</span>"
                : "<span style=\"font-size:11px;font-weight:800;color:#5a6b7d;background:#5a6b7d14;border:1px solid #5a6b7d44;border-radius:999px;padding:2px 9px\">No</span>";
        }
    }
}
