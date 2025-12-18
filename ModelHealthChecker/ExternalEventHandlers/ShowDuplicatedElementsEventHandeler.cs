using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ModelHealthChecker.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModelHealthChecker.ExternalEventHandlers
{
    public class ShowDuplicatedElementsEventHandeler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            RevitDuplication duplication = Command.VM.SelectedDuplication;


            if (doc.ActiveView.ViewType == ViewType.ThreeD)
            {
                if (duplication != null)
                {
                    using (Transaction transaction = new Transaction(doc, "Create 3D View"))
                    {
                        transaction.Start();

                        try
                        {
                            doc.ActiveView.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
                            List<ElementId> selectedElementIDs = new List<ElementId> { duplication.ElementId_1, duplication.ElementId_2 };
                            doc.ActiveView.IsolateElementsTemporary(selectedElementIDs);
                            uidoc.ShowElements(selectedElementIDs);
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
                    MessageBox.Show("Please Select a Duplication to Show", "FirstOption", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
    }
}
