using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Globalization;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;


namespace ViewsTransfer
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {

            //Panel
            RibbonPanel panel = ribbonpanel(a);
            //Assemblylocation
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
            string thisassemblypath = Assembly.GetExecutingAssembly().Location;
            //Images
            #region Images
            var img = Properties.Resources.Views;
            ImageSource imgsc = GetImageSource(img);

            #endregion
            //Buttons
            #region Buttons
            PushButton button = panel.AddItem(new PushButtonData("Views Transfer", "Views Transfer", thisassemblypath, "ViewsTransfer.Transfer")) as PushButton;

            button.Image = imgsc;
            button.LargeImage = imgsc;
            button.Enabled = true;


            #endregion
            // Firebase UI initialization
 

            a.ApplicationClosing += a_ApplicationClosing;
            a.Idling += a_Idling;
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
        public RibbonPanel ribbonpanel(UIControlledApplication a)
        {
            string tab = "Views Transfer";
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
                RibbonPanel panel = a.CreateRibbonPanel(tab, "Views Transfer");
            }
            catch { }
            //check if panel exist
            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels)
            {
                //check if the pannel exist if it exist return the pannel if not return the new pannel
                if (p.Name == "Views Transfer")
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
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
