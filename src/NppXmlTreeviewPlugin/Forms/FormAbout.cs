using System;
using System.Reflection;
using System.Windows.Forms;

namespace NppXmlTreeviewPlugin.Forms
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();

            this.label2.Text = $"Version {Assembly.GetExecutingAssembly().GetName().Version}";
        }

        private void ButtonDonate_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=7YQVNRJ7WAQ8G");
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
