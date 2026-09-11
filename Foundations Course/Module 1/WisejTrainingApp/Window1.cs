using System;
using Wisej.Web;

namespace WisejTrainingApp
{
    public partial class Window1 : Form
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void btnSayHello_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                lblStatus.Text = "Please enter a name.";
                return;
            }

            lblStatus.Text = $"Hello, {name}! Welcome to Wisej.NET.";
        }
    }
}
