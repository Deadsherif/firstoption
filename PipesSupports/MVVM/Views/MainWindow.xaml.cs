
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Firebase.Auth.UI;
using Firebase.Auth.Wpf.Sample;
using PipesSupports;
using PipesSupports.MVVM.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;


namespace Firebase.Auth.Wpf.Sample
{
    public partial class MainWindow : Window
    {
  
        private ExternalEvent _externalEvent;
        private CreatHangersExteranlEventHandler _creatHangersExteranlEventHandler;
        public static MainWindow instance { get; set; }
        public bool IsClosed { get; private set; }
     
        public MainWindow(CreatHangersExteranlEventHandler creatHangersExteranlEventHandler, ExternalEvent externalEvent)
        {
            InitializeComponent();
            _externalEvent = externalEvent;
            _creatHangersExteranlEventHandler = creatHangersExteranlEventHandler;
            //WindowInteropHelper helper = new WindowInteropHelper(this);
            //helper.Owner = revitMainWindowHandle;


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
                    var view = new PipeSupportView(_creatHangersExteranlEventHandler, _externalEvent);
                    this.Frame.Navigate(view);
                    this.Width= view.Width; this.Height= view.Height;
                    this.WindowState = WindowState.Normal;
                  
                    //this.Close();
                    //IsClosed = true;
                    //_centralView.Show();
                }
            });
        }
        public static MainWindow CreateInstance(CreatHangersExteranlEventHandler creatHangersExteranlEventHandler , ExternalEvent externalEvent)
        {
            if (instance == null || instance.IsClosed)
                instance = new MainWindow(creatHangersExteranlEventHandler, externalEvent);
            else
                instance.Activate();

            return instance;
        }
        protected override void OnClosed(EventArgs e) => IsClosed = true;

    }
}
