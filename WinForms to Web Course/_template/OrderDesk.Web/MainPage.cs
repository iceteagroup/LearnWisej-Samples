using System;
using OrderDesk.Domain;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Template main page. Each module replaces this with its own MainPage (+ MainPage.Designer.cs
    /// so it opens in the Wisej Designer). Layout convention: page background Palette.PageBackground,
    /// white cards with BorderStyle.Solid, the TracePanel docked on the right (width ~560).
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly OrderService _service = new OrderService();
        private readonly TracePanel _trace = new TracePanel();

        public MainPage()
        {
            this.Text = "OrderDesk";
            this.BackColor = Palette.PageBackground;

            _trace.Dock = DockStyle.Right;
            _trace.Width = 560;
            this.Controls.Add(_trace);

            var label = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(30, 0, 0, 0),
                Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold),
                Text = "OrderDesk — template (" + _service.Store.Count + " orders in the store)",
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            };
            this.Controls.Add(label);

            this.Load += (s, e) => _trace.Server("Program.Main", "Application.MainPage = new MainPage()  · session " + Application.SessionId.Substring(0, 8));
        }
    }
}
