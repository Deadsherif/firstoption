
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Firebase.Auth.UI;
using Firebase.Auth.Wpf.Sample;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;


namespace Firebase.Auth.Wpf.Sample
{
    public partial class MainWindow : Window
    {
   
    
        public static MainWindow instance { get; set; }
        public bool IsClosed { get; private set; }
        private ExternalEvent _externalEvent;


        public MainWindow(ExternalEvent externalEvent)
        {
            InitializeComponent();
            //IntPtr revitMainWindowHandle = new IntPtr(mainClass.uiapp.MainWindowHandle.ToInt64());
            _externalEvent = externalEvent;
            FirebaseUI.Instance.Client.AuthStateChanged += this.AuthStateChanged;
          
        }

        private void AuthStateChanged(object sender, UserEventArgs e)
        {
           this.Dispatcher.Invoke(async () =>
            {
                if (e.User == null)
                {
                    //CentralView.CreateInstance(_mainClass).Close();
                    //this.Show();
                    await FirebaseUI.Instance.Client.SignInAnonymouslyAsync();
                    this.Frame.Navigate(new LoginPage());
                }
                else if (e.User.IsAnonymous)
                {
                    //CentralView.CreateInstance(_mainClass).Close();
                    //this.Show();
                    this.Frame.Navigate(new LoginPage());
                }
                else if ((this.Frame.Content == null || this.Frame.Content.GetType() != typeof(MainPage)))
                {
                    //Hide();
                    //CentralView.CreateInstance(_mainClass).Show() ;
                    this.Frame.Navigate(new MainPage());
                    _externalEvent.Raise();
                    //this.Frame.Navigate();

                    //this.Close();
                    //IsClosed = true;
                    //_centralView.Show();
                }
            });
        }
        public static MainWindow CreateInstance(ExternalEvent externalEvent)
        {
            if (instance == null || instance.IsClosed)
                instance = new MainWindow(externalEvent);
            else
                instance.Activate();

            return instance;
        }
        protected override void OnClosed(EventArgs e) => IsClosed = true;

    }
}
