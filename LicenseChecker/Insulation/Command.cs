using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Insulation.ExternalEventHandelers;
using Insulation.MVVM.Model;
using Insulation.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insulation
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Database.selectedcategory = null;
            Database.selectedlevel = null;
            Database.selectedmaterial = null;
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            // get all categories and save it in database class
            var categories = doc.Settings.Categories;
            List<Category> cats = new List<Category>();
            foreach (Category cat in categories)
            {
                cats.Add(cat);
            }
            cats = cats.OrderBy(g => g.Name).ToList();

            Database.categories = cats;
            // get all materials and save it in database class 
            var fec = new FilteredElementCollector(doc);
            var mats = fec.OfCategory(BuiltInCategory.OST_Materials).WhereElementIsNotElementType().Cast<Material>().OrderBy(m => m.Name).ToList();
            Database.materials = mats;
            var fec2 = new FilteredElementCollector(doc);
            var levels = fec2.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToList();
            Database.levels = levels;
            var R = uidoc.Selection.PickObject(ObjectType.Element);
            InsulationType type = new InsulationType();
            ExternalEvent ev = ExternalEvent.Create(type);
            UIWindow ui = new UIWindow(ev);
            ui.ShowDialog();
            return Result.Succeeded;
        }
    }
}
