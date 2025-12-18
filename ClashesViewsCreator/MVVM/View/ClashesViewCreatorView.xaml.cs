using ClashesViewsCreator.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ClashesViewCreatorView.xaml
    /// </summary>
    public partial class ClashesViewCreatorView : Window
    {
        public ClashViewsCreatorViewModel VM;
        public ClashesViewCreatorView()
        {
            InitializeComponent();
            VM = new ClashViewsCreatorViewModel();
            this.DataContext = VM;
        }
        protected override void OnClosed(EventArgs e)
        {
            Command.IsOpen = false;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Command.IsOpen = false;
        }
    }
}
