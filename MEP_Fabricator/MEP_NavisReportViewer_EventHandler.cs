// Decompiled with JetBrains decompiler
// Type: MEP_Fabricator.MEP_NavisReportViewer_EventHandler
// Assembly: MEP_Fabricator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8CFBF883-28B0-455B-8DCF-FF413BFD7A7C
// Assembly location: \\fs\Temp-Share\03 R&D\omar amen\New folder (4)\2020\MEP_Fabricator.dll

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

 
namespace MEP_Fabricator
{

  internal class MEP_NavisReportViewer_EventHandler : IExternalEventHandler
  {
    public static int id1 = 971009;
    public static int id2 = 970917;
    public static int id1old = 0;
    public static int id2old = 0;

    public void Execute(UIApplication app)
    {
      UIDocument activeUiDocument = app.ActiveUIDocument;
      this.ShowClash(activeUiDocument.Document, activeUiDocument, MEP_NavisReportViewer_EventHandler.id1, MEP_NavisReportViewer_EventHandler.id2, MEP_NavisReportViewer_EventHandler.id1old, MEP_NavisReportViewer_EventHandler.id2old);
    }

    public string GetName() => nameof (MEP_NavisReportViewer_EventHandler);

    public void ShowClash(Document doc, UIDocument uidoc, int ID1, int ID2, int OLD1, int OLD2)
    {
      ElementId elementId1 = new ElementId(ID1);
      ElementId elementId2 = new ElementId(ID2);
      try
      {
        Element element1 = doc.GetElement(elementId1);
        Element element2 = doc.GetElement(elementId2);
        ElementSet elementSet = new ElementSet();
        if (element1 != null)
          elementSet.Insert(element1);
        if (element2 != null)
          elementSet.Insert(element2);
        uidoc.ShowElements(elementSet);
      }
      catch (Exception ex)
      {
      }
      ElementId elementId3 = new ElementId(OLD1);
      ElementId elementId4 = new ElementId(OLD2);
      ElementId elementId5 = new ElementId(0);
      foreach (Element element in new FilteredElementCollector(doc).OfClass(typeof (FillPatternElement)))
      {
        if (element.Name == "<Solid fill>")
        {
          elementId5 = element.Id;
          break;
        }
      }
      using (Transaction transaction = new Transaction(doc))
      {
        transaction.Start("Show Clash");
        this.resetClashedGraphics(doc, OLD1, OLD2);
        OverrideGraphicSettings overrideGraphicSettings1 = new OverrideGraphicSettings();
        OverrideGraphicSettings overrideGraphicSettings2 = new OverrideGraphicSettings();
        Color color1 = new Color(byte.MaxValue, (byte) 0, (byte) 0);
        Color color2 = new Color((byte) 0, byte.MaxValue, (byte) 0);
        Color color3 = new Color((byte) 0, (byte) 0, (byte) 0);
        overrideGraphicSettings1.SetCutLineColor(color3);
        overrideGraphicSettings1.SetProjectionLineColor(color3);
        overrideGraphicSettings1.SetSurfaceBackgroundPatternColor(color1);
        overrideGraphicSettings1.SetSurfaceBackgroundPatternId(elementId5);
        overrideGraphicSettings1.SetSurfaceBackgroundPatternVisible(true);
        overrideGraphicSettings1.SetSurfaceForegroundPatternColor(color1);
        overrideGraphicSettings1.SetSurfaceForegroundPatternId(elementId5);
        overrideGraphicSettings1.SetSurfaceForegroundPatternVisible(true);
        overrideGraphicSettings2.SetCutLineColor(color3);
        overrideGraphicSettings2.SetProjectionLineColor(color3);
        overrideGraphicSettings2.SetSurfaceBackgroundPatternColor(color2);
        overrideGraphicSettings2.SetSurfaceBackgroundPatternId(elementId5);
        overrideGraphicSettings2.SetSurfaceBackgroundPatternVisible(true);
        overrideGraphicSettings2.SetSurfaceForegroundPatternColor(color2);
        overrideGraphicSettings2.SetSurfaceForegroundPatternId(elementId5);
        overrideGraphicSettings2.SetSurfaceForegroundPatternVisible(true);
        try
        {
          doc.ActiveView.SetElementOverrides(elementId1, overrideGraphicSettings1);
        }
        catch (Exception ex)
        {
        }
        try
        {
          doc.ActiveView.SetElementOverrides(elementId2, overrideGraphicSettings2);
        }
        catch (Exception ex)
        {
        }
        transaction.Commit();
      }
    }

    public void resetClashedGraphics(Document doc, int OLD1, int OLD2)
    {
      OverrideGraphicSettings overrideGraphicSettings = new OverrideGraphicSettings();
      try
      {
        ElementId elementId = new ElementId(OLD1);
        doc.ActiveView.SetElementOverrides(elementId, overrideGraphicSettings);
      }
      catch (Exception ex)
      {
      }
      try
      {
        ElementId elementId = new ElementId(OLD2);
        doc.ActiveView.SetElementOverrides(elementId, overrideGraphicSettings);
      }
      catch (Exception ex)
      {
      }
    }
  }
}
