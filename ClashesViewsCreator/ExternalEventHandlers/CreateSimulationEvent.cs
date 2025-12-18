using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClashesViewsCreator.ExternalEventHandlers
{
    public class CreateSimulationEvent : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument UiDoc = Command.UiDoc;
            Document Doc = Command.Doc;
            List<int> CreatedElements = new List<int>();


            Transaction tr = new Transaction(Doc, "Create Spheres");
            int count = 0;
            tr.Start();
            for (int i = 0; i < Math.Min(Command.Elements_Id_1?.Count ?? 0, Command.Elements_Id_2?.Count ?? 0); i++)
            {
                try
                {
                    CreateSphereInClasedZone(Doc, new ElementId(Command.Elements_Id_1[i]), new ElementId(Command.Elements_Id_2[i]), Command.frm.VM.SelectedType);
                    count++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    break;

                }
            }


            FailureHandlingOptions options = tr.GetFailureHandlingOptions();
            options.SetFailuresPreprocessor(new WarningSwallower());
            //tr.SetFailureHandlingOptions(options);


            tr.Commit();
            MessageBox.Show($"Created {count} Elements");

        }

        public string GetName()
        {
            return "Mostafa";
        }
        public void Create3DViewContainsClashElement(UIDocument UIdoc, ElementId v01, ElementId v02, string name)
        {
            try
            {
                Document doc = UIdoc.Document;
                List<ElementId> elements = new List<ElementId>();
                elements.Add(v01);
                elements.Add(v02);

                var viewFamilyTypes = new FilteredElementCollector(doc).
                                      OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().
                                      Where(f => f.ViewFamily == ViewFamily.ThreeDimensional).FirstOrDefault();
                View3D new3DView = View3D.CreateIsometric(doc, viewFamilyTypes.Id);
                new3DView.Name = name;

                BoundingBoxXYZ bb = GetBoundingBoxAroundElements(doc, elements);
                new3DView.SetSectionBox(bb);
                new3DView.DisplayStyle = DisplayStyle.Shading;
                new3DView.DetailLevel = ViewDetailLevel.Fine;


            }
            catch
            {
                throw;
            }

        }
        public static BoundingBoxXYZ GetBoundingBoxAroundElements(Document doc, ICollection<ElementId> elementIds)
        {
            if (elementIds == null || elementIds.Count == 0)
            {
                throw new ArgumentException("The list of ElementIds is null or empty.");
            }

            BoundingBoxXYZ boundingBox = null;

            foreach (ElementId elementId in elementIds)
            {
                Element element = doc.GetElement(elementId);
                BoundingBoxXYZ elementBoundingBox = element.get_BoundingBox(null);

                if (boundingBox == null)
                {
                    // Initialize the bounding box with the first element
                    boundingBox = new BoundingBoxXYZ();
                    boundingBox.Min = elementBoundingBox.Min;
                    boundingBox.Max = elementBoundingBox.Max;
                }
                else
                {
                    // Update the bounding box to include the current element
                    boundingBox.Min = new XYZ(Math.Min(boundingBox.Min.X, elementBoundingBox.Min.X),
                                               Math.Min(boundingBox.Min.Y, elementBoundingBox.Min.Y),
                                               Math.Min(boundingBox.Min.Z, elementBoundingBox.Min.Z));

                    boundingBox.Max = new XYZ(Math.Max(boundingBox.Max.X, elementBoundingBox.Max.X),
                                               Math.Max(boundingBox.Max.Y, elementBoundingBox.Max.Y),
                                               Math.Max(boundingBox.Max.Z, elementBoundingBox.Max.Z));
                }
            }

            return boundingBox;
        }
        private void CreateSphereInClasedZone(Document doc, ElementId Id1, ElementId Id2, Element FamilySymbolOfMass)
        {
            Element e1 = doc.GetElement(Id1);
            Element e2 = doc.GetElement(Id2);

            Solid S1 = GetElementSolid(e1);
            Solid S2 = GetElementSolid(e2);

            Solid S = IntersectionSolid(S1, S2);

            XYZ Pt = new XYZ();

            if (S != null)
            {
                if (S.Faces.Size > 0)
                {
                    try
                    {
                        Pt = S.ComputeCentroid();
                    }
                    catch
                    {
                        Pt = CalculateSolidCenter(S);
                    }
                    if (Pt != null)
                    {
                        try
                        {

                            FamilySymbol fs = FamilySymbolOfMass as FamilySymbol;
                            fs.Activate();
                            Element Sphere = doc.Create.NewFamilyInstance(Pt, fs, StructuralType.NonStructural);

                        }
                        catch
                        {
                            throw;
                        }
                    }

                }
            }
        }

        private static Solid IntersectionSolid(Solid solidA, Solid solidB)
        {

            try
            {

                Solid intersection = BooleanOperationsUtils.ExecuteBooleanOperation(solidA, solidB, BooleanOperationsType.Intersect);
                if (intersection != null)
                {
                    return intersection;
                }
                else
                { return null; }
            }
            catch
            {

                return null;
            }


        }

        private static Solid GetElementSolid(Element element)
        {


            Solid solid = default;
            Options OP = new Options();
            OP.DetailLevel = ViewDetailLevel.Fine;
            OP.ComputeReferences = true;
            try
            {
                GeometryElement columnGeometry = element.get_Geometry(OP);
                foreach (GeometryObject geomObj in columnGeometry)
                {
                    GeometryInstance GeoIns = geomObj as GeometryInstance;

                    var GeoInstance = GeoIns.GetInstanceGeometry();
                    foreach (var item in GeoInstance)
                    {
                        solid = item as Solid;
                        if (solid != null && solid.Volume != 0)
                        {
                            return item as Solid;
                        }
                    }
                }

                return null;
            }
            catch
            {
                try
                {
                    GeometryElement columnGeometry = element.get_Geometry(OP);
                    foreach (var GeoEle in columnGeometry)
                    {
                        solid = GeoEle as Solid;
                        if (solid != null && solid.Volume != 0)
                        {
                            return solid;
                        }
                    }
                }
                catch
                {

                }

                return null;

            }

        }

        public XYZ CalculateSolidCenter(Solid solid)
        {
            // Check if the solid is not null
            if (solid != null)
            {
                // Get all the vertices of the solid
                List<XYZ> vertices = new List<XYZ>();

                foreach (Face face in solid.Faces)
                {
                    // Get the vertices of each face
                    foreach (XYZ vertex in face.Triangulate().Vertices)
                    {
                        vertices.Add(vertex);
                    }
                }

                // Calculate the average of all vertices to find the center point
                if (vertices.Count > 0)
                {
                    double averageX = vertices.Average(v => v.X);
                    double averageY = vertices.Average(v => v.Y);
                    double averageZ = vertices.Average(v => v.Z);

                    return new XYZ(averageX, averageY, averageZ);
                }
                else
                {
                    // Handle the case where there are no vertices
                    return null;
                }
            }
            else
            {
                // Handle the case where the solid is null
                return null;
            }
        }
    }

    public class WarningSwallower : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        { // Iterate through the failure messages
            IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();
            foreach (FailureMessageAccessor failureMessage in failureMessages)
            {
                // Check if the failure message has warning severity
                if (failureMessage.GetSeverity() == FailureSeverity.Warning)
                {
                    // Resolve the warning automatically
                    failuresAccessor.DeleteWarning(failureMessage);
                }
            }

            return FailureProcessingResult.ProceedWithCommit;
        }
    }
}
