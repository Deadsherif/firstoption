using AlignTags.MVVM.ViewModel;
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

namespace AlignTags.MVVM.View
{
    /// <summary>
    /// Interaction logic for AlignTagsView.xaml
    /// </summary>
    public partial class AlignTagsView : Window
    {
        public AlignTagsViewModel VM { get; set; }
        public AlignTagsView()
        {
            InitializeComponent();
            VM = new AlignTagsViewModel();
            this.DataContext = VM;
        }
    }
}
