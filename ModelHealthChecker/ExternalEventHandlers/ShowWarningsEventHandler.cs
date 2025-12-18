using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModelHealthChecker.ExternalEventHandlers
{
    public class ShowWarningsEventHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<ElementId> selectedWarnings = Command.VM.SeletedWarning?.FailureElements;

            if (doc.ActiveView.ViewType == ViewType.ThreeD)
            {
                if (selectedWarnings != null)
                {
                    using (Transaction transaction = new Transaction(doc, "Create 3D View"))
                    {
                        transaction.Start();

                        try
                        {

                            doc.ActiveView.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);

                            doc.ActiveView.IsolateElementsTemporary(selectedWarnings);
                            uidoc.ShowElements(selectedWarnings);

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
                    MessageBox.Show("Please Select a Warning to Show", "FirstOption", MessageBoxButton.OK, MessageBoxImage.Exclamation);

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
