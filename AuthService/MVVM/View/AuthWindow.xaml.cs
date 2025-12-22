using System.Windows;
using AuthService.MVVM.ViewModel;

namespace AuthService.MVVM.View
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
            DataContext = new AuthViewModel();
        }
    }
}

