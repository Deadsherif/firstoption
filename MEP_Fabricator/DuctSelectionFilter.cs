// Decompiled with JetBrains decompiler
// Type: MEP_Fabricator.DuctSelectionFilter
// Assembly: MEP_Fabricator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8CFBF883-28B0-455B-8DCF-FF413BFD7A7C
// Assembly location: \\fs\Temp-Share\03 R&D\omar amen\New folder (4)\2020\MEP_Fabricator.dll

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

 
namespace MEP_Fabricator
{
  public class DuctSelectionFilter : ISelectionFilter
  {
    public bool AllowElement(Element element) => element.Category.Name == "Ducts";

    public bool AllowReference(Reference reference, XYZ position) => false;
  }
}
