using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using COT.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COT.ExternalEventHandelers
{
    [Transaction(TransactionMode.Manual)]
    internal class SelectCustomConds : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            COT_Controller controller = new COT_Controller();

            TaskDialog.Show("Select custom conduits", "Please select custom conduits of the sample collection");

            IList<Reference> condRefs = sel.PickObjects(ObjectType.Element);
            List<MEPCurve> conds = new List<MEPCurve>();
            foreach (Reference condRef in condRefs)
            {
                conds.Add(doc.GetElement(condRef) as MEPCurve);
            }
            ApplicationStatic_DB.cutomConds = conds;
            ApplicationStatic_DB.MainForm.Show();
        }

        public string GetName()
        {
            return "SelectSortedTrays";
        }
    }
}
