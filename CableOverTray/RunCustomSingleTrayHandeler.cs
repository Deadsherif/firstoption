// Decompiled with JetBrains decompiler
// Type: CableOverTray.RunCustomSingleTrayHandeler
// Assembly: COT, Version=2021.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 774E24BD-3C4D-44CA-9437-58E3900DE92B
// Assembly location: C:\Users\ahmed\AppData\Roaming\Autodesk\Revit\Addins\2023\CableOverTray.dll

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;

 
namespace CableOverTray
{
  [Transaction(TransactionMode.Manual)]
  internal class RunCustomSingleTrayHandeler : IExternalEventHandler
  {
    public void Execute(UIApplication app)
    {
      UIDocument activeUiDocument = app.ActiveUIDocument;
      Document document = activeUiDocument.Document;
      Autodesk.Revit.UI.Selection.Selection selection = activeUiDocument.Selection;
      COT_Controller cotController = new COT_Controller();
      int num1 = (int) TaskDialog.Show("Custom Single tray run", "Select custom single cable tray to fill with conduits");
      XYZ xyz1 = new XYZ();
      XYZ xyz2 = new XYZ();
      XYZ sp;
      XYZ ep;
      CableTray e;
      try
      {
        XYZ globalPoint;
        Element element;
        do
        {
          Reference reference = selection.PickObject(ObjectType.Element);
          globalPoint = reference.GlobalPoint;
          element = document.GetElement(reference);
        }
        while (!(element is CableTray));
        Curve curve = (element.Location as LocationCurve).Curve;
        XYZ endPoint1 = curve.GetEndPoint(0);
        XYZ endPoint2 = curve.GetEndPoint(1);
        if (globalPoint.DistanceTo(endPoint1) <= globalPoint.DistanceTo(endPoint2))
        {
          sp = endPoint1;
          ep = endPoint2;
        }
        else
        {
          sp = endPoint2;
          ep = endPoint1;
        }
        e = element as CableTray;
      }
      catch (Exception ex)
      {
        ApplicationStatic_DB.MainForm.Show();
        return;
      }
      CurveWithData curveWithData = new CurveWithData((MEPCurve) e, sp, ep);
      List<CurveWithData> sortedCurves = new List<CurveWithData>();
      sortedCurves.Add(curveWithData);
      using (TransactionGroup transactionGroup = new TransactionGroup(document))
      {
        int num2 = (int) transactionGroup.Start("COT Multi-custom");
        using (Transaction transaction = new Transaction(document, "dummy"))
        {
          int num3 = (int) transaction.Start();
          int num4 = (int) transaction.Commit();
        }
        for (int index = 0; index < ApplicationStatic_DB.cutomConds.Count; ++index)
        {
          Curve curve = (ApplicationStatic_DB.cutomTray.Location as LocationCurve).Curve;
          XYZ endPoint = (ApplicationStatic_DB.cutomConds[index].Location as LocationCurve).Curve.GetEndPoint(0);
          XYZ xyzPoint1 = curve.Project(endPoint).XYZPoint;
          XYZ xyzPoint2 = ApplicationStatic_DB.customReferrenceAnchor.Project(endPoint).XYZPoint;
          double num5 = endPoint.Z - xyzPoint1.Z;
          double num6 = Math.Pow(Math.Pow(xyzPoint2.X - endPoint.X, 2.0) + Math.Pow(endPoint.Y - xyzPoint2.Y, 2.0), 0.5);
          double Y = num5 + ApplicationStatic_DB.cutomTray.Height / 2.0;
          double X = num6;
          cotController.DrawCustomConduits(document, sortedCurves, ApplicationStatic_DB.cutomConds[index].GetTypeId(), e.ReferenceLevel.Id, ApplicationStatic_DB.cutomConds[index].Diameter, X, Y, 0, false, false);
        }
        int num7 = (int) transactionGroup.Assimilate();
      }
      ApplicationStatic_DB.MainForm.Show();
    }

    public string GetName() => "";
  }
}
