using Beams_RCD.MVVM.ViewModel;
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

namespace Beams_RCD.MVVM.View
{
    /// <summary>
    /// Interaction logic for BeamsView.xaml
    /// </summary>
    public partial class BeamsView : Window
    {
        public static BeamsViewModel VM;
        public BeamsView()
        {
            InitializeComponent();
            VM = new BeamsViewModel();
            this.DataContext = VM;
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
