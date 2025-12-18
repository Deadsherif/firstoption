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
    public class CreateFilterEventHandeler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            Document doc = Command.Doc;
            UIDocument uiDoc = Command.UiDoc;
            using (Transaction trans = new Transaction(doc, "CreateView"))
            {
                trans.Start();
                for (int i = 0; i < Command.frm.VM.SelectedIds.Count; i++)
                {
                    try
                    {
                        Create3DViewContainsClashElement(uiDoc, new ElementId(Command.frm.VM.SelectedIds[i].Number), $"Clash_View_{i + 1}");

                    }
                    catch
                    {
                        Random r = new Random();
                        Create3DViewContainsClashElement(uiDoc, new ElementId(Command.frm.VM.SelectedIds[i].Number), $"Clash_View_{i + 1}_{r.Next(1, 9999)}");
                    }

                }


                trans.Commit();
            }

            MessageBox.Show("Done");
        }

        public string GetName()
        {
            return "Mostafa";
        }
        public void Create3DViewContainsClashElement(UIDocument UIdoc, ElementId v01, string name)
        {
            try
            {
                Document doc = UIdoc.Document;
                List<ElementId> elements = new List<ElementId>();
                elements.Add(v01);

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



    }
}
