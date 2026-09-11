using Wisej.Web;

namespace WisejTrainingApp.Views
{
    public partial class DeploymentView : UserControl
    {
        public DeploymentView()
        {
            InitializeComponent();
            UpdatePackageStatus();
        }

        private void chkReleaseChecks_AfterItemCheck(object sender, ItemCheckEventArgs e)
        {
            UpdatePackageStatus();
        }

        private void UpdatePackageStatus()
        {
            int done = chkReleaseChecks.CheckedItems.Count;
            int total = chkReleaseChecks.Items.Count;

            lblPackageStatus.Text = done == total
                ? "Ready for release."
                : $"Not ready — {done} / {total} release checks complete.";
        }
    }
}
