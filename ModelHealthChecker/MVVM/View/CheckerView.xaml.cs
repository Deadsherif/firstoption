using ModelHealthChecker.MVVM.Model;
using ModelHealthChecker.MVVM.ViewModel;
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

namespace ModelHealthChecker.MVVM.View
{
    /// <summary>
    /// Interaction logic for CheckerView.xaml
    /// </summary>
    public partial class CheckerView : Window
    {

        // Static instance of the MainWindowViewModel for shared state
        private static CheckerViewModel Vm;



        // Static instance of MainWindow for ensuring a single instance
        private static CheckerView instance;

        // Property indicating whether the window is closed
        private bool IsClosed { get; set; }


        private CheckerView()
        {
            InitializeComponent();

        }



        // Override OnClosed method to update the IsClosed property
        protected override void OnClosed(EventArgs e)
        {
            IsClosed = true;

        }

        // Parameterized constructor to initialize the MainWindow
        private CheckerView(CheckerViewModel mainWindowViewModel) : this()
        {
            // Set the MainWindowViewModel and ExternalEvent
            Vm = mainWindowViewModel;
            DataContext = mainWindowViewModel;

        }

        public static CheckerView CreateInstance(CheckerViewModel mainWindowViewModel)
        {
            if (instance == null || instance.IsClosed)
            {
                instance = new CheckerView(mainWindowViewModel);
            }
            else

                instance.Activate();

            return instance;
        }

        private void FamilyDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Command.VM.SelectedFamilies = new ObservableCollection<RevitFamiy>();
            foreach (var es in FamilyDG.SelectedItems)
            {
                Command.VM.SelectedFamilies.Add(es as RevitFamiy);
            }

        }


    }
}
