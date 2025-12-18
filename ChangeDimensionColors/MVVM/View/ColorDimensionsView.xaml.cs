using ChangeDimensionColors.MVVM.ViewModel;
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

namespace ChangeDimensionColors.MVVM.View
{
    /// <summary>
    /// Interaction logic for ColorDimensionsView.xaml
    /// </summary>
    public partial class ColorDimensionsView : Window
    {
        public ColorDimensionsViewModel VM;
        public ColorDimensionsView()
        {
            InitializeComponent();
            VM = new ColorDimensionsViewModel();
            this.DataContext = VM;
        }
    }
}
