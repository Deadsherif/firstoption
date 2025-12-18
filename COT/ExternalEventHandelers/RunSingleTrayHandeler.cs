using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Electrical;
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
    internal class RunSingleTrayHandeler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            ApplicationStatic_DB.points.Clear();
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            COT_Controller controller = new COT_Controller();

            CableTray singleTray = null;

            TaskDialog.Show("Single tray run", "Select single cable tray to fill with conduits");
            XYZ startP = new XYZ();
            XYZ endP = new XYZ();
            Reference reff = sel.PickObject(ObjectType.Element);
            XYZ pickPoint = reff.GlobalPoint;
            Element e = doc.GetElement(reff);
            try
            {
                while (true)
                {
                    if (e is CableTray)
                    {
                        Curve c = (e.Location as LocationCurve).Curve;
                        XYZ ep0 = c.GetEndPoint(0);
                        XYZ ep1 = c.GetEndPoint(1);
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
                        //////////////////
                        singleTray = e as CableTray;
                        break;
                    }
                }



            }
            catch (Exception)
            {

                ApplicationStatic_DB.MainForm.Show();
                return;
            }


            foreach (Element t in ApplicationStatic_DB.conduitTypes)
            {
                if (t.Name == ApplicationStatic_DB.conduitTypeName)
                {
                    ApplicationStatic_DB.conduitType = t;
                    break;
                }
            }
            if (ApplicationStatic_DB.conduitType == null || singleTray == null)
            {
                ApplicationStatic_DB.MainForm.Show();
                return;
            }






            CurveWithData singleTray_CWD = new CurveWithData(singleTray, startP, endP);
            List<CurveWithData> singleCurveWithData = new List<CurveWithData>();
            singleCurveWithData.Add(singleTray_CWD);

            Transaction tr = new Transaction(doc);
            tr.Start("COT");
            double lastOffset = 0;
            double D_Current = 0;
            double D_Last = 0;
            for (int i = 0; i < ApplicationStatic_DB.ConduitsData.Count; i++)
            {
                D_Current = ApplicationStatic_DB.ConduitsData[i];
                double _lastOffset = controller.DrawConduits(doc, singleCurveWithData, ApplicationStatic_DB.conduitType.Id, singleTray.ReferenceLevel.Id, ApplicationStatic_DB.TrayThickness, D_Current, D_Last, lastOffset, false, ApplicationStatic_DB.shiftToTrayBottom, false, false);
                D_Last = ApplicationStatic_DB.ConduitsData[i];
                lastOffset = _lastOffset;

            }

            var c1 = ApplicationStatic_DB.points.FirstOrDefault();

            var c2 = ApplicationStatic_DB.points.LastOrDefault();



            var cx = e.get_Parameter(BuiltInParameter.RBS_CURVE_HOR_OFFSET_PARAM);
            var zz = cx.SetValueString("Right");
            var distance = c1.DistanceTo(c2);
            var Dl = ApplicationStatic_DB.ConduitsData[ApplicationStatic_DB.ConduitsData.Count - 1];
            var Df = ApplicationStatic_DB.ConduitsData[0];
            var dist = 1.25 * (distance + Dl + Df) * 304.8;
            var Rdist = Math.Floor(dist / 50.0) * 50.0;
            var diff = dist - Rdist;

            //var rnum = Math.Round(num, MidpointRounding.AwayFromZero);

            e.LookupParameter("Width").SetValueString(Rdist + "mm");

            tr.Commit();
            ApplicationStatic_DB.MainForm.Show();
        }




        public string GetName()
        {
            return "RunSingleTrayHandeler";
        }
    }
}
