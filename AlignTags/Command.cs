using AlignTags.MVVM.View;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AlignTags
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static UIDocument UiDoc;
        public static Document Doc;
        public static AlignTagsView frm;
        public static List<Element> tags;
        public static List<Element> FinalTags;
        public static double SavedAngle;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UiDoc = commandData.Application.ActiveUIDocument;
            Doc = commandData.Application.ActiveUIDocument.Document;


            tags = new List<Element>();

            if (Doc.ActiveView is ViewPlan || Doc.ActiveView is ViewSection)
            {
                if (SavedAngle == 0)
                {
                    SavedAngle = 45;
                }
                frm = new AlignTagsView();
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("This tool can only be used in plan, elevation and section views!", "First Option", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return Result.Succeeded;
            }

            return Result.Succeeded;
        }
    }
}
