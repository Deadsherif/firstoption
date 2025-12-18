// Decompiled with JetBrains decompiler
// Type: CableOverTray.TraysSelectionFilter
// Assembly: COT, Version=2021.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 774E24BD-3C4D-44CA-9437-58E3900DE92B
// Assembly location: C:\Users\ahmed\AppData\Roaming\Autodesk\Revit\Addins\2023\CableOverTray.dll

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI.Selection;

 
namespace CableOverTray
{
  internal class TraysSelectionFilter : ISelectionFilter
  {
    public bool AllowElement(Element elem) => elem is CableTray;

    public bool AllowReference(Reference reference, XYZ position) => true;
  }
}
