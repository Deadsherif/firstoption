using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FamilyPlacer
{
   internal class FloorEventHandler : IExternalEventHandler
   {
      public void Execute(UIApplication app)
      {
         UIDocument uidoc = app.ActiveUIDocument;
         Document doc = uidoc.Document;

         // Get CAD data
         var dwgDataAll = Command.GetTuples_Points_Blocknames(DB.SelectedCadLink, doc);
         var dwgData = dwgDataAll.Distinct(new PointComparer());
         if (dwgData == null || !dwgData.Any())
         {
            TaskDialog.Show("Family Placer", "No CAD data found.");
            return;
         }
         var count = dwgData.Count();

         Transform cadTransform = Transform.Identity;
         if (DB.SelectedCadLink is ImportInstance importInstance)
            cadTransform = importInstance.GetTransform();

         FamilySymbol familySymbol = Command.GetFamilySymbole(doc, DB.SelectedType);
         if (familySymbol == null)
         {
            TaskDialog.Show("Family Placer", "No family symbol found.");
            return;
         }
         if (!familySymbol.IsActive)
            familySymbol.Activate();

         // Prepare filters
         ElementCategoryFilter ceilingFilter = new ElementCategoryFilter(BuiltInCategory.OST_Ceilings);
         ElementCategoryFilter floorFilter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
         LogicalOrFilter hostFilter = new LogicalOrFilter(ceilingFilter, floorFilter);

         // Active model intersector
         View3D active3DView = doc.ActiveView as View3D;
         if (active3DView == null)
         {
            TaskDialog.Show("Family Placer", "Please open a 3D view to use ray tracing.");
            return;
         }

         ReferenceIntersector mainIntersector = new ReferenceIntersector(hostFilter, FindReferenceTarget.Face, active3DView);

         // Linked model instances
         var linkedInstances = new FilteredElementCollector(doc)
            .OfClass(typeof(RevitLinkInstance))
            .Cast<RevitLinkInstance>()
            .ToList();

         using (Transaction tr = new Transaction(doc, "Place Families on Hosts from Links"))
         {
            tr.Start();

            foreach (var tuple in dwgData)
            {
               string blockName = tuple.Item1;
               XYZ cadPoint = tuple.Item2;

               if (blockName != DB.SelectedBlock)
                  continue;

               XYZ worldPoint = cadTransform.OfPoint(cadPoint);

               Reference hitRef = null;
               XYZ intersectionPoint = null;

               // 1️⃣ Try in the main model first
               ReferenceWithContext mainContext = mainIntersector.FindNearest(worldPoint, XYZ.BasisZ);
               double minDist = double.MaxValue;

               if (mainContext != null)
               {
                  minDist = mainContext.Proximity;
                  hitRef = mainContext.GetReference();
                  intersectionPoint = worldPoint + XYZ.BasisZ * mainContext.Proximity;
               }

               // 2️⃣ Try in each linked model
               foreach (var linkInstance in linkedInstances)
               {
                  Document linkDoc = linkInstance.GetLinkDocument();
                  if (linkDoc == null) continue;

                  Transform linkTransform = linkInstance.GetTotalTransform();
                  Transform invTransform = linkTransform.Inverse;

                  XYZ pointInLink = invTransform.OfPoint(worldPoint);
                  ReferenceIntersector linkIntersector = new ReferenceIntersector(hostFilter, FindReferenceTarget.Face, active3DView);
                  linkIntersector.TargetType = FindReferenceTarget.Face;

                  linkIntersector.FindReferencesInRevitLinks = true;
                  ReferenceWithContext linkContext = linkIntersector.FindNearest(pointInLink, XYZ.BasisZ);
                  if (linkContext == null) continue;

                  if (linkContext.Proximity < minDist)
                  {
                     minDist = linkContext.Proximity;
                     intersectionPoint = worldPoint + XYZ.BasisZ * linkContext.Proximity;
                     hitRef = linkContext.GetReference();
                  }
               }

               if (hitRef == null || intersectionPoint == null)
                  continue;

               try
               {
                  // Create family instance hosted on that face (even from a link)
                  FamilyInstance instance = doc.Create.NewFamilyInstance(hitRef, intersectionPoint, tuple.Item3, familySymbol);
                  instance.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM)?.Set(0);
               }
               catch (Exception ex)
               {
                  TaskDialog.Show("Placement Error", ex.Message);
               }
            }

            tr.Commit();
         }

         TaskDialog.Show("Family Placer", "Family placement completed successfully.");
      }

      public string GetName() => "Family Placement Event Handler (Raycast with Linked Hosts)";
   }
   public class PointComparer : IEqualityComparer<Tuple<string, XYZ, XYZ>>
   {
      private const double Tolerance = 0.001; // يمكنك تعديل الدقة حسب الحاجة

      public bool Equals(Tuple<string, XYZ, XYZ> x, Tuple<string, XYZ, XYZ> y)
      {
         if (x == null || y == null)
            return false;

         // مقارنة الاسم أولاً
         if (!string.Equals(x.Item1, y.Item1, StringComparison.OrdinalIgnoreCase))
            return false;

         // مقارنة الإحداثيات مع سماحية صغيرة
         return Math.Abs(x.Item2.X - y.Item2.X) < Tolerance &&
                Math.Abs(x.Item2.Y - y.Item2.Y) < Tolerance &&
                Math.Abs(x.Item2.Z - y.Item2.Z) < Tolerance;
      }

      public int GetHashCode(Tuple<string, XYZ, XYZ> obj)
      {
         if (obj == null) return 0;

         // تقريب القيم لعمل hash أكثر استقراراً
         int hashName = obj.Item1?.ToLower().GetHashCode() ?? 0;
         int hashX = Math.Round(obj.Item2.X / Tolerance).GetHashCode();
         int hashY = Math.Round(obj.Item2.Y / Tolerance).GetHashCode();
         int hashZ = Math.Round(obj.Item2.Z / Tolerance).GetHashCode();

         return hashName ^ hashX ^ hashY ^ hashZ;
      }
   }
}
