// Decompiled with JetBrains decompiler
// Type: CableOverTray.RunMultibleTraysHandeler
// Assembly: COT, Version=2021.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 774E24BD-3C4D-44CA-9437-58E3900DE92B
// Assembly location: C:\Users\ahmed\AppData\Roaming\Autodesk\Revit\Addins\2023\CableOverTray.dll

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

 
namespace CableOverTray
{
  [Transaction(TransactionMode.Manual)]
  internal class RunMultibleTraysHandeler : IExternalEventHandler
  {
    public void Execute(UIApplication app)
    {
      Document document = app.ActiveUIDocument.Document;
      COT_Controller cotController = new COT_Controller();
      Transaction transaction = new Transaction(document);
      if (ApplicationStatic_DB.ConduitsData == null || ApplicationStatic_DB.ConduitsData.Count == 0)
      {
        ApplicationStatic_DB.MainForm.Show();
      }
      else
      {
        foreach (Element conduitType in ApplicationStatic_DB.conduitTypes)
        {
          if (conduitType.Name == ApplicationStatic_DB.conduitTypeName)
          {
            ApplicationStatic_DB.conduitType = conduitType;
            break;
          }
        }
        if (ApplicationStatic_DB.conduitType == null)
        {
          ApplicationStatic_DB.MainForm.Show();
        }
        else
        {
          int num1 = (int) transaction.Start("COT");
          double lastOffset = 0.0;
          double D_last = 0.0;
          for (int index = 0; index < ApplicationStatic_DB.ConduitsData.Count; ++index)
          {
            double D_current = ApplicationStatic_DB.ConduitsData[index];
            double num2 = cotController.DrawConduits(document, ApplicationStatic_DB.SortedTrays, ApplicationStatic_DB.conduitType.Id, ApplicationStatic_DB.SortedTrays[0].curveHost.ReferenceLevel.Id, ApplicationStatic_DB.TrayThickness, D_current, D_last, lastOffset, ApplicationStatic_DB.firstTraySpacingCalculation, ApplicationStatic_DB.shiftToTrayBottom, ApplicationStatic_DB.withFittings, ApplicationStatic_DB.justifyFittings);
            D_last = ApplicationStatic_DB.ConduitsData[index];
            lastOffset = num2;
          }
          int num3 = (int) transaction.Commit();
          ApplicationStatic_DB.MainForm.Show();
        }
      }
    }

    public string GetName() => nameof (RunMultibleTraysHandeler);
  }
}
