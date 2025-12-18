using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



using Autodesk.Revit.DB.Events;
using COT.ExternalEventHandelers;
using COT.MVVM.Model;
using COT.MVVM.View;
using Firebase.Auth.Wpf.Sample;

namespace COT
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            message = "ERROR !!!";
            UIApplication uiapp = commandData.Application;
            uiapp.Application.FailuresProcessing += new EventHandler<FailuresProcessingEventArgs>(DoFailureProcessing);
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            RunSingleTrayHandeler rsteh = new RunSingleTrayHandeler();
            ExternalEvent rste = ExternalEvent.Create(rsteh);

            SelectSortedTrays ssteh = new SelectSortedTrays();
            ExternalEvent sste = ExternalEvent.Create(ssteh);

            RunMultibleTraysHandeler rmteh = new RunMultibleTraysHandeler();
            ExternalEvent rmte = ExternalEvent.Create(rmteh);

            RunCustomSingleTrayHandeler rcsteh = new RunCustomSingleTrayHandeler();
            ExternalEvent rcste = ExternalEvent.Create(rcsteh);

            SelectCustomTray scteh = new SelectCustomTray();
            ExternalEvent scte = ExternalEvent.Create(scteh);

            SelectCustomConds scceh = new SelectCustomConds();
            ExternalEvent scce = ExternalEvent.Create(scceh);

            RunMultibleCustomTraysHandeler rmcteh = new RunMultibleCustomTraysHandeler();
            ExternalEvent rmcte = ExternalEvent.Create(rmcteh);

            FilteredElementCollector Collector = new FilteredElementCollector(doc);
            IList<Element> conduitTypes = Collector.OfCategory(BuiltInCategory.OST_Conduit).WhereElementIsElementType().ToElements();

            List<Element> types = new List<Element>();


            foreach (Element ele in conduitTypes)
            {
                types.Add(ele);
            }

            ApplicationStatic_DB.conduitTypes = types;


            var mainWindow = MainWindow.CreateInstance(rste, sste, rmte, rcste, scte, scce, rmcte, types);
            mainWindow.Show();

            ////ExteranlEventHandler external = new ExteranlEventHandler();
            ////ExternalEvent.Create(external).Raise();
            ////var uiDoc = app.ActiveUIDocument;
            ////var doc = app.ActiveUIDocument.Document;
            //var ref1 = uidoc.Selection.PickObject(ObjectType.Element);
            //var ref2 = uidoc.Selection.PickObject(ObjectType.Element);
            //Element ele1 = doc.GetElement(ref1);
            //Element ele2 = doc.GetElement(ref2);


            //List<Connector> ccc = GetMEP_NearestConnector(ele1, ele2);


            //if (ccc != null)
            //{
            //    Transaction tr = new Transaction(doc);
            //    tr.Start("aa");
            //    try
            //    {
            //        doc.Create.NewElbowFitting(ccc[0], ccc[1]);
            //    }
            //    catch (Exception)
            //    {

            //        try
            //        {
            //            doc.Create.NewUnionFitting(ccc[0], ccc[1]);
            //        }
            //        catch (Exception)
            //        {


            //            tr.RollBack();

            //        }
            //    }


            //    tr.Commit();
            //}
           return Result.Succeeded;
            

        
        }

        public double DrawConduits(Document doc, List<CurveWithData> sortedCurves, ElementId conduitTypeID, ElementId levelID, double TrayThickness, double D_current, double D_last, double lastOffset, bool firstTraySpacingCalculation, bool shiftToTrayBottom, bool withFittings, bool justifyFittings)
        {
            double output = lastOffset;
            double offset = 0;
            List<Conduit> conds = new List<Conduit>();
            //Transaction ttt = new Transaction(doc);

            //ttt.Start("conduits");
            //foreach (CableTray tray in trays)
            //{
            //    LocationCurve ss = tray.Location as LocationCurve;


            //    conds.Add(Conduit.Create(doc, d.GetTypeId(), ss.Curve.GetEndPoint(0), ss.Curve.GetEndPoint(1), d.ReferenceLevel.Id));
            //}
            bool flag = true;
            foreach (CurveWithData curveData in sortedCurves)
            {
                double D = D_current;//From_User
                double r = D / 2;
                double trayThickness = TrayThickness; //From_User
                double _h = (curveData.curveHost.Height / 2);
                if (flag)
                {
                    if (D_last == 0)
                    {
                        offset = (curveData.curveHost.Width / 2) - (D_current);
                        output = offset;

                    }
                    else
                    {
                        double DD = 0;
                        double dd = 0;
                        if (D_current > D_last)
                        {
                            DD = D_current;
                            dd = D_last;
                        }
                        else
                        {
                            dd = D_current;
                            DD = D_last;
                        }

                        offset = lastOffset - (D_last / 2) - (1.5 * DD);
                        output = offset;
                    }
                    flag = false;
                }
                //if (!firstTraySpacingCalculation)
                //{
                //        offset = (curveData.curveHost.Width / 2) - (D_current);
                //}
                //else
                //{
                //        offset = (sortedCurves[0].curveHost.Width / 2) - (D_current);
                //}

                //double offset = (sortedCurves[0].curveHost.Width / 2) - ((2 * r)*20);
                List<XYZ> points = new List<XYZ>();
                if (shiftToTrayBottom)
                {
                    points = ShiftConduitPointsToTrayBottom(curveData.GetShiftedCurvePoints(offset), _h - trayThickness, r, curveData.curveHost);

                }
                else
                {
                    points = curveData.GetShiftedCurvePoints(offset);

                }
                try
                {
                    Conduit cd = Conduit.Create(doc, conduitTypeID, points[0], points[1], levelID);
                    cd.LookupParameter("Diameter(Trade Size)").Set(r * 2);
                    conds.Add(cd);
                }
                catch (Exception)
                {

                    conds.Add(null);
                }

            }


            if (withFittings)
            {
                List<FamilyInstance> ds = new List<FamilyInstance>();
                for (int i = 0; i < conds.Count - 1; i++)
                {
                    if (conds[i] != null && conds[i + 1] != null)
                    {
                        Element _ele1 = conds[i];
                        Element _ele2 = conds[i + 1];
                        List<Connector> _ccc = GetMEP_NearestConnector(_ele1, _ele2);
                        try
                        {
                            FamilyInstance ss = doc.Create.NewElbowFitting(_ccc[0], _ccc[1]);
                            //sortedCurves[i]
                            CableTray ct = sortedCurves[i].curveHost as CableTray;
                            double w = ct.Width;
                            double h = ct.Height;

                            BoundingBoxXYZ bx = ss.get_BoundingBox(null);
                            double dz = bx.Max.Z - bx.Min.Z;
                            if (justifyFittings)
                            {
                                if (dz == 0 || dz < 0.2)
                                {
                                    //horizontal fitting
                                    double r = 0.0885826771653543;
                                    // double offset = (sortedCurves[i].curveHost.Width / 2) - (2 * r);
                                    ss.LookupParameter("Bend Radius").Set(w + (w / 2 - offset));
                                }
                                else
                                {
                                    //vertical fitting
                                    ss.LookupParameter("Bend Radius").Set(w - h / 2);
                                }

                            }


                        }
                        catch (Exception)
                        {

                            try
                            {
                                FamilyInstance ss = doc.Create.NewUnionFitting(_ccc[0], _ccc[1]);
                                ds.Add(ss);
                                //azdoc.Delete(ss.Id);
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                }
                foreach (Element item in ds)
                {
                    doc.Delete(item.Id);
                }

            }

            return output;

            // ttt.Commit();
        }

        public XYZ ShiftConduitPoint_Elevation(XYZ endPoint, double _h, double r)
        {
            return new XYZ(endPoint.X, endPoint.Y, endPoint.Z - _h + r);
        }
        public List<XYZ> ShiftConduitPoints_Elevation(List<XYZ> endPoints, double _h, double r)
        {
            List<XYZ> output = new List<XYZ>();
            foreach (XYZ endPoint in endPoints)
            {
                output.Add(new XYZ(endPoint.X, endPoint.Y, endPoint.Z - _h + r));
            }
            return output;
        }
        public List<XYZ> ShiftConduitPointsToTrayBottom(List<XYZ> endPoints, double _h, double r, MEPCurve curveHost)
        {
            CableTray ct = curveHost as CableTray;
            List<XYZ> output = new List<XYZ>();


            XYZ trayNormal = ct.CurveNormal;
            XYZ _trayNormal = trayNormal.Negate();

            XYZ X = new XYZ(1, 0, 0);
            XYZ Y = new XYZ(0, 1, 0);
            XYZ Z = new XYZ(0, 0, 1);

            double th1 = _trayNormal.AngleTo(X);
            double th2 = _trayNormal.AngleTo(Y);
            double th3 = _trayNormal.AngleTo(Z);

            XYZ start = new XYZ((endPoints[0].X + ((_h - r) * Math.Cos(th1))), (endPoints[0].Y + ((_h - r) * Math.Cos(th2))), (endPoints[0].Z + ((_h - r) * Math.Cos(th3))));
            XYZ end = new XYZ((endPoints[1].X + ((_h - r) * Math.Cos(th1))), (endPoints[1].Y + ((_h - r) * Math.Cos(th2))), (endPoints[1].Z + ((_h - r) * Math.Cos(th3))));
            output.Add(start);
            output.Add(end);

            return output;
        }
        public List<CurveWithData> GetSortedCurves(List<CableTray> sortedTrays)
        {
            List<CurveWithData> output = new List<CurveWithData>();
            XYZ lastStartPoint = new XYZ();
            XYZ lastEndPoint = new XYZ();
            for (int i = 0; i < sortedTrays.Count; i++)
            {
                if (i == 0 && sortedTrays.Count > 1)
                {
                    LocationCurve lc1 = sortedTrays[0].Location as LocationCurve;
                    LocationCurve lc2 = sortedTrays[1].Location as LocationCurve;

                    XYZ p1 = lc1.Curve.GetEndPoint(0);
                    XYZ p2 = lc1.Curve.GetEndPoint(1);

                    XYZ rp = lc2.Curve.GetEndPoint(0);

                    double d1 = p1.DistanceTo(rp);
                    double d2 = p2.DistanceTo(rp);

                    if (d1 < d2)
                    {
                        lastStartPoint = p2;
                        lastEndPoint = p1;
                        output.Add(new CurveWithData(sortedTrays[0], lastStartPoint, lastEndPoint));
                    }
                    else
                    {
                        lastStartPoint = p1;
                        lastEndPoint = p2;
                        output.Add(new CurveWithData(sortedTrays[0], lastStartPoint, lastEndPoint));
                    }
                }
                else
                {
                    LocationCurve lc = sortedTrays[i].Location as LocationCurve;
                    XYZ p1 = lc.Curve.GetEndPoint(0);
                    XYZ p2 = lc.Curve.GetEndPoint(1);
                    double d1 = p1.DistanceTo(lastEndPoint);
                    double d2 = p2.DistanceTo(lastEndPoint);

                    if (d1 < d2)
                    {
                        lastStartPoint = p1;
                        lastEndPoint = p2;
                        output.Add(new CurveWithData(sortedTrays[i], lastStartPoint, lastEndPoint));
                    }
                    else
                    {
                        lastStartPoint = p2;
                        lastEndPoint = p1;
                        output.Add(new CurveWithData(sortedTrays[i], lastStartPoint, lastEndPoint));
                    }

                }
            }

            return output;
        }
        public List<CableTray> SelectSortedTrays(Selection sel, UIDocument uidoc, Document doc)
        {
            List<CableTray> output = new List<CableTray>();
            List<ElementId> selections = new List<ElementId>();
            while (true)
            {
                sel.SetElementIds(selections);
                //refresh View
                uidoc.RefreshActiveView();
                try
                {
                    Element ele = doc.GetElement(sel.PickObject(ObjectType.Element, $"Selected CableTrays: {output.Count} Trays"));
                    int counter = 0;
                    bool isRemovedFromSelection = false;

                    foreach (ElementId id in selections)
                    {
                        if (id == ele.Id)
                        {
                            output.RemoveAt(counter);
                            selections.RemoveAt(counter);
                            sel.SetElementIds(selections);
                            //refresh View
                            uidoc.RefreshActiveView();
                            isRemovedFromSelection = true;
                            break;
                        }
                        counter++;
                    }
                    if (ele is CableTray && !isRemovedFromSelection)
                    {
                        output.Add(ele as CableTray);
                        selections.Add(ele.Id);
                        sel.SetElementIds(selections);
                        //refresh View
                        uidoc.RefreshActiveView();
                    }


                }
                catch (Exception)
                {

                    break;
                }
            }
            sel.SetElementIds(selections);
            return output;
        }
        public List<Connector> GetMEP_NearestConnector(Element ele1, Element ele2)
        {
            if (ele1.Category.Name == ele2.Category.Name)
            {
                List<Connector> output = new List<Connector>();
                switch (ele1.Category.Name)
                {
                    case "Cable Trays":
                        CableTray t1 = ElementToMEP<CableTray>(ele1);
                        CableTray t2 = ElementToMEP<CableTray>(ele2);
                        List<List<Connector>> cons = GetConnectorsFromElements(t1, t2);
                        output = GetNearestConnectors(cons[0], cons[1]);
                        break;
                    case "Conduits":
                        Conduit c1 = ElementToMEP<Conduit>(ele1);
                        Conduit c2 = ElementToMEP<Conduit>(ele2);
                        List<List<Connector>> conss = GetConnectorsFromElements(c1, c2);
                        output = GetNearestConnectors(conss[0], conss[1]);
                        break;
                    case "Pipes":
                        Pipe p1 = ElementToMEP<Pipe>(ele1);
                        Pipe p2 = ElementToMEP<Pipe>(ele2);
                        List<List<Connector>> consss = GetConnectorsFromElements(p1, p2);
                        output = GetNearestConnectors(consss[0], consss[1]);
                        break;
                    case "Ducts":
                        Duct d1 = ElementToMEP<Duct>(ele1);
                        Duct d2 = ElementToMEP<Duct>(ele2);
                        List<List<Connector>> conssss = GetConnectorsFromElements(d1, d2);
                        output = GetNearestConnectors(conssss[0], conssss[1]);
                        break;

                }
                return output;
            }
            return null;

        }
        public T ElementToMEP<T>(Element element)
        {

            return (T)Convert.ChangeType(element, typeof(T));
        }
        public List<Connector> GetConnectorsFromElement(Conduit ele)
        {


            ConnectorSet set1 = ele.ConnectorManager.Connectors;
            List<Connector> cons1 = new List<Connector>();

            foreach (Connector con in set1)
            {
                cons1.Add(con);
            }

            return cons1;
        }
        public List<Connector> GetConnectorsFromElement(CableTray ele)
        {


            ConnectorSet set1 = ele.ConnectorManager.Connectors;
            List<Connector> cons1 = new List<Connector>();

            foreach (Connector con in set1)
            {
                cons1.Add(con);
            }

            return cons1;
        }
        public List<Connector> GetConnectorsFromElement(Duct ele)
        {


            ConnectorSet set1 = ele.ConnectorManager.Connectors;
            List<Connector> cons1 = new List<Connector>();

            foreach (Connector con in set1)
            {
                cons1.Add(con);
            }

            return cons1;
        }
        public List<Connector> GetConnectorsFromElement(Pipe ele)
        {


            ConnectorSet set1 = ele.ConnectorManager.Connectors;
            List<Connector> cons1 = new List<Connector>();

            foreach (Connector con in set1)
            {
                cons1.Add(con);
            }

            return cons1;
        }
        public List<List<Connector>> GetConnectorsFromElements(Conduit ele1, Conduit ele2)
        {
            List<List<Connector>> output = new List<List<Connector>>();

            ConnectorSet set1 = ele1.ConnectorManager.Connectors;
            List<Connector> cons1 = new List<Connector>();


            ConnectorSet set2 = ele2.ConnectorManager.Connectors;
            List<Connector> cons2 = new List<Connector>();
            foreach (Connector con in set1)
            {
                cons1.Add(con);
            }
            foreach (Connector con in set2)
            {
                cons2.Add(con);
            }

            output.Add(cons1);
            output.Add(cons2);

            return output;
        }
        public List<List<Connector>> GetConnectorsFromElements(CableTray ele1, CableTray ele2)
        {
            List<List<Connector>> output = new List<List<Connector>>();

            ConnectorSet set1 = ele1.ConnectorManager.Connectors;
            List<Connector> cons1 = new List<Connector>();


            ConnectorSet set2 = ele2.ConnectorManager.Connectors;
            List<Connector> cons2 = new List<Connector>();
            foreach (Connector con in set1)
            {
                cons1.Add(con);
            }
            foreach (Connector con in set2)
            {
                cons2.Add(con);
            }

            output.Add(cons1);
            output.Add(cons2);

            return output;
        }
        public List<List<Connector>> GetConnectorsFromElements(Duct ele1, Duct ele2)
        {
            List<List<Connector>> output = new List<List<Connector>>();

            ConnectorSet set1 = ele1.ConnectorManager.Connectors;
            List<Connector> cons1 = new List<Connector>();


            ConnectorSet set2 = ele2.ConnectorManager.Connectors;
            List<Connector> cons2 = new List<Connector>();
            foreach (Connector con in set1)
            {
                cons1.Add(con);
            }
            foreach (Connector con in set2)
            {
                cons2.Add(con);
            }

            output.Add(cons1);
            output.Add(cons2);

            return output;
        }
        public List<List<Connector>> GetConnectorsFromElements(Pipe ele1, Pipe ele2)
        {
            List<List<Connector>> output = new List<List<Connector>>();

            ConnectorSet set1 = ele1.ConnectorManager.Connectors;
            List<Connector> cons1 = new List<Connector>();


            ConnectorSet set2 = ele2.ConnectorManager.Connectors;
            List<Connector> cons2 = new List<Connector>();
            foreach (Connector con in set1)
            {
                cons1.Add(con);
            }
            foreach (Connector con in set2)
            {
                cons2.Add(con);
            }

            output.Add(cons1);
            output.Add(cons2);

            return output;
        }
        public List<Connector> GetNearestConnectors(List<Connector> cons_a, List<Connector> cons_b)
        {
            List<Connector> cons1 = new List<Connector>();
            List<Connector> cons2 = new List<Connector>();

            foreach (Connector con in cons_a)
            {
                if (!con.IsConnected) cons1.Add(con);
            }
            foreach (Connector con in cons_b)
            {
                if (!con.IsConnected) cons2.Add(con);
            }

            List<Connector> output = new List<Connector>();
            Connector c = null;
            Connector cc = null;

            if (cons1.Count > 0 && cons2.Count > 0)
            {
                double d = 0;
                for (int i = 0; i < cons1.Count; i++)
                {
                    for (int j = 0; j < cons2.Count; j++)
                    {
                        double k = cons1[i].Origin.DistanceTo(cons2[j].Origin);
                        if (i == 0 && j == 0)
                        {
                            c = cons1[0];
                            cc = cons2[0];
                            d = k;
                            continue;
                        }
                        if (cons1[i].Origin.DistanceTo(cons2[j].Origin) < d)
                        {
                            d = k;
                            c = cons1[i];
                            cc = cons2[j];
                        }
                    }
                }

                output.Add(c);
                output.Add(cc);
                return output;

            }
            return null;

        }
        public void DoFailureProcessing(object sender, FailuresProcessingEventArgs args)
        {
            FailuresAccessor failureMes = args.GetFailuresAccessor();

            // Inside event handler, get all warnings

            IList<FailureMessageAccessor> a = failureMes.GetFailureMessages();

            int count = 0;

            foreach (FailureMessageAccessor failure in a)
            {
                FailureSeverity fsav = failureMes.GetSeverity();


                if (fsav == FailureSeverity.Warning)
                {
                    failureMes.DeleteAllWarnings();

                }

                args.SetProcessingResult(FailureProcessingResult.ProceedWithCommit);
            }
        }
    }

}
