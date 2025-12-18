using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ModelHealthChecker.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModelHealthChecker.ExternalEventHandlers
{
    public class DeleteFamilyEventHandeler : IExternalEventHandler
    {

        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<Family> UnDeletedFamilies = new List<Family>();
            using (Transaction transaction = new Transaction(doc, "Create 3D View"))
            {
                transaction.Start();

                try
                {

                    ObservableCollection<RevitFamiy> selectedFamilies = Command.VM.SelectedFamilies;
                    List<ElementId> elements_Id = new List<ElementId>();

                    foreach (RevitFamiy R_family in selectedFamilies)
                    {
                        Family family = R_family.family;

                        List<Element> ElementsInFamily = GetAllElementsFromAFamily(doc, family);
                        if (ElementsInFamily.Count == 0)
                        {
                            doc.Delete(family.Id);
                            Command.VM.LoadedFamilies.Remove(R_family);
                        }
                        else
                        {
                            UnDeletedFamilies.Add(family);
                        }
                    }

                    if (UnDeletedFamilies.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("All Selected Families Have Been Deleted Except: ");
                        foreach (Family family in UnDeletedFamilies)
                        {
                            sb.AppendLine($"--{family.Name}.... Contains : {GetAllElementsFromAFamily(doc, family).Count} Elements");
                        }

                        MessageBox.Show(sb.ToString(), "First Option", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else

                    {
                        MessageBox.Show("All Selected Families Have Been Deleted", "First Option", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                }
                catch
                {

                }

                //Commit the transaction
                transaction.Commit();
            }
        }

        public string GetName()
        {
            return "A";
        }
        public List<Element> GetAllElementsFromAFamily(Autodesk.Revit.DB.Document doc, Family family)
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
    }

}
