using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MEP_Fabricator
{
    public partial class ClashManager : Form
    {
        public int totals;
        public List<int> eles1 = new List<int>();
        public List<int> eles2 = new List<int>();
        public ExternalEvent eve;
        public int old1 = 0;
        public int old2 = 0;

        public ClashManager(string T, List<int> a, List<int> b, ExternalEvent e)
        {
            InitializeComponent();
            this.label2.Text = T;
            this.totals = int.Parse(T);
            this.eles1 = a;
            this.eles2 = b;
            this.eve = e;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
                return;
            e.Handled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int num = int.Parse(this.textBox1.Text) + 1;
            if (num > 0 && num <= this.totals)
            {
                this.textBox1.Text = num.ToString();
                MEP_NavisReportViewer_EventHandler.id1old = this.old1;
                MEP_NavisReportViewer_EventHandler.id2old = this.old2;
                MEP_NavisReportViewer_EventHandler.id1 = this.eles1[num - 1];
                MEP_NavisReportViewer_EventHandler.id2 = this.eles2[num - 1];
                this.eve.Raise();
                this.old1 = this.eles1[num - 1];
                this.old2 = this.eles2[num - 1];
            }
            else
            {
                this.textBox1.Text = "1";
                MEP_NavisReportViewer_EventHandler.id1old = this.old1;
                MEP_NavisReportViewer_EventHandler.id2old = this.old2;
                MEP_NavisReportViewer_EventHandler.id1 = this.eles1[0];
                MEP_NavisReportViewer_EventHandler.id2 = this.eles2[0];
                this.eve.Raise();
                this.old1 = this.eles1[0];
                this.old2 = this.eles2[0];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int num = int.Parse(this.textBox1.Text) - 1;
            if (num > 0 && num <= this.totals)
            {
                this.textBox1.Text = num.ToString();
                MEP_NavisReportViewer_EventHandler.id1old = this.old1;
                MEP_NavisReportViewer_EventHandler.id2old = this.old2;
                MEP_NavisReportViewer_EventHandler.id1 = this.eles1[num - 1];
                MEP_NavisReportViewer_EventHandler.id2 = this.eles2[num - 1];
                this.eve.Raise();
                this.old1 = this.eles1[num - 1];
                this.old2 = this.eles2[num - 1];
            }
            else
            {
                this.textBox1.Text = "1";
                MEP_NavisReportViewer_EventHandler.id1old = this.old1;
                MEP_NavisReportViewer_EventHandler.id2old = this.old2;
                MEP_NavisReportViewer_EventHandler.id1 = this.eles1[0];
                MEP_NavisReportViewer_EventHandler.id2 = this.eles2[0];
                this.eve.Raise();
                this.old1 = this.eles1[0];
                this.old2 = this.eles2[0];
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int num = int.Parse(this.textBox1.Text);
            if (num > 0 && num <= this.totals)
            {
                MEP_NavisReportViewer_EventHandler.id1old = this.old1;
                MEP_NavisReportViewer_EventHandler.id2old = this.old2;
                MEP_NavisReportViewer_EventHandler.id1 = this.eles1[num - 1];
                MEP_NavisReportViewer_EventHandler.id2 = this.eles2[num - 1];
                this.eve.Raise();
                this.old1 = this.eles1[num - 1];
                this.old2 = this.eles2[num - 1];
            }
            else
            {
                this.textBox1.Text = "1";
                MEP_NavisReportViewer_EventHandler.id1old = this.old1;
                MEP_NavisReportViewer_EventHandler.id2old = this.old2;
                MEP_NavisReportViewer_EventHandler.id1 = this.eles1[0];
                MEP_NavisReportViewer_EventHandler.id2 = this.eles2[0];
                this.eve.Raise();
                this.old1 = this.eles1[0];
                this.old2 = this.eles2[0];
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MEP_NavisReportViewer_EventHandler.id1old = this.old1;
            MEP_NavisReportViewer_EventHandler.id2old = this.old2;
            MEP_NavisReportViewer_EventHandler.id1 = 0;
            MEP_NavisReportViewer_EventHandler.id2 = 0;
            this.eve.Raise();
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void ClashManager_Load(object sender, EventArgs e)
        {
        }
    }
}
