// Decompiled with JetBrains decompiler
// Type: CableOverTray.RunSingleTrayHandeler
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
  internal class RunSingleTrayHandeler : IExternalEventHandler
  {
    public void Execute(UIApplication app)
    {
      UIDocument activeUiDocument = app.ActiveUIDocument;
      Document document = activeUiDocument.Document;
      Autodesk.Revit.UI.Selection.Selection selection = activeUiDocument.Selection;
      COT_Controller cotController = new COT_Controller();
      int num1 = (int) TaskDialog.Show("Single tray run", "Select single cable tray to fill with conduits");
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
      foreach (Element conduitType in ApplicationStatic_DB.conduitTypes)
      {
        if (conduitType.Name == ApplicationStatic_DB.conduitTypeName)
        {
          ApplicationStatic_DB.conduitType = conduitType;
          break;
        }
      }
      if (ApplicationStatic_DB.conduitType == null || e == null)
      {
        ApplicationStatic_DB.MainForm.Show();
      }
      else
      {
        CurveWithData curveWithData = new CurveWithData((MEPCurve) e, sp, ep);
        List<CurveWithData> sortedCurves = new List<CurveWithData>();
        sortedCurves.Add(curveWithData);
        Transaction transaction = new Transaction(document);
        int num2 = (int) transaction.Start("COT");
        double lastOffset = 0.0;
        double D_last = 0.0;
        for (int index = 0; index < ApplicationStatic_DB.ConduitsData.Count; ++index)
        {
          double D_current = ApplicationStatic_DB.ConduitsData[index];
          double num3 = cotController.DrawConduits(document, sortedCurves, ApplicationStatic_DB.conduitType.Id, e.ReferenceLevel.Id, ApplicationStatic_DB.TrayThickness, D_current, D_last, lastOffset, false, ApplicationStatic_DB.shiftToTrayBottom, false, false);
          D_last = ApplicationStatic_DB.ConduitsData[index];
          lastOffset = num3;
        }
        int num4 = (int) transaction.Commit();
        ApplicationStatic_DB.MainForm.Show();
      }
    }

    public string GetName() => nameof (RunSingleTrayHandeler);
  }
}
