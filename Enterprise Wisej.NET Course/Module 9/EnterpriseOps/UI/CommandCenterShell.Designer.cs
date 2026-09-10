namespace EnterpriseOps.UI
{
    partial class CommandCenterShell
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblScreenName = new Wisej.Web.Label();
            this.lblTenantCaption = new Wisej.Web.Label();
            this.cboTenant = new Wisej.Web.ComboBox();
            this.lblUserCaption = new Wisej.Web.Label();
            this.cboUser = new Wisej.Web.ComboBox();
            this.lblRole = new Wisej.Web.Label();
            this.lblCorrelation = new Wisej.Web.Label();
            this.pnlPalette = new Wisej.Web.Panel();
            this.lblPaletteTitle = new Wisej.Web.Label();
            this.lblPaletteHint = new Wisej.Web.Label();
            this.commandPaletteHost = new EnterpriseOps.Interop.CommandPaletteHost();
            this.capabilityPanel = new EnterpriseOps.UI.BrowserCapabilityPanel();
            this.pnlTrace = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstTrace = new Wisej.Web.ListBox();
            this.pnlAudit = new Wisej.Web.Panel();
            this.lblAuditTitle = new Wisej.Web.Label();
            this.lblAuditHint = new Wisej.Web.Label();
            this.lstAudit = new Wisej.Web.ListBox();
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.lblTargetCaption = new Wisej.Web.Label();
            this.txtEntityId = new Wisej.Web.TextBox();
            this.btnSetTarget = new Wisej.Web.Button();
            this.btnOpenPalette = new Wisej.Web.Button();
            this.btnRunApprove = new Wisej.Web.Button();
            this.btnRunEscalate = new Wisej.Web.Button();
            this.btnRunQueue = new Wisej.Web.Button();
            this.lblPaletteState = new Wisej.Web.Label();
            this.btnUnknownCommand = new Wisej.Web.Button();
            this.btnMalformed = new Wisej.Web.Button();
            this.btnInvalidState = new Wisej.Web.Button();
            this.btnForeignTarget = new Wisej.Web.Button();
            this.btnTrustClient = new Wisej.Web.Button();
            this.btnRevert = new Wisej.Web.Button();
            this.btnForgeCapabilities = new Wisej.Web.Button();
            this.btnRecollect = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlPalette.SuspendLayout();
            this.pnlTrace.SuspendLayout();
            this.pnlAudit.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (slim header bar: screen · tenant · user · role · correlation id)
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblScreenName);
            this.pnlHeader.Controls.Add(this.lblTenantCaption);
            this.pnlHeader.Controls.Add(this.cboTenant);
            this.pnlHeader.Controls.Add(this.lblUserCaption);
            this.pnlHeader.Controls.Add(this.cboUser);
            this.pnlHeader.Controls.Add(this.lblRole);
            this.pnlHeader.Controls.Add(this.lblCorrelation);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1348, 44);
            //
            // lblScreenName
            //
            this.lblScreenName.AutoSize = false;
            this.lblScreenName.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblScreenName.ForeColor = System.Drawing.Color.White;
            this.lblScreenName.Location = new System.Drawing.Point(20, 11);
            this.lblScreenName.Name = "lblScreenName";
            this.lblScreenName.Size = new System.Drawing.Size(320, 22);
            this.lblScreenName.Text = "EnterpriseOps — Command Center";
            //
            // lblTenantCaption
            //
            this.lblTenantCaption.AutoSize = false;
            this.lblTenantCaption.Font = new System.Drawing.Font("default", 9F);
            this.lblTenantCaption.ForeColor = System.Drawing.Color.White;
            this.lblTenantCaption.Location = new System.Drawing.Point(356, 14);
            this.lblTenantCaption.Name = "lblTenantCaption";
            this.lblTenantCaption.Size = new System.Drawing.Size(48, 18);
            this.lblTenantCaption.Text = "Tenant";
            //
            // cboTenant
            //
            this.cboTenant.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboTenant.Location = new System.Drawing.Point(406, 8);
            this.cboTenant.Name = "cboTenant";
            this.cboTenant.Size = new System.Drawing.Size(158, 28);
            this.cboTenant.TabIndex = 0;
            this.cboTenant.SelectedIndexChanged += new System.EventHandler(this.cboTenant_SelectedIndexChanged);
            //
            // lblUserCaption
            //
            this.lblUserCaption.AutoSize = false;
            this.lblUserCaption.Font = new System.Drawing.Font("default", 9F);
            this.lblUserCaption.ForeColor = System.Drawing.Color.White;
            this.lblUserCaption.Location = new System.Drawing.Point(580, 14);
            this.lblUserCaption.Name = "lblUserCaption";
            this.lblUserCaption.Size = new System.Drawing.Size(36, 18);
            this.lblUserCaption.Text = "User";
            //
            // cboUser
            //
            this.cboUser.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboUser.Location = new System.Drawing.Point(618, 8);
            this.cboUser.Name = "cboUser";
            this.cboUser.Size = new System.Drawing.Size(172, 28);
            this.cboUser.TabIndex = 1;
            this.cboUser.SelectedIndexChanged += new System.EventHandler(this.cboUser_SelectedIndexChanged);
            //
            // lblRole
            //
            this.lblRole.AutoSize = false;
            this.lblRole.Font = new System.Drawing.Font("monospace", 9F);
            this.lblRole.ForeColor = System.Drawing.Color.White;
            this.lblRole.Location = new System.Drawing.Point(802, 14);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(220, 18);
            this.lblRole.Text = "role Manager (from the session)";
            //
            // lblCorrelation
            //
            this.lblCorrelation.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCorrelation.AutoSize = false;
            this.lblCorrelation.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCorrelation.ForeColor = System.Drawing.Color.White;
            this.lblCorrelation.Location = new System.Drawing.Point(1028, 14);
            this.lblCorrelation.Name = "lblCorrelation";
            this.lblCorrelation.Size = new System.Drawing.Size(300, 18);
            this.lblCorrelation.Text = "session —";
            this.lblCorrelation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlPalette  (card: the interop host)
            //
            this.pnlPalette.BackColor = System.Drawing.Color.White;
            this.pnlPalette.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlPalette.Controls.Add(this.lblPaletteTitle);
            this.pnlPalette.Controls.Add(this.lblPaletteHint);
            this.pnlPalette.Controls.Add(this.commandPaletteHost);
            this.pnlPalette.Location = new System.Drawing.Point(16, 54);
            this.pnlPalette.Name = "pnlPalette";
            this.pnlPalette.Size = new System.Drawing.Size(650, 238);
            //
            // lblPaletteTitle
            //
            this.lblPaletteTitle.AutoSize = false;
            this.lblPaletteTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblPaletteTitle.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblPaletteTitle.Location = new System.Drawing.Point(14, 8);
            this.lblPaletteTitle.Name = "lblPaletteTitle";
            this.lblPaletteTitle.Size = new System.Drawing.Size(400, 22);
            this.lblPaletteTitle.Text = "Command palette host";
            //
            // lblPaletteHint
            //
            this.lblPaletteHint.AutoSize = false;
            this.lblPaletteHint.Font = new System.Drawing.Font("monospace", 9F);
            this.lblPaletteHint.ForeColor = System.Drawing.Color.FromArgb(138, 151, 164);
            this.lblPaletteHint.Location = new System.Drawing.Point(14, 32);
            this.lblPaletteHint.Name = "lblPaletteHint";
            this.lblPaletteHint.Size = new System.Drawing.Size(620, 18);
            this.lblPaletteHint.Text = "package /Interop/palette.client.js · InitScript command-palette-host.js";
            //
            // commandPaletteHost
            //
            this.commandPaletteHost.Location = new System.Drawing.Point(14, 54);
            this.commandPaletteHost.Name = "commandPaletteHost";
            this.commandPaletteHost.Size = new System.Drawing.Size(620, 172);
            this.commandPaletteHost.TabIndex = 2;
            //
            // capabilityPanel  (card: browser capabilities — detection only)
            //
            this.capabilityPanel.Location = new System.Drawing.Point(16, 300);
            this.capabilityPanel.Name = "capabilityPanel";
            this.capabilityPanel.Size = new System.Drawing.Size(650, 228);
            this.capabilityPanel.TabIndex = 3;
            //
            // pnlTrace  (card: server · live activity trace)
            //
            this.pnlTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlTrace.BackColor = System.Drawing.Color.White;
            this.pnlTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTrace.Controls.Add(this.lblTraceTitle);
            this.pnlTrace.Controls.Add(this.lstTrace);
            this.pnlTrace.Location = new System.Drawing.Point(682, 54);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Size = new System.Drawing.Size(650, 238);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblTraceTitle.Location = new System.Drawing.Point(14, 8);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(480, 22);
            this.lblTraceTitle.Text = "Server · live activity trace";
            //
            // lstTrace
            //
            this.lstTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom
                | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.lstTrace.Location = new System.Drawing.Point(14, 34);
            this.lstTrace.Name = "lstTrace";
            this.lstTrace.Size = new System.Drawing.Size(622, 192);
            this.lstTrace.TabIndex = 4;
            //
            // pnlAudit  (card: security · audit log)
            //
            this.pnlAudit.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlAudit.BackColor = System.Drawing.Color.White;
            this.pnlAudit.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlAudit.Controls.Add(this.lblAuditTitle);
            this.pnlAudit.Controls.Add(this.lblAuditHint);
            this.pnlAudit.Controls.Add(this.lstAudit);
            this.pnlAudit.Location = new System.Drawing.Point(682, 300);
            this.pnlAudit.Name = "pnlAudit";
            this.pnlAudit.Size = new System.Drawing.Size(650, 228);
            //
            // lblAuditTitle
            //
            this.lblAuditTitle.AutoSize = false;
            this.lblAuditTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblAuditTitle.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblAuditTitle.Location = new System.Drawing.Point(14, 8);
            this.lblAuditTitle.Name = "lblAuditTitle";
            this.lblAuditTitle.Size = new System.Drawing.Size(480, 22);
            this.lblAuditTitle.Text = "Security · audit log (append-only)";
            //
            // lblAuditHint
            //
            this.lblAuditHint.AutoSize = false;
            this.lblAuditHint.Font = new System.Drawing.Font("monospace", 9F);
            this.lblAuditHint.ForeColor = System.Drawing.Color.FromArgb(138, 151, 164);
            this.lblAuditHint.Location = new System.Drawing.Point(14, 32);
            this.lblAuditHint.Name = "lblAuditHint";
            this.lblAuditHint.Size = new System.Drawing.Size(620, 18);
            this.lblAuditHint.Text = "outcome · user · tenant · correlation · command · entity";
            //
            // lstAudit
            //
            this.lstAudit.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom
                | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstAudit.Font = new System.Drawing.Font("monospace", 9F);
            this.lstAudit.Location = new System.Drawing.Point(14, 52);
            this.lstAudit.Name = "lstAudit";
            this.lstAudit.Size = new System.Drawing.Size(622, 162);
            this.lstAudit.TabIndex = 5;
            //
            // lblBanner  (failure banner — hidden until something fails)
            //
            this.lblBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 236);
            this.lblBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(124, 42, 42);
            this.lblBanner.Location = new System.Drawing.Point(16, 536);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.lblBanner.Size = new System.Drawing.Size(1316, 26);
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("monospace", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(16, 566);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1316, 20);
            this.lblStatus.Text = "Command Center — Ctrl+K opens the palette";
            //
            // pnlActions  (bottom bar: success · failure · recovery)
            //
            this.pnlActions.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActions.BackColor = System.Drawing.Color.White;
            this.pnlActions.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlActions.Controls.Add(this.lblTargetCaption);
            this.pnlActions.Controls.Add(this.txtEntityId);
            this.pnlActions.Controls.Add(this.btnSetTarget);
            this.pnlActions.Controls.Add(this.btnOpenPalette);
            this.pnlActions.Controls.Add(this.btnRunApprove);
            this.pnlActions.Controls.Add(this.btnRunEscalate);
            this.pnlActions.Controls.Add(this.btnRunQueue);
            this.pnlActions.Controls.Add(this.lblPaletteState);
            this.pnlActions.Controls.Add(this.btnUnknownCommand);
            this.pnlActions.Controls.Add(this.btnMalformed);
            this.pnlActions.Controls.Add(this.btnInvalidState);
            this.pnlActions.Controls.Add(this.btnForeignTarget);
            this.pnlActions.Controls.Add(this.btnTrustClient);
            this.pnlActions.Controls.Add(this.btnRevert);
            this.pnlActions.Controls.Add(this.btnForgeCapabilities);
            this.pnlActions.Controls.Add(this.btnRecollect);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Location = new System.Drawing.Point(16, 592);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1316, 82);
            //
            // lblTargetCaption
            //
            this.lblTargetCaption.AutoSize = false;
            this.lblTargetCaption.Font = new System.Drawing.Font("default", 9F);
            this.lblTargetCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblTargetCaption.Location = new System.Drawing.Point(12, 14);
            this.lblTargetCaption.Name = "lblTargetCaption";
            this.lblTargetCaption.Size = new System.Drawing.Size(46, 20);
            this.lblTargetCaption.Text = "Target";
            //
            // txtEntityId
            //
            this.txtEntityId.Font = new System.Drawing.Font("monospace", 9F);
            this.txtEntityId.Location = new System.Drawing.Point(58, 8);
            this.txtEntityId.MaxLength = 32;
            this.txtEntityId.Name = "txtEntityId";
            this.txtEntityId.Size = new System.Drawing.Size(96, 30);
            this.txtEntityId.TabIndex = 6;
            this.txtEntityId.Text = "WO-1040";
            //
            // btnSetTarget
            //
            this.btnSetTarget.Location = new System.Drawing.Point(160, 8);
            this.btnSetTarget.Name = "btnSetTarget";
            this.btnSetTarget.Size = new System.Drawing.Size(92, 30);
            this.btnSetTarget.TabIndex = 7;
            this.btnSetTarget.Text = "Set target";
            this.btnSetTarget.ToolTipText = "Pushes the target into the widget Options (server → client) and traces the queue row it points at.";
            this.btnSetTarget.Click += new System.EventHandler(this.btnSetTarget_Click);
            //
            // btnOpenPalette
            //
            this.btnOpenPalette.Location = new System.Drawing.Point(258, 8);
            this.btnOpenPalette.Name = "btnOpenPalette";
            this.btnOpenPalette.Size = new System.Drawing.Size(224, 30);
            this.btnOpenPalette.TabIndex = 8;
            this.btnOpenPalette.Text = "Open palette (server → client)";
            this.btnOpenPalette.ToolTipText = "Server callback: Call(\"paletteOpen\"). Deferred if the client widget does not exist yet.";
            this.btnOpenPalette.Click += new System.EventHandler(this.btnOpenPalette_Click);
            //
            // btnRunApprove
            //
            this.btnRunApprove.Location = new System.Drawing.Point(488, 8);
            this.btnRunApprove.Name = "btnRunApprove";
            this.btnRunApprove.Size = new System.Drawing.Size(212, 30);
            this.btnRunApprove.TabIndex = 9;
            this.btnRunApprove.Text = "Run approve (palette → server)";
            this.btnRunApprove.ToolTipText = "The script sends workorder.approve for the target through RunClientCommand.";
            this.btnRunApprove.Click += new System.EventHandler(this.btnRunApprove_Click);
            //
            // btnRunEscalate
            //
            this.btnRunEscalate.Location = new System.Drawing.Point(706, 8);
            this.btnRunEscalate.Name = "btnRunEscalate";
            this.btnRunEscalate.Size = new System.Drawing.Size(166, 30);
            this.btnRunEscalate.TabIndex = 10;
            this.btnRunEscalate.Text = "Run escalate";
            this.btnRunEscalate.ToolTipText = "The one state change a Technician is allowed to make — same wire, different permission.";
            this.btnRunEscalate.Click += new System.EventHandler(this.btnRunEscalate_Click);
            //
            // btnRunQueue
            //
            this.btnRunQueue.Location = new System.Drawing.Point(878, 8);
            this.btnRunQueue.Name = "btnRunQueue";
            this.btnRunQueue.Size = new System.Drawing.Size(186, 30);
            this.btnRunQueue.TabIndex = 11;
            this.btnRunQueue.Text = "Run open work queue";
            this.btnRunQueue.ToolTipText = "A command with no entity: sending one anyway is MALFORMED_PAYLOAD.";
            this.btnRunQueue.Click += new System.EventHandler(this.btnRunQueue_Click);
            //
            // lblPaletteState
            //
            this.lblPaletteState.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblPaletteState.AutoSize = false;
            this.lblPaletteState.Font = new System.Drawing.Font("monospace", 9F);
            this.lblPaletteState.ForeColor = System.Drawing.Color.FromArgb(106, 125, 146);
            this.lblPaletteState.Location = new System.Drawing.Point(1070, 14);
            this.lblPaletteState.Name = "lblPaletteState";
            this.lblPaletteState.Size = new System.Drawing.Size(234, 20);
            this.lblPaletteState.Text = "host: not created yet";
            this.lblPaletteState.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnUnknownCommand
            //
            this.btnUnknownCommand.Location = new System.Drawing.Point(12, 44);
            this.btnUnknownCommand.Name = "btnUnknownCommand";
            this.btnUnknownCommand.Size = new System.Drawing.Size(140, 30);
            this.btnUnknownCommand.TabIndex = 12;
            this.btnUnknownCommand.Text = "Unknown command";
            this.btnUnknownCommand.ToolTipText = "The script sends workorder.delete — not in the catalogue → UNKNOWN_COMMAND.";
            this.btnUnknownCommand.Click += new System.EventHandler(this.btnUnknownCommand_Click);
            //
            // btnMalformed
            //
            this.btnMalformed.Location = new System.Drawing.Point(158, 44);
            this.btnMalformed.Name = "btnMalformed";
            this.btnMalformed.Size = new System.Drawing.Size(140, 30);
            this.btnMalformed.TabIndex = 13;
            this.btnMalformed.Text = "Malformed payload";
            this.btnMalformed.ToolTipText = "An oversized entity id and a correlation id that is not 8 hex characters → MALFORMED_PAYLOAD.";
            this.btnMalformed.Click += new System.EventHandler(this.btnMalformed_Click);
            //
            // btnInvalidState
            //
            this.btnInvalidState.Location = new System.Drawing.Point(304, 44);
            this.btnInvalidState.Name = "btnInvalidState";
            this.btnInvalidState.Size = new System.Drawing.Size(140, 30);
            this.btnInvalidState.TabIndex = 14;
            this.btnInvalidState.Text = "Closed work order";
            this.btnInvalidState.ToolTipText = "Approve WO-1041 (Completed): permitted, resolvable, still rejected → INVALID_STATE.";
            this.btnInvalidState.Click += new System.EventHandler(this.btnInvalidState_Click);
            //
            // btnForeignTarget
            //
            this.btnForeignTarget.Location = new System.Drawing.Point(450, 44);
            this.btnForeignTarget.Name = "btnForeignTarget";
            this.btnForeignTarget.Size = new System.Drawing.Size(140, 30);
            this.btnForeignTarget.TabIndex = 15;
            this.btnForeignTarget.Text = "Other tenant's WO";
            this.btnForeignTarget.ToolTipText = "A well-formed id that belongs to another tenant → INVALID_TARGET (same answer as \"does not exist\").";
            this.btnForeignTarget.Click += new System.EventHandler(this.btnForeignTarget_Click);
            //
            // btnTrustClient
            //
            this.btnTrustClient.Location = new System.Drawing.Point(596, 44);
            this.btnTrustClient.Name = "btnTrustClient";
            this.btnTrustClient.Size = new System.Drawing.Size(140, 30);
            this.btnTrustClient.TabIndex = 16;
            this.btnTrustClient.Text = "⚠ Trust the client";
            this.btnTrustClient.ToolTipText = "The anti-pattern: a payload carrying role and new status, believed by the server.";
            this.btnTrustClient.Click += new System.EventHandler(this.btnTrustClient_Click);
            //
            // btnRevert
            //
            this.btnRevert.Location = new System.Drawing.Point(742, 44);
            this.btnRevert.Name = "btnRevert";
            this.btnRevert.Size = new System.Drawing.Size(140, 30);
            this.btnRevert.TabIndex = 17;
            this.btnRevert.Text = "Revert tampered";
            this.btnRevert.ToolTipText = "Recovery: restore the work order from the audit log's before-snapshot.";
            this.btnRevert.Click += new System.EventHandler(this.btnRevert_Click);
            //
            // btnForgeCapabilities
            //
            this.btnForgeCapabilities.Location = new System.Drawing.Point(888, 44);
            this.btnForgeCapabilities.Name = "btnForgeCapabilities";
            this.btnForgeCapabilities.Size = new System.Drawing.Size(140, 30);
            this.btnForgeCapabilities.TabIndex = 18;
            this.btnForgeCapabilities.Text = "Forged capabilities";
            this.btnForgeCapabilities.ToolTipText = "The report gains canApprove=1 and role=Admin — both dropped as unknown keys.";
            this.btnForgeCapabilities.Click += new System.EventHandler(this.btnForgeCapabilities_Click);
            //
            // btnRecollect
            //
            this.btnRecollect.Location = new System.Drawing.Point(1034, 44);
            this.btnRecollect.Name = "btnRecollect";
            this.btnRecollect.Size = new System.Drawing.Size(140, 30);
            this.btnRecollect.TabIndex = 19;
            this.btnRecollect.Text = "Re-detect browser";
            this.btnRecollect.ToolTipText = "Server callback: Call(\"paletteCollect\") → feature detection runs again in the browser.";
            this.btnRecollect.Click += new System.EventHandler(this.btnRecollect_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Location = new System.Drawing.Point(1180, 44);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(124, 30);
            this.btnClearTrace.TabIndex = 20;
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // CommandCenterShell
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlPalette);
            this.Controls.Add(this.capabilityPanel);
            this.Controls.Add(this.pnlTrace);
            this.Controls.Add(this.pnlAudit);
            this.Controls.Add(this.lblBanner);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlActions);
            this.Name = "CommandCenterShell";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "EnterpriseOps — Command Center";
            this.Load += new System.EventHandler(this.CommandCenterShell_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlPalette.ResumeLayout(false);
            this.pnlTrace.ResumeLayout(false);
            this.pnlAudit.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblScreenName;
        private Wisej.Web.Label lblTenantCaption;
        private Wisej.Web.ComboBox cboTenant;
        private Wisej.Web.Label lblUserCaption;
        private Wisej.Web.ComboBox cboUser;
        private Wisej.Web.Label lblRole;
        private Wisej.Web.Label lblCorrelation;
        private Wisej.Web.Panel pnlPalette;
        private Wisej.Web.Label lblPaletteTitle;
        private Wisej.Web.Label lblPaletteHint;
        private EnterpriseOps.Interop.CommandPaletteHost commandPaletteHost;
        private EnterpriseOps.UI.BrowserCapabilityPanel capabilityPanel;
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Panel pnlAudit;
        private Wisej.Web.Label lblAuditTitle;
        private Wisej.Web.Label lblAuditHint;
        private Wisej.Web.ListBox lstAudit;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Label lblTargetCaption;
        private Wisej.Web.TextBox txtEntityId;
        private Wisej.Web.Button btnSetTarget;
        private Wisej.Web.Button btnOpenPalette;
        private Wisej.Web.Button btnRunApprove;
        private Wisej.Web.Button btnRunEscalate;
        private Wisej.Web.Button btnRunQueue;
        private Wisej.Web.Label lblPaletteState;
        private Wisej.Web.Button btnUnknownCommand;
        private Wisej.Web.Button btnMalformed;
        private Wisej.Web.Button btnInvalidState;
        private Wisej.Web.Button btnForeignTarget;
        private Wisej.Web.Button btnTrustClient;
        private Wisej.Web.Button btnRevert;
        private Wisej.Web.Button btnForgeCapabilities;
        private Wisej.Web.Button btnRecollect;
        private Wisej.Web.Button btnClearTrace;
    }
}
