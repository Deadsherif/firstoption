using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloorSlicer21
{
    public class Helper
    {

        public static List<CompoundStructureLayer> GetFloorLayers(Document doc, Element floor)
        {
            if (floor is Floor f)
            {
                CompoundStructure compoundStructure = f.FloorType.GetCompoundStructure();
                return compoundStructure.GetLayers().ToList();
            }
            else if (floor is Ceiling c)
            {
                CeilingType Ctype = doc.GetElement(floor.GetTypeId()) as CeilingType;
                CompoundStructure compoundStructure = Ctype.GetCompoundStructure();
                return compoundStructure.GetLayers().ToList();
            }

            return null;

        }



        public static List<CurveArray> GetFloorCurveArray(Document doc, Element floor)
        {
            try
            {
                Floor f = floor as Floor;
                Options options = new Options();
                GeometryElement geometryElement = floor.get_Geometry(options);
                List<Face> faces2 = new List<Face>();
                List<CurveArray> Result = new List<CurveArray>();

                foreach (GeometryObject geomObject in geometryElement)
                {
                    if (geomObject is Solid solid)
                    {

                        FaceArray faces = solid.Faces;
                        foreach (Face face in faces)
                        {
                            faces2.Add(face);
                        }
                    }
                }

                Face TopFace = faces2.OrderByDescending(e => (e as PlanarFace)?.Origin.Z ?? -500000).FirstOrDefault();
                EdgeArrayArray EdgeArr = TopFace.EdgeLoops;

                foreach (EdgeArray EA in EdgeArr)
                {
                    CurveArray ca = new CurveArray();
                    foreach (Edge e in EA)
                    {
                        ca.Append(e.AsCurve());
                    }
                    Result.Add(ca);

                }


                return Result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static FloorType CreateNewFloorType(Document Doc, Element floor, List<CompoundStructureLayer> LayersToKeep, string Name)
        {
            FloorType oldFloorType = (floor as Floor).FloorType;
            FloorType NewType = default;

            using (Transaction trans = new Transaction(Doc, "CreatingWallType"))
            {
                try
                {
                    trans.Start();
                    NewType = oldFloorType.Duplicate(Name) as FloorType;
                    CompoundStructure compoundStructure = NewType.GetCompoundStructure();
                    IList<CompoundStructureLayer> NewLayers = compoundStructure.GetLayers();
                    int NumOfLayers = NewLayers.Count;
                    compoundStructure.SetLayers(LayersToKeep);
                    var NewLayerOfDuplicatedWall = compoundStructure.GetLayers();
                    compoundStructure.SetLayers(NewLayerOfDuplicatedWall);
                    NewType.SetCompoundStructure(compoundStructure);
                    trans.Commit();
                }
                catch (Exception)
                {

                    throw;
                }
            }

            return NewType;

        }

        public static double FloorThickness(Element floor, Document doc)
        {
            var layers = GetFloorLayers(doc, floor);
            double result = 0;
            foreach (var layer in layers)
            {
                result = result + layer.Width;

            }
            return result;
        }




    }
}
