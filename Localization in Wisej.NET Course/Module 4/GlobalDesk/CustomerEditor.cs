using System;
using System.Drawing;
using Wisej.Web;

namespace GlobalDesk
{
    /// <summary>
    /// The designer-localized customer editor, now a panel on the dashboard.
    ///
    /// Its captions are applied by <c>ApplyResources</c> inside <c>InitializeComponent</c>, which
    /// means <b>at construction</b>. The dashboard's <c>CultureChanged</c> handler therefore does
    /// not try to refresh it; it throws it away and builds a new one, in the same step as the
    /// captions and the formatted values, so the screen is never half translated.
    /// </summary>
    public partial class CustomerEditor : UserControl
    {
        public CustomerEditor()
        {
            InitializeComponent();

            // Customer data, not captions: the same two values in every language.
            this.txtCompany.Text = "Northwind Traders";
            this.txtCode.Text = "NWT-0041";
        }

        /// <summary>The culture this instance was constructed under. The dashboard compares it
        /// with the session's culture, which is how the badge can tell the truth.</summary>
        public string BuiltForCulture { get; } = Application.CurrentCulture.Name;

        /// <summary>Raised with a finished, already-localized sentence for the status strip.</summary>
        public event Action<string> Message;

        /// <summary>
        /// The badge in the panel header. Green when this instance was built for the session's
        /// current culture after a switch, red when it was not - which is the state Module 4
        /// exists to make impossible.
        /// </summary>
        public void ShowBadge(string text, bool good)
        {
            this.lblBadge.Visible = !string.IsNullOrEmpty(text);
            this.lblBadge.Text = text ?? string.Empty;
            this.lblBadge.BackColor = good ? Desk.Teal : Color.FromArgb(0xD6, 0x45, 0x45);
            this.lblBadge.CssStyle = "border-radius:999px";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var result = CustomerSaveService.Save(this.txtCompany.Text, null);
            this.Message?.Invoke(Texts.Get(CustomerSaveService.ResourceKeyFor(result)));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.txtCompany.Text = "Northwind Traders";
            this.txtCode.Text = "NWT-0041";
        }
    }
}
