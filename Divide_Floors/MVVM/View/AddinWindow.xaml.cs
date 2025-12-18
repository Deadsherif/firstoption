using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Divide_Floors.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

namespace Divide_Floors.MVVM.View
{
    /// <summary>
    /// Interaction logic for AddinWindow.xaml
    /// </summary>
    public partial class AddinWindow : Window
    {
        private UIDocument uidoc;
        private Document doc;
        public Reference roomreference;
        private Command fd;
        private ExternalCommandData cmdata;

        public AddinWindow(ExternalCommandData commandData)
        {
            fd = new Command();
            InitializeComponent();
            Bitmap bm = Divide_Floors.Properties.Resources.logo;


            logoImage.Source = BitmapToImageSource(bm);



            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;
            cmdata = commandData;
            var MATERIALnew = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Materials).AsEnumerable();
            foreach (var item in MATERIALnew)
            {
                MaterialCombo.Items.Add(item.Name);

            }

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
            Command.ApplyLogic(cmdata);
            this.ShowDialog();
        }

        private void OKbttn_Click(object sender, RoutedEventArgs e)
        {
            Command.DivideIt(doc);
            this.Close();
        }
        private void cancelbttn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private void selectpiecesbutton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Command.SelectDS_ByRoom(cmdata);
        }

        private void MaterialCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ytxt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
