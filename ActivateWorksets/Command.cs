using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ActivateWorksets
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Document document = commandData.Application.ActiveUIDocument.Document;
                using (Transaction transaction = new Transaction(document, "Activate Workset"))
                {
                    var workSets = (IEnumerable<Workset>)new FilteredWorksetCollector(document).OfKind((WorksetKind)4).ToWorksets();
                    transaction.Start();
                    foreach (Workset workset in workSets)
                    {
                        document.ActiveView.SetWorksetVisibility((workset as WorksetPreview).Id, WorksetVisibility.Visible);
                    }
                    transaction.Commit();
                }
                MessageBox.Show("Work Sets Activation Completed!", "First Option", MessageBoxButton.OK, MessageBoxImage.Information);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "First Option", MessageBoxButton.OK, MessageBoxImage.Error);
                return Result.Failed;

            }
        }
    }
}
