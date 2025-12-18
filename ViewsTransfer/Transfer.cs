// Decompiled with JetBrains decompiler
// Type: ViewsTransfer.Transfer
// Assembly: ViewsTransfer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 097D957F-A3C8-4D58-A2D2-2E99F75DDB65
// Assembly location: C:\Users\ahmed\Downloads\ViewsTransfer\ViewsTransfer\bin\Debug\ViewsTransfer.dll

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;



namespace ViewsTransfer
{
   [Transaction(TransactionMode.Manual)]
   public class Transfer : IExternalCommand
   {
      public static int state = 0;
      public static int count = 99;

      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         UIDocument activeUiDocument = commandData.Application.ActiveUIDocument;
         Application application = commandData.Application.Application;
         Document document = activeUiDocument.Document;
         commandData.Application.Application.FailuresProcessing += new EventHandler<FailuresProcessingEventArgs>(this.DoFailureProcessing);
         //if (application.Username == "Sarah-Tarraf" || application.Username == "Farah-alqassem" || application.Username == "andrew-wasfy" || application.Username == "Musaab-Mahmoud" || application.Username == "Nahla-Mohamed" || application.Username == "o.amen" || application.Username == "noha_salah")
         //{

         int num1 = (int)new Transfer_Views().ShowDialog();
         if (Transfer.state == 0)
         {
            if (activeUiDocument.ActiveView.Category.Name == "Sheets")
            {
               int num2 = (int)TaskDialog.Show("Warning", "Please close all Sheet views first!");
            }
            else
            {
               int num3 = (int)TaskDialog.Show("Success", "Ready to paste sheets!");
               CopyAllViewSheets(commandData);
            }
         }
         else if (Transfer.state == 1)
            PasteAllViewSheets(commandData);

         return Result.Succeeded;
      }

      public static void CopyViews(ExternalCommandData commandData)
      {
         ViewsTransfer.DB.DB_Doc = commandData.Application.ActiveUIDocument.Document;
         List<ElementId> elementIdList = new List<ElementId>();
         Transaction transaction = new Transaction(commandData.Application.ActiveUIDocument.Document);
         int num1 = (int)transaction.Start("aaaa");
         List<ElementId> list = commandData.Application.ActiveUIDocument.Document.Delete(new ElementId(355841)).ToList<ElementId>();
         int num2 = (int)transaction.RollBack();
         ViewsTransfer.DB.DB_View = new List<ElementId>()
      {
        new ElementId(355841)
      };
         ViewsTransfer.DB.DB_ViewData = list;
      }

      public static void CopyViewSheets(ExternalCommandData commandData)
      {
         ViewsTransfer.DB.DB_Doc = commandData.Application.ActiveUIDocument.Document;
         List<ElementId> elementIdList1 = new List<ElementId>();
         Transaction transaction1 = new Transaction(commandData.Application.ActiveUIDocument.Document);
         int num1 = (int)transaction1.Start("aaaa");
         List<ElementId> list1 = commandData.Application.ActiveUIDocument.Document.Delete(new ElementId(540167)).ToList<ElementId>();
         int num2 = (int)transaction1.RollBack();
         List<Element> elementList1 = new List<Element>();
         foreach (ElementId id in list1)
            elementList1.Add(commandData.Application.ActiveUIDocument.Document.GetElement(id));
         List<ElementId> elementIdList2 = new List<ElementId>();
         List<ElementId> elementIdList3 = new List<ElementId>();
         List<ElementId> elementIdList4 = new List<ElementId>();
         foreach (Element element in elementList1)
         {
            if (element.Category != null)
            {
               if (element.Category.Name != "Sheets" && element.Category.Name != "Title Blocks")
                  elementIdList2.Add(element.Id);
               else if (element.Category.Name == "Sheets")
                  elementIdList3.Add(element.Id);
               else if (element.Category.Name == "Title Blocks")
                  elementIdList4.Add(element.Id);
            }
         }
         List<ElementId> list2 = (commandData.Application.ActiveUIDocument.Document.GetElement(elementIdList3[0]) as ViewSheet).GetAllViewports().ToList<ElementId>();
         List<Element> elementList2 = new List<Element>();
         foreach (ElementId id in list2)
            elementList2.Add(commandData.Application.ActiveUIDocument.Document.GetElement(id));
         List<ElementId> elementIdList5 = new List<ElementId>();
         List<XYZ> xyzList = new List<XYZ>();
         foreach (Viewport viewport in elementList2)
         {
            elementIdList5.Add(viewport.ViewId);
            xyzList.Add(viewport.GetBoxCenter());
         }
         Transaction transaction2 = new Transaction(commandData.Application.ActiveUIDocument.Document);
         int num3 = (int)transaction2.Start("Cleaning sheet");
         foreach (ElementId elementId in list2)
         {
            try
            {
               commandData.Application.ActiveUIDocument.Document.Delete(elementId);
            }
            catch (Exception ex)
            {
            }
         }
         int num4 = (int)transaction2.Commit();
         new List<ElementId>() { new ElementId(355841) };
         ViewsTransfer.DB.DB_View = elementIdList3;
         ViewsTransfer.DB.DB_ViewData = elementIdList4;
         ViewsTransfer.DB.DB_SheetViews = elementIdList5;
         ViewsTransfer.DB.DB_SheetViewsPoint = xyzList;
      }

