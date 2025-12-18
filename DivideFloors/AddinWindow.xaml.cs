using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Reflection;
using System;
using System.Drawing;
using System.IO;

namespace DivideFloors
{
    /// <summary>
    /// Interaction logic for AddinWindow.xaml
    /// </summary>
    public partial class AddinWindow : Window
    {
        private UIDocument uidoc;
        private Document doc;
        public Reference roomreference;
        private FloorDivider fd;
        private ExternalCommandData cmdata;

        public AddinWindow( ExternalCommandData commandData)
        {
            fd = new FloorDivider();
            InitializeComponent();
            Bitmap bm = DivideFloors.Properties.Resources.LOGO;

            
            logoImage.Source = BitmapToImageSource(bm);



            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            cmdata = commandData;
            var MATERIALnew = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Materials).AsEnumerable();
            foreach (var item in MATERIALnew)
            {
                MaterialCombo.Items.Add(item.Name);
                
            }
            offsettxt.Text = Properties.Settings.Default.offset;
            widthtxt.Text = Properties.Settings.Default.width;
            lengthtxt.Text = Properties.Settings.Default.length;
            MaterialCombo.Text = Properties.Settings.Default.material;
            thicktxt.Text =  Properties.Settings.Default.thickness;
        }
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
  
      

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();
            roomreference = uidoc.Selection.PickObject(ObjectType.Element);
            this.ShowDialog();
        }
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
                ApplicationDB.reference = roomreference;
                ApplicationDB.Offset = offsettxt.Text;
                ApplicationDB.width = widthtxt.Text;
                ApplicationDB.length = lengthtxt.Text;
                ApplicationDB.GridX = xtxt.Text;
                ApplicationDB.GridY = ytxt.Text;
                ApplicationDB.thickness = thicktxt.Text;
                ApplicationDB.rotation = rotatxt.Text;
                ApplicationDB.material = MaterialCombo.Text;
            
            ///execute func
            FloorDivider.ApplyLogic(cmdata);
            this.ShowDialog();
        }

        private void OKbttn_Click(object sender, RoutedEventArgs e)
        {
            FloorDivider.DivideIt(doc);
            this.Close();
        }
        private void cancelbttn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void savebttn_Click(object sender, RoutedEventArgs e)
        { 
            Properties.Settings.Default.offset = offsettxt.Text;
            Properties.Settings.Default.width =  widthtxt.Text;
            Properties.Settings.Default.length = lengthtxt.Text;
            Properties.Settings.Default.material = MaterialCombo.Text;
            Properties.Settings.Default.thickness = thicktxt.Text;
            Properties.Settings.Default.Save();
        }

        private void selectpiecesbutton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            FloorDivider.SelectDS_ByRoom(cmdata);
        }
    }
}
