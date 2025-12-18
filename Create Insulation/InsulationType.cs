using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Insulation;
using Autodesk.Revit.UI.Selection;
using System.Windows;
using Autodesk.Revit.Attributes;

namespace Insulation
{
    internal class InsulationType : IExternalEventHandler
    {
        
        public void Execute(UIApplication app)
        {
            
            UIDocument uidoc = app.ActiveUIDocument;
            Insulation ins = new Insulation();
            List<PlanarFace> faceArray = new List<PlanarFace>();
            
            Document doc = uidoc.Document;

            Transaction tr = new Transaction(doc, "Creating Material");
            tr.Start();
            //ElementId materialID = Insulation.CreateNewMaterial(doc, 255, 0, 0, 0);   // choose or create material 
            var  materialID = Database.selectedmaterial;
            tr.Commit();
            var xname = Database.radioname;
            if (xname == "By selection")
            {
                Insulation.solids.Clear();
                Insulation.Insulatebyselection(uidoc, doc);
                var faceArrayfinal = Insulation.GetFaceArray();

                Transaction mtr = new Transaction(doc, "modeling transaction");
                mtr.Start();
                ins.ModelInsulation(doc, faceArrayfinal, Database.thickness, Database.selectedcategory, materialID, Database.booloffset);
                mtr.Commit();
            }
            else if (xname == "By Level")
            {
                Insulation.solids.Clear();
                Insulation.Insulatebylevel(uidoc, doc, Database.selectedlevel.Name);
                var faceArrayfinal =  Insulation.GetFaceArray();

                Transaction mtr = new Transaction(doc, "modeling transaction");
                mtr.Start();
                ins.ModelInsulation(doc, faceArrayfinal, Database.thickness, Database.selectedcategory, materialID, Database.booloffset);
                mtr.Commit();
            }
            else if (xname == "By Face")
            {
                var reference = uidoc.Selection.PickObject(ObjectType.Face);
                XYZ pos = reference.GlobalPoint;
                PlanarFace face = null;
                Options ops = new Options();
                ops.ComputeReferences = true;
                var element = doc.GetElement(reference);
                var solid = ins.GetSolidOfElement(doc, element);
                foreach (Face face2 in solid.Faces)
                {
                    try
                    {
                        if (face2.Project(pos).XYZPoint.DistanceTo(pos) == 0)
                        {
                            face = face2 as PlanarFace;
                        }
                    }
                    catch (NullReferenceException)
                    {

                    }
                }
                #region old code

                //foreach (var obj2 in element.get_Geometry(ops))
                //{
                //    var geo = obj2 as Solid;
                //    if (null != geo)
                //    {
                //        var solid = obj2 as Solid;
                //        foreach (Face face2 in solid.Faces)
                //        {
                //            try
                //            {
                //                if (face2.Project(pos).XYZPoint.DistanceTo(pos) == 0)
                //                {
                //                    face = face2 as PlanarFace;
                //                }
                //            }
                //            catch (NullReferenceException)
                //            {
                //            }
                //        }
                //    }
                //    else
                //    {
                //        var geometryinstance = obj2 as GeometryInstance;
                //        var geometryElement = geometryinstance.GetInstanceGeometry();
                //        foreach (var geoele in geometryElement)
                //        {
                //            var solid = geoele as Solid;
                //            foreach (Face face2 in solid.Faces)
                //            {
                //                try
                //                {
                //                    if (face2.Project(pos).XYZPoint.DistanceTo(pos) == 0)
                //                    {
                //                       face = face2 as PlanarFace;
                //                    }
                //                }
                //                catch (NullReferenceException)
                //                {

                //                }
                //            }
                //        }
                //    }
                #endregion
                Transaction mtr = new Transaction(doc, "modeling transaction");
                    mtr.Start();
                    Insulation.ModelInsulationOneFace(doc, face, Database.thickness, Database.selectedcategory,Database.booloffset);
                    mtr.Commit();
                

            }
            #region old code

            //var x = ins.JoinSolids(solids);
            //faceArray.AddRange(x.Faces.OfType<PlanarFace>().ToList());
            //foreach (var face in Insulation.topfaceofcolumn)
            //{
            //    try
            //    {
            //        var ele = faceArray.Where(z => z.Area == face.Area && z.Origin.IsAlmostEqualTo(face.Origin)).FirstOrDefault();
            //        faceArray.Remove(ele);
            //        faceArray.RemoveAll(f => f.FaceNormal.Z == -1);
            //    }
            //    catch (Exception)
            //    {


            //    }
            //}
            //foreach (var face in faceArray)
            //{
            //    faceArrayfinal.Append(face);
            //}
            #endregion
        }

        public string GetName()
        {
            return "ok";
        }
    }
}
