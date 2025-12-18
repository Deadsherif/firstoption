using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System.Collections.ObjectModel;
using Autodesk.Revit.Creation;
using Document = Autodesk.Revit.DB.Document;
using System.Xml.Linq;

namespace GetElementRoom
{
    public class Helper
    {
        public static List<Element> GetModelElements(Document document, View3D view)
        {
            var elements = new FilteredElementCollector(document, view.Id)
        .WhereElementIsNotElementType() // Filters out elements that are ElementType.
        .Where(X => X.Category != null && // Check if Category is not null
            (X.Category.CategoryType == CategoryType.Model))
        .ToList(); // Converts the filtered elements to a list.
            return elements;
        }
    }
}
