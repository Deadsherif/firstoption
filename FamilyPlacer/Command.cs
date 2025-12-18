using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DocumentFormat.OpenXml.Presentation;
using Firebase.Auth.Wpf.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Linq;
using static Autodesk.Revit.DB.SpecTypeId;
using Document = Autodesk.Revit.DB.Document;
using Reference = Autodesk.Revit.DB.Reference;

namespace FamilyPlacer
{
   [Transaction(TransactionMode.Manual)]
   internal class Command : IExternalCommand
   {
      UIDocument uidoc;
      Document doc;
      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {

         uidoc = commandData.Application.ActiveUIDocument;
         doc = uidoc.Document;
         FilteredElementCollector scopescollector = new FilteredElementCollector(doc);
         var levels = scopescollector.OfClass(typeof(Level)).Cast<Level>().ToList();
         DB.Levels = levels;
         //var elevation = level.Elevation;
         //XYZ max = new XYZ(0, 0, elevation + 1);
         //XYZ min = new XYZ(0, 0, elevation - 5);
         //Outline scopoutline = new Outline(min, max);

         // MEP Categories
         string[] categories = { "Hosted", "Non Hosted", "Pipes", "Ducts", "CableTrays", "Conduits", "Groups" };
         DB.Categories = categories;


         List<ElementType> elementTypes = new FilteredElementCollector(doc)
           .OfClass(typeof(ElementType))
            .WhereElementIsElementType()
            .Select(x => (ElementType)x)
            .ToList();
         DB.FamilyTypes = elementTypes;


         FilteredElementCollector cadfec = new FilteredElementCollector(doc);
         var cads = cadfec.OfClass(typeof(ImportInstance)).WhereElementIsNotElementType().ToList();
         DB.Cads = cads;


         FilteredElementCollector revitlinkfec = new FilteredElementCollector(doc);
         var revitlinks = revitlinkfec.OfClass(typeof(RevitLinkInstance)).WhereElementIsNotElementType().ToList();

         DB.RevitLinks = revitlinks;

         ///*** repeated code blocks, use function which uses template

         // Looking for the PipeType
         var pipeTypes =
           new FilteredElementCollector(doc)
             .OfClass(typeof(PipeType))
             .OfType<PipeType>()
             .ToList();
         // save pipe types to data base 
         DB.PipeTypes = pipeTypes;

         // Looking for the Conduit types
         var conduitTypes =
         new FilteredElementCollector(doc)
           .OfClass(typeof(ConduitType))
           .OfType<ConduitType>()
           .ToList();
         DB.ConduitTypes = conduitTypes;

         // Looking for the cabletrays types
         var cabletraysTypes =
       new FilteredElementCollector(doc)
         .OfClass(typeof(CableTrayType))
         .OfType<CableTrayType>()
         .ToList();
         DB.CabletraysTypes = cabletraysTypes;

         // Looking for the Duct types

         var ductTypes =
           new FilteredElementCollector(doc)
             .OfClass(typeof(DuctType))
             .OfType<DuctType>()
             .ToList();
         DB.DuctTypes = ductTypes;


         FilteredElementCollector fec4 = new FilteredElementCollector(doc);
         var groups = fec4.OfCategory(BuiltInCategory.OST_IOSModelGroups).WhereElementIsElementType().ToList();
         DB.Groups = groups;



         ///create external events

         ///*** can delete 1st line
         WallEventHandler wallexternalEvent = new WallEventHandler();
         ExternalEvent wallev = ExternalEvent.Create(wallexternalEvent);

         ///*** can delete 1st line
         FloorEventHandler FloorexternalEvent = new FloorEventHandler();
         ExternalEvent Floorev = ExternalEvent.Create(FloorexternalEvent);


         ///*** can delete 1st line
         NonHostedEventHandler NonhostexternalEvent = new NonHostedEventHandler();
         ExternalEvent nonhostev = ExternalEvent.Create(NonhostexternalEvent);

         ///*** can delete 1st line
         LineBasedExternalEvent linebasedexternalEvent = new LineBasedExternalEvent();
         ExternalEvent linebasedev = ExternalEvent.Create(linebasedexternalEvent);


         ///*** can delete 1st line
         GroupExternalEvent groupExternalEvent = new GroupExternalEvent();
         ExternalEvent groupev = ExternalEvent.Create(groupExternalEvent);



         MainWindow mainWindow = MainWindow.CreateInstance(doc, wallev, Floorev, nonhostev, linebasedev, groupev);
         mainWindow.Show();
         //FilteredElementCollector Lfec = new FilteredElementCollector(doc);
         //var reflink = uidoc.Selection.PickObject(ObjectType.Element);





         //var link = doc.GetElement(reflink) as RevitLinkInstance;

         var link = DB.SelectedRevitLink;

         //if (link != null)
         //{ var linked_Document = link.GetLinkDocument(); }
         // return wall types 
         FilteredElementCollector fectype = new FilteredElementCollector(doc);
         var walltypes = fectype.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType().Cast<WallType>().ToList();
         DB.WallTypes = walltypes;

         //try
         //{

         //    ui.ShowDialog();
         //}
         //catch (Exception e)
         //{
         //    //TaskDialog.Show("Error", e.Message);

         //}





         //if (ui.categorybox.SelectedItem as string == "Groups")
         //{

         //}


         //}





         return Result.Succeeded;

      }

