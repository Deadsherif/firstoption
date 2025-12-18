using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeDimensionColors.MVVM.View;

namespace ChangeDimensionColors
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static ColorDimensionsView frm;
        public static UIDocument UiDoc;
        public static Document Doc;
        public static SelectDims dimFilter;
        public static List<Reference> reffEle;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UiDoc = commandData.Application.ActiveUIDocument;
            Doc = commandData.Application.ActiveUIDocument.Document;
            dimFilter = new SelectDims();

            frm = new ColorDimensionsView();
            frm.ShowDialog();



            return Result.Succeeded;
        }


    }
    public class SelectDims : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category != null && elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Dimensions;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
