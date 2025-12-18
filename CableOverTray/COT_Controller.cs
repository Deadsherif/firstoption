// Decompiled with JetBrains decompiler
// Type: CableOverTray.COT_Controller
// Assembly: COT, Version=2021.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 774E24BD-3C4D-44CA-9437-58E3900DE92B
// Assembly location: C:\Users\ahmed\AppData\Roaming\Autodesk\Revit\Addins\2023\CableOverTray.dll

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;

 
namespace CableOverTray
{
  public class COT_Controller
  {
    public double DrawConduits(
      Document doc,
      List<CurveWithData> sortedCurves,
      ElementId conduitTypeID,
      ElementId levelID,
      double TrayThickness,
      double D_current,
      double D_last,
      double lastOffset,
      bool firstTraySpacingCalculation,
      bool shiftToTrayBottom,
      bool withFittings,
      bool justifyFittings)
    {
      double num1 = lastOffset;
      double offset = 0.0;
      List<Conduit> conduitList = new List<Conduit>();
      bool flag = true;
      foreach (CurveWithData sortedCurve in sortedCurves)
      {
        double r = D_current / 2.0;
        double num2 = TrayThickness;
        double num3 = sortedCurve.curveHost.Height / 2.0;
        if (flag)
        {
          if (D_last == 0.0)
          {
            offset = sortedCurve.curveHost.Width / 2.0 - D_current;
            num1 = offset;
          }
          else
          {
            double num4 = 0.0;
            double num5;
            if (D_current > D_last)
            {
              num5 = D_current;
              num4 = D_last;
            }
            else
            {
              num4 = D_current;
              num5 = D_last;
            }
            offset = lastOffset - D_last / 2.0 - 1.5 * num5;
            num1 = offset;
          }
          flag = false;
        }
        List<XYZ> xyzList1 = new List<XYZ>();
        List<XYZ> xyzList2 = !shiftToTrayBottom ? sortedCurve.GetShiftedCurvePoints(offset) : this.ShiftConduitPointsToTrayBottom(sortedCurve.GetShiftedCurvePoints(offset), num3 - num2, r, sortedCurve.curveHost);
        try
        {
          Conduit conduit = Conduit.Create(doc, conduitTypeID, xyzList2[0], xyzList2[1], levelID);
          conduit.LookupParameter("Diameter(Trade Size)").Set(r * 2.0);
          conduitList.Add(conduit);
        }
        catch (Exception ex)
        {
          conduitList.Add((Conduit) null);
        }
      }
      if (withFittings)
      {
        List<FamilyInstance> familyInstanceList = new List<FamilyInstance>();
        for (int index = 0; index < conduitList.Count - 1; ++index)
        {
          if (conduitList[index] != null && conduitList[index + 1] != null)
          {
            List<Connector> nearestConnector = this.GetMEP_NearestConnector((Element) conduitList[index], (Element) conduitList[index + 1]);
            try
            {
              FamilyInstance familyInstance = doc.Create.NewElbowFitting(nearestConnector[0], nearestConnector[1]);
              CableTray curveHost = sortedCurves[index].curveHost as CableTray;
              double width = curveHost.Width;
              double height = curveHost.Height;
              BoundingBoxXYZ boundingBoxXyz = familyInstance.get_BoundingBox((View) null);
              double num6 = boundingBoxXyz.Max.Z - boundingBoxXyz.Min.Z;
              if (justifyFittings)
              {
                if (num6 == 0.0 || num6 < 0.2)
                  familyInstance.LookupParameter("Bend Radius").Set(width + (width / 2.0 - offset));
                else
                  familyInstance.LookupParameter("Bend Radius").Set(width - height / 2.0);
              }
            }
            catch (Exception ex1)
            {
              try
              {
                FamilyInstance familyInstance = doc.Create.NewUnionFitting(nearestConnector[0], nearestConnector[1]);
                familyInstanceList.Add(familyInstance);
              }
              catch (Exception ex2)
              {
              }
            }
          }
        }
        foreach (Element element in familyInstanceList)
          doc.Delete(element.Id);
      }
      return num1;
    }

