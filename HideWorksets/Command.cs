using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HideWorksets
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument activeUiDocument = commandData.Application.ActiveUIDocument;
                Document document = activeUiDocument.Document;
                Reference reference = activeUiDocument.Selection.PickObject(ObjectType.Element,"Select Element in the workset you want to hide!");
                WorksetId worksetId = document.GetElement(reference).WorksetId;

                using (Transaction transaction = new Transaction(document, "Hide Workset"))
                {
                    transaction.Start();
                    document.ActiveView.SetWorksetVisibility(worksetId, WorksetVisibility.Hidden);
                    transaction.Commit();
                }
                MessageBox.Show("Work Sets Hide Completed!", "First Option", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "First Option", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return Result.Succeeded;
        }
    }
}

