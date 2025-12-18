
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CableOverTray;

using Firebase.Auth.UI;
using Firebase.Auth.Wpf.Sample;

using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;


namespace Firebase.Auth.Wpf.Sample
{
    public partial class MainWindow : Window
    {


        public static MainWindow instance { get; set; }
        public bool IsClosed { get; private set; }
        private ExternalEvent _rste;
        private ExternalEvent _sste;
        private ExternalEvent _rmte;
        private ExternalEvent _rcste;
        private ExternalEvent _scte;
        private ExternalEvent _scce;
        private ExternalEvent _rmcte;
        private List<Element> _types;


        public MainWindow(ExternalEvent rste, ExternalEvent sste, ExternalEvent rmte, ExternalEvent rcste, ExternalEvent scte, ExternalEvent scce, ExternalEvent rmcte, List<Element> types)
        {
            InitializeComponent();
            _rste = rste;
            _sste = sste;
            _rmte = rmte;
            _rcste = rcste;
            _scte = scte;
            _scce = scce;
            _rmcte = rmcte;
          
          
            _types = types;

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
                     this.Frame.Navigate(new MainPage());
                     ApplicationStatic_DB.MainForm = new COT_Form_Main(_rste, _sste, _rmte, _rcste, _scte, _scce, _rmcte, _types);
                     ApplicationStatic_DB.MainForm.Show();

                     //this.Close();
                     //IsClosed = true;
                     //_centralView.Show();
                 }
             });
        }
        public static MainWindow CreateInstance(ExternalEvent rste, ExternalEvent sste, ExternalEvent rmte, ExternalEvent rcste, ExternalEvent scte, ExternalEvent scce, ExternalEvent rmcte, List<Element> types)
        {
            if (instance == null || instance.IsClosed)
                instance = new MainWindow(rste, sste, rmte, rcste, scte, scce, rmcte, types);
            else
                instance.Activate();

            return instance;
        }
        protected override void OnClosed(EventArgs e) => IsClosed = true;

    }
}
