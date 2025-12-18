using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Insulation.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Insulation.MVVM.View
{
    /// <summary>
    /// Interaction logic for UIWindow.xaml
    /// </summary>
    public partial class UIWindow : Window
    {
        public ExternalEvent nev;
        public UIWindow(ExternalEvent ev)
        {
            Height = 485;
            Width = 320;
            nev = ev;
            InitializeComponent();
            foreach (Category cat in Database.categories)
            {
                catcombo.Items.Add(cat);
            }
            foreach (Material mat in Database.materials)
            {
                matcombo.Items.Add(mat);
            }
            foreach (Level level in Database.levels)
            {
                levelcombo.Items.Add(level);
            }


        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            levelcombo.IsEnabled = true;
            var radioname = sender as RadioButton;
            var x = radioname.Content.ToString();
            Database.radioname = x;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            offsettxt.IsEnabled = true;
            Database.booloffset = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            offsettxt.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (thicktxt.Text == null || catcombo.SelectedValue == null || matcombo.SelectedValue == null)
            {
                TaskDialog.Show("Erorr", "Please Fill All feilds ");
                this.Activate();
            }
            else
            {


                var thick = UnitUtils.ConvertToInternalUnits(double.Parse(thicktxt.Text), UnitTypeId.Millimeters);

                Database.thickness = thick;
                Database.selectedcategory = catcombo.SelectedValue as ElementId;
                Database.selectedlevel = levelcombo.SelectedItem as Level;
                Database.selectedmaterial = matcombo.SelectedValue as ElementId;
                if (offsettxt.IsEnabled)
                {
                    Database.offsetfromface = double.Parse(offsettxt.Text);

                }
                else
                {
                    nev.Raise();
                    //progressBar.Visibility = System.Windows.Visibility.Visible;
                    //CreateAThousandBuildings();
                    Close();
                }
            }




        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            levelcombo.IsEnabled = false;
            var radioname = sender as RadioButton;
            var x = radioname.Content.ToString();
            Database.radioname = x;

        }
        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            levelcombo.IsEnabled = false;
            var radioname = sender as RadioButton;
            var x = radioname.Content.ToString();
            Database.radioname = x;

        }
        private delegate void ProgressBarDelegate();
        //long busy loop
        private void CreateAThousandBuildings()
        {


            Process processes = Process.GetCurrentProcess();

            progressBar.IsIndeterminate = false;
            progressBar.Maximum = 500;
            progressBar.Minimum = 0;

            for (var i = 0; i < 500; i++)
            {
                progressBar.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress), DispatcherPriority.Background);
                //Create a Building...
                //...
            }
        }

        //update the progress bar
        private void UpdateProgress()
        {
            progressBar.Value += 1;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
