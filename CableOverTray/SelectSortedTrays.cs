// Decompiled with JetBrains decompiler
// Type: CableOverTray.SelectSortedTrays
// Assembly: COT, Version=2021.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 774E24BD-3C4D-44CA-9437-58E3900DE92B
// Assembly location: C:\Users\ahmed\AppData\Roaming\Autodesk\Revit\Addins\2023\CableOverTray.dll

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

 
namespace CableOverTray
{
  [Transaction(TransactionMode.Manual)]
  internal class SelectSortedTrays : IExternalEventHandler
  {
    public void Execute(UIApplication app)
    {
      UIDocument activeUiDocument = app.ActiveUIDocument;
      Document document = activeUiDocument.Document;
      Autodesk.Revit.UI.Selection.Selection selection = activeUiDocument.Selection;
      COT_Controller cotController = new COT_Controller();
      int num = (int) TaskDialog.Show("Select sorted trays", "Please select cable trays with correct order!");
      ApplicationStatic_DB.SortedTrays = cotController.GetSortedCurves(cotController.SelectSortedTrays(selection, activeUiDocument, document));
      ApplicationStatic_DB.MainForm.Show();
    }

    public string GetName() => nameof (SelectSortedTrays);
  }
}
