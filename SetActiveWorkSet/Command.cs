using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SetActiveWorkSet
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument activeUiDocument = commandData.Application.ActiveUIDocument;
            Document document = activeUiDocument.Document;
            Reference reference = null;

            try
            {
                reference = activeUiDocument.Selection.PickObject(ObjectType.Element);
            }
            catch
            {
                reference = null;
            }
            try
            {
                Transaction transaction = new Transaction(document, "Set Active Workset");
                if (reference != null)
                {
                    Parameter workset = document.GetElement(reference).LookupParameter("Workset");
                    transaction.Start();
                    Workset workset1 = ((IEnumerable<Workset>)new FilteredWorksetCollector(document)
                         .OfKind(WorksetKind.UserWorkset))
                         .FirstOrDefault<Workset>((Func<Workset, bool>)(x => ((WorksetPreview)x).Name == workset.AsValueString()));
                    if (workset1 != null)
                    {
                        document.GetWorksetTable().SetActiveWorksetId(((WorksetPreview)workset1).Id);
                    }
                    transaction.Commit();

                    MessageBox.Show("Completed!", "First Option", MessageBoxButton.OK, MessageBoxImage.Information);
                    return Result.Succeeded;
                }
                else
                {
                    if (!transaction.HasEnded())
                    {
                        transaction.RollBack(); 
                    }
                        return Result.Cancelled;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "First Option", MessageBoxButton.OK, MessageBoxImage.Error);
                return Result.Failed;
            }
        }


    }
}
