using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoLegend
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument activeUiDocument = commandData.Application.ActiveUIDocument;
            Document document = activeUiDocument.Document;
            ((IEnumerable<Element>)new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_LegendComponents).WhereElementIsNotElementType()).ToList<Element>().FirstOrDefault<Element>();
            FilteredElementCollector elementCollector = new FilteredElementCollector(document);
            ElementClassFilter elementClassFilter = new ElementClassFilter(typeof(FamilyInstance));
            View view = ((IEnumerable)new FilteredElementCollector(document).OfClass(typeof(View))).Cast<View>().Where<View>((Func<View, bool>)(x => x.ViewType ==ViewType.Legend)).ToList<View>().Where<View>((Func<View, bool>)(x => ((Element)x).Name == "Legend 1")).FirstOrDefault<View>();
            ICollection<ElementId> elementIds = activeUiDocument.Selection.GetElementIds();
            Transaction transaction = new Transaction(document, "Hakuren");
            transaction.Start();
            foreach (ElementId elementId1 in (IEnumerable<ElementId>)elementIds)
            {
                ViewSheet element1 = document.GetElement(elementId1) as ViewSheet;
                List<ElementId> source1 = new List<ElementId>();
                List<FamilySymbol> familySymbolList = new List<FamilySymbol>();
                foreach (ElementId allPlacedView in (IEnumerable<ElementId>)element1.GetAllPlacedViews())
                {
                    foreach (FamilyInstance familyInstance in ((IEnumerable<Element>)new FilteredElementCollector(document, allPlacedView).WherePasses((ElementFilter)elementClassFilter).WhereElementIsNotElementType()).ToList<Element>())
                    {
                        FamilySymbol symbol = familyInstance.Symbol;
                        source1.Add(((Element)symbol).Id);
                    }
                    foreach (ElementId elementId2 in source1.Distinct<ElementId>().ToList<ElementId>())
                    {
                        FamilySymbol element2 = document.GetElement(elementId2) as FamilySymbol;
                        familySymbolList.Add(element2);
                    }
                }
                ElementId elementId3 = view.Duplicate((ViewDuplicateOption)2);
                View element3 = document.GetElement(elementId3) as View;
                ((Element)element3).Name = element1.SheetNumber;
                XYZ xyz = new XYZ(3.0, 0.4, 0.0);
                Viewport.Create(document, ((Element)element1).Id, ((Element)element3).Id, xyz);
                List<Element> list1 = ((IEnumerable<Element>)new FilteredElementCollector(document, ((Element)element3).Id).OfCategory(BuiltInCategory.OST_Lines).OfClass(typeof(CurveElement))).ToList<Element>();
                List<DetailLine> detailLineList = new List<DetailLine>();
                List<DetailLine> source2 = new List<DetailLine>();
                foreach (Element element4 in list1)
                {
                    if (Math.Abs(((element4.Location as LocationCurve).Curve as Line).Direction.Y) == 1.0)
                        detailLineList.Add(element4 as DetailLine);
                    else
                        source2.Add(element4 as DetailLine);
                }
                Element element5 = ((IEnumerable<Element>)new FilteredElementCollector(document, ((Element)element3).Id).OfCategory(BuiltInCategory.OST_LegendComponents).WhereElementIsNotElementType()).ToList<Element>().FirstOrDefault<Element>();
                List<Element> list2 = ((IEnumerable<Element>)new FilteredElementCollector(document, ((Element)element3).Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType()).ToList<Element>();
                TextNote textNote1 = (TextNote)null;
                TextNote textNote2 = (TextNote)null;
                foreach (Element element6 in list2)
                {
                    TextNote textNote3 = element6 as TextNote;
                    if (((TextElement)textNote3).Text.Contains("revit API"))
                        textNote1 = textNote3;
                    if (((TextElement)textNote3).Text.Contains("3"))
                        textNote2 = textNote3;
                }
                int num1 = 0;
                int num2 = 0;
                foreach (FamilySymbol familySymbol in familySymbolList)
                {
                    FamilySymbol el = familySymbol;
                    int num3 = source1.Where<ElementId>((Func<ElementId, bool>)(x => x == ((Element)el).Id)).Count<ElementId>();
                    if (num1 == 0)
                    {
                        element5.get_Parameter(BuiltInParameter.LEGEND_COMPONENT)?.Set(((Element)el).Id);
                        string str = element5.LookupParameter("Component Type").AsValueString();
                        ((TextElement)textNote1).Text = str;
                        ((TextElement)textNote2).Text = num3.ToString();
                    }
                    else
                    {

                        Transform transform1 = element5.get_BoundingBox(element3).Transform;
                        //Transform transform1 = element5[element3].Transform;
                        transform1.Origin = new XYZ(-transform1.Origin.X, -transform1.Origin.Y - (double)num2, 0.0);
                        ICollection<ElementId> source3 = ElementTransformUtils.CopyElement(document, element5.Id, transform1.Origin);
                        Element element7 = document.GetElement(source3.FirstOrDefault<ElementId>());
                        element7.get_Parameter(BuiltInParameter.LEGEND_COMPONENT)?.Set(((Element)el).Id);
                        string str = element7.LookupParameter("Component Type").AsValueString();
                        Transform transform2 = ((Element)textNote1).get_BoundingBox(element3).Transform;
                        transform2.Origin = new XYZ(-transform2.Origin.X, -transform2.Origin.Y - (double)num2, 0.0);
                        ICollection<ElementId> source4 = ElementTransformUtils.CopyElement(document, ((Element)textNote1).Id, transform2.Origin);
                        ((TextElement)(document.GetElement(source4.FirstOrDefault<ElementId>()) as TextNote)).Text = str;
                        Transform transform3 = ((Element)textNote2).get_BoundingBox(element3).Transform;
                        transform3.Origin = new XYZ(-transform3.Origin.X, -transform3.Origin.Y - (double)num2, 0.0);
                        ICollection<ElementId> source5 = ElementTransformUtils.CopyElement(document, ((Element)textNote2).Id, transform3.Origin);
                        ((TextElement)(document.GetElement(source5.FirstOrDefault<ElementId>()) as TextNote)).Text = num3.ToString();
                        foreach (DetailLine detailLine in detailLineList)
                        {

                            Transform transform4 = ((Element)detailLine).get_BoundingBox(element3).Transform;
                            transform4.Origin = new XYZ(-transform4.Origin.X, -transform4.Origin.Y - (double)num2, 0.0);
                            ElementTransformUtils.CopyElement(document, ((Element)detailLine).Id, transform4.Origin);
                        }
                        Transform transform5 = ((Element)source2.LastOrDefault<DetailLine>()).get_BoundingBox(element3).Transform;
                        transform5.Origin = new XYZ(-transform5.Origin.X, -transform5.Origin.Y - (double)num2, 0.0);
                        ElementTransformUtils.CopyElement(document, ((Element)source2.LastOrDefault<DetailLine>()).Id, transform5.Origin);
                    }
                    ++num1;
                    num2 += 3;
                }
            }
            transaction.Commit();
            return Result.Succeeded;
        }
    }
}
