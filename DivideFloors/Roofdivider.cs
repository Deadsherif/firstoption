using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace DivideFloors
{
    [Transaction(TransactionMode.Manual)]
    public class Roofdivider : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;
               
                var footprintRoofs = uidoc.Selection.PickObjects(ObjectType.Element);
                foreach (var footprintRoofreference in footprintRoofs)
                {
                    var footprintRoof = doc.GetElement(footprintRoofreference) as FootPrintRoof;

                    // Divide the floor into small pieces 
                    var curtaingridsets = footprintRoof.CurtainGrids;
                    List<GeometryObject> _geometryObjects = new List<GeometryObject>();
                    foreach (CurtainGrid c in curtaingridsets)
                    {
                        var solidpanelsid = c.GetPanelIds().ToList();
                        foreach (var solidid in solidpanelsid)
                        {
                            var panel = doc.GetElement(solidid) as Autodesk.Revit.DB.Panel;
                            var opt = new Options();
                            opt.ComputeReferences = false;
                            opt.DetailLevel = ViewDetailLevel.Fine;
                            opt.IncludeNonVisibleObjects = false;
                            var panelgeometry = panel.get_Geometry(opt);
                            Solid ss = null;
                            foreach (var obj in panelgeometry)
                            {
                                var geometryinstance = obj as GeometryInstance;
                                if (null == geometryinstance)
                                {
                                    ss = obj as Solid;
                                }
                                else
                                {
                                    foreach (var instgeo in geometryinstance.GetInstanceGeometry())
                                    {
                                        ss = instgeo as Solid;
                                    }
                                }
                            }
                            Solid sss = SolidUtils.Clone(ss);
                            GeometryObject gg = sss as GeometryObject;
                            _geometryObjects.Add(gg);
                        }
                    }
                    ElementId categoryId = new ElementId(BuiltInCategory.OST_Floors);
                    foreach (GeometryObject geometryobj in _geometryObjects)
                    {
                        
                        try
                        {
                            Transaction tr = new Transaction(doc, "yarab");
                            tr.Start();
                            DirectShape ds = DirectShape.CreateElement(doc, categoryId);
                            ds.ApplicationId = Assembly.GetExecutingAssembly().GetType().GUID.ToString();
                            IList<GeometryObject> geometryObjects = new List<GeometryObject>();
                            geometryObjects.Add(geometryobj);
                            ds.ApplicationDataId = Guid.NewGuid().ToString();
                            ds.SetShape(geometryObjects);
                            ds.Name = "Floor Parts";
                            doc.Regenerate();
                            var CommentParameter = ds.LookupParameter("Comments");
                            CommentParameter.SetValueString(ds.Name);
                            try
                            {
                                var Areaparameter = ds.LookupParameter("SlabArea");
                                var solid = geometryobj as Solid;
                                foreach (PlanarFace face in solid.Faces)
                                {
                                    if (face.FaceNormal.IsAlmostEqualTo(XYZ.BasisZ))
                                    {
                                        //parameter.Set(UnitUtils.ConvertToInternalUnits(face.Area,UnitTypeId.SquareMeters));
                                        Areaparameter.Set(face.Area);
                                        break;
                                    }
                                }
                              tr.Commit();
                            }
                            
                            catch (Exception)
                            {
                                TaskDialog.Show("ERROR", "please Add Parameter with Name\"SlabArea\" for Floor Category");
                            }
                            
                        }
                        catch (Exception)
                        {


                        }


                    }
                    
                  
                }
            }
            catch (Exception)
            {
                throw;
                
            }
            return Result.Succeeded;
        }
    }
}
