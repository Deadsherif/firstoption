using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FamilyPlacer
{

    internal class WallEventHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {

            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            var dwgdata = Command.GetTuples_Points_Blocknames(DB.SelectedCadLink, doc);
            Transaction tr = new Transaction(doc, "Placing Families");
            tr.Start();
            #region Retrieve all Walls 
            // make family instance on ceiling as host 
            foreach (Wall wall in DB.walls)
            {

                var h = wall as HostObject;

                var reference = HostObjectUtils.GetSideFaces(h, ShellLayerType.Interior)[0];

                var linked_reference = reference.CreateLinkReference(DB.SelectedRevitLink);


                var parsed_ref = Command.parseLinkedReference(doc, linked_reference);
                var face = h.GetGeometryObjectFromReference(reference);
                var bb = wall.get_BoundingBox(doc.ActiveView);
                var bbmax = bb.Max;

                var bbmin = bb.Min.Subtract(new XYZ(5, 5, 0));
                //var med = (bbmax + bbmin) / 2;
                foreach (var tuple in dwgdata)
                {
                    if (tuple.Item1 == DB.SelectedBlock)
                    {
             
                        var point = tuple.Item2;
                        ///*** nested conditiont is nor required, oneis enough
                        if (point.X < bbmax.X && point.X > bbmin.X)
                        {
                            if (point.Y < bb.Max.Y && point.Y > bb.Min.Y)
                            {
                                var familysymbol = Command.GetFamilySymbole(doc, DB.SelectedType);
                                familysymbol.Activate();
                                try
                                {
                                   var familyinstance = doc.Create.NewFamilyInstance(parsed_ref, point, new XYZ(0, 0, 0), familysymbol);
                                   familyinstance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).SetValueString(DB.SelectedWallLevel.Name);
                                   
                                   //familyinstance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(0);
                                   //familyinstance.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).Set(0);
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                    }
                }

            }
            #endregion

            #region 
            // Get all walls in the current project
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Walls);

            foreach (Wall wall in collector)        
            {
                // Get the solid geometry of the wall
                Options options = new Options();
                GeometryElement geomElem = wall.get_Geometry(options);

                // Iterate through each face of the wall
                foreach (GeometryObject geomObj in geomElem)
                {
                    if (geomObj is Solid)
                    {
                        Solid solid = geomObj as Solid;
                        foreach (Face face in solid.Faces)
                        {
                            if (face.IsInside(null))
                            {
                                // This is the interior face of the wall
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
