using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        string figura;
        public Form3()
        {
            InitializeComponent();
        }

        public string Izbira()
        {
            ShowDialog();//prikaže
            while (figura == null) System.Threading.Thread.Sleep(500);//čaka na vrednost
            return figura;
        }

        private void kraljica_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            figura = button.Name;//dobim podatek o imenu
            this.Close();//zapiranje form2
        }
    }
}