    public void DrawCustomConduits(
      Document doc,
      List<CurveWithData> sortedCurves,
      ElementId conduitTypeID,
      ElementId levelID,
      double D_current,
      double X,
      double Y,
      int AnchorPosition,
      bool withFittings,
      bool justifyFittings)
    {
      List<Conduit> conduitList = new List<Conduit>();
      for (int index = 0; index < sortedCurves.Count; ++index)
      {
        CurveWithData sortedCurve = sortedCurves[index];
        double num1 = D_current / 2.0;
        double _h = sortedCurve.curveHost.Height / 2.0;
        List<XYZ> xyzList = new List<XYZ>();
        List<XYZ> trayBottom = this.ShiftConduitPointsToTrayBottom(sortedCurve.GetShiftedCurvePoints(sortedCurve.curveHost.Width / 2.0 - X), _h, Y, sortedCurve.curveHost);
        try
        {
          Transaction transaction = new Transaction(doc);
          int num2 = (int) transaction.Start("drawCond");
          Conduit conduit = Conduit.Create(doc, conduitTypeID, trayBottom[0], trayBottom[1], levelID);
          conduit.LookupParameter("Diameter(Trade Size)").Set(num1 * 2.0);
          int num3 = (int) transaction.Commit();
          conduitList.Add(conduit);
        }
        catch (Exception ex)
        {
          conduitList.Add((Conduit) null);
        }
      }
      if (!withFittings)
        return;
      List<Element> elementList = new List<Element>();
      List<FamilyInstance> familyInstanceList = new List<FamilyInstance>();
      for (int index = 0; index < conduitList.Count - 1; ++index)
      {
        try
        {
          if (conduitList[index] != null && conduitList[index + 1] != null)
          {
            List<Connector> nearestConnector = this.GetMEP_NearestConnector((Element) conduitList[index], (Element) conduitList[index + 1]);
            XYZ origin1 = nearestConnector[0].Origin;
            XYZ origin2 = nearestConnector[1].Origin;
            Curve curve;
            try
            {
              curve = (sortedCurves[index].curveHost.Location as LocationCurve).Curve;
            }
            catch (Exception ex)
            {
              continue;
            }
            XYZ xyz1 = new XYZ();
            XYZ xyz2 = new XYZ();
            XYZ endPoint1;
            XYZ endPoint2;
            if (origin2.DistanceTo(curve.GetEndPoint(0)) <= origin2.DistanceTo(curve.GetEndPoint(1)))
            {
              endPoint1 = curve.GetEndPoint(1);
              endPoint2 = curve.GetEndPoint(0);
            }
            else
            {
              endPoint1 = curve.GetEndPoint(0);
              endPoint2 = curve.GetEndPoint(1);
            }
            List<XYZ> shiftedCurvePoints = new CurveWithData(sortedCurves[index].curveHost, endPoint1, endPoint2).GetShiftedCurvePoints(sortedCurves[index].curveHost.Width / 2.0);
            XYZ xyz3 = new XYZ();
            XYZ source1 = shiftedCurvePoints[0].DistanceTo(origin1) > shiftedCurvePoints[1].DistanceTo(origin1) ? shiftedCurvePoints[1] : shiftedCurvePoints[0];
            XYZ xyz4 = new XYZ();
            XYZ source2 = curve.GetEndPoint(0).DistanceTo(origin1) > curve.GetEndPoint(1).DistanceTo(origin1) ? curve.GetEndPoint(1) : curve.GetEndPoint(0);
            Transaction transaction1 = new Transaction(doc);
            int num4 = (int) transaction1.Start("drawfittings");
            FamilyInstance familyInstance;
            try
            {
              familyInstance = doc.Create.NewElbowFitting(nearestConnector[0], nearestConnector[1]);
              int num5 = (int) transaction1.Commit();
            }
            catch (Exception ex1)
            {
              try
              {
                familyInstance = doc.Create.NewUnionFitting(nearestConnector[0], nearestConnector[1]);
                int num6 = (int) transaction1.Commit();
                elementList.Add((Element) familyInstance);
              }
              catch (Exception ex2)
              {
                int num7 = (int) transaction1.RollBack();
                continue;
              }
            }
            CableTray curveHost = sortedCurves[index].curveHost as CableTray;
            double width = curveHost.Width;
            double height = curveHost.Height;
            BoundingBoxXYZ boundingBoxXyz = familyInstance.get_BoundingBox((View) null);
            double num8 = boundingBoxXyz.Max.Z - boundingBoxXyz.Min.Z;
            double betweenTwoElements = this.GetCommonTrayElbowBendRadiusBetweenTwoElements(doc, (Element) conduitList[index], (Element) conduitList[index + 1]);
            if (justifyFittings && betweenTwoElements != -1.0)
            {
              if (num8 == 0.0 || num8 < 0.2)
              {
                if (origin2.DistanceTo(source1) <= origin2.DistanceTo(source2))
                {
                  Transaction transaction2 = new Transaction(doc);
                  int num9 = (int) transaction2.Start("justifyFitting_LEFT");
                  try
                  {
                    familyInstance.LookupParameter("Bend Radius").Set(betweenTwoElements + X);
                    int num10 = (int) transaction2.Commit();
                  }
                  catch (Exception ex)
                  {
                    int num11 = (int) transaction2.RollBack();
                  }
                }
                else
                {
                  Transaction transaction3 = new Transaction(doc);
                  int num12 = (int) transaction3.Start("justifyFitting_RIGHT");
                  try
                  {
                    familyInstance.LookupParameter("Bend Radius").Set(betweenTwoElements + width - X);
                    int num13 = (int) transaction3.Commit();
                  }
                  catch (Exception ex)
                  {
                    int num14 = (int) transaction3.RollBack();
                  }
                }
              }
              else if ((origin1.Z < origin2.Z ? "y" : "x") == (Math.Abs((conduitList[index].Location as LocationCurve).Curve.GetEndPoint(0).Z - (conduitList[index].Location as LocationCurve).Curve.GetEndPoint(1).Z) < Math.Abs((conduitList[index + 1].Location as LocationCurve).Curve.GetEndPoint(0).Z - (conduitList[index + 1].Location as LocationCurve).Curve.GetEndPoint(1).Z) ? "y" : "x"))
              {
                Transaction transaction4 = new Transaction(doc);
                int num15 = (int) transaction4.Start("justifyFitting_VERT");
                try
                {
                  familyInstance.LookupParameter("Bend Radius").Set(betweenTwoElements + (height - Y));
                  int num16 = (int) transaction4.Commit();
                }
                catch (Exception ex)
                {
                  int num17 = (int) transaction4.RollBack();
                }
              }
              else
              {
                Transaction transaction5 = new Transaction(doc);
                int num18 = (int) transaction5.Start("justifyFitting_VERT");
                try
                {
                  familyInstance.LookupParameter("Bend Radius").Set(betweenTwoElements + Y);
                  int num19 = (int) transaction5.Commit();
                }
                catch (Exception ex)
                {
                  int num20 = (int) transaction5.RollBack();
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
      Transaction transaction6 = new Transaction(doc);
      int num21 = (int) transaction6.Start("_");
      foreach (Element element in elementList)
      {
        try
        {
          doc.Delete(element.Id);
        }
        catch (Exception ex)
        {
        }
      }
      int num22 = (int) transaction6.Commit();
    }

    public double GetCommonTrayElbowBendRadiusBetweenTwoElements(
      Document doc,
      Element e1,
      Element e2)
    {
      IList<Element> elements1 = new FilteredElementCollector(doc).WherePasses((ElementFilter) new ElementIntersectsElementFilter(e1)).ToElements();
      List<Element> elementList = new List<Element>();
      foreach (Element element in (IEnumerable<Element>) elements1)
      {
        if (element.Category.Name == "Cable Tray Fittings" && element.LookupParameter("Bend Radius") != null)
          elementList.Add(element);
      }
      if (elementList.Count == 0)
        return -1.0;
      IList<Element> elements2 = new FilteredElementCollector(doc).WherePasses((ElementFilter) new ElementIntersectsElementFilter(e2)).ToElements();
      foreach (Element element1 in elementList)
      {
        foreach (Element element2 in (IEnumerable<Element>) elements2)
        {
          if (element1.Id.IntegerValue == element2.Id.IntegerValue)
            return element1.LookupParameter("Bend Radius").AsDouble();
        }
      }
      return -1.0;
    }

    public void DrawCustomConduitsDoubleElbow(
      Document doc,
      List<CurveWithData> sortedCurves,
      ElementId conduitTypeID,
      ElementId levelID,
      double D_current,
      double X,
      double Y,
      int AnchorPosition,
      bool withFittings,
      bool justifyFittings)
    {
      List<Conduit> conduitList = new List<Conduit>();
      for (int index = 0; index < sortedCurves.Count; ++index)
      {
        CurveWithData sortedCurve = sortedCurves[index];
        CurveWithData curveWithData = (CurveWithData) null;
        if (index + 1 < sortedCurves.Count)
          curveWithData = sortedCurves[index + 1];
        double num1 = D_current / 2.0;
        double _h = sortedCurve.curveHost.Height / 2.0;
        List<XYZ> xyzList1 = new List<XYZ>();
        List<XYZ> trayBottom = this.ShiftConduitPointsToTrayBottom(sortedCurve.GetShiftedCurvePoints(sortedCurve.curveHost.Width / 2.0 - X), _h, Y, sortedCurve.curveHost);
        List<XYZ> xyzList2 = new List<XYZ>();
        if (curveWithData != null)
          xyzList2 = this.ShiftConduitPointsToTrayBottom(curveWithData.GetShiftedCurvePoints(curveWithData.curveHost.Width / 2.0 - X), _h, Y, curveWithData.curveHost);
        try
        {
          Transaction transaction1 = new Transaction(doc);
          int num2 = (int) transaction1.Start("drawCond");
          Conduit conduit1 = Conduit.Create(doc, conduitTypeID, trayBottom[0], trayBottom[1], levelID);
          conduit1.LookupParameter("Diameter(Trade Size)").Set(num1 * 2.0);
          int num3 = (int) transaction1.Commit();
          conduitList.Add(conduit1);
          if (curveWithData != null)
          {
            Transaction transaction2 = new Transaction(doc);
            int num4 = (int) transaction2.Start("drawCond");
            Conduit conduit2 = Conduit.Create(doc, conduitTypeID, trayBottom[1], xyzList2[0], levelID);
            conduit2.LookupParameter("Diameter(Trade Size)").Set(num1 * 2.0);
            int num5 = (int) transaction2.Commit();
            conduitList.Add(conduit2);
          }
        }
        catch (Exception ex)
        {
          conduitList.Add((Conduit) null);
        }
      }
      if (!withFittings)
        return;
      List<FamilyInstance> familyInstanceList = new List<FamilyInstance>();
      for (int index = 0; index < conduitList.Count - 1; ++index)
      {
        if (conduitList[index] != null && conduitList[index + 1] != null)
        {
          List<Connector> nearestConnector = this.GetMEP_NearestConnector((Element) conduitList[index], (Element) conduitList[index + 1]);
          Transaction transaction3 = new Transaction(doc);
          int num6 = (int) transaction3.Start("drawfittings");
          FamilyInstance familyInstance;
          try
          {
            familyInstance = doc.Create.NewElbowFitting(nearestConnector[0], nearestConnector[1]);
            int num7 = (int) transaction3.Commit();
          }
          catch (Exception ex)
          {
            int num8 = (int) transaction3.RollBack();
            continue;
          }
          if (justifyFittings)
          {
            Transaction transaction4 = new Transaction(doc);
            int num9 = (int) transaction4.Start("justifyFitting_LEFT");
            try
            {
              familyInstance.LookupParameter("Bend Radius").Set(0.49212598);
              int num10 = (int) transaction4.Commit();
            }
            catch (Exception ex)
            {
              int num11 = (int) transaction4.RollBack();
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
      List<XYZ> xyzList = new List<XYZ>();
      foreach (XYZ endPoint in endPoints)
        xyzList.Add(new XYZ(endPoint.X, endPoint.Y, endPoint.Z - _h + r));
      return xyzList;
    }

    public List<XYZ> ShiftConduitPointsToTrayBottom(
      List<XYZ> endPoints,
      double _h,
      double r,
      MEPCurve curveHost)
    {
      CableTray cableTray = curveHost as CableTray;
      List<XYZ> trayBottom = new List<XYZ>();
      XYZ xyz1 = cableTray.CurveNormal.Negate();
      XYZ source1 = new XYZ(1.0, 0.0, 0.0);
      XYZ source2 = new XYZ(0.0, 1.0, 0.0);
      XYZ source3 = new XYZ(0.0, 0.0, 1.0);
      double d1 = xyz1.AngleTo(source1);
      double d2 = xyz1.AngleTo(source2);
      double d3 = xyz1.AngleTo(source3);
      XYZ xyz2 = new XYZ(endPoints[0].X + (_h - r) * Math.Cos(d1), endPoints[0].Y + (_h - r) * Math.Cos(d2), endPoints[0].Z + (_h - r) * Math.Cos(d3));
      XYZ xyz3 = new XYZ(endPoints[1].X + (_h - r) * Math.Cos(d1), endPoints[1].Y + (_h - r) * Math.Cos(d2), endPoints[1].Z + (_h - r) * Math.Cos(d3));
      trayBottom.Add(xyz2);
      trayBottom.Add(xyz3);
      return trayBottom;
    }

    public List<CurveWithData> GetSortedCurves(List<CableTray> sortedTrays)
    {
      List<CurveWithData> sortedCurves = new List<CurveWithData>();
      XYZ xyz1 = new XYZ();
      XYZ xyz2 = new XYZ();
      for (int index = 0; index < sortedTrays.Count; ++index)
      {
        if (index == 0 && sortedTrays.Count > 1)
        {
          LocationCurve location1 = sortedTrays[0].Location as LocationCurve;
          LocationCurve location2 = sortedTrays[1].Location as LocationCurve;
          XYZ endPoint1 = location1.Curve.GetEndPoint(0);
          XYZ endPoint2 = location1.Curve.GetEndPoint(1);
          XYZ endPoint3 = location2.Curve.GetEndPoint(0);
          if (endPoint1.DistanceTo(endPoint3) < endPoint2.DistanceTo(endPoint3))
          {
            XYZ sp = endPoint2;
            xyz2 = endPoint1;
            sortedCurves.Add(new CurveWithData((MEPCurve) sortedTrays[0], sp, xyz2));
          }
          else
          {
            XYZ sp = endPoint1;
            xyz2 = endPoint2;
            sortedCurves.Add(new CurveWithData((MEPCurve) sortedTrays[0], sp, xyz2));
          }
        }
        else
        {
          LocationCurve location = sortedTrays[index].Location as LocationCurve;
          XYZ endPoint4 = location.Curve.GetEndPoint(0);
          XYZ endPoint5 = location.Curve.GetEndPoint(1);
          if (endPoint4.DistanceTo(xyz2) < endPoint5.DistanceTo(xyz2))
          {
            XYZ sp = endPoint4;
            xyz2 = endPoint5;
            sortedCurves.Add(new CurveWithData((MEPCurve) sortedTrays[index], sp, xyz2));
          }
          else
          {
            XYZ sp = endPoint5;
            xyz2 = endPoint4;
            sortedCurves.Add(new CurveWithData((MEPCurve) sortedTrays[index], sp, xyz2));
          }
        }
      }
      return sortedCurves;
    }

    public List<CableTray> SelectSortedTrays(Autodesk.Revit.UI.Selection.Selection sel, UIDocument uidoc, Document doc)
    {
      List<CableTray> cableTrayList = new List<CableTray>();
      List<ElementId> elementIdList = new List<ElementId>();
      while (true)
      {
        sel.SetElementIds((ICollection<ElementId>) elementIdList);
        uidoc.RefreshActiveView();
        try
        {
          Element element = doc.GetElement(sel.PickObject(ObjectType.Element, (ISelectionFilter) new TraysSelectionFilter(), string.Format("Selected CableTrays: {0} Trays", (object) cableTrayList.Count)));
          int index = 0;
          bool flag = false;
          foreach (ElementId elementId in elementIdList)
          {
            if (elementId == element.Id)
            {
              cableTrayList.RemoveAt(index);
              elementIdList.RemoveAt(index);
              sel.SetElementIds((ICollection<ElementId>) elementIdList);
              uidoc.RefreshActiveView();
              flag = true;
              break;
            }
            ++index;
          }
          if (element is CableTray && !flag)
          {
            cableTrayList.Add(element as CableTray);
            elementIdList.Add(element.Id);
            sel.SetElementIds((ICollection<ElementId>) elementIdList);
            uidoc.RefreshActiveView();
          }
        }
        catch (Exception ex)
        {
          break;
        }
      }
      sel.SetElementIds((ICollection<ElementId>) elementIdList);
      return cableTrayList;
    }

    public List<Connector> GetMEP_NearestConnector(Element ele1, Element ele2)
    {
      if (!(ele1.Category.Name == ele2.Category.Name))
        return (List<Connector>) null;
      List<Connector> nearestConnector = new List<Connector>();
      switch (ele1.Category.Name)
      {
        case "Cable Trays":
          List<List<Connector>> connectorsFromElements1 = this.GetConnectorsFromElements(this.ElementToMEP<CableTray>(ele1), this.ElementToMEP<CableTray>(ele2));
          nearestConnector = this.GetNearestConnectors(connectorsFromElements1[0], connectorsFromElements1[1]);
          break;
        case "Conduits":
          List<List<Connector>> connectorsFromElements2 = this.GetConnectorsFromElements(this.ElementToMEP<Conduit>(ele1), this.ElementToMEP<Conduit>(ele2));
          nearestConnector = this.GetNearestConnectors(connectorsFromElements2[0], connectorsFromElements2[1]);
          break;
        case "Pipes":
          List<List<Connector>> connectorsFromElements3 = this.GetConnectorsFromElements(this.ElementToMEP<Pipe>(ele1), this.ElementToMEP<Pipe>(ele2));
          nearestConnector = this.GetNearestConnectors(connectorsFromElements3[0], connectorsFromElements3[1]);
          break;
        case "Ducts":
          List<List<Connector>> connectorsFromElements4 = this.GetConnectorsFromElements(this.ElementToMEP<Duct>(ele1), this.ElementToMEP<Duct>(ele2));
          nearestConnector = this.GetNearestConnectors(connectorsFromElements4[0], connectorsFromElements4[1]);
          break;
      }
      return nearestConnector;
    }

    public T ElementToMEP<T>(Element element)
    {
      return (T) Convert.ChangeType((object) element, typeof (T));
    }

    public List<Connector> GetConnectorsFromElement(Conduit ele)
    {
      ConnectorSet connectors = ele.ConnectorManager.Connectors;
      List<Connector> connectorsFromElement = new List<Connector>();
      foreach (Connector connector in connectors)
        connectorsFromElement.Add(connector);
      return connectorsFromElement;
    }

    public List<Connector> GetConnectorsFromElement(CableTray ele)
    {
      ConnectorSet connectors = ele.ConnectorManager.Connectors;
      List<Connector> connectorsFromElement = new List<Connector>();
      foreach (Connector connector in connectors)
        connectorsFromElement.Add(connector);
      return connectorsFromElement;
    }

    public List<Connector> GetConnectorsFromElement(Duct ele)
    {
      ConnectorSet connectors = ele.ConnectorManager.Connectors;
      List<Connector> connectorsFromElement = new List<Connector>();
      foreach (Connector connector in connectors)
        connectorsFromElement.Add(connector);
      return connectorsFromElement;
    }

    public List<Connector> GetConnectorsFromElement(Pipe ele)
    {
      ConnectorSet connectors = ele.ConnectorManager.Connectors;
      List<Connector> connectorsFromElement = new List<Connector>();
      foreach (Connector connector in connectors)
        connectorsFromElement.Add(connector);
      return connectorsFromElement;
    }

    public List<List<Connector>> GetConnectorsFromElements(Conduit ele1, Conduit ele2)
    {
      List<List<Connector>> connectorsFromElements = new List<List<Connector>>();
      ConnectorSet connectors1 = ele1.ConnectorManager.Connectors;
      List<Connector> connectorList1 = new List<Connector>();
      ConnectorSet connectors2 = ele2.ConnectorManager.Connectors;
      List<Connector> connectorList2 = new List<Connector>();
      foreach (Connector connector in connectors1)
        connectorList1.Add(connector);
      foreach (Connector connector in connectors2)
        connectorList2.Add(connector);
      connectorsFromElements.Add(connectorList1);
      connectorsFromElements.Add(connectorList2);
      return connectorsFromElements;
    }

    public List<List<Connector>> GetConnectorsFromElements(CableTray ele1, CableTray ele2)
    {
      List<List<Connector>> connectorsFromElements = new List<List<Connector>>();
      ConnectorSet connectors1 = ele1.ConnectorManager.Connectors;
      List<Connector> connectorList1 = new List<Connector>();
      ConnectorSet connectors2 = ele2.ConnectorManager.Connectors;
      List<Connector> connectorList2 = new List<Connector>();
      foreach (Connector connector in connectors1)
        connectorList1.Add(connector);
      foreach (Connector connector in connectors2)
        connectorList2.Add(connector);
      connectorsFromElements.Add(connectorList1);
      connectorsFromElements.Add(connectorList2);
      return connectorsFromElements;
    }

    public List<List<Connector>> GetConnectorsFromElements(Duct ele1, Duct ele2)
    {
      List<List<Connector>> connectorsFromElements = new List<List<Connector>>();
      ConnectorSet connectors1 = ele1.ConnectorManager.Connectors;
      List<Connector> connectorList1 = new List<Connector>();
      ConnectorSet connectors2 = ele2.ConnectorManager.Connectors;
      List<Connector> connectorList2 = new List<Connector>();
      foreach (Connector connector in connectors1)
        connectorList1.Add(connector);
      foreach (Connector connector in connectors2)
        connectorList2.Add(connector);
      connectorsFromElements.Add(connectorList1);
      connectorsFromElements.Add(connectorList2);
      return connectorsFromElements;
    }

    public List<List<Connector>> GetConnectorsFromElements(Pipe ele1, Pipe ele2)
    {
      List<List<Connector>> connectorsFromElements = new List<List<Connector>>();
      ConnectorSet connectors1 = ele1.ConnectorManager.Connectors;
      List<Connector> connectorList1 = new List<Connector>();
      ConnectorSet connectors2 = ele2.ConnectorManager.Connectors;
      List<Connector> connectorList2 = new List<Connector>();
      foreach (Connector connector in connectors1)
        connectorList1.Add(connector);
      foreach (Connector connector in connectors2)
        connectorList2.Add(connector);
      connectorsFromElements.Add(connectorList1);
      connectorsFromElements.Add(connectorList2);
      return connectorsFromElements;
    }

    public List<Connector> GetNearestConnectors(List<Connector> cons_a, List<Connector> cons_b)
    {
      List<Connector> connectorList1 = new List<Connector>();
      List<Connector> connectorList2 = new List<Connector>();
      foreach (Connector connector in cons_a)
      {
        if (!connector.IsConnected)
          connectorList1.Add(connector);
      }
      foreach (Connector connector in cons_b)
      {
        if (!connector.IsConnected)
          connectorList2.Add(connector);
      }
      List<Connector> nearestConnectors = new List<Connector>();
      Connector connector1 = (Connector) null;
      Connector connector2 = (Connector) null;
      if (connectorList1.Count <= 0 || connectorList2.Count <= 0)
        return (List<Connector>) null;
      double num1 = 0.0;
      for (int index1 = 0; index1 < connectorList1.Count; ++index1)
      {
        for (int index2 = 0; index2 < connectorList2.Count; ++index2)
        {
          double num2 = connectorList1[index1].Origin.DistanceTo(connectorList2[index2].Origin);
          if (index1 == 0 && index2 == 0)
          {
            connector1 = connectorList1[0];
            connector2 = connectorList2[0];
            num1 = num2;
          }
          else if (connectorList1[index1].Origin.DistanceTo(connectorList2[index2].Origin) < num1)
          {
            num1 = num2;
            connector1 = connectorList1[index1];
            connector2 = connectorList2[index2];
          }
        }
      }
      nearestConnectors.Add(connector1);
      nearestConnectors.Add(connector2);
      return nearestConnectors;
    }
  }
}
