using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using COT.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace COT.ExternalEventHandelers
{
    [Transaction(TransactionMode.Manual)]
    internal class SelectSortedTrays : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            COT_Controller controller = new COT_Controller();

            TaskDialog.Show("Select sorted trays", "Please select cable trays with correct order!");

            ApplicationStatic_DB.SortedTrays = controller.GetSortedCurves(controller.SelectSortedTrays(sel, uidoc, doc));

            ApplicationStatic_DB.MainForm.Show();
        }

        public string GetName()
        {
            return "SelectSortedTrays";
        }
    }
}
