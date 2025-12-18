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
    public class ShowFamilyElementsEventHandeler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            if (doc.ActiveView.ViewType == ViewType.ThreeD)
            {
                using (Transaction transaction = new Transaction(doc, "Create 3D View"))
                {
                    transaction.Start();

                    try
                    {
                        doc.ActiveView.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
                        ObservableCollection<RevitFamiy> selectedFamilies = Command.VM.SelectedFamilies;
                        List<ElementId> elements_Id = new List<ElementId>();

                        foreach (RevitFamiy R_family in selectedFamilies)
                        {
                            Family family = R_family.family;
                            List<ElementId> Temp = GetAllElements_IdFromAFamily(doc, family);
                            foreach (ElementId ElementId in Temp)
                            {
                                elements_Id.Add(ElementId);
                            }
                        }
                        if (elements_Id.Count > 0)
                        {
                            doc.ActiveView.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
                            doc.ActiveView.IsolateElementsTemporary(elements_Id);
                            uidoc.ShowElements(elements_Id);
                        }
                        else
                        {
                            MessageBox.Show("No elements found in the families you selected", "First Option", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
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

        public string GetName()
        {
            return "A";
        }

        public List<ElementId> GetAllElements_IdFromAFamily(Autodesk.Revit.DB.Document doc, Family family)
        {
            string FamilyName = family.Name;


            var Elements = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().
                OfType<FamilyInstance>().Where(x => x.Symbol.Family.Name == FamilyName);


            List<ElementId> elements = new List<ElementId>();
            foreach (Element element in Elements)
            {
                elements.Add(element.Id);
            }

            return elements;
        }


    }
}
