using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MEP_Fabricator
{
    public partial class StartIndex : Form
    {
        public StartIndex()
        {
            InitializeComponent();
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
                return;
            e.Handled = true;
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            // Handle key down event if needed
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
                return;
            e.Handled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EV.startIndex = int.Parse(this.textBox1.Text);
            EV.splitter = double.Parse(this.textBox2.Text) * 0.0032808399;
            this.Close();
        }

        private void StartIndex_Load(object sender, EventArgs e)
        {
            // Handle form load event if needed
        }
    }
}
