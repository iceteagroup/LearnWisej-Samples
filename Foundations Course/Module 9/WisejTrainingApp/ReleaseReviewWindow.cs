using System;
using Wisej.Web;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp
{
    public partial class ReleaseReviewWindow : Form
    {
        private readonly UserAccount currentUser = new UserAccount { Name = "Jamie Lee", Role = "Support Agent" };
        private readonly SecureConfig secureConfig = new SecureConfig();

        public ReleaseReviewWindow()
        {
            InitializeComponent();

            cboEnvironment.SelectedItem = "Production";
            cboTarget.SelectedItem = "IIS";
            cboRole.SelectedItem = currentUser.Role;

            // Show only whether the key is configured — never the key itself.
            lblLicenseKey.Text = secureConfig.IsLicenseKeyConfigured
                ? "License key: configured (read from secure config)."
                : "License key: not configured — set WISEJ_LICENSE_KEY or Web.config.";

            ApplyPermission();
            UpdatePackageStatus();
            AddLog("Release review opened.");
        }

        private void cboRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentUser.Role = cboRole.Text;
            ApplyPermission();
            AddLog("Role changed to " + currentUser.Role + ".");
        }

        private void ApplyPermission()
        {
            bool canReviewDeployment =
                currentUser.Role == "Admin" || currentUser.Role == "Team Lead";

            btnReviewPackage.Enabled = canReviewDeployment;
            lblPermission.Text = canReviewDeployment
                ? "Deployment review available."
                : "Review is restricted.";
        }

        private void cboTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboTarget.Text)
            {
                case "IIS":
                    txtNotes.Text = "Publish the app folder, review Web.config, check the hosting bundle, confirm the app pool, test the URL.";
                    break;
                case "Kestrel":
                    txtNotes.Text = "Run the published app on Kestrel, set the URL and port, put a reverse proxy in front if needed, test the URL.";
                    break;
                default:
                    txtNotes.Text = "Confirm app settings, secrets, static files, logging and the deployment slot / staging setup.";
                    break;
            }
        }

        private void chkRequired_AfterItemCheck(object sender, ItemCheckEventArgs e)
        {
            UpdatePackageStatus();
        }

        private void UpdatePackageStatus()
        {
            int done = chkRequired.CheckedItems.Count;
            int total = chkRequired.Items.Count;

            lblPackageStatus.Text = done == total
                ? $"Ready for review ({done} / {total} required checks)."
                : $"Not ready — {done} / {total} required checks complete.";
        }

        private void btnReviewPackage_Click(object sender, EventArgs e)
        {
            // Check again here, on the server — never rely only on a disabled button.
            if (currentUser.Role != "Admin" && currentUser.Role != "Team Lead")
            {
                AddLog("Review blocked: role " + currentUser.Role + " is not allowed.");
                MessageBox.Show("Review is restricted.");
                return;
            }

            if (chkRequired.CheckedItems.Count < chkRequired.Items.Count)
            {
                AddLog("Review blocked: required checks incomplete.");
                MessageBox.Show("Complete every required check first.");
                return;
            }

            txtSummary.Text =
                "Environment: " + cboEnvironment.Text + "\r\n" +
                "Target: " + cboTarget.Text + "\r\n" +
                "Version: " + txtVersion.Text + "\r\n" +
                "Reviewer: " + txtReviewer.Text + "\r\n" +
                "Status: " + lblPackageStatus.Text + "\r\n" +
                "Optional checks: " + chkOptional.CheckedItems.Count + " / " + chkOptional.Items.Count + "\r\n" +
                "Notes: " + txtNotes.Text;

            AddLog("Review package created for " + cboEnvironment.Text + " / " + cboTarget.Text + ".");
        }

        // Log what happened — never passwords, license keys or private data.
        private void AddLog(string message)
        {
            lstLog.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " - " + message);
        }
    }
}
