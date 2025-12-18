using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloorSlicer22
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

        public static List<CurveLoop> GetFloorCurveLoops(Document doc, Element floor)
        {
            try
            {
                if (floor is Floor)
                {
                    Sketch sketch = doc.GetElement((floor as Floor).SketchId) as Sketch;
                    List<CurveArray> curveArray = new List<CurveArray>();
                    foreach (CurveArray s in sketch.Profile)
                    {
                        curveArray.Add(s);

                    }


                    List<CurveLoop> Loops = new List<CurveLoop>();
                    foreach (CurveArray CA in curveArray)
                    {
                        CurveLoop curveloop = new CurveLoop();

                        foreach (Curve curve in CA)
                        {
                            curveloop.Append(curve);
                        }
                        Loops.Add(curveloop);


                    }

                    return Loops;
                }
                else if (floor is Ceiling)
                {
                    Sketch sketch = doc.GetElement((floor as Ceiling).SketchId) as Sketch;
                    List<CurveArray> curveArray = new List<CurveArray>();
                    foreach (CurveArray s in sketch.Profile)
                    {
                        curveArray.Add(s);

                    }
                    List<CurveLoop> Loops = new List<CurveLoop>();
                    foreach (CurveArray CA in curveArray)
                    {
                        CurveLoop curveloop = new CurveLoop();

                        foreach (Curve curve in CA)
                        {
                            curveloop.Append(curve);
                        }
                        Loops.Add(curveloop);


                    }

                    return Loops;
                }
                return null;
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
