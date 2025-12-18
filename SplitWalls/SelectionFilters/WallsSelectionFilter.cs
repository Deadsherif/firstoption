using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWalls.SelectionFilters
{
    public class WallsSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category != null && elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
