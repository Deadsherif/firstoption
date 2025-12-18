using ClashesViewsCreator.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ClashesViewsCreator.MVVM.View
{
    /// <summary>
    /// Interaction logic for FilterView.xaml
    /// </summary>
    public partial class FilterView : Window
    {
        public FilterView()
        {
            InitializeComponent();
            this.DataContext = Command.frm.VM;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ObservableCollection<FO_Num> selectedId = new ObservableCollection<FO_Num>();
            foreach (FO_Num s in listBox.SelectedItems)
            {
                selectedId.Add(s);
            }

            Command.frm.VM.SelectedIds = selectedId;
        }
    }
}
