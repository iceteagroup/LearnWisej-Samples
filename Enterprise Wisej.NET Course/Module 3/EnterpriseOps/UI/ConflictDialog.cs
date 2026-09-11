using System;
using System.Collections.Generic;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// The conflict dialog. A bare "save failed" leaves the user with lost work and no idea why, so this
    /// dialog does three things instead:
    ///
    ///  1. it <b>explains</b> — the record changed since it was opened, and nothing has been overwritten;
    ///  2. it <b>shows the evidence</b> — your edit against the current record, and the two versions;
    ///  3. it <b>offers three paths</b> — Reload (discard yours, take the current record),
    ///     Compare (both versions field by field, so the edit can be merged) and Cancel (decide later).
    ///
    /// The dialog decides nothing. The rows, the comparison and the audit entry all come from
    /// <see cref="ConflictResolutionService"/>, so a batch save or an import that hits the same conflict can
    /// reuse the same rules.
    /// </summary>
    public partial class ConflictDialog : Form
    {
        private readonly ConflictInfo _conflict;
        private readonly CommandContext _context;
        private readonly ConflictResolutionService _service;

        /// <summary>What the user chose. Cancel until they choose otherwise — closing the window loses nothing.</summary>
        public ConflictResolution Resolution { get; private set; } = ConflictResolution.Cancel;

        /// <summary>Designer constructor. The Wisej Designer needs it; the application never uses it.</summary>
        public ConflictDialog()
        {
            InitializeComponent();
        }

        public ConflictDialog(ConflictInfo conflict, CommandContext context, ConflictResolutionService service)
            : this()
        {
            _conflict = conflict;
            _context = context;
            _service = service;
        }

        #region Event handlers

        private void ConflictDialog_Load(object sender, EventArgs e)
        {
            if (_conflict == null)
                return;

            dgvVersions.DataSource = _service.Summarize(_conflict);
            lblFootnote.Text = _conflict.Footnote;
            lblExplain.Text = $"Session \"{_conflict.SavedByOther}\" saved a newer version while you were editing. " +
                              "Your edit is based on a stale copy — nothing has been overwritten.";
        }

        /// <summary>Reload: discard the local edits and show the current record. The screen does the reloading.</summary>
        private void btnReload_Click(object sender, EventArgs e)
        {
            Choose(ConflictResolution.Reload);
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Compare: reveal the field-by-field table. The dialog stays open — comparing is how the user decides
        /// between the other two paths.
        /// </summary>
        private void btnCompare_Click(object sender, EventArgs e)
        {
            if (_conflict == null)
                return;

            if (!pnlCompare.Visible)
            {
                IReadOnlyList<FieldComparison> rows = _service.Compare(_conflict);
                dgvCompare.DataSource = rows;
                Choose(ConflictResolution.Compare);
            }

            pnlCompare.Visible = !pnlCompare.Visible;
            btnCompare.Text = pnlCompare.Visible ? "Hide comparison" : "Compare changes";
        }

        /// <summary>Cancel: leave the screen exactly as it is. The edit is still on screen, still stale.</summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Choose(ConflictResolution.Cancel);
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion

        /// <summary>Records the choice with the correlation id in the audit trail.</summary>
        private void Choose(ConflictResolution choice)
        {
            Resolution = choice;

            if (_conflict != null && _context != null)
                _service.RecordChoice(_context, _conflict, choice);
        }
    }
}
