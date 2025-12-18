using System;
using System.Collections.Generic;
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
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace GridsDimension.MVVM.View
{
    /// <summary>
    /// Interaction logic for Grids.xaml
    /// </summary>
    public partial class Grids : Window
    {
        public Grids()
        {
            InitializeComponent();
            this.DataContext = Command.VMD;
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Autodesk.Revit.DB.View> Views = new List<Autodesk.Revit.DB.View>();
            foreach (Autodesk.Revit.DB.View V in LstBox.SelectedItems)
            {
                Views.Add(V);
            }
            Command.VMD.cmd.Execute(Views);

        }
    }
}