      ///*** read about SOLID prncipals, this function can be divided to make the function has less pronsibilities
      public static List<Tuple<string, XYZ,XYZ>> GetTuples_Points_Blocknames(ImportInstance dwg, Document doc)
      {
         ///*** alyaws use ctrl+K+D
         List<Tuple<string, XYZ, XYZ>> tuples = new List<Tuple<string, XYZ, XYZ>>();
         try
         {
            foreach (GeometryObject geometryObj in dwg.get_Geometry(new Options()))
            {
               GeometryInstance instance = geometryObj as GeometryInstance;
               if (null != instance)
               {
                  foreach (GeometryObject instObj in instance.SymbolGeometry)
                  {
                     if (instObj is GeometryInstance)
                     {

                        GeometryInstance gi_block = instObj as GeometryInstance;
                        GraphicsStyle gStyle = doc.GetElement(instObj.GraphicsStyleId) as GraphicsStyle;
                        foreach (var pl in gi_block.GetInstanceGeometry())
                        {
                           if (pl is PolyLine polyline)
                           {
                              var outline = polyline.GetOutline();
                              var origin = (outline.MaximumPoint + outline.MinimumPoint) / 2;
                              string layerName = gStyle.GraphicsStyleCategory.Name;
                              if (layerName != DB.SelectedBlock)
                                 continue;
                              // Find the longest segment and its direction
                              double maxLength = 0;
                              XYZ mainDirection = null;

                              // Iterate through segments
                              int n = polyline.NumberOfCoordinates;
                              for (int i = 0; i < n - 1; i++)
                              {
                                 XYZ p1 = polyline.GetCoordinate(i);
                                 XYZ p2 = polyline.GetCoordinate(i + 1);

                                 double length = p1.DistanceTo(p2);
                                 if (length > maxLength)
                                 {
                                    maxLength = length;
                                    mainDirection = (p2 - p1).Normalize();
                                 }
                              }

                              // Handle closed PolyLine (connect last to first)
                              XYZ last = polyline.GetCoordinate(n - 1);
                              XYZ first = polyline.GetCoordinate(0);
                              double lastSegLength = last.DistanceTo(first);
                              if (lastSegLength > maxLength)
                              {
                                 maxLength = lastSegLength;
                                 mainDirection = (first - last).Normalize();
                              }

                              // Store result
                              var tuple = new Tuple<string, XYZ,XYZ>(layerName, origin,mainDirection);
                              tuples.Add(tuple);

                              continue;
                           }
                           else if (pl is Arc arc)
                           {
                              string layerName = gStyle.GraphicsStyleCategory.Name;
                              if (layerName != DB.SelectedBlock)
                                 continue;

                              // Origin (center of the circle)
                              XYZ origin = arc.Center;

                            

                              // Store result
                              var tuple = new Tuple<string, XYZ, XYZ>(layerName, origin, new XYZ(0,0,0));
                              tuples.Add(tuple);
                              continue;
                           }


                        }



                     }
                  }
               }
            }
         }
         catch (Exception)
         {
            

         }
         return tuples;


      }
      ///*** read about SOLID prncipals, you always get the geometry of dwg, make an extension method to ImportInstance class and decrease responsibilites
      public static List<GeometryObject> GetpolyLines(ImportInstance dwg, Document doc)
      {

         List<GeometryObject> polylines = new List<GeometryObject>();

         try
         {
            foreach (GeometryObject geometryObj in dwg.get_Geometry(new Options()))
            {
               GeometryInstance instance = geometryObj as GeometryInstance;
               if (null != instance)
               {

                  var instancegeo = instance.GetInstanceGeometry();
                  foreach (var geo in instancegeo)
                  {

                     //if (geo != null)
                     //{
                     polylines.Add(geo/* as Line*/);

                     //}
                  }
               }
            }
         }
         catch (Exception)
         {


         }


         ///this code to draw system family from polyline that is( for every two points  draw line)
         //for (int i = 1; i <= listofpoints.Count - 1; i++)
         //{

         //    var pipe = Pipe.Create(doc, domesticHotWaterSystemType.Id, DB.SelectedPipeType.Id, level.Id, listofpoints.ElementAt(i - 1), listofpoints.ElementAt(i));

         //    pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).SetValueString(DB.PipeDiameterValue);
         //    pipe.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM).SetValueString(DB.PipeOffset);

         //}



         return polylines;


      }
      public static List<string> GetLayerNames(ImportInstance dwg, Document doc)
      {
         List<string> LayerNames = new List<string>();
         var Category = dwg.Category;
         var layers = Category.SubCategories;
         foreach (Category layer in layers)
         {
            LayerNames.Add(layer.Name);
         }
         LayerNames.Sort();
         return LayerNames;
      }



