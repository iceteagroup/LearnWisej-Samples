using System;
using Wisej.Web;

namespace WisejPerfLab.Forms
{
    /// <summary>
    /// Shows the trace note for the last measured scenario: the file name to save the trace under, and
    /// the header block that says what the trace actually is.
    /// </summary>
    /// <remarks>
    /// A <c>.diagsession</c> called <c>Report20260914.diagsession</c> is worthless a fortnight later.
    /// The name and the note together answer the questions the next reader has: which module, which
    /// scenario, which build, which dataset, which tool, and what the budget was at the time.
    /// </remarks>
    public partial class TraceNoteForm : Form
    {
        public TraceNoteForm(string note)
        {
            InitializeComponent();
            txtNote.Text = note;
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
