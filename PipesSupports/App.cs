using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Drawing.Imaging;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth.UI;

namespace PipesSupports
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            //Panel
            RibbonPanel panel = ribbonpanel(a, "First Option", "MEP");
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
            //Assemblylocation
            string thisassemblypath = Assembly.GetExecutingAssembly().Location;
            //Images
            #region Images
            var img = Properties.Resources.pipe;
            ImageSource imgsc = GetImageSource(img);

            #endregion
            //Buttons
            #region Buttons
            PushButton button = panel.AddItem(new PushButtonData("PipesSupport", "Pipes Hangers", thisassemblypath, "PipesSupports.Command")) as PushButton;

            button.Image = imgsc;
            button.LargeImage = imgsc;
            button.Enabled = true;


            #endregion
            // Firebase UI initialization
            FirebaseUI.Initialize(new FirebaseUIConfig
            {
                ApiKey = "AIzaSyD23YRTAiQoy8AH84I3tKdEU296UqyTzQk",
                AuthDomain = "firstoption-trial.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                },
                PrivacyPolicyUrl = "https://github.com/step-up-labs/firebase-authentication-dotnet",
                TermsOfServiceUrl = "https://github.com/step-up-labs/firebase-database-dotnet",
                IsAnonymousAllowed = false,
                AutoUpgradeAnonymousUsers = false,
                UserRepository = new FileUserRepository("FirebaseSample"),

                // Func called when upgrade of anonymous user fails because the user already exists
                // You should grab any data created under your anonymous user, sign in with the pending credential
                // and copy the existing data to the new user
                // see details here: https://github.com/firebase/firebaseui-web#upgrading-anonymous-users
                AnonymousUpgradeConflict = conflict => conflict.SignInWithPendingCredentialAsync(true)
            });
            a.ApplicationClosing += a_ApplicationClosing;
            a.Idling += a_Idling;
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
        private BitmapSource GetImageSource(System.Drawing.Image img)
        {
            BitmapImage bmp = new BitmapImage();

            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = null;
                bmp.StreamSource = ms;
                bmp.EndInit();
            }
            return bmp;
        }
        void a_ApplicationClosing(object sender, Autodesk.Revit.UI.Events.ApplicationClosingEventArgs e)
        {
            throw new NotImplementedException();
        }

        void a_Idling(object sender, Autodesk.Revit.UI.Events.IdlingEventArgs e)
        {

        }


        public RibbonPanel ribbonpanel(UIControlledApplication a, string tabName, string panelName)
        {
            string tab = tabName;
            RibbonPanel ribbonpanel = null;
            //create tab
            try
            {
                a.CreateRibbonTab(tab);
            }
            catch { }
            //create panel  
            try
            {
                //a.createRibbonPanel(Tab Name, Panel Name)
                RibbonPanel panel = a.CreateRibbonPanel(tab, panelName);
            }
            catch { }
            //check if panel exist
            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels)
            {
                //check if the pannel exist if it exist return the pannel if not return the new pannel
                if (p.Name == panelName)
                {
                    ribbonpanel = p;
                    break;
                }
            }
            return ribbonpanel;

        }
        private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            int position = args.Name.IndexOf(",");
            if (position > -1)
            {
                try
                {
                    string assemblyName = args.Name.Substring(0, position);
                    string assemblyFullPath = string.Empty;

                    //look in main folder
                    assemblyFullPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + assemblyName + ".dll";
                    if (File.Exists(assemblyFullPath))
                        return Assembly.LoadFrom(assemblyFullPath);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return null;
        }
    }
}