      public static void CopyAllViewSheets(ExternalCommandData commandData)
      {
         try
         {
            ViewsTransfer.DB.DBALL_Views.Clear();
         }
         catch (Exception ex)
         {
         }
         try
         {
            ViewsTransfer.DB.DBALL_TitleBlocks.Clear();
         }
         catch (Exception ex)
         {
         }
         try
         {
            ViewsTransfer.DB.DBALL_View.Clear();
         }
         catch (Exception ex)
         {
         }
         try
         {
            ViewsTransfer.DB.DBALL_TitleBlock.Clear();
         }
         catch (Exception ex)
         {
         }
         try
         {
            ViewsTransfer.DB.DBALL_SheetViews.Clear();
         }
         catch (Exception ex)
         {
         }
         try
         {
            ViewsTransfer.DB.DBALL_SheetViewsPoint.Clear();
         }
         catch (Exception ex)
         {
         }
         ViewsTransfer.DB.DB_Doc = commandData.Application.ActiveUIDocument.Document;
         FilteredElementCollector elementCollector = new FilteredElementCollector(commandData.Application.ActiveUIDocument.Document);
         ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Sheets);
         foreach (Element element in (IEnumerable<Element>)elementCollector.OfClass(typeof(ViewSheet)).WherePasses((ElementFilter)filter).ToElements())
         {
            ViewsTransfer.DB.DBALL_Views.Add(element.Id);
            ViewsTransfer.DB.DBALL_View.Add(new List<ElementId>()
        {
          element.Id
        });
         }
         List<ElementId> elementIdList1 = new List<ElementId>();
         List<List<Element>> elementListList = new List<List<Element>>();
         foreach (ElementId dballView in ViewsTransfer.DB.DBALL_Views)
         {
            Transaction transaction = new Transaction(commandData.Application.ActiveUIDocument.Document);
            int num1 = (int)transaction.Start("aaaa");
            List<ElementId> list = commandData.Application.ActiveUIDocument.Document.Delete(dballView).ToList<ElementId>();
            int num2 = (int)transaction.RollBack();
            List<Element> elementList = new List<Element>();
            foreach (ElementId id in list)
               elementList.Add(commandData.Application.ActiveUIDocument.Document.GetElement(id));
            elementListList.Add(elementList);
         }
         List<ElementId> elementIdList2 = new List<ElementId>();
         List<ElementId> elementIdList3 = new List<ElementId>();
         List<ElementId> elementIdList4 = new List<ElementId>();
         foreach (List<Element> elementList in elementListList)
         {
            bool flag = false;
            foreach (Element element in elementList)
            {
               if (element.Category != null && element.Category.Name == "Title Blocks")
               {
                  ViewsTransfer.DB.DBALL_TitleBlocks.Add(element.Id);
                  ViewsTransfer.DB.DBALL_TitleBlock.Add(new List<ElementId>()
            {
              element.Id
            });
                  flag = true;
                  break;
               }
            }
            if (flag)
               ;
         }
         foreach (ElementId dballView in ViewsTransfer.DB.DBALL_Views)
         {
            List<ElementId> list = (commandData.Application.ActiveUIDocument.Document.GetElement(dballView) as ViewSheet).GetAllViewports().ToList<ElementId>();
            List<Element> elementList = new List<Element>();
            foreach (ElementId id in list)
               elementList.Add(commandData.Application.ActiveUIDocument.Document.GetElement(id));
            List<ElementId> elementIdList5 = new List<ElementId>();
            List<XYZ> xyzList = new List<XYZ>();
            foreach (Viewport viewport in elementList)
            {
               elementIdList5.Add(viewport.ViewId);
               xyzList.Add(viewport.GetBoxCenter());
            }
            ViewsTransfer.DB.DBALL_SheetViews.Add(elementIdList5);
            ViewsTransfer.DB.DBALL_SheetViewsPoint.Add(xyzList);
            Transaction transaction = new Transaction(commandData.Application.ActiveUIDocument.Document);
            int num3 = (int)transaction.Start("Cleaning sheet");
            foreach (ElementId elementId in list)
            {
               try
               {
                  commandData.Application.ActiveUIDocument.Document.Delete(elementId);
               }
               catch (Exception ex)
               {
               }
            }
            int num4 = (int)transaction.Commit();
         }
      }

      public void PasteViews(ExternalCommandData commandData)
      {
         Transaction transaction1 = new Transaction(commandData.Application.ActiveUIDocument.Document);
         int num1 = (int)transaction1.Start("copy View");
         List<ElementId> list = ElementTransformUtils.CopyElements(ViewsTransfer.DB.DB_Doc, (ICollection<ElementId>)ViewsTransfer.DB.DB_View, commandData.Application.ActiveUIDocument.Document, Transform.Identity, new CopyPasteOptions()).ToList<ElementId>();
         commandData.Application.ActiveUIDocument.Document.GetElement(list[0]).Name = ViewsTransfer.DB.DB_View[0].IntegerValue.ToString();
         int num2 = (int)transaction1.Commit();
         List<Element> elementList1 = new List<Element>();
         foreach (ElementId id in ViewsTransfer.DB.DB_ViewData)
            elementList1.Add(ViewsTransfer.DB.DB_Doc.GetElement(id));
         Transaction transaction2 = new Transaction(commandData.Application.ActiveUIDocument.Document);
         int num3 = (int)transaction2.Start("copy View Elements");
         List<Element> elementList2 = new List<Element>();
         List<Type> typeList = new List<Type>();
         List<Element> elementList3 = new List<Element>();
         List<ElementId> elementIdList = new List<ElementId>();
         foreach (ElementId elementId in ViewsTransfer.DB.DB_ViewData)
         {
            List<ElementId> elementsToCopy = new List<ElementId>();
            elementsToCopy.Add(elementId);
            View element1 = ViewsTransfer.DB.DB_Doc.GetElement(ViewsTransfer.DB.DB_View[0]) as View;
            View element2 = commandData.Application.ActiveUIDocument.Document.GetElement(list[0]) as View;
            try
            {
               new List<ElementId>() { new ElementId(355831) };
               foreach (ElementId id in ElementTransformUtils.CopyElements(element1, (ICollection<ElementId>)elementsToCopy, element2, Transform.Identity, new CopyPasteOptions()).ToList<ElementId>())
               {
                  Element element3 = commandData.Application.ActiveUIDocument.Document.GetElement(id);
                  if (element3.GetType().Name == "Element" || element3.GetType().Name == "ViewPlan" || element3.GetType().Name == "ViewSection" || element3.GetType().Name == "Viewport" || element3.GetType().Name == "RevisionCloud" || element3.GetType().Name == "Sketch")
                  {
                     try
                     {
                        commandData.Application.ActiveUIDocument.Document.Delete(element3.Id);
                     }
                     catch (Exception ex)
                     {
                     }
                  }
                  elementList2.Add(commandData.Application.ActiveUIDocument.Document.GetElement(id));
                  elementIdList.Add(id);
               }
            }
            catch (Exception ex)
            {
            }
         }
         int num4 = (int)transaction2.Commit();
      }

      public static bool PasteViewSheets(
        ExternalCommandData commandData,
        List<ElementId> ViewSheetId,
        List<ElementId> ViewTitleBlock,
        List<ElementId> SheetViews,
        List<XYZ> SheetViewsLocation)
      {
         CopyPasteOptions options = new CopyPasteOptions();
         options.SetDuplicateTypeNamesHandler((IDuplicateTypeNamesHandler)new MyCopyHandler());
         Transaction transaction1 = new Transaction(commandData.Application.ActiveUIDocument.Document);
         int num1 = (int)transaction1.Start("Paste View Sheet");
         ICollection<ElementId> source;
         try
         {
            source = ElementTransformUtils.CopyElements(ViewsTransfer.DB.DB_Doc, (ICollection<ElementId>)ViewSheetId, commandData.Application.ActiveUIDocument.Document, Transform.Identity, options);
         }
         catch (Exception ex)
         {
            int num2 = (int)transaction1.RollBack();
            return false;
         }
         List<ElementId> list = source.ToList<ElementId>();
         int num3 = (int)transaction1.Commit();
         if (ViewTitleBlock != null)
         {
            List<Element> elementList1 = new List<Element>();
            foreach (ElementId id in ViewTitleBlock)
               elementList1.Add(ViewsTransfer.DB.DB_Doc.GetElement(id));
            Transaction transaction2 = new Transaction(commandData.Application.ActiveUIDocument.Document);
            int num4 = (int)transaction2.Start("Paste Sheet TitleBlock");
            List<Element> elementList2 = new List<Element>();
            List<Type> typeList = new List<Type>();
            List<Element> elementList3 = new List<Element>();
            List<ElementId> elementIdList = new List<ElementId>();
            foreach (ElementId elementId in ViewTitleBlock)
            {
               List<ElementId> elementsToCopy = new List<ElementId>();
               elementsToCopy.Add(elementId);
               View element1 = ViewsTransfer.DB.DB_Doc.GetElement(ViewSheetId[0]) as View;
               View element2 = commandData.Application.ActiveUIDocument.Document.GetElement(list[0]) as View;
               try
               {
                  ElementTransformUtils.CopyElements(element1, (ICollection<ElementId>)elementsToCopy, element2, Transform.Identity, options);
               }
               catch (Exception ex)
               {
               }
            }
            int num5 = (int)transaction2.Commit();
         }
         for (int index = 0; index < SheetViews.Count; ++index)
         {
            PasteViewAndCreateViewport(commandData.Application.ActiveUIDocument.Document, SheetViews[index], list[0], SheetViewsLocation[index]);
            try
            {
            }
            catch (Exception ex)
            {
            }
         }
         return true;
      }

      public static void PasteAllViewSheets(ExternalCommandData commandData)
      {
         for (int index = 0; index < ViewsTransfer.DB.DBALL_Views.Count; ++index)
         {
            if (index < ViewsTransfer.DB.DBALL_Views.Count && index < ViewsTransfer.DB.DBALL_TitleBlock.Count && index < ViewsTransfer.DB.DBALL_SheetViews.Count && index < ViewsTransfer.DB.DBALL_SheetViewsPoint.Count)
               PasteViewSheets(commandData, ViewsTransfer.DB.DBALL_View[index], ViewsTransfer.DB.DBALL_TitleBlock[index], ViewsTransfer.DB.DBALL_SheetViews[index], ViewsTransfer.DB.DBALL_SheetViewsPoint[index]);
         }
      }

      public static bool PasteViewAndCreateViewport(
        Document doc,
        ElementId viewID,
        ElementId sheetID,
        XYZ centerPoint)
      {
         CopyPasteOptions options = new CopyPasteOptions();
         options.SetDuplicateTypeNamesHandler((IDuplicateTypeNamesHandler)new MyCopyHandler());
         View element1 = ViewsTransfer.DB.DB_Doc.GetElement(viewID) as View;
         Transaction transaction1 = new Transaction(ViewsTransfer.DB.DB_Doc);
         int num1 = (int)transaction1.Start("______________");
         ElementId id1;
         try
         {
            id1 = element1.Duplicate(ViewDuplicateOption.WithDetailing);
         }
         catch (Exception ex)
         {
            int num2 = (int)transaction1.RollBack();
            return false;
         }
         ViewsTransfer.DB.DB_Doc.GetElement(id1);
         int num3 = (int)transaction1.Commit();
         viewID = id1;
         List<ElementId> elementsToCopy1 = new List<ElementId>();
         elementsToCopy1.Add(viewID);
         Transaction transaction2 = new Transaction(ViewsTransfer.DB.DB_Doc);
         int num4 = (int)transaction2.Start("aaaa");
         ICollection<ElementId> source1;
         try
         {
            source1 = ViewsTransfer.DB.DB_Doc.Delete(viewID);
         }
         catch (Exception ex)
         {
            int num5 = (int)transaction2.RollBack();
            return false;
         }
         List<ElementId> list1 = source1.ToList<ElementId>();
         int num6 = (int)transaction2.RollBack();
         Transaction transaction3 = new Transaction(doc);
         int num7 = (int)transaction3.Start("copy Vieww");
         ICollection<ElementId> source2;
         try
         {
            source2 = ElementTransformUtils.CopyElements(ViewsTransfer.DB.DB_Doc, (ICollection<ElementId>)elementsToCopy1, doc, Transform.Identity, options);
            int num8 = (int)transaction3.Commit();
         }
         catch (Exception ex)
         {
            int num9 = (int)transaction3.RollBack();
            return false;
         }
         List<ElementId> list2 = source2.ToList<ElementId>();
         List<Element> elementList1 = new List<Element>();
         foreach (ElementId id2 in list1)
            elementList1.Add(ViewsTransfer.DB.DB_Doc.GetElement(id2));
         Transaction transaction4 = new Transaction(doc);
         int num10 = (int)transaction4.Start("create viewport");
         try
         {
            Viewport.Create(doc, sheetID, list2[0], centerPoint);
            int num11 = (int)transaction4.Commit();
         }
         catch (Exception ex)
         {
            int num12 = (int)transaction4.RollBack();
            return false;
         }
         Transaction transaction5 = new Transaction(doc);
         int num13 = (int)transaction5.Start("copy View Elements");
         List<Element> elementList2 = new List<Element>();
         List<Type> typeList = new List<Type>();
         List<Element> elementList3 = new List<Element>();
         List<ElementId> elementIdList = new List<ElementId>();
         foreach (ElementId elementId in list1)
         {
            List<ElementId> elementsToCopy2 = new List<ElementId>();
            elementsToCopy2.Add(elementId);
            View element2 = ViewsTransfer.DB.DB_Doc.GetElement(viewID) as View;
            View element3 = doc.GetElement(list2[0]) as View;
            try
            {
               foreach (ElementId id3 in ElementTransformUtils.CopyElements(element2, (ICollection<ElementId>)elementsToCopy2, element3, Transform.Identity, options).ToList<ElementId>())
               {
                  Element element4 = doc.GetElement(id3);
                  if (element4 != null)
                  {
                     if (element4.GetType().Name == "Element" || element4.GetType().Name == "ViewPlan" || element4.GetType().Name == "ViewSection" || element4.GetType().Name == "Viewport" || element4.GetType().Name == "RevisionCloud" || element4.GetType().Name == "Sketch")
                     {
                        try
                        {
                           doc.Delete(element4.Id);
                        }
                        catch (Exception ex)
                        {
                        }
                     }
                  }
                  elementList2.Add(doc.GetElement(id3));
                  elementIdList.Add(id3);
               }
            }
            catch (Exception ex)
            {
            }
         }
         int num14 = (int)transaction5.Commit();
         return true;
      }

      public void DoFailureProcessing(object sender, FailuresProcessingEventArgs args)
      {
         FailuresAccessor failuresAccessor = args.GetFailuresAccessor();
         foreach (FailureMessageAccessor failureMessage in (IEnumerable<FailureMessageAccessor>)failuresAccessor.GetFailureMessages())
         {
            if (failuresAccessor.GetSeverity() == FailureSeverity.Warning)
            {
               failuresAccessor.DeleteAllWarnings();
            }
            else
            {
               failuresAccessor.ResolveFailure(failureMessage);
               args.SetProcessingResult(FailureProcessingResult.ProceedWithCommit);
            }
            args.SetProcessingResult(FailureProcessingResult.ProceedWithCommit);
         }
      }
   }
}
