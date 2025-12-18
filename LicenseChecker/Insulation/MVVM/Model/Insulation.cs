using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Insulation.MVVM.Model
{
    public class Insulation
    {
        public Solid JoinedSolid { get; set; }
        public static List<Solid> solids = new List<Solid>();
        public static Solid solid;
        public static List<PlanarFace> topfaceofcolumn = new List<PlanarFace>();
        public static ElementId graphicstyleid;
        public Solid JoinSolids(List<Solid> solids)
        {

            List<Solid> healthySolids = new List<Solid>();
            foreach (Solid solid in solids)
            {
                if (null != solid && 0 < solid.Faces.Size)
                {
                    var newsolid = SolidUtils.Clone(solid);
                    healthySolids.Add(newsolid);

                }
            }
            if (healthySolids.Count > 1)
            {
                for (int i = 1; i < healthySolids.Count; i++)
                {
                    if (JoinedSolid == null)
                    {
                        JoinedSolid = healthySolids[0];
                    }

                    JoinedSolid = BooleanOperationsUtils.ExecuteBooleanOperation(JoinedSolid, healthySolids[i], BooleanOperationsType.Union);

                }

            }
            else
            {
                JoinedSolid = solids.ElementAt(1);
            }
            return JoinedSolid;
        }
        public static ElementId CreateNewMaterial(Document doc, Byte R, Byte G, Byte B, int A)//alpha channel to be added
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
        public void ModelInsulation(Document doc, FaceArray faces, double insulation_thickness, ElementId categoryId, ElementId materialID, Boolean OffsetTheFaces)
        {
            foreach (Face face in faces)
            {
                SolidOptions solidOptions = new SolidOptions(materialID, graphicstyleid);
                IList<CurveLoop> curves = face.GetEdgesAsCurveLoops();
                if (OffsetTheFaces)
                {
                    MoveCurveLoops(curves, -0.0164042, (face as PlanarFace).FaceNormal);
                }
                var geometryextrusion = GeometryCreationUtilities.CreateExtrusionGeometry(curves, (face as PlanarFace).FaceNormal, insulation_thickness, solidOptions);
                GeometryObject go = geometryextrusion as GeometryObject;
                DirectShape ds = DirectShape.CreateElement(doc, categoryId);
                ds.ApplicationId = Assembly.GetExecutingAssembly().GetType().GUID.ToString();
                IList<GeometryObject> geometryObjects = new List<GeometryObject>();
                geometryObjects.Add(go);
                ds.ApplicationDataId = Guid.NewGuid().ToString();
                ds.SetShape(geometryObjects);
                ds.Name = "MyShape";
            }
        }
        public static void OffsetAllCurves(ref IList<CurveLoop> curves, double OffsetNum, XYZ FaceNormal)
        {
            List<CurveLoop> NewCurves = new List<CurveLoop>();
            foreach (var curveloop in curves)
            {
                var newcurve = CurveLoop.CreateViaOffset(curveloop, OffsetNum, FaceNormal);
                NewCurves.Add(newcurve);
            }
            curves = NewCurves;
        }
        public static void MoveCurveLoops(IList<CurveLoop> curves, double Distance, XYZ FaceNormal)
        {

            foreach (var curveloop in curves)
            {
                curveloop.Transform(Transform.CreateTranslation(FaceNormal.Normalize() * Distance));
            }

        }
        public static void ModelInsulationOneFace(Document doc, Face face, double insulation_thickness, ElementId categoryId, Boolean OffsetTheFaces)   // need some modification to make insulation for the face after joining with the other elem static ents around it
        {
            IList<CurveLoop> curves = face.GetEdgesAsCurveLoops();
            if (OffsetTheFaces)
            {
                MoveCurveLoops(curves, Database.offsetfromface, (face as PlanarFace).FaceNormal);
            }
            var geometryextrusion = GeometryCreationUtilities.CreateExtrusionGeometry(curves, (face as PlanarFace).FaceNormal, insulation_thickness); // let the
            GeometryObject go = geometryextrusion as GeometryObject;
            DirectShape ds = DirectShape.CreateElement(doc, categoryId);
            ds.ApplicationId = Assembly.GetExecutingAssembly().GetType().GUID.ToString();
            IList<GeometryObject> geometryObjects = new List<GeometryObject>();
            geometryObjects.Add(go);
            ds.ApplicationDataId = Guid.NewGuid().ToString();
            ds.SetShape(geometryObjects);
            ds.Name = "MyShape";
        }
        public Solid GetSolidOfElement(Document doc, Reference reference)
        {
            var ele = doc.GetElement(reference);
            Options opt1 = new Options();
            opt1.ComputeReferences = true;
            var geometryelement = ele.get_Geometry(opt1);
            foreach (var obj in geometryelement)
            {
                var geometryinstance = obj as GeometryInstance;
                if (null == geometryinstance)
                {
                    var solid = obj as Solid;
                }
                else
                {
                    foreach (var instgeo in geometryinstance.GetInstanceGeometry())
                    {
                        solid = instgeo as Solid;
                    }
                }
            }
            return solid;
        }
        public Solid GetSolidOfElement(Document doc, Element ele)
        {

            Options opt1 = new Options();
            opt1.ComputeReferences = true;
            var geometryelement = ele.get_Geometry(opt1);

            foreach (var obj in geometryelement)
            {
                if (obj != null)
                {
                    Solid xsolid = obj as Solid;
                    if (xsolid != null && xsolid.Volume != 0)
                    {
                        solid = xsolid;
                    }
                    else
                    {

                        var geometryinstance = obj as GeometryInstance;
                        if (null == geometryinstance)
                        {
                            var solid = obj as Solid;
                        }
                        else
                        {
                            foreach (var instgeo in geometryinstance.GetInstanceGeometry())
                            {
                                solid = instgeo as Solid;
                            }
                        }
                    }
                }
            }
            graphicstyleid = geometryelement.GraphicsStyleId;





            return solid;
        }
        public static void Insulatebylevel(UIDocument uidoc, Document doc, string levelname)
        {
            Insulation ins = new Insulation();
            List<Element> elements = new List<Element>();
            FilteredElementCollector fec = new FilteredElementCollector(doc);
            var level = fec.OfCategory(BuiltInCategory.OST_Levels).Where(x => x.Name == levelname).FirstOrDefault() as Level;
            var levelid = level.Id;
            //var levelbox = level.get_BoundingBox(doc.ActiveView);
            var center = level.Elevation;
            ICollection<ElementId> ids = new HashSet<ElementId>();
            ElementLevelFilter levelfilter = new ElementLevelFilter(level.Id);
            FilteredElementCollector fec2 = new FilteredElementCollector(doc);
            var columns = fec2.OfCategory(BuiltInCategory.OST_StructuralColumns).WhereElementIsNotElementType().ToElements();
            FilteredElementCollector fec3 = new FilteredElementCollector(doc);
            var foundations = fec3.OfCategory(BuiltInCategory.OST_StructuralFoundation).WhereElementIsNotElementType().ToElements();
            elements.AddRange(foundations);
            elements.AddRange(columns);
            FilteredElementCollector fec4 = new FilteredElementCollector(doc);
            var groundbeams = fec4.OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements();
            elements.AddRange(groundbeams);
            try
            {
                foreach (var col in columns)
                {
                    if (col != null)
                    {
                        var colbox = col.get_BoundingBox(uidoc.ActiveView);
                        if (center > colbox.Min.Z)
                        {
                            Transaction tr = new Transaction(doc, "Cutcolumnatlevel");
                            tr.Start();
                            var toplevelparameter = col.LookupParameter("Top Level");
                            var x = toplevelparameter.Set(levelid);

                            var topoffsetparameter = col.LookupParameter("Top Offset");
                            topoffsetparameter.Set(0);
                            doc.Regenerate();
                            var solid = ins.GetSolidOfElement(doc, col);
                            var clonedsolid = SolidUtils.Clone(solid);

                            foreach (PlanarFace face in clonedsolid.Faces)
                            {
                                var big = face.Origin.Z;
                                if (face.FaceNormal.Z == 1)
                                {
                                    topfaceofcolumn.Add(face);

                                }

                            }
                            solids.Add(clonedsolid);
                            tr.RollBack();
                        }
                    }
                }
                foreach (var foundation in foundations)
                {
                    if (foundation != null)
                    {
                        var fbox = foundation.get_BoundingBox(uidoc.ActiveView);
                        if (center > fbox.Min.Z)
                        {
                            Transaction tr = new Transaction(doc, "Cutcolumnatlevel");
                            tr.Start();
                            var solid = ins.GetSolidOfElement(doc, foundation);
                            var clonedsolid = SolidUtils.Clone(solid);
                            solids.Add(clonedsolid);
                            tr.Commit();
                        }
                    }
                }
                foreach (var beam in groundbeams)
                {
                    if (beam != null)
                    {
                        var bbox = beam.get_BoundingBox(uidoc.ActiveView);
                        if (center > bbox.Min.Z)
                        {
                            Transaction tr = new Transaction(doc, "Cutcolumnatlevel");
                            tr.Start();

                            var solid = ins.GetSolidOfElement(doc, beam);
                            var clonedsolid = SolidUtils.Clone(solid);
                            solids.Add(clonedsolid);
                            tr.Commit();
                        }
                    }
                }


            }
            catch (Exception)
            {

            }



        }
        public static void Insulatebyselection(UIDocument uidoc, Document doc)
        {
            Selection choices = uidoc.Selection;

            Transaction tr = new Transaction(doc, "ss");
            tr.Start();

            var references = choices.PickObjects(ObjectType.Element);
            foreach (var reference in references)
            {
                var ele = doc.GetElement(reference);
                Options opt1 = new Options();
                opt1.ComputeReferences = true;
                var geometryelement = ele.get_Geometry(opt1);
                foreach (var obj in geometryelement)
                {
                    var geometryinstance = obj as GeometryInstance;
                    if (null == geometryinstance)
                    {

                        var GeoSolid = obj as Solid;
                        if (GeoSolid != null && solid.Faces.Size != 0)
                        {
                            if (ele.Category.Id.ToString() == BuiltInCategory.OST_StructuralColumns.GetHashCode().ToString())
                            {
                                //CREATE LIST TO HAVE ONLLY TOPFACES OF COLUMN 
                                var topfaceofcolumn1 = GeoSolid.Faces.OfType<PlanarFace>().Where(i => i.FaceNormal.Z == 1).ToList();
                                if (topfaceofcolumn1 != null)
                                {
                                    topfaceofcolumn.AddRange(topfaceofcolumn1);
                                }
                            }
                            else if (ele.Category.Id.ToString() == BuiltInCategory.OST_Walls.GetHashCode().ToString())
                            {
                                var topfaceofcolumn1 = GeoSolid.Faces.OfType<PlanarFace>().Where(i => Math.Round(i.FaceNormal.Z) == 1).ToList();
                                if (topfaceofcolumn1 != null)
                                {
                                    topfaceofcolumn.AddRange(topfaceofcolumn1);
                                }
                            }
                            solids.Add(GeoSolid);
                        }
                    }

                    else
                    {
                        foreach (var instgeo in geometryinstance.GetInstanceGeometry())
                        {
                            var solid = instgeo as Solid;
                            if (solid != null && solid.Faces.Size != 0)
                            {
                                if (ele.Category.Id.ToString() == BuiltInCategory.OST_StructuralColumns.GetHashCode().ToString())
                                {
                                    //CREATE LIST TO HAVE ONLLY TOPFACES OF COLUMN 
                                    var topfaceofcolumn2 = solid.Faces.OfType<PlanarFace>().Where(i => Math.Round(i.FaceNormal.Z) == 1).ToList();
                                    if (topfaceofcolumn2 != null)
                                    {
                                        topfaceofcolumn.AddRange(topfaceofcolumn2);
                                    }
                                }
                                solids.Add(solid);
                            }
                        }
                    }
                }
                graphicstyleid = geometryelement.GraphicsStyleId;
            }
            tr.Commit();
        }
        public static FaceArray GetFaceArray()
        {
            List<PlanarFace> faceArray = new List<PlanarFace>();
            FaceArray faceArrayfinal = new FaceArray();
            Insulation ins = new Insulation();
            var solids = Insulation.solids;
            var x = ins.JoinSolids(solids);
            faceArray.AddRange(x.Faces.OfType<PlanarFace>().ToList());
            foreach (var face in Insulation.topfaceofcolumn)
            {
                try
                {
                    var ele = faceArray.Where(z => z.Area == face.Area && z.Origin.IsAlmostEqualTo(face.Origin)).FirstOrDefault();
                    faceArray.Remove(ele);
                    faceArray.RemoveAll(f => f.FaceNormal.Z == -1);
                }
                catch (Exception)
                {


                }
            }
            foreach (var face in faceArray)
            {
                faceArrayfinal.Append(face);
            }
            return faceArrayfinal;
        }

    }
}
