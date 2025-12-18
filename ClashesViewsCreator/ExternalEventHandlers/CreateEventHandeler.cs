using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ClashesViewsCreator.MVVM.View;

namespace ClashesViewsCreator.ExternalEventHandlers
{
    public class CreateEventHandeler : IExternalEventHandler
    {
        public static UIDocument UiDoc;
        public static Document Doc;

        public void Execute(UIApplication app)
        {
            UiDoc = Command.UiDoc;
            Doc = Command.Doc;
            List<int> CreatedElements = new List<int>();



            if (Command.frm.VM.IsAll)
            {
                using (Transaction trans = new Transaction(Doc, "CreateView"))
                {
                    trans.Start();
                    for (int i = 0; i < Math.Min(Command.Elements_Id_1.Count, Command.Elements_Id_2.Count); i++)
                    {
                        try
                        {
                            if (!CreatedElements.Contains(Command.Elements_Id_1[i]) && !CreatedElements.Contains(Command.Elements_Id_2[i]))
                            {
                                Create3DViewContainsClashElement(UiDoc, new ElementId(Command.Elements_Id_1[i]), new ElementId(Command.Elements_Id_2[i]), $"Clash_View_{i + 1}");
                                CreatedElements.Add(Command.Elements_Id_1[i]);
                                CreatedElements.Add(Command.Elements_Id_2[i]);

                            }

                        }
                        catch
                        {
                            if (!CreatedElements.Contains(Command.Elements_Id_1[i]) && !CreatedElements.Contains(Command.Elements_Id_2[i]))
                            {
                                Random r = new Random();

                                Create3DViewContainsClashElement(UiDoc, new ElementId(Command.Elements_Id_1[i]), new ElementId(Command.Elements_Id_2[i]), $"Clash_View_{i + 1}_{r.Next(1, 9999)}");
                                CreatedElements.Add(Command.Elements_Id_1[i]);
                                CreatedElements.Add(Command.Elements_Id_2[i]);

                            }

                        }

                    }


                    trans.Commit();


                }
                MessageBox.Show("Done");

            }
            else
            {
                FilterView vm = new FilterView();
                vm.Show();

            }



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


                Category MassCat = Category.GetCategory(doc, BuiltInCategory.OST_Mass);
                new3DView.SetCategoryHidden(MassCat.Id, false);

                Settings documentSettings = doc.Settings;
                Categories groups = documentSettings.Categories;

                Category ScopeBoxCategory = null;



                foreach (Category category in groups)
                {
                    if (category.Name == "Scope Boxes")
                    {
                        ScopeBoxCategory = category;
                    }
                }

                if (ScopeBoxCategory != null)
                {

                    Category SectionBoxes = Category.GetCategory(doc, ScopeBoxCategory.Id);
                    new3DView.SetCategoryHidden(SectionBoxes.Id, true);
                }



            }
            catch
            {
                throw;
            }

        }





        private BoundingBoxXYZ GetLargestBoundingBoxAroundElements(Document doc, List<ElementId> elementIds)
        {
            BoundingBoxXYZ boundingBox = new BoundingBoxXYZ();

            double temp = 0;
            double Distance = 0;
            BoundingBoxXYZ Result = new BoundingBoxXYZ();



            foreach (ElementId id in elementIds)
            {
                Element ele = doc.GetElement(id);
                XYZ p1 = new XYZ(0, 0, 0);
                XYZ p2 = ele.get_BoundingBox(null).Max;

                temp = GetDistanceBetweenTwoPoints(p1, p2);

                if (temp > Distance)
                {
                    Distance = temp;
                    Result = ele.get_BoundingBox(null);
                }

            }


            return Result;
        }

        public static double GetDistanceBetweenTwoPoints(XYZ point1, XYZ point2)
        {
            double deltaX = point2.X - point1.X;
            double deltaY = point2.Y - point1.Y;
            double deltaZ = point2.Z - point1.Z;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

            return distance;
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


        private void CreateSphereInClasedZone(Document doc, ElementId Id1, ElementId Id2, string SphereTypeName)
        {
            Element e1 = doc.GetElement(Id1);
            Element e2 = doc.GetElement(Id2);

            Solid S1 = GetElementSolid(e1);
            Solid S2 = GetElementSolid(e2);

            Solid S = IntersectionSolid(S1, S2);

            XYZ Pt = S.ComputeCentroid();

            List<Element> massTypes = new FilteredElementCollector(doc)
               .OfCategory(BuiltInCategory.OST_Mass)
               .WhereElementIsElementType()
               .ToElements().ToList();


            Element massType = massTypes
           .FirstOrDefault(element => element.Name == SphereTypeName);


            if (massType != null)
            {

                using (Transaction trans = new Transaction(doc, "Test"))
                {
                    trans.Start();
                    FamilySymbol fs = massType as FamilySymbol;
                    fs.Activate();
                    Element Sphere = doc.Create.NewFamilyInstance(Pt, fs, StructuralType.NonStructural);

                    Sphere.LookupParameter("Radius").Set(1000 * 0.00328084);

                    trans.Commit();
                }
            }
            else
            {
                MessageBox.Show("Load the sphere family first (SphereMass)");
            }
        }

        private static Solid IntersectionSolid(Solid solidA, Solid solidB)
        {


            Solid intersection = BooleanOperationsUtils.ExecuteBooleanOperation(solidA, solidB, BooleanOperationsType.Intersect);
            if (intersection != null)
            {
                return intersection;
            }
            else
            { return null; }


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

        public List<Category> GetAllCategories(Document document)
        {
            // Get all categories in the project
            var ss = new FilteredElementCollector(document)
                  .OfClass(typeof(Category)).ToList();

            List<Category> result = new List<Category>();

            foreach (var category in ss)
            {

                result.Add(category.Category);
            }
            return result;

        }
    }
}
