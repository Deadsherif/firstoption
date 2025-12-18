using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParameterWizard.MVVM.View
{
    public partial class MainView : Form
    {

        public string[] stringSeparators = new string[1]
            {
             "\r\n"
             };
        public MainView(bool shared, string sh, bool instance, string ins, bool type, string ty)
        {
            InitializeComponent();
            this.InitializeComponent();
            this.button1.Enabled = instance;
            this.button2.Enabled = type;
            this.button3.Enabled = shared;
            this.label4.Text = sh;
            this.label5.Text = ins;
            this.label6.Text = ty;
            if (instance)
                this.button1.Cursor = Cursors.Hand;
            if (type)
                this.button2.Cursor = Cursors.Hand;
            if (!shared)
                return;
            this.button3.Cursor = Cursors.Hand;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Command.ss = 2;
            string text = this.textBox1.Text;
            List<string> stringList = new List<string>();
            Command.names = ((IEnumerable<string>)text.Split(this.stringSeparators, StringSplitOptions.None)).ToList<string>();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Command.ss = 0;
            string text = this.textBox1.Text;
            List<string> stringList = new List<string>();
            Command.names = ((IEnumerable<string>)text.Split(this.stringSeparators, StringSplitOptions.None)).ToList<string>();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Command.ss = 1;
            string text = this.textBox1.Text;
            List<string> stringList = new List<string>();
            Command.names = ((IEnumerable<string>)text.Split(this.stringSeparators, StringSplitOptions.None)).ToList<string>();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.button3.Enabled = false;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            this.button3.Enabled = true;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
