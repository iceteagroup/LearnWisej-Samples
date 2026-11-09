using System.Drawing;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>Departments, Employees and Approvals: areas this lab does not build.</summary>
    public class EmptyAreaView : UserControl
    {
        public EmptyAreaView()
        {
            this.Name = "EmptyAreaView";
            this.BackColor = Color.White;
            var status = new StatusLine();
            status.Show("Ready", StatusTone.Idle);
            this.Controls.Add(status);
        }
    }
}
