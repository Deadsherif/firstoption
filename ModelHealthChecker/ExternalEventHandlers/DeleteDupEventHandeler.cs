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
    public class DeleteDupEventHandeler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;


            ObservableCollection<RevitDuplication> AllDuplications = Command.VM.Duplications;
            List<string> DeletedElements = new List<string>();
            List<RevitDuplication> DeletedDuplication = new List<RevitDuplication>();
            foreach (RevitDuplication duplication in AllDuplications)
            {
                if (duplication.IsSelected == true)
                {
                    ElementId e1 = duplication.ElementId_1;
                    ElementId e2 = duplication.ElementId_2;


                    using (Transaction transaction = new Transaction(doc, "Deleted Duplicated Element"))
                    {
                        transaction.Start();

                        try
                        {
                            string text = $"{doc.GetElement(e1).Category.Name}...[{e1.IntegerValue}]";
                            doc.Delete(e1);
                            DeletedElements.Add(text);
                            DeletedDuplication.Add(duplication);
                        }
                        catch
                        {

                        }

                        //Commit the transaction
                        transaction.Commit();
                    }
                }


            }
            foreach (var item in DeletedDuplication)
            {
                Command.VM.Duplications.Remove(item);
            }
            List<FailureMessage> warningSet = doc.GetWarnings().ToList();
            Command.VM.Warnings = new ObservableCollection<RevitWarning>();
            Command.VM.NumOfWarnings = Command.VM.Warnings.Count;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (string element in DeletedElements)
            {
                stringBuilder.AppendLine(element);
            }
            if (DeletedElements.Count == 0)
            {
                MessageBox.Show($"No Duplication Where selected , You have to selected through the checkbox next to the duplication", "First Option", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else
            {
                MessageBox.Show($"Number of Deleted Elements : {DeletedElements.Count}\n\n{stringBuilder.ToString()}", "First Option", MessageBoxButton.OK, MessageBoxImage.Information);

            }


        }

        public string GetName()
        {
            return "A";
        }
    }
}
