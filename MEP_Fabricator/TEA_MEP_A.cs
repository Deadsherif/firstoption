// Decompiled with JetBrains decompiler
// Type: MEP_Fabricator.TEA_MEP_A
// Assembly: MEP_Fabricator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8CFBF883-28B0-455B-8DCF-FF413BFD7A7C
// Assembly location: \\fs\Temp-Share\03 R&D\omar amen\New folder (4)\2020\MEP_Fabricator.dll

using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth;

using Firebase.Auth.UI;
using System.IO;



namespace MEP_Fabricator
{
    internal class TEA_MEP_A : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application) => (Result)0;

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                application.CreateRibbonTab("First Option");
            }
            catch (Exception)
            {

            }
            string location = Assembly.GetExecutingAssembly().Location;
            string str1 = location.Remove(location.Length - 18, 18);
            string str2 = str1 + "MEP_Fabricator.dll";
            string uriString = str1 + "fabricator.png";
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("First Option", "MEP");
            PushButtonData pushButtonData = new PushButtonData("btn3", "Fabricator", str2, "MEP_Fabricator.Fabricator");
            BitmapImage bitmapImage = new BitmapImage(new Uri(uriString));
            ((RibbonButton)(ribbonPanel.AddItem((RibbonItemData)pushButtonData) as PushButton)).LargeImage = (ImageSource)bitmapImage;
            // Firebase UI initialization
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
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
            return (Result)0;
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
