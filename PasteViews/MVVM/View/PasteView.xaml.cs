using PasteViews.MVVM.ViewModel;
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

namespace PasteViews.MVVM.View
{
    /// <summary>
    /// Interaction logic for PasteView.xaml
    /// </summary>
    public partial class PasteView : Window
    {
        public PasteViewModel VM;
        public PasteView()
        {
            InitializeComponent();
            VM = new PasteViewModel();
            this.DataContext = VM;
        }
    }
}
