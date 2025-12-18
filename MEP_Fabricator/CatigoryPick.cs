using System;
using System.Windows.Forms;

namespace MEP_Fabricator
{
    public partial class CatigoryPick : Form
    {
        public CatigoryPick()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EV.fabElem = 0;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EV.fabElem = 1;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            EV.fabElem = 2;
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            EV.fabElem = 3;
            this.Close();
        }

        private void CatigoryPick_Load(object sender, EventArgs e)
        {
        }

   
    }
}
