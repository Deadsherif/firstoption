// Decompiled with JetBrains decompiler
// Type: CableOverTray.SelectCustomTray
// Assembly: COT, Version=2021.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 774E24BD-3C4D-44CA-9437-58E3900DE92B
// Assembly location: C:\Users\ahmed\AppData\Roaming\Autodesk\Revit\Addins\2023\CableOverTray.dll

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

 
namespace CableOverTray
{
  [Transaction(TransactionMode.Manual)]
  internal class SelectCustomTray : IExternalEventHandler
  {
    public void Execute(UIApplication app)
    {
      UIDocument activeUiDocument = app.ActiveUIDocument;
      Document document = activeUiDocument.Document;
      Autodesk.Revit.UI.Selection.Selection selection = activeUiDocument.Selection;
      COT_Controller cotController = new COT_Controller();
      int num = (int) TaskDialog.Show("Select custom tray", "Please select cable tray of the sample collection");
      Reference reference = selection.PickObject(ObjectType.Element, (ISelectionFilter) new TraysSelectionFilter());
      XYZ globalPoint = reference.GlobalPoint;
      ApplicationStatic_DB.cutomTray = document.GetElement(reference) as MEPCurve;
      Curve curve = (ApplicationStatic_DB.cutomTray.Location as LocationCurve).Curve;
      XYZ endPoint1 = curve.GetEndPoint(0);
      XYZ endPoint2 = curve.GetEndPoint(1);
      XYZ xyz1 = new XYZ();
      XYZ xyz2 = new XYZ();
      XYZ sp;
      XYZ ep;
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
      List<XYZ> shiftedCurvePoints = new CurveWithData(ApplicationStatic_DB.cutomTray, sp, ep).GetShiftedCurvePoints(ApplicationStatic_DB.cutomTray.Width / 2.0);
      ApplicationStatic_DB.customReferrenceAnchor = (Curve) Line.CreateBound(shiftedCurvePoints[0], shiftedCurvePoints[1]);
      ApplicationStatic_DB.MainForm.Show();
    }

    public string GetName() => "SelectSortedTrays";
  }
}
