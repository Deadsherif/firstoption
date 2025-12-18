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
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace DivideFloors
{
    [Transaction(TransactionMode.Manual)]

    public class FloorDivider : IExternalCommand
    {
        public static FootPrintRoof footprintRoof = null;
        //public static List<ElementId> solidpanelsid;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            footprintRoof = null;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            AddinWindow ui = new AddinWindow(commandData);
            ui.ShowDialog();
            return Result.Succeeded;
        }
        public ElementId CreateNewMaterial(Document doc, Byte R, Byte G, Byte B, int A)//alpha channel to be added
        {
            string materialName = $"InsulationMaterial {R}-{G}-{B}-{A}";
            Material _mat = null;
            FilteredElementCollector fec = new FilteredElementCollector(doc);
            _mat = fec.OfCategory(BuiltInCategory.OST_Materials).Where(m => m.Name == materialName).FirstOrDefault() as Material;

            if (_mat == null)
            {
                ElementId matID = Material.Create(doc, materialName);
                Material mat = doc.GetElement(matID) as Material;
                mat.Color = new Color(R, G, B);

                int alpha = A > 100 ? 100 : A;
                alpha = alpha < 0 ? 0 : alpha;
                mat.Transparency = alpha;

                return matID;

            }
            else
            {

                return _mat.Id;
            }
        }
        public static void ApplyLogic(ExternalCommandData commandData) 
        {
           
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            AddinWindow ui = new AddinWindow(commandData);
            var roomreference = ApplicationDB.reference; 

            Autodesk.Revit.ApplicationServices.Application application = doc.Application;
            // Define the footprint for the roof based on user selection//////////////////
            RoofType roofType = new FilteredElementCollector(doc).OfClass(typeof(RoofType)).Where(x=>x.Name== "Sloped Glazing").FirstOrDefault<Element>() as RoofType;

            // Get and Set material 
            var paneltype = new FilteredElementCollector(doc).OfClass(typeof(PanelType)).Where(x => x.Name == "Solid").FirstOrDefault();
            var MATERIALnew = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Materials).Where(s => s.Name == ApplicationDB.material).FirstOrDefault();
            var MATERIALnewid = MATERIALnew.Id;
            var materialparam = paneltype.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM);
            Transaction fr = new Transaction(doc, "holly shitt");
            fr.Start();
            materialparam.Set(MATERIALnewid);
            // Get the handle of the application
            // Define the footprint for the roof based on user selection//////////////////
            ModelCurveArray footPrintToModelCurveMapping = new ModelCurveArray();
            var room = doc.GetElement(roomreference) as Room;
            var level = room.Level;
            var boundarySegments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
            CurveArray footprint = application.Create.NewCurveArray();
            foreach (var boundsegmentlist in boundarySegments)
            {
                foreach (var bs in boundsegmentlist)
                {
                    var x = bs.GetCurve();
                    footprint.Append(x);
                }
            }
            if (footprintRoof == null)
            {
                footprintRoof = doc.Create.NewFootPrintRoof(footprint, level, roofType, out footPrintToModelCurveMapping);
            }
            // offset
            var offsetparam = footprintRoof.get_Parameter(BuiltInParameter.ROOF_LEVEL_OFFSET_PARAM);
            var value = double.Parse(ApplicationDB.Offset) + 100 - 0.50 * double.Parse(ApplicationDB.thickness);
            offsetparam.Set(UnitUtils.ConvertToInternalUnits(value, UnitTypeId.Millimeters));
            // spacing for grid 1
            var spacingparam_1 = footprintRoof.RoofType.get_Parameter(BuiltInParameter.SPACING_LENGTH_1);
            spacingparam_1.Set(UnitUtils.ConvertToInternalUnits(double.Parse(ApplicationDB.width), UnitTypeId.Millimeters));
            // spacing for grid 2 
            var spacingparam_2 = footprintRoof.RoofType.get_Parameter(BuiltInParameter.SPACING_LENGTH_2);
            spacingparam_2.Set(UnitUtils.ConvertToInternalUnits(double.Parse(ApplicationDB.length), UnitTypeId.Millimeters));
            // translation  for grid 1 
            var translationparam_1 = footprintRoof.get_Parameter(BuiltInParameter.CURTAINGRID_ORIGIN_VERT);
            translationparam_1.Set(UnitUtils.ConvertToInternalUnits(double.Parse(ApplicationDB.GridX), UnitTypeId.Millimeters));
            // translation   for grid 2
            var translationparam_2 = footprintRoof.get_Parameter(BuiltInParameter.CURTAINGRID_ORIGIN_HORIZ);
            translationparam_2.Set(UnitUtils.ConvertToInternalUnits(double.Parse(ApplicationDB.GridY), UnitTypeId.Millimeters));
            // rotation
            var Rotation1 = footprintRoof.get_Parameter(BuiltInParameter.CURTAINGRID_ANGLE_1);
            Rotation1.Set(UnitUtils.ConvertToInternalUnits(double.Parse(ApplicationDB.rotation), UnitTypeId.Degrees));
            var Rotation2 = footprintRoof.get_Parameter(BuiltInParameter.CURTAINGRID_ANGLE_2);
            Rotation2.Set(UnitUtils.ConvertToInternalUnits(double.Parse(ApplicationDB.rotation), UnitTypeId.Degrees));
            ModelCurveArrayIterator iterator = footPrintToModelCurveMapping.ForwardIterator();
            iterator.Reset();
           
           doc.Regenerate();     // whaen somebody was testing this addin there was an exception called regenerate failed i couldnt do any thing for it 
           
           
            fr.Commit();
        }
        public static void DivideIt(Document doc)
        {
            Transaction tr = new Transaction(doc, "divide transaction");
            tr.Start();
            try
            {
                
                var footprintRoof = FloorDivider.footprintRoof;
                // Divide the floor into small pieces 
                var curtaingridsets = footprintRoof.CurtainGrids;
                List<GeometryObject> _geometryObjects = new List<GeometryObject>();
                foreach (CurtainGrid c in curtaingridsets)
                {
                    var solidpanelsid = c.GetPanelIds().ToList();
                    foreach (var solidid in solidpanelsid)
                    { 
                        var panel = doc.GetElement(solidid) as Autodesk.Revit.DB.Panel;
                        var thicknesparam = panel.PanelType.get_Parameter(BuiltInParameter.CURTAIN_WALL_SYSPANEL_THICKNESS);
                        thicknesparam.SetValueString(ApplicationDB.thickness);
                        doc.Regenerate();
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
                        Solid sss =  SolidUtils.Clone(ss);
                        GeometryObject gg = sss as GeometryObject;
                        _geometryObjects.Add(gg);
                    }           
                }
                ElementId categoryId = new ElementId(BuiltInCategory.OST_Floors);
                foreach (GeometryObject geometryobj in _geometryObjects)
                {
                    try
                    {
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
                        //try
                        //{
                        //    var Areaparameter = ds.LookupParameter("SlabArea");
                        //    var solid = geometryobj as Solid;
                        //    foreach (PlanarFace face in solid.Faces)
                        //    {
                        //        if (face.FaceNormal.IsAlmostEqualTo(XYZ.BasisZ))
                        //        {
                        //            //parameter.Set(UnitUtils.ConvertToInternalUnits(face.Area,UnitTypeId.SquareMeters));
                        //            Areaparameter.Set(face.Area);
                        //            break;
                        //        }
                        //    }
                        //}
                        //catch (Exception)
                        //{
                        //    TaskDialog.Show("ERROR", "please Add Parameter with Name\"SlabArea\" for Floor Category");
                        //}
                    }
                    catch (Exception)
                    {

                       
                    }
                  
                    
                }
                doc.Delete(footprintRoof.Id);
                
            }
            catch (Exception )
            {
                    
            }
            tr.Commit(); 
        }
        public static void SelectDS_ByRoom(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            // select room in the model
            var roomreference = uidoc.Selection.PickObject(ObjectType.Element);
            var room = doc.GetElement(roomreference) as Room;
            ICollection<ElementId> elementids = new List<ElementId>();
            FilteredElementCollector fec = new FilteredElementCollector(doc);
            IList<Element> directshapesids = fec.OfClass(typeof(DirectShape)).WhereElementIsNotElementType().ToElements();
            foreach (Element ele in directshapesids)
            {
                try
                {
                    BoundingBoxXYZ x = ele.get_BoundingBox(null);
                    XYZ point = x.Max;
                    if (room.IsPointInRoom(point))
                    {
                        elementids.Add(ele.Id);
                    }

                }
                catch (Exception)
                {

                    
                }
            }
            uidoc.Selection.SetElementIds(elementids);
        }

    }
}
    