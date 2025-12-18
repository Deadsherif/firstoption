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
    internal class SelectCustomTray : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            COT_Controller controller = new COT_Controller();

            TaskDialog.Show("Select custom tray", "Please select cable tray of the sample collection");
            Reference reff = sel.PickObject(ObjectType.Element);
            XYZ pickPoint = reff.GlobalPoint;
            ApplicationStatic_DB.cutomTray = doc.GetElement(reff) as MEPCurve;
            Curve c = (ApplicationStatic_DB.cutomTray.Location as LocationCurve).Curve;
            XYZ ep0 = c.GetEndPoint(0);
            XYZ ep1 = c.GetEndPoint(1);
            XYZ startP = new XYZ();
            XYZ endP = new XYZ();
            if (pickPoint.DistanceTo(ep0) <= pickPoint.DistanceTo(ep1))
            {
                startP = ep0;
                endP = ep1;
            }
            else
            {
                startP = ep1;
                endP = ep0;
            }
            CurveWithData cwd = new CurveWithData(ApplicationStatic_DB.cutomTray, startP, endP);
            List<XYZ> shiftedPoints = cwd.GetShiftedCurvePoints(ApplicationStatic_DB.cutomTray.Width / 2);
            ApplicationStatic_DB.customReferrenceAnchor = Line.CreateBound(shiftedPoints[0], shiftedPoints[1]);

            ApplicationStatic_DB.MainForm.Show();
        }

        public string GetName()
        {
            return "SelectSortedTrays";
        }
    }
}
