using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using COT.MVVM.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace COT.MVVM.View
{
    public partial class COT_Form_Main : System.Windows.Forms.Form
    {
        ExternalEvent rste;
        ExternalEvent sste;
        ExternalEvent rmte;
        ExternalEvent rcste;
        ExternalEvent scte;
        ExternalEvent scce;
        ExternalEvent rmcte;

        public COT_Form_Main(ExternalEvent rste, ExternalEvent sste, ExternalEvent rmte, ExternalEvent rcste, ExternalEvent scte, ExternalEvent scce, ExternalEvent rmcte, List<Element> types)
        {
            InitializeComponent();

            this.rste = rste;
            this.sste = sste;
            this.rmte = rmte;
            this.rcste = rcste;
            this.scte = scte;
            this.scce = scce;
            this.rmcte = rmcte;

            foreach (Element ele in types)
            {
                comboBox1.Items.Add(ele.Name);
            }



        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //save
            var FolderName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); 
            string fileName = $@"{FolderName}\test.txt";

            // Check if file already exists. If yes, delete it.     
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            // Create a new file     
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine("");

                sw.WriteLine(comboBox1.Text);
                sw.WriteLine(textBox1.Text);

                sw.WriteLine(checkBox1.Checked.ToString());
                sw.WriteLine(checkBox2.Checked.ToString());
                sw.WriteLine(checkBox3.Checked.ToString());
                sw.WriteLine(checkBox4.Checked.ToString());

                sw.WriteLine(checkBox5.Checked.ToString());
                sw.WriteLine(textBox2.Text);

                sw.WriteLine(checkBox6.Checked.ToString());
                sw.WriteLine(textBox3.Text);

                sw.WriteLine(checkBox8.Checked.ToString());
                sw.WriteLine(textBox5.Text);

                sw.WriteLine(checkBox7.Checked.ToString());
                sw.WriteLine(textBox4.Text);

                sw.WriteLine(checkBox12.Checked.ToString());
                sw.WriteLine(textBox9.Text);

                sw.WriteLine(checkBox11.Checked.ToString());
                sw.WriteLine(textBox8.Text);

                sw.WriteLine(checkBox10.Checked.ToString());
                sw.WriteLine(textBox7.Text);

                sw.WriteLine(checkBox9.Checked.ToString());
                sw.WriteLine(textBox6.Text);

                sw.WriteLine(checkBox20.Checked.ToString());
                sw.WriteLine(textBox17.Text);

                sw.WriteLine(checkBox19.Checked.ToString());
                sw.WriteLine(textBox16.Text);

                sw.WriteLine(checkBox18.Checked.ToString());
                sw.WriteLine(textBox15.Text);

                sw.WriteLine(checkBox17.Checked.ToString());
                sw.WriteLine(textBox14.Text);

                sw.WriteLine(checkBox16.Checked.ToString());
                sw.WriteLine(textBox13.Text);

                sw.WriteLine(checkBox15.Checked.ToString());
                sw.WriteLine(textBox12.Text);

                sw.WriteLine(checkBox14.Checked.ToString());
                sw.WriteLine(textBox11.Text);

                sw.WriteLine(checkBox13.Checked.ToString());
                sw.WriteLine(textBox10.Text);


            }

            // Write file contents on console.     
            //using (StreamReader sr = File.OpenText(fileName))
            //{
            //    string s = "";
            //    while ((s = sr.ReadLine()) != null)
            //    {
            //    var k = s;
            //    }
            //}


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ApplicationStatic_DB.conduitTypeName = comboBox1.Text;
            ApplicationStatic_DB.TrayThickness = double.Parse(textBox1.Text) * 0.003280839895;
            ApplicationStatic_DB.firstTraySpacingCalculation = false;
            ApplicationStatic_DB.justifyFittings = checkBox3.Checked;
            ApplicationStatic_DB.withFittings = checkBox2.Checked;
            ApplicationStatic_DB.shiftToTrayBottom = checkBox1.Checked;

            List<double> conduitsData = new List<double>();

            if (checkBox5.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox2.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox6.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox3.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox8.Checked)
            {
                try
                {

                    conduitsData.Add(double.Parse(textBox5.Text) * 0.003280839895);
                }
                catch (Exception) { }
            }


            if (checkBox7.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox4.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }


            if (checkBox12.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox9.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox11.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox8.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox10.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox7.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox9.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox6.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox20.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox17.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox19.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox16.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox18.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox15.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox17.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox14.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox16.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox13.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox15.Checked)
            {
                try
                {

                    conduitsData.Add(double.Parse(textBox12.Text) * 0.003280839895);
                }
                catch (Exception) { }
            }

            if (checkBox14.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox11.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox13.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox10.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            ApplicationStatic_DB.ConduitsData = conduitsData;

            ///////////////////////////////////////////////////////////////////////
            rste.Raise();
            ////////////////////////////////////////// there are an issue here in raising event handeler 
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sste.Raise();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ApplicationStatic_DB.conduitTypeName = comboBox1.Text;
            ApplicationStatic_DB.TrayThickness = double.Parse(textBox1.Text) * 0.003280839895;
            ApplicationStatic_DB.firstTraySpacingCalculation = false;
            ApplicationStatic_DB.justifyFittings = checkBox3.Checked;
            ApplicationStatic_DB.withFittings = checkBox2.Checked;
            ApplicationStatic_DB.shiftToTrayBottom = checkBox1.Checked;

            List<double> conduitsData = new List<double>();

            if (checkBox5.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox2.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox6.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox3.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox8.Checked)
            {
                try
                {

                    conduitsData.Add(double.Parse(textBox5.Text) * 0.003280839895);
                }
                catch (Exception) { }
            }


            if (checkBox7.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox4.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }


            if (checkBox12.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox9.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox11.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox8.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox10.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox7.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox9.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox6.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox20.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox17.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox19.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox16.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox18.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox15.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox17.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox14.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox16.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox13.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox15.Checked)
            {
                try
                {

                    conduitsData.Add(double.Parse(textBox12.Text) * 0.003280839895);
                }
                catch (Exception) { }
            }

            if (checkBox14.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox11.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            if (checkBox13.Checked)
            {
                try
                {
                    conduitsData.Add(double.Parse(textBox10.Text) * 0.003280839895);

                }
                catch (Exception) { }
            }

            ApplicationStatic_DB.ConduitsData = conduitsData;

            ////////////
            rmte.Raise();
            this.Hide();
        }

        /// <summary>
        /// custom single tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            ApplicationStatic_DB.conduitTypeName = comboBox1.Text;

            rcste.Raise();
            this.Hide();

        }

        /// <summary>
        /// select custom tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            this.Hide();
            scte.Raise();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Hide();
            scce.Raise();

        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Hide();
            rmcte.Raise();
        }
    }
}
