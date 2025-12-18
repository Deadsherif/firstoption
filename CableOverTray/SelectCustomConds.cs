// Decompiled with JetBrains decompiler
// Type: CableOverTray.SelectCustomConds
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
  internal class SelectCustomConds : IExternalEventHandler
  {
    public void Execute(UIApplication app)
    {
      UIDocument activeUiDocument = app.ActiveUIDocument;
      Document document = activeUiDocument.Document;
      Autodesk.Revit.UI.Selection.Selection selection = activeUiDocument.Selection;
      COT_Controller cotController = new COT_Controller();
      int num = (int) TaskDialog.Show("Select custom conduits", "Please select custom conduits of the sample collection");
      IList<Reference> referenceList = selection.PickObjects(ObjectType.Element, (ISelectionFilter) new ConduitsSelectionFilter());
      List<MEPCurve> mepCurveList = new List<MEPCurve>();
      foreach (Reference reference in (IEnumerable<Reference>) referenceList)
        mepCurveList.Add(document.GetElement(reference) as MEPCurve);
      ApplicationStatic_DB.cutomConds = mepCurveList;
      ApplicationStatic_DB.MainForm.Show();
    }

    public string GetName() => "SelectSortedTrays";
  }
}
