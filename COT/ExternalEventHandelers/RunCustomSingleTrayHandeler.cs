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
    internal class RunCustomSingleTrayHandeler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            COT_Controller controller = new COT_Controller();

            CableTray singleTray = null;

            TaskDialog.Show("Custom Single tray run", "Select custom single cable tray to fill with conduits");

            XYZ startP = new XYZ();
            XYZ endP = new XYZ();
            //try
            //{
            while (true)
            {
                Reference reff = sel.PickObject(ObjectType.Element);
                XYZ pickPoint = reff.GlobalPoint;
                Element e = doc.GetElement(reff);
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

                    break;

                }
                //}

            }
            //catch (Exception)
            //{

            //    ApplicationStatic_DB.MainForm.Show();
            //    return;
            //}


            Transaction tr = new Transaction(doc);
            tr.Start("COT");

            //List<CableTray> singleTrayListWrapper = new List<CableTray>();
            CurveWithData singleTray_CWD = new CurveWithData(singleTray, startP, endP);
            //singleTrayListWrapper.Add(singleTray);
            List<CurveWithData> singleCurveWithData = new List<CurveWithData>();
            singleCurveWithData.Add(singleTray_CWD);

            for (int i = 0; i < ApplicationStatic_DB.cutomConds.Count; i++)
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //////calculate X,Y foreach custom conduit
                Curve customTrayCurve = (ApplicationStatic_DB.cutomTray.Location as LocationCurve).Curve;
                XYZ conduitStartPoint = (ApplicationStatic_DB.cutomConds[i].Location as LocationCurve).Curve.GetEndPoint(0);
                XYZ projectedTrayPoint = customTrayCurve.Project(conduitStartPoint).XYZPoint;
                XYZ projectedShiftedTrayPoint = ApplicationStatic_DB.customReferrenceAnchor.Project(conduitStartPoint).XYZPoint;



                double _Y = conduitStartPoint.Z - projectedTrayPoint.Z;
                double _X = Math.Pow((Math.Pow(projectedShiftedTrayPoint.X - conduitStartPoint.X, 2) + Math.Pow(conduitStartPoint.Y - projectedShiftedTrayPoint.Y, 2)), 0.5);

                double Y = _Y + (ApplicationStatic_DB.cutomTray.Height / 2);//OK
                //double X = (ApplicationStatic_DB.cutomTray.Width / 2) - _X;//notOK
                double X = _X;//OK
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                controller.DrawCustomConduits(doc, singleCurveWithData, ApplicationStatic_DB.cutomConds[i].GetTypeId(), singleTray.ReferenceLevel.Id, ApplicationStatic_DB.cutomConds[i].Diameter, X, Y, 0, false, false);

            }
            tr.Commit();
            ApplicationStatic_DB.MainForm.Show();
        }

        public string GetName()
        {
            return "ok";
        }
    }
}
