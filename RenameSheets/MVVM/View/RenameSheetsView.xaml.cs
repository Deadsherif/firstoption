using RenameSheets.MVVM.ViewModel;
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

namespace RenameSheets.MVVM.View
{
    /// <summary>
    /// Interaction logic for RenameSheetsView.xaml
    /// </summary>
    public partial class RenameSheetsView : Window
    {
        public RenameSheetsViewModel VM;
        public RenameSheetsView()
        {
            InitializeComponent();
            VM = new RenameSheetsViewModel();
            this.DataContext = VM;
        }
    }
}
