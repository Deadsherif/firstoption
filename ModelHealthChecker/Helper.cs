using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModelHealthChecker
{
    public class Helper
    {
        public static void IsolateElementsIn3DView(UIDocument uidoc, Element element)
        {

            Document doc = uidoc.Document;

            if (doc.ActiveView.ViewType == ViewType.ThreeD)
            {

                using (Transaction transaction = new Transaction(doc, "Create 3D View"))
                {
                    transaction.Start();

                    try
                    {
                        doc.ActiveView.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
                        List<ElementId> selectedElementIDs = new List<ElementId> { element.Id };
                        doc.ActiveView.IsolateElementsTemporary(selectedElementIDs);
                        //uidoc.ShowElements(selectedElementIDs);
                    }
                    catch
                    {

                    }

                    //Commit the transaction
                    transaction.Commit();

                }

            }
            else
            {
                MessageBox.Show("Please Open a 3D View", "FirstOption", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        public static List<Element> GetAllElementsFromAFamily(Autodesk.Revit.DB.Document doc, Family family)
        {
            string FamilyName = family.Name;


            var Elements = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().
                OfType<FamilyInstance>().Where(x => x.Symbol.Family.Name == FamilyName);


            List<Element> elements = new List<Element>();
            foreach (Element element in Elements)
            {
                elements.Add(element);
            }

            return elements;
        }


        public static List<Element> FindDuplicatesByName(Autodesk.Revit.DB.Document Doc)
        {
            // Collect all non-element type elements in the model
            FilteredElementCollector collector = new FilteredElementCollector(Doc);
            ICollection<Element> allElements = collector.WhereElementIsNotElementType().ToElements();

            // Find and return duplicates based on element names
            return FindDuplicatesByName(allElements);
        }

        public static List<Element> FindDuplicatesByName(ICollection<Element> elements)
        {
            Dictionary<string, List<Element>> duplicateElements = new Dictionary<string, List<Element>>();
            List<Element> duplicatedElements = new List<Element>();

            foreach (Element element in elements)
            {
                // Extract name for comparison (customize this based on your criteria)
                string name = element.Name;

                // Check if the name is already in the dictionary
                if (duplicateElements.ContainsKey(name))
                {
                    // Add the current element to the list of duplicates
                    duplicateElements[name].Add(element);
                    duplicatedElements.Add(element);
                }
                else
                {
                    // Create a new list for the name and add the current element
                    duplicateElements[name] = new List<Element> { element };
                }
            }

            return duplicatedElements;
        }
    }
}
