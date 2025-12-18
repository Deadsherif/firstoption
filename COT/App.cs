using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace COT
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            //Panel
            RibbonPanel panel = ribbonpanel(a, "First Option", "MEP");
            //Assemblylocation
            string thisassemblypath = Assembly.GetExecutingAssembly().Location;
            //Images
            #region Images
            var img = Properties.Resources.wire;
            ImageSource imgsc = GetImageSource(img);

            #endregion
            //Buttons
            #region Buttons
            PushButton button = panel.AddItem(new PushButtonData("COT", "Cable On Tray", thisassemblypath, "COT.Command")) as PushButton;

            button.Image = imgsc;
            button.LargeImage = imgsc;
            button.Enabled = true;


            #endregion

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
    }
}