      ///*** follow naming convention IsIndise
      public bool isIndise(XYZ point, Outline bb)
      {
         bool flag = false;

         if (point.Z < bb.MaximumPoint.Z && point.Z > bb.MinimumPoint.Z)
         {
            flag = true;
         }

         return flag;
      }
      public static Solid GetSolidOfElement(Element ele)
      {

         Solid solid = null;

         Options opt1 = new Options();
         opt1.ComputeReferences = true;

         var geometryelement = ele.get_Geometry(opt1);

         foreach (var obj in geometryelement)
         {
            var geometryinstance = obj as GeometryInstance;

            if (null == geometryinstance)
            {

               solid = obj as Solid;
               if (solid.Volume != 0 && solid != null)
               {
                  break;
               }


            }
            else
            {

               foreach (var instgeo in geometryinstance.GetInstanceGeometry())
               {
                  var ssolid = instgeo as Solid;

                  if (null != ssolid && ssolid.Volume != 0)
                  {
                     solid = ssolid;
                  }

               }
            }
         }
         return solid;
      }
      public static Reference parseLinkedReference(Document doc, Reference linkedRef)
      {
         var reps = linkedRef.ConvertToStableRepresentation(doc).Split(':');
         var res = "";
         var first = true;
         foreach (var _tup_1 in reps.Select((_p_1, _p_2) => Tuple.Create(_p_2, _p_1)))
         {
            var i = _tup_1.Item1;
            var s = _tup_1.Item2;
            var t = s;
            if (s.Contains("RVTLINK"))
            {
               if (i < reps.Length - 1)
               {
                  if (reps[i + 1] == "0")
                  {
                     t = "RVTLINK";
                  }
                  else
                  {
                     t = "0:RVTLINK";
                  }
               }
               else
               {
                  t = "0:RVTLINK";
               }
            }
            if (!first)
            {
               res = res + ":" + t;
            }
            else
            {
               res = t;
               first = false;
            }
         }
         var @ref = Reference.ParseFromStableRepresentation(doc, res);
         return @ref;
      }
      public static FamilySymbol GetFamilySymbole(Document doc, string name)
      {
         FamilySymbol familysymbole = null;
         familysymbole = (FamilySymbol)new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == name).FirstOrDefault();
         familysymbole.Activate();

         return familysymbole;
      }
      public List<T> GetAll<T>()
      {
         return
          new FilteredElementCollector(doc)
         .OfClass(typeof(T))
         .OfType<T>()
         .ToList();
      }
   }
}
