using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlignTags.SelectionFilters
{
    public class TagsSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category != null &&
                (
                elem is IndependentTag
                );
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
