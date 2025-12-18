using CableTrayHangers.MVVM.ViewModel;
using Firebase.Auth.UI;
using Firebase.Auth;
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
using Autodesk.Revit.UI;

namespace CableTrayHangers.MVVM.View
{
    /// <summary>
    /// Interaction logic for PipeSupportView.xaml
    /// </summary>
    public partial class PipeSupportView : Page
    {
        public TraySupportViewModel VM;
        public PipeSupportView(CreatHangersExteranlEventHandler creatHangersExteranlEventHandler, ExternalEvent externalEvent)
        {
            InitializeComponent();
            VM = new TraySupportViewModel(this, externalEvent);
            this.DataContext = VM;
            creatHangersExteranlEventHandler.VM = VM;
        }
        private void AuthStateChanged(object sender, UserEventArgs e)
        {
            var user = e.User;

            this.Dispatcher.Invoke(() =>
            {

                //NameTextBlock.Text = user.Info.FirstName;
                if (user != null)
                {
                    EmailTextBlock.Text = user.Info.Email;
                    if (!string.IsNullOrWhiteSpace(user.Info.PhotoUrl))
                    {
                        //ProfileImage.Source = new BitmapImage(new Uri(user.Info.PhotoUrl));
                    }

                }
                //ProviderTextBlock.Text = user.Credential.ProviderType.ToString();


            });
        }

        private void SignOutClick(object sender, RoutedEventArgs e)
        {

            FirebaseUI.Instance.Client.AuthStateChanged -= this.AuthStateChanged;
            FirebaseUI.Instance.Client.SignOut();


        }

    }
}
