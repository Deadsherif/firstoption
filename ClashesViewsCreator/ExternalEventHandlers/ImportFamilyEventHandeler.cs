using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClashesViewsCreator.ExternalEventHandlers
{
    public class ImportFamilyEventHandeler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            Document doc = Command.Doc;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Revit Family Files (*.rfa)|*.rfa|All Files (*.*)|*.*";
            openFileDialog.Title = "Select a Revit Family File";

            if (openFileDialog.ShowDialog() == true)
            {
                // Get the selected file path
                string familyFilePath = openFileDialog.FileName;

                // Start a transaction to load the family
                using (Transaction transaction = new Transaction(doc, "Load Family"))
                {
                    transaction.Start();

                    // Load the family into the project
                    Family family = null;
                    if (doc.LoadFamily(familyFilePath, out family))
                    {
                        List<Element> massTypes = new FilteredElementCollector(Command.Doc)
                                                     .OfCategory(BuiltInCategory.OST_Mass)
                                                     .WhereElementIsElementType()
                                                     .ToElements().ToList();

                        List<Element> GerenricTypes = new FilteredElementCollector(Command.Doc)
                                                        .OfCategory(BuiltInCategory.OST_GenericModel)
                                                        .WhereElementIsElementType()
                                                        .ToElements().ToList();

                        Command.frm.VM.Types = new ObservableCollection<Element>(massTypes);
                        foreach (Element element in GerenricTypes)
                        {
                            Command.frm.VM.Types.Add(element);
                        }

                        foreach (var e in Command.frm.VM.Types)
                        {
                            if ((e as FamilySymbol).FamilyName == family.Name)
                            {
                                Command.frm.VM.SelectedType = e;
                                break;
                            }
                        }

                        MessageBox.Show("Family Loaded!", "First Option", MessageBoxButton.OK, MessageBoxImage.Information);

                        transaction.Commit();
                    }
                    else
                    {
                        // Handle the case where the family fails to load
                        MessageBox.Show("Failed to load family.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        transaction.RollBack();
                    }
                }
            }


        }

        public string GetName()
        {
            return "Hello";
        }
    }
}
