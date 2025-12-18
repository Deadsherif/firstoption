using ModelHealthChecker.MVVM.ViewModel;
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

namespace ModelHealthChecker.MVVM.View
{
    /// <summary>
    /// Interaction logic for WarningsView.xaml
    /// </summary>
    public partial class WarningsView : Window
    {
        private static CheckerViewModel Vm;
        // Static instance of MainWindow for ensuring a single instance
        private static WarningsView instance;
        // Property indicating whether the window is closed
        private bool IsClosed { get; set; }


        private WarningsView()
        {
            InitializeComponent();
            //this.DataContext = Checker_Context.frm.Vm;
        }

        private WarningsView(CheckerViewModel mainWindowViewModel) : this()
        {
            // Set the MainWindowViewModel and ExternalEvent
            Vm = mainWindowViewModel;
            DataContext = mainWindowViewModel;

        }

        public static WarningsView CreateInstance(CheckerViewModel mainWindowViewModel)
        {
            if (instance == null || instance.IsClosed)
            {
                instance = new WarningsView(mainWindowViewModel);
            }
            else
            {
                instance = new WarningsView(mainWindowViewModel);
            }

            return instance;
        }
    }
}
