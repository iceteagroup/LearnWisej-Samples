using System;
using System.Globalization;
using Wisej.Web;

namespace GlobalDesk
{
    /// <summary>
    /// The page that hosts the designer-localized <see cref="CustomerEditor"/>.
    ///
    /// It exists to make one rule visible: <c>ApplyResources</c> runs inside
    /// <c>InitializeComponent</c>, so a designer-localized control reads its resources <b>once,
    /// at construction</b>. Change the session's culture with the editor open and nothing on it
    /// moves. The only fix is a new instance, which is what <c>btnRecreateEditor</c> builds.
    ///
    /// The strip at the top is the whole demo: the culture chip changes the session's culture and
    /// does nothing else, and the button beside it rebuilds the control. Doing them in two steps
    /// is deliberate - it is the only way to see the half-translated screen that Module 4 then
    /// teaches you never to ship.
    /// </summary>
    public partial class CustomerEditorPage : Page
    {
        /// <summary>The two cultures this module ships resources for. Module 4 replaces the
        /// toggle with a real picker; two is enough to show the rule.</summary>
        private static readonly string[] Cultures = { "en", "de-DE" };

        private CustomerEditor editor;

        public CustomerEditorPage()
        {
            InitializeComponent();

            ApplyTextResources();
            CreateEditor();
            ShowStatus(Texts.Get("Status.EditorBuilt"));
        }

        /// <summary>
        /// The host page's own words, from the shared resource. Unlike the control it hosts,
        /// these are read on every call, so they follow a culture change as soon as something
        /// calls this method again.
        /// </summary>
        private void ApplyTextResources()
        {
            this.btnRecreateEditor.Text = Texts.Get("Dashboard.RecreateEditor");
            ShowCultureChip();
        }

        /// <summary>
        /// The culture chip. Blue for the neutral language, amber once the session has moved off
        /// it - the same two states the walkthrough shows.
        /// </summary>
        private void ShowCultureChip()
        {
            var name = Application.CurrentCulture.Name;
            var neutral = name == Cultures[0];

            // A culture name is an identifier, not a word: "de-DE" reads the same in every
            // language and never goes through a resource.
            this.btnCulture.Text = name;
            this.btnCulture.BackColor = neutral ? Desk.ChipBlueBack : Desk.WarnBack;
            this.btnCulture.ForeColor = neutral ? Desk.ChipBlueInk : Desk.WarnInk;
            this.btnCulture.CssStyle = neutral
                ? "border:1px solid #1565d8;border-radius:999px"
                : "border:1px solid #e8a13c;border-radius:999px";
        }

        /// <summary>
        /// Moves the session to the next culture and stops.
        ///
        /// Everything that should follow deliberately does not happen here. The page keeps its
        /// English captions and so does the editor, which is the state the walkthrough pauses on.
        /// </summary>
        private void btnCulture_Click(object sender, EventArgs e)
        {
            var current = Array.IndexOf(Cultures, Application.CurrentCulture.Name);
            var next = Cultures[(current + 1) % Cultures.Length];

            // Per session. Never a static field: one user's click would change another user's
            // screen, and it would never reproduce with one developer and one browser.
            Application.CurrentCulture = CultureInfo.GetCultureInfo(next);

            ShowCultureChip();
            ShowStatus(string.Format(Application.CurrentCulture,
                Texts.Get("Status.CultureChanged"), next, this.editor.BuiltForCulture));
        }

        /// <summary>
        /// Remove, dispose, construct, add back. Dispose matters: the old instance has a client
        /// counterpart, and leaving it behind leaks a control tree per rebuild.
        ///
        /// The real cost of this approach is in the first line of the method: whatever the user
        /// had typed is gone. A production screen saves the values around the rebuild, or does
        /// not offer the language switch while an editor is open.
        /// </summary>
        private void CreateEditor()
        {
            this.pnlEditorHost.Controls.Clear();
            this.editor?.Dispose();

            this.editor = new CustomerEditor();
            this.editor.Location = new System.Drawing.Point(0, 0);
            this.editor.Message += this.Editor_Message;
            this.pnlEditorHost.Controls.Add(this.editor);

            // The window title belongs to this screen, and this screen is the editor: it comes
            // from the control's own designer resource ($this.Text) and therefore changes at the
            // same moment its captions do. Module 3 moves it to the shared resource.
            this.lblAppTitle.Text = this.editor.Text;
        }

        private void btnRecreateEditor_Click(object sender, EventArgs e)
        {
            try
            {
                CreateEditor();
                ShowStatus(string.Format(Application.CurrentCulture,
                    Texts.Get("Status.EditorRebuilt"), this.editor.BuiltForCulture));
            }
            catch (Exception)
            {
                // One readable sentence rather than a stack trace. The page is still standing:
                // the old instance was already disposed, so the message has to say what state
                // the screen is in.
                ShowStatus(Texts.Get("Status.RebuildFailed"), bad: true);
            }
        }

        private void Editor_Message(string message) => ShowStatus(message);

        private void ShowStatus(string message, bool bad = false)
        {
            this.lblStatus.Text = message;
            this.lblStatus.BackColor = bad ? Desk.BadBack : Desk.CardHead;
            this.lblStatus.ForeColor = bad ? Desk.BadInk : Desk.Muted;
        }
    }
}
