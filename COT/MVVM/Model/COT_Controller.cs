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

namespace COT.MVVM.Model
{
    public class COT_Controller
    {

        public double DrawConduits(Document doc, List<CurveWithData> sortedCurves, ElementId conduitTypeID, ElementId levelID, double TrayThickness, double D_current, double D_last, double lastOffset, bool firstTraySpacingCalculation, bool shiftToTrayBottom, bool withFittings, bool justifyFittings)
        {
            double output = lastOffset;
            double offset = 0;
            List<Conduit> conds = new List<Conduit>();

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
                    //////////////////////////////////////////////////////////////////////////////////////////
                    Conduit cd = Conduit.Create(doc, conduitTypeID, points[0], points[1], levelID);
                    doc.Regenerate();
                    cd.LookupParameter("Diameter(Trade Size)").Set(r * 2);
                    conds.Add(cd);
                    ApplicationStatic_DB.points.Add(points[0]);
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
        }

        public void DrawCustomConduits(Document doc, List<CurveWithData> sortedCurves, ElementId conduitTypeID, ElementId levelID, double D_current, double X, double Y, int AnchorPosition, bool withFittings, bool justifyFittings)
        {
            List<Conduit> conds = new List<Conduit>();
            for (int i = 0; i < sortedCurves.Count; i++)
            //foreach (CurveWithData curveData in sortedCurves)
            {
                CurveWithData curveData = sortedCurves[i];
                double D = D_current;//From_User
                double r = D / 2;

                double _h = (curveData.curveHost.Height / 2);


                List<XYZ> points = new List<XYZ>();
                points = ShiftConduitPointsToTrayBottom(curveData.GetShiftedCurvePoints((curveData.curveHost.Width / 2) - X), _h, Y, curveData.curveHost);


                try
                {
                    Transaction tr1 = new Transaction(doc);
                    tr1.Start("drawCond");

                    Conduit cd = Conduit.Create(doc, conduitTypeID, points[0], points[1], levelID);
                    cd.LookupParameter("Diameter(Trade Size)").Set(r * 2);

                    tr1.Commit();
                    conds.Add(cd);
                }
                catch (Exception e)
                {

                    conds.Add(null);
                }

            }
            //doc.Regenerate();

            if (withFittings)
            {
                List<Element> unionsToBeDeleted = new List<Element>();

                List<FamilyInstance> ds = new List<FamilyInstance>();
                for (int i = 0; i < conds.Count - 1; i++)
                {
                    try
                    {
                        if (conds[i] != null && conds[i + 1] != null)
                        {
                            Element _ele1 = conds[i];
                            Element _ele2 = conds[i + 1];
                            List<Connector> _ccc = GetMEP_NearestConnector(_ele1, _ele2);
                            XYZ P1 = _ccc[0].Origin;
                            XYZ P2 = _ccc[1].Origin;
                            Curve trayCurve = null;
                            try
                            {
                                trayCurve = (sortedCurves[i].curveHost.Location as LocationCurve).Curve;
                            }
                            catch (Exception)
                            {
                                continue;
                            }

                            XYZ tray_startPoint = new XYZ();
                            XYZ tray_endPoint = new XYZ();
                            if (P2.DistanceTo(trayCurve.GetEndPoint(0)) <= P2.DistanceTo(trayCurve.GetEndPoint(1)))
                            {
                                tray_startPoint = trayCurve.GetEndPoint(1);
                                tray_endPoint = trayCurve.GetEndPoint(0);
                            }
                            else
                            {
                                tray_startPoint = trayCurve.GetEndPoint(0);
                                tray_endPoint = trayCurve.GetEndPoint(1);
                            }
                            CurveWithData tray = new CurveWithData(sortedCurves[i].curveHost, tray_startPoint, tray_endPoint);
                            List<XYZ> leftPoints = tray.GetShiftedCurvePoints(sortedCurves[i].curveHost.Width / 2);
                            XYZ Pt = new XYZ();
                            if (leftPoints[0].DistanceTo(P1) <= leftPoints[1].DistanceTo(P1))
                            {
                                Pt = leftPoints[0];
                            }
                            else
                            {
                                Pt = leftPoints[1];
                            }
                            XYZ PtCenter = new XYZ();
                            if (trayCurve.GetEndPoint(0).DistanceTo(P1) <= trayCurve.GetEndPoint(1).DistanceTo(P1))
                            {
                                PtCenter = trayCurve.GetEndPoint(0);
                            }
                            else
                            {
                                PtCenter = trayCurve.GetEndPoint(1);
                            }




                            Transaction tr2 = new Transaction(doc);
                            tr2.Start("drawfittings");
                            FamilyInstance ss = null;
                            try
                            {
                                ss = doc.Create.NewElbowFitting(_ccc[0], _ccc[1]);
                                tr2.Commit();

                            }
                            catch (Exception e)
                            {
                                try
                                {
                                    ss = doc.Create.NewUnionFitting(_ccc[0], _ccc[1]);
                                    tr2.Commit();
                                    unionsToBeDeleted.Add(ss);
                                }
                                catch (Exception ex)
                                {

                                    tr2.RollBack();
                                    continue;

                                }

                            }
                            //sortedCurves[i]

                            CableTray ct = sortedCurves[i].curveHost as CableTray;
                            double w = ct.Width;
                            double h = ct.Height;

                            BoundingBoxXYZ bx = ss.get_BoundingBox(null);
                            double dz = bx.Max.Z - bx.Min.Z;
                            //Cable Tray Fittings

                            double BR = GetCommonTrayElbowBendRadiusBetweenTwoElements(doc, conds[i], conds[i + 1]);
                            if (justifyFittings && BR != -1)
                            {
                                if (dz == 0 || dz < 0.2)
                                {
                                    //horizontal fitting
                                    if (P2.DistanceTo(Pt) <= P2.DistanceTo(PtCenter))
                                    {
                                        //left turn
                                        Transaction tr3 = new Transaction(doc);
                                        tr3.Start("justifyFitting_LEFT");

                                        try
                                        {

                                            ss.LookupParameter("Bend Radius").Set(BR + X);
                                            tr3.Commit();
                                        }
                                        catch (Exception)
                                        {

                                            tr3.RollBack();
                                        }
                                    }
                                    else
                                    {
                                        //right turn
                                        Transaction tr3 = new Transaction(doc);
                                        tr3.Start("justifyFitting_RIGHT");
                                        try
                                        {

                                            ss.LookupParameter("Bend Radius").Set(BR + w - X);
                                            tr3.Commit();
                                        }
                                        catch (Exception)
                                        {

                                            tr3.RollBack();
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    //vertical fitting
                                    string dH = "";
                                    string ddH = "";

                                    if (P1.Z >= P2.Z)
                                    {
                                        dH = "x";
                                    }
                                    else
                                    {
                                        dH = "y";
                                    }
                                    double ddx = Math.Abs(((conds[i].Location as LocationCurve).Curve.GetEndPoint(0).Z) - ((conds[i].Location as LocationCurve).Curve.GetEndPoint(1).Z));
                                    double ddy = Math.Abs(((conds[i + 1].Location as LocationCurve).Curve.GetEndPoint(0).Z) - ((conds[i + 1].Location as LocationCurve).Curve.GetEndPoint(1).Z));

                                    if (ddx >= ddy)
                                    {
                                        ddH = "x";
                                    }
                                    else
                                    {
                                        ddH = "y";
                                    }


                                    if (dH == ddH)
                                    {
                                        Transaction tr3 = new Transaction(doc);
                                        tr3.Start("justifyFitting_VERT");
                                        try
                                        {

                                            //up turn
                                            ss.LookupParameter("Bend Radius").Set(BR + (h - (Y)));
                                            tr3.Commit();
                                        }
                                        catch (Exception)
                                        {
                                            tr3.RollBack();
                                            continue;

                                        }
                                    }
                                    else
                                    {
                                        Transaction tr3 = new Transaction(doc);
                                        tr3.Start("justifyFitting_VERT");
                                        try
                                        {

                                            //down turn
                                            ss.LookupParameter("Bend Radius").Set(BR + ((Y)));
                                            tr3.Commit();
                                        }
                                        catch (Exception)
                                        {
                                            tr3.RollBack();
                                            continue;

                                        }

                                    }




                                }

                            }



                        }
                    }
                    catch (Exception e)
                    {
                        continue;

                    }
                }
                //foreach (Element item in ds)
                //{
                //    doc.Delete(item.Id);
                //}
                Transaction deletingTheUnion = new Transaction(doc);
                deletingTheUnion.Start("_");
                foreach (Element e in unionsToBeDeleted)
                {
                    try
                    {
                        doc.Delete(e.Id);

                    }
                    catch (Exception)
                    {

                        continue;
                    }
                }
                deletingTheUnion.Commit();

            }

        }

        public double GetCommonTrayElbowBendRadiusBetweenTwoElements(Document doc, Element e1, Element e2)
        {
            FilteredElementCollector coll1 = new FilteredElementCollector(doc);
            ElementIntersectsElementFilter f1 = new ElementIntersectsElementFilter(e1);
            IList<Element> list1 = coll1.WherePasses(f1).ToElements();
            List<Element> trayFittings1 = new List<Element>();
            foreach (Element ele in list1)
            {
                if (ele.Category.Name == "Cable Tray Fittings")
                {
                    Parameter p = ele.LookupParameter("Bend Radius");
                    if (p != null)
                    {
                        trayFittings1.Add(ele);
                    }
                }
            }
            if (trayFittings1.Count == 0)
            {
                return -1;

            }


            FilteredElementCollector coll2 = new FilteredElementCollector(doc);
            ElementIntersectsElementFilter f2 = new ElementIntersectsElementFilter(e2);
            IList<Element> list2 = coll2.WherePasses(f2).ToElements();
            foreach (Element ele1 in trayFittings1)
            {
                foreach (Element ele2 in list2)
                {
                    if (ele1.Id.IntegerValue == ele2.Id.IntegerValue)
                    {
                        return ele1.LookupParameter("Bend Radius").AsDouble();
                    }
                }
            }
            return -1;




        }
        public void DrawCustomConduitsDoubleElbow(Document doc, List<CurveWithData> sortedCurves, ElementId conduitTypeID, ElementId levelID, double D_current, double X, double Y, int AnchorPosition, bool withFittings, bool justifyFittings)
        {
            List<Conduit> conds = new List<Conduit>();
            for (int i = 0; i < sortedCurves.Count; i++)
            //foreach (CurveWithData curveData in sortedCurves)
            {
                CurveWithData curveData = sortedCurves[i];
                CurveWithData nextCurveData = null;
                if (i + 1 < sortedCurves.Count)
                {
                    nextCurveData = sortedCurves[i + 1];
                }
                double D = D_current;//From_User
                double r = D / 2;

                double _h = (curveData.curveHost.Height / 2);


                List<XYZ> points = new List<XYZ>();
                points = ShiftConduitPointsToTrayBottom(curveData.GetShiftedCurvePoints((curveData.curveHost.Width / 2) - X), _h, Y, curveData.curveHost);

                //points of the next tray
                List<XYZ> nextpoints = new List<XYZ>();
                if (nextCurveData != null)
                {

                    nextpoints = ShiftConduitPointsToTrayBottom(nextCurveData.GetShiftedCurvePoints((nextCurveData.curveHost.Width / 2) - X), _h, Y, nextCurveData.curveHost);

                }

                try
                {
                    Transaction tr1 = new Transaction(doc);
                    tr1.Start("drawCond");

                    Conduit cd = Conduit.Create(doc, conduitTypeID, points[0], points[1], levelID);
                    cd.LookupParameter("Diameter(Trade Size)").Set(r * 2);

                    tr1.Commit();
                    conds.Add(cd);
                    if (nextCurveData != null)
                    {

                        Transaction tr22 = new Transaction(doc);
                        tr22.Start("drawCond");

                        Conduit cd2 = Conduit.Create(doc, conduitTypeID, points[1], nextpoints[0], levelID);
                        cd2.LookupParameter("Diameter(Trade Size)").Set(r * 2);

                        tr22.Commit();
                        conds.Add(cd2);

                    }
                }
                catch (Exception e)
                {

                    conds.Add(null);
                }

            }
            //doc.Regenerate();

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

                        Transaction tr2 = new Transaction(doc);
                        tr2.Start("drawfittings");
                        FamilyInstance ss = null;
                        try
                        {

                            ss = doc.Create.NewElbowFitting(_ccc[0], _ccc[1]);
                            tr2.Commit();
                        }
                        catch (Exception)
                        {

                            tr2.RollBack();
                            continue;
                        }


                        if (justifyFittings)
                        {

                            Transaction tr3 = new Transaction(doc);
                            tr3.Start("justifyFitting_LEFT");

                            try
                            {

                                ss.LookupParameter("Bend Radius").Set(0.49212598);
                                tr3.Commit();

                            }
                            catch (Exception)
                            {

                                tr3.RollBack();
                                continue;
                            }
                        }



                    }
                }

            }

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
    }
}
