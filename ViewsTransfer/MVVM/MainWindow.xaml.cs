
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


using Firebase.Auth.UI;
using Firebase.Auth.Wpf.Sample;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using ViewsTransfer;




namespace Firebase.Auth.Wpf.Sample
{
    public partial class MainWindow : Window
    {


        public static MainWindow instance { get; set; }
        public bool IsClosed { get; private set; }
        private ExternalCommandData commandData;


        public MainWindow(ExternalCommandData externalCommandData)
        {
            InitializeComponent();


            commandData = externalCommandData;


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
                     //ApplicationStatic_DB.MainForm = new COT_Form_Main(_rste, _sste, _rmte, _rcste, _scte, _scce, _rmcte, _types);
                     //ApplicationStatic_DB.MainForm.Show();

                     int num1 = (int)new Transfer_Views().ShowDialog();
                     if (Transfer.state == 0)
                     {
                         if (commandData.Application.ActiveUIDocument.ActiveView.Category.Name == "Sheets")
                         {
                             int num2 = (int)TaskDialog.Show("Warning", "Please close all Sheet views first!");
                         }
                         else
                         {
                             int num3 = (int)TaskDialog.Show("Success", "Ready to paste sheets!");
                             Transfer.CopyAllViewSheets(commandData);
                         }
                     }
                     else if (Transfer.state == 1)
                         Transfer.PasteAllViewSheets(commandData);
                 }



                 //this.Close();
                 //IsClosed = true;
                 //_centralView.Show();
             
             });
        }
        public static MainWindow CreateInstance(ExternalCommandData externalCommandData)
        {
            if (instance == null || instance.IsClosed)
                instance = new MainWindow(externalCommandData);
            else
                instance.Activate();

            return instance;
        }
        protected override void OnClosed(EventArgs e) => IsClosed = true;

    }
}
