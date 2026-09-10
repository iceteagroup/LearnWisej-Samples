using System;
using EnterpriseOps.Hybrid;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// The sync conflict screen (deliverable 4).
    ///
    /// A conflict is a workflow, not a crash and not a silent overwrite. The panel does exactly three
    /// things: show the technician's local change, show the server's version, and offer the two resolutions
    /// the product allows. It decides nothing — it raises an event and the page calls
    /// <see cref="SyncWorkflow"/>, so the rules stay reviewable without opening the designer.
    /// </summary>
    public partial class SyncConflictPanel : UserControl
    {
        public SyncConflictPanel()
        {
            InitializeComponent();
        }

        /// <summary>"Keep server — attach my notes": the safe resolution, available to every technician.</summary>
        public event EventHandler KeepServerRequested;

        /// <summary>"Apply my completion…": needs workorder.override on the server; the attempt is audited either way.</summary>
        public event EventHandler ApplyMineRequested;

        /// <summary>The conflict currently on screen, or null.</summary>
        public SyncConflict Conflict { get; private set; }

        /// <summary>Fills both versions in and shows the panel.</summary>
        public void ShowConflict(SyncConflict conflict)
        {
            this.Conflict = conflict;

            this.lblConflictTitle.Text = "Sync conflict — " + conflict.Code;
            this.lblConflictSubtitle.Text = "The server changed this work order while you were offline. Nothing has been applied.";

            this.lblLocalHeader.Text = conflict.LocalHeader.ToUpperInvariant();
            this.lblLocalBody.Text = conflict.LocalBody;
            this.lblLocalBody.ToolTipText = conflict.LocalBody;

            this.lblServerHeader.Text = conflict.ServerHeader.ToUpperInvariant();
            this.lblServerBody.Text = conflict.ServerBody;
            this.lblServerBody.ToolTipText = conflict.ServerBody;

            this.btnKeepServer.Enabled = true;
            this.btnApplyMine.Enabled = true;
            this.Visible = true;
        }

        /// <summary>Hides the panel once the conflict has an outcome.</summary>
        public void Clear()
        {
            this.Conflict = null;
            this.Visible = false;
        }

        /// <summary>Both buttons stay visible while a resolution is running, so the user sees what is happening.</summary>
        public void SetBusy(bool busy)
        {
            this.btnKeepServer.Enabled = !busy;
            this.btnApplyMine.Enabled = !busy;
        }

        private void btnKeepServer_Click(object sender, EventArgs e)
            => KeepServerRequested?.Invoke(this, EventArgs.Empty);

        private void btnApplyMine_Click(object sender, EventArgs e)
            => ApplyMineRequested?.Invoke(this, EventArgs.Empty);
    }
}
