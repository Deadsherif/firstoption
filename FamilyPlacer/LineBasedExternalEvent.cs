using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Electrical;
using System.Windows.Shapes;
using Line = Autodesk.Revit.DB.Line;
using System.Windows;

namespace FamilyPlacer
{
   internal class LineBasedExternalEvent : IExternalEventHandler
   {
      public void Execute(UIApplication app)
      {
         UIDocument uidoc = app.ActiveUIDocument;
         Document doc = uidoc.Document;
         var geos = Command.GetpolyLines(DB.SelectedCadLink, doc);
         //"Pipes", "Ducts", "CableTray", "Conduits"
         //var dwgdata = Command.GetTuples_Points_Blocknames(DB.SelectedCadLink, doc);
         Transaction tr = new Transaction(doc, "Placing Families");
         tr.Start();



         #region pipes
         if (DB.SelectedCategory == "Pipes")
         {
            var mepSystemTypes = new FilteredElementCollector(doc).OfClass(typeof(PipingSystemType)).OfType<PipingSystemType>().ToList();

            // Get the Domestic hot water type
            var domesticHotWaterSystemType = mepSystemTypes.FirstOrDefault(st => st.SystemClassification == MEPSystemClassification.DomesticHotWater);

            var level = DB.SelectedPipeLevel;


            foreach (var geo in geos)
            {

               var gsi = doc.GetElement(geo.GraphicsStyleId);
               var gs = gsi as GraphicsStyle;


               if (geo is Line)
               {
                  var layername = gs.GraphicsStyleCategory.Name;
                  if (layername == DB.SelectedBlock)
                  {
                     var line = geo as Line;
                     // code to draw only the right 

                     // crete pipes from cad 
                     // Extract all pipe system types
                     var listofpoints = line.Tessellate();
                     var pipe = Pipe.Create(doc, domesticHotWaterSystemType.Id, DB.SelectedPipeType.Id, level.Id, line.GetEndPoint(0), line.GetEndPoint(1));

                     pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).SetValueString(DB.PipeDiameterValue);
                     pipe.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM).SetValueString(DB.PipeOffset);
                  }
               }
               else if (geo is PolyLine)
               {
                  var layername = gs.GraphicsStyleCategory.Name;

                  if (layername == DB.SelectedBlock)
                  {
                     var polyline = geo as PolyLine;
                     var listofpoints = polyline.GetCoordinates();
                     for (int i = 1; i <= listofpoints.Count - 1; i++)
                     {

                        var pipe = Pipe.Create(doc, domesticHotWaterSystemType.Id, DB.SelectedPipeType.Id, level.Id, listofpoints.ElementAt(i - 1), listofpoints.ElementAt(i));

                        pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).SetValueString(DB.PipeDiameterValue);
                        pipe.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM).SetValueString(DB.PipeOffset);

                     }
                  }
               }
            }
         }
         #endregion
         #region conduits

         else if (DB.SelectedCategory == "Conduits")
         {
            foreach (var geo in geos)
            {

               try
               {
                  var gsi = doc.GetElement(geo.GraphicsStyleId);
                  var gs = gsi as GraphicsStyle;
                  Level level = DB.SelectedConduitLevel;


                  if (geo is Line)
                  {
                     var layername = gs.GraphicsStyleCategory.Name;

                     if (layername == DB.SelectedBlock)
                     {
                        var line = geo as Line;
                        // code to draw only the right 

                        // crete pipes from cad 
                        // Extract all pipe system types
                        var listofpoints = line.Tessellate();
                        var conduit = Conduit.Create(doc, DB.SelectedConduitType.Id, line.GetEndPoint(0), line.GetEndPoint(1), level.Id);
                        conduit.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).SetValueString(DB.ConduitDiameterValue);
                        conduit.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM).SetValueString(DB.ConduitMiddleElevation);
                     }
                  }
                  else if (geo is PolyLine)
                  {
                     var layername = gs.GraphicsStyleCategory.Name;

                     if (layername == DB.SelectedBlock)
                     {
                        var polyline = geo as PolyLine;
                        var listofpoints = polyline.GetCoordinates();
                        for (int i = 1; i <= listofpoints.Count - 1; i++)
                        {

                           var conduit = Conduit.Create(doc, DB.SelectedConduitType.Id, listofpoints.ElementAt(i - 1), listofpoints.ElementAt(i), level.Id);
                           conduit.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).SetValueString(DB.ConduitDiameterValue);
                           conduit.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM).SetValueString(DB.ConduitMiddleElevation);

                        }
                     }
                  }
               }
               catch (Exception ex )
               {
                   
               }
            }

       
         }
         #endregion

         #region cabletrays
         else if (DB.SelectedCategory == "CableTrays")
         {
            foreach (var geo in geos)
            {

               var gsi = doc.GetElement(geo.GraphicsStyleId);
               var gs = gsi as GraphicsStyle;
               Level level = DB.SelectedCabletrayLevel;


               if (geo is Line)
               {
                  var layername = gs.GraphicsStyleCategory.Name;

                  if (layername == DB.SelectedBlock)
                  {
                     var line = geo as Line;
                     // code to draw only the right 

                     // crete pipes from cad 
                     // Extract all pipe system types
                     var listofpoints = line.Tessellate();
                     var cableTray = CableTray.Create(doc, DB.SelectedCabletrayType.Id, line.GetEndPoint(0), line.GetEndPoint(1), level.Id);
                     cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM).SetValueString(DB.CableTrayHeight);
                     cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM).SetValueString(DB.CableTrayWidth);
                     cableTray.get_Parameter(BuiltInParameter.RBS_CTC_BOTTOM_ELEVATION).SetValueString(DB.CableTrayBottomElevation);
                  }
               }
               else if (geo is PolyLine)
               {
                  var layername = gs.GraphicsStyleCategory.Name;

                  if (layername == DB.SelectedBlock)
                  {
                     var polyline = geo as PolyLine;
                     var listofpoints = polyline.GetCoordinates();
                     for (int i = 1; i <= listofpoints.Count - 1; i++)
                     {

                        var cableTray = CableTray.Create(doc, DB.SelectedCabletrayType.Id, listofpoints.ElementAt(i - 1), listofpoints.ElementAt(i), level.Id);
                        cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM).SetValueString(DB.CableTrayHeight);
                        cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM).SetValueString(DB.CableTrayWidth);
                        cableTray.get_Parameter(BuiltInParameter.RBS_CTC_BOTTOM_ELEVATION).SetValueString(DB.CableTrayBottomElevation);

                     }
                  }
               }
            }

         }
         #endregion

         #region ducts
         else if (DB.SelectedCategory == "Ducts")
         {

            MEPSystemType mepSystemType = new FilteredElementCollector(doc)
            .OfClass(typeof(MEPSystemType))
            .Cast<MEPSystemType>()
            .FirstOrDefault(sysType => sysType.SystemClassification == MEPSystemClassification.SupplyAir);
            Level level = DB.SelectedDuctLevel;
            foreach (var geo in geos)
            {

               var gsi = doc.GetElement(geo.GraphicsStyleId);
               var gs = gsi as GraphicsStyle;


               if (geo is Line)
               {
                  var layername = gs.GraphicsStyleCategory.Name;

                  if (layername == DB.SelectedBlock)
                  {
                     var line = geo as Line;
                     // code to draw only the right 

                     // crete pipes from cad 
                     // Extract all pipe system types
                     var listofpoints = line.Tessellate();

                     var duct = Duct.Create(doc, mepSystemType.Id, DB.SelectedDuctType.Id, level.Id, line.GetEndPoint(0), line.GetEndPoint(1));
                     duct.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM).SetValueString(DB.DuctHeight);
                     duct.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM).SetValueString(DB.DuctWidth);
                     duct.get_Parameter(BuiltInParameter.RBS_DUCT_BOTTOM_ELEVATION).SetValueString(DB.DuctBottomElevation);
                  }
               }
               else if (geo is PolyLine)
               {
                  var layername = gs.GraphicsStyleCategory.Name;

                  if (layername == DB.SelectedBlock)
                  {
                     var polyline = geo as PolyLine;
                     var listofpoints = polyline.GetCoordinates();
                     for (int i = 1; i <= listofpoints.Count - 1; i++)
                     {

                        var duct = Duct.Create(doc, mepSystemType.Id, DB.SelectedDuctType.Id, level.Id, listofpoints.ElementAt(i - 1), listofpoints.ElementAt(i));
                        duct.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM).SetValueString(DB.DuctHeight);
                        duct.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM).SetValueString(DB.DuctWidth);
                        duct.get_Parameter(BuiltInParameter.RBS_DUCT_BOTTOM_ELEVATION).SetValueString(DB.DuctBottomElevation);

                     }
                  }
               }
            }






         }
         #endregion
         tr.Commit();
      }

      public string GetName()
      {
         return "ok";
      }
   }
}
