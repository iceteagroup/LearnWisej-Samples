using Wisej.Web;

namespace GlobalDesk
{
    /// <summary>
    /// The page that hosts the customer editor.
    ///
    /// Module 3 takes the culture switch off the screen. <c>culture: "auto"</c> in
    /// <c>Default.json</c> starts each session from the browser's <c>Accept-Language</c>, and
    /// <c>?lang=de-DE</c> in the address bar overrides it for that session - which is how a
    /// support engineer reproduces a customer's screen without changing their own browser.
    ///
    /// The application bar caption comes from the shared resource now, not from the control's
    /// designer resource as it did in Module 2, because Module 3 is where the shared German file
    /// exists.
    /// </summary>
    public partial class CustomerEditorPage : Page
    {
        public CustomerEditorPage()
        {
            InitializeComponent();

            this.lblAppTitle.Text = Texts.Get("CustomerEditor.Title");

            var editor = new CustomerEditor();
            editor.Location = new System.Drawing.Point(0, 0);
            this.pnlEditorHost.Controls.Add(editor);
        }
    }
}
