// Decompiled with JetBrains decompiler
// Type: CableOverTray.RunMultibleCustomTraysHandeler
// Assembly: COT, Version=2021.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 774E24BD-3C4D-44CA-9437-58E3900DE92B
// Assembly location: C:\Users\ahmed\AppData\Roaming\Autodesk\Revit\Addins\2023\CableOverTray.dll

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

 
namespace CableOverTray
{
  [Transaction(TransactionMode.Manual)]
  internal class RunMultibleCustomTraysHandeler : IExternalEventHandler
  {
    public void Execute(UIApplication app)
    {
      Document document = app.ActiveUIDocument.Document;
      COT_Controller cotController = new COT_Controller();
      Transaction transaction1 = new Transaction(document);
      using (TransactionGroup transactionGroup = new TransactionGroup(document))
      {
        int num1 = (int) transactionGroup.Start("COT Multi-custom");
        using (Transaction transaction2 = new Transaction(document, "dummy"))
        {
          int num2 = (int) transaction2.Start();
          int num3 = (int) transaction2.Commit();
        }
        for (int index = 0; index < ApplicationStatic_DB.cutomConds.Count; ++index)
        {
          Curve curve = (ApplicationStatic_DB.cutomTray.Location as LocationCurve).Curve;
          XYZ endPoint = (ApplicationStatic_DB.cutomConds[index].Location as LocationCurve).Curve.GetEndPoint(0);
          XYZ xyzPoint1 = curve.Project(endPoint).XYZPoint;
          XYZ xyzPoint2 = ApplicationStatic_DB.customReferrenceAnchor.Project(endPoint).XYZPoint;
          double num4 = endPoint.Z - xyzPoint1.Z;
          double num5 = Math.Pow(Math.Pow(xyzPoint2.X - endPoint.X, 2.0) + Math.Pow(endPoint.Y - xyzPoint2.Y, 2.0), 0.5);
          double Y = num4 + ApplicationStatic_DB.cutomTray.Height / 2.0;
          double X = num5;
          cotController.DrawCustomConduits(document, ApplicationStatic_DB.SortedTrays, ApplicationStatic_DB.cutomConds[index].GetTypeId(), ApplicationStatic_DB.SortedTrays[0].curveHost.ReferenceLevel.Id, ApplicationStatic_DB.cutomConds[index].Diameter, X, Y, 0, true, true);
        }
        int num6 = (int) transactionGroup.Assimilate();
      }
      ApplicationStatic_DB.MainForm.Show();
    }

    public string GetName() => "RunMultibleTraysHandeler";
  }
}
